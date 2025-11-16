using System.Globalization;
using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Core.Entities;
using Microsoft.Data.SqlClient;

namespace RevaliInstruct.Core.Data
{
    public static class DbInitializer
    {
        public static async Task ResetAndSeedAsync(ApplicationDbContext context)
        {
            // Verwijder eerst de database volledig zodat het schema opnieuw wordt opgebouwd.
            // Dit wordt alleen aangeroepen wanneer je expliciet RESET_DB=1 zet of via dev endpoint.
            try
            {
                await context.Database.EnsureDeletedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: failed to EnsureDeleted: {ex.Message}");
            }

            // Probeer migrations uit te voeren; als dat faalt door permissies, fallback naar EnsureCreated
            try
            {
                await context.Database.MigrateAsync();
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 262 || ex.Message.Contains("permission", StringComparison.OrdinalIgnoreCase))
            {
                // Permission denied bij CREATE TABLE — log en fallback naar EnsureCreated
                Console.WriteLine($"Migration failed due to permission issue, falling back to EnsureCreated: {ex.Message}");
                await context.Database.EnsureCreatedAsync();
            }
            catch (Exception ex)
            {
                // Algemeen fallback zodat seeding kan doorgaan (development scenario)
                Console.WriteLine($"Migration failed, falling back to EnsureCreated: {ex.Message}");
                await context.Database.EnsureCreatedAsync();
            }

            await SeedAsync(context);
        }


        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            // Seed users (alleen als leeg)
            if (!await context.Users.AnyAsync())
            {
                var doctor = new User
                {
                    Username = "doctor",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("doctor123"),
                    Role = "Doctor",
                    FullName = "Dr. Revali",
                    Email = "doctor@example.com"
                };
                var admin = new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "Admin",
                    FullName = "Admin",
                    Email = "admin@example.com"
                };

                try
                {
                    // Probeer normale EF-seed
                    await context.Users.AddRangeAsync(doctor, admin);
                    await context.SaveChangesAsync();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
                {
                    // Fallback: mogelijk andere kolomnamen in bestaande DB (bv. DisplayName i.p.v. FullName, geen Email)
                    var msg = dbEx.InnerException?.Message ?? dbEx.Message;
                    if (msg.Contains("Invalid column name", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("EF seed failed due to schema mismatch; falling back to raw SQL inserts for Users.");

                        var sql = @"
IF NOT EXISTS(SELECT 1 FROM dbo.Users WHERE Username = @u1)
BEGIN
    INSERT INTO dbo.Users ([Username],[PasswordHash],[Role],[DisplayName],[CreatedAt])
    VALUES (@u1,@p1,@r1,@display1, SYSUTCDATETIME());
END;
IF NOT EXISTS(SELECT 1 FROM dbo.Users WHERE Username = @u2)
BEGIN
    INSERT INTO dbo.Users ([Username],[PasswordHash],[Role],[DisplayName],[CreatedAt])
    VALUES (@u2,@p2,@r2,@display2, SYSUTCDATETIME());
END;
";
                        var p = new[]
                        {
                            new Microsoft.Data.SqlClient.SqlParameter("@u1", doctor.Username),
                            new Microsoft.Data.SqlClient.SqlParameter("@p1", doctor.PasswordHash),
                            new Microsoft.Data.SqlClient.SqlParameter("@r1", doctor.Role),
                            new Microsoft.Data.SqlClient.SqlParameter("@display1", doctor.FullName ?? (object)DBNull.Value),

                            new Microsoft.Data.SqlClient.SqlParameter("@u2", admin.Username),
                            new Microsoft.Data.SqlClient.SqlParameter("@p2", admin.PasswordHash),
                            new Microsoft.Data.SqlClient.SqlParameter("@r2", admin.Role),
                            new Microsoft.Data.SqlClient.SqlParameter("@display2", admin.FullName ?? (object)DBNull.Value),
                        };

                        try
                        {
                            await context.Database.ExecuteSqlRawAsync(sql, p);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Fallback SQL seed also failed: {ex.Message}");
                            throw;
                        }
                    }
                    else
                    {
                        // andere DbUpdateException: opnieuw gooien
                        throw;
                    }
                }
            }

            // Bepaal arts-id (doctor role indien aanwezig, anders eerste user)
            var doctorId = await context.Users.Where(u => u.Role == "Doctor").Select(u => u.Id).FirstOrDefaultAsync();
            if (doctorId == 0)
                doctorId = await context.Users.Select(u => u.Id).FirstOrDefaultAsync();

            // Helper: use the class-level TableExistsAsync implemented below
            // (local helper removed to avoid duplicate/partial definitions)

            // Seed oefeningen (bibliotheek) — alleen als tabel bestaat
            if (await TableExistsAsync(context, "Exercises"))
            {
                if (!await context.Exercises.AnyAsync())
                {
                    await context.Exercises.AddRangeAsync(
                        new Exercise { Code = "EX-KNEE-EXT", Title = "Knie extensie", Description = "Versterk quadriceps." },
                        new Exercise { Code = "EX-HIP-AB", Title = "Heup abductie", Description = "Versterk heupspieren." }
                    );
                    await context.SaveChangesAsync();
                }
            }
            else
            {
                Console.WriteLine("Skipping Exercises seed — table dbo.Exercises does not exist.");
            }

            // Seed patiënten (alleen als tabel bestaat) - robuuste fallback bij schema mismatch
            if (await TableExistsAsync(context, "Patients"))
            {
                // gebruik raw-sql om te controleren of de tabel leeg is (vermijdt EF-model mapping)
                var patientsCount = await GetTableRowCountAsync(context, "Patients");
                if (patientsCount == 0)
                {
                    var p1 = new Patient
                    {
                        FirstName = "Jan",
                        LastName = "Jansen",
                        DateOfBirth = new DateTime(1980, 5, 15),
                        Diagnosis = "Kruisbandblessure",
                        Status = PatientStatus.IntakePlanned,
                        AssignedDoctorUserId = doctorId
                    };
                    var p2 = new Patient
                    {
                        FirstName = "Maria",
                        LastName = "Bakker",
                        DateOfBirth = new DateTime(1965, 8, 22),
                        Diagnosis = "Heupoperatie",
                        Status = PatientStatus.Active,
                        AssignedDoctorUserId = doctorId
                    };

                    // bepaal welke kolommen bestaan in dbo.Patients
                    var existingCols = await GetTableColumnsAsync(context, "Patients");

                    // Als tabel alleen de minimale kolommen heeft zoals in je screenshot:
                    // Id, FirstName, LastName, DateOfBirth, Notes, CreatedAt
                    var minimalCols = new[] { "FirstName", "LastName", "DateOfBirth", "Notes", "CreatedAt" };
                    if (minimalCols.All(c => existingCols.Contains(c, StringComparer.OrdinalIgnoreCase)))
                    {
                        // Gebruik eenvoudige, expliciete fallback-insert voor precies die kolommen
                        var sql = @"
IF NOT EXISTS(SELECT 1 FROM dbo.Patients WHERE FirstName = @fn0 AND LastName = @ln0 AND DateOfBirth = @dob0)
BEGIN
    INSERT INTO dbo.Patients (FirstName, LastName, DateOfBirth, Notes, CreatedAt)
    VALUES (@fn0, @ln0, @dob0, @notes0, SYSUTCDATETIME());
END;
IF NOT EXISTS(SELECT 1 FROM dbo.Patients WHERE FirstName = @fn1 AND LastName = @ln1 AND DateOfBirth = @dob1)
BEGIN
    INSERT INTO dbo.Patients (FirstName, LastName, DateOfBirth, Notes, CreatedAt)
    VALUES (@fn1, @ln1, @dob1, @notes1, SYSUTCDATETIME());
END;
";
                        var parameters = new[]
                        {
                            new Microsoft.Data.SqlClient.SqlParameter("@fn0", p1.FirstName ?? (object)DBNull.Value),
                            new Microsoft.Data.SqlClient.SqlParameter("@ln0", p1.LastName ?? (object)DBNull.Value),
                            new Microsoft.Data.SqlClient.SqlParameter("@dob0", p1.DateOfBirth),
                            new Microsoft.Data.SqlClient.SqlParameter("@notes0", DBNull.Value),

                            new Microsoft.Data.SqlClient.SqlParameter("@fn1", p2.FirstName ?? (object)DBNull.Value),
                            new Microsoft.Data.SqlClient.SqlParameter("@ln1", p2.LastName ?? (object)DBNull.Value),
                            new Microsoft.Data.SqlClient.SqlParameter("@dob1", p2.DateOfBirth),
                            new Microsoft.Data.SqlClient.SqlParameter("@notes1", DBNull.Value)
                        };

                        await context.Database.ExecuteSqlRawAsync(sql, parameters);
                    }
                    else
                    {
                        // bestaande logica: probeer EF seed als alle EF-kolommen aanwezig zijn,
                        // anders bouw dynamische fallback (ongewijzigd)
                        var efRequired = new[] { "AssignedDoctorUserId", "DateOfBirth", "Diagnosis", "FirstName", "LastName", "Notes", "StartDate", "Status" };

                        if (efRequired.All(c => existingCols.Contains(c, StringComparer.OrdinalIgnoreCase)))
                        {
                            // veilig om EF te gebruiken
                            await context.Patients.AddRangeAsync(p1, p2);
                            await context.SaveChangesAsync();
                        }
                        else
                        {
                            // dynamische fallback: kies intersectie en insert (oude code)
                            var rows = new[]
                            {
                                new { FirstName = p1.FirstName, LastName = p1.LastName, DateOfBirth = (object?)p1.DateOfBirth, Diagnosis = p1.Diagnosis, Status = p1.Status.ToString(), AssignedDoctorUserId = (object?)p1.AssignedDoctorUserId, Notes = (object?)null, StartDate = (object?)p1.StartDate },
                                new { FirstName = p2.FirstName, LastName = p2.LastName, DateOfBirth = (object?)p2.DateOfBirth, Diagnosis = p2.Diagnosis, Status = p2.Status.ToString(), AssignedDoctorUserId = (object?)p2.AssignedDoctorUserId, Notes = (object?)null, StartDate = (object?)p2.StartDate }
                            };

                            var wantCols = new[] { "FirstName", "LastName", "DateOfBirth", "Diagnosis", "Status", "AssignedDoctorUserId", "Notes", "StartDate" };
                            var insertCols = wantCols.Where(c => existingCols.Contains(c, StringComparer.OrdinalIgnoreCase)).ToArray();

                            if (insertCols.Length == 0)
                            {
                                Console.WriteLine("No compatible Patients columns found for fallback insert; skipping Patients seed.");
                            }
                            else
                            {
                                var sb = new System.Text.StringBuilder();
                                var parameters = new List<Microsoft.Data.SqlClient.SqlParameter>();
                                for (int i = 0; i < rows.Length; i++)
                                {
                                    var row = rows[i];
                                    var paramNames = new List<string>();
                                    var whereParts = new List<string>();

                                    foreach (var col in insertCols)
                                    {
                                        var pname = $"@{col}_{i}";
                                        paramNames.Add(pname);
                                        object val = col switch
                                        {
                                            "FirstName" => row.FirstName ?? (object)DBNull.Value,
                                            "LastName" => row.LastName ?? (object)DBNull.Value,
                                            "DateOfBirth" => row.DateOfBirth ?? (object)DBNull.Value,
                                            "Diagnosis" => row.Diagnosis ?? (object)DBNull.Value,
                                            "Status" => row.Status ?? (object)DBNull.Value,
                                            "AssignedDoctorUserId" => row.AssignedDoctorUserId ?? (object)DBNull.Value,
                                            "Notes" => row.Notes ?? (object)DBNull.Value,
                                            "StartDate" => row.StartDate ?? (object)DBNull.Value,
                                            _ => (object)DBNull.Value
                                        };
                                        parameters.Add(new Microsoft.Data.SqlClient.SqlParameter(pname, val ?? DBNull.Value));

                                        if (string.Equals(col, "FirstName", StringComparison.OrdinalIgnoreCase) ||
                                            string.Equals(col, "LastName", StringComparison.OrdinalIgnoreCase) ||
                                            string.Equals(col, "DateOfBirth", StringComparison.OrdinalIgnoreCase))
                                        {
                                            whereParts.Add($"{col} = {pname}");
                                        }
                                    }

                                    string whereClauseSql = "";
                                    if (whereParts.Count > 0)
                                    {
                                        whereClauseSql = " WHERE " + string.Join(" AND ", whereParts);
                                    }

                                    sb.AppendLine($@"IF NOT EXISTS(SELECT 1 FROM dbo.Patients{whereClauseSql})
BEGIN
    INSERT INTO dbo.Patients ({string.Join(", ", insertCols)})
    VALUES ({string.Join(", ", paramNames)});
END;");
                                }

                                await context.Database.ExecuteSqlRawAsync(sb.ToString(), parameters.ToArray());
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Patients table exists and has {patientsCount} rows; skipping patient seed.");
                }
            }
            else
            {
                Console.WriteLine("Skipping Patients seed — table dbo.Patients does not exist.");
            }

            // Optioneel: voorbeeld oefentoewijzing als er nog geen assignments zijn
            if (await TableExistsAsync(context, "ExerciseAssignments"))
            {
                // gebruik raw SQL om een voorbeeld patient/exercise id te vinden (vermijdt EF mapping)
                var anyPatientId = await GetAnyIdAsync(context, "Patients");
                var anyExerciseId = await GetAnyIdAsync(context, "Exercises");
                if (anyPatientId != 0 && anyExerciseId != 0)
                {
                    await context.ExerciseAssignments.AddAsync(new ExerciseAssignment
                    {
                        PatientId = anyPatientId,
                        ExerciseId = anyExerciseId,
                        Repetitions = 10,
                        Sets = 3,
                        Frequency = "3x/week",
                        Duration = TimeSpan.FromMinutes(10),
                        StartDateUtc = DateTime.UtcNow.Date,
                        AssignedByUserId = doctorId
                    });
                    await context.SaveChangesAsync();
                }
            }
            else
            {
                Console.WriteLine("Skipping ExerciseAssignments seed — table dbo.ExerciseAssignments does not exist.");
            }

            // Seed accessoire adviezen (alleen toevoegen als tabel leeg is)
            if (await TableExistsAsync(context, "AccessoryAdvices"))
            {
                if (!await IsTableEmptyAsync(context, "AccessoryAdvices"))
                {
                    Console.WriteLine("AccessoryAdvices not empty; skipping accessory seed.");
                }
                else
                {
                    var rows = new (int PatientId, int GPUserId, string Name, DateTime AdviceDate, string Period, string Status)[]
                    {
                        (1, 3, "Krukken", new DateTime(2025,1,9),  "6 weken",  "Actief"),
                        (1, 3, "Kniebrace", new DateTime(2025,1,15), "12 weken", "Actief"),
                        (2, 4, "Rugbrace", new DateTime(2025,2,1), "8 weken",  "Actief"),
                        (3, 3, "Schouderbrace", new DateTime(2025,1,16), "4 weken", "Afgerond"),
                        (4, 4, "Enkelbrace", new DateTime(2025,2,6), "6 weken", "Actief"),
                        (5, 3, "Nekbrace", new DateTime(2025,3,3), "8 weken", "Actief"),
                        (6, 4, "Fysiotherapie banden", new DateTime(2025,2,12), "Onbepaald", "Actief"),
                        (7, 3, "Meniscusbrace", new DateTime(2025,3,7), "10 weken", "Actief"),
                        (8, 4, "TENS apparaat", new DateTime(2025,4,3), "Onbepaald", "Actief"),
                        (9, 3, "Polsbrace", new DateTime(2025,3,12), "4 weken", "Afgerond"),
                        (10,4, "Kniebrace", new DateTime(2025,4,7), "8 weken", "Actief"),
                        (11,3, "Rugkussen", new DateTime(2025,4,11), "Onbepaald", "Actief"),
                        (12,4, "Enkelgewichtjes", new DateTime(2025,4,16), "6 weken", "Actief"),
                        (13,3, "Looprek", new DateTime(2025,4,21), "12 weken", "Actief"),
                        (14,4, "Schouderkatrol", new DateTime(2025,4,26), "8 weken", "Actief"),
                        (15,3, "Knieband", new DateTime(2025,5,2), "6 weken", "Actief"),
                        (16,4, "Enkelfles", new DateTime(2025,5,6), "Onbepaald", "Actief"),
                        (17,3, "Krukken", new DateTime(2025,5,11), "4 weken", "Actief"),
                        (18,4, "Rugbrace", new DateTime(2025,5,16), "8 weken", "Actief"),
                        (19,3, "Schouderriem", new DateTime(2025,5,21), "6 weken", "Actief"),
                        (20,4, "Enkelondersteuning", new DateTime(2025,5,26), "4 weken", "Actief"),
                        (21,3, "Kniebrace", new DateTime(2025,5,29), "10 weken", "Actief"),
                        (22,4, "Hielspoorzooltjes", new DateTime(2025,5,31), "Onbepaald", "Actief"),
                        (23,3, "Rolstoel", new DateTime(2025,6,2), "12 weken", "Actief"),
                        (24,4, "Handtrainer", new DateTime(2025,6,6), "Onbepaald", "Actief"),
                    };

                    // haal patient ids via raw SQL (vermijdt EF-mapping errors)
                    var existingPatientIds = await GetAllIdsAsync(context, "Patients");

                    var toAdd = rows
                        .Where(r => existingPatientIds.Contains(r.PatientId))
                        .Select(r => new AccessoryAdvice
                        {
                            PatientId = r.PatientId,
                            GPUserId = r.GPUserId,
                            Name = r.Name,
                            AdviceDateUtc = DateTime.SpecifyKind(r.AdviceDate, DateTimeKind.Utc),
                            ExpectedUsagePeriod = r.Period,
                            Status = r.Status
                        })
                        .ToArray();

                    if (toAdd.Length > 0)
                    {
                        await context.AccessoryAdvices.AddRangeAsync(toAdd);
                        await context.SaveChangesAsync();
                    }
                }
            }
            else
            {
                Console.WriteLine("Skipping AccessoryAdvices seed — table dbo.AccessoryAdvices does not exist.");
            }
        }

        async static Task<HashSet<string>> GetTableColumnsAsync(ApplicationDbContext ctx, string tableName)
        {
            var cols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var conn = ctx.Database.GetDbConnection();
            try
            {
                if (conn.State != System.Data.ConnectionState.Open)
                    await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = @t";
                var p = cmd.CreateParameter();
                p.ParameterName = "@t";
                p.Value = tableName;
                cmd.Parameters.Add(p);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    cols.Add(reader.GetString(0));
                }
            }
            catch
            {
                // ignore
            }
            finally
            {
                try { await conn.CloseAsync(); } catch { }
            }
            return cols;
        }

        async static Task<bool> TableExistsAsync(ApplicationDbContext ctx, string tableName)
        {
            var conn = ctx.Database.GetDbConnection();
            try
            {
                if (conn.State != System.Data.ConnectionState.Open)
                    await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(1) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = @t";
                var p = cmd.CreateParameter();
                p.ParameterName = "@t";
                p.Value = tableName;
                cmd.Parameters.Add(p);

                var res = await cmd.ExecuteScalarAsync();
                if (res is int i) return i > 0;
                if (res is long l) return l > 0;
                return Convert.ToInt32(res) > 0;
            }
            catch
            {
                return false;
            }
            finally
            {
                try { await conn.CloseAsync(); } catch { }
            }
        }

        // Helpers (nieuw)
        async static Task<int> GetTableRowCountAsync(ApplicationDbContext ctx, string tableName)
        {
            var conn = ctx.Database.GetDbConnection();
            try
            {
                if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT COUNT(1) FROM dbo.[{tableName}]";
                var res = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(res);
            }
            catch
            {
                return -1;
            }
            finally
            {
                try { await conn.CloseAsync(); } catch { }
            }
        }

        async static Task<bool> IsTableEmptyAsync(ApplicationDbContext ctx, string tableName)
        {
            var c = await GetTableRowCountAsync(ctx, tableName);
            return c == 0;
        }

        async static Task<int> GetAnyIdAsync(ApplicationDbContext ctx, string tableName)
        {
            var conn = ctx.Database.GetDbConnection();
            try
            {
                if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT TOP(1) [Id] FROM dbo.[{tableName}]";
                var res = await cmd.ExecuteScalarAsync();
                return res == null || res == DBNull.Value ? 0 : Convert.ToInt32(res);
            }
            catch
            {
                return 0;
            }
            finally
            {
                try { await conn.CloseAsync(); } catch { }
            }
        }

        async static Task<HashSet<int>> GetAllIdsAsync(ApplicationDbContext ctx, string tableName)
        {
            var set = new HashSet<int>();
            var conn = ctx.Database.GetDbConnection();
            try
            {
                if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT [Id] FROM dbo.[{tableName}]";
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    if (!reader.IsDBNull(0)) set.Add(reader.GetInt32(0));
                }
            }
            catch
            {
                // ignore
            }
            finally
            {
                try { await conn.CloseAsync(); } catch { }
            }
            return set;
        }
    }
}
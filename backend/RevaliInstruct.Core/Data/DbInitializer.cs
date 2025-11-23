using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Core.Entities;

namespace RevaliInstruct.Core.Data
{
    public static class DbInitializer
    {
        /// <summary>
        /// Volledige reset: database droppen (indien mogelijk), opnieuw aanmaken en seeden.
        /// Wordt gebruikt door het dev endpoint /api/dev/reset-db.
        /// </summary>
        public static async Task ResetAndSeedAsync(ApplicationDbContext context)
        {
            try
            {
                await context.Database.EnsureDeletedAsync();
                Console.WriteLine("Database dropped via EnsureDeletedAsync.");
            }
            catch (Exception ex)
            {
                // Dit is de warning die je ziet als revali_login geen ALTER rechten heeft.
                Console.WriteLine($"Warning: failed to EnsureDeleted: {ex.Message}");
            }

            await context.Database.EnsureCreatedAsync();
            Console.WriteLine("Database created via EnsureCreatedAsync.");

            await SeedAsync(context);
        }

        /// <summary>
        /// Idempotente seed: maakt basisdata aan als tabellen leeg zijn.
        /// Wordt in Development bij startup aangeroepen.
        /// </summary>
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            //
            // 1. Users seeden (doctor + admin) via EF — dit werkt al goed.
            //
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

                await context.Users.AddRangeAsync(doctor, admin);
                await context.SaveChangesAsync();

                Console.WriteLine("Seeded default users: doctor/admin.");
            }
            else
            {
                Console.WriteLine("Users table already contains data; skipping user seed.");
            }

            //
            // 2. Patient seeding via RAW SQL
            //    We gebruiken alleen de kolommen die in jouw bestaande dbo.Patients staan:
            //    Id, FirstName, LastName, DateOfBirth, Notes, CreatedAt, StartDate, Status
            //    -> dus GEEN AssignedDoctorUserId en GEEN Diagnosis meer hier.
            //
            var hasAnyPatients = await context.Patients.AnyAsync();
            if (!hasAnyPatients)
            {
                var sql = @"
IF NOT EXISTS(SELECT 1 FROM dbo.Patients WHERE FirstName = @fn1 AND LastName = @ln1)
BEGIN
    INSERT INTO dbo.Patients (FirstName, LastName, DateOfBirth, Notes, CreatedAt, StartDate, Status)
    VALUES (@fn1, @ln1, @dob1, NULL, SYSUTCDATETIME(), @sd1, @st1);
END;

IF NOT EXISTS(SELECT 1 FROM dbo.Patients WHERE FirstName = @fn2 AND LastName = @ln2)
BEGIN
    INSERT INTO dbo.Patients (FirstName, LastName, DateOfBirth, Notes, CreatedAt, StartDate, Status)
    VALUES (@fn2, @ln2, @dob2, NULL, SYSUTCDATETIME(), @sd2, @st2);
END;
";

                // LET OP: hier geven we de parameters los door als object[]
                var parameters = new object[]
                {
        new Microsoft.Data.SqlClient.SqlParameter("@fn1", "Jan"),
        new Microsoft.Data.SqlClient.SqlParameter("@ln1", "Jansen"),
        new Microsoft.Data.SqlClient.SqlParameter("@dob1", new DateTime(1980, 5, 15)),
        new Microsoft.Data.SqlClient.SqlParameter("@sd1", new DateTime(2024, 5, 1)),
        new Microsoft.Data.SqlClient.SqlParameter("@st1", (int)PatientStatus.IntakePlanned),

        new Microsoft.Data.SqlClient.SqlParameter("@fn2", "Maria"),
        new Microsoft.Data.SqlClient.SqlParameter("@ln2", "Bakker"),
        new Microsoft.Data.SqlClient.SqlParameter("@dob2", new DateTime(1965, 8, 22)),
        new Microsoft.Data.SqlClient.SqlParameter("@sd2", new DateTime(2024, 1, 15)),
        new Microsoft.Data.SqlClient.SqlParameter("@st2", (int)PatientStatus.Active)
                };

                await context.Database.ExecuteSqlRawAsync(sql, parameters);

                Console.WriteLine("Seeded default patients via raw SQL (Jan Jansen & Maria Bakker).");
            }
            else
            {
                Console.WriteLine("Patients table already contains data; skipping patient seed.");
            }
        }
    }
}
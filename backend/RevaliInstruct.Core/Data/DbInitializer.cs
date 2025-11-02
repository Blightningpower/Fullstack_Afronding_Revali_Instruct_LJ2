using System.Globalization;
using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Core.Entities;

namespace RevaliInstruct.Core.Data
{
    public static class DbInitializer
    {
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
                await context.Users.AddRangeAsync(doctor, admin);
                await context.SaveChangesAsync();
            }

            // Bepaal arts-id (doctor role indien aanwezig, anders eerste user)
            var doctorId = await context.Users.Where(u => u.Role == "Doctor").Select(u => u.Id).FirstOrDefaultAsync();
            if (doctorId == 0)
                doctorId = await context.Users.Select(u => u.Id).FirstOrDefaultAsync();

            // Seed oefeningen (bibliotheek)
            if (!await context.Exercises.AnyAsync())
            {
                await context.Exercises.AddRangeAsync(
                    new Exercise { Code = "EX-KNEE-EXT", Title = "Knie extensie", Description = "Versterk quadriceps." },
                    new Exercise { Code = "EX-HIP-AB", Title = "Heup abductie", Description = "Versterk heupspieren." }
                );
                await context.SaveChangesAsync();
            }

            // Seed patiënten (alleen als leeg)
            if (!await context.Patients.AnyAsync())
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
                await context.Patients.AddRangeAsync(p1, p2);
                await context.SaveChangesAsync();

                // Intake voor p2 (zodat je StartDate in overzicht hebt)
                await context.Intakes.AddAsync(new IntakeRecord
                {
                    PatientId = p2.Id,
                    Diagnosis = p2.Diagnosis ?? "Onbekend",
                    InjurySeverity = "Matig",
                    InitialGoals = "Herstel mobiliteit en kracht",
                    IsLocked = true,
                    CreatedAtUtc = new DateTime(2024, 1, 15, 9, 0, 0, DateTimeKind.Utc)
                });

                await context.SaveChangesAsync();
            }

            // Optioneel: voorbeeld oefentoewijzing als er nog geen assignments zijn
            if (!await context.ExerciseAssignments.AnyAsync())
            {
                var anyPatientId = await context.Patients.Select(p => p.Id).FirstOrDefaultAsync();
                var anyExerciseId = await context.Exercises.Select(e => e.Id).FirstOrDefaultAsync();
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

            // Seed accessoire adviezen (alleen toevoegen als tabel leeg is)
            if (!await context.AccessoryAdvices.AnyAsync())
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

                var existingPatientIds = await context.Patients.Select(p => p.Id).ToListAsync();
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
    }
}
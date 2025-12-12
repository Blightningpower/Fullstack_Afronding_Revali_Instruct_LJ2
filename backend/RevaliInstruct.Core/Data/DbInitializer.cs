using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            // 1. Users seeden (doctor1, doctor2, admin) via EF
            //
            User doctor1;
            User doctor2;

            if (!await context.Users.AnyAsync())
            {
                doctor1 = new User
                {
                    Username = "doctor1",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("doctor123"),
                    Role = "Doctor",
                    FullName = "Dr. Revali",
                    Email = "doctor1@example.com"
                };

                doctor2 = new User
                {
                    Username = "doctor2",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("doctor234"),
                    Role = "Doctor",
                    FullName = "Dr. Instruct",
                    Email = "doctor2@example.com"
                };

                var admin = new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "Admin",
                    FullName = "Admin",
                    Email = "admin@example.com"
                };

                await context.Users.AddRangeAsync(doctor1, doctor2, admin);
                await context.SaveChangesAsync();

                Console.WriteLine("Seeded default users: doctor1, doctor2, admin.");
            }
            else
            {
                doctor1 = await context.Users.FirstAsync(u => u.Username == "doctor1");
                doctor2 = await context.Users.FirstAsync(u => u.Username == "doctor2");

                Console.WriteLine("Users already present; using existing doctor1/doctor2.");
            }

            //
            // 2. Patients seeden (8 stuks, 4 per dokter) + e-mail, telefoon, verwijzend arts
            //
            if (!await context.Patients.AnyAsync())
            {
                var patients = new List<Patient>
                {
                    // ---- Dokter 1: alle 4 statussen ----
                    new Patient
                    {
                        FirstName = "Jan",
                        LastName = "Jansen",
                        DateOfBirth = new DateTime(1980, 5, 15),
                        StartDate = new DateTime(2024, 5, 1),
                        Status = PatientStatus.IntakePlanned,
                        Diagnosis = "Kruisbandblessure",
                        AssignedDoctorUserId = doctor1.Id,
                        Email = "jan.jansen@example.com",
                        Phone = "+31611111111",
                        ReferringDoctor = "Dr. Instruct"
                    },
                    new Patient
                    {
                        FirstName = "Maria",
                        LastName = "Bakker",
                        DateOfBirth = new DateTime(1965, 8, 22),
                        StartDate = new DateTime(2024, 1, 15),
                        Status = PatientStatus.Active,
                        Diagnosis = "Heupoperatie",
                        AssignedDoctorUserId = doctor1.Id,
                        Email = "maria.bakker@example.com",
                        Phone = "+31622222222",
                        ReferringDoctor = "Dr. Instruct"
                    },
                    new Patient
                    {
                        FirstName = "Peter",
                        LastName = "de Vries",
                        DateOfBirth = new DateTime(1972, 3, 3),
                        StartDate = new DateTime(2023, 11, 10),
                        Status = PatientStatus.Completed,
                        Diagnosis = "Enkelrevalidatie",
                        AssignedDoctorUserId = doctor1.Id,
                        Email = "peter.devries@example.com",
                        Phone = "+31633333333",
                        ReferringDoctor = "Dr. Instruct"
                    },
                    new Patient
                    {
                        FirstName = "Sanne",
                        LastName = "Kosters",
                        DateOfBirth = new DateTime(1990, 6, 18),
                        StartDate = new DateTime(2024, 2, 5),
                        Status = PatientStatus.OnHold,
                        Diagnosis = "Rugklachten",
                        AssignedDoctorUserId = doctor1.Id,
                        Email = "sanne.kosters@example.com",
                        Phone = "+31644444444",
                        ReferringDoctor = "Dr. Instruct"
                    },

                    // ---- Dokter 2: ook alle 4 statussen ----
                    new Patient
                    {
                        FirstName = "Tom",
                        LastName = "Vermeer",
                        DateOfBirth = new DateTime(1985, 9, 1),
                        StartDate = new DateTime(2024, 3, 1),
                        Status = PatientStatus.IntakePlanned,
                        Diagnosis = "Schouderklachten",
                        AssignedDoctorUserId = doctor2.Id,
                        Email = "tom.vermeer@example.com",
                        Phone = "+31655555555",
                        ReferringDoctor = "Dr. Revali"
                    },
                    new Patient
                    {
                        FirstName = "Lisa",
                        LastName = "Smits",
                        DateOfBirth = new DateTime(1992, 12, 12),
                        StartDate = new DateTime(2024, 4, 20),
                        Status = PatientStatus.Active,
                        Diagnosis = "Revalidatie na knieoperatie",
                        AssignedDoctorUserId = doctor2.Id,
                        Email = "lisa.smits@example.com",
                        Phone = "+31666666666",
                        ReferringDoctor = "Dr. Revali"
                    },
                    new Patient
                    {
                        FirstName = "Ahmed",
                        LastName = "Hassan",
                        DateOfBirth = new DateTime(1978, 1, 25),
                        StartDate = new DateTime(2023, 9, 5),
                        Status = PatientStatus.Completed,
                        Diagnosis = "Polsblessure",
                        AssignedDoctorUserId = doctor2.Id,
                        Email = "ahmed.hassan@example.com",
                        Phone = "+31677777777",
                        ReferringDoctor = "Dr. Revali"
                    },
                    new Patient
                    {
                        FirstName = "Eva",
                        LastName = "Mulder",
                        DateOfBirth = new DateTime(1988, 10, 30),
                        StartDate = new DateTime(2024, 6, 10),
                        Status = PatientStatus.OnHold,
                        Diagnosis = "Nekklachten",
                        AssignedDoctorUserId = doctor2.Id,
                        Email = "eva.mulder@example.com",
                        Phone = "+31688888888",
                        ReferringDoctor = "Dr. Revali"
                    }
                };

                await context.Patients.AddRangeAsync(patients);
                await context.SaveChangesAsync();

                Console.WriteLine("Seeded default patients via EF (8 patiënten, 4 per dokter, alle statussen).");
            }
            else
            {
                Console.WriteLine("Patients table already contains data; skipping patient seed.");
            }

            //
            // 3. Exercises seeden
            //
            if (!await context.Exercises.AnyAsync())
            {
                var knee = new Exercise
                {
                    Code = "EX-KNIE-1",
                    Title = "Kniebuigingen",
                    Description = "Versterking bovenbeenspieren"
                };
                var shoulder = new Exercise
                {
                    Code = "EX-SCHOUDER-1",
                    Title = "Schouderrotaties",
                    Description = "Mobiliteit van de schouder"
                };
                var hip = new Exercise
                {
                    Code = "EX-HEUP-1",
                    Title = "Heupheffingen",
                    Description = "Versterking heupspieren"
                };

                await context.Exercises.AddRangeAsync(knee, shoulder, hip);
                await context.SaveChangesAsync();

                Console.WriteLine("Seeded default exercises.");
            }

            //
            // 4. Dossier-data voor een paar patiënten (Ahmed als ‘mooie demo’)
            //
            var ahmed = await context.Patients.FirstOrDefaultAsync(p =>
                p.FirstName == "Ahmed" && p.LastName == "Hassan");
            var lisa = await context.Patients.FirstOrDefaultAsync(p =>
                p.FirstName == "Lisa" && p.LastName == "Smits");

            var exKnee = await context.Exercises.FirstOrDefaultAsync(e => e.Code == "EX-KNIE-1");
            var exShoulder = await context.Exercises.FirstOrDefaultAsync(e => e.Code == "EX-SCHOUDER-1");

            // 4a. Oefeningen voor Ahmed & Lisa
            if (ahmed != null && exKnee != null &&
                !await context.ExerciseAssignments.AnyAsync(a => a.PatientId == ahmed.Id))
            {
                await context.ExerciseAssignments.AddRangeAsync(
                    new ExerciseAssignment
                    {
                        PatientId = ahmed.Id,
                        ExerciseId = exKnee.Id,
                        Repetitions = 10,
                        Sets = 3,
                        Frequency = "Dagelijks",
                        Duration = TimeSpan.FromMinutes(10),
                        StartDateUtc = new DateTime(2023, 9, 5, 9, 0, 0, DateTimeKind.Utc),
                        EndDateUtc = new DateTime(2023, 10, 5, 9, 0, 0, DateTimeKind.Utc),
                        ClientCheckedOff = true,
                        AssignedByUserId = doctor2.Id
                    },
                    new ExerciseAssignment
                    {
                        PatientId = ahmed.Id,
                        ExerciseId = (exShoulder ?? exKnee).Id,
                        Repetitions = 15,
                        Sets = 2,
                        Frequency = "3x per week",
                        Duration = TimeSpan.FromMinutes(5),
                        StartDateUtc = new DateTime(2023, 9, 5, 9, 15, 0, DateTimeKind.Utc),
                        EndDateUtc = new DateTime(2023, 10, 20, 9, 15, 0, DateTimeKind.Utc),
                        ClientCheckedOff = true,
                        AssignedByUserId = doctor2.Id
                    }
                );

                await context.SaveChangesAsync();
                Console.WriteLine("Seeded exercise assignments for Ahmed.");
            }

            if (lisa != null && exKnee != null &&
                !await context.ExerciseAssignments.AnyAsync(a => a.PatientId == lisa.Id))
            {
                await context.ExerciseAssignments.AddAsync(
                    new ExerciseAssignment
                    {
                        PatientId = lisa.Id,
                        ExerciseId = exKnee.Id,
                        Repetitions = 12,
                        Sets = 3,
                        Frequency = "Om de dag",
                        Duration = TimeSpan.FromMinutes(8),
                        StartDateUtc = new DateTime(2024, 4, 20, 9, 0, 0, DateTimeKind.Utc),
                        EndDateUtc = null,
                        ClientCheckedOff = false,
                        AssignedByUserId = doctor2.Id
                    }
                );

                await context.SaveChangesAsync();
                Console.WriteLine("Seeded exercise assignments for Lisa.");
            }

            // 4b. Pijnregistratie voor Ahmed
            if (ahmed != null && !await context.PainEntries.AnyAsync(p => p.PatientId == ahmed.Id))
            {
                await context.PainEntries.AddRangeAsync(
                    new PainEntry
                    {
                        PatientId = ahmed.Id,
                        RecordedAtUtc = new DateTime(2023, 9, 5, 10, 0, 0, DateTimeKind.Utc),
                        Score = 7,
                        Location = "Pols",
                        Note = "Start revalidatie"
                    },
                    new PainEntry
                    {
                        PatientId = ahmed.Id,
                        RecordedAtUtc = new DateTime(2023, 9, 20, 10, 0, 0, DateTimeKind.Utc),
                        Score = 4,
                        Location = "Pols",
                        Note = "Duidelijke verbetering"
                    },
                    new PainEntry
                    {
                        PatientId = ahmed.Id,
                        RecordedAtUtc = new DateTime(2023, 10, 10, 10, 0, 0, DateTimeKind.Utc),
                        Score = 1,
                        Location = "Pols",
                        Note = "Bijna pijnvrij"
                    }
                );

                await context.SaveChangesAsync();
                Console.WriteLine("Seeded pain entries for Ahmed.");
            }

            // 4c. Activiteitenlog voor Ahmed
            if (ahmed != null && !await context.ActivityLogs.AnyAsync(a => a.PatientId == ahmed.Id))
            {
                await context.ActivityLogs.AddRangeAsync(
                    new ActivityLogEntry
                    {
                        PatientId = ahmed.Id,
                        LoggedAtUtc = new DateTime(2023, 9, 10, 18, 0, 0, DateTimeKind.Utc),
                        Activity = "Boodschappen doen",
                        Details = "Met lichte brace"
                    },
                    new ActivityLogEntry
                    {
                        PatientId = ahmed.Id,
                        LoggedAtUtc = new DateTime(2023, 9, 25, 18, 0, 0, DateTimeKind.Utc),
                        Activity = "Fietsen",
                        Details = "15 minuten zonder klachten"
                    }
                );

                await context.SaveChangesAsync();
                Console.WriteLine("Seeded activity logs for Ahmed.");
            }

            // 4d. Afspraken voor Ahmed
            if (ahmed != null && !await context.Appointments.AnyAsync(a => a.PatientId == ahmed.Id))
            {
                await context.Appointments.AddRangeAsync(
                    new Appointment
                    {
                        PatientId = ahmed.Id,
                        StartUtc = new DateTime(2023, 9, 5, 9, 0, 0, DateTimeKind.Utc),
                        Duration = TimeSpan.FromMinutes(30),
                        Type = "Intake",
                        Status = AppointmentStatus.Completed,
                        CreatedByUserId = doctor2.Id
                    },
                    new Appointment
                    {
                        PatientId = ahmed.Id,
                        StartUtc = new DateTime(2023, 10, 10, 9, 0, 0, DateTimeKind.Utc),
                        Duration = TimeSpan.FromMinutes(30),
                        Type = "Evaluatie",
                        Status = AppointmentStatus.Completed,
                        CreatedByUserId = doctor2.Id
                    }
                );

                await context.SaveChangesAsync();
                Console.WriteLine("Seeded appointments for Ahmed.");
            }

            // 4e. Medicatie/accessoires voor Ahmed via AccessoryAdvice
            if (ahmed != null && !await context.AccessoryAdvices.AnyAsync(a => a.PatientId == ahmed.Id))
            {
                await context.AccessoryAdvices.AddAsync(
                    new AccessoryAdvice
                    {
                        PatientId = ahmed.Id,
                        GPUserId = doctor1.Id, // huisarts in dataset; hier simpel doctor1
                        Name = "Polsbrace",
                        AdviceDateUtc = new DateTime(2023, 9, 5, 0, 0, 0, DateTimeKind.Utc),
                        ExpectedUsagePeriod = "6 weken",
                        Status = "Afgerond"
                    }
                );

                await context.SaveChangesAsync();
                Console.WriteLine("Seeded accessory advice for Ahmed.");
            }

            // (AuditLogs en PatientNotes kun je later op dezelfde manier vullen als dat nodig is.)
        }
    }
}
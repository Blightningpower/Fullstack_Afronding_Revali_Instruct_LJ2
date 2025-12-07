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
                // Als er al users zijn, haal doctor1/doctor2 op op basis van username
                doctor1 = await context.Users.FirstAsync(u => u.Username == "doctor1");
                doctor2 = await context.Users.FirstAsync(u => u.Username == "doctor2");

                Console.WriteLine("Users already present; using existing doctor1/doctor2.");
            }

            //
            // 2. Patients seeden via EF met AssignedDoctorUserId + alle statussen
            //
            var hasAnyPatients = await context.Patients.AnyAsync();
            if (!hasAnyPatients)
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
                        AssignedDoctorUserId = doctor1.Id
                    },
                    new Patient
                    {
                        FirstName = "Maria",
                        LastName = "Bakker",
                        DateOfBirth = new DateTime(1965, 8, 22),
                        StartDate = new DateTime(2024, 1, 15),
                        Status = PatientStatus.Active,
                        Diagnosis = "Heupoperatie",
                        AssignedDoctorUserId = doctor1.Id
                    },
                    new Patient
                    {
                        FirstName = "Peter",
                        LastName = "de Vries",
                        DateOfBirth = new DateTime(1972, 3, 3),
                        StartDate = new DateTime(2023, 11, 10),
                        Status = PatientStatus.Completed,
                        Diagnosis = "Enkelrevalidatie",
                        AssignedDoctorUserId = doctor1.Id
                    },
                    new Patient
                    {
                        FirstName = "Sanne",
                        LastName = "Kosters",
                        DateOfBirth = new DateTime(1990, 6, 18),
                        StartDate = new DateTime(2024, 2, 5),
                        Status = PatientStatus.OnHold,
                        Diagnosis = "Rugklachten",
                        AssignedDoctorUserId = doctor1.Id
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
                        AssignedDoctorUserId = doctor2.Id
                    },
                    new Patient
                    {
                        FirstName = "Lisa",
                        LastName = "Smits",
                        DateOfBirth = new DateTime(1992, 12, 12),
                        StartDate = new DateTime(2024, 4, 20),
                        Status = PatientStatus.Active,
                        Diagnosis = "Revalidatie na knieoperatie",
                        AssignedDoctorUserId = doctor2.Id
                    },
                    new Patient
                    {
                        FirstName = "Ahmed",
                        LastName = "Hassan",
                        DateOfBirth = new DateTime(1978, 1, 25),
                        StartDate = new DateTime(2023, 9, 5),
                        Status = PatientStatus.Completed,
                        Diagnosis = "Polsblessure",
                        AssignedDoctorUserId = doctor2.Id
                    },
                    new Patient
                    {
                        FirstName = "Eva",
                        LastName = "Mulder",
                        DateOfBirth = new DateTime(1988, 10, 30),
                        StartDate = new DateTime(2024, 6, 10),
                        Status = PatientStatus.OnHold,
                        Diagnosis = "Nekklachten",
                        AssignedDoctorUserId = doctor2.Id
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
        }
    }
}
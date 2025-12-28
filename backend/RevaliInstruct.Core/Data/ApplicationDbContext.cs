using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Core.Entities;
using BCrypt.Net;

namespace RevaliInstruct.Core.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<IntakeRecord> Intakes => Set<IntakeRecord>();
        public DbSet<Exercise> Exercises => Set<Exercise>();
        public DbSet<ExerciseAssignment> ExerciseAssignments => Set<ExerciseAssignment>();
        public DbSet<PainEntry> PainEntries => Set<PainEntry>();
        public DbSet<ActivityLogEntry> ActivityLogs => Set<ActivityLogEntry>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
        public DbSet<PatientNote> PatientNotes => Set<PatientNote>();
        public DbSet<AccessoryAdvice> AccessoryAdvices => Set<AccessoryAdvice>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Entity Configuraties ---

            // User configuratie
            modelBuilder.Entity<User>(b =>
            {
                b.HasIndex(u => u.Username).IsUnique();
                b.Property(u => u.Role).HasMaxLength(50);
            });

            // Patient configuratie met restrictie op verwijderen arts
            modelBuilder.Entity<Patient>(b =>
            {
                b.HasOne(p => p.AssignedDoctor)
                    .WithMany(u => u.Patients)
                    .HasForeignKey(p => p.AssignedDoctorUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Intake (1-1 relatie met Patient)
            modelBuilder.Entity<IntakeRecord>(b =>
            {
                b.HasOne(i => i.Patient)
                    .WithOne(p => p.Intake)
                    .HasForeignKey<IntakeRecord>(i => i.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ExerciseAssignment configuratie
            modelBuilder.Entity<ExerciseAssignment>(b =>
            {
                b.HasOne(a => a.Exercise)
                    .WithMany()
                    .HasForeignKey(a => a.ExerciseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Enum conversies naar string voor database opslag
            modelBuilder.Entity<Appointment>()
                .Property(a => a.Status)
                .HasConversion<string>();

            modelBuilder.Entity<InvoiceItem>()
                .Property(i => i.Status)
                .HasConversion<string>();

            // Decimal precisie voor InvoiceItem.Amount
            modelBuilder.Entity<InvoiceItem>()
                .Property(i => i.Amount)
                .HasPrecision(18, 2);

            // --- Data Seeding (gebaseerd op Eindopdracht-data.xlsx) ---

            // 1. Gebruikers
            var staticHash = "$2y$10$9mIxp6HbC1KobigZYb0qUu98xe11w45XH1kvPKX2qZ44BsF2qObDy"; // Hash voor "password123"
            
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "zvm_jansen", PasswordHash = staticHash, Role = "Zorgverzekeraar", FirstName = "Sophie", LastName = "Jansen" },
                new User { Id = 2, Username = "zvm_devries", PasswordHash = staticHash, Role = "Zorgverzekeraar", FirstName = "Mark", LastName = "de Vries" },
                new User { Id = 3, Username = "ha_bakker", PasswordHash = staticHash, Role = "Huisarts", FirstName = "Lisa", LastName = "Bakker" },
                new User { Id = 4, Username = "ha_janssen", PasswordHash = staticHash, Role = "Huisarts", FirstName = "Thomas", LastName = "Janssen" },
                new User { Id = 5, Username = "ra_smit", PasswordHash = staticHash, Role = "Revalidatiearts", FirstName = "Emma", LastName = "Smit" },
                new User { Id = 6, Username = "ra_groen", PasswordHash = staticHash, Role = "Revalidatiearts", FirstName = "David", LastName = "Groen" },
                new User { Id = 7, Username = "ra_visser", PasswordHash = staticHash, Role = "Revalidatiearts", FirstName = "Linda", LastName = "Visser" }
            );

            // 2. Patienten (Patienten.csv)
            modelBuilder.Entity<Patient>().HasData(
                new Patient { Id = 1, FirstName = "Freddy", LastName = "Voetballer", BirthDate = new DateTime(1995, 3, 15), Address = "Kruisbandstraat 10", Email = "freddy.v@example.com", AssignedDoctorUserId = 3, StartDate = new DateTime(2025, 1, 12), Status = PatientStatus.Active },
                new Patient { Id = 2, FirstName = "Anna", LastName = "Kamer", BirthDate = new DateTime(1988, 11, 22), Address = "Therapiestraat 5", Email = "anna.k@example.com", AssignedDoctorUserId = 4, StartDate = new DateTime(2025, 2, 3), Status = PatientStatus.Completed },
                new Patient { Id = 3, FirstName = "Bas", LastName = "Verkade", BirthDate = new DateTime(1976, 7, 1), Address = "Oefenlaan 12", Email = "bas.v@example.com", AssignedDoctorUserId = 3, StartDate = new DateTime(2025, 1, 17), Status = PatientStatus.Active },
                new Patient { Id = 4, FirstName = "Carla", LastName = "Dirksen", BirthDate = new DateTime(2001, 1, 30), Address = "Herstelweg 3", Email = "carla.d@example.com", AssignedDoctorUserId = 4, StartDate = new DateTime(2025, 4, 1), Status = PatientStatus.IntakePlanned },
                new Patient { Id = 5, FirstName = "Dirk", LastName = "Meijer", BirthDate = new DateTime(1965, 9, 10), Address = "Revalidatieplein 7", Email = "dirk.m@example.com", AssignedDoctorUserId = 3, StartDate = new DateTime(2025, 2, 7), Status = PatientStatus.OnHold },
                new Patient { Id = 6, FirstName = "Eva", LastName = "Peters", BirthDate = new DateTime(1999, 4, 5), Address = "Fysioweg 22", Email = "eva.p@example.com", AssignedDoctorUserId = 4, StartDate = new DateTime(2025, 2, 12), Status = PatientStatus.Active },
                new Patient { Id = 7, FirstName = "Frank", LastName = "de Jong", BirthDate = new DateTime(1970, 2, 18), Address = "Gipsstraat 1", Email = "frank.dj@example.com", AssignedDoctorUserId = 3, StartDate = new DateTime(2025, 3, 7), Status = PatientStatus.IntakePlanned },
                new Patient { Id = 8, FirstName = "Greetje", LastName = "Willems", BirthDate = new DateTime(1985, 12, 3), Address = "Rolstoelpad 8", Email = "greetje.w@example.com", AssignedDoctorUserId = 4, StartDate = new DateTime(2025, 3, 4), Status = PatientStatus.Active },
                new Patient { Id = 9, FirstName = "Hans", LastName = "Kramer", BirthDate = new DateTime(1993, 6, 25), Address = "Sportlaan 15", Email = "hans.k@example.com", AssignedDoctorUserId = 3, StartDate = new DateTime(2025, 3, 12), Status = PatientStatus.Completed },
                new Patient { Id = 10, FirstName = "Irene", LastName = "Van Dam", BirthDate = new DateTime(1972, 10, 14), Address = "Zorgcentrum 4", Email = "irene.vd@example.com", AssignedDoctorUserId = 4, StartDate = new DateTime(2025, 4, 7), Status = PatientStatus.IntakePlanned },
                new Patient { Id = 11, FirstName = "Jeroen", LastName = "Faber", BirthDate = new DateTime(1980, 8, 8), Address = "Gezondheidsstraat 2", Email = "jeroen.f@example.com", AssignedDoctorUserId = 3, StartDate = new DateTime(2025, 4, 10), Status = PatientStatus.Active },
                new Patient { Id = 12, FirstName = "Kim", LastName = "Schouten", BirthDate = new DateTime(1997, 1, 20), Address = "Pijnbestrijding 1", Email = "kim.s@example.com", AssignedDoctorUserId = 4, StartDate = new DateTime(2025, 4, 15), Status = PatientStatus.OnHold },
                new Patient { Id = 13, FirstName = "Lars", LastName = "Veenstra", BirthDate = new DateTime(1960, 5, 12), Address = "Welzijnshof 9", Email = "lars.v@example.com", AssignedDoctorUserId = 3, StartDate = new DateTime(2025, 4, 20), Status = PatientStatus.Active },
                new Patient { Id = 14, FirstName = "Mieke", LastName = "Bos", BirthDate = new DateTime(1990, 3, 3), Address = "Bewegingstraat 6", Email = "mieke.b@example.com", AssignedDoctorUserId = 4, StartDate = new DateTime(2025, 4, 25), Status = PatientStatus.IntakePlanned },
                new Patient { Id = 15, FirstName = "Niels", LastName = "Dekker", BirthDate = new DateTime(1978, 7, 28), Address = "Vitaliteitslaan 11", Email = "niels.d@example.com", AssignedDoctorUserId = 3, StartDate = new DateTime(2025, 5, 1), Status = PatientStatus.Active },
                new Patient { Id = 16, FirstName = "Olga", LastName = "Postma", BirthDate = new DateTime(1983, 9, 9), Address = "Herstelstraat 14", Email = "olga.p@example.com", AssignedDoctorUserId = 4, StartDate = new DateTime(2025, 5, 5), Status = PatientStatus.Completed },
                new Patient { Id = 17, FirstName = "Paul", LastName = "Kuipers", BirthDate = new DateTime(1992, 11, 11), Address = "Zorgpad 1", Email = "paul.k@example.com", AssignedDoctorUserId = 3, StartDate = new DateTime(2025, 5, 10), Status = PatientStatus.Active },
                new Patient { Id = 18, FirstName = "Quinn", LastName = "Vries", BirthDate = new DateTime(1975, 4, 4), Address = "Medicijnlaan 3", Email = "quinn.v@example.com", AssignedDoctorUserId = 4, StartDate = new DateTime(2025, 5, 15), Status = PatientStatus.IntakePlanned },
                new Patient { Id = 19, FirstName = "Ruben", LastName = "Smeets", BirthDate = new DateTime(1987, 2, 2), Address = "Oefenplein 5", Email = "ruben.s@example.com", AssignedDoctorUserId = 3, StartDate = new DateTime(2025, 5, 20), Status = PatientStatus.OnHold },
                new Patient { Id = 20, FirstName = "Sarah", LastName = "Koks", BirthDate = new DateTime(1994, 8, 18), Address = "Genezingsweg 10", Email = "sarah.k@example.com", AssignedDoctorUserId = 4, StartDate = new DateTime(2025, 5, 25), Status = PatientStatus.Active },
                new Patient { Id = 21, FirstName = "Tim", LastName = "Visser", BirthDate = new DateTime(1968, 10, 25), Address = "Sportlaan 2", Email = "tim.v@example.com", AssignedDoctorUserId = 3, StartDate = new DateTime(2025, 5, 28), Status = PatientStatus.Completed },
                new Patient { Id = 22, FirstName = "Ursula", LastName = "Bouwman", BirthDate = new DateTime(1981, 6, 7), Address = "Kwaliteitsstraat 7", Email = "ursula.b@example.com", AssignedDoctorUserId = 4, StartDate = new DateTime(2025, 5, 30), Status = PatientStatus.Active },
                new Patient { Id = 23, FirstName = "Vincent", LastName = "Meijerink", BirthDate = new DateTime(1996, 3, 29), Address = "Zorglaan 9", Email = "vincent.m@example.com", AssignedDoctorUserId = 3, StartDate = new DateTime(2025, 6, 1), Status = PatientStatus.IntakePlanned },
                new Patient { Id = 24, FirstName = "Wendy", LastName = "van der Velde", BirthDate = new DateTime(1979, 1, 13), Address = "Therapieplein 12", Email = "wendy.vdv@example.com", AssignedDoctorUserId = 4, StartDate = new DateTime(2025, 6, 5), Status = PatientStatus.Active },
                new Patient { Id = 25, FirstName = "Xavier", LastName = "De Ruiter", BirthDate = new DateTime(1986, 5, 5), Address = "Revalidatiepad 15", Email = "xavier.dr@example.com", AssignedDoctorUserId = 3, StartDate = new DateTime(2025, 6, 10), Status = PatientStatus.Completed }
            );

            // 3. Oefeningen (Oefeningen.csv)
            modelBuilder.Entity<Exercise>().HasData(
                new Exercise { Id = 1, Name = "Kniebuiging", Description = "Langzaam knieën buigen tot 90 graden rug recht houden.", VideoUrl = "https://www.youtube.com/watch?v=00g8DINXtIY" },
                new Exercise { Id = 2, Name = "Hamstring Stretch", Description = "Zittend één been gestrekt", VideoUrl = "https://www.youtube.com/shorts/ywgDXzYD6v8" },
                new Exercise { Id = 3, Name = "Calf Raises", Description = "Op de tenen staan en langzaam zakken.", VideoUrl = "https://www.youtube.com/watch?v=eMTy3qylqnE" },
                new Exercise { Id = 4, Name = "Core Stability Plank", Description = "Plankpositie rug recht", VideoUrl = "https://www.youtube.com/watch?v=pvIjsG5Svck" },
                new Exercise { Id = 5, Name = "Schouder Rotaties", Description = "Cirkelbewegingen met de armen.", VideoUrl = "https://www.youtube.com/watch?v=oLwTC-lAJws&list=PLurruTwy8-10c7tAfYnJRkFVy_U1O3IZ1" }
            );

            // 4. Intakegesprekken (Intakegesprekken.csv)
            modelBuilder.Entity<IntakeRecord>().HasData(
                new IntakeRecord { Id = 1, PatientId = 1, DoctorId = 5, Diagnosis = "Gescheurde kruisbanden (ACL)", Severity = "Ernstig", Goals = "Volledig herstel kniefunctie; wondgenezing; pijnmanagement", Date = new DateTime(2025, 1, 10) },
                new IntakeRecord { Id = 2, PatientId = 2, DoctorId = 6, Diagnosis = "Hernia L5-S1", Severity = "Matig", Goals = "Pijnreductie; mobiliteit; terugkeer naar werk", Date = new DateTime(2025, 2, 1) },
                new IntakeRecord { Id = 3, PatientId = 3, DoctorId = 5, Diagnosis = "Schouder impingement", Severity = "Licht tot matig", Goals = "Pijnvrij bewegen; krachtopbouw", Date = new DateTime(2025, 1, 15) }
            );

            // 5. Pijnindicaties (Pijnindicaties.csv)
            modelBuilder.Entity<PainEntry>().HasData(
                new PainEntry { Id = 1, PatientId = 1, Timestamp = new DateTime(2025, 1, 10, 10, 0, 0), Score = 7, Note = "Na de operatie" },
                new PainEntry { Id = 2, PatientId = 1, Timestamp = new DateTime(2025, 1, 11, 9, 0, 0), Score = 6, Note = "Iets minder pijn" },
                new PainEntry { Id = 3, PatientId = 1, Timestamp = new DateTime(2025, 1, 12, 9, 30, 0), Score = 5, Note = "Pijn bij bewegen" }
            );

            // 6. Notities (Notities.csv)
            modelBuilder.Entity<PatientNote>().HasData(
                new PatientNote { Id = 1, PatientId = 1, AuthorId = 3, Timestamp = new DateTime(2025, 1, 9, 16, 0, 0), Content = "Wondinfectie arm lijkt te verbeteren na start antibiotica." },
                new PatientNote { Id = 2, PatientId = 1, AuthorId = 5, Timestamp = new DateTime(2025, 1, 10, 15, 0, 0), Content = "Patiënt is gemotiveerd" }
            );

            // 7. Afspraken (Afspraken.csv)
            modelBuilder.Entity<Appointment>().HasData(
                new Appointment { Id = 1, PatientId = 1, DoctorId = 5, AppointmentDateTime = new DateTime(2025, 1, 10, 14, 0, 0), DurationMinutes = 60, Type = "Intake", Status = "Afgerond" },
                new Appointment { Id = 2, PatientId = 1, DoctorId = 5, AppointmentDateTime = new DateTime(2025, 1, 31, 10, 0, 0), DurationMinutes = 45, Type = "Controle", Status = "Afgerond" },
                new Appointment { Id = 5, PatientId = 1, DoctorId = 5, AppointmentDateTime = new DateTime(2025, 4, 4, 9, 0, 0), DurationMinutes = 30, Type = "Controle", Status = "Gepland" }
            );

            // 8. Declaraties (Declaraties.csv)
            modelBuilder.Entity<InvoiceItem>().HasData(
                new InvoiceItem { Id = 1, PatientId = 1, Type = "Huisarts", AuthorId = 3, Date = new DateTime(2025, 1, 8), Description = "Consult wondverzorging arm", Amount = 3500, Status = "Nieuw" },
                new InvoiceItem { Id = 27, PatientId = 1, Type = "Revalidatiearts", AuthorId = 5, Date = new DateTime(2025, 1, 10), Description = "Intake revalidatie knie", Amount = 8000, Status = "Nieuw" }
            );

            // 9. Accessoires (Accessoires.csv)
            modelBuilder.Entity<AccessoryAdvice>().HasData(
                new AccessoryAdvice { Id = 1, PatientId = 1, HuisartsId = 3, Name = "Krukken", AdviceDate = new DateTime(2025, 1, 9), Duration = "6 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 2, PatientId = 1, HuisartsId = 3, Name = "Kniebrace", AdviceDate = new DateTime(2025, 1, 15), Duration = "12 weken", Status = "Actief" }
            );
        }
    }
}
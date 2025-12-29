using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Core.Entities;
using System;

namespace RevaliInstruct.Core.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Exercise> Exercises => Set<Exercise>();
        public DbSet<ExerciseAssignment> ExerciseAssignments => Set<ExerciseAssignment>();
        public DbSet<PainEntry> PainEntries => Set<PainEntry>();
        public DbSet<ActivityLogEntry> ActivityLogs => Set<ActivityLogEntry>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<PatientNote> PatientNotes => Set<PatientNote>();
        public DbSet<AccessoryAdvice> AccessoryAdvices => Set<AccessoryAdvice>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relatie configuraties
            modelBuilder.Entity<Patient>(b =>
            {
                b.HasOne(p => p.AssignedDoctor).WithMany(u => u.Patients).HasForeignKey(p => p.AssignedDoctorUserId).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(p => p.ReferringDoctor).WithMany().HasForeignKey(p => p.ReferringDoctorUserId).OnDelete(DeleteBehavior.Restrict);
            });

            var staticHash = "$2y$10$9mIxp6HbC1KobigZYb0qUu98xe11w45XH1kvPKX2qZ44BsF2qObDy";

            // 1. Gebruikers
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "zvm_jansen", PasswordHash = staticHash, Role = "Zorgverzekeraar", FirstName = "Sophie", LastName = "Jansen" },
                new User { Id = 2, Username = "zvm_devries", PasswordHash = staticHash, Role = "Zorgverzekeraar", FirstName = "Mark", LastName = "de Vries" },
                new User { Id = 3, Username = "ha_bakker", PasswordHash = staticHash, Role = "Huisarts", FirstName = "Lisa", LastName = "Bakker" },
                new User { Id = 4, Username = "ha_janssen", PasswordHash = staticHash, Role = "Huisarts", FirstName = "Thomas", LastName = "Janssen" },
                new User { Id = 5, Username = "ra_smit", PasswordHash = staticHash, Role = "Revalidatiearts", FirstName = "Emma", LastName = "Smit" },
                new User { Id = 6, Username = "ra_groen", PasswordHash = staticHash, Role = "Revalidatiearts", FirstName = "David", LastName = "Groen" },
                new User { Id = 7, Username = "ra_visser", PasswordHash = staticHash, Role = "Revalidatiearts", FirstName = "Linda", LastName = "Visser" }
            );

            // 2. Patienten
            modelBuilder.Entity<Patient>().HasData(
                new Patient { Id = 1, FirstName = "Freddy", LastName = "Voetballer", BirthDate = new DateTime(1995, 3, 15), Phone = "0612345678", Email = "freddy.v@example.com", AssignedDoctorUserId = 5, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 1, 12), Status = PatientStatus.Active },
                new Patient { Id = 2, FirstName = "Anna", LastName = "Kamer", BirthDate = new DateTime(1988, 11, 22), Phone = "0623456789", Email = "anna.k@example.com", AssignedDoctorUserId = 6, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 2, 3), Status = PatientStatus.Completed },
                new Patient { Id = 3, FirstName = "Bas", LastName = "Verkade", BirthDate = new DateTime(1976, 7, 1), Phone = "0634567890", Email = "bas.v@example.com", AssignedDoctorUserId = 5, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 1, 17), Status = PatientStatus.Active },
                new Patient { Id = 4, FirstName = "Carla", LastName = "Dirksen", BirthDate = new DateTime(2001, 1, 30), Phone = "0645678901", Email = "carla.d@example.com", AssignedDoctorUserId = 7, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 4, 1), Status = PatientStatus.IntakePlanned },
                new Patient { Id = 5, FirstName = "Dirk", LastName = "Meijer", BirthDate = new DateTime(1965, 9, 10), Phone = "0656789012", Email = "dirk.m@example.com", AssignedDoctorUserId = 6, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 2, 7), Status = PatientStatus.OnHold },
                new Patient { Id = 6, FirstName = "Eva", LastName = "Peters", BirthDate = new DateTime(1999, 4, 5), Phone = "0667890123", AssignedDoctorUserId = 5, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 2, 12), Status = PatientStatus.Active },
                new Patient { Id = 7, FirstName = "Frank", LastName = "de Jong", BirthDate = new DateTime(1970, 2, 18), Phone = "0678901234", AssignedDoctorUserId = 7, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 3, 7), Status = PatientStatus.IntakePlanned },
                new Patient { Id = 8, FirstName = "Greetje", LastName = "Willems", BirthDate = new DateTime(1985, 12, 3), Phone = "0689012345", AssignedDoctorUserId = 6, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 3, 4), Status = PatientStatus.Active },
                new Patient { Id = 9, FirstName = "Hans", LastName = "Kramer", BirthDate = new DateTime(1993, 6, 25), Phone = "0690123456", AssignedDoctorUserId = 5, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 3, 12), Status = PatientStatus.Completed },
                new Patient { Id = 10, FirstName = "Irene", LastName = "Van Dam", BirthDate = new DateTime(1972, 10, 14), Phone = "0601234567", AssignedDoctorUserId = 7, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 4, 7), Status = PatientStatus.IntakePlanned },
                new Patient { Id = 11, FirstName = "Jeroen", LastName = "Faber", BirthDate = new DateTime(1980, 8, 8), Phone = "0611223344", AssignedDoctorUserId = 6, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 4, 10), Status = PatientStatus.Active },
                new Patient { Id = 12, FirstName = "Kim", LastName = "Schouten", BirthDate = new DateTime(1997, 1, 20), Phone = "0622334455", AssignedDoctorUserId = 5, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 4, 15), Status = PatientStatus.OnHold },
                new Patient { Id = 13, FirstName = "Lars", LastName = "Veenstra", BirthDate = new DateTime(1960, 5, 12), Phone = "0633445566", AssignedDoctorUserId = 7, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 4, 20), Status = PatientStatus.Active },
                new Patient { Id = 14, FirstName = "Mieke", LastName = "Bos", BirthDate = new DateTime(1990, 3, 3), Phone = "0644556677", AssignedDoctorUserId = 6, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 4, 25), Status = PatientStatus.IntakePlanned },
                new Patient { Id = 15, FirstName = "Niels", LastName = "Dekker", BirthDate = new DateTime(1978, 7, 28), Phone = "0655667788", AssignedDoctorUserId = 5, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 5, 1), Status = PatientStatus.Active },
                new Patient { Id = 16, FirstName = "Olga", LastName = "Postma", BirthDate = new DateTime(1983, 9, 9), Phone = "0666778899", AssignedDoctorUserId = 7, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 5, 5), Status = PatientStatus.Completed },
                new Patient { Id = 17, FirstName = "Paul", LastName = "Kuipers", BirthDate = new DateTime(1992, 11, 11), Phone = "0677889900", AssignedDoctorUserId = 6, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 5, 10), Status = PatientStatus.Active },
                new Patient { Id = 18, FirstName = "Quinn", LastName = "Vries", BirthDate = new DateTime(1975, 4, 4), Phone = "0688990011", AssignedDoctorUserId = 5, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 5, 15), Status = PatientStatus.IntakePlanned },
                new Patient { Id = 19, FirstName = "Ruben", LastName = "Smeets", BirthDate = new DateTime(1987, 2, 2), Phone = "0699001122", AssignedDoctorUserId = 7, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 5, 20), Status = PatientStatus.OnHold },
                new Patient { Id = 20, FirstName = "Sarah", LastName = "Koks", BirthDate = new DateTime(1994, 8, 18), Phone = "0610203040", AssignedDoctorUserId = 6, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 5, 25), Status = PatientStatus.Active },
                new Patient { Id = 21, FirstName = "Tim", LastName = "Visser", BirthDate = new DateTime(1968, 10, 25), Phone = "0620304050", AssignedDoctorUserId = 5, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 5, 28), Status = PatientStatus.Completed },
                new Patient { Id = 22, FirstName = "Ursula", LastName = "Bouwman", BirthDate = new DateTime(1981, 6, 7), Phone = "0630405060", AssignedDoctorUserId = 7, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 5, 30), Status = PatientStatus.Active },
                new Patient { Id = 23, FirstName = "Vincent", LastName = "Meijerink", BirthDate = new DateTime(1996, 3, 29), Phone = "0640506070", AssignedDoctorUserId = 6, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 6, 1), Status = PatientStatus.IntakePlanned },
                new Patient { Id = 24, FirstName = "Wendy", LastName = "van der Velde", BirthDate = new DateTime(1979, 1, 13), Phone = "0650607080", AssignedDoctorUserId = 5, ReferringDoctorUserId = 4, StartDate = new DateTime(2025, 6, 5), Status = PatientStatus.Active },
                new Patient { Id = 25, FirstName = "Xavier", LastName = "De Ruiter", BirthDate = new DateTime(1986, 5, 5), Phone = "0660708090", AssignedDoctorUserId = 7, ReferringDoctorUserId = 3, StartDate = new DateTime(2025, 6, 10), Status = PatientStatus.Completed }
            );

            // 3. Oefeningen
            modelBuilder.Entity<Exercise>().HasData(
                new Exercise { Id = 1, Name = "Kniebuiging", Description = "Langzaam knieën buigen tot 90 graden rug recht houden.", VideoUrl = "https://www.youtube.com/watch?v=00g8DINXtIY" },
                new Exercise { Id = 2, Name = "Hamstring Stretch", Description = "Zittend één been gestrekt", VideoUrl = "https://www.youtube.com/shorts/ywgDXzYD6v8" },
                new Exercise { Id = 3, Name = "Calf Raises", Description = "Op de tenen staan en langzaam zakken.", VideoUrl = "https://www.youtube.com/watch?v=eMTy3qylqnE" },
                new Exercise { Id = 4, Name = "Core Stability Plank", Description = "Plankpositie rug recht", VideoUrl = "https://www.youtube.com/watch?v=pvIjsG5Svck" },
                new Exercise { Id = 5, Name = "Schouder Rotaties", Description = "Cirkelbewegingen met de armen.", VideoUrl = "https://www.youtube.com/watch?v=oLwTC-lAJws" }
            );

            // 4. Oefenplannen
            modelBuilder.Entity<ExerciseAssignment>().HasData(
                new ExerciseAssignment { Id = 1, PatientId = 1, ExerciseId = 1, Repetitions = 10, Sets = 3, Frequency = "3", Notes = "Focus op stabiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 13, 18, 0, 0) },
                new ExerciseAssignment { Id = 2, PatientId = 1, ExerciseId = 2, Repetitions = 15, Sets = 3, Frequency = "3", Notes = "Hamstring flexibiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 13, 9, 15, 0) },
                new ExerciseAssignment { Id = 3, PatientId = 1, ExerciseId = 3, Repetitions = 12, Sets = 3, Frequency = "3", Notes = "Kuitspierversterking", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 14, 9, 30, 0) },
                new ExerciseAssignment { Id = 4, PatientId = 1, ExerciseId = 4, Repetitions = 1, Sets = 3, Frequency = "1", Notes = "Houd 30 sec vast", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 23, 10, 0, 0) },
                new ExerciseAssignment { Id = 5, PatientId = 1, ExerciseId = 5, Repetitions = 12, Sets = 4, Frequency = "2", Notes = "Verhoging intensiteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 3, 9, 0, 0) },
                new ExerciseAssignment { Id = 6, PatientId = 2, ExerciseId = 1, Repetitions = 20, Sets = 3, Frequency = "3", Notes = "Verdieping stretch", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 3, 18, 0, 0) },
                new ExerciseAssignment { Id = 7, PatientId = 1, ExerciseId = 5, Repetitions = 10, Sets = 3, Frequency = "3", Notes = "Lichte schouderoefeningen", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 3, 9, 15, 0) },
                new ExerciseAssignment { Id = 8, PatientId = 1, ExerciseId = 3, Repetitions = 15, Sets = 4, Frequency = "2", Notes = "Laatste fase knieherstel", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 5, 9, 0, 0) },
                new ExerciseAssignment { Id = 9, PatientId = 1, ExerciseId = 4, Repetitions = 1, Sets = 4, Frequency = "1", Notes = "Core versterking gevorderd", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 4, 6, 9, 0, 0) },
                new ExerciseAssignment { Id = 10, PatientId = 1, ExerciseId = 3, Repetitions = 15, Sets = 3, Frequency = "3", Notes = "Meer explosieve kuitbewegingen", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 4, 11, 9, 0, 0) },
                new ExerciseAssignment { Id = 11, PatientId = 2, ExerciseId = 1, Repetitions = 8, Sets = 3, Frequency = "3", Notes = "Voorzichtige beweging", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 4, 9, 0, 0) },
                new ExerciseAssignment { Id = 12, PatientId = 2, ExerciseId = 4, Repetitions = 1, Sets = 2, Frequency = "2", Notes = "Houd 20 sec vast", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 4, 10, 0, 0) },
                new ExerciseAssignment { Id = 13, PatientId = 2, ExerciseId = 2, Repetitions = 10, Sets = 3, Frequency = "1", Notes = "Verbeteren flexibiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 2, 9, 0, 0) },
                new ExerciseAssignment { Id = 14, PatientId = 2, ExerciseId = 1, Repetitions = 10, Sets = 3, Frequency = "2", Notes = "Verhoging belasting", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 4, 3, 9, 0, 0) },
                new ExerciseAssignment { Id = 15, PatientId = 2, ExerciseId = 3, Repetitions = 10, Sets = 2, Frequency = "2", Notes = "Enkelstabiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 4, 16, 9, 0, 0) },
                new ExerciseAssignment { Id = 16, PatientId = 3, ExerciseId = 5, Repetitions = 15, Sets = 3, Frequency = "3", Notes = "Focus op rotatie", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 18, 9, 0, 0) },
                new ExerciseAssignment { Id = 17, PatientId = 3, ExerciseId = 1, Repetitions = 10, Sets = 2, Frequency = "1", Notes = "Algemene conditie", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 2, 9, 0, 0) },
                new ExerciseAssignment { Id = 18, PatientId = 3, ExerciseId = 5, Repetitions = 12, Sets = 3, Frequency = "2", Notes = "Schouderflexibiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 6, 9, 0, 0) },
                new ExerciseAssignment { Id = 19, PatientId = 4, ExerciseId = 4, Repetitions = 1, Sets = 3, Frequency = "1", Notes = "Core stabiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 4, 2, 9, 0, 0) },
                new ExerciseAssignment { Id = 20, PatientId = 4, ExerciseId = 3, Repetitions = 12, Sets = 3, Frequency = "2", Notes = "Enkelversterking", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 8, 9, 0, 0) },
                new ExerciseAssignment { Id = 21, PatientId = 4, ExerciseId = 1, Repetitions = 8, Sets = 2, Frequency = "1", Notes = "Lichte knieoefeningen", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 8, 10, 0, 0) },
                new ExerciseAssignment { Id = 22, PatientId = 4, ExerciseId = 2, Repetitions = 10, Sets = 3, Frequency = "3", Notes = "Hamstring stretch balans", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 4, 3, 9, 0, 0) },
                new ExerciseAssignment { Id = 23, PatientId = 4, ExerciseId = 5, Repetitions = 10, Sets = 3, Frequency = "1", Notes = "Schouder mobiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 4, 11, 9, 0, 0) },
                new ExerciseAssignment { Id = 24, PatientId = 5, ExerciseId = 1, Repetitions = 8, Sets = 3, Frequency = "2", Notes = "Nek- en rugoefeningen", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 4, 9, 0, 0) },
                new ExerciseAssignment { Id = 25, PatientId = 5, ExerciseId = 4, Repetitions = 1, Sets = 2, Frequency = "1", Notes = "Nekstabiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 4, 10, 0, 0) },
                new ExerciseAssignment { Id = 26, PatientId = 5, ExerciseId = 2, Repetitions = 10, Sets = 3, Frequency = "1", Notes = "Arm- en schouderstretch", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 4, 6, 9, 0, 0) },
                new ExerciseAssignment { Id = 27, PatientId = 6, ExerciseId = 1, Repetitions = 10, Sets = 3, Frequency = "2", Notes = "Rugversterking", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 13, 9, 0, 0) },
                new ExerciseAssignment { Id = 28, PatientId = 6, ExerciseId = 2, Repetitions = 10, Sets = 3, Frequency = "1", Notes = "Onderrug stretch", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 13, 10, 0, 0) },
                new ExerciseAssignment { Id = 29, PatientId = 6, ExerciseId = 4, Repetitions = 1, Sets = 3, Frequency = "1", Notes = "Core voor rug", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 2, 9, 0, 0) },
                new ExerciseAssignment { Id = 30, PatientId = 7, ExerciseId = 1, Repetitions = 10, Sets = 3, Frequency = "2", Notes = "Kniebuiging lichte belasting", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 8, 9, 0, 0) },
                new ExerciseAssignment { Id = 31, PatientId = 7, ExerciseId = 2, Repetitions = 15, Sets = 3, Frequency = "1", Notes = "Hamstring flexibiliteit knie", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 8, 10, 0, 0) },
                new ExerciseAssignment { Id = 32, PatientId = 7, ExerciseId = 3, Repetitions = 12, Sets = 3, Frequency = "2", Notes = "Kuitspieren en balans", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 3, 21, 9, 0, 0) },
                new ExerciseAssignment { Id = 33, PatientId = 8, ExerciseId = 4, Repetitions = 1, Sets = 2, Frequency = "1", Notes = "Algemene core stability", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 4, 16, 9, 0, 0) },
                new ExerciseAssignment { Id = 34, PatientId = 8, ExerciseId = 5, Repetitions = 10, Sets = 3, Frequency = "1", Notes = "Lichte mobiliteitsoefeningen", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 15, 10, 0, 0) },
                new ExerciseAssignment { Id = 35, PatientId = 9, ExerciseId = 1, Repetitions = 10, Sets = 3, Frequency = "2", Notes = "Algemene krachtopbouw", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 16, 10, 0, 0) },
                new ExerciseAssignment { Id = 36, PatientId = 9, ExerciseId = 3, Repetitions = 12, Sets = 3, Frequency = "1", Notes = "Kuit- en enkelstabiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 17, 10, 0, 0) },
                new ExerciseAssignment { Id = 37, PatientId = 10, ExerciseId = 1, Repetitions = 8, Sets = 3, Frequency = "2", Notes = "Lichte kniebuigingen", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 18, 10, 0, 0) },
                new ExerciseAssignment { Id = 38, PatientId = 10, ExerciseId = 2, Repetitions = 10, Sets = 3, Frequency = "1", Notes = "Stretch voor mobiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 19, 10, 0, 0) },
                new ExerciseAssignment { Id = 39, PatientId = 11, ExerciseId = 4, Repetitions = 1, Sets = 3, Frequency = "1", Notes = "Core stability oefeningen", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 20, 10, 0, 0) },
                new ExerciseAssignment { Id = 40, PatientId = 11, ExerciseId = 5, Repetitions = 10, Sets = 2, Frequency = "1", Notes = "Schouder en nek mobiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 21, 10, 0, 0) },
                new ExerciseAssignment { Id = 41, PatientId = 12, ExerciseId = 1, Repetitions = 10, Sets = 3, Frequency = "2", Notes = "Lichte belasting oefeningen", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 22, 10, 0, 0) },
                new ExerciseAssignment { Id = 42, PatientId = 12, ExerciseId = 2, Repetitions = 15, Sets = 3, Frequency = "1", Notes = "Flexibiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 23, 10, 0, 0) },
                new ExerciseAssignment { Id = 43, PatientId = 13, ExerciseId = 1, Repetitions = 8, Sets = 3, Frequency = "2", Notes = "Knieversterking", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 24, 10, 0, 0) },
                new ExerciseAssignment { Id = 44, PatientId = 13, ExerciseId = 3, Repetitions = 12, Sets = 3, Frequency = "1", Notes = "Kuitspier activatie", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 25, 10, 0, 0) },
                new ExerciseAssignment { Id = 45, PatientId = 14, ExerciseId = 2, Repetitions = 10, Sets = 3, Frequency = "2", Notes = "Algemene stretch", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 26, 10, 0, 0) },
                new ExerciseAssignment { Id = 46, PatientId = 14, ExerciseId = 4, Repetitions = 1, Sets = 3, Frequency = "1", Notes = "Core stability algemeen", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 27, 10, 0, 0) },
                new ExerciseAssignment { Id = 47, PatientId = 15, ExerciseId = 1, Repetitions = 10, Sets = 3, Frequency = "2", Notes = "Start herstel", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 28, 10, 0, 0) },
                new ExerciseAssignment { Id = 48, PatientId = 15, ExerciseId = 2, Repetitions = 15, Sets = 3, Frequency = "1", Notes = "Flexibiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 29, 10, 0, 0) },
                new ExerciseAssignment { Id = 49, PatientId = 16, ExerciseId = 3, Repetitions = 12, Sets = 3, Frequency = "2", Notes = "Kuit en enkel", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 30, 10, 0, 0) },
                new ExerciseAssignment { Id = 50, PatientId = 16, ExerciseId = 4, Repetitions = 1, Sets = 3, Frequency = "1", Notes = "Core training", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 1, 31, 10, 0, 0) },
                new ExerciseAssignment { Id = 51, PatientId = 17, ExerciseId = 1, Repetitions = 10, Sets = 3, Frequency = "2", Notes = "Lichte kracht", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 1, 10, 0, 0) },
                new ExerciseAssignment { Id = 52, PatientId = 17, ExerciseId = 5, Repetitions = 10, Sets = 3, Frequency = "1", Notes = "Schouder mobiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 2, 10, 0, 0) },
                new ExerciseAssignment { Id = 53, PatientId = 18, ExerciseId = 2, Repetitions = 15, Sets = 3, Frequency = "2", Notes = "Algemene stretch", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 3, 10, 0, 0) },
                new ExerciseAssignment { Id = 54, PatientId = 18, ExerciseId = 3, Repetitions = 12, Sets = 3, Frequency = "1", Notes = "Kuit en balans", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 4, 10, 0, 0) },
                new ExerciseAssignment { Id = 55, PatientId = 19, ExerciseId = 4, Repetitions = 1, Sets = 3, Frequency = "1", Notes = "Core stability", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 5, 10, 0, 0) },
                new ExerciseAssignment { Id = 56, PatientId = 19, ExerciseId = 1, Repetitions = 10, Sets = 3, Frequency = "2", Notes = "Lichte knie", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 6, 10, 0, 0) },
                new ExerciseAssignment { Id = 57, PatientId = 20, ExerciseId = 5, Repetitions = 10, Sets = 3, Frequency = "1", Notes = "Schouder en nek", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 7, 10, 0, 0) },
                new ExerciseAssignment { Id = 58, PatientId = 20, ExerciseId = 2, Repetitions = 15, Sets = 3, Frequency = "2", Notes = "Hamstring", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 8, 10, 0, 0) },
                new ExerciseAssignment { Id = 59, PatientId = 21, ExerciseId = 3, Repetitions = 12, Sets = 3, Frequency = "1", Notes = "Kuitversterking", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 9, 10, 0, 0) },
                new ExerciseAssignment { Id = 60, PatientId = 21, ExerciseId = 1, Repetitions = 10, Sets = 3, Frequency = "2", Notes = "Kniebuigingen", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 10, 10, 0, 0) },
                new ExerciseAssignment { Id = 61, PatientId = 22, ExerciseId = 4, Repetitions = 1, Sets = 3, Frequency = "1", Notes = "Core oefeningen", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 11, 10, 0, 0) },
                new ExerciseAssignment { Id = 62, PatientId = 22, ExerciseId = 2, Repetitions = 10, Sets = 3, Frequency = "2", Notes = "Flexibiliteit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 12, 10, 0, 0) },
                new ExerciseAssignment { Id = 63, PatientId = 23, ExerciseId = 5, Repetitions = 10, Sets = 3, Frequency = "1", Notes = "Schouder", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 13, 10, 0, 0) },
                new ExerciseAssignment { Id = 64, PatientId = 23, ExerciseId = 1, Repetitions = 10, Sets = 3, Frequency = "2", Notes = "Knie", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 14, 10, 0, 0) },
                new ExerciseAssignment { Id = 65, PatientId = 24, ExerciseId = 2, Repetitions = 15, Sets = 3, Frequency = "2", Notes = "Hamstring", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 15, 10, 0, 0) },
                new ExerciseAssignment { Id = 66, PatientId = 24, ExerciseId = 3, Repetitions = 12, Sets = 3, Frequency = "1", Notes = "Kuit", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 16, 10, 0, 0) },
                new ExerciseAssignment { Id = 67, PatientId = 25, ExerciseId = 4, Repetitions = 1, Sets = 3, Frequency = "1", Notes = "Core", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 17, 10, 0, 0) },
                new ExerciseAssignment { Id = 68, PatientId = 25, ExerciseId = 5, Repetitions = 10, Sets = 3, Frequency = "3", Notes = "Schouder", ClientCheckedOff = true, CheckedOffAt = new DateTime(2025, 2, 18, 10, 0, 0) }
            );

            // 5. Pijnindicaties
            modelBuilder.Entity<PainEntry>().HasData(
                new PainEntry { Id = 1, PatientId = 1, Timestamp = new DateTime(2025, 1, 10, 10, 0, 0), Score = 7, Note = "Na de operatie" },
                new PainEntry { Id = 2, PatientId = 1, Timestamp = new DateTime(2025, 1, 11, 09, 0, 0), Score = 6, Note = "lets minder pijn" },
                new PainEntry { Id = 3, PatientId = 1, Timestamp = new DateTime(2025, 1, 12, 09, 30, 0), Score = 5, Note = "Pijn bij bewegen" },
                new PainEntry { Id = 4, PatientId = 1, Timestamp = new DateTime(2025, 1, 13, 09, 0, 0), Score = 5, Note = "Tijdens oefeningen wat meer pijn" },
                new PainEntry { Id = 5, PatientId = 1, Timestamp = new DateTime(2025, 1, 14, 09, 0, 0), Score = 4, Note = "Begin van de dag goed" },
                new PainEntry { Id = 6, PatientId = 1, Timestamp = new DateTime(2025, 1, 15, 09, 0, 0), Score = 4, Note = "Pijn nam licht toe na intensievere oefening" },
                new PainEntry { Id = 7, PatientId = 1, Timestamp = new DateTime(2025, 1, 16, 09, 0, 0), Score = 3, Note = "Gaat steeds beter" },
                new PainEntry { Id = 8, PatientId = 1, Timestamp = new DateTime(2025, 1, 17, 09, 0, 0), Score = 3, Note = "Alleen nog lichte pijn bij langdurig staan" },
                new PainEntry { Id = 9, PatientId = 1, Timestamp = new DateTime(2025, 1, 18, 09, 0, 0), Score = 2, Note = "Bijna pijnvrij in rust" },
                new PainEntry { Id = 10, PatientId = 1, Timestamp = new DateTime(2025, 1, 19, 09, 0, 0), Score = 2, Note = "Soms een steek bij onverwachte beweging" },
                new PainEntry { Id = 11, PatientId = 2, Timestamp = new DateTime(2025, 2, 1, 11, 0, 0), Score = 8, Note = "Erge uitstralende pijn in been" },
                new PainEntry { Id = 12, PatientId = 2, Timestamp = new DateTime(2025, 2, 2, 10, 0, 0), Score = 7, Note = "Medicatie helpt iets" },
                new PainEntry { Id = 13, PatientId = 2, Timestamp = new DateTime(2025, 2, 3, 09, 0, 0), Score = 6, Note = "Na fysiotherapie beter" },
                new PainEntry { Id = 14, PatientId = 2, Timestamp = new DateTime(2025, 2, 4, 09, 0, 0), Score = 6, Note = "Pijn blijft hoog na lange zit" },
                new PainEntry { Id = 15, PatientId = 2, Timestamp = new DateTime(2025, 2, 5, 09, 0, 0), Score = 5, Note = "Iets beter na meer rust" },
                new PainEntry { Id = 16, PatientId = 3, Timestamp = new DateTime(2025, 1, 15, 14, 0, 0), Score = 6, Note = "Zeurende pijn in schouder" },
                new PainEntry { Id = 17, PatientId = 3, Timestamp = new DateTime(2025, 1, 16, 10, 0, 0), Score = 5, Note = "Pijn bij bovenhandse bewegingen" },
                new PainEntry { Id = 18, PatientId = 4, Timestamp = new DateTime(2025, 2, 5, 13, 0, 0), Score = 7, Note = "Flink gezwollen enkel" },
                new PainEntry { Id = 19, PatientId = 4, Timestamp = new DateTime(2025, 2, 6, 09, 0, 0), Score = 6, Note = "Minder zwelling" },
                new PainEntry { Id = 20, PatientId = 5, Timestamp = new DateTime(2025, 3, 1, 16, 0, 0), Score = 9, Note = "Ondraaglijke nekpijn en tintelingen" },
                new PainEntry { Id = 21, PatientId = 5, Timestamp = new DateTime(2025, 3, 2, 09, 0, 0), Score = 8, Note = "Medicatie helpt" }
            );

            // 6. Activiteitenlogboeken
            modelBuilder.Entity<ActivityLogEntry>().HasData(
                new ActivityLogEntry { Id = 1, PatientId = 1, Timestamp = new DateTime(2025, 1, 10, 12, 0, 0), Activity = "Rustdag na operatie." },
                new ActivityLogEntry { Id = 2, PatientId = 1, Timestamp = new DateTime(2025, 1, 11, 15, 0, 0), Activity = "Korte wandeling met krukken rondom huis." },
                new ActivityLogEntry { Id = 3, PatientId = 1, Timestamp = new DateTime(2025, 1, 12, 10, 0, 0), Activity = "Huishoudelijke taken (afwas)." },
                new ActivityLogEntry { Id = 4, PatientId = 1, Timestamp = new DateTime(2025, 1, 12, 16, 0, 0), Activity = "Boodschappen gedaan met hulp." },
                new ActivityLogEntry { Id = 5, PatientId = 1, Timestamp = new DateTime(2025, 1, 13, 11, 0, 0), Activity = "30 minuten fysiotherapie oefeningen gedaan." },
                new ActivityLogEntry { Id = 6, PatientId = 1, Timestamp = new DateTime(2025, 1, 13, 17, 0, 0), Activity = "Korte fietstocht op hometrainer (15 min)." },
                new ActivityLogEntry { Id = 7, PatientId = 1, Timestamp = new DateTime(2025, 1, 14, 09, 0, 0), Activity = "Oefeningen volgens schema uitgevoerd." },
                new ActivityLogEntry { Id = 8, PatientId = 1, Timestamp = new DateTime(2025, 1, 14, 14, 0, 0), Activity = "Koken en voorbereiden maaltijden." },
                new ActivityLogEntry { Id = 9, PatientId = 1, Timestamp = new DateTime(2025, 1, 15, 10, 0, 0), Activity = "Oefeningen gedaan" },
                new ActivityLogEntry { Id = 10, PatientId = 1, Timestamp = new DateTime(2025, 1, 15, 16, 0, 0), Activity = "Wandeling buiten" },
                new ActivityLogEntry { Id = 11, PatientId = 2, Timestamp = new DateTime(2025, 2, 1, 14, 0, 0), Activity = "Eerste lichte oefeningen voor rug." },
                new ActivityLogEntry { Id = 12, PatientId = 2, Timestamp = new DateTime(2025, 2, 2, 09, 0, 0), Activity = "Half uur gefietst op hometrainer." },
                new ActivityLogEntry { Id = 13, PatientId = 2, Timestamp = new DateTime(2025, 2, 2, 18, 0, 0), Activity = "Korte wandeling buiten" },
                new ActivityLogEntry { Id = 14, PatientId = 2, Timestamp = new DateTime(2025, 2, 3, 10, 0, 0), Activity = "Rug oefeningen gedaan" },
                new ActivityLogEntry { Id = 15, PatientId = 2, Timestamp = new DateTime(2025, 2, 3, 15, 0, 0), Activity = "Wandeling met hond." },
                new ActivityLogEntry { Id = 16, PatientId = 3, Timestamp = new DateTime(2025, 1, 16, 09, 0, 0), Activity = "Oefeningen voor schouder mobiliteit." },
                new ActivityLogEntry { Id = 17, PatientId = 3, Timestamp = new DateTime(2025, 1, 17, 11, 0, 0), Activity = "Lichte huishoudelijke taken." },
                new ActivityLogEntry { Id = 18, PatientId = 4, Timestamp = new DateTime(2025, 2, 6, 10, 0, 0), Activity = "Enkel oefeningen volgens schema." },
                new ActivityLogEntry { Id = 19, PatientId = 4, Timestamp = new DateTime(2025, 2, 7, 14, 0, 0), Activity = "Korte wandeling binnenshuis." },
                new ActivityLogEntry { Id = 20, PatientId = 5, Timestamp = new DateTime(2025, 3, 3, 10, 0, 0), Activity = "Nekoefeningen met elastiek." },
                new ActivityLogEntry { Id = 21, PatientId = 5, Timestamp = new DateTime(2025, 3, 4, 15, 0, 0), Activity = "Rustig gewandeld." }
        );

            // 7. Medicatie
            modelBuilder.Entity<Medication>().HasData(
                new Medication { Id = 1, PatientId = 1, HuisartsId = 3, Name = "Amoxicilline", Dosage = "500mg", Frequency = "3x daags", StartDate = new DateTime(2025, 1, 8), EndDate = new DateTime(2025, 1, 18), Status = "Afgerond" },
                new Medication { Id = 2, PatientId = 1, HuisartsId = 3, Name = "Paracetamol", Dosage = "500mg", Frequency = "4x daags max", StartDate = new DateTime(2025, 1, 8), EndDate = new DateTime(2025, 2, 1), Status = "Afgerond" },
                new Medication { Id = 3, PatientId = 2, HuisartsId = 4, Name = "Diclofenac", Dosage = "50mg", Frequency = "2x daags", StartDate = new DateTime(2025, 2, 1), EndDate = new DateTime(2025, 3, 1), Status = "Afgerond" },
                new Medication { Id = 4, PatientId = 3, HuisartsId = 3, Name = "Ibuprofen", Dosage = "400mg", Frequency = "3x daags", StartDate = new DateTime(2025, 1, 15), EndDate = new DateTime(2025, 2, 15), Status = "Afgerond" },
                new Medication { Id = 5, PatientId = 4, HuisartsId = 4, Name = "Naproxen", Dosage = "250mg", Frequency = "2x daags", StartDate = new DateTime(2025, 2, 5), EndDate = new DateTime(2025, 3, 5), Status = "Afgerond" },
                new Medication { Id = 6, PatientId = 5, HuisartsId = 3, Name = "Tramadol", Dosage = "50mg", Frequency = "1x daags", StartDate = new DateTime(2025, 2, 3), EndDate = new DateTime(2025, 4, 1), Status = "Actief" },
                new Medication { Id = 7, PatientId = 6, HuisartsId = 4, Name = "Fentanyl pleister", Dosage = "12mcg/u", Frequency = "Elke 72 uur", StartDate = new DateTime(2025, 2, 10), EndDate = new DateTime(2025, 5, 1), Status = "Actief" },
                new Medication { Id = 8, PatientId = 7, HuisartsId = 3, Name = "Pijnstiller X", Dosage = "10mg", Frequency = "3x daags", StartDate = new DateTime(2025, 3, 6), EndDate = new DateTime(2025, 4, 6), Status = "Afgerond" },
                new Medication { Id = 9, PatientId = 8, HuisartsId = 4, Name = "Lyrica", Dosage = "75mg", Frequency = "2x daags", StartDate = new DateTime(2025, 2, 4), EndDate = new DateTime(2025, 6, 2), Status = "Actief" },
                new Medication { Id = 10, PatientId = 9, HuisartsId = 3, Name = "Corticosteroïden", Dosage = "10mg", Frequency = "1x daags", StartDate = new DateTime(2025, 3, 11), EndDate = new DateTime(2025, 3, 25), Status = "Afgerond" },
                new Medication { Id = 11, PatientId = 10, HuisartsId = 4, Name = "Diclofenac gel", Dosage = "Topisch", Frequency = "2x daags", StartDate = new DateTime(2025, 4, 6), EndDate = new DateTime(2025, 5, 6), Status = "Actief" },
                new Medication { Id = 12, PatientId = 11, HuisartsId = 3, Name = "Spierverslapper Y", Dosage = "5mg", Frequency = "1x daags", StartDate = new DateTime(2025, 4, 11), EndDate = new DateTime(2025, 5, 11), Status = "Actief" },
                new Medication { Id = 13, PatientId = 12, HuisartsId = 4, Name = "Pijnstiller Z", Dosage = "20mg", Frequency = "2x daags", StartDate = new DateTime(2025, 4, 16), EndDate = new DateTime(2025, 5, 16), Status = "Actief" },
                new Medication { Id = 14, PatientId = 13, HuisartsId = 3, Name = "Tramadol", Dosage = "50mg", Frequency = "1x daags", StartDate = new DateTime(2025, 4, 21), EndDate = new DateTime(2025, 5, 21), Status = "Actief" },
                new Medication { Id = 15, PatientId = 14, HuisartsId = 4, Name = "Paracetamol", Dosage = "500mg", Frequency = "4x daags", StartDate = new DateTime(2025, 4, 26), EndDate = new DateTime(2025, 5, 26), Status = "Actief" },
                new Medication { Id = 16, PatientId = 15, HuisartsId = 3, Name = "Ibuprofen", Dosage = "400mg", Frequency = "3x daags", StartDate = new DateTime(2025, 5, 2), EndDate = new DateTime(2025, 6, 2), Status = "Actief" },
                new Medication { Id = 17, PatientId = 16, HuisartsId = 4, Name = "Naproxen", Dosage = "250mg", Frequency = "2x daags", StartDate = new DateTime(2025, 5, 6), EndDate = new DateTime(2025, 6, 6), Status = "Actief" },
                new Medication { Id = 18, PatientId = 17, HuisartsId = 3, Name = "Codeine", Dosage = "30mg", Frequency = "2x daags", StartDate = new DateTime(2025, 5, 11), EndDate = new DateTime(2025, 6, 11), Status = "Actief" },
                new Medication { Id = 19, PatientId = 18, HuisartsId = 4, Name = "Paracetamol", Dosage = "500mg", Frequency = "4x daags", StartDate = new DateTime(2025, 5, 16), EndDate = new DateTime(2025, 6, 16), Status = "Actief" },
                new Medication { Id = 20, PatientId = 19, HuisartsId = 3, Name = "Diclofenac", Dosage = "50mg", Frequency = "2x daags", StartDate = new DateTime(2025, 5, 21), EndDate = new DateTime(2025, 6, 21), Status = "Actief" },
                new Medication { Id = 21, PatientId = 20, HuisartsId = 4, Name = "Ibuprofen", Dosage = "400mg", Frequency = "3x daags", StartDate = new DateTime(2025, 5, 26), EndDate = new DateTime(2025, 6, 26), Status = "Actief" },
                new Medication { Id = 22, PatientId = 21, HuisartsId = 3, Name = "Paracetamol", Dosage = "500mg", Frequency = "4x daags", StartDate = new DateTime(2025, 5, 29), EndDate = new DateTime(2025, 6, 29), Status = "Actief" },
                new Medication { Id = 23, PatientId = 22, HuisartsId = 4, Name = "Naproxen", Dosage = "250mg", Frequency = "2x daags", StartDate = new DateTime(2025, 5, 31), EndDate = new DateTime(2025, 6, 30), Status = "Actief" },
                new Medication { Id = 24, PatientId = 23, HuisartsId = 3, Name = "Tramadol", Dosage = "50mg", Frequency = "1x daags", StartDate = new DateTime(2025, 6, 2), EndDate = new DateTime(2025, 7, 2), Status = "Actief" },
                new Medication { Id = 25, PatientId = 24, HuisartsId = 4, Name = "Pijnstiller A", Dosage = "10mg", Frequency = "3x daags", StartDate = new DateTime(2025, 6, 6), EndDate = new DateTime(2025, 7, 6), Status = "Actief" }
            );

            // 8. Accessoires
            modelBuilder.Entity<AccessoryAdvice>().HasData(
                new AccessoryAdvice { Id = 1, PatientId = 1, HuisartsId = 3, Name = "Krukken", AdviceDate = new DateTime(2025, 1, 9), ExpectedUsagePeriod = "6 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 2, PatientId = 1, HuisartsId = 3, Name = "Kniebrace", AdviceDate = new DateTime(2025, 1, 15), ExpectedUsagePeriod = "12 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 3, PatientId = 2, HuisartsId = 4, Name = "Rugbrace", AdviceDate = new DateTime(2025, 2, 1), ExpectedUsagePeriod = "8 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 4, PatientId = 3, HuisartsId = 3, Name = "Schouderbrace", AdviceDate = new DateTime(2025, 1, 16), ExpectedUsagePeriod = "4 weken", Status = "Afgerond" },
                new AccessoryAdvice { Id = 5, PatientId = 4, HuisartsId = 4, Name = "Enkelbrace", AdviceDate = new DateTime(2025, 2, 6), ExpectedUsagePeriod = "6 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 6, PatientId = 5, HuisartsId = 3, Name = "Nekbrace", AdviceDate = new DateTime(2025, 3, 3), ExpectedUsagePeriod = "8 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 7, PatientId = 6, HuisartsId = 4, Name = "Fysiotherapie banden", AdviceDate = new DateTime(2025, 2, 12), ExpectedUsagePeriod = "Onbepaald", Status = "Actief" },
                new AccessoryAdvice { Id = 8, PatientId = 7, HuisartsId = 3, Name = "Meniscusbrace", AdviceDate = new DateTime(2025, 3, 7), ExpectedUsagePeriod = "10 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 9, PatientId = 8, HuisartsId = 4, Name = "TENS apparaat", AdviceDate = new DateTime(2025, 3, 4), ExpectedUsagePeriod = "Onbepaald", Status = "Actief" },
                new AccessoryAdvice { Id = 10, PatientId = 9, HuisartsId = 3, Name = "Polsbrace", AdviceDate = new DateTime(2025, 3, 12), ExpectedUsagePeriod = "4 weken", Status = "Afgerond" },
                new AccessoryAdvice { Id = 11, PatientId = 10, HuisartsId = 4, Name = "Kniebrace", AdviceDate = new DateTime(2025, 4, 7), ExpectedUsagePeriod = "8 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 12, PatientId = 11, HuisartsId = 3, Name = "Rugkussen", AdviceDate = new DateTime(2025, 4, 11), ExpectedUsagePeriod = "Onbepaald", Status = "Actief" },
                new AccessoryAdvice { Id = 13, PatientId = 12, HuisartsId = 4, Name = "Enkelgewichtjes", AdviceDate = new DateTime(2025, 4, 16), ExpectedUsagePeriod = "6 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 14, PatientId = 13, HuisartsId = 3, Name = "Looprek", AdviceDate = new DateTime(2025, 4, 21), ExpectedUsagePeriod = "12 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 15, PatientId = 14, HuisartsId = 4, Name = "Schouderkatrol", AdviceDate = new DateTime(2025, 4, 26), ExpectedUsagePeriod = "8 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 16, PatientId = 15, HuisartsId = 3, Name = "Knieband", AdviceDate = new DateTime(2025, 5, 2), ExpectedUsagePeriod = "6 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 17, PatientId = 16, HuisartsId = 4, Name = "Enkelfles", AdviceDate = new DateTime(2025, 5, 6), ExpectedUsagePeriod = "Onbepaald", Status = "Actief" },
                new AccessoryAdvice { Id = 18, PatientId = 17, HuisartsId = 3, Name = "Krukken", AdviceDate = new DateTime(2025, 5, 11), ExpectedUsagePeriod = "4 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 19, PatientId = 18, HuisartsId = 4, Name = "Rugbrace", AdviceDate = new DateTime(2025, 5, 16), ExpectedUsagePeriod = "8 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 20, PatientId = 19, HuisartsId = 3, Name = "Schouderriem", AdviceDate = new DateTime(2025, 5, 21), ExpectedUsagePeriod = "6 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 21, PatientId = 20, HuisartsId = 4, Name = "Enkelondersteuning", AdviceDate = new DateTime(2025, 5, 26), ExpectedUsagePeriod = "4 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 22, PatientId = 21, HuisartsId = 3, Name = "Kniebrace", AdviceDate = new DateTime(2025, 5, 29), ExpectedUsagePeriod = "10 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 23, PatientId = 22, HuisartsId = 4, Name = "Hielspoorzooltjes", AdviceDate = new DateTime(2025, 5, 31), ExpectedUsagePeriod = "Onbepaald", Status = "Actief" },
                new AccessoryAdvice { Id = 24, PatientId = 23, HuisartsId = 3, Name = "Rolstoel", AdviceDate = new DateTime(2025, 6, 2), ExpectedUsagePeriod = "12 weken", Status = "Actief" },
                new AccessoryAdvice { Id = 25, PatientId = 24, HuisartsId = 4, Name = "Handtrainer", AdviceDate = new DateTime(2025, 6, 6), ExpectedUsagePeriod = "Onbepaald", Status = "Actief" }
            );

            // 9. Afspraken
            modelBuilder.Entity<Appointment>().HasData(
                new Appointment { Id = 1, PatientId = 1, DoctorId = 5, AppointmentDateTime = new DateTime(2025, 1, 10, 14, 0, 0), DurationMinutes = 60, Type = "Intake", Status = "Afgerond" },
                new Appointment { Id = 2, PatientId = 1, DoctorId = 5, AppointmentDateTime = new DateTime(2025, 1, 31, 10, 0, 0), DurationMinutes = 45, Type = "Controle", Status = "Afgerond" },
                new Appointment { Id = 3, PatientId = 1, DoctorId = 5, AppointmentDateTime = new DateTime(2025, 2, 21, 11, 0, 0), DurationMinutes = 45, Type = "Controle", Status = "Afgerond" },
                new Appointment { Id = 4, PatientId = 1, DoctorId = 5, AppointmentDateTime = new DateTime(2025, 3, 14, 13, 0, 0), DurationMinutes = 30, Type = "Controle", Status = "Afgerond" },
                new Appointment { Id = 5, PatientId = 1, DoctorId = 5, AppointmentDateTime = new DateTime(2025, 4, 4, 09, 0, 0), DurationMinutes = 30, Type = "Controle", Status = "Gepland" },
                new Appointment { Id = 6, PatientId = 1, DoctorId = 5, AppointmentDateTime = new DateTime(2025, 4, 25, 10, 0, 0), DurationMinutes = 30, Type = "Controle", Status = "Gepland" },
                new Appointment { Id = 7, PatientId = 2, DoctorId = 6, AppointmentDateTime = new DateTime(2025, 2, 1, 09, 0, 0), DurationMinutes = 60, Type = "Intake", Status = "Afgerond" },
                new Appointment { Id = 8, PatientId = 2, DoctorId = 6, AppointmentDateTime = new DateTime(2025, 2, 22, 14, 0, 0), DurationMinutes = 45, Type = "Controle", Status = "Afgerond" },
                new Appointment { Id = 9, PatientId = 2, DoctorId = 6, AppointmentDateTime = new DateTime(2025, 3, 15, 10, 0, 0), DurationMinutes = 45, Type = "Gepland", Status = "Gepland" },
                new Appointment { Id = 10, PatientId = 2, DoctorId = 6, AppointmentDateTime = new DateTime(2025, 4, 5, 11, 0, 0), DurationMinutes = 30, Type = "Controle", Status = "Gepland" },
                new Appointment { Id = 11, PatientId = 3, DoctorId = 5, AppointmentDateTime = new DateTime(2025, 1, 15, 16, 0, 0), DurationMinutes = 60, Type = "Intake", Status = "Afgerond" },
                new Appointment { Id = 12, PatientId = 3, DoctorId = 5, AppointmentDateTime = new DateTime(2025, 2, 5, 15, 0, 0), DurationMinutes = 45, Type = "Controle", Status = "Afgerond" },
                new Appointment { Id = 13, PatientId = 3, DoctorId = 5, AppointmentDateTime = new DateTime(2025, 2, 26, 14, 0, 0), DurationMinutes = 30, Type = "Controle", Status = "Gepland" },
                new Appointment { Id = 14, PatientId = 4, DoctorId = 7, AppointmentDateTime = new DateTime(2025, 2, 5, 10, 0, 0), DurationMinutes = 60, Type = "Intake", Status = "Afgerond" },
                new Appointment { Id = 15, PatientId = 4, DoctorId = 7, AppointmentDateTime = new DateTime(2025, 2, 26, 11, 0, 0), DurationMinutes = 45, Type = "Controle", Status = "Afgerond" },
                new Appointment { Id = 16, PatientId = 4, DoctorId = 7, AppointmentDateTime = new DateTime(2025, 3, 19, 13, 0, 0), DurationMinutes = 30, Type = "Controle", Status = "Gepland" },
                new Appointment { Id = 17, PatientId = 5, DoctorId = 6, AppointmentDateTime = new DateTime(2025, 3, 1, 09, 0, 0), DurationMinutes = 60, Type = "Intake", Status = "Afgerond" },
                new Appointment { Id = 18, PatientId = 5, DoctorId = 6, AppointmentDateTime = new DateTime(2025, 3, 22, 10, 0, 0), DurationMinutes = 45, Type = "Controle", Status = "Gepland" },
                new Appointment { Id = 19, PatientId = 6, DoctorId = 5, AppointmentDateTime = new DateTime(2025, 2, 10, 14, 0, 0), DurationMinutes = 60, Type = "Intake", Status = "Afgerond" },
                new Appointment { Id = 20, PatientId = 6, DoctorId = 5, AppointmentDateTime = new DateTime(2025, 3, 3, 15, 0, 0), DurationMinutes = 45, Type = "Controle", Status = "Gepland" }
            );
        }
    }
}
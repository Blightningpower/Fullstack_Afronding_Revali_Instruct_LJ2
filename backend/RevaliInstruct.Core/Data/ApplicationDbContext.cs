using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Core.Entities;
using RevaliInstruct.Core.Data.Configurations;
using RevaliInstruct.Core.Data.Enums;

namespace RevaliInstruct.Core.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Tabeldefinities
        public DbSet<User> Users => Set<User>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Exercise> Exercises => Set<Exercise>();
        public DbSet<ExerciseAssignment> ExerciseAssignments => Set<ExerciseAssignment>();
        public DbSet<PainEntry> PainEntries => Set<PainEntry>();
        public DbSet<ActivityLogEntry> ActivityLogs => Set<ActivityLogEntry>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<PatientNote> PatientNotes => Set<PatientNote>();
        public DbSet<AccessoryAdvice> AccessoryAdvices => Set<AccessoryAdvice>();
        public DbSet<IntakeRecord> IntakeRecords => Set<IntakeRecord>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            modelBuilder.Entity<Patient>()
                .HasMany(p => p.ActivityLogEntries)
                .WithOne()
                .HasForeignKey("PatientId");

            modelBuilder.Seed();
        }
    }
}
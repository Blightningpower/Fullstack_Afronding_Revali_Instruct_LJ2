using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Core.Entities;

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
        public DbSet<AccessoryAdvice> AccessoryAdvices => Set<AccessoryAdvice>();
        public DbSet<IntakeRecord> IntakeRecords => Set<IntakeRecord>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<Declaration> Declarations => Set<Declaration>();
        public DbSet<PatientNote> PatientNotes => Set<PatientNote>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

    modelBuilder.Entity<ActivityLogEntry>()
        .HasOne<Patient>()
        .WithMany(p => p.ActivityLogEntries)
        .HasForeignKey(a => a.PatientId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<AuditLog>()
        .HasOne<User>()
        .WithMany()
        .HasForeignKey(a => a.UserId);

    modelBuilder.Seed();
}
    }
}
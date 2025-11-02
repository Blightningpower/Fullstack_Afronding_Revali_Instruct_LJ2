using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Core.Entities;

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

            // User
            modelBuilder.Entity<User>(b =>
            {
                b.HasIndex(u => u.Username).IsUnique();
                b.Property(u => u.Role).HasMaxLength(50);
            });

            // Patient
            modelBuilder.Entity<Patient>(b =>
            {
                b.Property(p => p.Status).HasConversion<string>();
                b.HasOne(p => p.AssignedDoctor)
                    .WithMany(u => u.Patients)
                    .HasForeignKey(p => p.AssignedDoctorUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Intake (1-1)
            modelBuilder.Entity<IntakeRecord>(b =>
            {
                b.HasOne(i => i.Patient)
                    .WithOne(p => p.Intake)
                    .HasForeignKey<IntakeRecord>(i => i.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ExerciseAssignment -> Exercise
            modelBuilder.Entity<ExerciseAssignment>(b =>
            {
                b.HasOne(a => a.Exercise)
                    .WithMany()
                    .HasForeignKey(a => a.ExerciseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Enums as string
            modelBuilder.Entity<Appointment>()
                .Property(a => a.Status)
                .HasConversion<string>();

            modelBuilder.Entity<InvoiceItem>()
                .Property(i => i.Status)
                .HasConversion<string>();
        }
    }
}
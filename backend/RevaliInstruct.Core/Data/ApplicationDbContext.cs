using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Core.Entities;

namespace RevaliInstruct.Core.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Bestaande DbSets
        public DbSet<Patient> Patients { get; set; } = null!;

        // **Nieuw: users voor login/authenticatie**
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Patient>(b =>
            {
                b.ToTable("Patients");
                b.HasKey(p => p.Id);

                b.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
                b.Property(p => p.LastName).IsRequired().HasMaxLength(100);
                b.Property(p => p.StartDate).HasColumnType("date").IsRequired();
                b.Property(p => p.Status).HasConversion<string>().IsRequired().HasMaxLength(20);
                b.Property(p => p.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                b.Property(p => p.CreatedBy).IsRequired().HasMaxLength(100);

                b.HasIndex(p => p.LastName).HasDatabaseName("IX_Patients_LastName");
                b.HasIndex(p => new { p.Status, p.StartDate }).HasDatabaseName("IX_Patients_Status_StartDate");
            });

            // Mapping voor User
            modelBuilder.Entity<User>(b =>
            {
                b.ToTable("Users");
                b.HasKey(u => u.UserId); // pas aan als jouw PK anders heet

                b.Property(u => u.Username).IsRequired().HasMaxLength(100);
                b.Property(u => u.PasswordHash).IsRequired().HasMaxLength(256);
                b.Property(u => u.Role).HasMaxLength(50);
                b.Property(u => u.DisplayName).HasMaxLength(200);

                b.HasIndex(u => u.Username).IsUnique().HasDatabaseName("IX_Users_Username");
            });
        }

        // Audit / SaveChanges override (blijft zoals jij had)
        public override int SaveChanges()
        {
            ApplyAuditInfo();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditInfo()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Patient && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var now = DateTime.UtcNow;
                if (entry.State == EntityState.Added)
                {
                    ((Patient)entry.Entity).CreatedAt = now;
                    ((Patient)entry.Entity).CreatedBy = ((Patient)entry.Entity).CreatedBy ?? "system";
                }
                else if (entry.State == EntityState.Modified)
                {
                    ((Patient)entry.Entity).ModifiedAt = now;
                    ((Patient)entry.Entity).ModifiedBy = "system";
                }
            }
        }
    }
}
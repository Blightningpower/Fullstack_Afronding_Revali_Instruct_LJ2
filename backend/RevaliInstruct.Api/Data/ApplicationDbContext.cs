using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Api.Models;

namespace RevaliInstruct.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

        public DbSet<User> Users { get; set; } = null!;
        // later voeg je hier Patients, Declarations, etc.

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(b =>
            {
                b.HasKey(u => u.UserId);
                b.HasIndex(u => u.Username).IsUnique();
                b.Property(u => u.Username).IsRequired().HasMaxLength(100);
                b.Property(u => u.PasswordHash).IsRequired();
                b.Property(u => u.Role).IsRequired().HasMaxLength(50);
            });
        }
    }
}
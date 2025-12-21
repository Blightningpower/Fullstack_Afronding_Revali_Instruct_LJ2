namespace RevaliInstruct.Core.Data
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Config voor Patient
            modelBuilder.Entity<Patient>()
                .Property(p => p.Status)
                .HasDefaultValue(PatientStatus.IntakePlanned); // 'Intake gepland'

            modelBuilder.Entity<Patient>()
                .Property(p => p.Gender)
                .HasDefaultValue(PatientGender.Male);

            // Overige properties zijn gewoon nullable, dus geen default
            // modelBuilder.Entity<Patient>().Property(p => p.BirthDate);
            // modelBuilder.Entity<Patient>().Property(p => p.Address);
            // modelBuilder.Entity<Patient>().Property(p => p.Phone);
            // modelBuilder.Entity<Patient>().Property(p => p.Email);
            // modelBuilder.Entity<Patient>().Property(p => p.NationalInsuranceNumber);

            // ...eventuele extra configuratie voor andere entiteiten...
        }
    }
}
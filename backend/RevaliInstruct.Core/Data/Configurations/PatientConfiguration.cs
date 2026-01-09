using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RevaliInstruct.Core.Entities;

namespace RevaliInstruct.Core.Data.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            // Schakel Temporal Tables in voor traceerbaarheid
            builder.ToTable("Patients", tb => tb.IsTemporal());

            // Data integriteit borgen met Constraints
            builder.HasIndex(p => p.Email).IsUnique(); // Unieke index op e-mail
            builder.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(p => p.LastName).IsRequired().HasMaxLength(100);

            // Bestaande relaties
            builder.HasOne(p => p.AssignedDoctor)
                   .WithMany(u => u.Patients)
                   .HasForeignKey(p => p.AssignedDoctorUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.ReferringDoctor)
                   .WithMany()
                   .HasForeignKey(p => p.ReferringDoctorUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
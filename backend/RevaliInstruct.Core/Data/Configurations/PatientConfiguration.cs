using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RevaliInstruct.Core.Entities;

namespace RevaliInstruct.Core.Data.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
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
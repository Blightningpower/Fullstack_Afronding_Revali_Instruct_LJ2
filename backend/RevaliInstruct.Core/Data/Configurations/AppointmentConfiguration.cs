using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RevaliInstruct.Core.Entities;

namespace RevaliInstruct.Core.Data.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            // Traceerbaarheid voor afspraken inschakelen
            builder.ToTable("Appointments", tb => tb.IsTemporal());
        }
    }
}
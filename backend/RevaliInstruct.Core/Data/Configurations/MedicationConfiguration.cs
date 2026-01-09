using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RevaliInstruct.Core.Entities;

namespace RevaliInstruct.Core.Data.Configurations
{
    public class MedicationConfiguration : IEntityTypeConfiguration<Medication>
    {
        public void Configure(EntityTypeBuilder<Medication> builder)
        {
            // Traceerbaarheid voor medicatie inschakelen
            builder.ToTable("Medication", tb => tb.IsTemporal());
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RevaliInstruct.Core.Entities;

namespace RevaliInstruct.Core.Data.Configurations
{
    public class DeclarationConfiguration : IEntityTypeConfiguration<Declaration>
    {
        public void Configure(EntityTypeBuilder<Declaration> builder)
        {
            // Traceerbaarheid inschakelen
            builder.ToTable("Declarations", tb => tb.IsTemporal());

            // Constraint toevoegen voor data-integriteit
            builder.Property(d => d.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();
        }
    }
}
using EcoMonitor.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoMonitor.DataAccess.Configurations
{
    public class BinTypeConfiguration : IEntityTypeConfiguration<BinTypeEntity>
    {
        public void Configure(EntityTypeBuilder<BinTypeEntity> builder)
        {
            builder.HasKey(bt => bt.Id);

            builder.Property(bt => bt.Code)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(bt => bt.Name)
                .IsRequired()
                .HasMaxLength(35);

            builder.HasMany(bt => bt.BinPhotoBinTypes)
                .WithOne(bbt => bbt.BinType)
                .HasForeignKey(bbt => bbt.BinTypeId);
        }
    }
}

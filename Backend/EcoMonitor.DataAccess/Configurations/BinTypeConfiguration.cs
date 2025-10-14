using EcoMonitor.Core.Models;
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

            builder.HasData(
                new BinTypeEntity
                {
                    Id = BinTypeConstants.Glass,
                    Code = "GLASS",
                    Name = "Стекло"
                },
                new BinTypeEntity
                {
                    Id = BinTypeConstants.Paper,
                    Code = "PAPER",
                    Name = "Бумага и картон"
                },
                new BinTypeEntity
                {
                    Id = BinTypeConstants.Plastic,
                    Code = "PLASTIC",
                    Name = "Пластик"
                },
                new BinTypeEntity
                {
                    Id = BinTypeConstants.Metal,
                    Code = "METAL",
                    Name = "Металл"
                },
                new BinTypeEntity
                {
                    Id = BinTypeConstants.Organic,
                    Code = "ORGANIC",
                    Name = "Органика и пищевые отходы"
                },
                new BinTypeEntity
                {
                    Id = BinTypeConstants.Universal,
                    Code = "UNIVERSAL",
                    Name = "Смешанные отходы"
                });

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

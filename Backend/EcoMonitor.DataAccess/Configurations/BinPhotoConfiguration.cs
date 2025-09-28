
using EcoMonitor.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoMonitor.DataAccess.Configurations
{
    public class BinPhotoConfiguration : IEntityTypeConfiguration<BinPhotoEntity>
    {
        public void Configure(EntityTypeBuilder<BinPhotoEntity> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.FileName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.UrlFile)
                .IsRequired();

            builder.Property(p => p.Location)
                .HasColumnType("geography (Point,4326)")
                .IsRequired();

            builder.Property(p => p.UploadedAt)
                .IsRequired();

            builder.Property(p => p.FillLevel)
                .IsRequired();

            builder.Property(p => p.IsOutsideBin)
                .IsRequired();

            builder.Property(p => p.Comment)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasOne(p => p.UploadedBy)
                .WithMany(u => u.BinPhoto)
                .HasForeignKey(p => p.UploadedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(bp => bp.BinPhotoBinTypes)
                .WithOne(bbt => bbt.BinPhoto)
                .HasForeignKey(bbt => bbt.BinPhotoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

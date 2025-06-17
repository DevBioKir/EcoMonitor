
using EcoMonitor.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoMonitor.DataAccess.Configurations
{
    class BinPhotoConfiguration : IEntityTypeConfiguration<BinPhotoEntities>
    {
        public void Configure(EntityTypeBuilder<BinPhotoEntities> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.UrlFile)
                .IsRequired();

            builder.Property(e => e.Latitude)
                .IsRequired();

            builder.Property(e => e.Longitude)
                .IsRequired();

            builder.Property(e => e.UploadedAt)
                .IsRequired();

            builder.Property(e => e.BinType)
                .IsRequired();

            builder.Property(e => e.FillLevel)
                .IsRequired();

            builder.Property(e => e.IsOutsideBin)
                .IsRequired();

            builder.Property(e => e.Comment)
                .IsRequired();
        }
    }
}

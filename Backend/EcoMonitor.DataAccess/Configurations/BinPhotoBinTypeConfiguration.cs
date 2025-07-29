using EcoMonitor.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoMonitor.DataAccess.Configurations
{
    public class BinPhotoBinTypeConfiguration : IEntityTypeConfiguration<BinPhotoBinTypeEntity>
    {
        public void Configure(EntityTypeBuilder<BinPhotoBinTypeEntity> builder)
        {
            builder.HasKey(pt => new { pt.BinPhotoId, pt.BinTypeId });

            builder.HasOne(bbt => bbt.BinPhoto)
                .WithMany(bp => bp.BinPhotoBinTypes)
                .HasForeignKey(bbt => bbt.BinPhotoId);

            builder.HasOne(bbt => bbt.BinType)
                .WithMany(bt => bt.BinPhotoBinTypes)
                .HasForeignKey(bbt => bbt.BinTypeId);
        }
    }
}

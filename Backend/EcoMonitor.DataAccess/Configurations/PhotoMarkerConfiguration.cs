using EcoMonitor.Contracts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoMonitor.DataAccess.Configurations;

public class PhotoMarkerConfiguration : IEntityTypeConfiguration<PhotoMarker>
{
    public void Configure(EntityTypeBuilder<PhotoMarker> builder)
    {
        builder.HasNoKey();
        builder.ToView(null);
    }
}
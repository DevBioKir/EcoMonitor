using EcoMonitor.DataAccess.Configurations;
using EcoMonitor.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoMonitor.DataAccess
{
    public class EcoMonitorDbContext : DbContext
    {
        public DbSet<BinPhotoEntity> BinPhotos { get; set; } = null!;
        public DbSet<BinTypeEntity> binTypes { get; set; } = null!;

        public EcoMonitorDbContext(DbContextOptions<EcoMonitorDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BinPhotoConfiguration());
            modelBuilder.ApplyConfiguration(new BinTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BinPhotoBinTypeConfiguration());
        }
    }
}

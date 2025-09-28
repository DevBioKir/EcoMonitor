using EcoMonitor.DataAccess.Configurations;
using EcoMonitor.DataAccess.Configurations.Users;
using EcoMonitor.DataAccess.Entities;
using EcoMonitor.DataAccess.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace EcoMonitor.DataAccess
{
    public class EcoMonitorDbContext : DbContext
    {
        public DbSet<BinPhotoEntity> BinPhotos { get; set; } = null!;
        public DbSet<BinTypeEntity> BinTypes { get; set; } = null!;
        public DbSet<BinPhotoBinTypeEntity> BinPhotoBinType { get; set; } = null!;
        public DbSet<UserEntity> Users { get; set; } = null!;
        public DbSet<UserRoleEntity> UserRoles { get; set; } = null!;



        public EcoMonitorDbContext(DbContextOptions<EcoMonitorDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BinPhotoConfiguration());
            modelBuilder.ApplyConfiguration(new BinTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BinPhotoBinTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EcoMonitor.DataAccess.Factory;

public class EcoMonitorDbContextFactory : IDesignTimeDbContextFactory<EcoMonitorDbContext>
{
    public EcoMonitorDbContext CreateDbContext(string[] args)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environmentName}.json", true)
            .Build();
        
        var connectionString = configuration.GetConnectionString("EcoMonitorDbContext");
        
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException(
                $"Connection string '{nameof(EcoMonitorDbContext)}' not found in appsettings.{environmentName}.json");
        
        var optionsBuilder = new DbContextOptionsBuilder<EcoMonitorDbContext>();
        optionsBuilder.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.UseNetTopologySuite());
          
        return new EcoMonitorDbContext(optionsBuilder.Options);
        
    }
}
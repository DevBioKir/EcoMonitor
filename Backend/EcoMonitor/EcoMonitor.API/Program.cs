using EcoMonitor.App.Mapper;
using EcoMonitor.DataAccess;
using EcoMonitor.DataAccess.Repositories;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<EcoMonitorDbContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString(nameof(EcoMonitorDbContext)));
});

TypeAdapterConfig.GlobalSettings.Scan(typeof(MappingConfig).Assembly);

builder.Services.AddSingleton(TypeAdapterConfig.GlobalSettings);
builder.Services.AddScoped<IMapper, ServiceMapper>();

builder.Services.AddScoped<IBinPhotoRepository, BinPhotoRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

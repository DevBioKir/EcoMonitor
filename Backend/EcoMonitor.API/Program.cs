using EcoMonitor.App.Mapper;
using EcoMonitor.DataAccess;
using EcoMonitor.DataAccess.Repositories;
using EcoMonitor.Infrastracture.Middleware;
using Microsoft.AspNetCore.Http.Features;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using EcoMonitor.App.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();

//для Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<EcoMonitorDbContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString(nameof(EcoMonitorDbContext)));
});

TypeAdapterConfig.GlobalSettings.Scan(typeof(MappingConfig).Assembly);

builder.Services.AddSingleton(TypeAdapterConfig.GlobalSettings);
builder.Services.AddScoped<IMapper, ServiceMapper>();

builder.Services.AddScoped<IBinPhotoRepository, BinPhotoRepository>();

builder.Services.AddScoped<IBinPhotoService, BinPhotoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Включаем Swagger в Development
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

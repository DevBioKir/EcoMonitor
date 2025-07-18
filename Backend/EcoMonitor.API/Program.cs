using EcoMonitor.App.Mapper;
using EcoMonitor.DataAccess;
using EcoMonitor.DataAccess.Repositories;
using EcoMonitor.Infrastracture.Middleware;
using Microsoft.AspNetCore.Http.Features;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using EcoMonitor.App.Services;
using EcoMonitor.Infrastracture.Abstractions;
using EcoMonitor.Infrastracture.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var env = builder.Environment;

builder.Services.AddControllers();

//дл€ Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<EcoMonitorDbContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString(nameof(EcoMonitorDbContext)));
});

TypeAdapterConfig.GlobalSettings.Scan(typeof(MappingConfig).Assembly);

builder.Services.AddSingleton(TypeAdapterConfig.GlobalSettings);
builder.Services.AddScoped<IMapper, ServiceMapper>();

// –егистрируем путь как строку
builder.Services.AddSingleton(env.WebRootPath);
builder.Services.AddScoped<IImageStorageService, ImageStorageService>();

//builder.Services.AddScoped<IImageStorageService>(sp =>
//{
//    var env = sp.GetRequiredService<IWebHostEnvironment>();
//    return new ImageStorageService(env.WebRootPath);
//});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://192.168.1.154:8081")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
                  //.AllowCredentials(); дл€ куки
        });
});

builder.Services.AddScoped<IBinPhotoRepository, BinPhotoRepository>();

builder.Services.AddScoped<IBinPhotoService, BinPhotoService>();

builder.Services.AddScoped<IGeolocationService, GeolocationService>();

builder.WebHost.UseUrls("http://0.0.0.0:5198");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // ¬ключаем Swagger в Development
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(env.ContentRootPath, "wwwroot", "Photos")),
    RequestPath = "/Photos"
});

app.UseStaticFiles();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCors("AllowFrontend");

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

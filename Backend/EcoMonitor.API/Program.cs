using EcoMonitor.API.Extensions;
using EcoMonitor.App.Abstractions;
using EcoMonitor.App.Factory.Users;
using EcoMonitor.App.Mapper;
using EcoMonitor.App.Services;
using EcoMonitor.App.Services.Authorization;
using EcoMonitor.App.Services.User;
using EcoMonitor.DataAccess;
using EcoMonitor.DataAccess.Repositories;
using EcoMonitor.DataAccess.Repositories.Auth;
using EcoMonitor.DataAccess.Repositories.Users;
using EcoMonitor.Infrastracture.Abstractions;
using EcoMonitor.Infrastracture.Authentication;
using EcoMonitor.Infrastracture.Middleware;
using EcoMonitor.Infrastracture.Pipeline;
using EcoMonitor.Infrastracture.Services;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var env = builder.Environment;

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<EcoMonitorDbContext>(options =>
{
    options.UseNpgsql(
        configuration.GetConnectionString(nameof(EcoMonitorDbContext)),
        npgsqlOptions => npgsqlOptions.UseNetTopologySuite());
});

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUserFactory, UserFactory>();
builder.Services.AddScoped<IUserRoleFactory, UserRoleFactory>();

builder.Services.AddLogging();

var serviceProvider = builder.Services.BuildServiceProvider();
var userFactory = serviceProvider.GetRequiredService<IUserFactory>();
var userRoleFactory = serviceProvider.GetRequiredService<IUserRoleFactory>();
var logger = serviceProvider.GetRequiredService<ILogger<MappingConfig>>();

var config = new TypeAdapterConfig();
config.Apply(new MappingConfig(userFactory, userRoleFactory, logger));

builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();

builder.Services.AddSingleton(env.WebRootPath);
builder.Services.AddScoped<IImageStorageService, ImageStorageService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.AllowAnyOrigin() //replace before deployment
            //policy.WithOrigins("http://192.168.1.255")
            //policy.WithOrigins("http://192.168.1.255:8081")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
                  //.AllowCredentials();
        });
});

builder.Services.AddScoped<IBinPhotoRepository, BinPhotoRepository>();
builder.Services.AddScoped<IBinTypeRepository, BinTypeRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddScoped<IBinPhotoService, BinPhotoService>();
builder.Services.AddScoped<IBinTypeService, BinTypeService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IGeolocationService, GeolocationService>();

builder.Services.AddScoped<IImagePipeline, ImagePipeline>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<IJWTService, JWTService>();

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
ApiExtensions.AddApiAuthentication(builder.Services, Options.Create(jwtSettings));

builder.WebHost.UseUrls("http://0.0.0.0:5198");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

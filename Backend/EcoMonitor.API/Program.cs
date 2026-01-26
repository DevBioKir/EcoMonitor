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
using System.Globalization;
using EcoMonitor.App.Factory.Users.Resolve;


var builder = WebApplication.CreateBuilder(args);

var culture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var configuration = builder.Configuration;
var env = builder.Environment;

builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new VersionedPrefixConvention());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "EcoMonitor API", Version = "v1" });
});

builder.Services.AddDbContext<EcoMonitorDbContext>(options =>
{
    options.UseNpgsql(
        configuration.GetConnectionString(nameof(EcoMonitorDbContext)),
        npgsqlOptions => npgsqlOptions.UseNetTopologySuite()
    );

    options.LogTo(Console.WriteLine, LogLevel.Information);
    options.EnableSensitiveDataLogging();
});

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUserFactory, UserFactory>();
builder.Services.AddScoped<IUserRoleFactory, UserRoleFactory>();
builder.Services.AddScoped<IUserRegisterFactory, UserRegisterFactory>();
builder.Services.AddScoped<IUserRegisterFactory, ManagerRegisterFactory>();
builder.Services.AddScoped<IUserRegisterFactoryResolver, UserRegisterFactoryResolver>();
builder.Services.AddLogging();

builder.Services.AddSingleton(TypeAdapterConfig.GlobalSettings); // глобальный Mapster config
builder.Services.AddScoped<IMapper, ServiceMapper>();

// var serviceProvider = builder.Services.BuildServiceProvider();
// var userFactory = serviceProvider.GetRequiredService<IUserFactory>();
// var userRoleFactory = serviceProvider.GetRequiredService<IUserRoleFactory>();
// var logger = serviceProvider.GetRequiredService<ILogger<MappingConfig>>();
//
// var config = new TypeAdapterConfig();
// config.Apply(new MappingConfig(userFactory, userRoleFactory, logger));
//
// builder.Services.AddSingleton(config);
// builder.Services.AddScoped<IMapper, ServiceMapper>();

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
if (jwtSettings == null)
    throw new InvalidOperationException("JwtSettings config section missing or misconfigured.");
AuthorizationSetup.AddApiAuthentication(builder.Services, Options.Create(jwtSettings));

builder.WebHost.UseUrls("http://0.0.0.0:5198", 
    "https://0.0.0.0:7198");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var config = scope.ServiceProvider.GetRequiredService<TypeAdapterConfig>();
    var userFactory = scope.ServiceProvider.GetRequiredService<IUserFactory>();
    var userRoleFactory = scope.ServiceProvider.GetRequiredService<IUserRoleFactory>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<MappingConfig>>();
    config.Apply(new MappingConfig(userFactory, userRoleFactory, logger));
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "EcoMonitor.API v1");
        options.RoutePrefix = string.Empty;
    });
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

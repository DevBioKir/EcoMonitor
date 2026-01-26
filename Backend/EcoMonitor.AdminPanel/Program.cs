using EcoMonitor.AdminPanel.Components;
using EcoMonitor.AdminPanel.Infrastucture.Http;
using EcoMonitor.AdminPanel.Infrastucture.TokenPersistence;
using EcoMonitor.AdminPanel.Infrastucture.TokenStorage;
using EcoMonitor.App.Services;
using EcoMonitor.App.Services.Authorization;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(o => o.DetailedErrors = true);

builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

// builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddMudServices();

builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

var apiBaseUrl = configuration.GetConnectionString("EcoMonitorAPI")
                 ?? throw new InvalidOperationException("Connection string 'EcoMonitorAPI' not found.");;

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<Http401Handler>();
builder.Services.AddScoped<Http401HandlerAccessor>();

builder.Services.AddSingleton<ITokenStore, TokenStore>();
builder.Services.AddSingleton<ITokenPersistenceService, TokenPersistenceService>();

builder.Services.AddScoped<AdminAuthHandler>();

builder.Services.AddHttpClient("PublicApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient("AdminApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
    .AddHttpMessageHandler<AdminAuthHandler>();

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
    logging.AddFilter("Microsoft", LogLevel.Warning);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapControllers();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
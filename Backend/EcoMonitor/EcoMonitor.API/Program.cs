using EcoMonitor.DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;



builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<EcoMonitorDbContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString(nameof(EcoMonitorDbContext)));
});

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

using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace EcoMonitor.API.Extensions;

public static class MapApiGroupsExtensions
{
    public static void MapApiGroups(this WebApplication app)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var controllers = assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(ControllerBase)));

        var publicGroups = app.MapGroup("/api/v1")
            .WithTags("Public API");
        foreach (var controller in controllers.Where(c => c))
            
    }
}
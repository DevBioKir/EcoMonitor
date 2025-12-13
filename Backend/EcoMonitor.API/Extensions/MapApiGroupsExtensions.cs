using System.Reflection;
using EcoMonitor.API.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace EcoMonitor.API.Extensions;

public static class MapApiGroupsExtensions
{
    public static void MapApiGroups(this WebApplication app)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var controllers = assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(ControllerBase)));

        var publicGroup = app.MapGroup("/api/public/v1")
            .WithTags("Public API");
        foreach (var controller in controllers.Where(c =>
                     c.GetCustomAttribute<PublicApiAttribute>() != null))
        {
            publicGroup.MapControllerRouteFor(controller);
        }
        // foreach (var controller in controllers.Where(c =>
        //              c.GetCustomAttribute<PublicApiAttribute>() != null))
        //     publicGroup.MapControllerRouteFor(controller);
        
        var adminGroup = app.MapGroup("/api/admin/v1")
            .WithTags("Admin API");
        foreach (var controller in controllers.Where(c =>
                     c.GetCustomAttribute<AdminApiAttribute>() != null))
        {
            adminGroup.MapControllerRouteFor(controller);
        }
        
        // foreach (var controller in controllers.Where(c => 
        //              c.GetCustomAttribute<AdminApiAttribute>() != null))
        //          adminGroup.MapControllerRouteFor(controller);
    }

    private static void MapControllerRouteFor(this RouteGroupBuilder group, Type controllerType)
    {
        string controllerName = controllerType.Name.Replace("Controller", string.Empty);

        group.MapControllerRoute(
            name: controllerName,
            pattern: $"{controllerName}/{{action}}/{{id?}}",
            defaults: new {controller = controllerName});
    }
}
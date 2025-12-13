using EcoMonitor.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace EcoMonitor.API.Extensions;

public class VersionedPrefixConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            var hasPublic = controller.Attributes.OfType<PublicApiAttribute>().Any();
            var hasAdmin = controller.Attributes.OfType<AdminApiAttribute>().Any();

            string? prefix = hasPublic ? "api/public/v1" : 
                            hasAdmin ? "api/admin/v1" : 
                            null;
            
            if (prefix is null)
                continue;

            var prefixModel = new AttributeRouteModel(new RouteAttribute(prefix));

            foreach (var selector in controller.Selectors)
            {
                selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(prefixModel, 
                    selector.AttributeRouteModel);
            }
        }
    }
}
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using Microsoft.AspNetCore.Http;

public class SwaggerFileOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.RequestBody == null || !operation.RequestBody.Content.ContainsKey("multipart/form-data"))
            return;

        var schema = operation.RequestBody.Content["multipart/form-data"].Schema;

        var fileParams = context.MethodInfo
            .GetParameters()
            .Where(p => p.ParameterType == typeof(IFormFile) || p.ParameterType == typeof(IFormFile[]));

        foreach (var param in fileParams)
        {
            schema.Properties[param.Name] = new OpenApiSchema
            {
                Type = "string",
                Format = "binary"
            };
        }
    }
}
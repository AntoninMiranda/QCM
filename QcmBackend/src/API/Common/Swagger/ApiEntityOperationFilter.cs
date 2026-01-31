using System;
using System.Collections.Generic;
using System.Linq;
using QcmBackend.API.Common.Attributes;
using QcmBackend.Application.Common.Include;
using QcmBackend.Application.Common.Interfaces;
using QcmBackend.Application.Common.Sorting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace QcmBackend.API.Common.Swagger;

public class ApiEntityOperationFilter(IServiceProvider serviceProvider) : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        ApiEntityAttribute? attribute = context.MethodInfo.GetCustomAttribute<ApiEntityAttribute>();
        if (attribute == null)
        {
            return;
        }

        using IServiceScope scope = serviceProvider.CreateScope();

        if (operation.Parameters.Any(p => p.Name.Equals("include", StringComparison.OrdinalIgnoreCase)))
        {
            AddDescription<IIncludeConfigurator<object>>(scope, attribute.EntityType, operation, "Includable properties");
        }

        if (operation.Parameters.Any(p => p.Name.Equals("sorts", StringComparison.OrdinalIgnoreCase)))
        {
            AddDescription<ISortConfigurator<object>>(scope, attribute.EntityType, operation, "Sortable properties");
        }
    }

    private static void AddDescription<TConfigurator>(IServiceScope scope, Type entityType, OpenApiOperation operation, string title)
    {
        Type configuratorType = typeof(TConfigurator).GetGenericTypeDefinition().MakeGenericType(entityType);
        object? configurator = scope.ServiceProvider.GetService(configuratorType);

        if (configurator != null)
        {
            MethodInfo? configureMethod = configurator.GetType().GetMethod("Configure");
            IPropertiesConfigurator? propertiesConfigurator = (IPropertiesConfigurator?)configureMethod?.Invoke(configurator, null);
            IEnumerable<string> descriptiveProperties = propertiesConfigurator?.GetDescriptiveProperties() ?? Array.Empty<string>();

            if (descriptiveProperties.Any())
            {
                string propertiesString = string.Join(", ", descriptiveProperties);
                operation.Description += $"\n\n**{title}:** {propertiesString}";
            }
        }
    }
}

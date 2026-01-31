using System;
using System.Linq;
using QcmBackend.Application.Common.Include;
using QcmBackend.Application.Common.Sorting;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace QcmBackend.Application.Common
{
    public static class ApplicationConfiguratorRegistration
    {
        public static IServiceCollection AddConfiguratorsFromAssembly(this IServiceCollection services, Assembly applicationAssembly)
        {
            Type[] configuratorTypes = [typeof(IIncludeConfigurator<>), typeof(ISortConfigurator<>)];

            var configurators = applicationAssembly
                .GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Select(t => new
                {
                    Implementation = t,
                    Interfaces = t.GetInterfaces()
                        .Where(i => i.IsGenericType && configuratorTypes.Contains(i.GetGenericTypeDefinition()))
                })
                .Where(x => x.Interfaces.Any());

            foreach (var cfg in configurators)
            {
                foreach (Type? @interface in cfg.Interfaces)
                {
                    _ = services.AddSingleton(@interface, cfg.Implementation);
                }
            }

            return services;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Sconfig.Interfaces.Mapping;
using Sconfig.Interfaces.Services;
using Sconfig.Mapping;
using Sconfig.Services;

namespace Sconfig.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSConfig(this IServiceCollection collection)
        {
            // services
            collection.AddTransient<ICustomerService, CustomerService>();
            collection.AddTransient<IProjectService, ProjectService>();
            collection.AddTransient<IEnvironmentService, EnvironmentService>();
            collection.AddTransient<IConfigurationGroupService, ConfigurationGroupService>();
            collection.AddTransient<IConfigurationItemService, ConfigurationItemService>();

            // mapping
            collection.AddTransient<ICustomerMapper, CustomerMapper>();
            collection.AddTransient<IProjectMapper, ProjectMapper>();
            collection.AddTransient<IEnvironmentMapper, EnvironmentMapper>();
        }
    }
}

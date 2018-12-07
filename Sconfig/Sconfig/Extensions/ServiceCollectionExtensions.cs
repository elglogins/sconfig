using Microsoft.Extensions.DependencyInjection;
using Sconfig.Interfaces.Services;
using Sconfig.Services;

namespace Sconfig.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSConfig(this IServiceCollection collection)
        {
            collection.AddTransient<ICustomerService, CustomerService>();
            //collection.AddTransient<IConfigurationGroupService, ConfigurationGroupService>();
        }
    }
}

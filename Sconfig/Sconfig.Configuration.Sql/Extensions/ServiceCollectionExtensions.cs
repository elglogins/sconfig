using Microsoft.Extensions.DependencyInjection;
using Sconfig.Configuration.Sql.Interfaces;
using Sconfig.Configuration.Sql.Repositories;
using Sconfig.Interfaces.Repositories;

namespace Sconfig.Configuration.Sql.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSConfigSqlConfiguration(this IServiceCollection collection, ISconfigSqlConfiguration configuration)
        {
            collection.AddSingleton<ISconfigSqlConfiguration>(configuration);
            collection.AddSingleton<ICustomerRepository, CustomerRepository>();
            collection.AddSingleton<IConfigurationItemRepository, ConfigurationItemRepository>();
            collection.AddSingleton<IConfigurationGroupRepository, ConfigurationGroupRepository>();
        }
    }
}

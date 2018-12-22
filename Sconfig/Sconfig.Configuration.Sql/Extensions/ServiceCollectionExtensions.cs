using Microsoft.Extensions.DependencyInjection;
using Sconfig.Configuration.Sql.Factories;
using Sconfig.Configuration.Sql.Interfaces;
using Sconfig.Configuration.Sql.Repositories;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Repositories;

namespace Sconfig.Configuration.Sql.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSConfigSqlConfiguration(this IServiceCollection collection, ISconfigSqlConfiguration configuration)
        {
            collection.AddSingleton<ISconfigSqlConfiguration>(configuration);
        }

        public static void AddSConfigSql(this IServiceCollection collection)
        {
            collection.AddSConfigSqlCustomers();
            collection.AddSConfigSqlConfigurationGroupsAndItems();
            collection.AddSConfigSqlEnvironments();
            collection.AddSConfigSqlApplications();
            collection.AddSConfigSqlProjects();
        }

        public static void AddSConfigSqlCustomers(this IServiceCollection collection)
        {
            collection.AddSingleton<ICustomerRepository, CustomerRepository>();
            collection.AddTransient<ICustomerFactory, CustomerFactory>();
        }

        public static void AddSConfigSqlConfigurationGroupsAndItems(this IServiceCollection collection)
        {
            collection.AddSingleton<IConfigurationItemRepository, ConfigurationItemRepository>();
            collection.AddSingleton<IConfigurationGroupRepository, ConfigurationGroupRepository>();
            collection.AddSingleton<IConfigurationItemFactory, ConfigurationItemFactory>();
            collection.AddSingleton<IConfigurationGroupFactory, ConfigurationGroupFactory>();
        }

        public static void AddSConfigSqlEnvironments(this IServiceCollection collection)
        {
            collection.AddSingleton<IEnvironmentRepository, EnvironmentRepository>();
            collection.AddSingleton<IEnvironmentFactory, EnvironmentFactory>();
        }

        public static void AddSConfigSqlApplications(this IServiceCollection collection)
        {
            collection.AddSingleton<IApplicationRepository, ApplicationRepository>();
            collection.AddSingleton<IApplicationFactory, ApplicationFactory>();
        }

        public static void AddSConfigSqlProjects(this IServiceCollection collection)
        {
            collection.AddSingleton<IProjectRepository, ProjectRepository>();
            collection.AddSingleton<IProjectFactory, ProjectFactory>();
        }
    }
}

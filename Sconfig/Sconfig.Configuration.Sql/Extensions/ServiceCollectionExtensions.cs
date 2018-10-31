using Microsoft.Extensions.DependencyInjection;
using Sconfig.Configuration.Sql.Interfaces;

namespace Sconfig.Configuration.Sql.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSConfigSqlConfiguration(this IServiceCollection collection, ISconfigSqlConfiguration configuration)
        {
            collection.AddSingleton<ISconfigSqlConfiguration>(configuration);
        }
    }
}

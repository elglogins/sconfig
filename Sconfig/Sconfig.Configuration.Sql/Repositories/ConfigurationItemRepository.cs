using Sconfig.Configuration.Sql.Interfaces;
using Sconfig.Configuration.Sql.Models;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;

namespace Sconfig.Configuration.Sql.Repositories
{
    class ConfigurationItemRepository : AbstractSqlRespository<IConfigurationItemModel, ConfigurationItemModel>, IConfigurationItemRepository
    {
        public ConfigurationItemRepository(ISconfigSqlConfiguration configuration) : base(configuration)
        {
        }
    }
}

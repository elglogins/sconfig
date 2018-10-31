using Sconfig.Configuration.Sql.Interfaces;
using Sconfig.Configuration.Sql.Models;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;

namespace Sconfig.Configuration.Sql.Repositories
{
    class ConfigurationGroupRepository : AbstractSqlRespository<IConfigurationGroupModel, ConfigurationGroupModel>, IConfigurationGroupRepository
    {
        public ConfigurationGroupRepository(ISconfigSqlConfiguration configuration) : base(configuration)
        {
        }
    }
}

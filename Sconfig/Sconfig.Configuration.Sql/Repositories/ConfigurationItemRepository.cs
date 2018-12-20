using System.Collections.Generic;
using System.Threading.Tasks;
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

        public Task<IEnumerable<IConfigurationItemModel>> GetByName(string name, string projectId, string applicationId, string parentId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<IConfigurationItemModel>> GetByParent(string parentId, string projectId, string applicationId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<IConfigurationItemModel>> GetRootLevelByApplication(string applicationId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<IConfigurationItemModel>> GetRootLevelByProject(string projectId)
        {
            throw new System.NotImplementedException();
        }
    }
}

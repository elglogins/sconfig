using System.Collections.Generic;
using System.Threading.Tasks;
using Sconfig.Configuration.Sql.Interfaces;
using Sconfig.Configuration.Sql.Models;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;

namespace Sconfig.Configuration.Sql.Repositories
{
    internal class ConfigurationGroupRepository : AbstractSqlRespository<IConfigurationGroupModel, ConfigurationGroupModel>, IConfigurationGroupRepository
    {
        public ConfigurationGroupRepository(ISconfigSqlConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IEnumerable<IConfigurationGroupModel>> GetByApplication(string projectId, string applicationId)
        {
            using(var db = GetClient())
            {
                return await db.FetchAsync<ConfigurationGroupModel>($"SELECT * FROM [{TableName}] WHERE [ApplicationId] = @0 AND [ProjectId] = @1", applicationId, projectId);
            }
        }

        public async Task<IConfigurationGroupModel> GetByNameAndByProject(string name, string projectId)
        {
            using (var db = GetClient())
            {
                return await db.FirstOrDefaultAsync<ConfigurationGroupModel>($"SELECT * FROM [{TableName}] WHERE [Name] = @0 AND [ProjectId] = @1", name, projectId);
            }
        }

        public async Task<IEnumerable<IConfigurationGroupModel>> GetByParentGroup(string parentGroupId)
        {
            using (var db = GetClient())
            {
                return await db.FetchAsync<ConfigurationGroupModel>($"SELECT * FROM [{TableName}] WHERE [ParentId] = @0", parentGroupId);
            }
        }

        public async Task<IEnumerable<IConfigurationGroupModel>> GetByProject(string projectId)
        {
            using (var db = GetClient())
            {
                return await db.FetchAsync<ConfigurationGroupModel>($"SELECT * FROM [{TableName}] WHERE [ProjectId] = @0", projectId);
            }
        }

        public async Task<IEnumerable<IConfigurationGroupModel>> GetRootLevelByProject(string projectId)
        {
            using (var db = GetClient())
            {
                return await db.FetchAsync<ConfigurationGroupModel>($"SELECT * FROM [{TableName}] WHERE [ProjectId] = @0 AND [ParentId] IS NULL", projectId);
            }
        }
    }
}

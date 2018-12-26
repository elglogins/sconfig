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

        public async Task<IEnumerable<IConfigurationItemModel>> GetByApplication(string projectId, string applicationId)
        {
            using (var db = GetClient())
            {
                return await db.FetchAsync<ConfigurationItemModel>($"SELECT * FROM {TableName} WHERE [ProjectId] = @0 AND [ApplicationId] = @1", projectId, applicationId);
            }
        }

        public async Task<IEnumerable<IConfigurationItemModel>> GetByName(string name, string projectId, string applicationId, string parentId)
        {
            var sql = string.Format(@"
                SELECT * FROM {0}
                WHERE
                    [Name] = @0
                    AND [ProjectId] = @1
                    {1}
                    {2}
            ",
                TableName,
                string.IsNullOrWhiteSpace(applicationId) ? "AND [ApplicationId] IS NULL" : " AND [ApplicationId] = @2",
                string.IsNullOrWhiteSpace(parentId) ? "AND [ParentId] IS NULL" : " AND [ParentId] = @3"
            );

            using (var db = GetClient())
            {
                return await db.FetchAsync<ConfigurationItemModel>(sql, name, projectId, applicationId, parentId);
            }
        }

        public async Task<IEnumerable<IConfigurationItemModel>> GetByParent(string parentId, string projectId, string applicationId)
        {
            var sql = string.Format(@"
                SELECT * FROM {0}
                WHERE
                    [ProjectId] = @0
                    AND [ParentId] = @1
                    {1}
            ",
                TableName,
                string.IsNullOrWhiteSpace(applicationId) ? "AND [ApplicationId] IS NULL" : " AND [ApplicationId] = @2"
            );

            using (var db = GetClient())
            {
                return await db.FetchAsync<ConfigurationItemModel>(sql, projectId, parentId, applicationId);
            }
        }

        public async Task<IEnumerable<IConfigurationItemModel>> GetByProject(string projectId)
        {
            using (var db = GetClient())
            {
                return await db.FetchAsync<ConfigurationItemModel>($"SELECT * FROM {TableName} WHERE [ProjectId] = @0 AND [ApplicationId] IS NULL", projectId);
            }
        }

        public async Task<IEnumerable<IConfigurationItemModel>> GetRootLevelByApplication(string applicationId)
        {
            using (var db = GetClient())
            {
                return await db.FetchAsync<ConfigurationItemModel>($"SELECT * FROM {TableName} WHERE [ApplicationId] = @0", applicationId);
            }
        }

        public async Task<IEnumerable<IConfigurationItemModel>> GetRootLevelByProject(string projectId)
        {
            using (var db = GetClient())
            {
                return await db.FetchAsync<ConfigurationItemModel>($"SELECT * FROM {TableName} WHERE [ProjectId] = @0", projectId);
            }
        }
    }
}

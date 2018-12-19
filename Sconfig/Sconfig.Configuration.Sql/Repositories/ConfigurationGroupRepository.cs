using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task<IConfigurationGroupModel> GetByNameAndByCustomer(string name, string customerId)
        {
            using (var db = GetClient())
            {
                return await db.FirstOrDefaultAsync<ConfigurationGroupModel>($"SELECT TOP 1 * FROM [{TableName}] WHERE [Name] = @0 AND [CustomerId] = @1", name, customerId);
            }
        }

        public async Task<IEnumerable<IConfigurationGroupModel>> GetByParentGroup(string parentGroupId)
        {
            using (var db = GetClient())
            {
                return await db.FetchAsync<ConfigurationGroupModel>($"SELECT * FROM [{TableName}] WHERE [ParentGroupId] = @0", parentGroupId);
            }
        }

        public async Task<IEnumerable<IConfigurationGroupModel>> GetWithoutParentGroupAndByCustomer(string customerId)
        {
            using (var db = GetClient())
            {
                return await db.FetchAsync<ConfigurationGroupModel>($"SELECT * FROM [{TableName}] WHERE [ParentGroupId] IS NULL AND [CustomerId] = @0", customerId);
            }
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Sconfig.Configuration.Sql.Interfaces;
using Sconfig.Configuration.Sql.Models;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;

namespace Sconfig.Configuration.Sql.Repositories
{
    internal class EnvironmentRepository : AbstractSqlRespository<IEnvironmentModel, EnvironmentModel>, IEnvironmentRepository
    {
        public EnvironmentRepository(ISconfigSqlConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IEnvironmentModel> GetByName(string name, string projectId)
        {
            using (var db = GetClient())
            {
                return await db.FirstOrDefaultAsync<EnvironmentModel>($"SELECT TOP 1 * FROM [{TableName}] WHERE [Name] = @0 AND [ProjectId] = @1", name, projectId);
            }
        }

        public async Task<IEnumerable<IEnvironmentModel>> GetByProject(string projectId)
        {
            using (var db = GetClient())
            {
                return await db.FetchAsync<EnvironmentModel>($"SELECT * FROM [{TableName}] WHERE [ProjectId] = @0", projectId);
            }
        }
    }
}

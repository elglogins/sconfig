using Sconfig.Configuration.Sql.Interfaces;
using Sconfig.Configuration.Sql.Models;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;
using System.Threading.Tasks;

namespace Sconfig.Configuration.Sql.Repositories
{
    class ProjectRepository : AbstractSqlRespository<IProjectModel, ProjectModel>, IProjectRepository
    {
        public ProjectRepository(ISconfigSqlConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IProjectModel> GetByName(string name, string customerId)
        {
            using (var db = GetClient())
            {
                return await db.FirstOrDefaultAsync<ProjectModel>($"SELECT TOP 1 * FROM [{TableName}] WHERE [Name] = @0 AND [CustomerId] = @1", name, customerId);
            }
        }
    }
}

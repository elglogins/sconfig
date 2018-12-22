using Sconfig.Interfaces.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sconfig.Interfaces.Repositories
{
    public interface IEnvironmentRepository : IRepo<IEnvironmentModel>
    {
        Task<IEnvironmentModel> GetByName(string name, string projectId);

        Task<IEnumerable<IEnvironmentModel>> GetByProject(string projectId);
    }
}

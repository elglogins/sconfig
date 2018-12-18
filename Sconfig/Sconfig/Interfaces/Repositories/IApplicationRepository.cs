using System.Threading.Tasks;
using Sconfig.Interfaces.Models;

namespace Sconfig.Interfaces.Repositories
{
    public interface IApplicationRepository : IRepo<IApplicationModel>
    {
        Task<IApplicationModel> GetByName(string name, string projectId);
    }
}

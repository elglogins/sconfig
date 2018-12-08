using Sconfig.Interfaces.Models;
using System.Threading.Tasks;

namespace Sconfig.Interfaces.Repositories
{
    public interface IProjectRepository : IRepo<IProjectModel>
    {
        Task<IProjectModel> GetByName(string name, string customerId);
    }
}

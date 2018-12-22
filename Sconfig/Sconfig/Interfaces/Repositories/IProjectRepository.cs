using Sconfig.Interfaces.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sconfig.Interfaces.Repositories
{
    public interface IProjectRepository : IRepo<IProjectModel>
    {
        Task<IProjectModel> GetByName(string name, string customerId);
        Task<IEnumerable<IProjectModel>> GetByCustomer(string customerId);
    }
}

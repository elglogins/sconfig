using Sconfig.Interfaces.Models;
using System.Threading.Tasks;

namespace Sconfig.Interfaces.Repositories
{
    public interface ICustomerRepository : IRepo<ICustomerModel>
    {
        Task<ICustomerModel> GetByName(string name);
    }
}

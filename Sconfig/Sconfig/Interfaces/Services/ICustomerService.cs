using Sconfig.Contracts.Customer;
using Sconfig.Contracts.Customer.Reads;
using System.Threading.Tasks;

namespace Sconfig.Interfaces.Services
{
    public interface ICustomerService
    {
        Task<CustomerContract> Get(string id);

        Task<CustomerContract> Create(CreateCustomerContract contract);
    }
}

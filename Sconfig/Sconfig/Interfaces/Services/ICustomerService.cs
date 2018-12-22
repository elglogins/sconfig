using Sconfig.Contracts.Customer;
using Sconfig.Contracts.Customer.Reads;
using Sconfig.Contracts.Customer.Writes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sconfig.Interfaces.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerContract>> GetAll();

        Task<CustomerContract> Get(string id);

        Task<CustomerContract> Create(CreateCustomerContract contract);

        Task<CustomerContract> Enable(string id);

        Task<CustomerContract> Disable(string id);

        Task<CustomerContract> Edit(EditCustomerContract contract);
    }
}

using Sconfig.Contracts.Customer.Reads;
using Sconfig.Interfaces.Models;

namespace Sconfig.Interfaces.Mapping
{
    public interface ICustomerMapper
    {
        CustomerContract Map(ICustomerModel model);
    }
}

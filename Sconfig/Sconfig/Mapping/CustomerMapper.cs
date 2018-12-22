using Sconfig.Contracts.Customer.Reads;
using Sconfig.Interfaces.Mapping;
using Sconfig.Interfaces.Models;

namespace Sconfig.Mapping
{
    class CustomerMapper : ICustomerMapper
    {
        public CustomerContract Map(ICustomerModel model)
        {
            if (model == null)
                return null;

            return new CustomerContract
            {
                Id = model.Id,
                Name = model.Name,
                CreatedOn = model.CreatedOn,
                Active = model.Active
            };
        }
    }
}

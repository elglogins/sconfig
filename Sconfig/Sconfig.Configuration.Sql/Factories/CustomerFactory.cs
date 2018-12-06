using Sconfig.Configuration.Sql.Models;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;

namespace Sconfig.Configuration.Sql.Factories
{
    class CustomerFactory : ICustomerFactory
    {
        public ICustomerModel InitCustomerModel()
        {
            return new CustomerModel();
        }
    }
}

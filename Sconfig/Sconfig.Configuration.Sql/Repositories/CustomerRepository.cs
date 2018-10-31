using Sconfig.Configuration.Sql.Interfaces;
using Sconfig.Configuration.Sql.Models;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;

namespace Sconfig.Configuration.Sql.Repositories
{
    class CustomerRepository : AbstractSqlRespository<ICustomerModel, CustomerModel>, ICustomerRepository
    {
        public CustomerRepository(ISconfigSqlConfiguration configuration) : base(configuration)
        {
        }
    }
}

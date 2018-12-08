using Sconfig.Configuration.Sql.Interfaces;
using Sconfig.Configuration.Sql.Models;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;
using System.Threading.Tasks;

namespace Sconfig.Configuration.Sql.Repositories
{
    class CustomerRepository : AbstractSqlRespository<ICustomerModel, CustomerModel>, ICustomerRepository
    {
        public CustomerRepository(ISconfigSqlConfiguration configuration) : base(configuration)
        {
        }

        public async Task<ICustomerModel> GetByName(string name)
        {
            using (var db = GetClient())
            {
                return await db.FirstOrDefaultAsync<CustomerModel>($"SELECT TOP 1 * FROM [{TableName}] WHERE [Name] = @0", name);
            }
        }
    }
}

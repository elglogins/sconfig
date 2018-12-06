using Sconfig.Interfaces.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sconfig.Interfaces.Repositories
{
    public interface IConfigurationGroupRepository : IRepo<IConfigurationGroupModel>
    {
        Task<IConfigurationGroupModel> GetByNameAndByCustomer(string name, string customerId);

        Task<IEnumerable<IConfigurationGroupModel>> GetByParentGroupAndByCustomer(string parentGroupId, string customerId);

        Task<IEnumerable<IConfigurationGroupModel>> GetWithoutParentGroupAndByCustomer(string customerId);
    }
}

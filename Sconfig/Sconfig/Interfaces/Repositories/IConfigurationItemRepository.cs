using System.Collections.Generic;
using System.Threading.Tasks;
using Sconfig.Interfaces.Models;

namespace Sconfig.Interfaces.Repositories
{
    public interface IConfigurationItemRepository : IRepo<IConfigurationItemModel>
    {
        Task<IEnumerable<IConfigurationItemModel>> GetByName(string name, string projectId, string applicationId, string parentId);

        Task<IEnumerable<IConfigurationItemModel>> GetByParent(string parentId, string projectId, string applicationId);

        Task<IEnumerable<IConfigurationItemModel>> GetRootLevelByProject(string projectId);
        Task<IEnumerable<IConfigurationItemModel>> GetByProject(string projectId);

        Task<IEnumerable<IConfigurationItemModel>> GetRootLevelByApplication(string applicationId);
        Task<IEnumerable<IConfigurationItemModel>> GetByApplication(string projectId, string applicationId);
    }
}

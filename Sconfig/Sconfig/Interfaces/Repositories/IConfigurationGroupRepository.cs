using System.Collections.Generic;
using System.Threading.Tasks;
using Sconfig.Interfaces.Models;

namespace Sconfig.Interfaces.Repositories
{
    public interface IConfigurationGroupRepository : IRepo<IConfigurationGroupModel>
    {
        Task<IConfigurationGroupModel> GetByNameAndByProject(string name, string projectId);

        Task<IEnumerable<IConfigurationGroupModel>> GetByParentGroup(string parentGroupId);

        Task<IEnumerable<IConfigurationGroupModel>> GetRootLevelByProject(string projectId);
        Task<IEnumerable<IConfigurationGroupModel>> GetByProject(string projectId);
        Task<IEnumerable<IConfigurationGroupModel>> GetByApplication(string projectId, string applicationId);
    }
}

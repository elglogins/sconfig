using Sconfig.Interfaces.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sconfig.Interfaces.Repositories
{
    public interface IConfigurationGroupRepository : IRepo<IConfigurationGroupModel>
    {
        Task<IConfigurationGroupModel> GetByNameAndByProject(string name, string projectId);

        Task<IEnumerable<IConfigurationGroupModel>> GetByParentGroup(string parentGroupId);

        Task<IEnumerable<IConfigurationGroupModel>> GetRootLevelByProject(string projectId);
    }
}

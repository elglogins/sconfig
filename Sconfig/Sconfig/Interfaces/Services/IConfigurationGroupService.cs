using System.Threading.Tasks;
using Sconfig.Contracts.Configuration.ConfigurationGroup.Reads;
using Sconfig.Contracts.Configuration.ConfigurationGroup.Writes;

namespace Sconfig.Interfaces.Services
{
    public interface IConfigurationGroupService
    {
        Task<ConfigurationGroupContract> Create(CreateConfigurationGroupContract contract);

        Task<ConfigurationGroupContract> Get(string id, string projectId, string applicationId);

        Task Delete(string id, string projectId, string applicationId);

        Task<ConfigurationGroupContract> Edit(EditConfigurationGroupContract contract);
    }
}

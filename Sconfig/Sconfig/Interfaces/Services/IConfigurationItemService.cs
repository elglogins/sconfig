using System.Threading.Tasks;
using Sconfig.Contracts.Configuration.ConfigurationItem.Reads;
using Sconfig.Contracts.Configuration.ConfigurationItem.Writes;

namespace Sconfig.Interfaces.Services
{
    public interface IConfigurationItemService
    {
        Task<ConfigurationItemContract> Create(CreateConfigurationItemContract contract);

        Task<ConfigurationItemContract> Get(string id, string projectId, string applicationId, string environmentId);

        Task Delete(string id, string projectId, string applicationId, string environmentId);

        Task<ConfigurationItemContract> Edit(EditConfigurationItemContract contract);
    }
}

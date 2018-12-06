using System.Threading.Tasks;
using Sconfig.Contracts.Configuration.ConfigurationGroup.Reads;
using Sconfig.Contracts.Configuration.ConfigurationGroup.Writes;

namespace Sconfig.Interfaces.Services
{
    public interface IConfigurationGroupService
    {
        Task<ConfigurationGroupContract> Create(CreateConfigurationGroupContract contract, string customerId);
    }
}
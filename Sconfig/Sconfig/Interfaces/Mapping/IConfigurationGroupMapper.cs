using Sconfig.Contracts.Configuration.ConfigurationGroup.Reads;
using Sconfig.Interfaces.Models;

namespace Sconfig.Mapping
{
    public interface IConfigurationGroupMapper
    {
        ConfigurationGroupContract Map(IConfigurationGroupModel model);
    }
}

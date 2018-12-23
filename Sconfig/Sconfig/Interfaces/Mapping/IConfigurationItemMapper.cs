using Sconfig.Contracts.Configuration.ConfigurationItem.Reads;
using Sconfig.Interfaces.Models;

namespace Sconfig.Interfaces.Mapping
{
    public interface IConfigurationItemMapper
    {
        ConfigurationItemContract Map(IConfigurationItemModel model);
    }
}

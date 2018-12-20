using Sconfig.Interfaces.Models;

namespace Sconfig.Interfaces.Factories
{
    public interface IConfigurationItemFactory
    {
        IConfigurationItemModel InitConfigurationItemModel();
    }
}

using Sconfig.Configuration.Sql.Models;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;

namespace Sconfig.Configuration.Sql.Factories
{
    class ConfigurationItemFactory : IConfigurationItemFactory
    {
        public IConfigurationItemModel InitConfigurationItemModel()
        {
            return new ConfigurationItemModel();
        }
    }
}

using Sconfig.Configuration.Sql.Models;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;

namespace Sconfig.Configuration.Sql.Factories
{
    class ConfigurationGroupFactory : IConfigurationGroupFactory
    {
        public IConfigurationGroupModel InitConfigurationGroupModel()
        {
            return new ConfigurationGroupModel();
        }
    }
}

using Sconfig.Configuration.Sql.Models;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;

namespace Sconfig.Configuration.Sql.Factories
{
    public class EnvironmentFactory : IEnvironmentFactory
    {
        public IEnvironmentModel InitEnvironmentModel()
        {
            return new EnvironmentModel();
        }
    }
}

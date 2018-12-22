using Sconfig.Configuration.Sql.Models;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;

namespace Sconfig.Configuration.Sql.Factories
{
    class ApplicationFactory : IApplicationFactory
    {
        public IApplicationModel InitApplicationModel()
        {
            return new ApplicationModel();
        }
    }
}

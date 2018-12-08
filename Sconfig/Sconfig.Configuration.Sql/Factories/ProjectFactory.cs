using Sconfig.Configuration.Sql.Models;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;

namespace Sconfig.Configuration.Sql.Factories
{
    class ProjectFactory : IProjectFactory
    {
        public IProjectModel InitProjectModel()
        {
            return new ProjectModel();
        }
    }
}

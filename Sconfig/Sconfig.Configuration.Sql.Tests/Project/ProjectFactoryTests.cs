using Sconfig.Configuration.Sql.Factories;
using Sconfig.Interfaces.Factories;
using Xunit;

namespace Sconfig.Configuration.Sql.Tests.Project
{
    public class ProjectFactoryTests
    {
        private readonly IProjectFactory _projectFactory;

        public ProjectFactoryTests()
        {
            _projectFactory = new ProjectFactory();
        }

        [Fact]
        public void InitNewProjectModel()
        {
            var result = _projectFactory.InitProjectModel();
            Assert.NotNull(result);
        }
    }
}

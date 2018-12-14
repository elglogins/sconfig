using Sconfig.Configuration.Sql.Factories;
using Sconfig.Interfaces.Factories;
using Xunit;

namespace Sconfig.Configuration.Sql.Tests.Environment
{
    public class EnvironmentFactoryTests
    {
        private readonly IEnvironmentFactory _projectFactory;
        public EnvironmentFactoryTests()
        {
            _projectFactory = new EnvironmentFactory();
        }

        [Fact]
        public void InitNewProjectModel()
        {
            var result = _projectFactory.InitEnvironmentModel();
            Assert.NotNull(result);
        }
    }
}

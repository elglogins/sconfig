using Moq;
using Sconfig.Configuration.Sql.Models;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;
using Sconfig.Interfaces.Services;
using Sconfig.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Sconfig.Configuration.Sql.Tests.Project
{
    public class ProjectServiceTests
    {
        // services, factories
        private readonly IProjectService _projectService;

        // repository entities
        private readonly IProjectModel _storedProjectModel;

        public ProjectServiceTests()
        {
            // repository entities
            _storedProjectModel = new ProjectModel()
            {
                CreatedOn = DateTime.Now,
                Id = "TEST-PROJECT-1",
                Name = "TEST PROJECT",
                CustomerId = "TEST-CUSTOMER-1"
            };

            // services, factories
            var projectFactoryMock = new Mock<IProjectFactory>();
            projectFactoryMock
               .Setup(_ => _.InitProjectModel())
               .Returns(new ProjectModel());

            // reads
            var projectRepositoryMock = new Mock<IProjectRepository>();
            projectRepositoryMock
               .Setup(_ => _.Get(It.Is<string>(s => s == _storedProjectModel.Id)))
               .Returns(Task.FromResult(_storedProjectModel));

            projectRepositoryMock
             .Setup(_ => _.GetByName(It.Is<string>(s => s == _storedProjectModel.Name), It.Is<string>(s => s == _storedProjectModel.CustomerId)))
             .Returns(Task.FromResult(_storedProjectModel));

            _projectService = new ProjectService(projectFactoryMock.Object, projectRepositoryMock.Object);
        }

        [Fact]
        public void GetExisting()
        {
            var result = _projectService.Get(_storedProjectModel.Id, _storedProjectModel.CustomerId).Result;

            Assert.NotNull(result);
            Assert.Equal(_storedProjectModel.Id, result.Id);
            Assert.Equal(_storedProjectModel.Name, result.Name);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(" ", null)]
        [InlineData(null, " ")]
        public void GetWithNullIdentifiers(string projectId, string customerId)
        {
            var result = _projectService.Get(projectId, customerId).Result;
            Assert.Null(result);
        }

        [Fact]
        public void GetNotExisting()
        {
            var result = _projectService.Get("NOT-EXISTING-PROJECT-ID", _storedProjectModel.CustomerId).Result;
            Assert.Null(result);
        }
    }
}

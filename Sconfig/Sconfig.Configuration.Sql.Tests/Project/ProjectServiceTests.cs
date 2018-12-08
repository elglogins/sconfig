using Moq;
using Sconfig.Configuration.Sql.Models;
using Sconfig.Contracts.Project.Enums;
using Sconfig.Contracts.Project.Writes;
using Sconfig.Exceptions;
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

            // writes
            projectRepositoryMock
               .Setup(_ => _.Insert(It.IsAny<IProjectModel>()))
               .Returns<IProjectModel>(x => Task.FromResult(x));

            projectRepositoryMock
               .Setup(_ => _.Save(It.IsAny<IProjectModel>()))
               .Returns<IProjectModel>(x => x);

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

        [Theory]
        [InlineData("MY-TEST-PROJECT")]
        [InlineData("ThisIsEdgeValueOfAllowedLength12345678901234567890")]
        public void Create(string name)
        {
            var contract = new CreateProjectContract()
            {
                Name = name
            };

            var result = _projectService.Create(contract, _storedProjectModel.CustomerId).Result;
            Assert.NotNull(result);
            Assert.NotNull(result.Id);
            Assert.Equal(contract.Name, result.Name);
            Assert.NotEqual(DateTime.MinValue, result.CreatedOn);
        }

        [Fact]
        public async Task CreateAlreadyExisting()
        {
            var contract = new CreateProjectContract()
            {
                Name = _storedProjectModel.Name
            };

            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => _projectService.Create(contract, _storedProjectModel.CustomerId));
            Assert.Equal(ProjectValidationCode.PROJECT_ALREADY_EXISTS.ToString(), exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ThisNameIsLongerThanIsAllowed!!12345678901234567890")]
        public async Task CreateWithInvalidName(string name)
        {
            var contract = new CreateProjectContract()
            {
                Name = name
            };

            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => _projectService.Create(contract, _storedProjectModel.CustomerId));
            Assert.Equal(ProjectValidationCode.INVALID_PROJECT_NAME.ToString(), exception.Message);
        }
    }
}

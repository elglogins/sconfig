using System;
using System.Threading.Tasks;
using Moq;
using Sconfig.Contracts.Environment.Enums;
using Sconfig.Contracts.Environment.Writes;
using Sconfig.Exceptions;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;
using Sconfig.Interfaces.Services;
using Sconfig.Services;
using Sconfig.Tests.Models;
using Xunit;

namespace Sconfig.Configuration.Sql.Tests.Environment
{
    public class EnvironmentServiceTests
    {
        // services, factories
        private readonly IEnvironmentService _environmentService;

        // repository entities
        private readonly IEnvironmentModel _storedEnvironmentModel;

        public EnvironmentServiceTests()
        {
            // repository entities
            _storedEnvironmentModel = new EnvironmentTestModel()
            {
                CreatedOn = DateTime.Now,
                Id = "TEST-ENVIRONMENT-1",
                Name = "TEST ENVIRONMENT",
                ProjectId = "TEST-PROJECT-1"
            };

            // services, factories
            var environmentFactoryMock = new Mock<IEnvironmentFactory>();
            environmentFactoryMock
               .Setup(_ => _.InitEnvironmentModel())
               .Returns(new EnvironmentTestModel());

            // reads
            var environmentRepositoryMock = new Mock<IEnvironmentRepository>();
            environmentRepositoryMock
               .Setup(_ => _.Get(It.Is<string>(s => s == _storedEnvironmentModel.Id)))
               .Returns(Task.FromResult(_storedEnvironmentModel));

            environmentRepositoryMock
             .Setup(_ => _.GetByName(It.Is<string>(s => s == _storedEnvironmentModel.Name),
                It.Is<string>(s => s == _storedEnvironmentModel.ProjectId)))
             .Returns(Task.FromResult(_storedEnvironmentModel));

            // writes
            environmentRepositoryMock
               .Setup(_ => _.Insert(It.IsAny<IEnvironmentModel>()))
               .Returns<IEnvironmentModel>(x => Task.FromResult(x));

            environmentRepositoryMock
               .Setup(_ => _.Save(It.IsAny<IEnvironmentModel>()))
               .Returns<IEnvironmentModel>(x => x);

            _environmentService = new EnvironmentService(environmentRepositoryMock.Object, environmentFactoryMock.Object);
        }

        [Fact]
        public void GetExisting()
        {
            var result = _environmentService.Get(_storedEnvironmentModel.Id, _storedEnvironmentModel.ProjectId).Result;

            Assert.NotNull(result);
            Assert.Equal(_storedEnvironmentModel.Id, result.Id);
            Assert.Equal(_storedEnvironmentModel.Name, result.Name);
        }

        [Fact]
        public void GetForWrongOwnerProject()
        {
            var result = _environmentService.Get(_storedEnvironmentModel.Id, "NOT-OWNER-PROJECT-ID").Result;
            Assert.Null(result);
        }

        [Fact]
        public void GetNotExisting()
        {
            var result = _environmentService.Get("NOT-EXISTING-ENVIRONMENT-ID", _storedEnvironmentModel.ProjectId).Result;
            Assert.Null(result);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(" ", null)]
        [InlineData(null, " ")]
        public void GetWithNullIdentifiers(string environmentId, string projectId)
        {
            var result = _environmentService.Get(environmentId, projectId).Result;
            Assert.Null(result);
        }

        [Theory]
        [InlineData("MY-TEST-ENVIRONMENT")]
        [InlineData("ThisIsEdgeValueOfAllowedLength12345678901234567890")]
        public void Create(string name)
        {
            var contract = new CreateEnvironmentContract()
            {
                Name = name,
                ProjectId = _storedEnvironmentModel.ProjectId
            };

            var result = _environmentService.Create(contract).Result;
            Assert.NotNull(result);
            Assert.NotNull(result.Id);
            Assert.Equal(contract.Name, result.Name);
            Assert.NotEqual(DateTime.MinValue, result.CreatedOn);
        }

        [Fact]
        public async Task CreateAlreadyExisting()
        {
            var contract = new CreateEnvironmentContract()
            {
                Name = _storedEnvironmentModel.Name,
                ProjectId = _storedEnvironmentModel.ProjectId
            };

            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => _environmentService.Create(contract));
            Assert.Equal(EnvironmentValidationCode.ENVIRONMENT_ALREADY_EXISTS.ToString(), exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ThisNameIsLongerThanIsAllowed!!12345678901234567890")]
        public async Task CreateWithInvalidName(string name)
        {
            var contract = new CreateEnvironmentContract()
            {
                Name = name,
                ProjectId = _storedEnvironmentModel.ProjectId
            };

            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => _environmentService.Create(contract));
            Assert.Equal(EnvironmentValidationCode.INVALID_ENVIRONMENT_NAME.ToString(), exception.Message);
        }
    }
}

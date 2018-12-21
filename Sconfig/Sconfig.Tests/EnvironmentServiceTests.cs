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

namespace Sconfig.Tests
{
    public class EnvironmentServiceTests
    {
        #region Privates / mocking

        private IEnvironmentModel DefaultEnvironmentModel
        {
            get
            {
                return new EnvironmentTestModel()
                {
                    CreatedOn = DateTime.Now,
                    Id = "E-TEST-ENVIRONMENT-1",
                    Name = "TEST ENVIRONMENT",
                    ProjectId = "TEST-PROJECT-1"
                };
            }
        }

        private Mock<IEnvironmentFactory> DefaultEnvironmentFactoryMock
        {
            get
            {
                var environmentFactoryMock = new Mock<IEnvironmentFactory>();
                environmentFactoryMock
                   .Setup(_ => _.InitEnvironmentModel())
                   .Returns(new EnvironmentTestModel());
                return environmentFactoryMock;
            }
        }

        private Mock<IEnvironmentRepository> DefaultEnvironmentRepositoryMock
        {
            get
            {
                // reads
                var environmentRepositoryMock = new Mock<IEnvironmentRepository>();
                environmentRepositoryMock
                   .Setup(_ => _.Get(It.Is<string>(s => s == DefaultEnvironmentModel.Id)))
                   .Returns(Task.FromResult(DefaultEnvironmentModel));

                environmentRepositoryMock
                 .Setup(_ => _.GetByName(It.Is<string>(s => s == DefaultEnvironmentModel.Name),
                    It.Is<string>(s => s == DefaultEnvironmentModel.ProjectId)))
                 .Returns(Task.FromResult(DefaultEnvironmentModel));

                // writes
                environmentRepositoryMock
                   .Setup(_ => _.Insert(It.IsAny<IEnvironmentModel>()))
                   .Returns<IEnvironmentModel>(x => Task.FromResult(x));

                environmentRepositoryMock
                   .Setup(_ => _.Save(It.IsAny<IEnvironmentModel>()))
                   .Returns<IEnvironmentModel>(x => x);

                return environmentRepositoryMock;
            }
        }

        #endregion

        [Fact]
        public void GetExisting()
        {
            var environmentService = new EnvironmentService(DefaultEnvironmentRepositoryMock.Object, DefaultEnvironmentFactoryMock.Object);
            var result = environmentService.Get(DefaultEnvironmentModel.Id, DefaultEnvironmentModel.ProjectId).Result;

            Assert.NotNull(result);
            Assert.Equal(DefaultEnvironmentModel.Id, result.Id);
            Assert.Equal(DefaultEnvironmentModel.Name, result.Name);
        }

        [Fact]
        public void GetForWrongOwnerProject()
        {
            var environmentService = new EnvironmentService(DefaultEnvironmentRepositoryMock.Object, DefaultEnvironmentFactoryMock.Object);
            var result = environmentService.Get(DefaultEnvironmentModel.Id, "NOT-OWNER-PROJECT-ID").Result;
            Assert.Null(result);
        }

        [Fact]
        public void GetNotExisting()
        {
            var environmentService = new EnvironmentService(DefaultEnvironmentRepositoryMock.Object, DefaultEnvironmentFactoryMock.Object);
            var result = environmentService.Get("NOT-EXISTING-ENVIRONMENT-ID", DefaultEnvironmentModel.ProjectId).Result;
            Assert.Null(result);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(" ", null)]
        [InlineData(null, " ")]
        public void GetWithNullIdentifiers(string environmentId, string projectId)
        {
            var environmentService = new EnvironmentService(DefaultEnvironmentRepositoryMock.Object, DefaultEnvironmentFactoryMock.Object);
            var result = environmentService.Get(environmentId, projectId).Result;
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
                ProjectId = DefaultEnvironmentModel.ProjectId
            };

            var environmentRepositoryMock = DefaultEnvironmentRepositoryMock;
            var environmentService = new EnvironmentService(environmentRepositoryMock.Object, DefaultEnvironmentFactoryMock.Object);
            var result = environmentService.Create(contract).Result;
            Assert.NotNull(result);
            Assert.NotNull(result.Id);
            Assert.Equal(contract.Name, result.Name);
            Assert.NotEqual(DateTime.MinValue, result.CreatedOn);
            environmentRepositoryMock.Verify(m => m.Insert(It.IsAny<IEnvironmentModel>()), Times.Once);
        }

        [Fact]
        public async Task CreateAlreadyExisting()
        {
            var contract = new CreateEnvironmentContract()
            {
                Name = DefaultEnvironmentModel.Name,
                ProjectId = DefaultEnvironmentModel.ProjectId
            };

            var environmentService = new EnvironmentService(DefaultEnvironmentRepositoryMock.Object, DefaultEnvironmentFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => environmentService.Create(contract));
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
                ProjectId = DefaultEnvironmentModel.ProjectId
            };

            var environmentService = new EnvironmentService(DefaultEnvironmentRepositoryMock.Object, DefaultEnvironmentFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => environmentService.Create(contract));
            Assert.Equal(EnvironmentValidationCode.INVALID_ENVIRONMENT_NAME.ToString(), exception.Message);
        }

        [Theory]
        [InlineData("MY-NEW-ENVIRONMENT-NAME")]
        [InlineData("ThisIsEdgeValueOfAllowedLength12345678901234567890")]
        public async Task Edit(string name)
        {
            var editContract = new EditEnvironmentContract()
            {
                Id = DefaultEnvironmentModel.Id,
                Name = name
            };

            var environmentRepositoryMock = DefaultEnvironmentRepositoryMock;
            var environmentService = new EnvironmentService(environmentRepositoryMock.Object, DefaultEnvironmentFactoryMock.Object);
            var result = await environmentService.Edit(editContract, DefaultEnvironmentModel.ProjectId);
            Assert.NotNull(result);
            Assert.Equal(editContract.Name, result.Name);
            environmentRepositoryMock.Verify(m => m.Save(It.Is<IEnvironmentModel>(p => p.Id == editContract.Id)), Times.Once);
        }

        [Fact]
        public async Task EditNotExisting()
        {
            var contract = new EditEnvironmentContract()
            {
                Id = "NOT-EXISTING-ENVIRONMENT-ID",
                Name = "ThisIsNewAndValidName"
            };

            var environmentService = new EnvironmentService(DefaultEnvironmentRepositoryMock.Object, DefaultEnvironmentFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => environmentService.Edit(contract, DefaultEnvironmentModel.ProjectId));
            Assert.Equal(EnvironmentValidationCode.ENVIRONMENT_DOES_NOT_EXIST.ToString(), exception.Message);
        }

        [Fact]
        public async Task EditForInvalid()
        {
            var contract = new EditEnvironmentContract()
            {
                Id = DefaultEnvironmentModel.Id,
                Name = "ThisIsNewAndValidName"
            };

            var environmentService = new EnvironmentService(DefaultEnvironmentRepositoryMock.Object, DefaultEnvironmentFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => environmentService.Edit(contract, "INVALID-PROJECT-ID"));
            Assert.Equal(EnvironmentValidationCode.INVALID_ENVIRONMENT_PROJECT.ToString(), exception.Message);
        }

        [Fact]
        public async Task EditAlreadyExistingName()
        {
            var contract = new EditEnvironmentContract()
            {
                Id = DefaultEnvironmentModel.Id,
                Name = DefaultEnvironmentModel.Name
            };

            var environmentService = new EnvironmentService(DefaultEnvironmentRepositoryMock.Object, DefaultEnvironmentFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => environmentService.Edit(contract, DefaultEnvironmentModel.ProjectId));
            Assert.Equal(EnvironmentValidationCode.ENVIRONMENT_ALREADY_EXISTS.ToString(), exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ThisNameIsLongerThanIsAllowed!!12345678901234567890")]
        public async Task EditWithInvalidName(string name)
        {
            var contract = new EditEnvironmentContract()
            {
                Name = name,
                Id = DefaultEnvironmentModel.Id
            };

            var environmentService = new EnvironmentService(DefaultEnvironmentRepositoryMock.Object, DefaultEnvironmentFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => environmentService.Edit(contract, DefaultEnvironmentModel.ProjectId));
            Assert.Equal(EnvironmentValidationCode.INVALID_ENVIRONMENT_NAME.ToString(), exception.Message);
        }


        [Fact]
        public async Task Delete()
        {
            var environmentRepositoryMock = DefaultEnvironmentRepositoryMock;
            var environmentService = new EnvironmentService(environmentRepositoryMock.Object, DefaultEnvironmentFactoryMock.Object);
            await environmentService.Delete(DefaultEnvironmentModel.Id, DefaultEnvironmentModel.ProjectId);
            environmentRepositoryMock.Verify(m => m.Delete(It.Is<string>(p => p == DefaultEnvironmentModel.Id)), Times.Once);
        }

        [Theory]
        // environment id invalid
        [InlineData(null, "PROJECT-ID")]
        [InlineData("", "PROJECT-ID")]
        [InlineData(" ", "PROJECT-ID")]
        // project id invalid
        [InlineData("ENVIRONMENT-ID", null)]
        [InlineData("ENVIRONMENT-ID", "")]
        [InlineData("ENVIRONMENT-ID", " ")]
        public async Task DeleteUsingEmptyIds(string environmentId, string projectId)
        {
            var environmentService = new EnvironmentService(DefaultEnvironmentRepositoryMock.Object, DefaultEnvironmentFactoryMock.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => environmentService.Delete(environmentId, projectId));
        }

        [Theory]
        [InlineData("NOT-EXISTING-ENVIRONMENT-ID", "TEST-PROJECT-1", EnvironmentValidationCode.ENVIRONMENT_DOES_NOT_EXIST)]
        [InlineData("E-TEST-ENVIRONMENT-1", "INVALID-PROJECT-ID", EnvironmentValidationCode.INVALID_ENVIRONMENT_PROJECT)]
        public async Task DeleteInvalid(string environmentId, string projectId, Enum exceptionMessage)
        {
            var environmentService = new EnvironmentService(DefaultEnvironmentRepositoryMock.Object, DefaultEnvironmentFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => environmentService.Delete(environmentId, projectId));
            Assert.Equal(exceptionMessage.ToString(), exception.Message);
        }
    }
}

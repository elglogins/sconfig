using System;
using System.Threading.Tasks;
using Moq;
using Sconfig.Contracts.Application.Enums;
using Sconfig.Contracts.Application.Writes;
using Sconfig.Exceptions;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;
using Sconfig.Services;
using Sconfig.Tests.Models;
using Xunit;

namespace Sconfig.Tests
{
    public class ApplicationServiceTests
    {
        #region Privates / mocking

        private IApplicationModel DefaultApplicationModel
        {
            get
            {
                return new ApplicationTestModel()
                {
                    CreatedOn = DateTime.Now,
                    Id = "TEST-APPLICATION-1",
                    Name = "TEST APPLICATION",
                    ProjectId = "TEST-PROJECT-1"
                };
            }
        }

        private Mock<IApplicationFactory> DefaultApplicationFactoryMock
        {
            get
            {
                var applicationFactoryMock = new Mock<IApplicationFactory>();
                applicationFactoryMock
                   .Setup(_ => _.InitApplicationModel())
                   .Returns(new ApplicationTestModel());
                return applicationFactoryMock;
            }
        }

        private Mock<IApplicationRepository> DefaultApplicationRepositoryMock
        {
            get
            {
                // reads
                var applicationRepositoryMock = new Mock<IApplicationRepository>();
                applicationRepositoryMock
                   .Setup(_ => _.Get(It.Is<string>(s => s == DefaultApplicationModel.Id)))
                   .Returns(Task.FromResult(DefaultApplicationModel));

                applicationRepositoryMock
                 .Setup(_ => _.GetByName(It.Is<string>(s => s == DefaultApplicationModel.Name),
                    It.Is<string>(s => s == DefaultApplicationModel.ProjectId)))
                 .Returns(Task.FromResult(DefaultApplicationModel));

                // writes
                applicationRepositoryMock
                   .Setup(_ => _.Insert(It.IsAny<IApplicationModel>()))
                   .Returns<IApplicationModel>(x => Task.FromResult(x));

                applicationRepositoryMock
                   .Setup(_ => _.Save(It.IsAny<IApplicationModel>()))
                   .Returns<IApplicationModel>(x => x);

                return applicationRepositoryMock;
            }
        }

        #endregion

        [Fact]
        public void GetExisting()
        {
            var applicationService = new ApplicationService(DefaultApplicationRepositoryMock.Object, DefaultApplicationFactoryMock.Object);
            var result = applicationService.Get(DefaultApplicationModel.Id, DefaultApplicationModel.ProjectId).Result;

            Assert.NotNull(result);
            Assert.Equal(DefaultApplicationModel.Id, result.Id);
            Assert.Equal(DefaultApplicationModel.Name, result.Name);
        }

        [Fact]
        public void GetForWrongOwnerProject()
        {
            var applicationService = new ApplicationService(DefaultApplicationRepositoryMock.Object, DefaultApplicationFactoryMock.Object);
            var result = applicationService.Get(DefaultApplicationModel.Id, "NOT-OWNER-PROJECT-ID").Result;
            Assert.Null(result);
        }

        [Fact]
        public void GetNotExisting()
        {
            var applicationService = new ApplicationService(DefaultApplicationRepositoryMock.Object, DefaultApplicationFactoryMock.Object);
            var result = applicationService.Get("NOT-EXISTING-APPLICATION-ID", DefaultApplicationModel.ProjectId).Result;
            Assert.Null(result);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(" ", null)]
        [InlineData(null, " ")]
        public void GetWithNullIdentifiers(string appicationId, string projectId)
        {
            var applicationService = new ApplicationService(DefaultApplicationRepositoryMock.Object, DefaultApplicationFactoryMock.Object);
            var result = applicationService.Get(appicationId, projectId).Result;
            Assert.Null(result);
        }

        [Theory]
        [InlineData("MY-TEST-APPLICATION")]
        [InlineData("ThisIsEdgeValueOfAllowedLength12345678901234567890")]
        public void Create(string name)
        {
            var contract = new CreateApplicationContract()
            {
                Name = name,
                ProjectId = DefaultApplicationModel.ProjectId
            };

            var applicationRepositoryMock = DefaultApplicationRepositoryMock;
            var applicationService = new ApplicationService(applicationRepositoryMock.Object, DefaultApplicationFactoryMock.Object);
            var result = applicationService.Create(contract).Result;
            Assert.NotNull(result);
            Assert.NotNull(result.Id);
            Assert.Equal(contract.Name, result.Name);
            Assert.NotEqual(DateTime.MinValue, result.CreatedOn);
            applicationRepositoryMock.Verify(m => m.Insert(It.IsAny<IApplicationModel>()), Times.Once);
        }

        [Fact]
        public async Task CreateAlreadyExisting()
        {
            var contract = new CreateApplicationContract()
            {
                Name = DefaultApplicationModel.Name,
                ProjectId = DefaultApplicationModel.ProjectId
            };

            var applicationService = new ApplicationService(DefaultApplicationRepositoryMock.Object, DefaultApplicationFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => applicationService.Create(contract));
            Assert.Equal(ApplicationValidationCodes.APPLICATION_ALREADY_EXISTS.ToString(), exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ThisNameIsLongerThanIsAllowed!!12345678901234567890")]
        public async Task CreateWithInvalidName(string name)
        {
            var contract = new CreateApplicationContract()
            {
                Name = name,
                ProjectId = DefaultApplicationModel.ProjectId
            };

            var applicationService = new ApplicationService(DefaultApplicationRepositoryMock.Object, DefaultApplicationFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => applicationService.Create(contract));
            Assert.Equal(ApplicationValidationCodes.INVALID_APPLICATION_NAME.ToString(), exception.Message);
        }

        [Theory]
        [InlineData("MY-NEW-APPLICATION-NAME")]
        [InlineData("ThisIsEdgeValueOfAllowedLength12345678901234567890")]
        public async Task Edit(string name)
        {
            var editContract = new EditApplicationContract()
            {
                Id = DefaultApplicationModel.Id,
                Name = name
            };

            var applicationRepositoryMock = DefaultApplicationRepositoryMock;
            var applicationService = new ApplicationService(applicationRepositoryMock.Object, DefaultApplicationFactoryMock.Object);
            var result = await applicationService.Edit(editContract, DefaultApplicationModel.ProjectId);
            Assert.NotNull(result);
            Assert.Equal(editContract.Name, result.Name);
            applicationRepositoryMock.Verify(m => m.Save(It.Is<IApplicationModel>(p => p.Id == editContract.Id)), Times.Once);
        }

        [Fact]
        public async Task EditNotExisting()
        {
            var contract = new EditApplicationContract()
            {
                Id = "NOT-EXISTING-APPLICATION-ID",
                Name = "ThisIsNewAndValidName"
            };

            var applicationService = new ApplicationService(DefaultApplicationRepositoryMock.Object, DefaultApplicationFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => applicationService.Edit(contract, DefaultApplicationModel.ProjectId));
            Assert.Equal(ApplicationValidationCodes.APPLICATION_DOES_NOT_EXIST.ToString(), exception.Message);
        }

        [Fact]
        public async Task EditForInvalid()
        {
            var contract = new EditApplicationContract()
            {
                Id = DefaultApplicationModel.Id,
                Name = "ThisIsNewAndValidName"
            };

            var applicationService = new ApplicationService(DefaultApplicationRepositoryMock.Object, DefaultApplicationFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => applicationService.Edit(contract, "INVALID-PROJECT-ID"));
            Assert.Equal(ApplicationValidationCodes.INVALID_APPLICATION_PROJECT.ToString(), exception.Message);
        }

        [Fact]
        public async Task EditAlreadyExistingName()
        {
            var contract = new EditApplicationContract()
            {
                Id = DefaultApplicationModel.Id,
                Name = DefaultApplicationModel.Name
            };

            var applicationService = new ApplicationService(DefaultApplicationRepositoryMock.Object, DefaultApplicationFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => applicationService.Edit(contract, DefaultApplicationModel.ProjectId));
            Assert.Equal(ApplicationValidationCodes.APPLICATION_ALREADY_EXISTS.ToString(), exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ThisNameIsLongerThanIsAllowed!!12345678901234567890")]
        public async Task EditWithInvalidName(string name)
        {
            var contract = new EditApplicationContract()
            {
                Name = name,
                Id = DefaultApplicationModel.Id
            };

            var applicationService = new ApplicationService(DefaultApplicationRepositoryMock.Object, DefaultApplicationFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => applicationService.Edit(contract, DefaultApplicationModel.ProjectId));
            Assert.Equal(ApplicationValidationCodes.INVALID_APPLICATION_NAME.ToString(), exception.Message);
        }

        [Fact]
        public async Task Delete()
        {
            var applicationRepositoryMock = DefaultApplicationRepositoryMock;
            var applicationService = new ApplicationService(applicationRepositoryMock.Object, DefaultApplicationFactoryMock.Object);
            await applicationService.Delete(DefaultApplicationModel.Id, DefaultApplicationModel.ProjectId);
            applicationRepositoryMock.Verify(m => m.Delete(It.Is<string>(p => p == DefaultApplicationModel.Id)), Times.Once);
        }

        [Theory]
        // application id invalid
        [InlineData(null, "PROJECT-ID")]
        [InlineData("", "PROJECT-ID")]
        [InlineData(" ", "PROJECT-ID")]
        // project id invalid
        [InlineData("APPLICATION-ID", null)]
        [InlineData("APPLICATION-ID", "")]
        [InlineData("APPLICATION-ID", " ")]
        public async Task DeleteUsingEmptyIds(string applicationId, string projectId)
        {
            var applicationService = new ApplicationService(DefaultApplicationRepositoryMock.Object, DefaultApplicationFactoryMock.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => applicationService.Delete(applicationId, projectId));
        }

        [Theory]
        [InlineData("NOT-EXISTING-APPLICATION-ID", "TEST-PROJECT-1", ApplicationValidationCodes.APPLICATION_DOES_NOT_EXIST)]
        [InlineData("TEST-APPLICATION-1", "INVALID-PROJECT-ID", ApplicationValidationCodes.INVALID_APPLICATION_PROJECT)]
        public async Task DeleteInvalid(string applicationId, string projectId, Enum exceptionMessage)
        {
            var applicationService = new ApplicationService(DefaultApplicationRepositoryMock.Object, DefaultApplicationFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => applicationService.Delete(applicationId, projectId));
            Assert.Equal(exceptionMessage.ToString(), exception.Message);
        }
    }
}

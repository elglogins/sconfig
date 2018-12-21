using System;
using System.Threading.Tasks;
using Moq;
using Sconfig.Contracts.Project.Enums;
using Sconfig.Contracts.Project.Writes;
using Sconfig.Exceptions;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;
using Sconfig.Services;
using Sconfig.Tests.Models;
using Xunit;

namespace Sconfig.Tests
{
    public class ProjectServiceTests
    {
        #region Privates / mocking

        private IProjectModel DefaultProjectModel
        {
            get
            {
                return new ProjectTestModel()
                {
                    CreatedOn = DateTime.Now,
                    Id = "P-TEST-PROJECT-1",
                    Name = "TEST PROJECT",
                    CustomerId = "TEST-CUSTOMER-1"
                };
            }
        }

        private Mock<IProjectFactory> DefaultProjectFactoryMock
        {
            get
            {
                var projectFactoryMock = new Mock<IProjectFactory>();
                projectFactoryMock
                   .Setup(_ => _.InitProjectModel())
                   .Returns(new ProjectTestModel());
                return projectFactoryMock;
            }
        }

        private Mock<IProjectRepository> DefaultProjectRepositoryMock
        {
            get
            {
                // reads
                var projectRepositoryMock = new Mock<IProjectRepository>();
                projectRepositoryMock
                   .Setup(_ => _.Get(It.Is<string>(s => s == DefaultProjectModel.Id)))
                   .Returns(Task.FromResult(DefaultProjectModel));

                projectRepositoryMock
                 .Setup(_ => _.GetByName(It.Is<string>(s => s == DefaultProjectModel.Name), It.Is<string>(s => s == DefaultProjectModel.CustomerId)))
                 .Returns(Task.FromResult(DefaultProjectModel));

                // writes
                projectRepositoryMock
                   .Setup(_ => _.Insert(It.IsAny<IProjectModel>()))
                   .Returns<IProjectModel>(x => Task.FromResult(x));

                projectRepositoryMock
                   .Setup(_ => _.Save(It.IsAny<IProjectModel>()))
                   .Returns<IProjectModel>(x => x);

                return projectRepositoryMock;
            }
        }

        #endregion

        [Fact]
        public void GetExisting()
        {
            var projectService = new ProjectService(DefaultProjectFactoryMock.Object, DefaultProjectRepositoryMock.Object);
            var result = projectService.Get(DefaultProjectModel.Id, DefaultProjectModel.CustomerId).Result;

            Assert.NotNull(result);
            Assert.Equal(DefaultProjectModel.Id, result.Id);
            Assert.Equal(DefaultProjectModel.Name, result.Name);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(" ", null)]
        [InlineData(null, " ")]
        public void GetWithNullIdentifiers(string projectId, string customerId)
        {
            var projectService = new ProjectService(DefaultProjectFactoryMock.Object, DefaultProjectRepositoryMock.Object);
            var result = projectService.Get(projectId, customerId).Result;
            Assert.Null(result);
        }

        [Fact]
        public void GetNotExisting()
        {
            var projectService = new ProjectService(DefaultProjectFactoryMock.Object, DefaultProjectRepositoryMock.Object);
            var result = projectService.Get("NOT-EXISTING-PROJECT-ID", DefaultProjectModel.CustomerId).Result;
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

            var projectRepositoryMock = DefaultProjectRepositoryMock;
            var projectService = new ProjectService(DefaultProjectFactoryMock.Object, projectRepositoryMock.Object);
            var result = projectService.Create(contract, DefaultProjectModel.CustomerId).Result;
            Assert.NotNull(result);
            Assert.NotNull(result.Id);
            Assert.Equal(contract.Name, result.Name);
            Assert.NotEqual(DateTime.MinValue, result.CreatedOn);
            projectRepositoryMock.Verify(m => m.Insert(It.IsAny<IProjectModel>()), Times.Once);
        }

        [Fact]
        public async Task CreateAlreadyExisting()
        {
            var contract = new CreateProjectContract()
            {
                Name = DefaultProjectModel.Name
            };

            var projectService = new ProjectService(DefaultProjectFactoryMock.Object, DefaultProjectRepositoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => projectService.Create(contract, DefaultProjectModel.CustomerId));
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

            var projectService = new ProjectService(DefaultProjectFactoryMock.Object, DefaultProjectRepositoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => projectService.Create(contract, DefaultProjectModel.CustomerId));
            Assert.Equal(ProjectValidationCode.INVALID_PROJECT_NAME.ToString(), exception.Message);
        }

        [Theory]
        [InlineData("MY-NEW-PROJECT-NAME")]
        [InlineData("ThisIsEdgeValueOfAllowedLength12345678901234567890")]
        public async Task Edit(string name)
        {
            var editContract = new EditProjectContract()
            {
                Id = DefaultProjectModel.Id,
                Name = name
            };

            var projectRepositoryMock = DefaultProjectRepositoryMock;
            var projectService = new ProjectService(DefaultProjectFactoryMock.Object, projectRepositoryMock.Object);
            var result = await projectService.Edit(editContract, DefaultProjectModel.CustomerId);
            Assert.NotNull(result);
            Assert.Equal(editContract.Name, result.Name);
            projectRepositoryMock.Verify(m => m.Save(It.Is<IProjectModel>(p => p.Id == editContract.Id)), Times.Once);
        }

        [Fact]
        public async Task EditNotExisting()
        {
            var contract = new EditProjectContract()
            {
                Id = "NOT-EXISTING-PROJECT-ID",
                Name = "ThisIsNewAndValidName"
            };

            var projectService = new ProjectService(DefaultProjectFactoryMock.Object, DefaultProjectRepositoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => projectService.Edit(contract, DefaultProjectModel.CustomerId));
            Assert.Equal(ProjectValidationCode.PROJECT_DOES_NOT_EXIST.ToString(), exception.Message);
        }

        [Fact]
        public async Task EditAlreadyExistingName()
        {
            var contract = new EditProjectContract()
            {
                Id = DefaultProjectModel.Id,
                Name = DefaultProjectModel.Name
            };

            var projectService = new ProjectService(DefaultProjectFactoryMock.Object, DefaultProjectRepositoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => projectService.Edit(contract, DefaultProjectModel.CustomerId));
            Assert.Equal(ProjectValidationCode.PROJECT_ALREADY_EXISTS.ToString(), exception.Message);
        }

        [Fact]
        public async Task EditForInvalidOwner()
        {
            var contract = new EditProjectContract()
            {
                Id = DefaultProjectModel.Id,
                Name = "ThisIsNewAndValidName"
            };

            var projectService = new ProjectService(DefaultProjectFactoryMock.Object, DefaultProjectRepositoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => projectService.Edit(contract, "INVALID-CUSTOMER-NAME"));
            Assert.Equal(ProjectValidationCode.INVALID_PROJECT_OWNER.ToString(), exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ThisNameIsLongerThanIsAllowed!!12345678901234567890")]
        public async Task EditWithInvalidName(string name)
        {
            var contract = new CreateProjectContract()
            {
                Name = name
            };

            var projectService = new ProjectService(DefaultProjectFactoryMock.Object, DefaultProjectRepositoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => projectService.Create(contract, DefaultProjectModel.CustomerId));
            Assert.Equal(ProjectValidationCode.INVALID_PROJECT_NAME.ToString(), exception.Message);
        }

        [Fact]
        public async Task Delete()
        {
            var projectRepositoryMock = DefaultProjectRepositoryMock;
            var projectService = new ProjectService(DefaultProjectFactoryMock.Object, projectRepositoryMock.Object);
            await projectService.Delete(DefaultProjectModel.Id, DefaultProjectModel.CustomerId);
            projectRepositoryMock.Verify(m => m.Delete(It.Is<string>(p => p == DefaultProjectModel.Id)), Times.Once);
        }

        [Theory]
        // project id invalid
        [InlineData(null, "CUSTOMER-ID")]
        [InlineData("", "CUSTOMER-ID")]
        [InlineData(" ", "CUSTOMER-ID")]
        // customer id invalid
        [InlineData("PROJECT-ID", null)]
        [InlineData("PROJECT-ID", "")]
        [InlineData("PROJECT-ID", " ")]
        public async Task DeleteUsingEmptyIds(string projectId, string customerId)
        {
            var projectService = new ProjectService(DefaultProjectFactoryMock.Object, DefaultProjectRepositoryMock.Object);
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => projectService.Delete(projectId, customerId));
        }

        [Theory]
        [InlineData("NOT-EXISTING-PROJECT-ID", "TEST-CUSTOMER-1", ProjectValidationCode.PROJECT_DOES_NOT_EXIST)]
        [InlineData("P-TEST-PROJECT-1", "INVALID-CUSTOMER-ID", ProjectValidationCode.INVALID_PROJECT_OWNER)]
        public async Task DeleteInvalid(string projectId, string customerId, Enum exceptionMessage)
        {
            var projectService = new ProjectService(DefaultProjectFactoryMock.Object, DefaultProjectRepositoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => projectService.Delete(projectId, customerId));
            Assert.Equal(exceptionMessage.ToString(), exception.Message);
        }
    }
}

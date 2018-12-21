using System;
using System.Threading.Tasks;
using Moq;
using Sconfig.Contracts.Configuration.ConfigurationGroup.Writes;
using Sconfig.Contracts.Configuration.Enums;
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
    public class ConfigurationGroupTests
    {
        #region Privates / mocking

        private IConfigurationGroupModel DefaultProjectConfigurationGroupModel
        {
            get
            {
                return new ConfigurationGroupTestModel()
                {
                    CreatedOn = DateTime.Now,
                    Id = "G-TEST-CONFIGURATION-GROUP-2",
                    Name = "TEST CONFIGURATION GROUP",
                    ProjectId = "TEST-PROJECT-1",
                    ApplicationId = null,
                    ParentId = null,
                    SortingIndex = 0
                };
            }
        }

        private IConfigurationGroupModel DefaultApplicationConfigurationGroupModel
        {
            get
            {
                return new ConfigurationGroupTestModel()
                {
                    CreatedOn = DateTime.Now,
                    Id = "G-TEST-CONFIGURATION-GROUP-1",
                    Name = "TEST CONFIGURATION GROUP",
                    ProjectId = "TEST-PROJECT-1",
                    ApplicationId = "TEST-APPLICATION-1",
                    ParentId = null,
                    SortingIndex = 0
                };
            }
        }

        private IConfigurationGroupModel DefaultChildConfigurationGroupModel
        {
            get
            {
                return new ConfigurationGroupTestModel()
                {
                    CreatedOn = DateTime.Now,
                    Id = "G-TEST-CONFIGURATION-GROUP-3",
                    Name = "TEST CONFIGURATION GROUP",
                    ProjectId = "TEST-PROJECT-1",
                    ApplicationId = "TEST-APPLICATION-1",
                    ParentId = "TEST-CONFIGURATION-GROUP-1",
                    SortingIndex = 0
                };
            }
        }

        private Mock<IConfigurationGroupFactory> DefaultConfigurationGroupFactoryMock
        {
            get
            {
                var configurationGroupFactoryMock = new Mock<IConfigurationGroupFactory>();
                configurationGroupFactoryMock
                   .Setup(_ => _.InitConfigurationGroupModel())
                   .Returns(new ConfigurationGroupTestModel());
                return configurationGroupFactoryMock;
            }
        }

        private Mock<IConfigurationGroupRepository> DefaultConfigurationGroupRepositoryMock
        {
            get
            {
                // reads
                var configurationGroupRepository = new Mock<IConfigurationGroupRepository>();
                configurationGroupRepository
                   .Setup(_ => _.Get(It.Is<string>(s => s == DefaultProjectConfigurationGroupModel.Id)))
                   .Returns(Task.FromResult(DefaultProjectConfigurationGroupModel));

                configurationGroupRepository
                   .Setup(_ => _.Get(It.Is<string>(s => s == DefaultApplicationConfigurationGroupModel.Id)))
                   .Returns(Task.FromResult(DefaultApplicationConfigurationGroupModel));

                configurationGroupRepository
                 .Setup(_ => _.Get(It.Is<string>(s => s == DefaultChildConfigurationGroupModel.Id)))
                 .Returns(Task.FromResult(DefaultChildConfigurationGroupModel));

                // writes
                configurationGroupRepository
                   .Setup(_ => _.Insert(It.IsAny<IConfigurationGroupModel>()))
                   .Returns<IConfigurationGroupModel>(x => Task.FromResult(x));

                configurationGroupRepository
                   .Setup(_ => _.Save(It.IsAny<IConfigurationGroupModel>()))
                   .Returns<IConfigurationGroupModel>(x => x);

                return configurationGroupRepository;
            }
        }

        private IConfigurationGroupService InitConfigurationGroupService(IConfigurationGroupRepository repository,
            IConfigurationGroupFactory factory)
        {
            return new ConfigurationGroupService(repository, factory);
        }

        #endregion

        [Theory]
        [InlineData("G-TEST-CONFIGURATION-GROUP-2", "TEST-PROJECT-1", null)]
        [InlineData("G-TEST-CONFIGURATION-GROUP-1", "TEST-PROJECT-1", "TEST-APPLICATION-1")]
        public void GetExisting(string id, string projectId, string applicationId)
        {
            var configurationGroupService = InitConfigurationGroupService(DefaultConfigurationGroupRepositoryMock.Object, DefaultConfigurationGroupFactoryMock.Object);
            var result = configurationGroupService.Get(id, projectId, applicationId).Result;
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(projectId, result.ProjectId);
            Assert.Equal(applicationId, result.ApplicationId);
        }

        [Theory]
        [InlineData("G-TEST-CONFIGURATION-GROUP-1", "TEST-PROJECT-1", null)]
        [InlineData("G-TEST-CONFIGURATION-GROUP-1", "TEST-PROJECT-1", "TEST-APPLICATION-2")]
        [InlineData("G-TEST-CONFIGURATION-GROUP-1", "TEST-PROJECT-1", "")]
        [InlineData("G-TEST-CONFIGURATION-GROUP-1", "TEST-PROJECT-1", " ")]
        public void GetWithForWrongApplication(string id, string projectId, string applicationId)
        {
            var configurationGroupService = InitConfigurationGroupService(DefaultConfigurationGroupRepositoryMock.Object, DefaultConfigurationGroupFactoryMock.Object);
            var result = configurationGroupService.Get(id, projectId, applicationId).Result;
            Assert.Null(result);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(" ", null)]
        [InlineData(null, " ")]
        public void GetWithNullIdentifiers(string id, string projectId)
        {
            var configurationGroupService = InitConfigurationGroupService(DefaultConfigurationGroupRepositoryMock.Object, DefaultConfigurationGroupFactoryMock.Object);
            var result = configurationGroupService.Get(id, projectId, null).Result;
            Assert.Null(result);
        }

        [Theory]
        [InlineData("MY-TEST-CONFIGURATION-GROUP")]
        [InlineData("ThisIsEdgeValueOfAllowedLength")]
        public void Create(string name)
        {
            var contract = new CreateConfigurationGroupContract()
            {
                Name = name,
                ProjectId = DefaultApplicationConfigurationGroupModel.ProjectId,
                ApplicationId = DefaultApplicationConfigurationGroupModel.ApplicationId
            };

            var configurationGroupRepositoryMock = DefaultConfigurationGroupRepositoryMock;
            var configurationGroupService = InitConfigurationGroupService(configurationGroupRepositoryMock.Object, DefaultConfigurationGroupFactoryMock.Object);

            var result = configurationGroupService.Create(contract).Result;
            Assert.NotNull(result);
            Assert.NotNull(result.Id);
            Assert.Equal(contract.Name, result.Name);
            Assert.Equal(contract.ApplicationId, result.ApplicationId);
            Assert.Equal(contract.ProjectId, result.ProjectId);
            Assert.NotEqual(DateTime.MinValue, result.CreatedOn);
            configurationGroupRepositoryMock.Verify(m => m.Insert(It.IsAny<IConfigurationGroupModel>()), Times.Once);
        }

        [Fact]
        public async Task CreateAlreadyExisting()
        {
            var contract = new CreateConfigurationGroupContract()
            {
                Name = DefaultApplicationConfigurationGroupModel.Name,
                ProjectId = DefaultApplicationConfigurationGroupModel.ProjectId,
                ApplicationId = DefaultApplicationConfigurationGroupModel.ApplicationId
            };

            var configurationGroupRepositoryMock = DefaultConfigurationGroupRepositoryMock;
            configurationGroupRepositoryMock
                  .Setup(_ => _.GetByNameAndByProject(It.IsAny<string>(), It.IsAny<string>()))
                  .Returns(Task.FromResult(DefaultApplicationConfigurationGroupModel));

            var configurationGroupService = InitConfigurationGroupService(configurationGroupRepositoryMock.Object, DefaultConfigurationGroupFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => configurationGroupService.Create(contract));
            Assert.Equal(ConfigurationGroupValidationCodes.CONFIGURATION_GROUP_ALREADY_EXISTS.ToString(), exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ThisNameIsLongerThanIsAllowed!!")]
        public async Task CreateWithInvalidName(string name)
        {
            var contract = new CreateConfigurationGroupContract()
            {
                Name = name,
                ProjectId = DefaultApplicationConfigurationGroupModel.ProjectId,
                ApplicationId = DefaultApplicationConfigurationGroupModel.ApplicationId
            };

            var configurationGroupService = InitConfigurationGroupService(DefaultConfigurationGroupRepositoryMock.Object, DefaultConfigurationGroupFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => configurationGroupService.Create(contract));
            Assert.Equal(ConfigurationGroupValidationCodes.INVALID_CONFIGURATION_GROUP_NAME.ToString(), exception.Message);
        }

        [Theory]
        [InlineData("MY-CONFIGURATION-GROUP-NAME")]
        [InlineData("ThisIsEdgeValueOfAllowedLength")]
        public async Task Edit(string name)
        {
            var editContract = new EditConfigurationGroupContract()
            {
                Id = DefaultApplicationConfigurationGroupModel.Id,
                ApplicationId = DefaultApplicationConfigurationGroupModel.ApplicationId,
                ParentId = DefaultApplicationConfigurationGroupModel.ParentId,
                ProjectId = DefaultApplicationConfigurationGroupModel.ProjectId,
                SortingIndex = DefaultApplicationConfigurationGroupModel.SortingIndex,
                Name = name
            };

            var configurationGroupRepositoryMock = DefaultConfigurationGroupRepositoryMock;
            var configurationGroupService = InitConfigurationGroupService(configurationGroupRepositoryMock.Object, DefaultConfigurationGroupFactoryMock.Object);

            var result = await configurationGroupService.Edit(editContract);
            Assert.NotNull(result);
            Assert.Equal(editContract.Name, result.Name);
            configurationGroupRepositoryMock.Verify(m => m.Save(It.Is<IConfigurationGroupModel>(p => p.Id == editContract.Id)), Times.Once);
        }

        [Fact]
        public async Task EditNotExisting()
        {
            var contract = new EditConfigurationGroupContract()
            {
                Id = "NOT-EXISTING-CONFIGURATION-GROUP-ID",
                Name = "ThisIsNewAndValidName",
                ProjectId = DefaultChildConfigurationGroupModel.ProjectId
            };

            var configurationGroupService = InitConfigurationGroupService(DefaultConfigurationGroupRepositoryMock.Object, DefaultConfigurationGroupFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => configurationGroupService.Edit(contract));
            Assert.Equal(ConfigurationGroupValidationCodes.CONFIGURATION_GROUP_DOES_NOT_EXIST.ToString(), exception.Message);
        }

        [Fact]
        public async Task EditForInvalidProject()
        {
            var contract = new EditConfigurationGroupContract()
            {
                Id = DefaultChildConfigurationGroupModel.Id,
                Name = "ThisIsNewAndValidName",
                ProjectId = "INVALID-PROJECT-ID"
            };

            var configurationGroupService = InitConfigurationGroupService(DefaultConfigurationGroupRepositoryMock.Object, DefaultConfigurationGroupFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => configurationGroupService.Edit(contract));
            Assert.Equal(ConfigurationGroupValidationCodes.INVALID_CONFIGURATION_GROUP_PROJECT.ToString(), exception.Message);
        }


        [Fact]
        public async Task EditForInvalidApplication()
        {
            var contract = new EditConfigurationGroupContract()
            {
                Id = DefaultChildConfigurationGroupModel.Id,
                Name = "ThisIsNewAndValidName",
                ProjectId = DefaultChildConfigurationGroupModel.ProjectId,
                ApplicationId = "INVALID-APPLICATION-ID"
            };

            var configurationGroupService = InitConfigurationGroupService(DefaultConfigurationGroupRepositoryMock.Object, DefaultConfigurationGroupFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => configurationGroupService.Edit(contract));
            Assert.Equal(ConfigurationGroupValidationCodes.INVALID_CONFIGURATION_GROUP_APPLICATION.ToString(), exception.Message);
        }

        [Fact]
        public async Task EditAlreadyExistingName()
        {
            var contract = new EditConfigurationGroupContract()
            {
                Id = DefaultChildConfigurationGroupModel.Id,
                Name = DefaultChildConfigurationGroupModel.Name,
                ProjectId = DefaultChildConfigurationGroupModel.ProjectId
            };

            var configurationGroupRepositoryMock = DefaultConfigurationGroupRepositoryMock;
            configurationGroupRepositoryMock
                  .Setup(_ => _.GetByNameAndByProject(It.IsAny<string>(), It.IsAny<string>()))
                  .Returns(Task.FromResult(DefaultApplicationConfigurationGroupModel));

            var configurationGroupService = InitConfigurationGroupService(configurationGroupRepositoryMock.Object, DefaultConfigurationGroupFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => configurationGroupService.Edit(contract));
            Assert.Equal(ConfigurationGroupValidationCodes.CONFIGURATION_GROUP_ALREADY_EXISTS.ToString(), exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ThisNameIsLongerThanIsAllowed!!")]
        public async Task EditWithInvalidName(string name)
        {
            var contract = new EditConfigurationGroupContract()
            {
                Id = DefaultChildConfigurationGroupModel.Id,
                Name = name,
                ProjectId = DefaultChildConfigurationGroupModel.ProjectId
            };

            var configurationGroupService = InitConfigurationGroupService(DefaultConfigurationGroupRepositoryMock.Object, DefaultConfigurationGroupFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => configurationGroupService.Edit(contract));
            Assert.Equal(ConfigurationGroupValidationCodes.INVALID_CONFIGURATION_GROUP_NAME.ToString(), exception.Message);
        }

        [Fact]
        public async Task Delete()
        {
            var configurationGroupRepositoryMock = DefaultConfigurationGroupRepositoryMock;
            var configurationGroupService = InitConfigurationGroupService(configurationGroupRepositoryMock.Object, DefaultConfigurationGroupFactoryMock.Object);
            await configurationGroupService.Delete(DefaultApplicationConfigurationGroupModel.Id, DefaultApplicationConfigurationGroupModel.ProjectId,
                DefaultApplicationConfigurationGroupModel.ApplicationId);
            configurationGroupRepositoryMock.Verify(m => m.Delete(It.Is<string>(p => p == DefaultApplicationConfigurationGroupModel.Id)), Times.Once);
        }

        [Theory]
        // id invalid
        [InlineData("PROJECT-ID", null)]
        [InlineData("PROJECT-ID", "")]
        [InlineData("PROJECT-ID", " ")]
        // project id invalid
        [InlineData(null, "CONFIGURATION-GROUP-ID")]
        [InlineData("", "CONFIGURATION-GROUP-ID")]
        [InlineData(" ", "CONFIGURATION-GROUP-ID")]
        public async Task DeleteUsingEmptyIds(string projectId, string id)
        {
            var configurationGroupService = InitConfigurationGroupService(DefaultConfigurationGroupRepositoryMock.Object, DefaultConfigurationGroupFactoryMock.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => configurationGroupService.Delete(id, projectId, null));
        }

        [Theory]
        [InlineData("NOT-EXISTING-CONFIGURATION-GROUP-ID", "TEST-PROJECT-1", null, ConfigurationGroupValidationCodes.CONFIGURATION_GROUP_DOES_NOT_EXIST)]
        [InlineData("G-TEST-CONFIGURATION-GROUP-1", "INVALID-PROJECT-ID", null, ConfigurationGroupValidationCodes.INVALID_CONFIGURATION_GROUP_PROJECT)]
        [InlineData("G-TEST-CONFIGURATION-GROUP-1", "TEST-PROJECT-1", "INVALID-APPLICATION-ID", ConfigurationGroupValidationCodes.INVALID_CONFIGURATION_GROUP_APPLICATION)]
        public async Task DeleteInvalid(string id, string projectId, string applicationId, Enum exceptionMessage)
        {
            var configurationGroupService = InitConfigurationGroupService(DefaultConfigurationGroupRepositoryMock.Object, DefaultConfigurationGroupFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => configurationGroupService.Delete(id, projectId, applicationId));
            Assert.Equal(exceptionMessage.ToString(), exception.Message);
        }
    }
}

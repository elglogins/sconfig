using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Sconfig.Contracts.Configuration.ConfigurationItem.Enums;
using Sconfig.Contracts.Configuration.ConfigurationItem.Writes;
using Sconfig.Exceptions;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;
using Sconfig.Interfaces.Services;
using Sconfig.Mapping;
using Sconfig.Services;
using Sconfig.Tests.Models;
using Xunit;

namespace Sconfig.Tests
{
    public class ConfigurationItemServiceTests
    {
        #region Privates / mocking

        private IConfigurationItemModel DefaultConfigurationItemModel
        {
            get
            {
                return new ConfigurationItemTestModel()
                {
                    CreatedOn = DateTime.Now,
                    Id = "I-TEST-CONFIGURATION-ITEM-1",
                    Name = "TEST CONFIGURATION ITEM",
                    ProjectId = "TEST-PROJECT-1",
                    ApplicationId = null,
                    ParentId = null,
                    EnvironmentId = null,
                    Value = "TEST-VALUE",
                    SortingIndex = 0
                };
            }
        }

        private Mock<IConfigurationItemFactory> DefaultConfigurationItemFactoryMock
        {
            get
            {
                var configurationItemFactoryMock = new Mock<IConfigurationItemFactory>();
                configurationItemFactoryMock
                   .Setup(_ => _.InitConfigurationItemModel())
                   .Returns(new ConfigurationItemTestModel());
                return configurationItemFactoryMock;
            }
        }

        private Mock<IConfigurationItemRepository> DefaultConfigurationItemRepositoryMock
        {
            get
            {
                // reads
                var configurationItemRepository = new Mock<IConfigurationItemRepository>();
                configurationItemRepository
                   .Setup(_ => _.Get(It.Is<string>(s => s == DefaultConfigurationItemModel.Id)))
                   .Returns(Task.FromResult(DefaultConfigurationItemModel));

                // writes
                configurationItemRepository
                   .Setup(_ => _.Insert(It.IsAny<IConfigurationItemModel>()))
                   .Returns<IConfigurationItemModel>(x => Task.FromResult(x));

                configurationItemRepository
                   .Setup(_ => _.Save(It.IsAny<IConfigurationItemModel>()))
                   .Returns<IConfigurationItemModel>(x => x);

                return configurationItemRepository;
            }
        }

        private IConfigurationItemService InitConfigurationItemService(IConfigurationItemRepository repository,
           IConfigurationItemFactory factory)
        {
            return new ConfigurationItemService(factory, repository, new ConfigurationItemMapper());
        }

        #endregion

        [Fact]
        public void GetForProject()
        {
            var configurationItemService = InitConfigurationItemService(DefaultConfigurationItemRepositoryMock.Object, DefaultConfigurationItemFactoryMock.Object);
            var result = configurationItemService.Get(DefaultConfigurationItemModel.Id, DefaultConfigurationItemModel.ProjectId, null, null).Result;
            Assert.NotNull(result);
            Assert.Equal(DefaultConfigurationItemModel.Id, result.Id);
        }

        [Fact]
        public void GetForProjectAndApplication()
        {
            var model = DefaultConfigurationItemModel;
            model.ApplicationId = "APPLICATION-ID";

            var configurationItemRepository = DefaultConfigurationItemRepositoryMock;
            configurationItemRepository
                 .Setup(_ => _.Get(It.Is<string>(s => s == model.Id)))
                 .Returns(Task.FromResult(model));

            var configurationItemService = InitConfigurationItemService(configurationItemRepository.Object, DefaultConfigurationItemFactoryMock.Object);
            var result = configurationItemService.Get(model.Id, model.ProjectId, model.ApplicationId, null).Result;
            Assert.NotNull(result);
            Assert.Equal(DefaultConfigurationItemModel.Id, result.Id);
        }

        [Fact]
        public void GetForProjectAndEnvironment()
        {
            var model = DefaultConfigurationItemModel;
            model.EnvironmentId = "ENVIRONMENT-ID";

            var configurationItemRepository = DefaultConfigurationItemRepositoryMock;
            configurationItemRepository
                 .Setup(_ => _.Get(It.Is<string>(s => s == model.Id)))
                 .Returns(Task.FromResult(model));

            var configurationItemService = InitConfigurationItemService(configurationItemRepository.Object, DefaultConfigurationItemFactoryMock.Object);
            var result = configurationItemService.Get(model.Id, model.ProjectId, null, model.EnvironmentId).Result;
            Assert.NotNull(result);
            Assert.Equal(DefaultConfigurationItemModel.Id, result.Id);
        }

        [Fact]
        public void Get()
        {
            var model = DefaultConfigurationItemModel;
            model.EnvironmentId = "ENVIRONMENT-ID";
            model.ApplicationId = "APPLICATION-ID";

            var configurationItemRepository = DefaultConfigurationItemRepositoryMock;
            configurationItemRepository
                 .Setup(_ => _.Get(It.Is<string>(s => s == model.Id)))
                 .Returns(Task.FromResult(model));

            var configurationItemService = InitConfigurationItemService(configurationItemRepository.Object, DefaultConfigurationItemFactoryMock.Object);
            var result = configurationItemService.Get(model.Id, model.ProjectId, model.ApplicationId, model.EnvironmentId).Result;
            Assert.NotNull(result);
            Assert.Equal(DefaultConfigurationItemModel.Id, result.Id);
        }

        [Fact]
        public void GetWithInvalidEnvironment()
        {
            var configurationItemService = InitConfigurationItemService(DefaultConfigurationItemRepositoryMock.Object, DefaultConfigurationItemFactoryMock.Object);
            var result = configurationItemService.Get(DefaultConfigurationItemModel.Id, DefaultConfigurationItemModel.ProjectId, null, "INVALID-ENVIRONMENT-ID").Result;
            Assert.Null(result);
        }

        [Fact]
        public void GetWithInvalidProject()
        {
            var configurationItemService = InitConfigurationItemService(DefaultConfigurationItemRepositoryMock.Object, DefaultConfigurationItemFactoryMock.Object);
            var result = configurationItemService.Get(DefaultConfigurationItemModel.Id, "INVALID-PROJECT-ID", null, null).Result;
            Assert.Null(result);
        }

        [Fact]
        public void GetWithInvalidApplication()
        {
            var configurationItemService = InitConfigurationItemService(DefaultConfigurationItemRepositoryMock.Object, DefaultConfigurationItemFactoryMock.Object);
            var result = configurationItemService.Get(DefaultConfigurationItemModel.Id, DefaultConfigurationItemModel.ProjectId, "INVALID-APPLICATION-ID", null).Result;
            Assert.Null(result);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(" ", null)]
        [InlineData(null, " ")]
        public void GetForProjectWithNullIdentifiers(string id, string projectId)
        {
            var configurationItemService = InitConfigurationItemService(DefaultConfigurationItemRepositoryMock.Object, DefaultConfigurationItemFactoryMock.Object);
            var result = configurationItemService.Get(id, projectId, null, null).Result;
            Assert.Null(result);
        }

        [Fact]
        public async Task Delete()
        {
            var configurationItemRepositoryMock = DefaultConfigurationItemRepositoryMock;
            var configurationItemService = InitConfigurationItemService(configurationItemRepositoryMock.Object, DefaultConfigurationItemFactoryMock.Object);

            await configurationItemService.Delete(DefaultConfigurationItemModel.Id, DefaultConfigurationItemModel.ProjectId,
                DefaultConfigurationItemModel.ApplicationId, DefaultConfigurationItemModel.EnvironmentId);
            configurationItemRepositoryMock.Verify(m => m.Delete(It.Is<string>(p => p == DefaultConfigurationItemModel.Id)), Times.Once);
        }

        [Theory]
        // project invalid
        [InlineData("CONFIGURATION-ITEM-ID", null)]
        [InlineData("CONFIGURATION-ITEM-ID", "")]
        [InlineData("CONFIGURATION-ITEM-ID", " ")]
        // id invalid
        [InlineData(null, "PROJECT-ID")]
        [InlineData("", "PROJECT-ID")]
        [InlineData(" ", "PROJECT-ID")]
        public async Task DeleteUsingEmptyIds(string id, string projectId)
        {
            var configurationItemService = InitConfigurationItemService(DefaultConfigurationItemRepositoryMock.Object, DefaultConfigurationItemFactoryMock.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => configurationItemService.Delete(id, projectId, null, null));
        }

        [Theory]
        [InlineData("INVALID-CONFIGURATION-ITEM-ID", "TEST-PROJECT-1", "TEST-APPLICATION-1", "TEST-ENVIRONMENT-1", ConfigurationItemValidationCodes.CONFIGURATION_ITEM_DOES_NOT_EXIST)]
        [InlineData("I-TEST-CONFIGURATION-ITEM-1", "INVALID-PROJECT", "TEST-APPLICATION-1", "TEST-ENVIRONMENT-1", ConfigurationItemValidationCodes.INVALID_CONFIGURATION_ITEM_PROJECT)]
        [InlineData("I-TEST-CONFIGURATION-ITEM-1", "TEST-PROJECT-1", "INVALID-APPLICATION", "TEST-ENVIRONMENT-1", ConfigurationItemValidationCodes.INVALID_CONFIGURATION_ITEM_APPLICATION)]
        [InlineData("I-TEST-CONFIGURATION-ITEM-1", "TEST-PROJECT-1", "TEST-APPLICATION-1", "INVALID-ENVIRONMENT", ConfigurationItemValidationCodes.INVALID_CONFIGURATION_ITEM_ENVIRONMENT)]
        public async Task DeleteInvalid(string id, string projectId, string applicationId, string environmentId, Enum exceptionMessage)
        {
            var model = DefaultConfigurationItemModel;
            model.ApplicationId = "TEST-APPLICATION-1";
            model.EnvironmentId = "TEST-ENVIRONMENT-1";

            var configurationItemRepository = DefaultConfigurationItemRepositoryMock;
            configurationItemRepository
                 .Setup(_ => _.Get(It.Is<string>(s => s == model.Id)))
                 .Returns(Task.FromResult(model));

            var configurationItemService = InitConfigurationItemService(configurationItemRepository.Object, DefaultConfigurationItemFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => configurationItemService.Delete(id, projectId, applicationId, environmentId));
            Assert.Equal(exceptionMessage.ToString(), exception.Message);
        }

        [Theory]
        [InlineData("MY-TEST-CONFIGURATION-ITEM")]
        [InlineData("ThisIsEdgeValueOfAllowedLength")]
        public void Create(string name)
        {
            var contract = new CreateConfigurationItemContract()
            {
                Name = name,
                EnvironmentId = DefaultConfigurationItemModel.EnvironmentId,
                ProjectId = DefaultConfigurationItemModel.ProjectId,
                ApplicationId = DefaultConfigurationItemModel.ApplicationId
            };

            var configurationItemRepositoryMock = DefaultConfigurationItemRepositoryMock;
            var configurationItemService = InitConfigurationItemService(configurationItemRepositoryMock.Object, DefaultConfigurationItemFactoryMock.Object);

            var result = configurationItemService.Create(contract).Result;
            Assert.NotNull(result);
            Assert.NotNull(result.Id);
            Assert.Equal(contract.Name, result.Name);
            Assert.Equal(contract.ApplicationId, result.ApplicationId);
            Assert.Equal(contract.ProjectId, result.ProjectId);
            Assert.Equal(contract.EnvironmentId, result.EnvironmentId);
            Assert.NotEqual(DateTime.MinValue, result.CreatedOn);
            configurationItemRepositoryMock.Verify(m => m.Insert(It.IsAny<IConfigurationItemModel>()), Times.Once);
        }

        [Fact]
        public async Task CreateAlreadyExisting()
        {
            var contract = new CreateConfigurationItemContract()
            {
                Name = DefaultConfigurationItemModel.Name,
                EnvironmentId = DefaultConfigurationItemModel.EnvironmentId,
                ProjectId = DefaultConfigurationItemModel.ProjectId,
                ApplicationId = DefaultConfigurationItemModel.ApplicationId
            };

            var configurationGroupRepositoryMock = DefaultConfigurationItemRepositoryMock;
            configurationGroupRepositoryMock
                  .Setup(_ => _.GetByName(It.IsAny<string>(), It.IsAny<string>(),
                  It.IsAny<string>(),
                  It.IsAny<string>()))
                  .Returns(Task.FromResult((IEnumerable<IConfigurationItemModel>) new List<IConfigurationItemModel>() { DefaultConfigurationItemModel }));

            var configurationItemService = InitConfigurationItemService(configurationGroupRepositoryMock.Object, DefaultConfigurationItemFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => configurationItemService.Create(contract));
            Assert.Equal(ConfigurationItemValidationCodes.CONFIGURATION_ITEM_ALREADY_EXISTS.ToString(), exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ThisNameIsLongerThanIsAllowed!!")]
        public async Task CreateWithInvalidName(string name)
        {
            var contract = new CreateConfigurationItemContract()
            {
                Name = name,
                EnvironmentId = DefaultConfigurationItemModel.EnvironmentId,
                ProjectId = DefaultConfigurationItemModel.ProjectId,
                ApplicationId = DefaultConfigurationItemModel.ApplicationId
            };

            var configurationItemService = InitConfigurationItemService(DefaultConfigurationItemRepositoryMock.Object, DefaultConfigurationItemFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => configurationItemService.Create(contract));
            Assert.Equal(ConfigurationItemValidationCodes.INVALID_CONFIGURATION_ITEM_NAME.ToString(), exception.Message);
        }

        [Theory]
        [InlineData("MY-CONFIGURATION-GROUP-NAME")]
        [InlineData("ThisIsEdgeValueOfAllowedLength")]
        public void Edit(string name)
        {
            var editContract = new EditConfigurationItemContract()
            {
                Id = DefaultConfigurationItemModel.Id,
                ApplicationId = DefaultConfigurationItemModel.ApplicationId,
                ParentId = DefaultConfigurationItemModel.ParentId,
                ProjectId = DefaultConfigurationItemModel.ProjectId,
                SortingIndex = DefaultConfigurationItemModel.SortingIndex,
                EnvironmentId = DefaultConfigurationItemModel.EnvironmentId,
                Value = DefaultConfigurationItemModel.Value,
                Name = name
            };

            var configurationItemRepositoryMock = DefaultConfigurationItemRepositoryMock;
            var configurationItemService = InitConfigurationItemService(configurationItemRepositoryMock.Object, DefaultConfigurationItemFactoryMock.Object);

            var result = configurationItemService.Edit(editContract).Result;
            Assert.NotNull(result);
            Assert.Equal(editContract.Id, result.Id);
            Assert.Equal(editContract.Name, result.Name);
            configurationItemRepositoryMock.Verify(m => m.Save(It.Is<IConfigurationItemModel>(p => p.Id == editContract.Id)), Times.Once);
        }

        [Fact]
        public async Task EditNotExisting()
        {
            var contract = new EditConfigurationItemContract()
            {
                Id = "NOT-EXISTING-CONFIGURATION-ITEM-ID",
                Name = "ThisIsNewAndValidName",
                ProjectId = DefaultConfigurationItemModel.ProjectId
            };

            var configurationItemService = InitConfigurationItemService(DefaultConfigurationItemRepositoryMock.Object, DefaultConfigurationItemFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => configurationItemService.Edit(contract));
            Assert.Equal(ConfigurationItemValidationCodes.CONFIGURATION_ITEM_DOES_NOT_EXIST.ToString(), exception.Message);
        }

        [Fact]
        public async Task EditAlreadyExistingName()
        {
            var contract = new EditConfigurationItemContract()
            {
                Id = DefaultConfigurationItemModel.Id,
                Name = DefaultConfigurationItemModel.Name,
                ProjectId = DefaultConfigurationItemModel.ProjectId
            };

            var configurationGroupRepositoryMock = DefaultConfigurationItemRepositoryMock;
            var reservedNameModel = new ConfigurationItemTestModel()
            {
                Id = "TEST-CONFIGURATION-ITEM-2",
                Name = DefaultConfigurationItemModel.Name
            };

            configurationGroupRepositoryMock
                  .Setup(_ => _.GetByName(It.IsAny<string>(), It.IsAny<string>(),
                  It.IsAny<string>(),
                  It.IsAny<string>()))
                  .Returns(Task.FromResult((IEnumerable<IConfigurationItemModel>)new List<IConfigurationItemModel>() { DefaultConfigurationItemModel, reservedNameModel }));

            var configurationItemService = InitConfigurationItemService(configurationGroupRepositoryMock.Object, DefaultConfigurationItemFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => configurationItemService.Edit(contract));
            Assert.Equal(ConfigurationItemValidationCodes.CONFIGURATION_ITEM_ALREADY_EXISTS.ToString(), exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ThisNameIsLongerThanIsAllowed!!")]
        public async Task EditWithInvalidName(string name)
        {
            var contract = new EditConfigurationItemContract()
            {
                Id = DefaultConfigurationItemModel.Id,
                ApplicationId = DefaultConfigurationItemModel.ApplicationId,
                ParentId = DefaultConfigurationItemModel.ParentId,
                ProjectId = DefaultConfigurationItemModel.ProjectId,
                SortingIndex = DefaultConfigurationItemModel.SortingIndex,
                EnvironmentId = DefaultConfigurationItemModel.EnvironmentId,
                Value = DefaultConfigurationItemModel.Value,
                Name = name
            };

            var configurationItemService = InitConfigurationItemService(DefaultConfigurationItemRepositoryMock.Object, DefaultConfigurationItemFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => configurationItemService.Edit(contract));
            Assert.Equal(ConfigurationItemValidationCodes.INVALID_CONFIGURATION_ITEM_NAME.ToString(), exception.Message);
        }
    }
}

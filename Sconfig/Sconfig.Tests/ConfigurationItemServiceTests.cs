using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Sconfig.Contracts.Configuration.ConfigurationItem.Enums;
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
                    Id = "TEST-CONFIGURATION-ITEM-1",
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
            return new ConfigurationItemService(factory, repository);
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
        [InlineData("TEST-CONFIGURATION-ITEM-1", "INVALID-PROJECT", "TEST-APPLICATION-1", "TEST-ENVIRONMENT-1", ConfigurationItemValidationCodes.INVALID_CONFIGURATION_ITEM_PROJECT)]
        [InlineData("TEST-CONFIGURATION-ITEM-1", "TEST-PROJECT-1", "INVALID-APPLICATION", "TEST-ENVIRONMENT-1", ConfigurationItemValidationCodes.INVALID_CONFIGURATION_ITEM_APPLICATION)]
        [InlineData("TEST-CONFIGURATION-ITEM-1", "TEST-PROJECT-1", "TEST-APPLICATION-1", "INVALID-ENVIRONMENT", ConfigurationItemValidationCodes.INVALID_CONFIGURATION_ITEM_ENVIRONMENT)]
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
    }
}

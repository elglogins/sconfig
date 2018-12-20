using System;
using System.Threading.Tasks;
using Sconfig.Contracts.Configuration.ConfigurationItem.Enums;
using Sconfig.Contracts.Configuration.ConfigurationItem.Reads;
using Sconfig.Contracts.Configuration.ConfigurationItem.Writes;
using Sconfig.Exceptions;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;
using Sconfig.Interfaces.Services;

namespace Sconfig.Services
{
    class ConfigurationItemService : IConfigurationItemService
    {
        private readonly IConfigurationItemFactory _configurationItemFactory;
        private readonly IConfigurationItemRepository _configurationItemRepository;

        public ConfigurationItemService(IConfigurationItemFactory configurationItemFactory, IConfigurationItemRepository configurationItemRepository)
        {
            _configurationItemFactory = configurationItemFactory;
            _configurationItemRepository = configurationItemRepository;
        }

        public Task<ConfigurationItemContract> Create(CreateConfigurationItemContract contract)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(string id, string projectId, string applicationId, string environmentId)
        {
            if (String.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            if (String.IsNullOrWhiteSpace(projectId))
                throw new ArgumentNullException(nameof(projectId));

            var item = await _configurationItemRepository.Get(id);
            if (item == null)
                throw new ValidationCodeException(ConfigurationItemValidationCodes.CONFIGURATION_ITEM_DOES_NOT_EXIST);

            if (item.ProjectId != projectId)
                throw new ValidationCodeException(ConfigurationItemValidationCodes.INVALID_CONFIGURATION_ITEM_PROJECT);

            if (item.ApplicationId != applicationId)
                throw new ValidationCodeException(ConfigurationItemValidationCodes.INVALID_CONFIGURATION_ITEM_APPLICATION);

            if (item.EnvironmentId != environmentId)
                throw new ValidationCodeException(ConfigurationItemValidationCodes.INVALID_CONFIGURATION_ITEM_ENVIRONMENT);

            await _configurationItemRepository.Delete(item.Id);
        }

        public Task<ConfigurationItemContract> Edit(EditConfigurationItemContract contract)
        {
            throw new NotImplementedException();
        }

        public async Task<ConfigurationItemContract> Get(string id, string projectId, string applicationId, string environmentId)
        {
            if (String.IsNullOrWhiteSpace(id))
                return null;

            if (String.IsNullOrWhiteSpace(projectId))
                return null;

            var group = await _configurationItemRepository.Get(id);
            if (group == null)
                return null;

            if (group.ProjectId != projectId)
                return null;

            if (group.ApplicationId != applicationId)
                return null;

            if (group.EnvironmentId != environmentId)
                return null;

            return Map(group);
        }

        private ConfigurationItemContract Map(IConfigurationItemModel model)
        {
            if (model == null)
                return null;

            return new ConfigurationItemContract()
            {
                Id = model.Id,
                Name = model.Name,
                ApplicationId = model.ApplicationId,
                CreatedOn = model.CreatedOn,
                EnvironmentId = model.EnvironmentId,
                ParentId = model.ParentId,
                ProjectId = model.ProjectId,
                SortingIndex = model.SortingIndex,
                Value = model.Value
            };
        }
    }
}

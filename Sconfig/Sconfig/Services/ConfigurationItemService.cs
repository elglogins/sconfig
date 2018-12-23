using System;
using System.Linq;
using System.Threading.Tasks;
using Sconfig.Contracts.Configuration.ConfigurationItem.Enums;
using Sconfig.Contracts.Configuration.ConfigurationItem.Reads;
using Sconfig.Contracts.Configuration.ConfigurationItem.Writes;
using Sconfig.Exceptions;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Mapping;
using Sconfig.Interfaces.Repositories;
using Sconfig.Interfaces.Services;

namespace Sconfig.Services
{
    class ConfigurationItemService : IConfigurationItemService
    {
        private readonly IConfigurationItemFactory _configurationItemFactory;
        private readonly IConfigurationItemRepository _configurationItemRepository;
        private readonly IConfigurationItemMapper _configurationItemMapper;

        private const int MaxConfigurationItemNameLength = 30;
        private const string IdentifierPrefix = "I-";

        public ConfigurationItemService(IConfigurationItemFactory configurationItemFactory, IConfigurationItemRepository configurationItemRepository, IConfigurationItemMapper configurationItemMapper)
        {
            _configurationItemFactory = configurationItemFactory;
            _configurationItemRepository = configurationItemRepository;
            _configurationItemMapper = configurationItemMapper;
        }

        public async Task<ConfigurationItemContract> Create(CreateConfigurationItemContract contract)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            if (contract.ProjectId == null)
                throw new ArgumentNullException(nameof(contract.ProjectId));

            // validate name
            if (String.IsNullOrWhiteSpace(contract.Name)
                || contract.Name.Length > MaxConfigurationItemNameLength)
                throw new ValidationCodeException(ConfigurationItemValidationCodes.INVALID_CONFIGURATION_ITEM_NAME);

            // ensure that name is unique along same subset of configurations
            var existingByName = await _configurationItemRepository.GetByName(contract.Name, contract.ProjectId, contract.ApplicationId, contract.ParentId);

            // check if exists for the same environment
            if (existingByName.Any(a => a.EnvironmentId == contract.EnvironmentId))
                throw new ValidationCodeException(ConfigurationItemValidationCodes.CONFIGURATION_ITEM_ALREADY_EXISTS);

            int? sortingIndex = null;
            if (!String.IsNullOrWhiteSpace(contract.ParentId))
            {
                var parentChildren = await _configurationItemRepository.GetByParent(contract.ParentId, contract.ProjectId, contract.ApplicationId);
                sortingIndex = parentChildren.Count();
            }
            else
            {
                // root level configuration item, get already existing items
                sortingIndex = (String.IsNullOrWhiteSpace(contract.ApplicationId) ? await _configurationItemRepository.GetRootLevelByProject(contract.ProjectId)
                    : await _configurationItemRepository.GetRootLevelByApplication(contract.ApplicationId)).Count();
            }

            var model = _configurationItemFactory.InitConfigurationItemModel();
            model.Id = IdentifierPrefix + Guid.NewGuid().ToString();
            model.Name = contract.Name;
            model.ParentId = contract.ParentId;
            model.ApplicationId = contract.ApplicationId;
            model.CreatedOn = DateTime.Now;
            model.ProjectId = contract.ProjectId;
            model.Value = contract.Value;
            model.EnvironmentId = contract.EnvironmentId;
            model.SortingIndex = sortingIndex ?? 0;

            await _configurationItemRepository.Insert(model);
            return _configurationItemMapper.Map(model);
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

        public async Task<ConfigurationItemContract> Edit(EditConfigurationItemContract contract)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            if (String.IsNullOrWhiteSpace(contract.Id))
                throw new ArgumentNullException(nameof(contract.Id));

            if (String.IsNullOrWhiteSpace(contract.ProjectId))
                throw new ArgumentNullException(nameof(contract.ProjectId));

            // validate name
            if (String.IsNullOrWhiteSpace(contract.Name)
                || contract.Name.Length > MaxConfigurationItemNameLength)
                throw new ValidationCodeException(ConfigurationItemValidationCodes.INVALID_CONFIGURATION_ITEM_NAME);

            var item = await _configurationItemRepository.Get(contract.Id);
            if (item == null)
                throw new ValidationCodeException(ConfigurationItemValidationCodes.CONFIGURATION_ITEM_DOES_NOT_EXIST);

            if (item.ProjectId != contract.ProjectId)
                throw new ValidationCodeException(ConfigurationItemValidationCodes.INVALID_CONFIGURATION_ITEM_PROJECT);

            var existingItems = await _configurationItemRepository.GetByName(contract.Name, contract.ProjectId, contract.ApplicationId, contract.ParentId);

            if (existingItems.Where(c => c.Id != contract.Id).Any())
                throw new ValidationCodeException(ConfigurationItemValidationCodes.CONFIGURATION_ITEM_ALREADY_EXISTS);

            item.Name = contract.Name;
            item.SortingIndex = contract.SortingIndex;
            item.EnvironmentId = contract.EnvironmentId;
            item.ApplicationId = contract.ApplicationId;
            item.ParentId = contract.ParentId;
            item.Value = contract.Value;

            return _configurationItemMapper.Map(_configurationItemRepository.Save(item));
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

            return _configurationItemMapper.Map(group);
        }
    }
}

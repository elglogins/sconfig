using System;
using System.Linq;
using System.Threading.Tasks;
using Sconfig.Contracts.Configuration.ConfigurationGroup.Reads;
using Sconfig.Contracts.Configuration.ConfigurationGroup.Writes;
using Sconfig.Contracts.Configuration.Enums;
using Sconfig.Exceptions;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;
using Sconfig.Interfaces.Services;
using Sconfig.Mapping;

namespace Sconfig.Services
{
    public class ConfigurationGroupService : IConfigurationGroupService
    {
        private readonly IConfigurationGroupRepository _configurationGroupRepository;
        private readonly IConfigurationGroupFactory _configurationGroupFactory;
        private readonly IConfigurationGroupMapper _configurationGroupMapper;

        private const int MaxConfigurationGroupNameLength = 30;
        private const string IdentifierPrefix = "G-";

        public ConfigurationGroupService(IConfigurationGroupRepository configurationGroupRepository, IConfigurationGroupFactory configurationGroupFactory, IConfigurationGroupMapper configurationGroupMapper)
        {
            _configurationGroupRepository = configurationGroupRepository;
            _configurationGroupFactory = configurationGroupFactory;
            _configurationGroupMapper = configurationGroupMapper;
        }

        private async Task<int> GetSortingIndex(IConfigurationGroupModel parentGroup, string projectId)
        {
            if (parentGroup != null)
            {
                var parentsChildrenGroups = await _configurationGroupRepository.GetByParentGroup(parentGroup.Id);
                return parentsChildrenGroups.Count();
            }

            // sort index based on count of groups that project has without parent group id
            var customersRootGroups = await _configurationGroupRepository.GetRootLevelByProject(projectId);
            return customersRootGroups.Count();
        }

        public async Task<ConfigurationGroupContract> Create(CreateConfigurationGroupContract contract)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            if (contract.ProjectId == null)
                throw new ArgumentNullException(nameof(contract.ProjectId));

            // validate name
            if (String.IsNullOrWhiteSpace(contract.Name)
                || contract.Name.Length > MaxConfigurationGroupNameLength)
                throw new ValidationCodeException(ConfigurationGroupValidationCodes.INVALID_CONFIGURATION_GROUP_NAME);

            // ensure that name is unique
            var existingByName = await _configurationGroupRepository.GetByNameAndByProject(contract.Name, contract.ProjectId);
            if (existingByName != null)
                throw new ValidationCodeException(ConfigurationGroupValidationCodes.CONFIGURATION_GROUP_ALREADY_EXISTS);

            int? sortingIndex = null;

            // validate parent
            if (!String.IsNullOrWhiteSpace(contract.ParentId))
            {
                var parent = await _configurationGroupRepository.Get(contract.ParentId);
                if (parent == null
                    || parent.ProjectId != contract.ProjectId
                    || parent.ApplicationId != contract.ApplicationId)
                    throw new ValidationCodeException(ConfigurationGroupValidationCodes.INVALID_CONFIGURATION_GROUP_PARENT);
                else
                    sortingIndex = await GetSortingIndex(parent, contract.ProjectId);
            }

            var model = _configurationGroupFactory.InitConfigurationGroupModel();
            model.Id = IdentifierPrefix + Guid.NewGuid().ToString();
            model.Name = contract.Name;
            model.ParentId = contract.ParentId;
            model.ApplicationId = contract.ApplicationId;
            model.CreatedOn = DateTime.Now;
            model.ProjectId = contract.ProjectId;
            model.SortingIndex = sortingIndex ?? await GetSortingIndex(null, contract.ProjectId);

            await _configurationGroupRepository.Insert(model);
            return _configurationGroupMapper.Map(model);
        }

        public async Task Delete(string id, string projectId, string applicationId)
        {
            if (String.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            if (String.IsNullOrWhiteSpace(projectId))
                throw new ArgumentNullException(nameof(projectId));

            var group = await _configurationGroupRepository.Get(id);
            if (group == null)
                throw new ValidationCodeException(ConfigurationGroupValidationCodes.CONFIGURATION_GROUP_DOES_NOT_EXIST);

            if (group.ProjectId != projectId)
                throw new ValidationCodeException(ConfigurationGroupValidationCodes.INVALID_CONFIGURATION_GROUP_PROJECT);

            if (group.ApplicationId != applicationId)
                throw new ValidationCodeException(ConfigurationGroupValidationCodes.INVALID_CONFIGURATION_GROUP_APPLICATION);

            await _configurationGroupRepository.Delete(group.Id);
        }

        public async Task<ConfigurationGroupContract> Edit(EditConfigurationGroupContract contract)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            if (String.IsNullOrWhiteSpace(contract.ProjectId))
                throw new ArgumentNullException(nameof(contract.ProjectId));

            // validate name
            if (String.IsNullOrWhiteSpace(contract.Name)
                || contract.Name.Length > MaxConfigurationGroupNameLength)
                throw new ValidationCodeException(ConfigurationGroupValidationCodes.INVALID_CONFIGURATION_GROUP_NAME);

            var group = await _configurationGroupRepository.Get(contract.Id);
            if (group == null)
                throw new ValidationCodeException(ConfigurationGroupValidationCodes.CONFIGURATION_GROUP_DOES_NOT_EXIST);

            if (group.ProjectId != contract.ProjectId)
                throw new ValidationCodeException(ConfigurationGroupValidationCodes.INVALID_CONFIGURATION_GROUP_PROJECT);

            if (!String.IsNullOrWhiteSpace(contract.ApplicationId)
                && !String.IsNullOrWhiteSpace(group.ApplicationId)
                && group.ApplicationId != contract.ApplicationId)
                throw new ValidationCodeException(ConfigurationGroupValidationCodes.INVALID_CONFIGURATION_GROUP_APPLICATION);

            // ensure that name is unique
            var existingByName = await _configurationGroupRepository.GetByNameAndByProject(contract.Name, contract.ProjectId);
            if (existingByName != null)
                throw new ValidationCodeException(ConfigurationGroupValidationCodes.CONFIGURATION_GROUP_ALREADY_EXISTS);

            group.Name = contract.Name;
            group.SortingIndex = contract.SortingIndex;
            group.ParentId = contract.ParentId;
            return _configurationGroupMapper.Map(_configurationGroupRepository.Save(group));
        }

        public async Task<ConfigurationGroupContract> Get(string id, string projectId, string applicationId)
        {
            if (String.IsNullOrWhiteSpace(id))
                return null;

            if (String.IsNullOrWhiteSpace(projectId))
                return null;

            var group = await _configurationGroupRepository.Get(id);
            if (group == null)
                return null;

            if (group.ProjectId != projectId)
                return null;

            if (group.ApplicationId != applicationId)
                return null;

            return _configurationGroupMapper.Map(group);
        }
    }
}

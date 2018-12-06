using Sconfig.Contracts.Configuration.ConfigurationGroup.Reads;
using Sconfig.Contracts.Configuration.ConfigurationGroup.Writes;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;
using Sconfig.Interfaces.Services;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconfig.Services
{
    public class ConfigurationGroupService : IConfigurationGroupService
    {
        private readonly IConfigurationGroupRepository _configurationGroupRepository;
        private readonly IConfigurationGroupFactory _configurationGroupFactory;

        public ConfigurationGroupService(
            IConfigurationGroupRepository configurationGroupRepository,
            IConfigurationGroupFactory configurationGroupFactory
            )
        {
            _configurationGroupRepository = configurationGroupRepository;
            _configurationGroupFactory = configurationGroupFactory;
        }

        // TODO: customer id to be set through api key on request (and perform validation)
        public async Task<ConfigurationGroupContract> Create(CreateConfigurationGroupContract contract, string customerId)
        {
            // ensure that there is no dublication of group names for customer
            var existing = await _configurationGroupRepository.GetByNameAndByCustomer(contract.Name, customerId);
            if (existing != null)
                throw new Exception("Configuration group with this name already exists for customer");

            // sorting index of configuration group
            int? sortIndex;
            
            // validate that parent is valid
            if (!String.IsNullOrEmpty(contract.ParentGroupId))
            {
                var parent = await _configurationGroupRepository.Get(contract.ParentGroupId);
                if (parent == null)
                    throw new Exception("Parent configuration group does not exist");

                // if parent configuration group belongs to other customer
                if (parent.CustomerId != customerId)
                    throw new Exception("Parent configuration group belogns to another customer");

                var parentsChildrenGroups = await _configurationGroupRepository.GetByParentGroupAndByCustomer(parent.Id, customerId);
                sortIndex = parentsChildrenGroups.Count();
            }
            else
            {
                // sort index based on count of groups that customers has without parent group id
                var customersRootGroups = await _configurationGroupRepository.GetWithoutParentGroupAndByCustomer(customerId);
                sortIndex = customersRootGroups.Count();
            }


            var model = _configurationGroupFactory.InitConfigurationGroupModel();
            model.Id = Guid.NewGuid().ToString();
            model.CreatedOn = DateTime.Now;
            model.CustomerId = customerId;
            model.ParentGroupId = String.IsNullOrEmpty(contract.ParentGroupId) ? null : contract.ParentGroupId;
            model.Name = contract.Name;
            model.SortingIndex = sortIndex.Value;

            var result = await _configurationGroupRepository.Insert(model);
            return Map(result);
        }

        private ConfigurationGroupContract Map(IConfigurationGroupModel model)
        {
            if (model == null)
                return null;

            return new ConfigurationGroupContract
            {
                Id = model.Id,
                Name = model.Name,
                CreatedOn = model.CreatedOn,
                ParentGroupId = model.ParentGroupId,
                SortingIndex = model.SortingIndex
            };
        }
    }
}

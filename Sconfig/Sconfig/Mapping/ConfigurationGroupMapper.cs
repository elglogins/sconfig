using Sconfig.Contracts.Configuration.ConfigurationGroup.Reads;
using Sconfig.Interfaces.Models;
using Sconfig.Mapping;

namespace Sconfig.Interfaces.Mapping
{
    class ConfigurationGroupMapper : IConfigurationGroupMapper
    {
        public ConfigurationGroupContract Map(IConfigurationGroupModel model)
        {
            if (model == null)
                return null;

            return new ConfigurationGroupContract
            {
                Id = model.Id,
                Name = model.Name,
                CreatedOn = model.CreatedOn,
                ParentId = model.ParentId,
                ApplicationId = model.ApplicationId,
                ProjectId = model.ProjectId,
                SortingIndex = model.SortingIndex
            };
        }
    }
}

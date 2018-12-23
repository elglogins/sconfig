using Sconfig.Contracts.Configuration.ConfigurationItem.Reads;
using Sconfig.Interfaces.Mapping;
using Sconfig.Interfaces.Models;

namespace Sconfig.Mapping
{
    class ConfigurationItemMapper : IConfigurationItemMapper
    {
        public ConfigurationItemContract Map(IConfigurationItemModel model)
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

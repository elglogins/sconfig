using Sconfig.Interfaces.Models.Descriptors;
using System;

namespace Sconfig.Interfaces.Models
{
    public interface IConfigurationItemModel : ISortableEntity, IGroupableEntity, IStringKeyEntity, IApplicationsEntity, IProjectsEntity
    {
        string EnvironmentId { get; set; }
        string Value { get; set; }
        DateTime CreatedOn { get; set; }
    }
}

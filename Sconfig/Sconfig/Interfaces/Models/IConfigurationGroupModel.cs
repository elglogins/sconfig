using Sconfig.Interfaces.Models.Descriptors;
using System;

namespace Sconfig.Interfaces.Models
{
    public interface IConfigurationGroupModel : ISortableEntity, IGroupableEntity, ICustomersEntity, IStringKeyEntity
    {
        string Name { get; set; }

        DateTime CreatedOn { get; set; }
    }
}

using Sconfig.Interfaces.Models.Descriptors;
using System;

namespace Sconfig.Interfaces.Models
{
    public interface IProjectModel : IStringKeyEntity, ICustomersEntity
    {
        string Name { get; set; }

        DateTime CreatedOn { get; set; }
    }
}

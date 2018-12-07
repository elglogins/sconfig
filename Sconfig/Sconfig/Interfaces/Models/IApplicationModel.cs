using Sconfig.Interfaces.Models.Descriptors;
using System;

namespace Sconfig.Interfaces.Models
{
    public interface IApplicationModel : IStringKeyEntity, IProjectsEntity
    {
        string Name { get; set; }

        DateTime CreatedOn { get; set; }
    }
}

using Sconfig.Interfaces.Models.Descriptors;
using System;
namespace Sconfig.Interfaces.Models
{
    public interface IEnvironmentModel : IStringKeyEntity, IProjectsEntity
    {
        string Name { get; set; }

        DateTime CreatedOn { get; set; }
    }
}

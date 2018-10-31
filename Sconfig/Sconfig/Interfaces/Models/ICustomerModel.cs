using Sconfig.Interfaces.Models.Descriptors;
using System;

namespace Sconfig.Interfaces.Models
{
    public interface ICustomerModel : IStringKeyEntity
    {
        string Name { get; set; }
        bool Active { get; set; }
        DateTime CreatedOn { get; set; }
    }
}

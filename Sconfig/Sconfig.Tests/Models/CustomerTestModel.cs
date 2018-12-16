using System;
using Sconfig.Interfaces.Models;

namespace Sconfig.Tests.Models
{
    class CustomerTestModel : ICustomerModel
    {
        public string Name { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Id { get; set; }
    }
}

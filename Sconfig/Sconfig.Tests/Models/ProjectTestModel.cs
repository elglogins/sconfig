using System;
using Sconfig.Interfaces.Models;

namespace Sconfig.Tests.Models
{
    class ProjectTestModel : IProjectModel
    {
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Id { get; set; }
        public string CustomerId { get; set; }
    }
}

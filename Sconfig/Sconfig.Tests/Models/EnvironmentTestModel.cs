using System;
using Sconfig.Interfaces.Models;

namespace Sconfig.Tests.Models
{
    class EnvironmentTestModel : IEnvironmentModel
    {
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Id { get; set; }
        public string ProjectId { get; set; }
    }
}

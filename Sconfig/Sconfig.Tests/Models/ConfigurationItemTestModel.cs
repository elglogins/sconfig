using System;
using Sconfig.Interfaces.Models;

namespace Sconfig.Tests.Models
{
    public class ConfigurationItemTestModel : IConfigurationItemModel
    {
        public string Name { get; set; }
        public string EnvironmentId { get; set; }
        public string Value { get; set; }
        public DateTime CreatedOn { get; set; }
        public int SortingIndex { get; set; }
        public string ParentId { get; set; }
        public string Id { get; set; }
        public string ApplicationId { get; set; }
        public string ProjectId { get; set; }
    }
}

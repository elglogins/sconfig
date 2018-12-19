using System;
using Sconfig.Interfaces.Models;

namespace Sconfig.Tests.Models
{
    class ConfigurationGroupTestModel : IConfigurationGroupModel
    {
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public int SortingIndex { get; set; }
        public string ParentId { get; set; }
        public string ProjectId { get; set; }
        public string ApplicationId { get; set; }
        public string Id { get; set; }
    }
}

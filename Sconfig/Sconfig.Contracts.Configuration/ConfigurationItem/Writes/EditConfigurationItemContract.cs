using System;
using System.Collections.Generic;
using System.Text;

namespace Sconfig.Contracts.Configuration.ConfigurationItem.Writes
{
    public class EditConfigurationItemContract
    {
        public string Id { get; set; }
        public string EnvironmentId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string ParentId { get; set; }
        public string ApplicationId { get; set; }
        public string ProjectId { get; set; }
        public int SortingIndex { get; set; }
    }
}

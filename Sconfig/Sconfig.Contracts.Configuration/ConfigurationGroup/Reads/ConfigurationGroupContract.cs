using System;
namespace Sconfig.Contracts.Configuration.ConfigurationGroup.Reads
{
    public class ConfigurationGroupContract
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ParentId { get; set; }
        public string ProjectId { get; set; }
        public string ApplicationId { get; set; }
        public int SortingIndex { get; set; }
    }
}

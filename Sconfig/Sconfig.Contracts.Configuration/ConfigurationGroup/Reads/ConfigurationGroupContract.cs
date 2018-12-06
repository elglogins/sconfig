using System;
namespace Sconfig.Contracts.Configuration.ConfigurationGroup.Reads
{
    public class ConfigurationGroupContract
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ParentGroupId { get; set; }
        public int SortingIndex { get; set; }
    }
}

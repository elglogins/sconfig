namespace Sconfig.Contracts.Configuration.ConfigurationGroup.Writes
{
    public class EditConfigurationGroupContract
    {
        public string Id { get; set; }
        public string ApplicationId { get; set; }
        public string ProjectId { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public int SortingIndex { get; set; }
    }
}

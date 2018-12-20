namespace Sconfig.Contracts.Configuration.ConfigurationItem.Writes
{
    public class CreateConfigurationItemContract
    {
        public string EnvironmentId { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public string ApplicationId { get; set; }
        public string ProjectId { get; set; }
    }
}

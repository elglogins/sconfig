﻿namespace Sconfig.Contracts.Configuration.ConfigurationGroup.Writes
{
    public class CreateConfigurationGroupContract
    {
        public string ProjectId { get; set; }

        public string ApplicationId { get; set; }

        public string ParentId { get; set; }

        public string Name { get; set; }
    }
}

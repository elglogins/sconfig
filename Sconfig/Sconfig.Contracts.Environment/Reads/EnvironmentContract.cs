using System;

namespace Sconfig.Contracts.Environment.Reads
{
    public class EnvironmentContract
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}

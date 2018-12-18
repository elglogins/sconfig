using System;

namespace Sconfig.Contracts.Application.Reads
{
    public class ApplicationContract
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}

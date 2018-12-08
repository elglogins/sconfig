using System;
namespace Sconfig.Contracts.Project.Reads
{
    public class ProjectContract
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}

using NPoco;
using Sconfig.Interfaces.Models;
using System;

namespace Sconfig.Configuration.Sql.Models
{
    [TableName("Project")]
    [PrimaryKey("Id", AutoIncrement = false)]
    class ProjectModel : IProjectModel
    {
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Id { get; set; }
        public string CustomerId { get; set; }
    }
}

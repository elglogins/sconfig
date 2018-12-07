using NPoco;
using Sconfig.Interfaces.Models;
using System;

namespace Sconfig.Configuration.Sql.Models
{
    [TableName("Environment")]
    [PrimaryKey("Id", AutoIncrement = false)]
    class EnvironmentModel : IEnvironmentModel
    {
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Id { get; set; }
        public string ProjectId { get; set; }
    }
}

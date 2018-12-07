using Sconfig.Interfaces.Models;
using System;
using NPoco;

namespace Sconfig.Configuration.Sql.Models
{
    [TableName("ConfigurationItem")]
    [PrimaryKey("Id")]
    class ConfigurationItemModel : IConfigurationItemModel
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public DateTime CreatedOn { get; set; }
        public int SortingIndex { get; set; }
        public string EnvironmentId { get; set; }
        public string ParentId { get; set; }
        public string ApplicationId { get; set; }
        public string ProjectId { get; set; }
    }
}

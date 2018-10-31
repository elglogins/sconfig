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
        public string ParentGroupId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int SortingIndex { get; set; }
    }
}

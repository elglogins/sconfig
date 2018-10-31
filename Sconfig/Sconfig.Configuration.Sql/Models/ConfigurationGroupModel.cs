using NPoco;
using Sconfig.Interfaces.Models;
using System;

namespace Sconfig.Configuration.Sql.Models
{
    [TableName("ConfigurationGroup")]
    [PrimaryKey("Id")]
    class ConfigurationGroupModel : IConfigurationGroupModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ParentGroupId { get; set; }
        public int SortingIndex { get; set; }
        public string CustomerId { get; set; }
    }
}

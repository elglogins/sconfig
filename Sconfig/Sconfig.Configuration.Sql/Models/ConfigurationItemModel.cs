using Sconfig.Interfaces.Models;
using System;

namespace Sconfig.Configuration.Sql.Models
{
    public class ConfigurationItemModel : IConfigurationItemModel
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public string ParentGroupId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int SortingIndex { get; set; }
    }
}

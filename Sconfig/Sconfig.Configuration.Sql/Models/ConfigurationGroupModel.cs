﻿using NPoco;
using Sconfig.Interfaces.Models;
using System;

namespace Sconfig.Configuration.Sql.Models
{
    [TableName("ConfigurationGroup")]
    [PrimaryKey("Id", AutoIncrement = false)]
    class ConfigurationGroupModel : IConfigurationGroupModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public int SortingIndex { get; set; }
        public string ParentId { get; set; }
        public string ProjectId { get; set; }
        public string ApplicationId { get; set; }
    }
}

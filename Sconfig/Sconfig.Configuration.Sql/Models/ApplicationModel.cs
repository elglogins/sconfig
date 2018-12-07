using NPoco;
using Sconfig.Interfaces.Models;
using System;
namespace Sconfig.Configuration.Sql.Models
{
    [TableName("Application")]
    [PrimaryKey("Id", AutoIncrement = false)]
    class ApplicationModel : IApplicationModel
    {
        public string Name {get;set;}
        public DateTime CreatedOn {get;set;}
        public string Id {get;set;}
        public string ProjectId {get;set;}
    }
}

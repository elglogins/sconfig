using NPoco;
using Sconfig.Interfaces.Models;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sconfig.Configuration.Sql.Tests")]
namespace Sconfig.Configuration.Sql.Models
{
    [TableName("Customer")]
    [PrimaryKey("Id", AutoIncrement = false)]
    class CustomerModel : ICustomerModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}

using Sconfig.Configuration.Sql.Interfaces;

namespace Sconfig.Configuration.Sql.Models
{
    public class SconfigSqlConfiguration : ISconfigSqlConfiguration
    {
        public string ConnectionString { get; set; }
    }
}

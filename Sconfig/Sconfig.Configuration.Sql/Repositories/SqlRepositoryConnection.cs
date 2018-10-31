using NPoco;
using Sconfig.Configuration.Sql.Interfaces;
using System.Data.SqlClient;

namespace Sconfig.Configuration.Sql.Repositories
{
    abstract class SqlRepositoryConnection
    {
        private readonly ISconfigSqlConfiguration _configuration;

        protected SqlRepositoryConnection(ISconfigSqlConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected Database GetClient()
        {
            return new Database(_configuration.ConnectionString, DatabaseType.SqlServer2008, SqlClientFactory.Instance);
        }
    }
}

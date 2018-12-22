using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using NPoco;
using Sconfig.Configuration.Sql.Interfaces;
using Sconfig.Interfaces.Models.Descriptors;
using Sconfig.Interfaces.Repositories;

namespace Sconfig.Configuration.Sql.Repositories
{
    internal abstract class AbstractSqlRespository<T, TK> : SqlRepositoryConnection, IRepo<T>
        where T : IStringKeyEntity
        where TK : class, T
    {
        public AbstractSqlRespository(ISconfigSqlConfiguration configuration)
            : base(configuration)
        {

        }

        public async Task Delete(string id)
        {
            using (var db = GetClient())
            {
                await db.ExecuteAsync($"DELETE FROM [{TableName}] WHERE [{PrimaryKey}] = @0", id);
            }
        }

        public virtual async Task<T> Get(string id)
        {
            using (var db = GetClient())
            {
                return await db.FirstOrDefaultAsync<TK>($"SELECT TOP 1 * FROM [{TableName}] WHERE [{PrimaryKey}] = @0", id);
            }
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            using (var db = GetClient())
            {
                return await db.FetchAsync<TK>($"SELECT * FROM [{TableName}]");
            }
        }

        public async Task<IEnumerable<T>> GetByIds(string[] ids)
        {
            using (var db = GetClient())
            {
                return await db.FetchAsync<TK>($"SELECT * FROM [{TableName}] WHERE [{PrimaryKey}] IN (@0)", ids);
            }
        }

        public async Task<T> Insert(T obj)
        {
            using (var db = GetClient())
            {
                var o = (TK)obj;
                await db.InsertAsync(TableName, PrimaryKey, false, o);
                return obj;
            }
        }

        public async Task Update(T obj)
        {
            using (var db = GetClient())
            {
                await db.UpdateAsync((TK)obj);
            }
        }

        public T Save(T obj)
        {
            using (var db = GetClient())
            {
                var o = (TK)obj;
                db.Save(o);
                return o;
            }
        }

        #region(Properties)

        private string _primaryKey;
        public virtual string PrimaryKey
        {
            get
            {
                if (string.IsNullOrEmpty(_primaryKey))
                {
                    var attr = (PrimaryKeyAttribute)typeof(TK).GetCustomAttribute(typeof(PrimaryKeyAttribute));
                    _primaryKey = attr != null ? attr.Value : typeof(T).Name + "Id";
                }

                return _primaryKey;
            }
        }

        private string _tableName;
        public virtual string TableName
        {
            get
            {
                if (string.IsNullOrEmpty(_tableName))
                {
                    var attr = (TableNameAttribute)typeof(TK).GetCustomAttribute(typeof(TableNameAttribute));
                    _tableName = attr != null ? attr.Value : typeof(T).Name;
                }

                return _tableName;
            }
        }

        #endregion
    }
}

using Sconfig.Interfaces.Models.Descriptors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sconfig.Interfaces.Repositories
{
    public interface IRepo<T>
            where T : IStringKeyEntity
    {
        Task<T> Insert(T obj);
        Task Update(T obj);
        Task<T> Get(string id);
        Task<IEnumerable<T>> GetByIds(string[] ids);
        Task Delete(string id);
        Task<IEnumerable<T>> GetAll();
        T Save(T obj);
    }
}

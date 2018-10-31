using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sconfig.Interfaces.Repositories
{
    public interface IConfigurationRepository
    {
        Task<IEnumerable<object>> Get();
    }
}

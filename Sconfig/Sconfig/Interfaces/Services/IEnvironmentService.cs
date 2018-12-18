using Sconfig.Contracts.Environment.Reads;
using Sconfig.Contracts.Environment.Writes;
using System.Threading.Tasks;

namespace Sconfig.Interfaces.Services
{
    public interface IEnvironmentService
    {
        Task<EnvironmentContract> Get(string id, string projectId);

        Task<EnvironmentContract> Create(CreateEnvironmentContract contract);

        Task<EnvironmentContract> Edit(EditEnvironmentContract contract, string projectId);

        Task Delete(string environmentId, string projectId);
    }
}

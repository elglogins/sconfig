using System.Threading.Tasks;
using Sconfig.Contracts.Application.Reads;
using Sconfig.Contracts.Application.Writes;

namespace Sconfig.Interfaces.Services
{
    public interface IApplicationService
    {
        Task<ApplicationContract> Get(string id, string projectId);

        Task<ApplicationContract> Create(CreateApplicationContract contract);

        Task Delete(string id, string projectId);

        Task<ApplicationContract> Edit(EditApplicationContract contract, string projectId);
    }
}

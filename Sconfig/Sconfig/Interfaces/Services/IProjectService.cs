using Sconfig.Contracts.Project.Reads;
using Sconfig.Contracts.Project.Writes;
using System.Threading.Tasks;

namespace Sconfig.Interfaces.Services
{
    public interface IProjectService
    {
        Task<ProjectContract> Get(string id, string customerId);

        Task<ProjectContract> Create(CreateProjectContract contract);
    }
}

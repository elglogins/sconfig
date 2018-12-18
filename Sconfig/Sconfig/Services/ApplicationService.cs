using System.Threading.Tasks;
using Sconfig.Contracts.Application.Reads;
using Sconfig.Contracts.Application.Writes;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Repositories;
using Sconfig.Interfaces.Services;

namespace Sconfig.Services
{
    class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IApplicationFactory _applicationFactory;

        public ApplicationService(IApplicationRepository applicationRepository, IApplicationFactory applicationFactory)
        {
            _applicationRepository = applicationRepository;
            _applicationFactory = applicationFactory;
        }

        public Task<ApplicationContract> Create(CreateApplicationContract contract)
        {
            throw new System.NotImplementedException();
        }

        public Task<ApplicationContract> Delete(string id, string projectId)
        {
            throw new System.NotImplementedException();
        }

        public Task<ApplicationContract> Edit(EditApplicationContract contract, string projectId)
        {
            throw new System.NotImplementedException();
        }

        public Task<ApplicationContract> Get(string id, string projectId)
        {
            throw new System.NotImplementedException();
        }
    }
}

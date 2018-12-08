using Sconfig.Contracts.Project.Reads;
using Sconfig.Contracts.Project.Writes;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;
using Sconfig.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Sconfig.Services
{
    internal class ProjectService : IProjectService
    {
        private readonly IProjectFactory _projectFactory;
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectFactory projectFactory, IProjectRepository projectRepository)
        {
            _projectFactory = projectFactory;
            _projectRepository = projectRepository;
        }

        public Task<ProjectContract> Create(CreateProjectContract contract)
        {
            throw new NotImplementedException();
        }

        public async Task<ProjectContract> Get(string id, string customerId)
        {
            if (String.IsNullOrEmpty(customerId))
                return null;

            var project = await _projectRepository.Get(id);
            if (project == null )
                return null;

            if (project.CustomerId != customerId)
                return null;

            return Map(project);
        }

        private ProjectContract Map(IProjectModel model)
        {
            if (model == null)
                return null;

            return new ProjectContract
            {
                Id = model.Id,
                Name = model.Name,
                CreatedOn = model.CreatedOn
            };
        }
    }
}

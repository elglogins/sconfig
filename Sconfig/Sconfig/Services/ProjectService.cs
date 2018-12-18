using System;
using System.Threading.Tasks;
using Sconfig.Contracts.Project.Enums;
using Sconfig.Contracts.Project.Reads;
using Sconfig.Contracts.Project.Writes;
using Sconfig.Exceptions;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;
using Sconfig.Interfaces.Services;

namespace Sconfig.Services
{
    class ProjectService : IProjectService
    {
        private readonly IProjectFactory _projectFactory;
        private readonly IProjectRepository _projectRepository;

        private const int MaxProjectNameLength = 50;

        public ProjectService(IProjectFactory projectFactory, IProjectRepository projectRepository)
        {
            _projectFactory = projectFactory;
            _projectRepository = projectRepository;
        }

        public async Task<ProjectContract> Create(CreateProjectContract contract, string customerId)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            // validate name
            if (String.IsNullOrWhiteSpace(contract.Name)
                || contract.Name.Length > MaxProjectNameLength)
                throw new ValidationCodeException(ProjectValidationCode.INVALID_PROJECT_NAME);

            // ensure that name is not used
            var existing = await _projectRepository.GetByName(contract.Name, customerId);
            if (existing != null)
                throw new ValidationCodeException(ProjectValidationCode.PROJECT_ALREADY_EXISTS);

            var model = _projectFactory.InitProjectModel();
            model.Id = Guid.NewGuid().ToString();
            model.Name = contract.Name;
            model.CustomerId = customerId;
            model.CreatedOn = DateTime.Now;
            await _projectRepository.Insert(model);
            return Map(model);
        }

        public async Task<ProjectContract> Get(string id, string customerId)
        {
            if (String.IsNullOrWhiteSpace(customerId))
                return null;

            if (String.IsNullOrWhiteSpace(id))
                return null;

            var project = await _projectRepository.Get(id);
            if (project == null)
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

        public async Task<ProjectContract> Edit(EditProjectContract contract, string customerId)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            // validate name
            if (String.IsNullOrWhiteSpace(contract.Name)
                || contract.Name.Length > MaxProjectNameLength)
                throw new ValidationCodeException(ProjectValidationCode.INVALID_PROJECT_NAME);

            var project = await _projectRepository.Get(contract.Id);
            if (project == null)
                throw new ValidationCodeException(ProjectValidationCode.PROJECT_DOES_NOT_EXIST);

            if (project.CustomerId != customerId)
                throw new ValidationCodeException(ProjectValidationCode.INVALID_PROJECT_OWNER);

            // ensure that name is unique
            var existingByName = await _projectRepository.GetByName(contract.Name, customerId);
            if (existingByName != null)
                throw new ValidationCodeException(ProjectValidationCode.PROJECT_ALREADY_EXISTS);

            project.Name = contract.Name;
            return Map(_projectRepository.Save(project));
        }

        public async Task Delete(string projectId, string customerId)
        {
            if (String.IsNullOrWhiteSpace(projectId))
                throw new ArgumentNullException(nameof(projectId));

            if (String.IsNullOrWhiteSpace(customerId))
                throw new ArgumentNullException(nameof(customerId));

            var project = await _projectRepository.Get(projectId);
            if (project == null)
                throw new ValidationCodeException(ProjectValidationCode.PROJECT_DOES_NOT_EXIST);

            if (project.CustomerId != customerId)
                throw new ValidationCodeException(ProjectValidationCode.INVALID_PROJECT_OWNER);

            await _projectRepository.Delete(projectId);
        }
    }
}

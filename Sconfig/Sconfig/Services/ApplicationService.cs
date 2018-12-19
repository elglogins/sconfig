using System;
using System.Threading.Tasks;
using Sconfig.Contracts.Application.Enums;
using Sconfig.Contracts.Application.Reads;
using Sconfig.Contracts.Application.Writes;
using Sconfig.Exceptions;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;
using Sconfig.Interfaces.Services;

namespace Sconfig.Services
{
    class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IApplicationFactory _applicationFactory;

        private const int MaxApplicationNameLength = 50;

        public ApplicationService(IApplicationRepository applicationRepository, IApplicationFactory applicationFactory)
        {
            _applicationRepository = applicationRepository;
            _applicationFactory = applicationFactory;
        }

        public async Task<ApplicationContract> Create(CreateApplicationContract contract)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            // validate name
            if (String.IsNullOrWhiteSpace(contract.Name)
                || contract.Name.Length > MaxApplicationNameLength)
                throw new ValidationCodeException(ApplicationValidationCodes.INVALID_APPLICATION_NAME);

            // ensure that name is not used
            var existing = await _applicationRepository.GetByName(contract.Name, contract.ProjectId);
            if (existing != null)
                throw new ValidationCodeException(ApplicationValidationCodes.APPLICATION_ALREADY_EXISTS);

            var model = _applicationFactory.InitApplicationModel();
            model.Id = Guid.NewGuid().ToString();
            model.Name = contract.Name;
            model.ProjectId = contract.ProjectId;
            model.CreatedOn = DateTime.Now;
            await _applicationRepository.Insert(model);
            return Map(model);
        }

        public async Task Delete(string id, string projectId)
        {
            if (String.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            if (String.IsNullOrWhiteSpace(projectId))
                throw new ArgumentNullException(nameof(projectId));

            var application = await _applicationRepository.Get(id);
            if (application == null)
                throw new ValidationCodeException(ApplicationValidationCodes.APPLICATION_DOES_NOT_EXIST);

            if (application.ProjectId != projectId)
                throw new ValidationCodeException(ApplicationValidationCodes.INVALID_APPLICATION_PROJECT);

            await _applicationRepository.Delete(id);
        }

        public async Task<ApplicationContract> Edit(EditApplicationContract contract, string projectId)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            // validate name
            if (String.IsNullOrWhiteSpace(contract.Name)
                || contract.Name.Length > MaxApplicationNameLength)
                throw new ValidationCodeException(ApplicationValidationCodes.INVALID_APPLICATION_NAME);

            var environment = await _applicationRepository.Get(contract.Id);
            if (environment == null)
                throw new ValidationCodeException(ApplicationValidationCodes.APPLICATION_DOES_NOT_EXIST);

            if (environment.ProjectId != projectId)
                throw new ValidationCodeException(ApplicationValidationCodes.INVALID_APPLICATION_PROJECT);

            // ensure name is unique
            var existingByName = await _applicationRepository.GetByName(contract.Name, projectId);
            if (existingByName != null)
                throw new ValidationCodeException(ApplicationValidationCodes.APPLICATION_ALREADY_EXISTS);

            environment.Name = contract.Name;
            return Map(_applicationRepository.Save(environment));
        }

        public async Task<ApplicationContract> Get(string id, string projectId)
        {
            var model = await _applicationRepository.Get(id);
            if (model == null || model.ProjectId != projectId)
                return null;

            return Map(model);
        }

        private ApplicationContract Map(IApplicationModel model)
        {
            if (model == null)
                return null;

            return new ApplicationContract
            {
                Id = model.Id,
                Name = model.Name,
                CreatedOn = model.CreatedOn
            };
        }
    }
}

using System;
using System.Threading.Tasks;
using Sconfig.Contracts.Environment.Enums;
using Sconfig.Contracts.Environment.Reads;
using Sconfig.Contracts.Environment.Writes;
using Sconfig.Exceptions;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;
using Sconfig.Interfaces.Services;

namespace Sconfig.Services
{
    class EnvironmentService : IEnvironmentService
    {
        private readonly IEnvironmentRepository _environmentRepository;
        private readonly IEnvironmentFactory _environmentFactory;

        private const int MaxEnvironmentNameLength = 50;
        private const string IdentifierPrefix = "E-";

        public EnvironmentService(IEnvironmentRepository environmentRepository, IEnvironmentFactory environmentFactory)
        {
            _environmentRepository = environmentRepository;
            _environmentFactory = environmentFactory;
        }

        public async Task<EnvironmentContract> Create(CreateEnvironmentContract contract)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            // validate name
            if (String.IsNullOrWhiteSpace(contract.Name)
                || contract.Name.Length > MaxEnvironmentNameLength)
                throw new ValidationCodeException(EnvironmentValidationCode.INVALID_ENVIRONMENT_NAME);

            // ensure that name is not used
            var existing = await _environmentRepository.GetByName(contract.Name, contract.ProjectId);
            if (existing != null)
                throw new ValidationCodeException(EnvironmentValidationCode.ENVIRONMENT_ALREADY_EXISTS);

            var model = _environmentFactory.InitEnvironmentModel();

            model.Id = IdentifierPrefix + Guid.NewGuid().ToString();
            model.Name = contract.Name;
            model.ProjectId = contract.ProjectId;
            model.CreatedOn = DateTime.Now;
            await _environmentRepository.Insert(model);
            return Map(model);
        }

        public async Task<EnvironmentContract> Get(string id, string projectId)
        {
            var model = await _environmentRepository.Get(id);
            if (model == null || model.ProjectId != projectId)
                return null;

            return Map(model);
        }

        public async Task Delete(string id, string projectId)
        {
            if (String.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            if (String.IsNullOrWhiteSpace(projectId))
                throw new ArgumentNullException(nameof(projectId));

            var environment = await _environmentRepository.Get(id);
            if (environment == null)
                throw new ValidationCodeException(EnvironmentValidationCode.ENVIRONMENT_DOES_NOT_EXIST);

            if (environment.ProjectId != projectId)
                throw new ValidationCodeException(EnvironmentValidationCode.INVALID_ENVIRONMENT_PROJECT);

            await _environmentRepository.Delete(id);
        }

        public async Task<EnvironmentContract> Edit(EditEnvironmentContract contract, string projectId)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            // validate name
            if (String.IsNullOrWhiteSpace(contract.Name)
                || contract.Name.Length > MaxEnvironmentNameLength)
                throw new ValidationCodeException(EnvironmentValidationCode.INVALID_ENVIRONMENT_NAME);

            var environment = await _environmentRepository.Get(contract.Id);
            if (environment == null)
                throw new ValidationCodeException(EnvironmentValidationCode.ENVIRONMENT_DOES_NOT_EXIST);

            if (environment.ProjectId != projectId)
                throw new ValidationCodeException(EnvironmentValidationCode.INVALID_ENVIRONMENT_PROJECT);

            // ensure name is unique
            var existingByName = await _environmentRepository.GetByName(contract.Name, projectId);
            if (existingByName != null)
                throw new ValidationCodeException(EnvironmentValidationCode.ENVIRONMENT_ALREADY_EXISTS);

            environment.Name = contract.Name;
            return Map(_environmentRepository.Save(environment));
        }


        private EnvironmentContract Map(IEnvironmentModel model)
        {
            if (model == null)
                return null;

            return new EnvironmentContract
            {
                Id = model.Id,
                Name = model.Name,
                CreatedOn = model.CreatedOn
            };
        }

    }
}

using Sconfig.Contracts.Customer;
using Sconfig.Contracts.Customer.Enums;
using Sconfig.Contracts.Customer.Reads;
using Sconfig.Contracts.Customer.Writes;
using Sconfig.Exceptions;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;
using Sconfig.Interfaces.Services;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Sconfig.Services
{
    class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerFactory _customerFactory;

        private const int CustomerNameLength = 30;

        public CustomerService(
            ICustomerRepository customerRepository,
            ICustomerFactory customerFactory
            )
        {
            _customerRepository = customerRepository;
            _customerFactory = customerFactory;
        }

        public virtual async Task<CustomerContract> Get(string id)
        {
            return Map(await _customerRepository.Get(id));
        }

        public async Task<CustomerContract> Create(CreateCustomerContract contract)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            // validate name
            if (String.IsNullOrWhiteSpace(contract.Name)
                || contract.Name.Length > CustomerNameLength)
                throw new ValidationCodeException(CustomerValidationCode.INVALID_CUSTOMER_NAME);

            // ensure that name is not used
            var existing = await _customerRepository.GetByName(contract.Name);
            if (existing != null)
                throw new ValidationCodeException(CustomerValidationCode.CUSTOMER_ALREADY_EXISTS);

            var model = _customerFactory.InitCustomerModel();
            model.Id = Guid.NewGuid().ToString();
            model.Name = contract.Name;
            model.Active = true;
            model.CreatedOn = DateTime.Now;
            await _customerRepository.Insert(model);
            return Map(model);
        }

        private CustomerContract Map(ICustomerModel model)
        {
            if (model == null)
                return null;

            return new CustomerContract
            {
                Id = model.Id,
                Name = model.Name,
                CreatedOn = model.CreatedOn,
                Active = model.Active
            };
        }

        public async Task<CustomerContract> Disable(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            var customer = await _customerRepository.Get(id);
            if (customer == null)
                throw new ValidationCodeException(CustomerValidationCode.CUSTOMER_DOES_NOT_EXIST);

            customer.Active = false;
            return Map(_customerRepository.Save(customer));
        }

        public async Task<CustomerContract> Enable(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            var customer = await _customerRepository.Get(id);
            if (customer == null)
                throw new ValidationCodeException(CustomerValidationCode.CUSTOMER_DOES_NOT_EXIST);

            customer.Active = true;
            return Map(_customerRepository.Save(customer));
        }

        public async Task<CustomerContract> Edit(EditCustomerContract contract)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            // validate name
            if (String.IsNullOrWhiteSpace(contract.Name)
                || contract.Name.Length > CustomerNameLength)
                throw new ValidationCodeException(CustomerValidationCode.INVALID_CUSTOMER_NAME);

            var customer = await _customerRepository.Get(contract.Id);
            if (customer == null)
                throw new ValidationCodeException(CustomerValidationCode.CUSTOMER_DOES_NOT_EXIST);

            customer.Name = contract.Name;
            return Map(_customerRepository.Save(customer));
        }
    }
}

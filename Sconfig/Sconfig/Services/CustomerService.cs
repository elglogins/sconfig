using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sconfig.Contracts.Customer;
using Sconfig.Contracts.Customer.Enums;
using Sconfig.Contracts.Customer.Reads;
using Sconfig.Contracts.Customer.Writes;
using Sconfig.Exceptions;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Mapping;
using Sconfig.Interfaces.Repositories;
using Sconfig.Interfaces.Services;

namespace Sconfig.Services
{
    class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerFactory _customerFactory;
        private readonly ICustomerMapper _customerMapper;

        private const int MaxCustomerNameLength = 30;
        private const string IdentifierPrefix = "C-";

        public CustomerService(ICustomerRepository customerRepository, ICustomerFactory customerFactory, ICustomerMapper customerMapper)
        {
            _customerRepository = customerRepository;
            _customerFactory = customerFactory;
            _customerMapper = customerMapper;
        }

        public async Task<IEnumerable<CustomerContract>> GetAll()
        {
            return (await _customerRepository.GetAll()).Select(_customerMapper.Map);
        }

        public async Task<CustomerContract> Get(string id)
        {
            return _customerMapper.Map(await _customerRepository.Get(id));
        }

        public async Task<CustomerContract> Create(CreateCustomerContract contract)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            // validate name
            if (String.IsNullOrWhiteSpace(contract.Name)
                || contract.Name.Length > MaxCustomerNameLength)
                throw new ValidationCodeException(CustomerValidationCode.INVALID_CUSTOMER_NAME);

            // ensure that name is not used
            var existing = await _customerRepository.GetByName(contract.Name);
            if (existing != null)
                throw new ValidationCodeException(CustomerValidationCode.CUSTOMER_ALREADY_EXISTS);

            var model = _customerFactory.InitCustomerModel();
            model.Id = IdentifierPrefix + Guid.NewGuid().ToString();
            model.Name = contract.Name;
            model.Active = true;
            model.CreatedOn = DateTime.Now;
            await _customerRepository.Insert(model);
            return _customerMapper.Map(model);
        }

        public async Task<CustomerContract> Disable(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            var customer = await _customerRepository.Get(id);
            if (customer == null)
                throw new ValidationCodeException(CustomerValidationCode.CUSTOMER_DOES_NOT_EXIST);

            customer.Active = false;
            return _customerMapper.Map(_customerRepository.Save(customer));
        }

        public async Task<CustomerContract> Enable(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            var customer = await _customerRepository.Get(id);
            if (customer == null)
                throw new ValidationCodeException(CustomerValidationCode.CUSTOMER_DOES_NOT_EXIST);

            customer.Active = true;
            return _customerMapper.Map(_customerRepository.Save(customer));
        }

        public async Task<CustomerContract> Edit(EditCustomerContract contract)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            // validate name
            if (String.IsNullOrWhiteSpace(contract.Name)
                || contract.Name.Length > MaxCustomerNameLength)
                throw new ValidationCodeException(CustomerValidationCode.INVALID_CUSTOMER_NAME);

            var customer = await _customerRepository.Get(contract.Id);
            if (customer == null)
                throw new ValidationCodeException(CustomerValidationCode.CUSTOMER_DOES_NOT_EXIST);

            // ensure that name is unique
            var alreadyExistingByName = await _customerRepository.GetByName(contract.Name);
            if (alreadyExistingByName != null)
                throw new ValidationCodeException(CustomerValidationCode.CUSTOMER_ALREADY_EXISTS);

            customer.Name = contract.Name;
            return _customerMapper.Map(_customerRepository.Save(customer));
        }
    }
}

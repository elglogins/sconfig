using Sconfig.Contracts.Customer;
using Sconfig.Contracts.Customer.Reads;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;
using Sconfig.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Sconfig.Services
{
    class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerFactory _customerFactory;

        public CustomerService(
            ICustomerRepository customerRepository,
            ICustomerFactory customerFactory
            )
        {
            _customerRepository = customerRepository;
            _customerFactory = customerFactory;
        }

        public async Task<CustomerContract> Get(string id)
        {
            return Map(await _customerRepository.Get(id));
        }

        public async Task<CustomerContract> Create(CreateCustomerContract contract)
        {
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
                Name = model.Name
            };
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Sconfig.Contracts.Customer;
using Sconfig.Contracts.Customer.Reads;
using Sconfig.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Sconfig.Applications.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerContract>> Get(string id)
        {
            var customerContract = await _customerService.Get(id);
            if (customerContract == null)
                return NotFound();

            return Ok(customerContract);
        }

        [HttpPost]
        public async Task<ActionResult<CustomerContract>> Post([FromBody] CreateCustomerContract contract)
        {
            if (contract == null)
                return BadRequest();

            if (String.IsNullOrEmpty(contract.Name))
                return BadRequest();

            var customerContractResult = await _customerService.Create(contract);
            return Ok(customerContractResult);
        }
    }
}

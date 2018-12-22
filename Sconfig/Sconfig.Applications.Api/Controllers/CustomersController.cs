using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sconfig.Contracts.Customer;
using Sconfig.Contracts.Customer.Reads;
using Sconfig.Contracts.Customer.Writes;
using Sconfig.Interfaces.Services;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerContract>>> GetAll()
        {
            var customerContracts = await _customerService.GetAll();
            return Ok(customerContracts);
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
        public async Task<ActionResult<CustomerContract>> Create([FromBody] CreateCustomerContract contract)
        {
            var customerContractResult = await _customerService.Create(contract);
            return Ok(customerContractResult);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerContract>> Edit(string id, [FromBody] EditCustomerContract contract)
        {
            var customerContractResult = await _customerService.Edit(contract);
            return Ok(customerContractResult);
        }

        [HttpPost("{id}/disable")]
        public async Task<ActionResult<CustomerContract>> Disable(string id)
        {
            var customerContractResult = await _customerService.Disable(id);
            return Ok(customerContractResult);
        }

        [HttpPost("{id}/enable")]
        public async Task<ActionResult<CustomerContract>> Enable(string id)
        {
            var customerContractResult = await _customerService.Enable(id);
            return Ok(customerContractResult);
        }
    }
}

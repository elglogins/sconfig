using Microsoft.AspNetCore.Mvc;
using Sconfig.Contracts.Configuration.ConfigurationGroup.Reads;
using Sconfig.Contracts.Configuration.ConfigurationGroup.Writes;
using Sconfig.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sconfig.Applications.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationGroupController : ControllerBase
    {
        private readonly IConfigurationGroupService _configurationGroupService;

        public ConfigurationGroupController(
            IConfigurationGroupService configurationGroupService
            )
        {
            _configurationGroupService = configurationGroupService;
        }

        [HttpPost]
        public async Task<ActionResult<ConfigurationGroupContract>> Post([FromBody] CreateConfigurationGroupContract contract)
        {
            if (contract == null)
                return BadRequest();

            if (String.IsNullOrEmpty(contract.Name))
                return BadRequest();

            // TODO: use actual from api key header
            var customerId = "4bb36290-5fd4-4952-919c-d40d1c391b0f";

            var customerContractResult = await _configurationGroupService.Create(contract, customerId);
            return Ok(customerContractResult);
        }
    }
}

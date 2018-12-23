using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sconfig.Contracts.Configuration.ConfigurationItem.Reads;
using Sconfig.Contracts.Configuration.ConfigurationItem.Writes;
using Sconfig.Interfaces.Mapping;
using Sconfig.Interfaces.Repositories;
using Sconfig.Interfaces.Services;

namespace Sconfig.Applications.Api.Controllers
{
    [ApiController]
    public class ConfigurationItemsController : ControllerBase
    {
        private readonly IConfigurationItemService _configurationItemService;
        private readonly IConfigurationItemRepository _configurationItemRepository;
        private readonly IConfigurationItemMapper _configurationItemMapper;

        public ConfigurationItemsController(IConfigurationItemService configurationItemService, IConfigurationItemRepository configurationItemRepository, IConfigurationItemMapper configurationItemMapper)
        {
            _configurationItemService = configurationItemService;
            _configurationItemRepository = configurationItemRepository;
            _configurationItemMapper = configurationItemMapper;
        }

        [HttpGet]
        [Route("api/Projects/{projectId}/Items/{id}")]
        public async Task<ActionResult<ConfigurationItemContract>> Get(string projectId, string id, string applicationId, string environmentId)
        {
            return await _configurationItemService.Get(id, projectId, applicationId, environmentId);
        }

        [HttpGet]
        [Route("api/Projects/{projectId}/Items")]
        public async Task<ActionResult<IEnumerable<ConfigurationItemContract>>> GetByProject(string projectId)
        {
            return Ok((await _configurationItemRepository.GetByProject(projectId)).Select(_configurationItemMapper.Map));
        }

        [HttpGet]
        [Route("api/Projects/{projectId}/Applications/{applicationId}/Items")]
        public async Task<ActionResult<IEnumerable<ConfigurationItemContract>>> GetByApplication(string projectId, string applicationId)
        {
            return Ok((await _configurationItemRepository.GetByApplication(projectId, applicationId)).Select(_configurationItemMapper.Map));
        }

        [HttpPost]
        [Route("api/Projects/{projectId}/Items")]
        public async Task<ActionResult<ConfigurationItemContract>> Create(string projectId, [FromBody]CreateConfigurationItemContract contract)
        {
            var response = await _configurationItemService.Create(contract);
            return Ok(response);
        }

        [HttpPut]
        [Route("api/Projects/{projectId}/Items/{id}")]
        public async Task<ActionResult<ConfigurationItemContract>> Edit(string projectId, string id, [FromBody] EditConfigurationItemContract contract)
        {
            var response = await _configurationItemService.Edit(contract);
            return Ok(response);
        }

        [HttpDelete]
        [Route("api/Projects/{projectId}/Items/{id}")]
        public async Task<ActionResult> Delete(string projectId, string id, string applicationId = null, string environmentId = null)
        {
            await _configurationItemService.Delete(id, projectId, applicationId, environmentId);
            return Ok();
        }
    }
}

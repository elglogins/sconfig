using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sconfig.Contracts.Configuration.ConfigurationGroup.Reads;
using Sconfig.Contracts.Configuration.ConfigurationGroup.Writes;
using Sconfig.Interfaces.Repositories;
using Sconfig.Interfaces.Services;
using Sconfig.Mapping;

namespace Sconfig.Applications.Api.Controllers
{
    [ApiController]
    public class ConfigurationGroupsController : ControllerBase
    {
        private readonly IConfigurationGroupService _configurationGroupService;
        private readonly IConfigurationGroupRepository _configurationGroupRepository;
        private readonly IConfigurationGroupMapper _configurationGroupMapper;

        public ConfigurationGroupsController(IConfigurationGroupService configurationGroupService, IConfigurationGroupRepository configurationGroupRepository, IConfigurationGroupMapper configurationGroupMapper)
        {
            _configurationGroupService = configurationGroupService;
            _configurationGroupRepository = configurationGroupRepository;
            _configurationGroupMapper = configurationGroupMapper;
        }

        [HttpGet]
        [Route("api/Projects/{projectId}/Groups/{id}")]
        public async Task<ActionResult<ConfigurationGroupContract>> Get(string projectId, string id)
        {
            return await _configurationGroupService.Get(id, projectId, null);
        }

        [HttpGet]
        [Route("api/Projects/{projectId}/Applications/{applicationId}/Groups/{id}")]
        public async Task<ActionResult<ConfigurationGroupContract>> Get(string projectId, string id, string applicationId)
        {
            return await _configurationGroupService.Get(id, projectId, applicationId);
        }

        [HttpGet]
        [Route("api/Projects/{projectId}/Groups")]
        public async Task<ActionResult<IEnumerable<ConfigurationGroupContract>>> GetByProject(string projectId)
        {
            return Ok((await _configurationGroupRepository.GetByProject(projectId)).Select(_configurationGroupMapper.Map));
        }

        [HttpGet]
        [Route("api/Projects/{projectId}/Applications/{applicationId}/Groups")]
        public async Task<ActionResult<IEnumerable<ConfigurationGroupContract>>> GetByProjectApplication(string projectId, string applicationId)
        {
            return Ok((await _configurationGroupRepository.GetByApplication(projectId, applicationId)).Select(_configurationGroupMapper.Map));
        }


        [HttpPost]
        [Route("api/Projects/{projectId}/Groups")]
        public async Task<ActionResult<ConfigurationGroupContract>> Create(string projectId, [FromBody]CreateConfigurationGroupContract contract)
        {
            var response = await _configurationGroupService.Create(contract);
            return Ok(response);
        }

        [HttpPut]
        [Route("api/Projects/{projectId}/Groups/{id}")]
        public async Task<ActionResult<ConfigurationGroupContract>> Edit(string projectId, string id, [FromBody]EditConfigurationGroupContract contract)
        {
            var response = await _configurationGroupService.Edit(contract);
            return Ok(response);
        }

        [HttpDelete]
        [Route("api/Projects/{projectId}/Groups/{id}")]
        public async Task<ActionResult> Delete(string projectId, string id)
        {
            await _configurationGroupService.Delete(id, projectId, null);
            return Ok();
        }

        [HttpDelete]
        [Route("api/Projects/{projectId}/Applications/{applicationId}/Groups/{id}")]
        public async Task<ActionResult> Delete(string projectId, string id, string applicationId)
        {
            await _configurationGroupService.Delete(id, projectId, applicationId);
            return Ok();
        }
    }
}

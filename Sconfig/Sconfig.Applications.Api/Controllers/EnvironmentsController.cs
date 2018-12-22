using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sconfig.Contracts.Environment.Reads;
using Sconfig.Contracts.Environment.Writes;
using Sconfig.Interfaces.Mapping;
using Sconfig.Interfaces.Repositories;
using Sconfig.Interfaces.Services;

namespace Sconfig.Applications.Api.Controllers
{
    [ApiController]
    public class EnvironmentsController : ControllerBase
    {
        private readonly IEnvironmentService _environmentService;
        private readonly IEnvironmentRepository _environmentRepository;
        private readonly IEnvironmentMapper _environmentMapper;

        public EnvironmentsController(IEnvironmentService environmentService, IEnvironmentRepository environmentRepository, IEnvironmentMapper environmentMapper)
        {
            _environmentService = environmentService;
            _environmentRepository = environmentRepository;
            _environmentMapper = environmentMapper;
        }

        [HttpGet]
        [Route("api/Projects/{projectId}/Environments/{id}")]
        public async Task<ActionResult<EnvironmentContract>> Get(string projectId, string id)
        {
            return await _environmentService.Get(id, projectId);
        }

        [HttpGet]
        [Route("api/Projects/{projectId}/Environments")]
        public async Task<ActionResult<IEnumerable<EnvironmentContract>>> GetByProject(string projectId)
        {
            if (string.IsNullOrWhiteSpace(projectId))
                return BadRequest();

            return Ok((await _environmentRepository.GetByProject(projectId)).Select(_environmentMapper.Map));
        }

        [HttpPost]
        [Route("api/Projects/{projectId}/Environments")]
        public async Task<ActionResult<EnvironmentContract>> Create(string projectId, [FromBody]CreateEnvironmentContract contract)
        {
            var response = await _environmentService.Create(contract);
            return Ok(response);
        }

        [HttpPost]
        [Route("api/Projects/{projectId}/Environments/{id}")]
        public async Task<ActionResult<EnvironmentContract>> Edit(string projectId, string id, [FromBody]EditEnvironmentContract contract)
        {
            var response = await _environmentService.Edit(contract, projectId);
            return Ok(response);
        }

        [HttpDelete]
        [Route("api/Projects/{projectId}/Environments/{id}")]
        public async Task<ActionResult<EnvironmentContract>> Delete(string projectId, string id)
        {
            await _environmentService.Delete(id, projectId);
            return Ok();
        }
    }
}

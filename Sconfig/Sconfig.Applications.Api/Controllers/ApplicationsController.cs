using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sconfig.Contracts.Application.Reads;
using Sconfig.Contracts.Application.Writes;
using Sconfig.Interfaces.Mapping;
using Sconfig.Interfaces.Repositories;
using Sconfig.Interfaces.Services;

namespace Sconfig.Applications.Api.Controllers
{
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IApplicationMapper _applicationMapper;

        public ApplicationsController(IApplicationService applicationService, IApplicationRepository applicationRepository, IApplicationMapper applicationMapper)
        {
            _applicationService = applicationService;
            _applicationRepository = applicationRepository;
            _applicationMapper = applicationMapper;
        }

        [HttpGet]
        [Route("api/Projects/{projectId}/Applications/{id}")]
        public async Task<ActionResult<ApplicationContract>> Get(string projectId, string id)
        {
            return await _applicationService.Get(id, projectId);
        }

        [HttpGet]
        [Route("api/Projects/{projectId}/Applications")]
        public async Task<ActionResult<ApplicationContract>> GetAllByProject(string projectId)
        {
            return Ok((await _applicationRepository.GetByProject(projectId)).Select(_applicationMapper.Map));
        }

        [HttpPost]
        [Route("api/Projects/{projectId}/Applications")]
        public async Task<ActionResult<ApplicationContract>> Create(string projectId, [FromBody]CreateApplicationContract contract)
        {
            var response = await _applicationService.Create(contract);
            return Ok(response);
        }

        [HttpPut]
        [Route("api/Projects/{projectId}/Applications/{id}")]
        public async Task<ActionResult<ApplicationContract>> Edit(string projectId, string id, [FromBody]EditApplicationContract contract)
        {
            var response = await _applicationService.Edit(contract, projectId);
            return Ok(response);
        }

        [HttpDelete]
        [Route("api/Projects/{projectId}/Applications/{id}")]
        public async Task<ActionResult> Delete(string projectId, string id)
        {
            await _applicationService.Delete(id, projectId);
            return Ok();
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sconfig.Contracts.Project.Reads;
using Sconfig.Contracts.Project.Writes;
using Sconfig.Interfaces.Services;

namespace Sconfig.Applications.Api.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        [Route("api/Customers/{customerId}/Projects/{id}")]
        public async Task<ActionResult<ProjectContract>> GetProject(string customerId, string id)
        {
            var customerContract = await _projectService.Get(id, customerId);
            if (customerContract == null)
                return NotFound();

            return Ok(customerContract);
        }

        [HttpPost]
        [Route("api/Customers/{customerId}/Projects")]
        public async Task<ActionResult<ProjectContract>> Create(string customerId, [FromBody]CreateProjectContract contract)
        {
            var response = await _projectService.Create(contract, customerId);
            return Ok(response);
        }

        [HttpPut]
        [Route("api/Customers/{customerId}/Projects/{id}")]
        public async Task<ActionResult<ProjectContract>> Edit(string customerId, string id, [FromBody]EditProjectContract contract)
        {
            var response = await _projectService.Edit(contract, customerId);
            return Ok(response);
        }

        [HttpDelete]
        [Route("api/Customers/{customerId}/Projects/{id}")]
        public async Task<ActionResult<ProjectContract>> Delete(string customerId, string id)
        {
            await _projectService.Delete(id, customerId);
            return Ok();
        }

        [HttpGet]
        [Route("api/Customers/{customerId}/Projects")]
        public async Task<ActionResult<ProjectContract>> GetProjects(string customerId)
        {
            var customerContract = await _projectService.GetByCustomer(customerId);
            if (customerContract == null)
                return NotFound();

            return Ok(customerContract);
        }
    }
}

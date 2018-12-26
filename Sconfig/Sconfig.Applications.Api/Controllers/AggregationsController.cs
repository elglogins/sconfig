using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sconfig.Configuration.Sql.Factories;
using Sconfig.Contracts.Aggregation.Reads;
using Sconfig.Interfaces.Factories;

namespace Sconfig.Applications.Api.Controllers
{
    [ApiController]
    public class AggregationsController : ControllerBase
    {
        private readonly IAggregationFactory _aggregationFactory;

        public AggregationsController(IAggregationFactory aggregationFactory)
        {
            _aggregationFactory = aggregationFactory;
        }

        [HttpGet]
        [Route("api/Projects/{projectId}/Aggregations")]
        public async Task<ActionResult<AggregationsTreeContract>> ForProject(string projectId)
        {
            return Ok(await _aggregationFactory.InitAggregationsTreeForProject(projectId));
        }

        [HttpGet]
        [Route("api/Projects/{projectId}/Applications/{applicationId}/Aggregations")]
        public async Task<ActionResult<AggregationsTreeContract>> ForApplicaiton(string projectId, string applicationId)
        {
            return Ok(await _aggregationFactory.InitAggregationsTreeForApplication(projectId, applicationId));
        }
    }
}

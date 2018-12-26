using System.Threading.Tasks;
using Sconfig.Contracts.Aggregation.Reads;

namespace Sconfig.Interfaces.Factories
{
    public interface IAggregationFactory
    {
        Task<AggregationsTreeContract> InitAggregationsTreeForProject(string projectId);

        Task<AggregationsTreeContract> InitAggregationsTreeForApplication(string projectId, string applicationId);
    }
}

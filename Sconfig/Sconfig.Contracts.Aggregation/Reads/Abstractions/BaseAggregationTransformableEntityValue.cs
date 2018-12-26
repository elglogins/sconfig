using Sconfig.Contracts.Aggregation.Decorators;

namespace Sconfig.Contracts.Aggregation.Reads
{
    public abstract class BaseAggregationTransformableEntityValue : IAggregationStringKeyEntity
    {
        public string Value { get; set; }
        public string EnvironmentId { get; set; }
        public string Id { get; set; }
    }
}

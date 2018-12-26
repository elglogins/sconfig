using Sconfig.Contracts.Aggregation.Decorators;
using Sconfig.Contracts.Aggregation.Enums;

namespace Sconfig.Contracts.Aggregation.Reads
{
    public abstract class BaseConfigurationAggregationEntity : IConfigurationAggregationEntity
    {
        public string Name { get; set; }
        public string ApplicationId { get; set; }
        public string ProjectId { get; set; }
        public AggregationEntityType Type { get; set; }
        public int SortingIndex { get; set; }
        public string ParentId { get; set; }
    }
}

using Sconfig.Contracts.Aggregation.Enums;

namespace Sconfig.Contracts.Aggregation.Decorators
{
    public interface IConfigurationAggregationEntity : IAggregationSortableEntity, IAggregationGroupableEntity
    {
        string Name { get; set; }
        string ApplicationId { get; set; }
        string ProjectId { get; set; }
        AggregationEntityType Type { get; set; }
    }
}

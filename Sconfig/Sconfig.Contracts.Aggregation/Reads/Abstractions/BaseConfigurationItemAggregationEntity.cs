using System.Collections.Generic;
using Sconfig.Contracts.Aggregation.Decorators;

namespace Sconfig.Contracts.Aggregation.Reads
{
    public abstract class BaseConfigurationItemAggregationEntity : BaseConfigurationAggregationEntity, IAggregationTransformableEntity
    {
        public IList<AggregationTransformableEntityValue> Values { get; set; }
    }
}

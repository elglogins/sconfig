using System.Collections.Generic;
using Sconfig.Contracts.Aggregation.Decorators;

namespace Sconfig.Contracts.Aggregation.Reads
{
    public abstract class BaseConfigurationGroupAggregationEntity : BaseConfigurationAggregationEntity, IAggregationStringKeyEntity
    {
        public string Id { get; set; }
        public IList<BaseConfigurationAggregationEntity> Children { get; set; }
    }
}

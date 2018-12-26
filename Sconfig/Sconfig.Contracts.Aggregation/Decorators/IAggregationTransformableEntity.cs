using System.Collections.Generic;
using Sconfig.Contracts.Aggregation.Reads;

namespace Sconfig.Contracts.Aggregation.Decorators
{
    public interface IAggregationTransformableEntity
    {
        IList<AggregationTransformableEntityValue> Values { get; set; }
    }
}

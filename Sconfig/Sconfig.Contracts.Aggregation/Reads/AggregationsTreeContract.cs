using System.Collections.Generic;

namespace Sconfig.Contracts.Aggregation.Reads
{
    public class AggregationsTreeContract
    {
        public IEnumerable<BaseConfigurationAggregationEntity> Configurations { get; set; }
    }
}

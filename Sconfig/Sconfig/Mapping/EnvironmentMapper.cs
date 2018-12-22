using Sconfig.Contracts.Environment.Reads;
using Sconfig.Interfaces.Mapping;
using Sconfig.Interfaces.Models;

namespace Sconfig.Mapping
{
    class EnvironmentMapper : IEnvironmentMapper
    {
        public EnvironmentContract Map(IEnvironmentModel model)
        {
            if (model == null)
                return null;

            return new EnvironmentContract
            {
                Id = model.Id,
                Name = model.Name,
                CreatedOn = model.CreatedOn
            };
        }
    }
}

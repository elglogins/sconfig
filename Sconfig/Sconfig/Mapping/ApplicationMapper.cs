using Sconfig.Contracts.Application.Reads;
using Sconfig.Interfaces.Mapping;
using Sconfig.Interfaces.Models;

namespace Sconfig.Mapping
{
    class ApplicationMapper : IApplicationMapper
    {
        public ApplicationContract Map(IApplicationModel model)
        {
            if (model == null)
                return null;

            return new ApplicationContract
            {
                Id = model.Id,
                Name = model.Name,
                CreatedOn = model.CreatedOn
            };
        }
    }
}

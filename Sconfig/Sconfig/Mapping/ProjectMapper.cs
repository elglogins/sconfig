using Sconfig.Contracts.Project.Reads;
using Sconfig.Interfaces.Mapping;
using Sconfig.Interfaces.Models;

namespace Sconfig.Mapping
{
    class ProjectMapper : IProjectMapper
    {
        public ProjectContract Map(IProjectModel model)
        {
            if (model == null)
                return null;

            return new ProjectContract
            {
                Id = model.Id,
                Name = model.Name,
                CreatedOn = model.CreatedOn
            };
        }
    }
}

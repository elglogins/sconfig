using Sconfig.Contracts.Project.Reads;
using Sconfig.Interfaces.Models;

namespace Sconfig.Interfaces.Mapping
{
    public interface IProjectMapper
    {
        ProjectContract Map(IProjectModel model);
    }
}

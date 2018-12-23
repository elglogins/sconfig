using Sconfig.Contracts.Application.Reads;
using Sconfig.Interfaces.Models;

namespace Sconfig.Interfaces.Mapping
{
    public interface IApplicationMapper
    {
        ApplicationContract Map(IApplicationModel model);
    }
}

using Sconfig.Contracts.Environment.Reads;
using Sconfig.Interfaces.Models;

namespace Sconfig.Interfaces.Mapping
{
    public interface IEnvironmentMapper
    {
        EnvironmentContract Map(IEnvironmentModel model);
    }
}

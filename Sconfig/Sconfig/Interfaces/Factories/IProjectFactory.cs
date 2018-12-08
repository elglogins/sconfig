using Sconfig.Interfaces.Models;

namespace Sconfig.Interfaces.Factories
{
    public interface IProjectFactory
    {
        IProjectModel InitProjectModel();
    }
}

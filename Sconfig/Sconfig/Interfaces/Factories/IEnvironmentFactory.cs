﻿using Sconfig.Interfaces.Models;

namespace Sconfig.Interfaces.Factories
{
    public interface IEnvironmentFactory
    {
        IEnvironmentModel InitEnvironmentModel();
    }
}

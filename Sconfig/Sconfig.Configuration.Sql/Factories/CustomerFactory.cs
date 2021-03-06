﻿using Sconfig.Configuration.Sql.Models;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;
using System.Runtime.CompilerServices;

namespace Sconfig.Configuration.Sql.Factories
{
    class CustomerFactory : ICustomerFactory
    {
        public virtual ICustomerModel InitCustomerModel()
        {
            return new CustomerModel();
        }
    }
}

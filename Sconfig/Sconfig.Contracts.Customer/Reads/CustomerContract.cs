﻿using System;

namespace Sconfig.Contracts.Customer.Reads
{
    public class CustomerContract
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Active { get; set; }
    }
}

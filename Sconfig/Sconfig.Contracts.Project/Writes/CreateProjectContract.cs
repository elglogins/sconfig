﻿using System;
namespace Sconfig.Contracts.Project.Writes
{
    public class CreateProjectContract
    {
        public string Name { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}

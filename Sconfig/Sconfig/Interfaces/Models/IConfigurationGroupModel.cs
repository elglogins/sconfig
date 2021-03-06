﻿using Sconfig.Interfaces.Models.Descriptors;
using System;

namespace Sconfig.Interfaces.Models
{
    public interface IConfigurationGroupModel : ISortableEntity, IGroupableEntity, IProjectsEntity, IApplicationsEntity, IStringKeyEntity
    {
        string Name { get; set; }

        DateTime CreatedOn { get; set; }
    }
}

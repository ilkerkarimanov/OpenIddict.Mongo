﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDDD.Core.Domain
{
    public interface IHasHistory
    {
        DateTime? Created { get; }
        DateTime? Modified { get; }
        void SetAsCreated();
        void SetAsModified();
    }
}

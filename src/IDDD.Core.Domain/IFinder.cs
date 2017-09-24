﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IDDD.Core.Domain
{
    public interface IFinder<TEntity, in TIdentity> 
        where TEntity : Entity
        where TIdentity: Identity
    {
        Task<Maybe<TEntity>> GetById(TIdentity id, CancellationToken cancellationToken = default(CancellationToken));
    }
}

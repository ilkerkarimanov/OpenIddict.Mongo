using IDDD.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDDD.Domain.Membership.Clients
{
    public class ClientId: Identity
    {
        public ClientId() { }

        public ClientId(string id)
            : base(id)
        {
        }
    }
}

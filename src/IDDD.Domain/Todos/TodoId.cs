using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDDD.Domain.Todos
{
    public class TodoId: IDDD.Core.Domain.Identity
    {
        public TodoId()
        {

        }

        public TodoId(string id): base(id)
        {

        }
    }
}

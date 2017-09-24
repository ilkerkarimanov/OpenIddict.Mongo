using IDDD.Core.Domain;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IDDD.Domain.Todos
{
    public interface ITodoFinder: IFinder<Todo, TodoId>
    {
        Task<IEnumerable<Todo>> All(CancellationToken cancellationToken = default(CancellationToken));

    }
}

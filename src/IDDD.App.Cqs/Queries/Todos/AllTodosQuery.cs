using IDDD.App.Cqs.QueryResult.Todos;
using IDDD.Core.Cqs.Query;
using System.Collections.Generic;

namespace IDDD.App.Cqs.Queries.Todos
{
    public class AllTodosQuery : IQuery<IEnumerable<TodoResult>> { }
}

using IDDD.Core.Domain;

namespace IDDD.Domain.Todos
{
    public interface ITodoRepository: IRepository<Todo, TodoId>
    {
    }
}

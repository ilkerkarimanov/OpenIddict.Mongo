using IDDD.Core.Domain;
using IDDD.Domain.Actions;

namespace IDDD.Domain.Todos
{
    public class TodoAction: Action<TodoState>
    {

        public TodoAction(
            TodoState state) : base(state) {
        }
    }
    
}

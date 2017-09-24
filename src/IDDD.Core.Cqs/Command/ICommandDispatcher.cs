using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDDD.Core.Cqs.Command
{
    public interface ICommandDispatcher
    {
        Task<TReturn> DispatchAsync<TCommand,TReturn>(TCommand command);
    }
}

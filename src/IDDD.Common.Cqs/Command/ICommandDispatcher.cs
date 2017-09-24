using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDDD.Common.Cqs.Command
{
    public interface ICommandDispatcher
    {
        Task<TReturn> DispatchAsync<TCommand,TReturn>(TCommand command);
    }
}

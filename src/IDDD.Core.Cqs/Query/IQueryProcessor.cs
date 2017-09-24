using System.Threading.Tasks;

namespace IDDD.Core.Cqs.Query
{
    public interface IQueryProcessor
    {
        Task<TResult> ProcessAsync<TResult>(IQuery<TResult> query);
    }
}
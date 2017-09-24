using System.Threading.Tasks;

namespace IDDD.Common.Cqs.Query
{
    public interface IQueryProcessor
    {
        Task<TResult> ProcessAsync<TResult>(IQuery<TResult> query);
    }
}
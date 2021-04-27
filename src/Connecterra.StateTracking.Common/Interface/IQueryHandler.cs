using Connecterra.StateTracking.Common.Core;
using System.Threading.Tasks;

namespace Connecterra.StateTracking.Common.Interface
{
    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        Task<Result<TResult>> HandleAsync(TQuery query);
    }
}

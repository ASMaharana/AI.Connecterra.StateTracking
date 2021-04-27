using Connecterra.StateTracking.Common.Core;
using Connecterra.StateTracking.Common.Interface;
using System;
using System.Threading.Tasks;

namespace Connecterra.StateTracking.Common.Dispatcher
{
    public interface IQueryDispatcher
    {
        Task<Result<TResult>> HandleAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>;
    }
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryDispatcher(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public async Task<Result<TResult>> HandleAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
        {
            var service = this._serviceProvider.GetService(typeof(IQueryHandler<TQuery, TResult>)) as IQueryHandler<TQuery, TResult>;
            return await service.HandleAsync(query);
        }
    }
}

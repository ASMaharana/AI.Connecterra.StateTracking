using Connecterra.StateTracking.Common.Core;
using Connecterra.StateTracking.Common.Dispatcher;
using Connecterra.StateTracking.Common.Interface;
using System.Threading.Tasks;

namespace Connecterra.StateTracking.Common.Routing
{
    public interface IRouter
    {
        Task<Result> ExecuteAsync<T>(T command) where T : ICommand;

        Task<Result<TResult>> ExecuteAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>;
    }
    public class Router : IRouter
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public Router(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        public Task<Result> ExecuteAsync<T>(T command) where T : ICommand
        {
            return _commandDispatcher.HandleAsync(command);
        }

        public Task<Result<TResult>> ExecuteAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
        {
            return _queryDispatcher.HandleAsync<TQuery, TResult>(query);
        }
    }
}

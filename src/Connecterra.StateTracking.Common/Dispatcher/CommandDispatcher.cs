using Connecterra.StateTracking.Common.Core;
using Connecterra.StateTracking.Common.Interface;
using System;
using System.Threading.Tasks;

namespace Connecterra.StateTracking.Common.Dispatcher
{
    public interface ICommandDispatcher
    {
        Task<Result> HandleAsync<T>(T command) where T : ICommand;
    }
    public class CommandDispatcher : ICommandDispatcher
    {
        private IServiceProvider _serviceProvider;

        public CommandDispatcher(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public async Task<Result> HandleAsync<T>(T command) where T : ICommand
        {
            var service = this._serviceProvider.GetService(typeof(ICommandHandler<T>)) as ICommandHandler<T>;
            return await service.HandleAsync(command);
        }
    }
}

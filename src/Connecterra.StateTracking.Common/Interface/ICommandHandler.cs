using Connecterra.StateTracking.Common.Core;
using System.Threading.Tasks;

namespace Connecterra.StateTracking.Common.Interface
{
    public interface ICommandHandler<TCommand> where TCommand:ICommand  
    {  
        Task<Result> HandleAsync(TCommand command);
    }
}

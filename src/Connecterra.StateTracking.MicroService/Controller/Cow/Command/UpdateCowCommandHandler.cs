using Connecterra.StateTracking.Common.Core;
using Connecterra.StateTracking.Common.Interface;
using Connecterra.StateTracking.MicroService.Domain;
using Connecterra.StateTracking.MicroService.Domain.Repository;
using System.Threading.Tasks;

namespace Connecterra.StateTracking.MicroService.Command
{
    public class UpdateCowCommandHandler : ICommandHandler<UpdateCowCommand>
    {
        private readonly IRepositorySnapshotDecorator<Cow> _cowRepository;

        public UpdateCowCommandHandler(IRepositorySnapshotDecorator<Cow> cowRepository)
        {
            _cowRepository = cowRepository;
        }
        
        public async Task<Result> HandleAsync(UpdateCowCommand command)
        {
            var getResult = await _cowRepository.LoadAsync(command.CowId);
            if (getResult.IsSuccess)
            {
                var changeStateResult = getResult.Value.ChangeState(command.State);
                if (changeStateResult.IsSuccess)
                {
                    var saveResult = await _cowRepository.SaveAsync(getResult.Value);
                    return saveResult;
                }
                return changeStateResult;
            }
            return Result.Error(getResult.ErrorMessage);
        }
    }
}

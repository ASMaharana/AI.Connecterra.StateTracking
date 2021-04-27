using Connecterra.StateTracking.Common.Core;
using Connecterra.StateTracking.Common.Interface;
using Connecterra.StateTracking.MicroService.Domain;
using Connecterra.StateTracking.MicroService.Domain.Repository;
using System.Threading.Tasks;

namespace Connecterra.StateTracking.MicroService.Command
{
    public class CreateCowCommandHandler : ICommandHandler<CreateCowCommand>
    {
        private readonly IRepository<Cow> _cowRepository;

        public CreateCowCommandHandler(IRepository<Cow> cowRepository)
        {
            _cowRepository = cowRepository;
        }
        
        public async Task<Result> HandleAsync(CreateCowCommand command)
        {
            var cow = new Cow(command.CowId, command.FarmId);
            var result = await _cowRepository.SaveAsync(cow);
            return result;
        }
    }
}

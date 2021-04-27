using Connecterra.StateTracking.Common.Interface;
using Connecterra.StateTracking.MicroService.Domain;
using Newtonsoft.Json;

namespace Connecterra.StateTracking.MicroService.Command
{
    public class UpdateCowCommand : ICommand
    {
        public string CowId { get; }
        public string FarmId { get; }
        public CowState State { get; }

        [JsonConstructor]
        public UpdateCowCommand(string cowId, string farmId, CowState state)
        {
            this.CowId = cowId;
            this.FarmId = farmId;
            this.State = state;
        }
    }
}

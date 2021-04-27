using Connecterra.StateTracking.Common.Interface;
using Newtonsoft.Json;

namespace Connecterra.StateTracking.MicroService.Command
{
    public class CreateCowCommand : ICommand
    {
        public string CowId { get; }
        public string FarmId { get; }

        [JsonConstructor]
        public CreateCowCommand(string cowId, string farmId)
        {
            this.CowId = cowId;
            this.FarmId = farmId;
        }
    }
}

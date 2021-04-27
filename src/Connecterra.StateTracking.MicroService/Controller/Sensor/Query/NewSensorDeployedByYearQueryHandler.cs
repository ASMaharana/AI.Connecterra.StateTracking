using Connecterra.StateTracking.Common.Core;
using Connecterra.StateTracking.Common.Interface;
using Connecterra.StateTracking.MicroService.Projections;
using Connecterra.StateTracking.Persistent.Projections;
using System.Threading.Tasks;

namespace Connecterra.StateTracking.MicroService.Query
{
    public class NewSensorDeployedByYearQueryHandler : IQueryHandler<NewSensorDeployedByYearQuery, int>
    {
        private readonly IViewRepository _viewRepository;
        public NewSensorDeployedByYearQueryHandler(IViewRepository viewRepository)
        {
            _viewRepository = viewRepository;
        }
        public async Task<Result<int>> HandleAsync(NewSensorDeployedByYearQuery query)
        {
            var result = await _viewRepository.LoadViewAsync(Helper.GetNewSensorViewName(query.YearToQuery));

            if (result.IsSuccess)
            {
                var newSensorDeployedByYear = result.Value.Payload.ToObject<NewSensorDeployedByYear>().Deployed;
                return Result.OK(newSensorDeployedByYear.Values.Count / 12);
            }

            return Result.Error<int>(result.ErrorMessage);
        }
    }
}

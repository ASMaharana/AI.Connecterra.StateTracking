using Connecterra.StateTracking.Common.Core;
using Connecterra.StateTracking.Common.Interface;
using Connecterra.StateTracking.MicroService.Projections;
using Connecterra.StateTracking.Persistent.Projections;
using System.Threading.Tasks;

namespace Connecterra.StateTracking.MicroService.Query
{
    public class SensorDiedByMonthQueryHandler : IQueryHandler<SensorDiedByMonthQuery, int>
    {
        private readonly IViewRepository _viewRepository;
        public SensorDiedByMonthQueryHandler(IViewRepository viewRepository)
        {
            _viewRepository = viewRepository;
        }
        public async Task<Result<int>> HandleAsync(SensorDiedByMonthQuery query)
        {
            var result = await _viewRepository.LoadViewAsync(Helper.GetDiedSensorViewName(query.MonthToQuery, query.YearToQuery)) ;
            if (result.IsSuccess)
                return Result.OK(result.Value.Payload.ToObject<SensorDiedByMonth>().Count);

            return Result.Error<int>(result.ErrorMessage);
        }
    }
}

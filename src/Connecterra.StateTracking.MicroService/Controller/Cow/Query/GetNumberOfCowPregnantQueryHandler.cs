using Connecterra.StateTracking.Common.Core;
using Connecterra.StateTracking.Common.Interface;
using Connecterra.StateTracking.MicroService.Projections;
using Connecterra.StateTracking.Persistent.Projections;
using System.Threading.Tasks;

namespace Connecterra.StateTracking.MicroService.Query
{
    public class GetNumberOfCowPregnantQueryHandler : IQueryHandler<GetNumberOfCowPregnantQuery, int>
    {
        private readonly IViewRepository _viewRepository;
        public GetNumberOfCowPregnantQueryHandler(IViewRepository viewRepository)
        {
            _viewRepository = viewRepository;
        }
        public async Task<Result<int>> HandleAsync(GetNumberOfCowPregnantQuery query)
        {
            var result = await _viewRepository.LoadViewAsync(Helper.GetPregnantCowViewName(query.DateToQuery));
            if (result.IsSuccess)
                return Result.OK(result.Value.Payload.ToObject<PregnantCowDailyTotal>().Count);

            return Result.Error<int>(result.ErrorMessage);
        }
    }
}

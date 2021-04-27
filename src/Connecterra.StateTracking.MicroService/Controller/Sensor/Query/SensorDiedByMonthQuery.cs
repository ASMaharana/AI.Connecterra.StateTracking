using Connecterra.StateTracking.Common.Interface;

namespace Connecterra.StateTracking.MicroService.Query
{
    public class SensorDiedByMonthQuery : IQuery<int>
    {
        public int MonthToQuery { get; }
        public int YearToQuery { get; }
        public SensorDiedByMonthQuery(int monthToQuery, int yearToQuery)
        {
            MonthToQuery = monthToQuery;
            YearToQuery = yearToQuery;
        }
    }
}

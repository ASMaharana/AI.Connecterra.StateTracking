using Connecterra.StateTracking.Common.Interface;

namespace Connecterra.StateTracking.MicroService.Query
{
    public class NewSensorDeployedByYearQuery : IQuery<int>
    {
        public int YearToQuery { get; }
        public NewSensorDeployedByYearQuery(int yearToQuery)
        {
            YearToQuery = yearToQuery;
        }
    }
}

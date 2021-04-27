using Connecterra.StateTracking.Common.Interface;
using System;

namespace Connecterra.StateTracking.MicroService.Query
{
    public class GetNumberOfCowPregnantQuery : IQuery<int>
    {
        public DateTime DateToQuery { get; }
        public GetNumberOfCowPregnantQuery(DateTime dateToQuery)
        {
            DateToQuery = dateToQuery.Date;
        }
    }
}

using System;

namespace Connecterra.StateTracking.MicroService.Projections
{
    public class Helper 
    {
        public static string GetNewSensorViewName(int year)
        {
            return $"NewSensorDeployedByYear:{year}";
        }
        public static string GetPregnantCowViewName(DateTime timeStamp)
        {
            return $"PregnantCowDailyTotal:{timeStamp.ToString("dd-M-yyyy")}";
        }
        public static string GetDiedSensorViewName(int month, int year)
        {
            return $"SensorDiedByMonth:{month}-{year}";
        }
    }
}
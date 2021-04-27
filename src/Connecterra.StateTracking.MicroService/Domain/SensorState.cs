namespace Connecterra.StateTracking.MicroService.Domain
{
    public enum SensorState : int
    {
        Inventory = 1, 
        Deployed = 2, 
        FarmerTriage = 3, 
        Returned = 4, 
        Dead = 5, 
        Refurbished = 6
    }
}

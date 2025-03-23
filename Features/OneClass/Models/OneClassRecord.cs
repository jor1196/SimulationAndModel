namespace SimulationAndModel.Features.OneClass.Models;

public class OneClassRecord
{
    public TimeSpan? CurrentTime { get; set; }
    public TimeSpan? CustomerNextArrivalTime { get; set; }
    public TimeSpan? NextEndServiceTime { get; set; }
    public int CustomerQueueCount { get; set; }
    public bool ServiceStationState { get; set; }
    public int CustomerServedCount { get; set; }
}

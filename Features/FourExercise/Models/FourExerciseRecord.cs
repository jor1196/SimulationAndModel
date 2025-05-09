namespace SimulationAndModel.Features.FourExercise.Models;

public record FourExerciseRecord
{
    public required TimeSpan CurrentTime { get; set; }
    public required TimeSpan CustomerNextArrivalTime { get; set; }
    public required TimeSpan ImportantCustomerNextArrivalTime { get; set; }
    public required TimeSpan NextEndServiceTime { get; set; }
    public int CustomerQueueCount { get; set; }
    public int ImportantCustomerQueueCount { get; set; }
    public bool ServiceStationState { get; set; }
    public int CustomerServedCount { get; set; }
    public int ImportantCustomerServedCount { get; set; }
}

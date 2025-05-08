namespace SimulationAndModel.Features.TwoExercise.Models;

public class TwoExerciseRecord
{
    public required TimeSpan CurrentTime { get; set; }
    public required TimeSpan CustomerNextArrivalTime { get; set; }
    public required TimeSpan NextEndServiceTime { get; set; }
    public required TimeSpan ServerAbandonmentTime { get; set; }
    public required TimeSpan ServerReturnTime { get; set; }
    public int CustomerQueueCount { get; set; }
    public bool ServiceStationState { get; set; }
    public bool ServerState { get; set; }
    public int CustomerServedCount { get; set; }
}

namespace SimulationAndModel.Features.OneExercise.Models;

public class OneExerciseRecord
{
    public required TimeSpan CurrentTime { get; set; }
    public required TimeSpan CustomerNextArrivalTime { get; set; }
    public required TimeSpan NextEndServiceTime { get; set; }
    public int CustomerQueueCount { get; set; }
    public bool ServiceStationState { get; set; }
    public int CustomerServedCount { get; set; }
}

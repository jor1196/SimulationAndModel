namespace SimulationAndModel.Features.OneClass.Models;

public class FilterOneClass
{
    public TimeSpan? InitialTime { get; set; }
    public int? CustomerArrivalTime { get; set; }
    public int? EndServiceTime { get; set; }
    public TimeSpan? EndTime { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationAndModel.Features.ThreeExercise.Models;

public record ThreeExerciseRecord
{
    public required TimeSpan CurrentTime { get; set; }
    public required TimeSpan CustomerNextArrivalTime { get; set; }
    public required TimeSpan NextEndServiceTime { get; set; }
    public int CustomerQueueCount { get; set; }
    public bool ServiceStationState { get; set; }
    public int CustomerServedCount { get; set; }
    public List<TimeSpan> CustomerQueueTime { get; set; } = [];
    public List<TimeSpan> CustomerLeaveQueueTimes { get; set; } = [];
}

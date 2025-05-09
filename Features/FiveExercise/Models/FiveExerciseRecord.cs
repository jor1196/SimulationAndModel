using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationAndModel.Features.FiveExercise.Models;

public record FiveExerciseRecord
{
    public required TimeSpan CurrentTime { get; set; }
    public required TimeSpan CustomerNextArrivalTime { get; set; }
    public required TimeSpan NextEndServiceTime { get; set; }
    public int CustomerQueueCount { get; set; }
    public bool ServiceStationState { get; set; }
    public bool SecureZoneState { get; set; }
    public int CustomerServedCount { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationAndModel.Common.Extensions;

internal static class TimeSpanExtensions
{
    internal static TimeSpan GeneratorRandomTimeSpan(int maxHours)
    {
        Random _random = new();

        long randomTicks = (long)(_random.NextDouble() * TimeSpan.FromHours(maxHours).Ticks);

        return new TimeSpan(randomTicks);
    }

    internal static TimeSpan SumSeconds(this TimeSpan source, int seconds)
    {
        return new(source.Hours,  source.Minutes, source.Seconds + seconds);
    }
}

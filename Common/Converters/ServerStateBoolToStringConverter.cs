using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationAndModel.Common.Converters;

internal class ServerStateBoolToStringConverter : IValueConverter
{
    private const string RESTED = "Descansando";
    private const string WORK = "Trabajando";

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var valueConveted = (bool?)value;
        if (valueConveted.GetValueOrDefault())
            return WORK;

        return RESTED;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (string.IsNullOrEmpty(value?.ToString()))
            return false;

        string state = (string)value;
        if (state == WORK)
            return true;

        return false;
    }
}

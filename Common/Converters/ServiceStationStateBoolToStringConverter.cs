using System.Globalization;

namespace SimulationAndModel.Common.Converters;

public class ServiceStationStateBoolToStringConverter : IValueConverter
{
    private const string AVAILABLE = "Disponible";
    private const string BUSY = "Ocupado";
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var valueConveted = (bool?)value;
        if (valueConveted.GetValueOrDefault())
            return BUSY;

        return AVAILABLE;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (string.IsNullOrEmpty(value?.ToString()))
            return false;

        string state = (string)value;
        if (state == BUSY)
            return true;

        return false;
    }
}

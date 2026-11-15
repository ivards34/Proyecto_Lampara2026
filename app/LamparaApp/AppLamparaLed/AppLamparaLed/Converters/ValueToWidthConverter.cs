using System.Globalization;
using Microsoft.Maui.Controls;

namespace AppLamparaLed.Converters;

public class ValueToWidthConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double doubleValue)
        {
            // Escalar el valor para que se vea bien como barra
            // Asumimos un rango típico de 0-50 para temperatura/humedad
            var scaleFactor = 2.0; // Cada valor = 2px de ancho
            return doubleValue * scaleFactor;
        }
        return 0;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

using EcoMonitor.Infrastracture.Abstractions;
using SixLabors.ImageSharp;

namespace EcoMonitor.Infrastracture.Utils
{
    public class ExifHelper
    {
        public static double? ConvertExifGpsToDouble(Rational[] coordinates, string refValue)
        {
            if(coordinates == null || coordinates.Length != 3) return null;
            
            double degrees = coordinates[0].ToDouble(); // конвертация градусов в double
            double minutes = coordinates[1].ToDouble(); // конвертация минут в double
            double seconds = coordinates[2].ToDouble(); // конвертация секунды в double

            // DecimalDegrees = Degrees + Minutes / 60 + Seconds / 3600
            // Преобразование из DMS(Degrees, Minutes, Seconds) в DD(Decimal Degrees))
            double result = degrees + minutes / 60 + seconds / 3600;
            
            if (refValue == "S" || refValue == "W")
                result = -result;

            return result;
        }
    }
}

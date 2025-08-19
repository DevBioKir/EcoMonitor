using SixLabors.ImageSharp;

namespace EcoMonitor.Infrastracture.Abstractions
{
    public interface IExifHelper
    {
        double? ConvertExifGpsToDouble(Rational[] coordinates, string refValue);
    }
}
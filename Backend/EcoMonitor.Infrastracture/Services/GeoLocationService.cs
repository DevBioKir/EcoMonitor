using EcoMonitor.Infrastracture.Abstractions;
using EcoMonitor.Infrastracture.Utils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace EcoMonitor.Infrastracture.Services
{
    public class GeolocationService : IGeolocationService
    {
        public (double? Latitude, double? Longitude) GeoLocationService(ExifProfile profile)
        {
            if (profile == null) return (null, null);

            profile.TryGetValue(ExifTag.GPSLatitude, out IExifValue<Rational[]>? latRaw); // Значение координаты
            profile.TryGetValue(ExifTag.GPSLatitudeRef, out IExifValue<string>? latRefRaw); // Референс направления North(Север от 0 до +90 градусов)
                                                                                              // South (Юг от 0 до -90 градусов)
            profile.TryGetValue(ExifTag.GPSLongitude, out IExifValue<Rational[]>? lonRaw); // Значение координаты
            profile.TryGetValue(ExifTag.GPSLongitudeRef, out IExifValue<string>? lonRefRaw); // Референс направления East(Восток от 0 до +180 градусов)
                                                                                             // West(Запад от 0 до -180 градусов)

            var lat = latRaw?.Value;
            var latRef = latRefRaw?.Value;

            var lon = lonRaw?.Value;
            var lonRef = lonRefRaw?.Value;

            return (ExifHelper.ConvertExifGpsToDouble(lat, latRef),
                ExifHelper.ConvertExifGpsToDouble(lon, lonRef));
        }
    }
}

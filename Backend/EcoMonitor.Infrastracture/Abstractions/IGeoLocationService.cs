using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace EcoMonitor.Infrastracture.Abstractions
{
    public interface IGeolocationService
    {
        public (double? Latitude, double? Longitude) GeoLocationService(ExifProfile profile);
    }
}

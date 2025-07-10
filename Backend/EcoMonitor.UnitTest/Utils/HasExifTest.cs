using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace EcoMonitor.UnitTest.Utils
{
    public class HasExifTest
    {
        [Fact]
        public async Task HasExifMetadataAsync()
        {
            // Arrage
            var imagePath = Path.Combine("TestPhotos", "Бак_возле_работы.jpeg");

            Assert.True(File.Exists(imagePath), $"Файл не найден: {imagePath}");

            // Arrage
            using var image = await Image.LoadAsync(imagePath);
            var exif = image.Metadata.ExifProfile;

            // Assert EXIF presence
            Assert.NotNull(exif);
            Assert.NotEmpty(exif.Values);

            // Conclusion to the console
            foreach (var value in exif.Values)
            {
                Console.WriteLine($"{value.Tag}: {value.GetValue()}");
            }

            // Assert GPS coordinates
            var lat = TryGetGpsCoordinate(exif, ExifTag.GPSLatitude, ExifTag.GPSLatitudeRef);
            var lon = TryGetGpsCoordinate(exif, ExifTag.GPSLongitude, ExifTag.GPSLongitudeRef);

            Assert.NotNull(lat);
            Assert.NotNull(lon);

            Console.WriteLine($"Latitude: {lat}");
            Console.WriteLine($"Longitude: {lon}");
        }

        private static double? TryGetGpsCoordinate(
            ExifProfile exif,
            ExifTag<Rational[]> tag,
            ExifTag<string> refTag)
        {
            if (exif == null) return null;

            exif.TryGetValue(tag, out IExifValue<Rational[]>? rawValue);
            exif.TryGetValue(refTag, out IExifValue<string>? refValue);

            var coordinates = rawValue?.Value;
            var direction = refValue?.Value;

            return ConvertExifGpsToDouble(coordinates, direction);

        }

        private static double? ConvertExifGpsToDouble(Rational[] coordinates, string refValue)
        {
            if (coordinates == null || coordinates.Length != 3) return null;

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

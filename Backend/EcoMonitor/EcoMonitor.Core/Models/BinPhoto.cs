

namespace EcoMonitor.Core.Models
{
    public class BinPhoto
    {
        public Guid Id { get; private set; }
        public string FileName { get; private set; } = string.Empty;
        public string UrlFile { get; private set; } = string.Empty;
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public DateTime UploadedAt { get; private set; }
        public string BinType { get; private set; } = string.Empty;
        public double FillLevel { get; private set; }
        public bool IsOutsideBin { get; private set; }
        public string Comment { get; private set; } = string.Empty;

        private BinPhoto() { }

        private BinPhoto(
            string fileName,
            string urlFile,
            double latitude,
            double longitude,
            string binType,
            double fillLevel,
            bool isOutsideBin,
            string comment)
        {
            Id = Guid.NewGuid();
            FileName = fileName;
            UrlFile = urlFile;
            Latitude = latitude;
            Longitude = longitude;
            UploadedAt = DateTime.UtcNow;
            BinType = binType;
            FillLevel = fillLevel;
            IsOutsideBin = isOutsideBin;
            Comment = comment;
            Validate();
        }

        private void Validate()
        {
            if(string.IsNullOrWhiteSpace(FileName)) throw new ArgumentException("FileName required");
            if(string.IsNullOrWhiteSpace(UrlFile)) throw new ArgumentException("UrlFile required");
            if(Latitude < -90 || Latitude > 90) throw new ArgumentOutOfRangeException(nameof(Latitude), "Latitude must be between -90 and 90 degrees");
            if (Longitude < -180 || Longitude > 180) throw new ArgumentOutOfRangeException(nameof(Longitude), "Longitude must be between -180 and 180 degrees");
            if (string.IsNullOrWhiteSpace(BinType)) throw new ArgumentException("BinType required");
            if (FillLevel < 0.0 || FillLevel > 1.0) 
                throw new ArgumentOutOfRangeException(nameof(FillLevel), "FillLevel must be between 0.0 and 1.0");
        }

        public BinPhoto Create(
            string fileName,
            string urlFile,
            double latitude,
            double longitude,
            DateTime uploadedAt,
            string binType,
            double fillLevel,
            bool isOutsideBin,
            string comment)
        {
            return new BinPhoto(
                fileName, 
                urlFile, 
                latitude, 
                longitude,
                binType,
                fillLevel,
                isOutsideBin,
                comment);
        }
    }
}

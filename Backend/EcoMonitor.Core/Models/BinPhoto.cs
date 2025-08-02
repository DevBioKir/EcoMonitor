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
        public double FillLevel { get; private set; }
        public bool IsOutsideBin { get; private set; }
        public string Comment { get; private set; } = string.Empty;

        public ICollection<BinPhotoBinType> BinPhotoBinTypes { get; private set; } = new List<BinPhotoBinType>();

        private BinPhoto() { }

        private void Validate()
        {
            if(string.IsNullOrWhiteSpace(FileName)) 
                throw new ArgumentException("FileName required");
            if(string.IsNullOrWhiteSpace(UrlFile)) 
                throw new ArgumentException("UrlFile required");
            if(Latitude < -90 || Latitude > 90) 
                throw new ArgumentOutOfRangeException(nameof(Latitude), "Latitude must be between -90 and 90 degrees");
            if (Longitude < -180 || Longitude > 180) 
                throw new ArgumentOutOfRangeException(nameof(Longitude), "Longitude must be between -180 and 180 degrees");
            if (FillLevel < 0.0 || FillLevel > 1.0) 
                throw new ArgumentOutOfRangeException(nameof(FillLevel), "FillLevel must be between 0.0 and 1.0");
        }

        public static BinPhoto Create(
            string fileName,
            string urlFile,
            double latitude,
            double longitude,
            IEnumerable<Guid> BinTypeId,
            double fillLevel,
            bool isOutsideBin,
            string comment)
        {
            var photo = new BinPhoto
            {
                Id = Guid.NewGuid(),
                FileName = fileName,
                UrlFile = urlFile,
                Latitude = latitude,
                Longitude = longitude,
                UploadedAt = DateTime.UtcNow,
                FillLevel = fillLevel,
                IsOutsideBin = isOutsideBin,
                Comment = comment
            };

            if (BinTypeId == null || BinTypeId == null || !BinTypeId.Any())
            {
               throw new Exception("BinTypeId required");
            }
            foreach (var id in BinTypeId)
            {
                photo.AddBinType(id);
            }

            photo.Validate();
            return photo;
        }

        public void AddBinType(Guid binTypeId)
        {
            if (!BinPhotoBinTypes.Any(x => x.BinTypeId == binTypeId) 
                && binTypeId != Guid.Empty)
            {
                BinPhotoBinTypes.Add(new BinPhotoBinType(Id, binTypeId));
            }
        }

        public void RemoveBinType(Guid binTypeId)
        {
            var link = BinPhotoBinTypes.FirstOrDefault(bbt => bbt.BinTypeId == binTypeId);
            if (link != null)
            {
                BinPhotoBinTypes.Remove(link);
            }
        }
    }
}

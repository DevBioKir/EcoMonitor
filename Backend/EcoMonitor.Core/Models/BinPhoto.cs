using EcoMonitor.Core.Models.Users;
using NetTopologySuite.Geometries;

namespace EcoMonitor.Core.Models
{
    public class BinPhoto
    {
        public Guid Id { get; private set; }
        public string FileName { get; private set; } = string.Empty;
        public string UrlFile { get; private set; } = string.Empty;
        // PostGIS
        //public Point Location { get; private set; } = null!;
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public DateTime UploadedAt { get; private set; }
        public double FillLevel { get; private set; }
        public bool IsOutsideBin { get; private set; }
        public string Comment { get; private set; } = string.Empty;

        public ICollection<BinPhotoBinType> BinPhotoBinTypes { get; private set; } = new List<BinPhotoBinType>();
        public User UploadedBy { get; private set; } = null!;
        public Guid UploadedById { get; private set; }

        private BinPhoto() { }
        private BinPhoto(
            string fileName,
            string urlFile,
            double latitude,
            double longitude,
            IEnumerable<Guid> BinTypeId,
            double fillLevel,
            bool isOutsideBin,
            string comment,
            User uploadedBy)
        {
            if (uploadedBy == null)
                throw new ArgumentNullException(nameof(uploadedBy));

            var geometryFactory = NetTopologySuite.NtsGeometryServices.Instance
                .CreateGeometryFactory(srid: 4326);
            var point = geometryFactory
                .CreatePoint(new Coordinate(longitude, latitude));

            Id = Guid.NewGuid();
            FileName = fileName;
            UrlFile = urlFile;
            Latitude = latitude;
            Longitude = longitude;
            UploadedAt = DateTime.UtcNow;
            FillLevel = fillLevel;
            IsOutsideBin = isOutsideBin;
            Comment = comment;
            UploadedBy = uploadedBy;
            UploadedById = uploadedBy.Id;
        }

        private BinPhoto(
            Guid id,
            string fileName,
            string urlFile,
            double latitude,
            double longitude,
            DateTime uploadedAt,
            IEnumerable<Guid> BinTypeId,
            double fillLevel,
            bool isOutsideBin,
            string comment,
            User uploadedBy)
        {
            Id = id;
            FileName = fileName;
            UrlFile = urlFile;
            Latitude = latitude;
            Longitude = longitude;
            UploadedAt = uploadedAt;
            FillLevel = fillLevel;
            IsOutsideBin = isOutsideBin;
            Comment = comment;
            UploadedBy = uploadedBy;
            UploadedById = uploadedBy.Id;
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(FileName)) 
                throw new ArgumentException("FileName required");

            if (string.IsNullOrWhiteSpace(UrlFile)) 
                throw new ArgumentException("UrlFile required");

            ValidateLocation(Longitude, Latitude);

            if (FillLevel < 0.0 || FillLevel > 1.0) 
                throw new ArgumentOutOfRangeException(nameof(FillLevel), "FillLevel must be between 0.0 and 1.0");
            
            if (BinPhotoBinTypes == null || !BinPhotoBinTypes.Any())
                throw new Exception("At least one BinType is required");
        }

        private void ValidateLocation(double longitude, double latitude)
        {
            if (longitude == null)
                throw new ArgumentNullException(nameof(longitude));
            if (latitude == null)
                throw new ArgumentNullException(nameof(latitude));
            

            if (latitude < -90 || latitude > 90)
                throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90 degrees");

            if (longitude < -180 || longitude > 180)
                throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180 degrees");
        }

        public static BinPhoto Create(
            string fileName,
            string urlFile,
            double latitude,
            double longitude,
            IEnumerable<Guid> BinTypeId,
            double fillLevel,
            bool isOutsideBin,
            string comment,
            User uploadedBy)
        {
            var photo = new BinPhoto(
                fileName, 
                urlFile, 
                latitude, 
                longitude, 
                BinTypeId, 
                fillLevel, 
                isOutsideBin, 
                comment, 
                uploadedBy);

            if (BinTypeId == null || !BinTypeId.Any())
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
        
        public static BinPhoto Restore(
            Guid id,
            string fileName,
            string urlFile,
            double latitude,
            double longitude,
            DateTime uploadedAt,
            IEnumerable<Guid> BinTypeId,
            double fillLevel,
            bool isOutsideBin,
            string comment,
            User uploadedBy)
        {
            var photo = new BinPhoto(
                id, 
                fileName,
                urlFile,
                latitude,
                longitude,
                uploadedAt,
                BinTypeId,
                fillLevel,
                isOutsideBin,
                comment,
                uploadedBy);

            if (BinTypeId == null || !BinTypeId.Any())
            {
                throw new Exception("BinTypeId required");
            }

            foreach (var binTypeId in BinTypeId)
            {
                photo.AddBinType(binTypeId);
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

        public void SetUploadedBy(User uploadedBy)
        {
            UploadedBy = uploadedBy ?? throw new ArgumentNullException(nameof(uploadedBy));
        }
    }
}

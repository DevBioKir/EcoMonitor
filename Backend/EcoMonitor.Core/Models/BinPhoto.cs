using EcoMonitor.Core.Models.Users;
using NetTopologySuite.Geometries;

namespace EcoMonitor.Core.Models
{
    public class BinPhoto
    {
        public Guid Id { get; private set; }
        public string FileName { get; private set; } = string.Empty;
        public string UrlFile { get; private set; } = string.Empty;
        //public double Latitude { get; private set; }
        //public double Longitude { get; private set; }

        // PostGIS
        public Point Location { get; private set; } = null!;
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
            Location = point;
            UploadedAt = DateTime.UtcNow;
            FillLevel = fillLevel;
            IsOutsideBin = isOutsideBin;
            Comment = comment;
            UploadedBy = uploadedBy;
            UploadedById = uploadedBy.Id;

            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(FileName)) 
                throw new ArgumentException("FileName required");

            if (string.IsNullOrWhiteSpace(UrlFile)) 
                throw new ArgumentException("UrlFile required");

            ValidateLocation(Location);

            if (FillLevel < 0.0 || FillLevel > 1.0) 
                throw new ArgumentOutOfRangeException(nameof(FillLevel), "FillLevel must be between 0.0 and 1.0");
            
            if (BinPhotoBinTypes == null || !BinPhotoBinTypes.Any())
                throw new Exception("At least one BinType is required");
        }

        private void ValidateLocation(Point location)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            if (location.Y < -90 || location.Y > 90)
                throw new ArgumentOutOfRangeException(nameof(location.Y), "Latitude must be between -90 and 90 degrees");

            if (location.X < -180 || location.X > 180)
                throw new ArgumentOutOfRangeException(nameof(location.X), "Longitude must be between -180 and 180 degrees");
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

using EcoMonitor.Core.Models.Users;
using EcoMonitor.DataAccess.Entities.Users;
using NetTopologySuite.Geometries;

namespace EcoMonitor.DataAccess.Entities
{
    public class BinPhotoEntity
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string UrlFile { get; set; } = string.Empty;
        public Point Location { get; set; } = null!;
        public DateTime UploadedAt { get; set; }
        public double FillLevel { get; set; }
        public bool IsOutsideBin { get; set; } = false;
        public string Comment { get; set; } = string.Empty;

        public ICollection<BinPhotoBinTypeEntity> BinPhotoBinTypes { get; set; } = new List<BinPhotoBinTypeEntity>();
        public UserEntity UploadedBy { get; set; }
        public Guid UploadedById { get; set; }
    }
}

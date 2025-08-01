namespace EcoMonitor.DataAccess.Entities
{
    public class BinPhotoEntity
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string UrlFile { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime UploadedAt { get; set; }
        public double FillLevel { get; set; }
        public bool IsOutsideBin { get; set; } = false;
        public string Comment { get; set; } = string.Empty;

        public ICollection<BinPhotoBinTypeEntity> BinPhotoBinTypes { get; private set; } = new List<BinPhotoBinTypeEntity>();
    }
}

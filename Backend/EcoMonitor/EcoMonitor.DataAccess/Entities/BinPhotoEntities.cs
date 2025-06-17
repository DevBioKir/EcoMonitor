namespace EcoMonitor.DataAccess.Entities
{
    public class BinPhotoEntities
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string UrlFile { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime UploadedAt { get; set; }
        public string BinType { get; set; } = string.Empty;
        public double FillLevel { get; set; }
        public bool IsOutsideBin { get; set; } = false;
        public string Comment { get; set; } = string.Empty;
    }
}

namespace EcoMonitor.Core.Models
{
    public class BinPhotoBinType
    {
        public Guid BinPhotoId { get; private set; }
        public BinPhoto BinPhoto { get; private set; }

        public Guid BinTypeId { get; private set; }
        public BinType BinType { get; private set; }

        private BinPhotoBinType() {}

        public BinPhotoBinType(Guid binPhotoId, Guid binTypeId)
        {
            BinPhotoId = binPhotoId;
            BinTypeId = binTypeId;
        }
    }
}

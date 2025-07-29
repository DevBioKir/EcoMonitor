using EcoMonitor.Core.Models;

namespace EcoMonitor.DataAccess.Entities
{
    public class BinPhotoBinTypeEntity
    {
        public Guid BinPhotoId { get; set; }
        public BinPhotoEntity BinPhoto { get; set; }

        public Guid BinTypeId { get; set; }
        public BinTypeEntity BinType { get; set; }
    }
}

using EcoMonitor.Core.Models;

namespace EcoMonitor.DataAccess.Entities
{
    public class BinTypeEntity
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ICollection<BinPhotoBinTypeEntity> BinPhotoBinTypes { get; private set; } = new List<BinPhotoBinTypeEntity>();
    }
}

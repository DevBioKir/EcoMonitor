using EcoMonitor.Core.Models;
using EcoMonitor.Core.Models.Users;

namespace EcoMonitor.App.Abstractions;

public interface IBinPhotoFactory
{
    Core.Models.BinPhoto Create(
        string fileName,
        string urlFile,
        double latitude,
        double longitude,
        IEnumerable<Guid> BinTypeId,
        double fillLevel,
        bool isOutsideBin,
        string comment,
        User uploadedBy);
    
    Core.Models.BinPhoto Restore(
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
        User uploadedBy);
}
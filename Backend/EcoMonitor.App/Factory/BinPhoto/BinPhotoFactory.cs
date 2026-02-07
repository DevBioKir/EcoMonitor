using EcoMonitor.App.Abstractions;
using EcoMonitor.Core.Models.Users;

namespace EcoMonitor.App.Factory.BinPhoto;

public class BinPhotoFactory : IBinPhotoFactory
{
    public Core.Models.BinPhoto Create(
        string fileName, 
        string urlFile, 
        double latitude, 
        double longitude, 
        District district,
        IEnumerable<Guid> BinTypeId,
        double fillLevel, 
        bool isOutsideBin, 
        string comment,
        int totalBins,
        Guid uploadedById)
    {
        return Core.Models.BinPhoto.Create(
            fileName, 
            urlFile, 
            latitude, 
            longitude,
            district,
            BinTypeId, 
            fillLevel, 
            isOutsideBin, 
            comment,
            totalBins,
            uploadedById);
    }

    public Core.Models.BinPhoto Restore(
        Guid id, 
        string fileName, 
        string urlFile, 
        double latitude, 
        double longitude,
        District district,
        DateTime uploadedAt,
        IEnumerable<Guid> BinTypeId, 
        double fillLevel, 
        bool isOutsideBin, 
        string comment,
        int totalBins,
        User uploadedBy)
    {
        return Core.Models.BinPhoto.Restore(
            id, 
            fileName, 
            urlFile, 
            latitude, 
            longitude, 
            district,
            uploadedAt,
            BinTypeId, 
            fillLevel, 
            isOutsideBin, 
            comment,
            totalBins,
            uploadedBy);
    }
}


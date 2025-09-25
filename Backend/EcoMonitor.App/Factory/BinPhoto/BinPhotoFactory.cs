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
        IEnumerable<Guid> BinTypeId,
        double fillLevel, 
        bool isOutsideBin, 
        string comment, 
        User uploadedBy)
    {
        return Core.Models.BinPhoto.Create(
            fileName, 
            urlFile, 
            latitude, 
            longitude,
            BinTypeId, 
            fillLevel, 
            isOutsideBin, 
            comment, 
            uploadedBy);
    }

    public Core.Models.BinPhoto Restore(
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
        return Core.Models.BinPhoto.Restore(
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
    }
}


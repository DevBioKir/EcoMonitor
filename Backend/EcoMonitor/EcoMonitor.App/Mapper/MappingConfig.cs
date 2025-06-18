using EcoMonitor.Core.Models;
using EcoMonitor.DataAccess.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace EcoMonitor.App.Mapper
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<BinPhoto, BinPhoto>()
                .ConstructUsing(src => BinPhoto.Create(
                    src.FileName,
                    src.UrlFile,
                    src.Latitude,
                    src.Longitude,
                    src.BinType,
                    src.FillLevel,
                    src.IsOutsideBin,
                    src.Comment))
                .Map(dest => dest.Id, src => src.Id);

            config.NewConfig<BinPhoto, BinPhotoEntities>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.FileName, src => src.FileName)
                .Map(dest => dest.UrlFile, src => src.UrlFile)
                .Map(dest => dest.Latitude, src => src.Latitude)
                .Map(dest => dest.Longitude, src => src.Longitude)
                .Map(dest => dest.UploadedAt, src => src.UploadedAt)
                .Map(dest => dest.BinType, src => src.BinType)
                .Map(dest => dest.FillLevel, src => src.FillLevel)
                .Map(dest => dest.Comment, src => src.Comment);

            config.NewConfig<BinPhotoEntities, BinPhoto>()
                .ConstructUsing(src => BinPhoto.Create(
                    src.FileName,
                    src.UrlFile,
                    src.Latitude,
                    src.Longitude,
                    src.BinType,
                    src.FillLevel,
                    src.IsOutsideBin,
                    src.Comment));
        }
    }
}

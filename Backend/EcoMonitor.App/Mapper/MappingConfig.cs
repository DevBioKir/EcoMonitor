using EcoMonitor.Contracts.Contracts;
using EcoMonitor.Core.Models;
using EcoMonitor.DataAccess.Entities;
using Mapster;

namespace EcoMonitor.App.Mapper
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Mapping Entities, Domain for BinPhoto
            config.NewConfig<BinPhoto, BinPhoto>()
                .ConstructUsing(src => BinPhoto.Create(
                    src.FileName,
                    src.UrlFile,
                    src.Latitude,
                    src.Longitude,
                    src.BinPhotoBinTypes.Select(bbt => bbt.BinTypeId),
                    src.FillLevel,
                    src.IsOutsideBin,
                    src.Comment))
                .AfterMapping((src, dest) =>
                {
                    foreach (var bbt in src.BinPhotoBinTypes)
                    {
                        dest.AddBinType(bbt.BinTypeId);
                    }
                });

            config.NewConfig<BinPhoto, BinPhotoEntity>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.FileName, src => src.FileName)
                .Map(dest => dest.UrlFile, src => src.UrlFile)
                .Map(dest => dest.Latitude, src => src.Latitude)
                .Map(dest => dest.Longitude, src => src.Longitude)
                .Map(dest => dest.UploadedAt, src => src.UploadedAt)
                .Map(dest => dest.FillLevel, src => src.FillLevel)
                .Map(dest => dest.Comment, src => src.Comment)
                .Map(dest => dest.BinPhotoBinTypes, src =>
        src.BinPhotoBinTypes.Select(bbt => bbt.Adapt<BinPhotoBinTypeEntity>()).ToList());
                //.Ignore(dest => dest.BinPhotoBinTypes);

            config.NewConfig<BinPhotoEntity, BinPhoto>()
                .ConstructUsing(src => BinPhoto.Create(
                    src.FileName,
                    src.UrlFile,
                    src.Latitude,
                    src.Longitude,
                    src.BinPhotoBinTypes.Select(bbt => bbt.BinTypeId),
                    src.FillLevel,
                    src.IsOutsideBin,
                    src.Comment))
                .Ignore(dest => dest.BinPhotoBinTypes)
                .AfterMapping((src, dest) =>
                {
                    foreach (var bbt in src.BinPhotoBinTypes)
                    {
                        dest.AddBinType(bbt.BinTypeId);
                    }
                });

            // Mapping Entities, Domain for BinType
            config.NewConfig<BinType, BinType>()
                .ConstructUsing(src => BinType.Create(
                    src.Code,
                    src.Name));

            config.NewConfig<BinType, BinTypeEntity>()
                .Map(dest => dest.Code, src => src.Code)
                .Map(dest => dest.Name, src => src.Name);

            config.NewConfig<BinTypeEntity, BinType>()
                .ConstructUsing(src => BinType.Create(
                    src.Code,
                    src.Name));

            // Mapping Entities, Domain for BinPhotoBinType
            config.NewConfig<BinPhotoBinType, BinPhotoBinType>()
                .Map(dest => dest.BinPhotoId, src => src.BinPhotoId)
                .Map(dest => dest.BinTypeId, src => src.BinTypeId);

            config.NewConfig<BinPhotoBinType, BinPhotoBinTypeEntity>()
                .Map(dest => dest.BinPhotoId, src => src.BinPhotoId)
                .Map(dest => dest.BinTypeId, src => src.BinTypeId);

            config.NewConfig<BinPhotoBinTypeEntity, BinPhotoBinType>()
                .ConstructUsing(src => new BinPhotoBinType(src.BinTypeId, src.BinPhotoId));

            // Mapping DTOs for BinPhoto
            config.NewConfig<BinPhotoRequest, BinPhoto>()
                .ConstructUsing(src => BinPhoto.Create(
                    src.FileName,
                    src.UrlFile,
                    src.Latitude,
                    src.Longitude,
                    src.BinTypeId,
                    src.FillLevel,
                    src.IsOutsideBin,
                    src.Comment));

            config.NewConfig<BinPhoto, BinPhotoRequest>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.FileName, src => src.FileName)
                .Map(dest => dest.UrlFile, src => src.UrlFile)
                .Map(dest => dest.Latitude, src => src.Latitude)
                .Map(dest => dest.Longitude, src => src.Longitude)
                .Map(dest => dest.UploadedAt, src => src.UploadedAt)
                .Map(dest => dest.BinTypeId, src => src.BinPhotoBinTypes.Select(bbt => bbt.BinTypeId).ToList())
                .Map(dest => dest.FillLevel, src => src.FillLevel)
                .Map(dest => dest.IsOutsideBin, src => src.IsOutsideBin)
                .Map(dest => dest.Comment, src => src.Comment);


            config.NewConfig<BinPhoto, BinPhotoResponse>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.FileName, src => src.FileName)
                .Map(dest => dest.UrlFile, src => src.UrlFile)
                .Map(dest => dest.Latitude, src => src.Latitude)
                .Map(dest => dest.Longitude, src => src.Longitude)
                .Map(dest => dest.UploadedAt, src => src.UploadedAt)
                .Map(dest => dest.BinTypeId, src => 
                    src.BinPhotoBinTypes.Select(bbt => bbt.BinTypeId).ToList())
                .Map(dest => dest.FillLevel, src => src.FillLevel)
                .Map(dest => dest.IsOutsideBin, src => src.IsOutsideBin)
                .Map(dest => dest.Comment, src => src.Comment);

            // Mapping DTOs for BinType
            config.NewConfig<BinTypeRequest, BinType>()
                .ConstructUsing(src => BinType.Create(
                    src.Code,
                    src.Name));

            config.NewConfig<BinType, BinTypeResponse>()
                .Map(dest => dest.Code, src => src.Code)
                .Map(dest => dest.Name, src => src.Name);

            // Mapping DTOs for BinPhotoBinType
            config.NewConfig<BinPhotoBinTypeRequest, BinPhotoBinType>()
                .Map(dest => dest.BinPhotoId, src => src.BinPhotoId)
                .Map(dest => dest.BinTypeId, src => src.BinTypeId);

            config.NewConfig<BinPhotoBinType, BinPhotoBinTypeResponse>()
                .Map(dest => dest.BinPhotoId, src => src.BinPhotoId)
                .Map(dest => dest.BinTypeId, src => src.BinTypeId);


            config.NewConfig<BinPhoto, BinPhotoWithTypesResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.FileName, src => src.FileName)
            .Map(dest => dest.UrlFile, src => src.UrlFile)
            .Map(dest => dest.Latitude, src => src.Latitude)
            .Map(dest => dest.Longitude, src => src.Longitude)
            .Map(dest => dest.UploadedAt, src => src.UploadedAt)
            .Map(dest => dest.FillLevel, src => src.FillLevel)
            .Map(dest => dest.IsOutsideBin, src => src.IsOutsideBin)
            .Map(dest => dest.Comment, src => src.Comment)
            .Map(dest => dest.BinTypes, src => src.BinPhotoBinTypes.Select(b => b.BinType.Adapt<BinTypeResponse>()).ToList());
        }
    }
}

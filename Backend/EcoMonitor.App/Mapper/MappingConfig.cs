using EcoMonitor.App.Abstractions;
using EcoMonitor.Contracts.Contracts;
using EcoMonitor.Contracts.Contracts.BinPhoto;
using EcoMonitor.Contracts.Contracts.BinPhotoBinType;
using EcoMonitor.Contracts.Contracts.BinType;
using EcoMonitor.Contracts.Contracts.User;
using EcoMonitor.Contracts.Contracts.Users;
using EcoMonitor.Core.Models;
using EcoMonitor.Core.Models.Users;
using EcoMonitor.Core.ValueObjects;
using EcoMonitor.DataAccess.Entities;
using EcoMonitor.DataAccess.Entities.Users;
using Mapster;

namespace EcoMonitor.App.Mapper
{
    public class MappingConfig : IRegister
    {
        private readonly IUserFactory _userFactory;

        public MappingConfig(IUserFactory userFactory)
        {
            _userFactory = userFactory;
        }

        public void Register(TypeAdapterConfig config)
        {
            /// <summary>
            /// Mapping Entities, Domain for BinPhoto
            /// </summary>
            config.NewConfig<BinPhoto, BinPhoto>()
                .ConstructUsing(src => BinPhoto.Create(
                        src.FileName,
                        src.UrlFile,
                        src.Location.Y, // latitude
                        src.Location.X,  // longitude
                        src.BinPhotoBinTypes.Select(bbt => bbt.BinTypeId),
                        src.FillLevel,
                        src.IsOutsideBin,
                        src.Comment,
                        src.UploadedBy))
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
                .Map(dest => dest.Location, src => src.Location)
                //.Map(dest => dest.Longitude, src => src.Longitude)
                .Map(dest => dest.UploadedAt, src => src.UploadedAt)
                .Map(dest => dest.FillLevel, src => src.FillLevel)
                .Map(dest => dest.Comment, src => src.Comment)
                .Map(dest => dest.BinPhotoBinTypes, src =>
        src.BinPhotoBinTypes.Select(bbt => bbt.Adapt<BinPhotoBinTypeEntity>()).ToList())
                .Map(dest => dest.UploadedBy, src => src.UploadedBy);
                //.Ignore(dest => dest.BinPhotoBinTypes);

            config.NewConfig<BinPhotoEntity, BinPhoto>()
                .ConstructUsing(src => BinPhoto.Create(
                    src.FileName,
                    src.UrlFile,
                    src.Location.Y, // latitude
                    src.Location.X,  // longitude
                    src.BinPhotoBinTypes.Select(bbt => bbt.BinTypeId),
                    src.FillLevel,
                    src.IsOutsideBin,
                    src.Comment,
                    src.UploadedBy != null ? src.UploadedBy.Adapt<User>() : null))
                .Ignore(dest => dest.BinPhotoBinTypes)
                .AfterMapping((src, dest) =>
                {
                    foreach (var bbt in src.BinPhotoBinTypes)
                    {
                        dest.AddBinType(bbt.BinTypeId);
                    }
                });

            /// <summary>
            /// Mapping Entities, Domain for BinType
            /// </summary>
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

            /// <summary>
            /// Mapping Entities, Domain for BinPhotoBinType
            /// </summary>
            config.NewConfig<BinPhotoBinType, BinPhotoBinType>()
                .Map(dest => dest.BinPhotoId, src => src.BinPhotoId)
                .Map(dest => dest.BinTypeId, src => src.BinTypeId);

            config.NewConfig<BinPhotoBinType, BinPhotoBinTypeEntity>()
                .Map(dest => dest.BinPhotoId, src => src.BinPhotoId)
                .Map(dest => dest.BinTypeId, src => src.BinTypeId);

            config.NewConfig<BinPhotoBinTypeEntity, BinPhotoBinType>()
                .ConstructUsing(src => new BinPhotoBinType(src.BinTypeId, src.BinPhotoId));

            /// <summary>
            /// Mapping Entities, Domain for User
            /// </summary>
            config.NewConfig<User, UserEntity>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Firstname, src => src.Firstname)
                .Map(dest => dest.Surname, src => src.Surname)
                .Map(dest => dest.Email, src => src.Email.Value)          // VO → string
                .Map(dest => dest.PasswordHash, src => src.PasswordHash.Hash) // VO → string
                .Map(dest => dest.isLoginConfirmed, src => src.isLoginConfirmed)
                .Map(dest => dest.RoleId, src => src.RoleId)
                .Map(dest => dest.Role, src => src.Role.Adapt<UserRoleEntity>())
                .Map(dest => dest.CreatedAt, src => src.CreatedAt)
                .Map(dest => dest.LastLogindAt, src => src.LastLogindAt)
                .Map(dest => dest.LockedUntil, src => src.LockedUntil)
                .Map(dest => dest.BinPhoto, src => src.Photos.Adapt<List<BinPhotoEntity>>());

            config.NewConfig<UserEntity, User>()
                .ConstructUsing(src => _userFactory.Restore(
                    src.Id,
                    src.Firstname,
                    src.Surname,
                    src.Email,
                    src.PasswordHash,
                    src.Role.Adapt<UserRole>(),
                    src.CreatedAt,
                    src.LastLogindAt,
                    src.LockedUntil,
                    src.BinPhoto.Adapt<List<BinPhoto>>()
                ));

            /// <summary>
            /// Mapping Entities, Domain for UserRole
            /// </summary>
            config.NewConfig<UserRoleEntity, UserRole>()
                .Map(dest => dest, src => UserRole.Create(
                    src.Name,
                    src.Description,
                    src.Permissions.Select(p => new Permission(p.Code))
                    ));

            config.NewConfig<UserRole, UserRoleEntity>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.Permissions, src => src.Permissions.Select(p => new PermissionEntity { Code = p.Code }));

            /// <summary>
            /// Mapping Entities, Domain for Permission
            /// </summary>
            config.NewConfig<PermissionEntity, Permission>()
                .ConstructUsing(src => new Permission(src.Code));


            /// <summary>
            /// Mapping DTOs for User
            /// </summary>
            config.NewConfig<UserRequest, User>()
                .ConstructUsing(src => _userFactory.Create(
                    src.Firstname,
                    src.Surname,
                    src.Email,
                    src.Password
                ));

            config.NewConfig<RegisterUserRequest, User>()
                .ConstructUsing(src => _userFactory.Create(
                    src.Firstname,
                    src.Surname,
                    src.Email,
                    src.Password
                ));

            config.NewConfig<User, UserRequest>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Firstname, src => src.Firstname)
                .Map(dest => dest.Surname, src => src.Surname)
                .Map(dest => dest.Email, src => src.Email.Value)          // VO → string
                .Map(dest => dest.Password, src => src.PasswordHash.Hash) // VO → string
                .Map(dest => dest.isLoginConfirmed, src => src.isLoginConfirmed)
                .Map(dest => dest.RoleId, src => src.RoleId)
                .Map(dest => dest.Role, src => src.Role.Adapt<UserRoleEntity>())
                .Map(dest => dest.CreatedAt, src => src.CreatedAt)
                .Map(dest => dest.LastLogindAt, src => src.LastLogindAt)
                .Map(dest => dest.LockedUntil, src => src.LockedUntil)
                .Map(dest => dest.BinPhoto, src => src.Photos.Adapt<List<BinPhotoEntity>>());

            /// <summary>
            /// Mapping DTOs for BinPhoto
            /// </summary>
            config.NewConfig<BinPhotoRequest, BinPhoto>()
                .ConstructUsing(src => BinPhoto.Create(
                    src.FileName,
                    src.UrlFile,
                    src.Location.Y, // latitude
                    src.Location.X,  // longitude
                    src.BinTypeId,
                    src.FillLevel,
                    src.IsOutsideBin,
                    src.Comment,
                    src.UploadedBy != null ? src.UploadedBy.Adapt<User>() : null));

            config.NewConfig<BinPhoto, BinPhotoRequest>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.FileName, src => src.FileName)
                .Map(dest => dest.UrlFile, src => src.UrlFile)
                .Map(dest => dest.Location, src => src.Location)
                //.Map(dest => dest.Longitude, src => src.Longitude)
                .Map(dest => dest.UploadedAt, src => src.UploadedAt)
                .Map(dest => dest.BinTypeId, src => src.BinPhotoBinTypes.Select(bbt => bbt.BinTypeId).ToList())
                .Map(dest => dest.FillLevel, src => src.FillLevel)
                .Map(dest => dest.IsOutsideBin, src => src.IsOutsideBin)
                .Map(dest => dest.Comment, src => src.Comment)
                .Map(dest => dest.UploadedBy, src => src.UploadedBy);


            config.NewConfig<BinPhoto, BinPhotoResponse>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.FileName, src => src.FileName)
                .Map(dest => dest.UrlFile, src => src.UrlFile)
                .Map(dest => dest.Location, src => src.Location)
                //.Map(dest => dest.Longitude, src => src.Longitude)
                .Map(dest => dest.UploadedAt, src => src.UploadedAt)
                .Map(dest => dest.BinTypeId, src =>
                    src.BinPhotoBinTypes.Select(bbt => bbt.BinTypeId).ToList())
                .Map(dest => dest.FillLevel, src => src.FillLevel)
                .Map(dest => dest.IsOutsideBin, src => src.IsOutsideBin)
                .Map(dest => dest.Comment, src => src.Comment)
                .Map(dest => dest.UploadedBy, src => src.UploadedBy);

            /// <summary>
            /// Mapping DTOs for BinType
            /// </summary>
            config.NewConfig<BinTypeRequest, BinType>()
                .ConstructUsing(src => BinType.Create(
                    src.Code,
                    src.Name));

            config.NewConfig<BinType, BinTypeResponse>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Code, src => src.Code)
                .Map(dest => dest.Name, src => src.Name);

            /// <summary>
            /// Mapping DTOs for BinPhotoBinType
            /// </summary>
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
            .Map(dest => dest.Location, src => src.Location)
            //.Map(dest => dest.Latitude, src => src.Latitude)
            //.Map(dest => dest.Longitude, src => src.Longitude)
            .Map(dest => dest.UploadedAt, src => src.UploadedAt)
            .Map(dest => dest.FillLevel, src => src.FillLevel)
            .Map(dest => dest.IsOutsideBin, src => src.IsOutsideBin)
            .Map(dest => dest.Comment, src => src.Comment)
            .Map(dest => dest.BinTypes, src => src.BinPhotoBinTypes.Select(b => b.BinType.Adapt<BinTypeResponse>()).ToList());
        }
    }
}

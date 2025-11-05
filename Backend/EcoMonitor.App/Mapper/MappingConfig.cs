using EcoMonitor.App.Abstractions;
using EcoMonitor.App.Factory.Users;
using EcoMonitor.Contracts.Contracts;
using EcoMonitor.Contracts.Contracts.BinPhoto;
using EcoMonitor.Contracts.Contracts.BinPhotoBinType;
using EcoMonitor.Contracts.Contracts.BinType;
using EcoMonitor.Contracts.Contracts.User;
using EcoMonitor.Contracts.Contracts.Users;
using EcoMonitor.Contracts.Models;
using EcoMonitor.Core.Models;
using EcoMonitor.Core.Models.Auth;
using EcoMonitor.Core.Models.Users;
using EcoMonitor.Core.ValueObjects;
using EcoMonitor.DataAccess.Entities;
using EcoMonitor.DataAccess.Entities.Auth;
using EcoMonitor.DataAccess.Entities.Users;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace EcoMonitor.App.Mapper
{
    public class MappingConfig : IRegister
    {
        private readonly IUserFactory _userFactory;
        private readonly IUserRoleFactory _userRoleFactory;
        private readonly ILogger<MappingConfig> _logger;
        
        public MappingConfig(
            IUserFactory userFactory, 
            IUserRoleFactory userRoleFactory,
            ILogger<MappingConfig> logger)
        {
            _userFactory = userFactory;
            _userRoleFactory = userRoleFactory;
            _logger = logger;
        }
        
        private static IEnumerable<Guid> EnsureBinTypeId(BinPhotoEntity src)
        {
            if (src.BinPhotoBinTypes == null || !src.BinPhotoBinTypes.Any())
                throw new Exception("BinPhotoEntity.Id=" + src.Id + " has no BinTypeId");
            return src.BinPhotoBinTypes.Select(bbt => bbt.BinTypeId);
        }

        public void Register(TypeAdapterConfig config)
        {
            _logger.LogInformation("MappingConfig.Register called - Mapster configuration starting");
            /// <summary>
            /// Mapping VO, string for Email
            /// </summary>
            config.NewConfig<Email, string>()
                .MapWith(src => src.Value);

            config.NewConfig<string, Email>()
                .MapWith(src => Email.Create(src));

            /// <summary>
            /// Mapping VO, string for PasswordHash
            /// </summary>
            config.NewConfig<PasswordHash, string>()
                .MapWith(src => src.Hash);

            config.NewConfig<string, PasswordHash>()
                .MapWith(src => PasswordHash.FromHash(src));
  
            /// <summary>
            /// Mapping Entities, Domain for BinPhoto
            /// </summary>
            // config.NewConfig<BinPhoto, BinPhoto>()
            //     .ConstructUsing(src => BinPhoto.Create(
            //             src.FileName,
            //             src.UrlFile,
            //             src.Longitude,  // longitude
            //             src.Latitude, // latitude
            //             src.BinPhotoBinTypes.Select(bbt => bbt.BinTypeId),
            //             src.FillLevel,
            //             src.IsOutsideBin,
            //             src.Comment,
            //             src.TotalBins,
            //             src.UploadedBy))
            //     .AfterMapping((src, dest) =>
            //     {
            //         foreach (var bbt in src.BinPhotoBinTypes)
            //         {
            //             dest.AddBinType(bbt.BinTypeId);
            //         }
            //     });

            config.NewConfig<Point, Point>()
                .MapWith(src => src == null ? null : new Point(new Coordinate(src.X, src.Y)) { SRID = 4326 });

            config.NewConfig<BinPhoto, BinPhotoEntity>()
                .PreserveReference(true) // одна ссылка - один объект
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.FileName, src => src.FileName)
                .Map(dest => dest.UrlFile, src => src.UrlFile)
                .Map(dest => dest.Location, src => new Point(src.Longitude, src.Latitude) {SRID = 4326})
                .Map(dest => dest.UploadedAt, src => src.UploadedAt)
                .Map(dest => dest.FillLevel, src => src.FillLevel)
                .Map(dest => dest.Comment, src => src.Comment)
                .Map(dest => dest.BinPhotoBinTypes, src =>
                src.BinPhotoBinTypes.Select(bbt => bbt.Adapt<BinPhotoBinTypeEntity>()).ToList())
                .Map(dest => dest.UploadedBy, src => src.UploadedBy);
                //.Ignore(dest => dest.BinPhotoBinTypes);

                config.NewConfig<BinPhotoEntity, BinPhoto>()
                    .MapWith(src =>
                        BinPhoto.Restore(
                            src.Id,
                            src.FileName,
                            src.UrlFile,
                            src.Location.Y,
                            src.Location.X,
                            src.UploadedAt,
                            EnsureBinTypeId(src),
                            src.FillLevel,
                            src.IsOutsideBin,
                            src.Comment,
                            src.TotalBins,
                            _userFactory.Restore(
                                src.UploadedBy.Id,
                                src.UploadedBy.Firstname,
                                src.UploadedBy.Surname,
                                Email.Create(src.UploadedBy.Email),
                                PasswordHash.FromHash(src.UploadedBy.PasswordHash),
                                UserRole.Restore(
                                    src.UploadedBy.Role.Id,
                                    src.UploadedBy.Role.Name,
                                    src.UploadedBy.Role.Description,
                                    src.UploadedBy.Role.Permissions.Select(p => new Permission(p.Code)).ToList()
                                ),
                                src.UploadedBy.CreatedAt,
                                src.UploadedBy.LastLogindAt,
                                src.UploadedBy.LockedUntil,
                                new List<BinPhoto>()
                            )
                        )
                    );

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
                .PreserveReference(true) // одна ссылка - один объект
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Firstname, src => src.Firstname)
                .Map(dest => dest.Surname, src => src.Surname)
                .Map(dest => dest.Email, src => src.Email.Value) // VO → string
                .Map(dest => dest.PasswordHash, src => src.PasswordHash.Hash) // VO → string
                .Map(dest => dest.isLoginConfirmed, src => src.isLoginConfirmed)
                .Map(dest => dest.RoleId, src => src.RoleId)
                // .Map(dest => dest.Role, src => new UserRoleEntity
                //     {
                //         Id = src.Role.Id,
                //         Name = src.Role.Name,
                //         Description = src.Role.Description,
                //         Permissions = src.Role.Permissions.Select(p => new PermissionEntity { Code = p.Code }).ToList()
                //     })
                .Map(dest => dest.CreatedAt, src => src.CreatedAt)
                .Map(dest => dest.LastLogindAt, src => src.LastLogindAt)
                .Map(dest => dest.LockedUntil, src => src.LockedUntil)
                .Map(dest => dest.BinPhoto, src => src.Photos.Adapt<List<BinPhotoEntity>>())
                .Ignore(dest => dest.Role);

            config.NewConfig<UserEntity, User>()
                .ConstructUsing(src => _userFactory.Restore(
                    src.Id,
                    src.Firstname,
                    src.Surname,
                    Email.Create(src.Email),
                    PasswordHash.FromHash(src.PasswordHash),
                    _userRoleFactory.Restore(
                        src.Role.Id,
                        src.Role.Name,
                        src.Role.Description,
                        src.Role.Permissions.Select(p => new Permission(p.Code)).ToList()),
                    src.CreatedAt,
                    src.LastLogindAt,
                    src.LockedUntil,
                    src.BinPhoto.Select(bp => bp.Adapt<BinPhoto>()).ToList() ?? new List<BinPhoto>()
                ));

            /// <summary>
            /// Mapping Entities, Domain for UserRole
            /// </summary>
            config.NewConfig<UserRoleEntity, UserRole>()
                .ConstructUsing(src => UserRole.Restore(
                    src.Id,
                    src.Name,
                    src.Description,
                    src.Permissions.Select(p => new Permission(p.Code)).ToList()
                ));

            config.NewConfig<UserRole, UserRoleEntity>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.Permissions, src => src.Permissions.Select(p => new PermissionEntity { Code = p.Code }).ToList());

            /// <summary>
            /// Mapping Entities, Domain for Permission
            /// </summary>
            config.NewConfig<PermissionEntity, Permission>()
                .ConstructUsing(src => new Permission(src.Code));

            config.NewConfig<Permission, PermissionEntity>()
                .Map(dest => dest.Code, src => src.Code);
            
            /// <summary>
            /// Mapping Entities, Domain for RefreshToken
            /// </summary>
            config.NewConfig<RefreshTokenEntity, RefreshToken>()
                .ConstructUsing(src => RefreshToken.Restore(
                    src.Id,
                    src.UserId,
                    src.User != null
                        ? _userFactory.Restore(
                            src.User.Id,
                            src.User.Firstname,
                            src.User.Surname,
                            Email.Create(src.User.Email),
                            PasswordHash.FromHash(src.User.PasswordHash),
                            src.User.Role != null
                                ? UserRole.Restore(
                                    src.User.Role.Id,
                                    src.User.Role.Name,
                                    src.User.Role.Description,
                                    src.User.Role.Permissions.Select(p => new Permission(p.Code)).ToList() ?? new List<Permission>())
                                : null,
                            src.User.CreatedAt,
                            src.User.LastLogindAt,
                            src.User.LockedUntil,
                            new List<BinPhoto>())
                        : null,
                    src.TokenHash,
                    src.IssuedAt,
                    src.ExpireAt,
                    src.Revoked));

            config.NewConfig<RefreshToken, RefreshTokenEntity>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.UserId, src => src.UserId)
                .Map(dest => dest.User, src => src.User)
                .Map(dest => dest.TokenHash, src => src.TokenHash)
                .Map(dest => dest.IssuedAt, src => src.IssuedAt)
                .Map(dest => dest.ExpireAt, src => src.ExpireAt)
                .Map(dest => dest.Revoked, src => src.Revoked);

            /// <summary>
            /// Mapping DTOs for User
            /// </summary>
            config.NewConfig<UserRequest, User>()
                .ConstructUsing(src => _userFactory.Restore(
                    src.Id,
                    src.Firstname,
                    src.Surname,
                    Email.Create(src.Email),
                    PasswordHash.FromHash(src.Password),
                    _userRoleFactory.Restore(
                        src.Role.Id,
                        src.Role.Name,
                        src.Role.Description,
                        src.Role.Permissions.Select(p => new Permission(p.Code)).ToList()),
                    src.CreatedAt,
                    src.LastLogindAt,
                    src.LockedUntil,
                    src.BinPhoto.Select(bp => bp.Adapt<BinPhoto>()).ToList()));

            // config.NewConfig<RegisterUserRequest, User>()
            //     .ConstructUsing(src => _userFactory.Create(
            //         src.Firstname,
            //         src.Surname,
            //         src.Email,
            //         src.Password,
            //         _userRoleFactory.Restore(
            //             s)
            //     ));

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
            
            config.NewConfig<User, UserResponse>()
                .Map(dest => dest.Firstname, src => src.Firstname)
                .Map(dest => dest.Surname, src => src.Surname)
                .Map(dest => dest.Email, src => src.Email.Value)          // VO → string
                .Map(dest => dest.BinPhoto, src => src.Photos.Select(p => p.Adapt<BinPhotoResponse>()).ToList());

            config.NewConfig<User, RegisterUserRequest>()
                .Map(dest => dest.Firstname, src => src.Firstname)
                .Map(dest => dest.Surname, src => src.Surname)
                .Map(dest => dest.Email, src => src.Email.Value)
                .Map(dest => dest.Password, src => src.PasswordHash.Hash);

            /// <summary>
            /// Mapping DTOs for BinPhoto
            /// </summary>
            // config.NewConfig<BinPhotoRequest, BinPhoto>()
            //     .ConstructUsing(src => BinPhoto.Create(
            //         src.FileName,
            //         src.UrlFile,
            //         src.Latitude, // latitude
            //         src.Longitude,  // longitude
            //         src.BinTypeId,
            //         src.FillLevel,
            //         src.IsOutsideBin,
            //         src.Comment,
            //         src.UploadedBy != null ? src.UploadedBy.Adapt<User>() : null));

            config.NewConfig<BinPhoto, BinPhotoRequest>()
                //.Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.FileName, src => src.FileName)
                .Map(dest => dest.UrlFile, src => src.UrlFile)
                .Map(dest => dest.Longitude, src => src.Longitude)
                .Map(dest => dest.Latitude, src => src.Latitude)
                .Map(dest => dest.UploadedAt, src => src.UploadedAt)
                .Map(dest => dest.BinTypeId, src => src.BinPhotoBinTypes.Select(bbt => bbt.BinTypeId).ToList())
                .Map(dest => dest.FillLevel, src => src.FillLevel)
                .Map(dest => dest.IsOutsideBin, src => src.IsOutsideBin)
                .Map(dest => dest.Comment, src => src.Comment)
                .Map(dest => dest.UploadedById, src => src.UploadedBy.Id);
            
            config.NewConfig<BinPhoto, BinPhotoResponse>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.FileName, src => src.FileName)
                .Map(dest => dest.UrlFile, src => src.UrlFile)
                .Map(dest => dest.Longitude, src => src.Longitude)
                .Map(dest => dest.Latitude, src => src.Latitude)
                .Map(dest => dest.UploadedAt, src => src.UploadedAt)
                .Map(dest => dest.BinTypeId, src =>
                    src.BinPhotoBinTypes.Select(bbt => bbt.BinTypeId).ToList())
                .Map(dest => dest.FillLevel, src => src.FillLevel)
                .Map(dest => dest.IsOutsideBin, src => src.IsOutsideBin)
                .Map(dest => dest.Comment, src => src.Comment)
                //.Map(dest => dest.UploadedBy, src => src.UploadedBy)
                .Map(dest => dest.UploadedById, src => src.UploadedById);

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
            .Map(dest => dest.Longitude, src => src.Longitude)
            .Map(dest => dest.Latitude, src => src.Latitude)
            .Map(dest => dest.UploadedAt, src => src.UploadedAt)
            .Map(dest => dest.FillLevel, src => src.FillLevel)
            .Map(dest => dest.IsOutsideBin, src => src.IsOutsideBin)
            .Map(dest => dest.Comment, src => src.Comment)
            .Map(dest => dest.BinTypes, src 
                => src.BinPhotoBinTypes.Select(b => b.BinType.Adapt<BinTypeResponse>()).ToList());

            config.NewConfig<PhotoFilterDTO, PhotoQuery>()
                .Map(dest => dest.Page, src => src.Page)
                .Map(dest => dest.PageSize, src => src.PageSize)
                .Map(dest => dest.SortBy, src => src.SortBy)
                .Map(dest => dest.OnlyOutsideBin, src => src.OnlyOutsideBin)
                .Map(dest => dest.MinFillLevel, src => src.MinFillLevel)
                .Map(dest => dest.MaxFillLevel, src => src.MaxFillLevel)
                .Map(dest => dest.FromDate, src => src.FromDate)
                .Map(dest => dest.ToDate, src => src.ToDate);
        }
    }
}

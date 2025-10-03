using EcoMonitor.App.Services;
using EcoMonitor.Contracts.Contracts.BinPhotoUpload;
using EcoMonitor.Contracts.Contracts.BinPhoto;
using EcoMonitor.Core.Models;
using EcoMonitor.DataAccess.Entities;
using EcoMonitor.DataAccess.Repositories;
using EcoMonitor.Infrastracture.Pipeline;
using EcoMonitor.Infrastracture.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EcoMonitor.Contracts.Contracts.User;
using EcoMonitor.Core.Models.Users;
using EcoMonitor.DataAccess.Entities.Users;

namespace EcoMonitor.UnitTest.Services
{
    public class BinPhotoServiceTests : TestBase
    {
        [Fact]
        public async Task GetAllBinPhotos_ReturnsAllBinPhotoResponse()
        {
            // Arrange
            var plasticId = Guid.NewGuid();
            var organicId = Guid.NewGuid();

            var roleEntity = new UserRoleEntity
            {
                Id = UserRole.User.Id,
                Name = UserRole.User.Name,
                Description = UserRole.User.Description,
                Permissions = UserRole.User.Permissions
                    .Select(p => new PermissionEntity { Code = p.Code })
                    .ToList()
            };

            await _context.UserRoles.AddAsync(roleEntity);
            await _context.SaveChangesAsync();

            var userEntity = _mapper.Map<UserEntity>(_user);
            userEntity.RoleId = roleEntity.Id;
            userEntity.Role = roleEntity;

            await _context.Users.AddAsync(userEntity);
            await _context.SaveChangesAsync();
            
            var binPhotos = new List<BinPhoto>()
            {
                BinPhoto.Create(
                "Бак на Калинина.jpg",
                "C:\\EcoMonitor\\EcoMonitor\\Backend\\EcoMonitor\\EcoMonitor.API\\wwwroot\\BinPhotos",
                57.55,
                38.41,
                new List<Guid> { plasticId },
                0.7,
                true,
                "Test photo",
                _user
                ),
                BinPhoto.Create(
                "Бак на Кирова.jpg",
                "C:\\EcoMonitor\\EcoMonitor\\Backend\\EcoMonitor\\EcoMonitor.API\\wwwroot\\BinPhotos",
                55.75,
                37.61,
                new List<Guid> { plasticId, organicId },
                0.8,
                true,
                "Test photo",
                _user
                )
            };

            var binPhotosEntity = binPhotos.Select(bp =>
            {
                var entity = _mapper.Map<BinPhotoEntity>(bp);
                entity.UploadedById = userEntity.Id;
                entity.UploadedBy = userEntity;
                return entity;
            }).ToList();

            await _context.BinPhotos.AddRangeAsync(binPhotosEntity);
            await _context.SaveChangesAsync();

            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            Directory.CreateDirectory(webRootPath);

            var imageStorageService = new ImageStorageService(webRootPath);
            var geoLocationService = new GeolocationService();

            var binPhotoRepo = new BinPhotoRepository(_context, _mapper);
            var loggerService = _serviceProvider.GetRequiredService<ILogger<BinPhotoService>>();
            var loggerPipeline = _serviceProvider.GetRequiredService<ILogger<ImagePipeline>>();

            var pipeline = new ImagePipeline(imageStorageService, geoLocationService, loggerPipeline);

            var binPhotoService = new BinPhotoService(_mapper, binPhotoRepo, loggerService, pipeline, _userRepository);

            // Act
            var result = await binPhotoService.GetAllBinPhotosAsync();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, bp => bp.FileName == "Бак на Калинина.jpg");
            Assert.Contains(result, bp => bp.FileName == "Бак на Кирова.jpg");
            //Assert.Contains(result, bp => bp.UploadedById = _user.Id);
        }

        [Fact]
        public async Task GetPhotoByIdAsync_ReturnsMappedBinPhoto_WhenPhotoExists()
        {
            // Arrange
            var plasticId = Guid.NewGuid();
            var organicId = Guid.NewGuid();

            var roleEntity = new UserRoleEntity
            {
                Id = UserRole.User.Id,
                Name = UserRole.User.Name,
                Description = UserRole.User.Description,
                Permissions = UserRole.User.Permissions
                    .Select(p => new PermissionEntity { Code = p.Code })
                    .ToList()
            };

            await _context.UserRoles.AddAsync(roleEntity);
            await _context.SaveChangesAsync();

            var userEntity = _mapper.Map<UserEntity>(_user);
            userEntity.RoleId = roleEntity.Id;
            userEntity.Role = roleEntity;

            await _context.Users.AddAsync(userEntity);
            await _context.SaveChangesAsync();
            
            var binPhoto = BinPhoto.Create(
                "Бак на Кирова.jpg",
                "C:\\EcoMonitor\\EcoMonitor\\Backend\\EcoMonitor\\EcoMonitor.API\\wwwroot\\BinPhotos",
                55.75,
                37.61,
                new List<Guid> { plasticId, organicId },
                0.8,
                true,
                "Test photo",
                _user
                );

            var binPhotoEntity = _mapper.Map<BinPhotoEntity>(binPhoto);
            
            binPhotoEntity.UploadedById = userEntity.Id;
            binPhotoEntity.UploadedBy = userEntity;

            await _context.BinPhotos.AddAsync(binPhotoEntity);
            await _context.SaveChangesAsync();

            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            Directory.CreateDirectory(webRootPath);

            var imageStorageService = new ImageStorageService(webRootPath);
            var geoLocationService = new GeolocationService();

            var binPhotoRepo = new BinPhotoRepository(_context, _mapper);
            var loggerService = _serviceProvider.GetRequiredService<ILogger<BinPhotoService>>();
            var loggerPipeline = _serviceProvider.GetRequiredService<ILogger<ImagePipeline>>();

            var pipeline = new ImagePipeline(imageStorageService, geoLocationService, loggerPipeline);

            var binPhotoService = new BinPhotoService(_mapper, binPhotoRepo, loggerService, pipeline, _userRepository);

            var binPhotoRequest = _mapper.Map<BinPhotoRequest>(binPhotoEntity);

            // Act
            var result = await binPhotoService.GetPhotoByIdAsync(binPhotoRequest.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(binPhoto.Id, result.Id);
            Assert.Equal("Бак на Кирова.jpg", result.FileName);
            Assert.Equal(
                "C:\\EcoMonitor\\EcoMonitor\\Backend\\EcoMonitor\\EcoMonitor.API\\wwwroot\\BinPhotos",
                result.UrlFile);
            Assert.Equal(55.75, result.Latitude);
            Assert.Equal(37.61, result.Longitude);
            //Assert.Equal("Plastic", result.BinType);
            Assert.Equal(0.8, result.FillLevel);
            Assert.Equal(true, result.IsOutsideBin);
            Assert.Equal("Test photo", result.Comment);
        }

        [Fact]
        public async Task AddBinPhotoAsync_ReturnsTheAddedBinPhotoResponse()
        {
            // Arrange
            var plasticId = Guid.NewGuid();
            var organicId = Guid.NewGuid();
            
            var roleEntity = new UserRoleEntity
            {
                Id = UserRole.User.Id,
                Name = UserRole.User.Name,
                Description = UserRole.User.Description,
                Permissions = UserRole.User.Permissions
                    .Select(p => new PermissionEntity { Code = p.Code })
                    .ToList()
            };

            await _context.UserRoles.AddAsync(roleEntity);
            await _context.SaveChangesAsync();

            var userEntity = _mapper.Map<UserEntity>(_user);
            userEntity.RoleId = roleEntity.Id;
            userEntity.Role = roleEntity;

            await _context.Users.AddAsync(userEntity);
            await _context.SaveChangesAsync();

            var binPhoto = BinPhoto.Create(
                "Бак на Кирова.jpg",
                "C:\\EcoMonitor\\EcoMonitor\\Backend\\EcoMonitor\\EcoMonitor.API\\wwwroot\\BinPhotos",
                55.75,
                37.61,
                new List<Guid> { plasticId, organicId },
                0.8,
                true,
                "Test photo",
                _user
                );
            
            var binPhotoEntity = _mapper.Map<BinPhotoEntity>(binPhoto);
            
            binPhotoEntity.UploadedById = userEntity.Id;
            binPhotoEntity.UploadedBy = userEntity;

            await _context.BinPhotos.AddAsync(binPhotoEntity);
            await _context.SaveChangesAsync();

            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            Directory.CreateDirectory(webRootPath);

            var imageStorageService = new ImageStorageService(webRootPath);
            var geoLocationService = new GeolocationService();

            var binPhotoRepo = new BinPhotoRepository(_context, _mapper);
            var loggerService = _serviceProvider.GetRequiredService<ILogger<BinPhotoService>>();
            var loggerPipeline = _serviceProvider.GetRequiredService<ILogger<ImagePipeline>>();

            var pipeline = new ImagePipeline(imageStorageService, geoLocationService, loggerPipeline);

            var binPhotoService = new BinPhotoService(_mapper, binPhotoRepo, loggerService, pipeline, _userRepository);

            var binPhotoRequest = _mapper.Map<BinPhotoRequest>(binPhoto);

            // Act
            var result = await binPhotoService.AddBinPhotoAsync(binPhotoRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(binPhoto.Id, result.Id);
            Assert.Equal("Бак на Кирова.jpg", result.FileName);
            Assert.Equal(
                "C:\\EcoMonitor\\EcoMonitor\\Backend\\EcoMonitor\\EcoMonitor.API\\wwwroot\\BinPhotos",
                result.UrlFile);
            Assert.Equal(55.75, result.Latitude);
            Assert.Equal(37.61, result.Longitude);
            //Assert.Equal("Plastic", result.BinType);
            Assert.Equal(0.8, result.FillLevel);
            Assert.Equal(true, result.IsOutsideBin);
            Assert.Equal("Test photo", result.Comment);
        }
        
        [Theory]
        [InlineData("20250630_201412.jpg", "image/jpg")]
        [InlineData("Home.png", "image/png")]
        [InlineData("IMG_6453.HEIC", "image/heic")]
        public async Task UploadImage_ShouldReturnCorrectResponse(string fileName, string contentType)
        {
            var imagePath = Path.Combine("TestPhotos", fileName);
            var imagesBytes = await File.ReadAllBytesAsync(imagePath);
            var stream = new MemoryStream(imagesBytes);

            var formFile = new FormFile(stream, 0, stream.Length, "image", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };

            var plasticId = Guid.NewGuid();
            var organicId = Guid.NewGuid();
            
            var userRequest = _mapper.Map<UserRequest>(_user);

            var request = new BinPhotoUploadRequest(
                Photo: formFile,
                BinTypeId: new List<Guid> { plasticId, organicId },
                FillLevel: 0.6,
                IsOutsideBin: true,
                Comment: "Бак на кирова",
                userRequest,
                userRequest.Id);

            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            Directory.CreateDirectory(webRootPath);

            var imageStorageService = new ImageStorageService(webRootPath);
            var geoLocationService = new GeolocationService();

            var binPhotoRepo = new BinPhotoRepository(_context, _mapper);
            var loggerService = _serviceProvider.GetRequiredService<ILogger<BinPhotoService>>();
            var loggerPipeline = _serviceProvider.GetRequiredService<ILogger<ImagePipeline>>();

            var pipeline = new ImagePipeline(imageStorageService, geoLocationService, loggerPipeline);

            var binPhotoService = new BinPhotoService(_mapper, binPhotoRepo, loggerService, pipeline, _userRepository);
            var ct = new CancellationToken();

            // Act
            var result = await binPhotoService.UploadImage(request, ct);

            // Assert
            Assert.NotNull(result);
            //Assert.Equal("Plastic", result.BinType);
            Assert.Equal("Бак на кирова", result.Comment);

            var saved = await _context.BinPhotos.FirstOrDefaultAsync(x => x.Id == result.Id);
            Assert.NotNull(saved);
            Assert.Equal(0.6, saved.FillLevel);
        }
    }
}

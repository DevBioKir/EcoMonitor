using EcoMonitor.Core.Models;
using EcoMonitor.Core.Models.Users;
using EcoMonitor.Core.ValueObjects;
using EcoMonitor.DataAccess.Entities;
using EcoMonitor.DataAccess.Entities.Users;
using EcoMonitor.DataAccess.Repositories;

namespace EcoMonitor.UnitTest.Repositories
{
    public class BinPhotoRepositoryTests : TestBase
    {
        [Fact]
        public async Task GetAllBinPhotos_ReturnsAllBinPhotos()
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

            var binPhotoRepo = new BinPhotoRepository(_context, _mapper);

            // Act
            var photos = await binPhotoRepo.GetAllBinPhotosAsync();

            // Assert
            Assert.NotNull(photos);
            Assert.Equal(2, photos.Count());
            Assert.Contains(photos, bp => bp.FileName == "Бак на Калинина.jpg");
            Assert.Contains(photos, bp => bp.FileName == "Бак на Кирова.jpg");

            foreach (var photo in photos)
            {
                Assert.NotNull(photo.UploadedBy);
                Assert.Equal(_user.Email.Value, photo.UploadedBy.Email.Value);
                Assert.Equal(_user.Firstname, photo.UploadedBy.Firstname);
                Assert.Equal(_user.Surname, photo.UploadedBy.Surname);
                Assert.Equal(_user.Role.Name, photo.UploadedBy.Role.Name);
            }
        }

        [Fact]
        public async Task GetPhotoByIdAsync_ReturnsMappedBinPhoto_WhenPhotoExists()
        {
            // Arrange
            var plasticId = Guid.NewGuid();
            
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
                "Бак на Калинина.jpg",
                "C:\\EcoMonitor\\EcoMonitor\\Backend\\EcoMonitor\\EcoMonitor.API\\wwwroot\\BinPhotos",
                57.55,
                38.41,
                new List<Guid> { plasticId },
                0.7,
                true,
                "Test photo",
                _user
                );

            var binPhotoEntity = _mapper.Map<BinPhotoEntity>(binPhoto);
            
            binPhotoEntity.UploadedById = userEntity.Id;
            binPhotoEntity.UploadedBy = userEntity;

            await _context.BinPhotos.AddAsync(binPhotoEntity);
            await _context.SaveChangesAsync();

            var binPhotoRepo = new BinPhotoRepository(_context, _mapper);


            // Act
            var photo = await binPhotoRepo.GetPhotoByIdAsync(binPhotoEntity.Id);

            // Assert
            Assert.NotNull(photo);
            Assert.Equal(binPhoto.Id, photo.Id);
            Assert.Equal("Бак на Калинина.jpg", photo.FileName);
            Assert.Equal(
                "C:\\EcoMonitor\\EcoMonitor\\Backend\\EcoMonitor\\EcoMonitor.API\\wwwroot\\BinPhotos",
                photo.UrlFile);
            Assert.Equal(57.55, photo.Latitude);
            Assert.Equal(38.41, photo.Longitude);
            //Assert.Equal("Plastic", result.BinType);
            Assert.Equal(0.7, photo.FillLevel);
            Assert.Equal(true, photo.IsOutsideBin);
            Assert.Equal("Test photo", photo.Comment);

            Assert.NotNull(photo.UploadedBy);
            Assert.Equal(_user.Id, photo.UploadedBy.Id);
            Assert.Equal(_user.Email.Value, photo.UploadedBy.Email.Value);
            Assert.Equal(_user.Firstname, photo.UploadedBy.Firstname);
            Assert.Equal(_user.Surname, photo.UploadedBy.Surname);
            Assert.Equal(_user.Role.Name, photo.UploadedBy.Role.Name);
        }

        [Fact]
        public async Task AddBinPhotoAsync_ReturnsTheAddedBinPhoto()
        {
            // Arrange
            var plasticId = Guid.NewGuid();
            var organicId = Guid.NewGuid();

            var binPhoto = BinPhoto.Create(
                "Бак на Калинина.jpg",
                "C:\\EcoMonitor\\EcoMonitor\\Backend\\EcoMonitor\\EcoMonitor.API\\wwwroot\\BinPhotos",
                57.55,
                38.41,
                new List<Guid> { plasticId },
                0.7,
                true,
                "Test photo",
                _user
                );

            var binPhotoRepo = new BinPhotoRepository(_context, _mapper);

            // Act
            var result = await binPhotoRepo.AddBinPhotoAsync(binPhoto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(binPhoto.Id, result.Id);
            Assert.Equal("Бак на Калинина.jpg", result.FileName);
            Assert.Equal(
                "C:\\EcoMonitor\\EcoMonitor\\Backend\\EcoMonitor\\EcoMonitor.API\\wwwroot\\BinPhotos",
                result.UrlFile);
            Assert.Equal(57.55, result.Latitude);
            Assert.Equal(38.41, result.Longitude);
            //Assert.Equal("Plastic", result.BinType);
            Assert.Equal(0.7, result.FillLevel);
            Assert.Equal(true, result.IsOutsideBin);
            Assert.Equal("Test photo", result.Comment);
        }

        [Fact]
        public async Task DeleteBinPhotoAsync_ReturnsIdDeletedPhoto()
        {
            // Arrange
            var plasticId = Guid.NewGuid();
            var organicId = Guid.NewGuid();

            var binPhoto = BinPhoto.Create(
                "Бак на Калинина.jpg",
                "C:\\EcoMonitor\\EcoMonitor\\Backend\\EcoMonitor\\EcoMonitor.API\\wwwroot\\BinPhotos",
                57.55,
                38.41,
                new List<Guid> { plasticId },
                0.7,
                true,
                "Test photo",
                _user
                );

            var binPhotoEntity = _mapper.Map<BinPhotoEntity>(binPhoto);

            await _context.AddAsync(binPhotoEntity);
            await _context.SaveChangesAsync();

            var binPhotoRepo = new BinPhotoRepository(_context, _mapper);

            // Act
            var result = await binPhotoRepo.DeleteBinPhotoAsync(binPhotoEntity.Id);

            // Assert
            var deletedBinPhoto = await _context.BinPhotos.FindAsync(binPhotoEntity.Id);


            Assert.Null(deletedBinPhoto);
            Assert.Equal(binPhotoEntity.Id, result);
        }

        [Fact]
        public async Task DeleteBinPhotoAsync_WhenPhotoNotFound_ThrowsException()
        {
            // Arrange
            var binPhotoRepo = new BinPhotoRepository(_context, _mapper);
            var nonExistentId = Guid.NewGuid();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NullReferenceException>(async () =>
            {
                await binPhotoRepo.DeleteBinPhotoAsync(nonExistentId);
            });

            Assert.Contains($"Container photo with ID", exception.Message);
        }
    }
}

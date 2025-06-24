using EcoMonitor.Core.Models;
using EcoMonitor.DataAccess.Entities;
using EcoMonitor.DataAccess.Repositories;

namespace EcoMonitor.UnitTest.Repositories
{
    public class EcoMonitorRepositoryTests : TestBase
    {
        [Fact]
        public async Task GetAllBinPhotos_ReturnsAllBinPhotos()
        {
            // Arrage
            var binPhotos = new List<BinPhoto>()
            {
                BinPhoto.Create(
                "Бак на Калинина.jpg",
                "C:\\EcoMonitor\\EcoMonitor\\Backend\\EcoMonitor\\EcoMonitor.API\\wwwroot\\BinPhotos",
                57.55,
                38.41,
                "Plastic",
                0.7,
                true,
                "Test photo"
                ),
                BinPhoto.Create(
                "Бак на Кирова.jpg",
                "C:\\EcoMonitor\\EcoMonitor\\Backend\\EcoMonitor\\EcoMonitor.API\\wwwroot\\BinPhotos",
                55.75,
                37.61,
                "Plastic",
                0.8,
                true,
                "Test photo"
                )
            };

            var binPhotosEntity = _mapper.Map<List<BinPhotoEntity>>(binPhotos);

            await _context.BinPhotos.AddRangeAsync(binPhotosEntity);
            await _context.SaveChangesAsync();

            var binPhotoRepo = new BinPhotoRepository(_context, _mapper);

            // Act
            var result = await binPhotoRepo.GetAllBinPhotosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, bp => bp.FileName == "Бак на Калинина.jpg");
            Assert.Contains(result, bp => bp.FileName == "Бак на Кирова.jpg");
        }

        [Fact]
        public async Task GetPhotoByIdAsync_ReturnsMappedBinPhoto_WhenPhotoExists()
        {
            // Arrage
            var binPhoto = BinPhoto.Create(
                "Бак на Кирова.jpg",
                "C:\\EcoMonitor\\EcoMonitor\\Backend\\EcoMonitor\\EcoMonitor.API\\wwwroot\\BinPhotos",
                55.75,
                37.61,
                "Plastic",
                0.8,
                true,
                "Test photo"
                );

            var binPhotoEntity = _mapper.Map<BinPhotoEntity>(binPhoto);

            await _context.BinPhotos.AddAsync(binPhotoEntity);
            await _context.SaveChangesAsync();

            var binPhotoRepo = new BinPhotoRepository(_context, _mapper);

            // Act
            var result = await binPhotoRepo.GetPhotoByIdAsync(binPhoto.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(binPhoto.Id, result.Id);
            Assert.Equal("Бак на Кирова.jpg", result.FileName);
            Assert.Equal(
                "C:\\EcoMonitor\\EcoMonitor\\Backend\\EcoMonitor\\EcoMonitor.API\\wwwroot\\BinPhotos",
                result.UrlFile);
            Assert.Equal(55.75, result.Latitude);
            Assert.Equal(37.61, result.Longitude);
            Assert.Equal("Plastic", result.BinType);
            Assert.Equal(0.8, result.FillLevel);
            Assert.Equal(true, result.IsOutsideBin);
            Assert.Equal("Test photo", result.Comment);
        }

        [Fact]
        public async Task AddBinPhotoAsync_ReturnsTheAddedBinPhoto()
        {
            // Arrage
            var binPhoto = BinPhoto.Create(
                "Бак на Кирова.jpg",
                "C:\\EcoMonitor\\EcoMonitor\\Backend\\EcoMonitor\\EcoMonitor.API\\wwwroot\\BinPhotos",
                55.75,
                37.61,
                "Plastic",
                0.8,
                true,
                "Test photo"
                );
            
            var binPhotoRepo = new BinPhotoRepository(_context, _mapper);

            // Act
            var result = await binPhotoRepo.AddBinPhotoAsync(binPhoto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(binPhoto.Id, result.Id);
            Assert.Equal("Бак на Кирова.jpg", result.FileName);
            Assert.Equal(
                "C:\\EcoMonitor\\EcoMonitor\\Backend\\EcoMonitor\\EcoMonitor.API\\wwwroot\\BinPhotos",
                result.UrlFile);
            Assert.Equal(55.75, result.Latitude);
            Assert.Equal(37.61, result.Longitude);
            Assert.Equal("Plastic", result.BinType);
            Assert.Equal(0.8, result.FillLevel);
            Assert.Equal(true, result.IsOutsideBin);
            Assert.Equal("Test photo", result.Comment);
        }

        [Fact]
        public async Task DeleteBinPhotoAsync_ReturnsIdDeletedPhoto()
        {
            // Arrage
            var binPhoto = BinPhoto.Create(
                "Бак на Кирова.jpg",
                "C:\\EcoMonitor\\EcoMonitor\\Backend\\EcoMonitor\\EcoMonitor.API\\wwwroot\\BinPhotos",
                55.75,
                37.61,
                "Plastic",
                0.8,
                true,
                "Test photo"
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

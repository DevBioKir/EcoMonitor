using EcoMonitor.App.Services;
using EcoMonitor.Contracts.Contracts;
using EcoMonitor.Core.Models;
using EcoMonitor.DataAccess.Entities;
using EcoMonitor.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EcoMonitor.UnitTest.Services
{
    public class EcoMonitorServiceTests : TestBase
    {
        [Fact]
        public async Task GetAllBinPhotos_ReturnsAllBinPhotoResponse()
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
            var logger = _serviceProvider.GetRequiredService<ILogger<BinPhotoService>>();

            var binPhotoService = new BinPhotoService(_mapper, binPhotoRepo, logger);

            //var binPhotosRequest = _mapper.Map<BinPhotoRequest>(binPhotos);

            // Act
            var result = await binPhotoService.GetAllBinPhotosAsync();

            //Assert
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
            var logger = _serviceProvider.GetRequiredService<ILogger<BinPhotoService>>();

            var binPhotoService = new BinPhotoService(_mapper, binPhotoRepo, logger);

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
            Assert.Equal("Plastic", result.BinType);
            Assert.Equal(0.8, result.FillLevel);
            Assert.Equal(true, result.IsOutsideBin);
            Assert.Equal("Test photo", result.Comment);
        }

        [Fact]
        public async Task AddBinPhotoAsync_ReturnsTheAddedBinPhotoResponse()
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
            var logger = _serviceProvider.GetRequiredService<ILogger<BinPhotoService>>();
            var binPhotoService = new BinPhotoService(_mapper, binPhotoRepo, logger);

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
            Assert.Equal("Plastic", result.BinType);
            Assert.Equal(0.8, result.FillLevel);
            Assert.Equal(true, result.IsOutsideBin);
            Assert.Equal("Test photo", result.Comment);
        }
    }
}

﻿using EcoMonitor.App.Services;
using EcoMonitor.Contracts.Contracts;
using EcoMonitor.Core.Models;
using EcoMonitor.DataAccess.Entities;
using EcoMonitor.DataAccess.Repositories;
using EcoMonitor.Infrastracture.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

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

            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            Directory.CreateDirectory(webRootPath);

            var imageStorageService = new ImageStorageService(webRootPath);
            var geoLocationService = new GeolocationService();

            var binPhotoRepo = new BinPhotoRepository(_context, _mapper);
            var logger = _serviceProvider.GetRequiredService<ILogger<BinPhotoService>>();

            var binPhotoService = new BinPhotoService(_mapper, binPhotoRepo, logger, imageStorageService, geoLocationService);

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

            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            Directory.CreateDirectory(webRootPath);

            var imageStorageService = new ImageStorageService(webRootPath);
            var geoLocationService = new GeolocationService();

            var binPhotoRepo = new BinPhotoRepository(_context, _mapper);
            var logger = _serviceProvider.GetRequiredService<ILogger<BinPhotoService>>();

            var binPhotoService = new BinPhotoService(_mapper, binPhotoRepo, logger, imageStorageService, geoLocationService);

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

            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            Directory.CreateDirectory(webRootPath);

            var imageStorageService = new ImageStorageService(webRootPath);
            var geoLocationService = new GeolocationService();

            var binPhotoRepo = new BinPhotoRepository(_context, _mapper);
            var logger = _serviceProvider.GetRequiredService<ILogger<BinPhotoService>>();
            var binPhotoService = new BinPhotoService(_mapper, binPhotoRepo, logger, imageStorageService, geoLocationService);

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

        [Fact]
        public async Task UploadImage_ShouldReturnCorrectResponse()
        {
            // Arrage
            var fileName = "20250630_201412.jpg";
            var imagePath = Path.Combine("TestPhotos", "20250630_201412.jpg");
            var imagesBytes = await File.ReadAllBytesAsync(imagePath);
            var stream = new MemoryStream(imagesBytes);

            var formFile = new FormFile(stream, 0, stream.Length, "image", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpg"
            };

            var request = new BinPhotoUploadRequest(
                Photo: formFile,
                BinType: "Plastic",
                FillLevel: 0.6,
                IsOutsideBin: true,
                Comment: "Бак на кирова");

            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            Directory.CreateDirectory(webRootPath);

            var imageStorageService = new ImageStorageService(webRootPath);
            var geoLocationService = new GeolocationService();

            var binPhotoRepo = new BinPhotoRepository(_context, _mapper);
            var logger = _serviceProvider.GetRequiredService<ILogger<BinPhotoService>>();
            var binPhotoService = new BinPhotoService(_mapper, binPhotoRepo, logger, imageStorageService, geoLocationService);

            // Act
            var result = await binPhotoService.UploadImage(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Plastic", result.BinType);
            Assert.Equal("Бак на кирова", result.Comment);

            var saved = await _context.BinPhotos.FirstOrDefaultAsync(x => x.Id == result.Id);
            Assert.NotNull(saved);
            Assert.Equal(0.6, saved.FillLevel);
        }
    }
}

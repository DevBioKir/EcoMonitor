using EcoMonitor.App.Services;
using EcoMonitor.Core.Models;
using EcoMonitor.DataAccess.Entities;
using EcoMonitor.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EcoMonitor.UnitTest.Services
{
    public class BinTypeServiceTests : TestBase
    {
        [Fact]
        public async Task GetAllBinTypes_ReturnsAllBinTypesResponse()
        {
            // Arrange
            var binTypes = new List<BinType>
            {
                BinType.Create(
                    "Organic",
                    "Органика"),
                BinType.Create(
                    "Plastic",
                    "Пластик"),
                BinType.Create(
                    "Paper/Cardboard",
                    "Бумага/Картон"),
                BinType.Create(
                    "Metal",
                    "Металлы")
            };

            var binTypeEntity = _mapper.Map<IReadOnlyList<BinTypeEntity>>(binTypes);

            await _context.AddRangeAsync(binTypeEntity);
            await _context.SaveChangesAsync();

            var binTypeRepo = new BinTypeRepository(_context, _mapper);
            var logger = _serviceProvider.GetRequiredService<ILogger<BinTypeService>>();

            var binTypeService = new BinTypeService(binTypeRepo, _mapper, logger);

            // Act
            var result = await binTypeService.GetAllBinTypesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Count);
            Assert.Contains(result, bt => bt.Code == "Organic");
            Assert.Contains(result, bt => bt.Name == "Органика");
        }
    }
}

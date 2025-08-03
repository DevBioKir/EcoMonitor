using EcoMonitor.Core.Models;
using EcoMonitor.DataAccess.Entities;
using EcoMonitor.DataAccess.Repositories;

namespace EcoMonitor.UnitTest.Repositories
{
    public class BinTypeRepositoryTests : TestBase
    {
        [Fact]
        public async Task GetAllBinTypes_ReturnsAllBinTypes()
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

            var binTypeEntities = _mapper.Map<IEnumerable<BinTypeEntity>>(binTypes);

            await _context.BinTypes.AddRangeAsync(binTypeEntities);
            await _context.SaveChangesAsync();

            var binTypeRepo = new BinTypeRepository(_context, _mapper);

            // Act
            var result = await binTypeRepo.GetAllBinTypesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Count());
            Assert.Contains(result, bt => bt.Name == "Органика");
            Assert.Contains(result, bt => bt.Code == "Organic");
        }

        [Fact]
        public async Task GetBinTypeByIdAsync_ReturnsMappedBinType_WhenTypeExists()
        {
            // Arrange 
            var binType = BinType.Create(
                    "Organic",
                    "Органика");

            var binTypeEntity = _mapper.Map<BinTypeEntity>(binType);

            await _context.BinTypes.AddAsync(binTypeEntity);
            await _context.SaveChangesAsync();

            var binTypeRepo = new BinTypeRepository(_context, _mapper);

            // Act
            var result = await binTypeRepo.GetBinTypeByIdAsync(binTypeEntity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(binTypeEntity.Id, result.Id);
            Assert.Equal("Organic", result.Code);
            Assert.Equal("Органика", result.Name);
        }

        [Fact]
        public async Task AddBinTypeAsync_ReturnsTheAddedBinType()
        {
            var binType = BinType.Create(
                    "Organic",
                    "Органика");

            var binTypeRepo = new BinTypeRepository(_context, _mapper);

            // Act 
            var result = await binTypeRepo.AddBinTypeAsync(binType);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(binType.Id, result.Id);
            Assert.Equal("Organic", result.Code);
            Assert.Equal("Органика", result.Name);
        }

        [Fact]
        public async Task DeleteBinTypeAsync_ReturnsIdDeletedType()
        {
            // Arrange
            var binType = BinType.Create(
                    "Organic",
                    "Органика");

            var binTypeEntity = _mapper.Map<BinTypeEntity>(binType);

            await _context.AddAsync(binTypeEntity);
            await _context.SaveChangesAsync();

            var binTypeRepo = new BinTypeRepository(_context, _mapper);

            // Act
            var result = await binTypeRepo.DeleteBinTypeAsync(binTypeEntity.Id);

            // Assert
            var deleteBinType = await _context.BinTypes.FindAsync(binTypeEntity.Id);

            Assert.Null(deleteBinType);
            Assert.Equal(binTypeEntity.Id, result);
        }
    }
}

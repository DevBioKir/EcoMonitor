using EcoMonitor.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoMonitor.UnitTest
{
    public class EcoMonitorRepositoryTests
    {
        [Fact]
        public async Task GetPhotoByIdAsync_ReturnsMappedBinPhoto_WhenPhotoExists()
        {
            //Arrage
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





            var photoId = Guid.NewGuid();

            var binPhotoEntity = new EcoMonitor.DataAccess.Entities.BinPhotoEntities
            {
                Id = photoId,
                FileName = "test_photo.jpg",
                UrlFile = "C:\\EcoMonitor\\EcoMonitor\\Backend\\EcoMonitor\\EcoMonitor.API\\wwwroot\\BinPhotos",
                Latitude = 55.75,
                Longitude = 37.61,
                BinType = "Plastic",
                FillLevel = 0.8,
                IsOutsideBin = true,
                Comment = "Test photo"
            };
        }
    }
}

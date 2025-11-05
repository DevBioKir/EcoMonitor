using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoMonitor.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddBinPhotoIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Составной индекс для базового запроса + сортировки по дате
            migrationBuilder.CreateIndex(
                name: "IX_BinPhotos_UploadedById_UploadedAt",
                table: "BinPhotos",
                columns: new[] { "UploadedById", "UploadedAt" });

            // Индекс для фильтрации по уровню заполненности
            migrationBuilder.CreateIndex(
                name: "IX_BinPhotos_UploadedById_FillLevel",
                table: "BinPhotos",
                columns: new[] { "UploadedById", "FillLevel" });

            // Частичный индекс для фильтрации "вне контейнера"
            migrationBuilder.Sql(@"CREATE INDEX IX_BinPhotos_UploadedById_IsOutsideBin 
                ON ""BinPhotos"" (""UploadedById"", ""UploadedAt"" DESC) 
                WHERE ""IsOutsideBin"" = true");

            // Индекс для сортировки по количеству контейнеров
            migrationBuilder.CreateIndex(
                name: "IX_BinPhotos_UploadedById_TotalBins",
                table: "BinPhotos",
                columns: new[] { "UploadedById", "TotalBins", "UploadedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BinPhotos_UploadedById_UploadedAt",
                table: "BinPhotos");

            migrationBuilder.DropIndex(
                name: "IX_BinPhotos_UploadedById_FillLevel",
                table: "BinPhotos");

            migrationBuilder.DropIndex(
                name: "IX_BinPhotos_UploadedById_TotalBins",
                table: "BinPhotos");

            migrationBuilder.Sql(@"DROP INDEX IF EXISTS IX_BinPhotos_UploadedById_IsOutsideBin");
        }
    }
}

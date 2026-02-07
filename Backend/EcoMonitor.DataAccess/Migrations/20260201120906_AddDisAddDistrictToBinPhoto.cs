using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoMonitor.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddDisAddDistrictToBinPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "District",
                table: "BinPhotos",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "District",
                table: "BinPhotos");
        }
    }
}

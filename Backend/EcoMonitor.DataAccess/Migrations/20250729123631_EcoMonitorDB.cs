using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoMonitor.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EcoMonitorDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BinPhotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UrlFile = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BinType = table.Column<string>(type: "text", nullable: false),
                    FillLevel = table.Column<double>(type: "double precision", nullable: false),
                    IsOutsideBin = table.Column<bool>(type: "boolean", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BinPhotos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "binTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_binTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BinPhotoBinTypeEntity",
                columns: table => new
                {
                    BinPhotoId = table.Column<Guid>(type: "uuid", nullable: false),
                    BinTypeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BinPhotoBinTypeEntity", x => new { x.BinPhotoId, x.BinTypeId });
                    table.ForeignKey(
                        name: "FK_BinPhotoBinTypeEntity_BinPhotos_BinPhotoId",
                        column: x => x.BinPhotoId,
                        principalTable: "BinPhotos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BinPhotoBinTypeEntity_binTypes_BinTypeId",
                        column: x => x.BinTypeId,
                        principalTable: "binTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BinPhotoBinTypeEntity_BinTypeId",
                table: "BinPhotoBinTypeEntity",
                column: "BinTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BinPhotoBinTypeEntity");

            migrationBuilder.DropTable(
                name: "BinPhotos");

            migrationBuilder.DropTable(
                name: "binTypes");
        }
    }
}

using EcoMonitor.Core.Models.Users;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoMonitor.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminAccount : Migration
    {
        private readonly Guid adminUserId =  Guid.Parse("bb105bce-e45e-4be6-9255-0745353d6cbb");
        private readonly Guid adminRoleId = RoleConstants.AdminId;
        private const string email = "ecomonitor.support@mail.com";
        // this password
        private const string passwordHash = "04w9E4JPJjmwmVAIhfGWYqH8jly8MpS2gYtwhXymGbl7fgIkWMjX9SQB35pEtiII";
        
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[]
                {
                    "Id", 
                    "Firstname", 
                    "Surname", 
                    "Email", 
                    "PasswordHash",
                    "RoleId", 
                    "isLoginConfirmed", 
                    "CreatedAt", 
                    "LastLogindAt", 
                    "LockedUntil"
                },
                values: new object[]
                {
                    adminUserId, 
                    "Admin", 
                    "User", 
                    email, 
                    passwordHash, 
                    adminRoleId, 
                    true, 
                    DateTime.UtcNow, 
                    DateTime.UtcNow, 
                    DateTime.MinValue,
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("Users", "Id", adminUserId);
        }
    }
}

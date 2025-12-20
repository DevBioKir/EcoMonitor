using EcoMonitor.Core.Models.Users;
using EcoMonitor.Core.ValueObjects;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoMonitor.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedPermissionsAndRoleBindings : Migration
    {
        private static readonly Guid AdminRoleId = RoleConstants.AdminId;
        private static readonly Guid UserRoleId = RoleConstants.UserId;
        private static readonly Guid ManagerRoleId = RoleConstants.ManagerId;
        
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PermissionEntity",
                columns: new[] { "Id", "Code" },
                values: new object[,]
                {
                    { PermissionConstants.UsersViewId, "Users.View" },
                    { PermissionConstants.UsersAddId, "Users.Add" },
                    { PermissionConstants.UsersEditId, "Users.Edit" },
                    { PermissionConstants.UsersBlockId, "Users.Block" },
                    { PermissionConstants.UsersUnblockId, "Users.Unblock" },
                    
                    { PermissionConstants.RolesManageId, "Roles.Manage" },
                    
                    { PermissionConstants.PhotosViewId, "Photos.View" },
                    { PermissionConstants.PhotosAddId, "Photos.Add" },
                    { PermissionConstants.PhotosEditId, "Photos.Edit" },
                    { PermissionConstants.PhotosDeleteId, "Photos.Delete" }
                });

            // Admin
            migrationBuilder.InsertData(
                table: "PermissionEntityUserRoleEntity",
                columns: new[] { "PermissionsId", "RolesId" },
                values: new object[,]
                {
                    { PermissionConstants.UsersViewId, AdminRoleId },
                    { PermissionConstants.UsersAddId, AdminRoleId },
                    { PermissionConstants.UsersEditId, AdminRoleId },
                    { PermissionConstants.UsersBlockId, AdminRoleId },
                    { PermissionConstants.UsersUnblockId, AdminRoleId },
                    
                    { PermissionConstants.RolesManageId, AdminRoleId },
                    
                    { PermissionConstants.PhotosViewId, AdminRoleId },
                    { PermissionConstants.PhotosAddId, AdminRoleId },
                    { PermissionConstants.PhotosEditId, AdminRoleId },
                    { PermissionConstants.PhotosDeleteId, AdminRoleId }
                });
            
            // Manager
            migrationBuilder.InsertData(
                table: "PermissionEntityUserRoleEntity",
                columns: new[] { "PermissionsId", "RolesId" },
                values: new object[,]
                {
                    { PermissionConstants.RolesManageId, ManagerRoleId },
                    
                    { PermissionConstants.PhotosViewId, ManagerRoleId },
                    { PermissionConstants.PhotosAddId, ManagerRoleId },
                    { PermissionConstants.PhotosEditId, ManagerRoleId },
                    { PermissionConstants.PhotosDeleteId, ManagerRoleId }
                });
            
            // User
            migrationBuilder.InsertData(
                table: "PermissionEntityUserRoleEntity",
                columns: new[] { "PermissionsId", "RolesId" },
                values: new object[,]
                {
                    { PermissionConstants.PhotosViewId, UserRoleId },
                    { PermissionConstants.PhotosAddId, UserRoleId },
                    { PermissionConstants.PhotosEditId, UserRoleId },
                    { PermissionConstants.PhotosDeleteId, UserRoleId }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            Guid[,] rolePerms =
            {
                { PermissionConstants.UsersViewId, AdminRoleId },
                { PermissionConstants.UsersAddId, AdminRoleId },
                { PermissionConstants.UsersEditId, AdminRoleId },
                { PermissionConstants.UsersBlockId, AdminRoleId },
                { PermissionConstants.UsersUnblockId, AdminRoleId },
                
                { PermissionConstants.RolesManageId, AdminRoleId },
                
                { PermissionConstants.PhotosViewId, AdminRoleId },
                { PermissionConstants.PhotosAddId, AdminRoleId },
                { PermissionConstants.PhotosEditId, AdminRoleId },
                { PermissionConstants.PhotosDeleteId, AdminRoleId },
                

                { PermissionConstants.UsersViewId, ManagerRoleId },
                
                { PermissionConstants.PhotosViewId, ManagerRoleId },
                { PermissionConstants.PhotosAddId, ManagerRoleId },
                { PermissionConstants.PhotosEditId, ManagerRoleId },
                { PermissionConstants.PhotosDeleteId, ManagerRoleId },

                { PermissionConstants.PhotosViewId, UserRoleId },
                { PermissionConstants.PhotosAddId, UserRoleId },
                { PermissionConstants.PhotosEditId, UserRoleId },
                { PermissionConstants.PhotosDeleteId, UserRoleId }
                
            };

            for (int i = 0; i < rolePerms.GetLength(0); i++)
            {
                migrationBuilder.DeleteData(
                    table: "PermissionEntityUserRoleEntity",
                    keyColumns: new[] { "PermissionsId", "RolesId" },
                    keyValues: new object[]
                    {
                        rolePerms[i,0], rolePerms[i,1]
                    });
            }
            
            Guid[] perms =
            {
                PermissionConstants.UsersViewId, 
                PermissionConstants.UsersAddId, 
                PermissionConstants.UsersEditId, 
                PermissionConstants.UsersBlockId, 
                PermissionConstants.UsersUnblockId,
                
                PermissionConstants.RolesManageId,
                
                PermissionConstants.PhotosViewId, 
                PermissionConstants.PhotosAddId, 
                PermissionConstants.PhotosEditId, 
                PermissionConstants.PhotosDeleteId
            };

            foreach (var id in perms)
            {
                migrationBuilder.DeleteData(
                    table: "PermissionEntity",
                    keyColumn: "Id",
                    keyValue: id);
            }
        }
    }
}

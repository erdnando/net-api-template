using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace netapi_template.Migrations
{
    /// <inheritdoc />
    public partial class InitModuleFields_Clean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserPermissions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UserPermissions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UserPermissions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "UserPermissions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "UserPermissions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.AddColumn<bool>(
                name: "AdminOnly",
                table: "Modules",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Modules",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Modules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Modules",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Catalogs",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 7, 5, 16, 52, 44, 613, DateTimeKind.Utc).AddTicks(8));

            migrationBuilder.UpdateData(
                table: "Catalogs",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 7, 5, 16, 52, 44, 613, DateTimeKind.Utc).AddTicks(1345));

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AdminOnly", "Icon", "Order", "Path" },
                values: new object[] { false, "HomeIcon", 1, "/" });

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AdminOnly", "Icon", "Order", "Path" },
                values: new object[] { false, "AssignmentIcon", 2, "/tasks" });

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AdminOnly", "Icon", "Order", "Path" },
                values: new object[] { false, "PeopleIcon", 3, "/users" });

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AdminOnly", "Code", "Description", "Icon", "Name", "Order", "Path" },
                values: new object[] { false, "ROLES", "Role management", "SecurityIcon", "Roles", 4, "/roles" });

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "AdminOnly", "Code", "Description", "Icon", "Name", "Order", "Path" },
                values: new object[] { false, "CATALOGS", "Catalog management", "CategoryIcon", "Catalogs", 5, "/catalogs" });

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "AdminOnly", "Code", "CreatedAt", "Description", "Icon", "IsActive", "IsDeleted", "Name", "Order", "Path", "UpdatedAt" },
                values: new object[,]
                {
                    { 6, false, "PERMISSIONS", new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission management", "AssignmentIcon", true, false, "Permisos", 6, "/permissions", null },
                    { 7, true, "ADMIN_UTILS", new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin utilities", "SecurityIcon", true, false, "Admin Utilities", 7, "/admin/utils", null }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$vBrtikfPT99ECMJKU9.7nefD.oNufTJQL8tToFnGgr7wJPZsuIBWi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$h0e/HNMlWYkO7RYNU6M0aeMQZjDgPEOJV6DfFmDmZhpAombPyKr4e");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DropColumn(
                name: "AdminOnly",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "Modules");

            migrationBuilder.UpdateData(
                table: "Catalogs",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 6, 29, 2, 18, 37, 34, DateTimeKind.Utc).AddTicks(3339));

            migrationBuilder.UpdateData(
                table: "Catalogs",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 6, 29, 2, 18, 37, 34, DateTimeKind.Utc).AddTicks(4862));

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Code", "Description", "Name" },
                values: new object[] { "CATALOGS", "Catalog management", "Catalogs" });

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Code", "Description", "Name" },
                values: new object[] { "PERMISSIONS", "Permission management", "Permissions" });

            migrationBuilder.InsertData(
                table: "UserPermissions",
                columns: new[] { "Id", "CreatedAt", "ModuleId", "PermissionType", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Admin", null, 1 },
                    { 2, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "Admin", null, 1 },
                    { 3, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "Admin", null, 1 },
                    { 4, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, "Admin", null, 1 },
                    { 5, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, "Admin", null, 1 }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$SHIQ59osDgT7QPItswsDz.u9LV1G0tycz2teHgbUuwSHbBaBzbxUa");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$WxMhE/zi0z8501m8vvzPfepbBfFuN1ZbiCYA3tvDWqDdytzCmFyNe");
        }
    }
}

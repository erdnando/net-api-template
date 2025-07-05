using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace netapi_template.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAdminOnlyFromModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminOnly",
                table: "Modules");

            migrationBuilder.UpdateData(
                table: "Catalogs",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 7, 5, 17, 37, 16, 232, DateTimeKind.Utc).AddTicks(5363));

            migrationBuilder.UpdateData(
                table: "Catalogs",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 7, 5, 17, 37, 16, 232, DateTimeKind.Utc).AddTicks(6287));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$sts3YE1YhypcLxRe0jhoGuBuloOrwrUo0MOsBOjMIcFOnrQ9UPIJG");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$Y2dlWM3L6WpG58AAvDLl0.RU/wLH4aYmXuexaZI7YYr33obRTeBva");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AdminOnly",
                table: "Modules",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

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
                column: "AdminOnly",
                value: false);

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 2,
                column: "AdminOnly",
                value: false);

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 3,
                column: "AdminOnly",
                value: false);

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 4,
                column: "AdminOnly",
                value: false);

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 5,
                column: "AdminOnly",
                value: false);

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 6,
                column: "AdminOnly",
                value: false);

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 7,
                column: "AdminOnly",
                value: true);

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
    }
}

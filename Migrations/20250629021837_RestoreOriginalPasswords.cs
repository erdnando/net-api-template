using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace netapi_template.Migrations
{
    /// <inheritdoc />
    public partial class RestoreOriginalPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Catalogs",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 6, 29, 1, 56, 11, 241, DateTimeKind.Utc).AddTicks(7731));

            migrationBuilder.UpdateData(
                table: "Catalogs",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 6, 29, 1, 56, 11, 241, DateTimeKind.Utc).AddTicks(8648));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$mZLzucHD8GoeIE1lAD7Wj.O5TICRXy6iH971qIjArOpk4CNMZfEqq");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$2h0KPJyDSkT5oPP/JntjUuNykfYJ5kDiqJy7Fxe6slEaee8gcPXiS");
        }
    }
}

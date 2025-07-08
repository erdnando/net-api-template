using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace netapi_template.Migrations
{
    /// <inheritdoc />
    public partial class UserPermissionSeed_Final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Catalogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Category = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Image = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Rating = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    InStock = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalogs", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Path = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Icon = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsSystemRole = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Avatar = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Token = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Completed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Priority = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ModuleId = table.Column<int>(type: "int", nullable: false),
                    PermissionType = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Catalogs",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "Image", "InStock", "Price", "Rating", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Electronics", new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "High-quality wireless headphones with noise cancellation", null, true, 299.99m, 4.5m, "Premium Headphones", new DateTime(2025, 7, 7, 22, 37, 58, 327, DateTimeKind.Utc).AddTicks(9706) },
                    { 2, "Electronics", new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "High-performance laptop for gaming and professional work", null, true, 1299.99m, 4.8m, "Gaming Laptop", new DateTime(2025, 7, 7, 22, 37, 58, 328, DateTimeKind.Utc).AddTicks(1376) }
                });

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "Icon", "IsActive", "IsDeleted", "Name", "Order", "Path", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "HOME", new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Home dashboard", "HomeIcon", true, false, "Home", 1, "/", null },
                    { 2, "TASKS", new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Task management", "AssignmentIcon", true, false, "Tasks", 2, "/tasks", null },
                    { 3, "USERS", new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "User management", "PeopleIcon", true, false, "Users", 3, "/users", null },
                    { 4, "ROLES", new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Role management", "SecurityIcon", true, false, "Roles", 4, "/roles", null },
                    { 5, "CATALOGS", new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Catalog management", "CategoryIcon", true, false, "Catalogs", 5, "/catalogs", null },
                    { 6, "PERMISSIONS", new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission management", "AssignmentIcon", true, false, "Permisos", 6, "/permissions", null },
                    { 7, "ADMIN_UTILS", new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin utilities", "SecurityIcon", true, false, "Admin Utilities", 7, "/admin/utils", null }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "IsDeleted", "IsSystemRole", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Administrator role with full access", false, true, "Administrador", null },
                    { 2, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Unassigned role", false, true, "Sin asignar", null },
                    { 3, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Analyst role", false, false, "Analista", null },
                    { 4, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Reports role", false, false, "Reportes", null },
                    { 5, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Support role", false, false, "Soporte", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Avatar", "CreatedAt", "Email", "FirstName", "IsDeleted", "LastLoginAt", "LastName", "PasswordHash", "RoleId", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@sistema.com", "Admin", false, null, "sistema", "$2a$11$cV8dxl/nAg40pP1YcIaoau5s7L3WAK1kUigNNJjRCJ93JHm05e71K", 1, "Active", null },
                    { 2, null, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "erdnando@gmail.com", "Erdnando", false, null, "User", "$2a$11$F8dPpwfecyNKVAz86iaDVOYeHJQPmOpaYHbY1yrvHsYBnafWzRvBe", 3, "Active", null }
                });

            migrationBuilder.InsertData(
                table: "UserPermissions",
                columns: new[] { "Id", "CreatedAt", "ModuleId", "PermissionType", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 6, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 20, null, 2 },
                    { 7, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 20, null, 2 },
                    { 8, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 0, null, 2 },
                    { 9, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 0, null, 2 },
                    { 10, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, 20, null, 2 },
                    { 11, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, 0, null, 2 },
                    { 12, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, 0, null, 2 },
                    { 13, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 20, null, 1 },
                    { 14, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 0, null, 1 },
                    { 15, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 20, null, 1 },
                    { 16, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 20, null, 1 },
                    { 17, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, 0, null, 1 },
                    { 18, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, 20, null, 1 },
                    { 19, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, 20, null, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Modules_Code",
                table: "Modules",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_Token",
                table: "PasswordResetTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_UserId",
                table: "PasswordResetTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserId",
                table: "Tasks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_ModuleId",
                table: "UserPermissions",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_UserId_ModuleId",
                table: "UserPermissions",
                columns: new[] { "UserId", "ModuleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Catalogs");

            migrationBuilder.DropTable(
                name: "PasswordResetTokens");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "UserPermissions");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}

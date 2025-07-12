using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConnectApp.Migrations
{
    /// <inheritdoc />
    public partial class initialtables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfilePictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NrOfReports = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "FullName", "ProfilePictureUrl" },
                values: new object[,]
                {
                    { 1, "Ahmed Mahmoud", " " },
                    { 2, "Youssef Mostafa", " " }
                });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "Content", "DateCreated", "DateUpdated", "ImageUrl", "NrOfReports", "UserId" },
                values: new object[,]
                {
                    { 1, "Welcome to ConnectApp! This is your first post.", new DateTime(2025, 7, 12, 14, 34, 0, 0, DateTimeKind.Utc), new DateTime(2025, 7, 12, 14, 34, 0, 0, DateTimeKind.Utc), null, 0, 1 },
                    { 2, "ConnectApp is designed to help you connect with others.", new DateTime(2025, 7, 12, 14, 34, 0, 0, DateTimeKind.Utc), new DateTime(2025, 7, 12, 14, 34, 0, 0, DateTimeKind.Utc), null, 0, 2 },
                    { 3, "Feel free to explore and share your thoughts!", new DateTime(2025, 7, 12, 14, 34, 0, 0, DateTimeKind.Utc), new DateTime(2025, 7, 12, 14, 34, 0, 0, DateTimeKind.Utc), null, 0, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId",
                table: "Posts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConnectApp.Migrations
{
    /// <inheritdoc />
    public partial class postmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    PostId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NrOfReports = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.PostId);
                });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "PostId", "Content", "DateCreated", "DateUpdated", "ImageUrl", "NrOfReports" },
                values: new object[,]
                {
                    { 1, "Welcome to ConnectApp! This is your first post.", new DateTime(2025, 7, 12, 14, 34, 0, 0, DateTimeKind.Utc), new DateTime(2025, 7, 12, 14, 34, 0, 0, DateTimeKind.Utc), null, 0 },
                    { 2, "ConnectApp is designed to help you connect with others.", new DateTime(2025, 7, 12, 14, 34, 0, 0, DateTimeKind.Utc), new DateTime(2025, 7, 12, 14, 34, 0, 0, DateTimeKind.Utc), null, 0 },
                    { 3, "Feel free to explore and share your thoughts!", new DateTime(2025, 7, 12, 14, 34, 0, 0, DateTimeKind.Utc), new DateTime(2025, 7, 12, 14, 34, 0, 0, DateTimeKind.Utc), null, 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");
        }
    }
}

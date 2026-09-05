using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicTacToe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Board = table.Column<string>(type: "TEXT", nullable: false),
                    CurrentPlayer = table.Column<string>(type: "TEXT", nullable: false),
                    GameMode = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Winner = table.Column<string>(type: "TEXT", nullable: true),
                    WinningCells = table.Column<string>(type: "TEXT", nullable: false),
                    Moves = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scoreboards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    XWins = table.Column<int>(type: "INTEGER", nullable: false),
                    OWins = table.Column<int>(type: "INTEGER", nullable: false),
                    Draws = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scoreboards", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Scoreboards",
                columns: new[] { "Id", "Draws", "OWins", "XWins" },
                values: new object[] { 1, 0, 0, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Scoreboards");
        }
    }
}

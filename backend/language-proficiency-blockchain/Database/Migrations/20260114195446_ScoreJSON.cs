using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace language_proficiency_blockchain.Database.Migrations
{
    public partial class ScoreJSON : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop old string column
            migrationBuilder.DropColumn(
                name: "Score",
                table: "TestResults");

            // Add new jsonb column
            migrationBuilder.AddColumn<JsonDocument>(
                name: "Score",
                table: "TestResults",
                type: "jsonb",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop jsonb column
            migrationBuilder.DropColumn(
                name: "Score",
                table: "TestResults");

            // Re-add string column
            migrationBuilder.AddColumn<string>(
                name: "Score",
                table: "TestResults",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false);
        }
    }
}
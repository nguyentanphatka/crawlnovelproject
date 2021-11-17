using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrawlProjectConsole.Migrations
{
    public partial class DbStoryAddColumnLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "Story",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Link",
                table: "Story");
        }
    }
}

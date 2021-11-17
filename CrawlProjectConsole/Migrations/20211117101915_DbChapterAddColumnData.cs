using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrawlProjectConsole.Migrations
{
    public partial class DbChapterAddColumnData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Data",
                table: "Chapter",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "Chapter");
        }
    }
}

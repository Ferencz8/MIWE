using Microsoft.EntityFrameworkCore.Migrations;

namespace MIWE.Data.Migrations
{
    public partial class ResultPath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResultPath",
                table: "JobSessions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultPath",
                table: "JobSessions");
        }
    }
}

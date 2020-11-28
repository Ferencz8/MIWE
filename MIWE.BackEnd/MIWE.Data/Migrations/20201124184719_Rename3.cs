using Microsoft.EntityFrameworkCore.Migrations;

namespace MIWE.Data.Migrations
{
    public partial class Rename3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CronExpresion",
                table: "ScheduledJobs");

            migrationBuilder.AddColumn<int>(
                name: "OSType",
                table: "ScheduledJobs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OSType",
                table: "ScheduledJobs");

            migrationBuilder.AddColumn<string>(
                name: "CronExpresion",
                table: "ScheduledJobs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

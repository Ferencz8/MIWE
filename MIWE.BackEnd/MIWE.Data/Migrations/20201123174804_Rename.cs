using Microsoft.EntityFrameworkCore.Migrations;

namespace MIWE.Data.Migrations
{
    public partial class Rename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobType",
                table: "ScheduledJobs");

            migrationBuilder.AddColumn<int>(
                name: "PluginType",
                table: "ScheduledJobs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Scheduling",
                table: "JobOrchestrations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PluginType",
                table: "ScheduledJobs");

            migrationBuilder.DropColumn(
                name: "Scheduling",
                table: "JobOrchestrations");

            migrationBuilder.AddColumn<int>(
                name: "JobType",
                table: "ScheduledJobs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

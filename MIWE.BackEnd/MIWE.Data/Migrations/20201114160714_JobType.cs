using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MIWE.Data.Migrations
{
    public partial class JobType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_ScheduledJobs_ScheduledJobId",
                table: "Jobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Jobs",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CrawlPath",
                table: "ScheduledJobs");

            migrationBuilder.RenameTable(
                name: "Jobs",
                newName: "JobSessions");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_ScheduledJobId",
                table: "JobSessions",
                newName: "IX_JobSessions_ScheduledJobId");

            migrationBuilder.AddColumn<int>(
                name: "JobType",
                table: "ScheduledJobs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PluginPath",
                table: "ScheduledJobs",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobSessions",
                table: "JobSessions",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "JobOrchestrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    MainJob = table.Column<Guid>(nullable: false),
                    NextJobs = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobOrchestrations", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_JobSessions_ScheduledJobs_ScheduledJobId",
                table: "JobSessions",
                column: "ScheduledJobId",
                principalTable: "ScheduledJobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSessions_ScheduledJobs_ScheduledJobId",
                table: "JobSessions");

            migrationBuilder.DropTable(
                name: "JobOrchestrations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobSessions",
                table: "JobSessions");

            migrationBuilder.DropColumn(
                name: "JobType",
                table: "ScheduledJobs");

            migrationBuilder.DropColumn(
                name: "PluginPath",
                table: "ScheduledJobs");

            migrationBuilder.RenameTable(
                name: "JobSessions",
                newName: "Jobs");

            migrationBuilder.RenameIndex(
                name: "IX_JobSessions_ScheduledJobId",
                table: "Jobs",
                newName: "IX_Jobs_ScheduledJobId");

            migrationBuilder.AddColumn<string>(
                name: "CrawlPath",
                table: "ScheduledJobs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Jobs",
                table: "Jobs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_ScheduledJobs_ScheduledJobId",
                table: "Jobs",
                column: "ScheduledJobId",
                principalTable: "ScheduledJobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

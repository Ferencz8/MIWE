using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MIWE.Data.Migrations
{
    public partial class JobRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSessions_JobOrchestrations_JobOrchestrationId",
                table: "JobSessions");

            migrationBuilder.DropTable(
                name: "JobOrchestrations");

            migrationBuilder.DropTable(
                name: "ScheduledJobs");

            migrationBuilder.DropIndex(
                name: "IX_JobSessions_JobOrchestrationId",
                table: "JobSessions");

            migrationBuilder.DropColumn(
                name: "JobOrchestrationId",
                table: "JobSessions");

            migrationBuilder.AddColumn<Guid>(
                name: "EntityId",
                table: "JobSessions",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    DateStart = table.Column<DateTime>(nullable: false),
                    PluginPath = table.Column<string>(nullable: false),
                    OSType = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsRunning = table.Column<bool>(nullable: false),
                    PluginType = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Scheduling = table.Column<string>(nullable: true),
                    MainJob = table.Column<Guid>(nullable: false),
                    NextJobs = table.Column<string>(nullable: true),
                    IsRunning = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSchedules", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "JobSchedules");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "JobSessions");

            migrationBuilder.AddColumn<Guid>(
                name: "JobOrchestrationId",
                table: "JobSessions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "JobOrchestrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsRunning = table.Column<bool>(type: "bit", nullable: false),
                    MainJob = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NextJobs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Scheduling = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobOrchestrations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsRunning = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OSType = table.Column<int>(type: "int", nullable: false),
                    PluginPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PluginType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledJobs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobSessions_JobOrchestrationId",
                table: "JobSessions",
                column: "JobOrchestrationId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSessions_JobOrchestrations_JobOrchestrationId",
                table: "JobSessions",
                column: "JobOrchestrationId",
                principalTable: "JobOrchestrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

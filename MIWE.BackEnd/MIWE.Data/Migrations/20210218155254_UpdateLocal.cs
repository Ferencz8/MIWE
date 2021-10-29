using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MIWE.Data.Migrations
{
    public partial class UpdateLocal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobScheduleLastSessionDto",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Scheduling = table.Column<string>(nullable: true),
                    MainJob = table.Column<Guid>(nullable: false),
                    NextJobs = table.Column<string>(nullable: true),
                    IsRunning = table.Column<bool>(nullable: false),
                    LastSessionDateStart = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "JobSchedulePipelineDto",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Pipeline = table.Column<string>(nullable: true),
                    Scheduling = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "JobSessionDto",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateStart = table.Column<DateTime>(nullable: false),
                    DateEnd = table.Column<DateTime>(nullable: true),
                    Instance = table.Column<string>(nullable: true),
                    IsSuccess = table.Column<bool>(nullable: false),
                    JobPipeline = table.Column<string>(nullable: true),
                    ResultContentType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobScheduleLastSessionDto");

            migrationBuilder.DropTable(
                name: "JobSchedulePipelineDto");

            migrationBuilder.DropTable(
                name: "JobSessionDto");
        }
    }
}

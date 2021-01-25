using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MIWE.Data.Migrations
{
    public partial class RemoveTbls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobScheduleLastSessionDto");

            migrationBuilder.DropTable(
                name: "JobSchedulePipelineDto");

            migrationBuilder.DropTable(
                name: "JobSessionDto");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobScheduleLastSessionDto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsRunning = table.Column<bool>(type: "bit", nullable: false),
                    LastSessionDateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MainJob = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NextJobs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Scheduling = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "JobSchedulePipelineDto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Pipeline = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Scheduling = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "JobSessionDto",
                columns: table => new
                {
                    DateEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Instance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    JobPipeline = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultContentType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });
        }
    }
}

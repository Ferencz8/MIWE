using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MIWE.Data.Migrations
{
    public partial class JobOrchestration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSessions_ScheduledJobs_ScheduledJobId",
                table: "JobSessions");

            migrationBuilder.DropIndex(
                name: "IX_JobSessions_ScheduledJobId",
                table: "JobSessions");

            migrationBuilder.DropColumn(
                name: "ScheduledJobId",
                table: "JobSessions");

            migrationBuilder.AddColumn<Guid>(
                name: "JobOrchestrationId",
                table: "JobSessions",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSessions_JobOrchestrations_JobOrchestrationId",
                table: "JobSessions");

            migrationBuilder.DropIndex(
                name: "IX_JobSessions_JobOrchestrationId",
                table: "JobSessions");

            migrationBuilder.DropColumn(
                name: "JobOrchestrationId",
                table: "JobSessions");

            migrationBuilder.AddColumn<Guid>(
                name: "ScheduledJobId",
                table: "JobSessions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_JobSessions_ScheduledJobId",
                table: "JobSessions",
                column: "ScheduledJobId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSessions_ScheduledJobs_ScheduledJobId",
                table: "JobSessions",
                column: "ScheduledJobId",
                principalTable: "ScheduledJobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

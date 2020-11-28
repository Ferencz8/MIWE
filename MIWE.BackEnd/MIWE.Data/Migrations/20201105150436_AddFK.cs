using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MIWE.Data.Migrations
{
    public partial class AddFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ScheduledJobId",
                table: "Jobs",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_ScheduledJobId",
                table: "Jobs",
                column: "ScheduledJobId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_ScheduledJobs_ScheduledJobId",
                table: "Jobs",
                column: "ScheduledJobId",
                principalTable: "ScheduledJobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_ScheduledJobs_ScheduledJobId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_ScheduledJobId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ScheduledJobId",
                table: "Jobs");
        }
    }
}

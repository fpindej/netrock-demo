using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Netrock.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddJobExecutionTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "jobexecutions",
                schema: "hangfire",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RecurringJobId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    HangfireJobId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    TriggeredBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jobexecutions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "jobexecutionlogentries",
                schema: "hangfire",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JobExecutionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Level = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jobexecutionlogentries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_jobexecutionlogentries_jobexecutions_JobExecutionId",
                        column: x => x.JobExecutionId,
                        principalSchema: "hangfire",
                        principalTable: "jobexecutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_jobexecutionlogentries_JobExecutionId",
                schema: "hangfire",
                table: "jobexecutionlogentries",
                column: "JobExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_jobexecutions_RecurringJobId_StartedAt",
                schema: "hangfire",
                table: "jobexecutions",
                columns: new[] { "RecurringJobId", "StartedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "jobexecutionlogentries",
                schema: "hangfire");

            migrationBuilder.DropTable(
                name: "jobexecutions",
                schema: "hangfire");
        }
    }
}

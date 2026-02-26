using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Netrock.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHangfireJobId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HangfireJobId",
                schema: "hangfire",
                table: "jobexecutions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HangfireJobId",
                schema: "hangfire",
                table: "jobexecutions",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}

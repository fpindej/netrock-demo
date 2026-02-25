using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Netrock.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDemoExpiresAtUtc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DemoExpiresAtUtc",
                schema: "auth",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DemoExpiresAtUtc",
                schema: "auth",
                table: "Users");
        }
    }
}

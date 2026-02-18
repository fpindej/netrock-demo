using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Netrock.Infrastructure.Features.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "audit");

            migrationBuilder.CreateTable(
                name: "AuditEvents",
                schema: "audit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TargetEntityType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TargetEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    Metadata = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_Action",
                schema: "audit",
                table: "AuditEvents",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_CreatedAt",
                schema: "audit",
                table: "AuditEvents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_TargetEntityId",
                schema: "audit",
                table: "AuditEvents",
                column: "TargetEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_UserId",
                schema: "audit",
                table: "AuditEvents",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditEvents",
                schema: "audit");
        }
    }
}

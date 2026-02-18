using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Netrock.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnablePgTrgm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP EXTENSION IF EXISTS pg_trgm;");
        }
    }
}

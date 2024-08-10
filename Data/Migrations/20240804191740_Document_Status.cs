using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace metalduck.Data.Migrations
{
    /// <inheritdoc />
    public partial class Document_Status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Documents",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Documents");
        }
    }
}

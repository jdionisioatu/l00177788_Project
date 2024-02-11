using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeScanning.Data.Migrations
{
    /// <inheritdoc />
    public partial class DDToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "defectDojoApiToken",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "defectDojoApiToken",
                table: "Settings");
        }
    }
}

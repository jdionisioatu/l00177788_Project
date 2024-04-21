using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeScanning.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserOrOrgName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "gitHubUserNameOrOrgName",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "gitHubUserNameOrOrgName",
                table: "Settings");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace USQLCSharp.Migrations
{
    public partial class devices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Devices");

            migrationBuilder.AddColumn<int>(
                name: "FreeMem",
                table: "Devices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Devices",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FreeMem",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Devices");

            migrationBuilder.AddColumn<bool>(
                name: "State",
                table: "Devices",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

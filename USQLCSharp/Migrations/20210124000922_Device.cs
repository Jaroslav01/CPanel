using Microsoft.EntityFrameworkCore.Migrations;

namespace USQLCSharp.Migrations
{
    public partial class Device : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ip",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mac",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rssi",
                table: "Devices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Uptime",
                table: "Devices",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ip",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Mac",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Rssi",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Uptime",
                table: "Devices");
        }
    }
}

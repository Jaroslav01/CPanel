using Microsoft.EntityFrameworkCore.Migrations;

namespace USQLCSharp.Migrations
{
    public partial class DeviceRefactored : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "Devices");

            migrationBuilder.CreateTable(
                name: "Parameters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Topic = table.Column<string>(nullable: true),
                    Data = table.Column<string>(nullable: true),
                    DeviceId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parameters_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_DeviceId",
                table: "Parameters",
                column: "DeviceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Parameters");

            migrationBuilder.AddColumn<string>(
                name: "Data",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

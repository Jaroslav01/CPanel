using Microsoft.EntityFrameworkCore.Migrations;

namespace USQLCSharp.Migrations
{
    public partial class Person : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_People_PersonId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_People_PersonId",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonStates_People_PersonId",
                table: "PersonStates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_People",
                table: "People");

            migrationBuilder.RenameTable(
                name: "People",
                newName: "Person");

            migrationBuilder.AddColumn<int>(
                name: "Admittance",
                table: "Contacts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SubmitEmailAddress",
                table: "Contacts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SubmitPhoneNumber",
                table: "Contacts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Login",
                table: "Person",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Person",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Person",
                table: "Person",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Person_PersonId",
                table: "Contacts",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Person_PersonId",
                table: "Devices",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonStates_Person_PersonId",
                table: "PersonStates",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Person_PersonId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Person_PersonId",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonStates_Person_PersonId",
                table: "PersonStates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Person",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "Admittance",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "SubmitEmailAddress",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "SubmitPhoneNumber",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Login",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Person");

            migrationBuilder.RenameTable(
                name: "Person",
                newName: "People");

            migrationBuilder.AddPrimaryKey(
                name: "PK_People",
                table: "People",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_People_PersonId",
                table: "Contacts",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_People_PersonId",
                table: "Devices",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonStates_People_PersonId",
                table: "PersonStates",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Lekkerbek.Web.Migrations
{
    public partial class Korting2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Korting",
                table: "Korting");

            migrationBuilder.RenameTable(
                name: "Korting",
                newName: "Kortingen");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kortingen",
                table: "Kortingen",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Kortingen",
                table: "Kortingen");

            migrationBuilder.RenameTable(
                name: "Kortingen",
                newName: "Korting");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Korting",
                table: "Korting",
                column: "Id");
        }
    }
}

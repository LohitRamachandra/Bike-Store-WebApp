using Microsoft.EntityFrameworkCore.Migrations;

namespace Bike.Web.Migrations
{
    public partial class AddName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MyProperty",
                table: "Makes",
                newName: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Makes",
                newName: "MyProperty");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace dotnet_mvc.Migrations
{
    public partial class restore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "ProductCharacteristic",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "ProductCharacteristic",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "ProductCharacteristic");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "ProductCharacteristic");
        }
    }
}

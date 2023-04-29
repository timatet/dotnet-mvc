using Microsoft.EntityFrameworkCore.Migrations;

namespace dotnet_mvc.Migrations
{
    public partial class BrandProductTables2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Brands_BrandId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "BrandId",
                table: "Products",
                newName: "BrandId1");

            migrationBuilder.RenameIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                newName: "IX_Products_BrandId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Brands_BrandId1",
                table: "Products",
                column: "BrandId1",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Brands_BrandId1",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "BrandId1",
                table: "Products",
                newName: "BrandId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_BrandId1",
                table: "Products",
                newName: "IX_Products_BrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Brands_BrandId",
                table: "Products",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

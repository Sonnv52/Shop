using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test.Migrations
{
    /// <inheritdoc />
    public partial class addsizenew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductSize");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductsId",
                table: "Size",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Qty",
                table: "Size",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Size_ProductsId",
                table: "Size",
                column: "ProductsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Size_Product_ProductsId",
                table: "Size",
                column: "ProductsId",
                principalTable: "Product",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Size_Product_ProductsId",
                table: "Size");

            migrationBuilder.DropIndex(
                name: "IX_Size_ProductsId",
                table: "Size");

            migrationBuilder.DropColumn(
                name: "ProductsId",
                table: "Size");

            migrationBuilder.DropColumn(
                name: "Qty",
                table: "Size");

            migrationBuilder.CreateTable(
                name: "ProductSize",
                columns: table => new
                {
                    ProductsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SizesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSize", x => new { x.ProductsId, x.SizesId });
                    table.ForeignKey(
                        name: "FK_ProductSize_Product_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductSize_Size_SizesId",
                        column: x => x.SizesId,
                        principalTable: "Size",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductSize_SizesId",
                table: "ProductSize",
                column: "SizesId");
        }
    }
}

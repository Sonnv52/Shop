using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.Api.Migrations
{
    /// <inheritdoc />
    public partial class newaddddd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillDetail_Size_SizeIdSizelog",
                table: "BillDetail");

            migrationBuilder.DropIndex(
                name: "IX_BillDetail_SizeIdSizelog",
                table: "BillDetail");

            migrationBuilder.DropColumn(
                name: "SizeIdSizelog",
                table: "BillDetail");

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "BillDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Adress",
                table: "Bill",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Bill",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "BillDetail");

            migrationBuilder.DropColumn(
                name: "Adress",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Bill");

            migrationBuilder.AddColumn<Guid>(
                name: "SizeIdSizelog",
                table: "BillDetail",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BillDetail_SizeIdSizelog",
                table: "BillDetail",
                column: "SizeIdSizelog");

            migrationBuilder.AddForeignKey(
                name: "FK_BillDetail_Size_SizeIdSizelog",
                table: "BillDetail",
                column: "SizeIdSizelog",
                principalTable: "Size",
                principalColumn: "IdSizelog");
        }
    }
}

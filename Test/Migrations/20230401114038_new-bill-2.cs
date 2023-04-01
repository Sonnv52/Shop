using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.Api.Migrations
{
    /// <inheritdoc />
    public partial class newbill2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}

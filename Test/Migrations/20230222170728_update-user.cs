using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test.Migrations
{
    /// <inheritdoc />
    public partial class updateuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bill_Customer_CustomersId",
                table: "Bill");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_Bill_CustomersId",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "CustomersId",
                table: "Bill");

            migrationBuilder.AddColumn<string>(
                name: "UserAppId",
                table: "Bill",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bill_UserAppId",
                table: "Bill",
                column: "UserAppId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bill_AspNetUsers_UserAppId",
                table: "Bill",
                column: "UserAppId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bill_AspNetUsers_UserAppId",
                table: "Bill");

            migrationBuilder.DropIndex(
                name: "IX_Bill_UserAppId",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "UserAppId",
                table: "Bill");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomersId",
                table: "Bill",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bill_CustomersId",
                table: "Bill",
                column: "CustomersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bill_Customer_CustomersId",
                table: "Bill",
                column: "CustomersId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

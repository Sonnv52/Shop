using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.Api.Migrations
{
    /// <inheritdoc />
    public partial class newcollumPayStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PayStatus",
                table: "Bill",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayStatus",
                table: "Bill");
        }
    }
}

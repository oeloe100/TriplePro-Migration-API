using Microsoft.EntityFrameworkCore.Migrations;

namespace TPMApi.Migrations.Woo
{
    public partial class ColumnNameUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WooKey",
                table: "WooAccess");

            migrationBuilder.DropColumn(
                name: "WooSecret",
                table: "WooAccess");

            migrationBuilder.AddColumn<string>(
                name: "WooClientKey",
                table: "WooAccess",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WooClientSecret",
                table: "WooAccess",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WooClientKey",
                table: "WooAccess");

            migrationBuilder.DropColumn(
                name: "WooClientSecret",
                table: "WooAccess");

            migrationBuilder.AddColumn<string>(
                name: "WooKey",
                table: "WooAccess",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WooSecret",
                table: "WooAccess",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

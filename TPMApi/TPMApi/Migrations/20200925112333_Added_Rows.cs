using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TPMApi.Migrations
{
    public partial class Added_Rows : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AfostoKey",
                table: "AfostoAccess",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AfostoSecret",
                table: "AfostoAccess",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created_At",
                table: "AfostoAccess",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AfostoAccess",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AfostoAccess",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AfostoKey",
                table: "AfostoAccess");

            migrationBuilder.DropColumn(
                name: "AfostoSecret",
                table: "AfostoAccess");

            migrationBuilder.DropColumn(
                name: "Created_At",
                table: "AfostoAccess");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AfostoAccess");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AfostoAccess");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bullows.Database.Migrations
{
    /// <inheritdoc />
    public partial class CustomerNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "CustomerMasters",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "CustomerMasters",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PAN",
                table: "CustomerMasters",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Pin",
                table: "CustomerMasters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "CustomerMasters",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "CustomerMasters");

            migrationBuilder.DropColumn(
                name: "District",
                table: "CustomerMasters");

            migrationBuilder.DropColumn(
                name: "PAN",
                table: "CustomerMasters");

            migrationBuilder.DropColumn(
                name: "Pin",
                table: "CustomerMasters");

            migrationBuilder.DropColumn(
                name: "State",
                table: "CustomerMasters");
        }
    }
}

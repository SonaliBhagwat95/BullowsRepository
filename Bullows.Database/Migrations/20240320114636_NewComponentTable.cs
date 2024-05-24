using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bullows.Database.Migrations
{
    /// <inheritdoc />
    public partial class NewComponentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LenghtSize",
                table: "ComponentTables",
                newName: "Length");

            migrationBuilder.AddColumn<string>(
                name: "Image_Path",
                table: "ComponentTables",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image_Path",
                table: "ComponentTables");

            migrationBuilder.RenameColumn(
                name: "Length",
                table: "ComponentTables",
                newName: "LenghtSize");
        }
    }
}

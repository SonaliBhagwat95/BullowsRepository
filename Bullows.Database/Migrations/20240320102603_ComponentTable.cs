using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bullows.Database.Migrations
{
    /// <inheritdoc />
    public partial class ComponentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComponentTables",
                columns: table => new
                {
                    ComponentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Component = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LenghtSize = table.Column<double>(type: "double", nullable: false),
                    WidthSize = table.Column<double>(type: "double", nullable: false),
                    HeightSize = table.Column<double>(type: "double", nullable: false),
                    Weight = table.Column<double>(type: "double", nullable: false),
                    QtyperAssembly = table.Column<int>(type: "int", nullable: false),
                    MaterialofConstruction = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SurfaceArea = table.Column<double>(type: "double", nullable: false),
                    WallThickness = table.Column<double>(type: "double", nullable: false),
                    ProductionRequirement = table.Column<int>(type: "int", nullable: false),
                    Workingdays = table.Column<int>(type: "int", nullable: false),
                    NumberofShifts = table.Column<int>(type: "int", nullable: false),
                    EffectiveWorking = table.Column<double>(type: "double", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentTables", x => x.ComponentID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComponentTables");
        }
    }
}

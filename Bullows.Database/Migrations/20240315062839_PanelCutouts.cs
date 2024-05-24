using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bullows.Database.Migrations
{
    /// <inheritdoc />
    public partial class PanelCutouts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Panel_Cutouts");

            migrationBuilder.CreateTable(
                name: "PanelCutouts",
                columns: table => new
                {
                    CutoutID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProjectID = table.Column<int>(type: "int", nullable: false),
                    PanelInputID = table.Column<int>(type: "int", nullable: false),
                    PartName = table.Column<int>(type: "int", nullable: false),
                    CutoutLength = table.Column<double>(type: "double", nullable: false),
                    CutoutWidth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CutoutXDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CutoutYDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PanelCutouts", x => x.CutoutID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PanelCutouts");

            migrationBuilder.CreateTable(
                name: "Panel_Cutouts",
                columns: table => new
                {
                    CutoutID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CutoutLength = table.Column<double>(type: "double", nullable: false),
                    CutoutWidth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CutoutXDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CutoutYDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PanelInputID = table.Column<int>(type: "int", nullable: false),
                    PartName = table.Column<int>(type: "int", nullable: false),
                    ProjectID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Panel_Cutouts", x => x.CutoutID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bullows.Database.Migrations
{
    /// <inheritdoc />
    public partial class DbInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PanelInput");

            migrationBuilder.CreateTable(
                name: "Panel_DuctCutoutDetails",
                columns: table => new
                {
                    DuctCutoutID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProjectID = table.Column<int>(type: "int", nullable: false),
                    PartName = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CutoutLength = table.Column<double>(type: "double", nullable: false),
                    CutoutWidth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CutoutXDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CutoutYDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Panel_DuctCutoutDetails", x => x.DuctCutoutID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Panel_ServiceDoorDetails",
                columns: table => new
                {
                    ServicedoorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PanelInputID = table.Column<int>(type: "int", nullable: false),
                    PartName = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ServiceDoorHeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ServiceDoorWidth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GlassLength = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GlassWidth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DoorXDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DoorYDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Panel_ServiceDoorDetails", x => x.ServicedoorID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Panel_TubeLightDetails",
                columns: table => new
                {
                    TubeLightID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PanelInputID = table.Column<int>(type: "int", nullable: false),
                    PartName = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    TubelightWidth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TubelightHeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    XDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    YDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Panel_TubeLightDetails", x => x.TubeLightID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Panel_WatchGlassDetails",
                columns: table => new
                {
                    WatchGlassID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProjectID = table.Column<int>(type: "int", nullable: false),
                    PartName = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    GlassLength = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GlassWidth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DoorXDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DoorYDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Panel_WatchGlassDetails", x => x.WatchGlassID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PanelInputDetails",
                columns: table => new
                {
                    PanelInputID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProjectID = table.Column<int>(type: "int", nullable: false),
                    PanelWidth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PanelHeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SheetThickness = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StandardBend1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StandardBend2 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PitchDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SlotDimentions = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NoofPanels = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PanelInputDetails", x => x.PanelInputID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Panel_DuctCutoutDetails");

            migrationBuilder.DropTable(
                name: "Panel_ServiceDoorDetails");

            migrationBuilder.DropTable(
                name: "Panel_TubeLightDetails");

            migrationBuilder.DropTable(
                name: "Panel_WatchGlassDetails");

            migrationBuilder.DropTable(
                name: "PanelInputDetails");

            migrationBuilder.CreateTable(
                name: "PanelInput",
                columns: table => new
                {
                    PanelID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CutoutLength = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CutoutWidth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CutoutXDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CutoutYDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DoorXDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DoorYDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GlassLength = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GlassWidth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    NoofPanels = table.Column<int>(type: "int", nullable: false),
                    PanelHeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PanelWidth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PartName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PitchDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProjectID = table.Column<int>(type: "int", nullable: false),
                    ServiceDoorHeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ServiceDoorWidth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SheetThickness = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SlotDimentions = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StandardBend1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StandardBend2 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TubelightHeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TubelightWidth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    XDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    YDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PanelInput", x => x.PanelID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}

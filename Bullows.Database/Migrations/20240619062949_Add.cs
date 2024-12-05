using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bullows.Database.Migrations
{
    /// <inheritdoc />
    public partial class Add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComponentTables",
                columns: table => new
                {
                    ComponentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Component = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Length = table.Column<double>(type: "float", nullable: false),
                    WidthSize = table.Column<double>(type: "float", nullable: false),
                    HeightSize = table.Column<double>(type: "float", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    QtyperAssembly = table.Column<int>(type: "int", nullable: false),
                    MaterialofConstruction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SurfaceArea = table.Column<double>(type: "float", nullable: false),
                    WallThickness = table.Column<double>(type: "float", nullable: false),
                    ProductionRequirement = table.Column<int>(type: "int", nullable: false),
                    Workingdays = table.Column<int>(type: "int", nullable: false),
                    NumberofShifts = table.Column<int>(type: "int", nullable: false),
                    Image_Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EffectiveWorking = table.Column<double>(type: "float", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Conveyor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pitch = table.Column<double>(type: "float", nullable: false),
                    Speed = table.Column<double>(type: "float", nullable: false),
                    LoadingUnloading = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComponentHandling = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoOfColors = table.Column<int>(type: "int", nullable: false),
                    NoOfCoats = table.Column<int>(type: "int", nullable: false),
                    Viscosity = table.Column<double>(type: "float", nullable: false),
                    Paint = table.Column<double>(type: "float", nullable: false),
                    Powder = table.Column<double>(type: "float", nullable: false),
                    DFT = table.Column<double>(type: "float", nullable: false),
                    ConsumptionPerDay = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentTables", x => x.ComponentID);
                });

            migrationBuilder.CreateTable(
                name: "CustomerMasters",
                columns: table => new
                {
                    CustomerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StateId = table.Column<int>(type: "int", nullable: false),
                    DistrictId = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    Pin = table.Column<int>(type: "int", nullable: false),
                    PAN = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerMasters", x => x.CustomerID);
                });

            migrationBuilder.CreateTable(
                name: "EnquiryMasters",
                columns: table => new
                {
                    EnquiryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ComponentID = table.Column<int>(type: "int", nullable: false),
                    ProposalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SalesNO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnquiryMasters", x => x.EnquiryID);
                });

            migrationBuilder.CreateTable(
                name: "PaintBooths",
                columns: table => new
                {
                    PaintBoothID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnquiryId = table.Column<int>(type: "int", nullable: false),
                    D1 = table.Column<double>(type: "float", nullable: false),
                    jobsize = table.Column<double>(type: "float", nullable: false),
                    D2 = table.Column<double>(type: "float", nullable: false),
                    D3 = table.Column<double>(type: "float", nullable: false),
                    W1 = table.Column<double>(type: "float", nullable: false),
                    W2 = table.Column<double>(type: "float", nullable: false),
                    W3 = table.Column<double>(type: "float", nullable: false),
                    D = table.Column<double>(type: "float", nullable: false),
                    H1 = table.Column<double>(type: "float", nullable: false),
                    H2 = table.Column<double>(type: "float", nullable: false),
                    W = table.Column<double>(type: "float", nullable: false),
                    H = table.Column<double>(type: "float", nullable: false),
                    PanelWidth = table.Column<double>(type: "float", nullable: false),
                    PanelHeight = table.Column<double>(type: "float", nullable: false),
                    SheetThickness = table.Column<double>(type: "float", nullable: false),
                    StandardBend1 = table.Column<double>(type: "float", nullable: false),
                    StandardBend2 = table.Column<double>(type: "float", nullable: false),
                    PitchDistance = table.Column<double>(type: "float", nullable: false),
                    noofpanels = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaintBooths", x => x.PaintBoothID);
                });

            migrationBuilder.CreateTable(
                name: "PanelCutouts",
                columns: table => new
                {
                    CutoutID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectID = table.Column<int>(type: "int", nullable: false),
                    PanelInputID = table.Column<int>(type: "int", nullable: false),
                    PartName = table.Column<int>(type: "int", nullable: false),
                    CutoutLength = table.Column<double>(type: "float", nullable: false),
                    CutoutWidth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CutoutXDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CutoutYDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PanelCutouts", x => x.CutoutID);
                });

            migrationBuilder.CreateTable(
                name: "tblAddContactPersons",
                columns: table => new
                {
                    ContactId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<int>(type: "int", nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblAddContactPersons", x => x.ContactId);
                });

            migrationBuilder.CreateTable(
                name: "TblCities",
                columns: table => new
                {
                    CityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DistrictId = table.Column<int>(type: "int", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Isdeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblCities", x => x.CityId);
                });

            migrationBuilder.CreateTable(
                name: "tblDistricts",
                columns: table => new
                {
                    DistrictId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StateId = table.Column<int>(type: "int", nullable: false),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Isdeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblDistricts", x => x.DistrictId);
                });

            migrationBuilder.CreateTable(
                name: "tblStates",
                columns: table => new
                {
                    StateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Isdeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblStates", x => x.StateId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComponentTables");

            migrationBuilder.DropTable(
                name: "CustomerMasters");

            migrationBuilder.DropTable(
                name: "EnquiryMasters");

            migrationBuilder.DropTable(
                name: "PaintBooths");

            migrationBuilder.DropTable(
                name: "PanelCutouts");

            migrationBuilder.DropTable(
                name: "tblAddContactPersons");

            migrationBuilder.DropTable(
                name: "TblCities");

            migrationBuilder.DropTable(
                name: "tblDistricts");

            migrationBuilder.DropTable(
                name: "tblStates");
        }
    }
}

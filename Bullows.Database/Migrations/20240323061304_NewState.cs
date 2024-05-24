using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bullows.Database.Migrations
{
    /// <inheritdoc />
    public partial class NewState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "CustomerMasters");

            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "CustomerMasters");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "CustomerMasters");

            migrationBuilder.DropColumn(
                name: "District",
                table: "CustomerMasters");

            migrationBuilder.DropColumn(
                name: "EmailId",
                table: "CustomerMasters");

            migrationBuilder.DropColumn(
                name: "MobileNo",
                table: "CustomerMasters");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "CustomerMasters",
                newName: "CompanyName");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "CustomerMasters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "CustomerMasters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "CustomerMasters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "tblAddContactPersons",
                columns: table => new
                {
                    ContactId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CustomerID = table.Column<int>(type: "int", nullable: false),
                    ContactPerson = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MobileNo = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailId = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblAddContactPersons", x => x.ContactId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TblCities",
                columns: table => new
                {
                    CityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DistrictId = table.Column<int>(type: "int", nullable: false),
                    City = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Isdeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblCities", x => x.CityId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tblDistricts",
                columns: table => new
                {
                    DistrictId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StateId = table.Column<int>(type: "int", nullable: false),
                    District = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Isdeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblDistricts", x => x.DistrictId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tblStates",
                columns: table => new
                {
                    StateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    State = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Isdeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblStates", x => x.StateId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblAddContactPersons");

            migrationBuilder.DropTable(
                name: "TblCities");

            migrationBuilder.DropTable(
                name: "tblDistricts");

            migrationBuilder.DropTable(
                name: "tblStates");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "CustomerMasters");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "CustomerMasters");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "CustomerMasters");

            migrationBuilder.RenameColumn(
                name: "CompanyName",
                table: "CustomerMasters",
                newName: "State");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "CustomerMasters",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "CustomerMasters",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
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
                name: "EmailId",
                table: "CustomerMasters",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "MobileNo",
                table: "CustomerMasters",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}

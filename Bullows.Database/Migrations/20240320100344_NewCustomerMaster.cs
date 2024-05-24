using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bullows.Database.Migrations
{
    /// <inheritdoc />
    public partial class NewCustomerMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "CustomerMasters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "CustomerMasters",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CustomerMasters",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "CustomerMasters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "CustomerMasters",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CustomerMasters");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "CustomerMasters");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CustomerMasters");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "CustomerMasters");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "CustomerMasters");
        }
    }
}

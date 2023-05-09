using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastracture.Migrations
{
    public partial class AddCountry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarOwners_governments_GovernmentId",
                table: "CarOwners");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_governments_GovernmentId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_governments_GovernmentId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Marketers_governments_GovernmentId",
                table: "Marketers");

            migrationBuilder.AlterColumn<Guid>(
                name: "GovernmentId",
                table: "Marketers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Marketers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "GovernmentId",
                table: "Employees",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "GovernmentId",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "GovernmentId",
                table: "CarOwners",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "CarOwners",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CarOwners_governments_GovernmentId",
                table: "CarOwners",
                column: "GovernmentId",
                principalTable: "governments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_governments_GovernmentId",
                table: "Customers",
                column: "GovernmentId",
                principalTable: "governments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_governments_GovernmentId",
                table: "Employees",
                column: "GovernmentId",
                principalTable: "governments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Marketers_governments_GovernmentId",
                table: "Marketers",
                column: "GovernmentId",
                principalTable: "governments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarOwners_governments_GovernmentId",
                table: "CarOwners");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_governments_GovernmentId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_governments_GovernmentId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Marketers_governments_GovernmentId",
                table: "Marketers");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Marketers");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "CarOwners");

            migrationBuilder.AlterColumn<Guid>(
                name: "GovernmentId",
                table: "Marketers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "GovernmentId",
                table: "Employees",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "GovernmentId",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "GovernmentId",
                table: "CarOwners",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CarOwners_governments_GovernmentId",
                table: "CarOwners",
                column: "GovernmentId",
                principalTable: "governments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_governments_GovernmentId",
                table: "Customers",
                column: "GovernmentId",
                principalTable: "governments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_governments_GovernmentId",
                table: "Employees",
                column: "GovernmentId",
                principalTable: "governments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Marketers_governments_GovernmentId",
                table: "Marketers",
                column: "GovernmentId",
                principalTable: "governments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

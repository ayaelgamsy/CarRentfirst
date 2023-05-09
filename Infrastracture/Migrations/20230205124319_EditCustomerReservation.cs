using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastracture.Migrations
{
    public partial class EditCustomerReservation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerReservations_Customers_CustomerId",
                table: "CustomerReservations");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerReservations_Stocks_StockId",
                table: "CustomerReservations");

            migrationBuilder.DropIndex(
                name: "IX_CustomerReservations_CustomerId",
                table: "CustomerReservations");

            migrationBuilder.DropIndex(
                name: "IX_CustomerReservations_StockId",
                table: "CustomerReservations");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "CustomerReservations");

            migrationBuilder.DropColumn(
                name: "RestValue",
                table: "CustomerReservations");

            migrationBuilder.DropColumn(
                name: "StockId",
                table: "CustomerReservations");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "CustomerReservations");

            migrationBuilder.RenameColumn(
                name: "payment",
                table: "CustomerReservations",
                newName: "Value");

            migrationBuilder.AddColumn<string>(
                name: "Customer",
                table: "CustomerReservations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerReservationPhoto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastEditUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastEditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReservationPhoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerReservationPhoto_CustomerReservations_CustomerReservationId",
                        column: x => x.CustomerReservationId,
                        principalTable: "CustomerReservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReservationPhoto_CustomerReservationId",
                table: "CustomerReservationPhoto",
                column: "CustomerReservationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerReservationPhoto");

            migrationBuilder.DropColumn(
                name: "Customer",
                table: "CustomerReservations");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "CustomerReservations",
                newName: "payment");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "CustomerReservations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<double>(
                name: "RestValue",
                table: "CustomerReservations",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<Guid>(
                name: "StockId",
                table: "CustomerReservations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<double>(
                name: "Total",
                table: "CustomerReservations",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReservations_CustomerId",
                table: "CustomerReservations",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReservations_StockId",
                table: "CustomerReservations",
                column: "StockId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerReservations_Customers_CustomerId",
                table: "CustomerReservations",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerReservations_Stocks_StockId",
                table: "CustomerReservations",
                column: "StockId",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

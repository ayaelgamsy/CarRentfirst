using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastracture.Migrations
{
    public partial class AddCustomerEvaluation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CustomerEvaluationId",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GobTitle",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerEvaluationId",
                table: "CustomerRents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerEvaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerEvaluations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerEvaluationId",
                table: "Customers",
                column: "CustomerEvaluationId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRents_CustomerEvaluationId",
                table: "CustomerRents",
                column: "CustomerEvaluationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerRents_CustomerEvaluations_CustomerEvaluationId",
                table: "CustomerRents",
                column: "CustomerEvaluationId",
                principalTable: "CustomerEvaluations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_CustomerEvaluations_CustomerEvaluationId",
                table: "Customers",
                column: "CustomerEvaluationId",
                principalTable: "CustomerEvaluations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerRents_CustomerEvaluations_CustomerEvaluationId",
                table: "CustomerRents");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_CustomerEvaluations_CustomerEvaluationId",
                table: "Customers");

            migrationBuilder.DropTable(
                name: "CustomerEvaluations");

            migrationBuilder.DropIndex(
                name: "IX_Customers_CustomerEvaluationId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_CustomerRents_CustomerEvaluationId",
                table: "CustomerRents");

            migrationBuilder.DropColumn(
                name: "CustomerEvaluationId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "GobTitle",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CustomerEvaluationId",
                table: "CustomerRents");
        }
    }
}

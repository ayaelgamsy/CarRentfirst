using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastracture.Migrations
{
    public partial class AddGurantorPhoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuarantorPhotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerRentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuarantorPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuarantorPhotos_CustomerRents_CustomerRentId",
                        column: x => x.CustomerRentId,
                        principalTable: "CustomerRents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuarantorPhotos_CustomerRentId",
                table: "GuarantorPhotos",
                column: "CustomerRentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuarantorPhotos");
        }
    }
}

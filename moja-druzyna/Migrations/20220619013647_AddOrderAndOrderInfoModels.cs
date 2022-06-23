using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace moja_druzyna.Migrations
{
    public partial class AddOrderAndOrderInfoModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Contents = table.Column<string>(type: "nvarchar(max)", maxLength: 12000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ScoutPeselScout = table.Column<string>(type: "nvarchar(11)", nullable: true),
                    TeamIdTeam = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderInfos_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderInfos_scout_ScoutPeselScout",
                        column: x => x.ScoutPeselScout,
                        principalTable: "scout",
                        principalColumn: "PeselScout",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderInfos_Teams_TeamIdTeam",
                        column: x => x.TeamIdTeam,
                        principalTable: "Teams",
                        principalColumn: "IdTeam",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderInfos_OrderId",
                table: "OrderInfos",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderInfos_ScoutPeselScout",
                table: "OrderInfos",
                column: "ScoutPeselScout");

            migrationBuilder.CreateIndex(
                name: "IX_OrderInfos_TeamIdTeam",
                table: "OrderInfos",
                column: "TeamIdTeam");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderInfos");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}

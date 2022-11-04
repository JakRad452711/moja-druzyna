using Microsoft.EntityFrameworkCore.Migrations;

namespace moja_druzyna.Migrations
{
    public partial class FixScoutModelSoItCanHaveMoreThan1Achievement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_scout_achievement_ScoutPeselScout",
                table: "scout_achievement");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_scout_achievement_ScoutPeselScout",
                table: "scout_achievement",
                column: "ScoutPeselScout",
                unique: true);
        }
    }
}

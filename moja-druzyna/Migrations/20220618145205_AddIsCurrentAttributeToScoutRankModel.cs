using Microsoft.EntityFrameworkCore.Migrations;

namespace moja_druzyna.Migrations
{
    public partial class AddIsCurrentAttributeToScoutRankModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "scout");

            migrationBuilder.AddColumn<bool>(
                name: "IsCurrent",
                table: "scout_rank",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCurrent",
                table: "scout_rank");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "scout",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

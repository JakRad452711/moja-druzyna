using Microsoft.EntityFrameworkCore.Migrations;

namespace moja_druzyna.Migrations
{
    public partial class AddIsDeletedFieldToScout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "scout",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "scout");
        }
    }
}

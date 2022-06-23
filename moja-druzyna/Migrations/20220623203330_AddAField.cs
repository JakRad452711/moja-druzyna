using Microsoft.EntityFrameworkCore.Migrations;

namespace moja_druzyna.Migrations
{
    public partial class AddAField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "OrderInfos",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "OrderInfos");
        }
    }
}

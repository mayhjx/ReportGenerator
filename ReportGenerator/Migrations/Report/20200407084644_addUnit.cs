using Microsoft.EntityFrameworkCore.Migrations;

namespace ReportGenerator.Migrations.Report
{
    public partial class addUnit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Report",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Report");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace ReportGenerator.Migrations.Report
{
    public partial class addbias : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bias",
                table: "Report",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bias",
                table: "Report");
        }
    }
}

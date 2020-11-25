using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ReportGenerator.Migrations.Report
{
    public partial class AddFrontPageInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastInvestigationDate",
                table: "Report",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProtocalID",
                table: "Report",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                table: "Report",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastInvestigationDate",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "ProtocalID",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "Report");
        }
    }
}

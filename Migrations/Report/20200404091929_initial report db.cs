using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ReportGenerator.Migrations.Report
{
    public partial class initialreportdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(nullable: false),
                    Item = table.Column<string>(nullable: true),
                    TargetInstrumentName = table.Column<string>(nullable: true),
                    MatchInstrumentName = table.Column<string>(nullable: true),
                    TargetReagentLot = table.Column<string>(nullable: true),
                    MatchReagentLot = table.Column<string>(nullable: true),
                    StartTestDate = table.Column<DateTime>(nullable: false),
                    EndTestDate = table.Column<DateTime>(nullable: false),
                    EvaluationDate = table.Column<DateTime>(nullable: false),
                    Technician = table.Column<string>(nullable: true),
                    SampleName = table.Column<string>(nullable: true),
                    TargetResult = table.Column<string>(nullable: true),
                    MatchResult = table.Column<string>(nullable: true),
                    ALE = table.Column<double>(nullable: false),
                    Xc1 = table.Column<double>(nullable: false),
                    Xc2 = table.Column<double>(nullable: false),
                    a = table.Column<double>(nullable: false),
                    aUCI = table.Column<double>(nullable: false),
                    aLCI = table.Column<double>(nullable: false),
                    b = table.Column<double>(nullable: false),
                    bUCI = table.Column<double>(nullable: false),
                    bLCI = table.Column<double>(nullable: false),
                    P = table.Column<double>(nullable: false),
                    PicturePath = table.Column<string>(nullable: true),
                    Investigator = table.Column<string>(nullable: true),
                    InvestigationDate = table.Column<DateTime>(nullable: false),
                    Approver = table.Column<string>(nullable: true),
                    ApprovalDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Report");
        }
    }
}

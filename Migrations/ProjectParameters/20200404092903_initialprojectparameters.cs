using Microsoft.EntityFrameworkCore.Migrations;

namespace ReportGenerator.Migrations.ProjectParameters
{
    public partial class initialprojectparameters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectParameter",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    ALE = table.Column<double>(nullable: false),
                    Xc1 = table.Column<double>(nullable: false),
                    Xc2 = table.Column<double>(nullable: false),
                    SignificantDigits = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectParameter", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectParameter");
        }
    }
}

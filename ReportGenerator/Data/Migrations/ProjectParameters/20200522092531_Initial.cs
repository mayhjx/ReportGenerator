using Microsoft.EntityFrameworkCore.Migrations;

namespace ReportGenerator.Migrations.ProjectParameters
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectParameter",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    SpecificationOneConcRange = table.Column<double>(nullable: false),
                    SpecificationOne = table.Column<double>(nullable: false),
                    SpecificationTwoConcRange = table.Column<double>(nullable: false),
                    SpecificationTwo = table.Column<double>(nullable: false),
                    Xc1 = table.Column<double>(nullable: false),
                    Xc2 = table.Column<double>(nullable: false),
                    SignificantDigits = table.Column<int>(nullable: false),
                    Unit = table.Column<string>(nullable: false),
                    LOQ = table.Column<double>(nullable: false),
                    ALE = table.Column<double>(nullable: false)
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

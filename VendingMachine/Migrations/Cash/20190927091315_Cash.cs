using Microsoft.EntityFrameworkCore.Migrations;

namespace VendingMachine.Migrations.Cash
{
    public partial class Cash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cashes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FaceValue = table.Column<float>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    IsBlocked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cashes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cashes");
        }
    }
}

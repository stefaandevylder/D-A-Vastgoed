using Microsoft.EntityFrameworkCore.Migrations;

namespace DnaVastgoed.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    IsUploaded = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    Energy = table.Column<string>(nullable: true),
                    Price = table.Column<string>(nullable: true),
                    LotArea = table.Column<string>(nullable: true),
                    LivingArea = table.Column<string>(nullable: true),
                    Rooms = table.Column<string>(nullable: true),
                    Bedrooms = table.Column<string>(nullable: true),
                    Bathrooms = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    EPCNumber = table.Column<string>(nullable: true),
                    Katastraalinkomen = table.Column<string>(nullable: true),
                    OrientatieAchtergevel = table.Column<string>(nullable: true),
                    Elektriciteitskeuring = table.Column<string>(nullable: true),
                    Bouwvergunning = table.Column<string>(nullable: true),
                    StedenbouwkundigeBestemming = table.Column<string>(nullable: true),
                    Verkavelingsvergunning = table.Column<string>(nullable: true),
                    Dagvaarding = table.Column<string>(nullable: true),
                    Verkooprecht = table.Column<string>(nullable: true),
                    RisicoOverstroming = table.Column<string>(nullable: true),
                    AfgebakendOverstromingsGebied = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Properties");
        }
    }
}

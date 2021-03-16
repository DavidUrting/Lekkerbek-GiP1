using Microsoft.EntityFrameworkCore.Migrations;

namespace Lekkerbek.Web.Migrations
{
    public partial class KlantEnGerecht : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gerechten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Categorie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Naam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Omschrijving = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Prijs = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gerechten", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Klanten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StraatEnNummer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Postcode = table.Column<int>(type: "int", nullable: false),
                    Woonplaats = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefoonnummer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Opmerkingen = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Klanten", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Gerechten",
                columns: new[] { "Id", "Categorie", "Naam", "Omschrijving", "Prijs" },
                values: new object[,]
                {
                    { -1, "Sandwiches", "Ruebens Style Pastrami (Homemade)", "Mierikswortel, zuurkool, augurk 1, parmezaan", 9.5m },
                    { -17, "Visgerechten", "Salmona alla Siciliana", "Zalm met klappertjes en parika in kruidige tomatensaus", 21.5m },
                    { -16, "Visgerechten", "Pesce spada in griglia", "Zwaardvisfilet van de gril, geserveerd met 2 sauzen", 18.5m },
                    { -15, "Visgerechten", "Salmona in griglia", "Gegrilde zalm, geserveerd met 2 sauzen", 22.5m },
                    { -14, "Visgerechten", "Trota salmonata", "Zalmforel in totmaten-knoflooksaus", 18m },
                    { -13, "Vleesgerechten", "Bocconcini della mamma", "Kalsflapjes in salie-witte wijnsaus", 21.5m },
                    { -12, "Vleesgerechten", "Filetto del maestro", "Varkenshaas met ham en kaas in knoflookroomsaus", 20.75m },
                    { -10, "Vleesgerechten", "Petto di pollo ripieno", "Kip gevuld met mozzarella, champignons en spinazie", 17.25m },
                    { -11, "Vleesgerechten", "Scaloppina alla casalinga", "Varkensfilets met ham en champignon-roomsaus", 18m },
                    { -8, "Salades", "Mozzarella di Bufala", "Parmezan, zongedroogde tomaat, pesto", 15m },
                    { -7, "Salades", "Green Tea Smoked salmon", "Pickle, groene kruiden, frisse salade", 10.5m },
                    { -6, "Salades", "Salade geitenkaas", "Bessenhoning, mosterddressing, noten", 11m },
                    { -5, "Sandwiches", "Torta pizza", "Frisse sla, hummus, geroosterde veggies, avocado", 8m },
                    { -4, "Sandwiches", "Steambun crispy chicken", "Bosui, rode peper, sesam, homemade stickysaus", 8.5m },
                    { -3, "Sandwiches", "Mozzarella di bufala", "Parmezaan, pesto, rucola, zontomaat, walnoot", 9m },
                    { -2, "Sandwiches", "Green tea smoked salmon", "Crostini's , vinaigrette,  frisse sla, pickle", 9.5m },
                    { -9, "Salades", "Asian salad met veggies", "Sesamdressing, avocado, frisse salade", 16m }
                });

            migrationBuilder.InsertData(
                table: "Klanten",
                columns: new[] { "Id", "Email", "Naam", "Opmerkingen", "Postcode", "StraatEnNummer", "Telefoonnummer", "Woonplaats" },
                values: new object[,]
                {
                    { -3, "henk.verelst@ucll.be", "Henk Verelst", null, 3272, "Teststraat 122", "012345672", "Testelt" },
                    { -1, "guy.marckelbach@ucll.be", "Guy Marckelbach", null, 3272, "Teststraat 120", "012345670", "Testelt" },
                    { -2, "elise.coenen@ucll.be", "Elise Coenen", null, 3272, "Teststraat 121", "012345671", "Testelt" },
                    { -4, "david.urting@ucll.be", "David Urting", null, 3272, "Teststraat 123", "012345673", "Testelt" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Gerechten");

            migrationBuilder.DropTable(
                name: "Klanten");
        }
    }
}

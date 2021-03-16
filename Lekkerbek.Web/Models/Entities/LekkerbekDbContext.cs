using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lekkerbek.Web.Models.Entities
{
    public class LekkerbekDbContext: DbContext
    {
        public LekkerbekDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Korting> Kortingen { get; set; }

        public DbSet<Klant> Klanten { get; set; }
        public DbSet<Gerecht> Gerechten { get; set; }
        public DbSet<Bestelling> Bestellingen { get; set; }
        public DbSet<BesteldGerecht> BesteldeGerechten { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<Korting>()
                .Property(g => g.Percentage)
                .HasPrecision(3, 0);
            modelBuilder
                .Entity<Gerecht>()
                .Property(g => g.Prijs)
                .HasPrecision(6, 2); // Functioneel ok? Prijs kan tot €9.999,99 gaan. Zou voldoende moeten zijn :)

            // Initiële inserts ... 
            // Zie https://docs.microsoft.com/en-us/ef/core/modeling/data-seeding
            modelBuilder
                .Entity<Korting>()
                .HasData(
                    new Korting()
                    {
                        Id = -1,
                        AantalBestellingen = 3,
                        Percentage = 10
                    }
            );
            modelBuilder
                .Entity<Klant>()
                .HasData(
                    new Klant()
                    {
                        Id = -1,
                        Naam = "Guy Marckelbach",
                        Email = "guy.marckelbach@ucll.be",
                        Postcode = 3272,
                        StraatEnNummer = "Teststraat 120",
                        Woonplaats = "Testelt",
                        Telefoonnummer = "012345670"
                    },
                    new Klant()
                    {
                        Id = -2,
                        Naam = "Elise Coenen",
                        Email = "elise.coenen@ucll.be",
                        Postcode = 3272,
                        StraatEnNummer = "Teststraat 121",
                        Woonplaats = "Testelt",
                        Telefoonnummer = "012345671"
                    },
                    new Klant()
                    {
                        Id = -3,
                        Naam = "Henk Verelst",
                        Email = "henk.verelst@ucll.be",
                        Postcode = 3272,
                        StraatEnNummer = "Teststraat 122",
                        Woonplaats = "Testelt",
                        Telefoonnummer = "012345672"
                    },
                    new Klant()
                    {
                        Id = -4,
                        Naam = "David Urting",
                        Email = "david.urting@ucll.be",
                        Postcode = 3272,
                        StraatEnNummer = "Teststraat 123",
                        Woonplaats = "Testelt",
                        Telefoonnummer = "012345673"
                    }
                );

            modelBuilder
                .Entity<Gerecht>()
                .HasData(
                    // SANDWICHES
                    new Gerecht
                    {
                        Id = -1,
                        Categorie = "Sandwiches",
                        Naam = "Ruebens Style Pastrami (Homemade)",
                        Omschrijving = "Mierikswortel, zuurkool, augurk 1, parmezaan",
                        Prijs = 9.5m
                    },
                    new Gerecht
                    {
                        Id = -2,
                        Categorie = "Sandwiches",
                        Naam = "Green tea smoked salmon",
                        Omschrijving = "Crostini's , vinaigrette,  frisse sla, pickle",
                        Prijs = 9.5m
                    },
                    new Gerecht
                    {
                        Id = -3,
                        Categorie = "Sandwiches",
                        Naam = "Mozzarella di bufala",
                        Omschrijving = "Parmezaan, pesto, rucola, zontomaat, walnoot",
                        Prijs = 9m
                    },
                    new Gerecht
                    {
                        Id = -4,
                        Categorie = "Sandwiches",
                        Naam = "Steambun crispy chicken",
                        Omschrijving = "Bosui, rode peper, sesam, homemade stickysaus",
                        Prijs = 8.5m
                    },
                    new Gerecht
                    {
                        Id = -5,
                        Categorie = "Sandwiches",
                        Naam = "Torta pizza",
                        Omschrijving = "Frisse sla, hummus, geroosterde veggies, avocado",
                        Prijs = 8m
                    },
                    // SALADES
                    new Gerecht
                    {
                        Id = -6,
                        Categorie = "Salades",
                        Naam = "Salade geitenkaas",
                        Omschrijving = "Bessenhoning, mosterddressing, noten",
                        Prijs = 11m
                    },
                    new Gerecht
                    {
                        Id = -7,
                        Categorie = "Salades",
                        Naam = "Green Tea Smoked salmon",
                        Omschrijving = "Pickle, groene kruiden, frisse salade",
                        Prijs = 10.5m
                    },
                    new Gerecht
                    {
                        Id = -8,
                        Categorie = "Salades",
                        Naam = "Mozzarella di Bufala",
                        Omschrijving = "Parmezan, zongedroogde tomaat, pesto",
                        Prijs = 15m
                    },
                    new Gerecht
                    {
                        Id = -9,
                        Categorie = "Salades",
                        Naam = "Asian salad met veggies",
                        Omschrijving = "Sesamdressing, avocado, frisse salade",
                        Prijs = 16m
                    },
                    // VLEESGERECHTEN
                    new Gerecht
                    {
                        Id = -10,
                        Categorie = "Vleesgerechten",
                        Naam = "Petto di pollo ripieno",
                        Omschrijving = "Kip gevuld met mozzarella, champignons en spinazie",
                        Prijs = 17.25m
                    },
                    new Gerecht
                    {
                        Id = -11,
                        Categorie = "Vleesgerechten",
                        Naam = "Scaloppina alla casalinga",
                        Omschrijving = "Varkensfilets met ham en champignon-roomsaus",
                        Prijs = 18m
                    },
                    new Gerecht
                    {
                        Id = -12,
                        Categorie = "Vleesgerechten",
                        Naam = "Filetto del maestro",
                        Omschrijving = "Varkenshaas met ham en kaas in knoflookroomsaus",
                        Prijs = 20.75m
                    },
                    new Gerecht
                    {
                        Id = -13,
                        Categorie = "Vleesgerechten",
                        Naam = "Bocconcini della mamma",
                        Omschrijving = "Kalsflapjes in salie-witte wijnsaus",
                        Prijs = 21.5m
                    },
                    // VISGERECHTEN
                    new Gerecht
                    {
                        Id = -14,
                        Categorie = "Visgerechten",
                        Naam = "Trota salmonata",
                        Omschrijving = "Zalmforel in totmaten-knoflooksaus",
                        Prijs = 18m
                    },
                    new Gerecht
                    {
                        Id = -15,
                        Categorie = "Visgerechten",
                        Naam = "Salmona in griglia",
                        Omschrijving = "Gegrilde zalm, geserveerd met 2 sauzen",
                        Prijs = 22.5m
                    },
                    new Gerecht
                    {
                        Id = -16,
                        Categorie = "Visgerechten",
                        Naam = "Pesce spada in griglia",
                        Omschrijving = "Zwaardvisfilet van de gril, geserveerd met 2 sauzen",
                        Prijs = 18.5m
                    },
                    new Gerecht
                    {
                        Id = -17,
                        Categorie = "Visgerechten",
                        Naam = "Salmona alla Siciliana",
                        Omschrijving = "Zalm met klappertjes en parika in kruidige tomatensaus",
                        Prijs = 21.5m
                    }
                );
        }
    }
}

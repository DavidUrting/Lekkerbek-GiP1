using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lekkerbek.Web.Models.Entities;

namespace Lekkerbek.Web.Controllers
{
    public class BestellingController : Controller
    {
        public const string ACTIE_BEWAREN = "Bewaren";
        public const string ACTIE_BESTELLEN = "Bestellen";
        public const string ACTIE_AFLEVEREN = "Afleveren";
        public const string ACTIE_ANNULEREN = "Annuleren";

        private readonly LekkerbekDbContext _context;

        public BestellingController(LekkerbekDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Geeft een view met alle actieve bestellingen terug.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var actieveBestellingen = await _context
                .Bestellingen
                .Include(b => b.Klant)
                .Where(b => b.Status != Bestelling.EnumStatus.Geannuleerd)
                .OrderByDescending(b => b.Tijdslot)
                .ToListAsync();

            return View(actieveBestellingen);
        }

        /// <summary>
        /// Geeft een view met alle, voor vandaag, nog af te rekenen bestellingen terug.
        /// Dit wordt gebruikt door de kassamedewerker.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> AfTeLeverenBestellingen()
        {
            DateTime startVanDag = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime eindeVanDag = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59); ;
            var afTeRekenenBestellingen = await _context
                .Bestellingen
                .Include(b => b.Klant)
                .Where(b => 
                    b.Status == Bestelling.EnumStatus.Besteld
                    &&
                    !b.Betaald
                    &&
                    b.Tijdslot >= startVanDag
                    &&
                    b.Tijdslot <= eindeVanDag
                    )
                .OrderBy(b => b.Tijdslot)
                .ToListAsync();

            return View(afTeRekenenBestellingen);
        }

        // GET: Bestelling/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bestelling = await _context.Bestellingen
                .Include(b => b.Klant)
                .Include(b => b.BesteldeGerechten)
                .ThenInclude(bg => bg.Gerecht)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bestelling == null)
            {
                return NotFound();
            }
            else
            {
                ViewData["Prijs"] = await BerekenTotalePrijsMetKorting(bestelling);
                return View(bestelling);
            }
        }

        // GET: Bestelling/Create
        public IActionResult Create()
        {
            Bestelling bestelling = new Bestelling()
            {
                Status = Bestelling.EnumStatus.Nieuw
            };
            ViewData["KlantId"] = new SelectList(_context.Klanten, "Id", "Email");
            return View(bestelling);
        }

        // POST: Bestelling/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Status,Betaald,KlantId,Tijdslot,Opmerkingen")] Bestelling bestelling)
        {
            await ControleerWijzigingen(null, bestelling);

            if (ModelState.IsValid)
            {
                _context.Add(bestelling);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Edit), new { id = bestelling.Id });
            }
            else
            {
                ViewData["KlantId"] = new SelectList(_context.Klanten, "Id", "Email", bestelling.KlantId);
                return View(bestelling);
            }
        }

        // GET: Bestelling/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bestelling = await _context.Bestellingen
                .Include(b => b.Klant)
                .Include(b => b.BesteldeGerechten)
                .ThenInclude(bg => bg.Gerecht)
                .Where(b => b.Id == id)
                .FirstOrDefaultAsync();

            if (bestelling == null)
            {
                return NotFound();
            }
            else
            {
                ViewData["GeldigeWijzigingen"] = bestelling.BepaalGeldigeWijzigingen();
                ViewData["GeldigeStatusovergangen"] = bestelling.BepaalGeldigeStatusovergangen();
                ViewData["KlantId"] = new SelectList(_context.Klanten, "Id", "Email", bestelling.KlantId);
                ViewData["Prijs"] = await BerekenTotalePrijsMetKorting(bestelling);
                return View(bestelling);
            }
        }

        // POST: Bestelling/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, 
            [Bind("Id,Status,Betaald,KlantId,Tijdslot,Opmerkingen")] Bestelling bestelling,
            [Bind("actie")] string actie)
        {
            if (id != bestelling.Id)
            {
                return NotFound();
            }

            // De vorige versie van de bestelling ophalen.
            Bestelling oudeBestelling = await _context.Bestellingen
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(b => b.Id == bestelling.Id);

            // Op de Edit.cshtml werden meerdere input's van het type 'submit' voorzien.
            // Elke submit heeft een eigen value die via deze switch omgezet wordt naar 
            // een specifieke status op de bestelling.
            switch (actie)
            {
                case ACTIE_BESTELLEN:
                    bestelling.Status = Bestelling.EnumStatus.Besteld;
                    break;
                case ACTIE_AFLEVEREN:
                    bestelling.Status = Bestelling.EnumStatus.Afgeleverd;
                    break;
                case ACTIE_ANNULEREN:
                    bestelling.Status = Bestelling.EnumStatus.Geannuleerd;
                    break;
                case ACTIE_BEWAREN:
                    // fallthrough naar 'default'.
                default:
                    // Indien geen actie, geen status wijziging.
                    // De 'oude' bestellingstatus wordt wel expliciet gezet omdat 'Status' mogelijk niet in de ge-POSTe form zit (cfr. disabled) OF omdat die gemanipuleerd zou kunnen zijn.
                    bestelling.Status = oudeBestelling.Status;
                    break;
            }

            // Het Bestelling-object dat aan deze action is 'gebind' is momenteel nog vrij leeg: er hangen geen child entiteiten onder.
            // Vandaar moeten deze dus uit de database opgehaald worden.
            bestelling.Klant = await (from k in _context.Klanten
                                      where k.Id == bestelling.KlantId
                                      select k).FirstAsync();
            bestelling.BesteldeGerechten = await (from bg in _context.BesteldeGerechten
                                                  .Include(bg => bg.Gerecht)
                                                  where bg.BestellingId == bestelling.Id
                                                  select bg).ToListAsync();

            // Controleren of eventuele wijzigingen en statusovergangen toegestaan zijn.
            await ControleerWijzigingen(oudeBestelling, bestelling);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bestelling);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BestellingExists(bestelling.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Edit), new { id = bestelling.Id });
            }
            else
            {
                ViewData["GeldigeWijzigingen"] = oudeBestelling.BepaalGeldigeWijzigingen(); // Opgelet: de geldige wijzigingen mogen enkel bepaald worden op basis van de vorige versie.
                ViewData["GeldigeStatusovergangen"] = oudeBestelling.BepaalGeldigeStatusovergangen(); // Opgelet: de geldige statussen mogen enkel bepaald worden op basis van de vorige versie.
                ViewData["KlantId"] = new SelectList(_context.Klanten, "Id", "Email", bestelling.KlantId);
                ViewData["Prijs"] = await BerekenTotalePrijsMetKorting(bestelling);

                bestelling.Status = oudeBestelling.Status; // Een eventuele aangevraagde statusovergang terug ongedaan maken.
                return View(bestelling);
            }
        }

        public async Task<IActionResult> Factuur(int id)
        {
            Bestelling bestelling = await (from b in _context.Bestellingen
                                    .Include(b => b.Klant)
                                    .Include(b => b.BesteldeGerechten)
                                    .ThenInclude(bg => bg.Gerecht)
                                           where b.Id == id
                                           select b).FirstOrDefaultAsync();
            if (bestelling == null)
            {
                return NotFound();
            }
            else
            {
                ViewData["Prijs"] = await BerekenTotalePrijsMetKorting(bestelling);
                return View(bestelling);
            }
        }

        private bool BestellingExists(int id)
        {
            return _context.Bestellingen.Any(e => e.Id == id);
        }

        #region Validates en berekeningen
        private async Task ControleerWijzigingen(Bestelling oudeBestelling, Bestelling nieuweBestelling)
        {
            // Controleer op geldigheid (verplicht, formaat, niet-wijzigbaar, ...) van velden.
            await ControleerWijzigingen_GeldigheidVelden(oudeBestelling, nieuweBestelling);

            // Controleer of een (eventuele) statusovergang toegelaten is.
            await ControleerWijzigingen_Statusovergang(oudeBestelling, nieuweBestelling);
        }

        private async Task ControleerWijzigingen_GeldigheidVelden(Bestelling oudeBestelling, Bestelling nieuweBestelling)
        {
            switch (nieuweBestelling.Status)
            {
                case Bestelling.EnumStatus.Nieuw:
                    if (nieuweBestelling.Betaald)
                    {
                        ModelState.AddModelError(nameof(Bestelling.Betaald), $"Een tijdslot kan enkel toegekend worden indien u bestelt of aflevert.");
                    }
                    if (nieuweBestelling.Tijdslot != null)
                    {
                        ModelState.AddModelError(nameof(Bestelling.Tijdslot), $"Een tijdslot kan enkel toegekend worden indien u bestelt.");
                    }
                    break;
                case Bestelling.EnumStatus.Besteld:
                    // Tijdslot afchecken
                    if (nieuweBestelling.Tijdslot == null)
                    {
                        ModelState.AddModelError(nameof(Bestelling.Tijdslot), $"Het tijdslot dient ingevuld te zijn.");
                    }
                    // Controleer of het een 'kwartier' betreft.
                    else if (nieuweBestelling.Tijdslot.Value.Minute % 15 != 0)
                    {
                        ModelState.AddModelError(nameof(Bestelling.Tijdslot), "Gelieve een kwartier te kiezen (0, 15, 30 of 45 minuten)");
                    }
                    // Controleer of er nog bestellingen mogelijk zijn op het gewenste tijdslot.
                    else if (await _context.Bestellingen
                            .CountAsync(b => b.Tijdslot == nieuweBestelling.Tijdslot && b.Id != nieuweBestelling.Id) >= 2)
                    {
                        ModelState.AddModelError(nameof(Bestelling.Tijdslot), $"Er is geen tijdslot beschikbaar op {nieuweBestelling.Tijdslot.Value}.");
                    }
                    // Gebeuren er geen wijzigingen in het laatste uur?
                    // Opgelet: dergelijke controle is ook nodig in BesteldGerechtController.
                    else if (
                        oudeBestelling != null
                        &&
                        oudeBestelling.Tijdslot != null
                        &&
                        oudeBestelling.Tijdslot.Value < DateTime.Now.AddHours(1)
                        &&
                        (
                            oudeBestelling.KlantId != nieuweBestelling.KlantId
                            ||
                            oudeBestelling.Opmerkingen != nieuweBestelling.Opmerkingen
                            ||
                            oudeBestelling.Tijdslot != nieuweBestelling.Tijdslot))
                    {
                        ModelState.AddModelError(nameof(Bestelling.Tijdslot), $"De bestelling kan niet meer gewijzigd worden.");
                    }

                    break;
                case Bestelling.EnumStatus.Afgeleverd:
                    if (!nieuweBestelling.Betaald)
                    {
                        ModelState.AddModelError(nameof(Bestelling.Betaald), $"De bestelling moet betaald worden.");
                    }
                    break;
                case Bestelling.EnumStatus.Geannuleerd:
                    break;
            }
        }

        private async Task ControleerWijzigingen_Statusovergang(Bestelling oudeBestelling, Bestelling nieuweBestelling)
        {
            Bestelling.EnumStatus oudeBestellingStatus = oudeBestelling == null ? Bestelling.EnumStatus.Nieuw : oudeBestelling.Status;

            // Eerst controleren of er wel degelijk een statusovergang gebeurt.
            if (oudeBestellingStatus != nieuweBestelling.Status)
            {
                // Is de statusovergang toegelaten?
                if (!oudeBestelling.BepaalGeldigeStatusovergangen().Contains(nieuweBestelling.Status))
                {
                    ModelState.AddModelError(nameof(Bestelling.Status), $"De statusovergang van {oudeBestelling.Status} naar {nieuweBestelling.Status} is niet toegelaten.");
                }
                else
                {
                    // Indien de statusovergang toegelaten is: nog eventuele status-specifieke controles uitvoeren.
                    switch (nieuweBestelling.Status)
                    {
                        case Bestelling.EnumStatus.Besteld:
                            // Een bestelling kan enkel geplaatst worden voor een tijdslot in de toekomst.
                            if (nieuweBestelling.Tijdslot != null && nieuweBestelling.Tijdslot.Value < DateTime.Now)
                            {
                                ModelState.AddModelError(nameof(Bestelling.Tijdslot), $"Het tijdslot ligt in het verleden.");
                            }

                            // Een bestelling kan pas besteld worden wanneer er minstens één gerecht besteld is.
                            if (nieuweBestelling.BesteldeGerechten.Count == 0)
                            {
                                ModelState.AddModelError(nameof(Bestelling.Status), $"Gelieve minstens één gerecht toe te voegen.");
                            }

                            break;
                        case Bestelling.EnumStatus.Geannuleerd:
                            // Gebeurt er geen annulatie in de laatste 2 uur?
                            if (oudeBestelling.Status != Bestelling.EnumStatus.Nieuw && nieuweBestelling.Tijdslot.Value < DateTime.Now.AddHours(2))
                            {
                                ModelState.AddModelError(nameof(Bestelling.Status), $"De bestelling kan niet meer geannuleerd worden.");
                            }
                            break;
                    }
                }
            }
        }


        private async Task<decimal> BerekenTotalePrijsMetKorting(Bestelling bestelling)
        {
            decimal totalePrijsMetKorting = bestelling.TotalePrijsZonderKorting;

            int aantalBetaaldeBestellingen = await _context.Bestellingen
                .CountAsync(b => b.KlantId == bestelling.KlantId
                    &&
                    b.Status != Bestelling.EnumStatus.Nieuw
                    &&
                    b.Status != Bestelling.EnumStatus.Geannuleerd
                    &&
                    b.Betaald);

            Korting maximaleKorting = await _context.Kortingen
                .Where(k => aantalBetaaldeBestellingen >= k.AantalBestellingen)
                // De beste korting staat bovenaan :)
                .OrderByDescending(k => k.Percentage)
                .FirstOrDefaultAsync();

            if (maximaleKorting != null)
            {
                totalePrijsMetKorting -= (totalePrijsMetKorting * maximaleKorting.Percentage * 0.01m); // Korting is in de range 0-100. 
            }

            return totalePrijsMetKorting;
        }
        #endregion
    }
}

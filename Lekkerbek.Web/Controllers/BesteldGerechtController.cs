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
    public class BesteldGerechtController : Controller
    {
        private readonly LekkerbekDbContext _context;

        public BesteldGerechtController(LekkerbekDbContext context)
        {
            _context = context;
        }

        // GET: BesteldGerecht/Create
        public IActionResult Create(int bestellingId)
        {
            ViewData["BestellingId"] = new SelectList(
                _context.Bestellingen, 
                nameof(Bestelling.Id), 
                nameof(Bestelling.Id));
            ViewData["GerechtId"] = new SelectList(
                FilterEnSorteerGerechten(bestellingId, null).Select(g => new
                {
                    Id = g.Id,
                    SamengesteldeNaam = $"{g.Categorie} - {g.Naam}"
                }), 
                "Id", 
                "SamengesteldeNaam");

            // BestelGerecht object met 'defaults'
            BesteldGerecht bg = new BesteldGerecht()
            {
                BestellingId = bestellingId,
                Bestelling = _context.Bestellingen.First(b => b.Id == bestellingId),
                Aantal = 1
            };

            return View(bg);
        }

        // POST: BesteldGerecht/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BestellingId,GerechtId,Aantal,Opmerkingen")] BesteldGerecht besteldGerecht)
        {
            if (ModelState.IsValid)
            {
                _context.Add(besteldGerecht);
                await _context.SaveChangesAsync();

                // Terug naar de bestelling waarvan dit bestelde gerecht deel uitmaakt.
                return RedirectToAction(
                    nameof(BestellingController.Edit),
                    nameof(BestellingController).Replace("Controller", ""),
                    new { id = besteldGerecht.BestellingId });
            }
            else
            {
                ViewData["BestellingId"] = new SelectList(
                    _context.Bestellingen,
                    nameof(Bestelling.Id),
                    nameof(Bestelling.Id),
                    besteldGerecht.BestellingId);
                ViewData["GerechtId"] = new SelectList(
                    FilterEnSorteerGerechten(besteldGerecht.BestellingId, null).Select(g => new
                    {
                        Id = g.Id,
                        SamengesteldeNaam = $"{g.Categorie} - {g.Naam}"
                    }),
                    "Id",
                    "SamengesteldNaam",
                    besteldGerecht.GerechtId);
                return View(besteldGerecht);
            }
        }

        // GET: BesteldGerecht/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var besteldGerecht = await _context.BesteldeGerechten
                .Include(b => b.Bestelling)
                .Include(b => b.Gerecht)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (besteldGerecht == null)
            {
                return NotFound();
            }

            return View(besteldGerecht);
        }


        // GET: BesteldGerecht/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var besteldGerecht = await _context.BesteldeGerechten.FindAsync(id);
            if (besteldGerecht == null)
            {
                return NotFound();
            }
            ViewData["BestellingId"] = new SelectList(
                _context.Bestellingen, 
                "Id", 
                "Id", 
                besteldGerecht.BestellingId);

            // Reeds bestelde gerechten mogen niet opnieuw in de lijst getoond worden.
            ViewData["GerechtId"] = new SelectList(
                FilterEnSorteerGerechten(besteldGerecht.BestellingId, besteldGerecht.GerechtId).Select(g => new
                {
                    Id = g.Id,
                    SamengesteldeNaam = $"{g.Categorie} - {g.Naam}"
                }),
                "Id",
                "SamengesteldeNaam", 
                besteldGerecht.GerechtId);
            return View(besteldGerecht);
        }

        // POST: BesteldGerecht/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BestellingId,GerechtId,Aantal,Opmerkingen")] BesteldGerecht besteldGerecht)
        {
            if (id != besteldGerecht.Id)
            {
                return NotFound();
            }

            ControleerWijzigbaarheid(besteldGerecht);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(besteldGerecht);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BesteldGerechtExists(besteldGerecht.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // Terug naar de bestelling waarvan dit bestelde gerecht deel uitmaakt.
                return RedirectToAction(
                    nameof(BestellingController.Edit),
                    nameof(BestellingController).Replace("Controller", ""),
                    new { id = besteldGerecht.BestellingId });
            }
            else
            {
                ViewData["BestellingId"] = new SelectList(
                    _context.Bestellingen,
                    "Id",
                    "Id",
                    besteldGerecht.BestellingId);
                ViewData["GerechtId"] = new SelectList(
                    FilterEnSorteerGerechten(besteldGerecht.BestellingId, besteldGerecht.GerechtId).Select(g => new
                    {
                        Id = g.Id,
                        SamengesteldeNaam = $"{g.Categorie} - {g.Naam}"
                    }),
                    "Id",
                    "SamengesteldeNaam",
                    besteldGerecht.GerechtId);

                // Terug naar de bestelling waarvan dit bestelde gerecht deel uitmaakt.
                return View(besteldGerecht);
            }
        }

        // GET: BesteldGerecht/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var besteldGerecht = await _context.BesteldeGerechten
                .Include(b => b.Bestelling)
                .Include(b => b.Gerecht)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (besteldGerecht == null)
            {
                return NotFound();
            }

            return View(besteldGerecht);
        }

        // POST: BesteldGerecht/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var besteldGerecht = await _context
                .BesteldeGerechten
                .FindAsync(id);

            ControleerWijzigbaarheid(besteldGerecht);

            if (ModelState.IsValid)
            {
                _context.BesteldeGerechten.Remove(besteldGerecht);
                await _context.SaveChangesAsync();

                // Terug naar de bestelling waarvan dit bestelde gerecht deel uitmaakte.
                return RedirectToAction(
                    nameof(BestellingController.Edit),
                    nameof(BestellingController).Replace("Controller", ""),
                    new { id = besteldGerecht.BestellingId });
            }
            else return BadRequest("Besteld gerecht kan niet verwijderd worden.");
        }

        private bool BesteldGerechtExists(int id)
        {
            return _context.BesteldeGerechten.Any(e => e.Id == id);
        }

        /// <summary>
        /// Gerechten die al toegevoegd werden aan de bestelling mogen niet opnieuw getoond worden in de lijst van bestelbare gerechten.
        /// </summary>
        /// <param name="bestellingId"></param>
        /// <param name="gerechtId">Optionele parameter: nodig voor het edit scherm -> in het edit scherm moet je natuurlijk ook het gerecht kunnen selecteren/zien dat werd toegekend.</param>
        /// <returns></returns>
        private List<Gerecht> FilterEnSorteerGerechten(int bestellingId, int? gerechtId) 
        {
            var reedsBesteldeGerechten =
                from b in _context.Bestellingen
                join bg in _context.BesteldeGerechten on b.Id equals bg.BestellingId
                join g in _context.Gerechten on bg.GerechtId equals g.Id
                where b.Id == bestellingId && (gerechtId == null || g.Id != gerechtId)
                select g;

            return _context
                .Gerechten
                .Except(reedsBesteldeGerechten)
                .OrderBy(g => g.Categorie)
                .ThenBy(g => g.Naam)
                .ToList();
        }

        private void ControleerWijzigbaarheid(BesteldGerecht besteldGerecht)
        {
            // Een (eventuele) vorige versie van de bestelling ophalen.
            Bestelling bestelling = _context.Bestellingen
                .AsNoTracking()
                .FirstOrDefault(b => b.Id == besteldGerecht.BestellingId);

            if (bestelling.Tijdslot != null && bestelling.Tijdslot.Value < DateTime.Now.AddHours(1))
            {
                ModelState.AddModelError(nameof(Bestelling.Tijdslot), $"De bestelling kan niet meer gewijzigd worden.");
            }
        }

    }
}

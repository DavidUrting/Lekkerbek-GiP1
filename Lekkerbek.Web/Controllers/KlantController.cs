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
    public class KlantController : Controller
    {
        private readonly LekkerbekDbContext _context;

        public KlantController(LekkerbekDbContext context)
        {
            _context = context;
        }

        // GET: overzicht van alle klanten
        public async Task<IActionResult> Index()
        {
            return View(await _context.Klanten.ToListAsync());
        }

        // GET: Klant/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var klant = await _context.Klanten
                .FirstOrDefaultAsync(m => m.Id == id);
            if (klant == null)
            {
                return NotFound();
            }

            return View(klant);
        }

        // GET: Klant/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Klant/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Naam,StraatEnNummer,Postcode,Woonplaats,Telefoonnummer,Email,Opmerkingen")] Klant klant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(klant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(klant);
        }

        // GET: Klant/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var klant = await _context.Klanten.FindAsync(id);
            if (klant == null)
            {
                return NotFound();
            }
            return View(klant);
        }

        // POST: Klant/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naam,StraatEnNummer,Postcode,Woonplaats,Telefoonnummer,Email,Opmerkingen")] Klant klant)
        {
            if (id != klant.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(klant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KlantExists(klant.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(klant);
        }

        // GET: Klant/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var klant = await _context.Klanten
                .FirstOrDefaultAsync(m => m.Id == id);
            if (klant == null)
            {
                return NotFound();
            }

            ViewData["AantalBestellingen"] = AantalBestellingenGekoppeldAanKlant(id.Value);

            return View(klant);
        }

        // POST: Klant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (AantalBestellingenGekoppeldAanKlant(id) > 0)
            {
                return BadRequest($"Klant kan niet verwijderd worden want deze is gekoppeld aan bestellingen.");
            }

            var klant = await _context.Klanten.FindAsync(id);
            _context.Klanten.Remove(klant);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KlantExists(int id)
        {
            return _context.Klanten.Any(e => e.Id == id);
        }

        private int AantalBestellingenGekoppeldAanKlant(int id)
        {
            return _context.BesteldeGerechten
                .Count(bg => bg.Bestelling.KlantId == id);
        }
    }
}

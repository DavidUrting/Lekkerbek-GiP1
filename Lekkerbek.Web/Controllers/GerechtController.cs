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
    public class GerechtController : Controller
    {
        private readonly LekkerbekDbContext _context;

        public GerechtController(LekkerbekDbContext context)
        {
            _context = context;
        }

        // GET: Gerecht
        public async Task<IActionResult> Index()
        {
            return View(await _context
                .Gerechten
                .OrderBy(g => g.Categorie)
                .ThenBy(g => g.Naam)
                .ToListAsync());
        }

        // GET: Gerecht/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gerecht = await _context.Gerechten
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gerecht == null)
            {
                return NotFound();
            }

            return View(gerecht);
        }

        // GET: Gerecht/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Gerecht/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Categorie,Naam,Omschrijving,Prijs")] Gerecht gerecht)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gerecht);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gerecht);
        }

        // GET: Gerecht/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gerecht = await _context.Gerechten.FindAsync(id);
            if (gerecht == null)
            {
                return NotFound();
            }
            return View(gerecht);
        }

        // POST: Gerecht/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Categorie,Naam,Omschrijving,Prijs")] Gerecht gerecht)
        {
            if (id != gerecht.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gerecht);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GerechtExists(gerecht.Id))
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
            return View(gerecht);
        }

        // GET: Gerecht/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gerecht = await _context.Gerechten
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gerecht == null)
            {
                return NotFound();
            }

            ViewData["AantalBestellingen"] = AantalBestellingenWaarinGerechtGebruiktWordt(id.Value);

            return View(gerecht);
        }

        // POST: Gerecht/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (AantalBestellingenWaarinGerechtGebruiktWordt(id) > 0)
            {
                return BadRequest($"Gerecht kan niet verwijderd worden want deze is gekoppeld aan bestellingen.");
            }

            var gerecht = await _context.Gerechten.FindAsync(id);
            _context.Gerechten.Remove(gerecht);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GerechtExists(int id)
        {
            return _context.Gerechten.Any(e => e.Id == id);
        }

        private int AantalBestellingenWaarinGerechtGebruiktWordt(int id)
        {
            return _context.BesteldeGerechten
                .Count(bg => bg.GerechtId == id);
        }
    }
}

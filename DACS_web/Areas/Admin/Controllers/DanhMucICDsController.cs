using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DACS_web.Models;
using Microsoft.AspNetCore.Authorization;

namespace DACS_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class DanhMucICDsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DanhMucICDsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DanhMucICDs
        public async Task<IActionResult> Index()
        {
            return View(await _context.DanhMucICDs.ToListAsync());
        }

        // GET: DanhMucICDs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var icd = await _context.DanhMucICDs
                .FirstOrDefaultAsync(m => m.MaICD == id);

            if (icd == null) return NotFound();

            return View(icd);
        }

        // GET: DanhMucICDs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DanhMucICDs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DanhMucICD icd)
        {
            if (ModelState.IsValid)
            {
                _context.Add(icd);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(icd);
        }

        // GET: DanhMucICDs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var icd = await _context.DanhMucICDs.FindAsync(id);
            if (icd == null) return NotFound();

            return View(icd);
        }

        // POST: DanhMucICDs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, DanhMucICD icd)
        {
            if (id != icd.MaICD) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(icd);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DanhMucICDExists(icd.MaICD)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(icd);
        }

        // GET: DanhMucICDs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var icd = await _context.DanhMucICDs
                .FirstOrDefaultAsync(m => m.MaICD == id);

            if (icd == null) return NotFound();

            return View(icd);
        }

        // POST: DanhMucICDs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var icd = await _context.DanhMucICDs.FindAsync(id);
            if (icd != null)
            {
                _context.DanhMucICDs.Remove(icd);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DanhMucICDExists(string id)
        {
            return _context.DanhMucICDs.Any(e => e.MaICD == id);
        }
    }
}

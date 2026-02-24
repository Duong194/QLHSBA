using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DACS_web.Models;
using Microsoft.AspNetCore.Authorization;

namespace DACS_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)] // có thể thêm [Authorize(Roles = "Admin")] nếu muốn giới hạn
    public class ThuocsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThuocsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Thuocs
        public async Task<IActionResult> Index()
        {
            var thuocs = await _context.Thuocs.ToListAsync();
            return View(thuocs);
        }

        // GET: Thuocs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Thuocs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Thuoc thuoc)
        {
            if (ModelState.IsValid)
            {
                // sinh mã thuốc nếu cần
                if (string.IsNullOrEmpty(thuoc.MaThuoc))
                {
                    var last = await _context.Thuocs
                        .OrderByDescending(t => t.MaThuoc)
                        .Select(t => t.MaThuoc)
                        .FirstOrDefaultAsync();

                    int nextId = 1;
                    if (!string.IsNullOrEmpty(last) && last.StartsWith("T"))
                    {
                        int.TryParse(last.Substring(1), out nextId);
                        nextId++;
                    }

                    thuoc.MaThuoc = $"T{nextId:D3}";
                }

                _context.Add(thuoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(thuoc);
        }

        // GET: Thuocs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var thuoc = await _context.Thuocs.FindAsync(id);
            if (thuoc == null) return NotFound();

            return View(thuoc);
        }

        // POST: Thuocs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Thuoc thuoc)
        {
            if (id != thuoc.MaThuoc) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(thuoc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Thuocs.Any(t => t.MaThuoc == thuoc.MaThuoc))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(thuoc);
        }

        // GET: Thuocs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var thuoc = await _context.Thuocs.FirstOrDefaultAsync(t => t.MaThuoc == id);
            if (thuoc == null) return NotFound();

            return View(thuoc);
        }

        // POST: Thuocs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var thuoc = await _context.Thuocs.FindAsync(id);
            if (thuoc != null)
            {
                _context.Thuocs.Remove(thuoc);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

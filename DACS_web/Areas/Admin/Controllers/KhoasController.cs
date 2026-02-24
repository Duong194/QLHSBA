using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DACS_web.Models;
using Microsoft.AspNetCore.Authorization;

namespace DACS_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class KhoasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KhoasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Khoas
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var khoaQuery = from k in _context.Khoas
                            select k;

            if (!String.IsNullOrEmpty(searchString))
            {
                khoaQuery = khoaQuery.Where(k => k.TenKhoa.Contains(searchString));
            }

            return View(await khoaQuery.ToListAsync());
        }

        // GET: Khoas/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var khoa = await _context.Khoas.FirstOrDefaultAsync(m => m.MaKhoa == id);
            if (khoa == null)
                return NotFound();

            return View(khoa);
        }

        // GET: Khoas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Khoas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Khoa khoa)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Sinh mã tự động KHxxx
                    var maxMa = await _context.Khoas
                        .OrderByDescending(k => k.MaKhoa)
                        .Select(k => k.MaKhoa)
                        .FirstOrDefaultAsync();

                    int nextId = 1;
                    if (!string.IsNullOrEmpty(maxMa) && maxMa.Length >= 5)
                    {
                        int.TryParse(maxMa.Substring(2), out nextId);
                        nextId++;
                    }

                    khoa.MaKhoa = $"KH{nextId:D3}"; // Ví dụ: KH001, KH002

                    _context.Khoas.Add(khoa);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Lỗi lưu database: {ex.Message}");
                }
            }
            return View(khoa);
        }


        // GET: Khoas/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var khoa = await _context.Khoas.FindAsync(id);
            if (khoa == null)
                return NotFound();

            return View(khoa);
        }

        // POST: Khoas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Khoa khoa)
        {
            if (id != khoa.MaKhoa)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khoa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhoaExists(khoa.MaKhoa))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(khoa);
        }

        // GET: Khoas/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var khoa = await _context.Khoas.FirstOrDefaultAsync(m => m.MaKhoa == id);
            if (khoa == null)
                return NotFound();

            return View(khoa);
        }

        // POST: Khoas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var khoa = await _context.Khoas.FindAsync(id);
            if (khoa != null)
            {
                _context.Khoas.Remove(khoa);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool KhoaExists(string id)
        {
            return _context.Khoas.Any(e => e.MaKhoa == id);
        }
    }
}

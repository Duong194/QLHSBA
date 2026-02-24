using DACS_web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DACS_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PhuongPhapDieuTrisController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhuongPhapDieuTrisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PhuongPhapDieuTris
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BenhNhan + "," + SD.Role_BacSi + "," + SD.Role_YTa)]

        public async Task<IActionResult> Index()
        {
            var data = _context.PhuongPhapDieuTris.Include(p => p.HoSoBenhAn);
            return View(await data.ToListAsync());
        }

        // GET: PhuongPhapDieuTris/Create
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        public IActionResult Create(string maHoSo)
        {
            var phuongPhapDieuTri = new PhuongPhapDieuTri
            {
                MaHoSo = maHoSo // Gán sẵn mã hồ sơ vào Model
            };
            return View();
        }

        // POST: PhuongPhapDieuTris/Create
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhuongPhapDieuTri phuongPhapDieuTri)
        {
            if (ModelState.IsValid)
            {
                // Sinh mã tự động nếu cần (nếu MaPhuongPhap là string và cần sinh)
                var last = await _context.PhuongPhapDieuTris.OrderByDescending(p => p.MaPhuongPhap).FirstOrDefaultAsync();
                int nextId = 1;
                if (last != null && int.TryParse(last.MaPhuongPhap?.Substring(2), out int lastId))
                {
                    nextId = lastId + 1;
                }
                phuongPhapDieuTri.MaPhuongPhap = "PP" + nextId.ToString("D4");

                _context.Add(phuongPhapDieuTri);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "HoSoBenhAns", new { id = phuongPhapDieuTri.MaHoSo });
            }
            ViewData["MaHoSo"] = new SelectList(_context.HoSoBenhAns, "MaHoSo", "MaHoSo", phuongPhapDieuTri.MaHoSo);
            return View(phuongPhapDieuTri);
        }

        // GET: PhuongPhapDieuTris/Edit/5
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var ppdt = await _context.PhuongPhapDieuTris.FindAsync(id);
            if (ppdt == null) return NotFound();

            ViewData["MaHoSo"] = new SelectList(_context.HoSoBenhAns, "MaHoSo", "MaHoSo", ppdt.MaHoSo);
            ViewBag.MaHoSo = ppdt.MaHoSo;
            return View(ppdt);
        }

        // POST: PhuongPhapDieuTris/Edit/5
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, PhuongPhapDieuTri phuongPhapDieuTri)
        {
            if (id != phuongPhapDieuTri.MaPhuongPhap) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phuongPhapDieuTri);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhuongPhapDieuTriExists(phuongPhapDieuTri.MaPhuongPhap))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Details", "HoSoBenhAns", new { id = phuongPhapDieuTri.MaHoSo });
            }
            ViewData["MaHoSo"] = new SelectList(_context.HoSoBenhAns, "MaHoSo", "MaHoSo", phuongPhapDieuTri.MaHoSo);
            return View(phuongPhapDieuTri);
        }

        // GET: PhuongPhapDieuTris/Details/5
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BenhNhan + "," + SD.Role_BacSi + "," + SD.Role_YTa)]

        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var ppdt = await _context.PhuongPhapDieuTris
                            .Include(p => p.HoSoBenhAn)
                            .FirstOrDefaultAsync(m => m.MaPhuongPhap == id);

            if (ppdt == null) return NotFound();

            return View(ppdt);
        }

        // GET: PhuongPhapDieuTris/Delete/5
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var ppdt = await _context.PhuongPhapDieuTris
                            .Include(p => p.HoSoBenhAn)
                            .FirstOrDefaultAsync(m => m.MaPhuongPhap == id);

            if (ppdt == null) return NotFound();
            ViewBag.MaHoSo = ppdt.MaHoSo;

            return View(ppdt);
        }

        // POST: PhuongPhapDieuTris/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var ppdt = await _context.PhuongPhapDieuTris.FindAsync(id);
            if (ppdt != null)
            {
                _context.PhuongPhapDieuTris.Remove(ppdt);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", "HoSoBenhAns", new { id = ppdt.MaHoSo });
        }

        private bool PhuongPhapDieuTriExists(string id)
        {
            return _context.PhuongPhapDieuTris.Any(e => e.MaPhuongPhap == id);
        }
    }
}

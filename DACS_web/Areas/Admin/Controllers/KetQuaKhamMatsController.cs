using DACS_web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DACS_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class KetQuaKhamMatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KetQuaKhamMatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: KetQuaKhamMats
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BenhNhan + "," + SD.Role_BacSi + "," + SD.Role_YTa)]

        public async Task<IActionResult> Index()
        {
            var results = _context.KetQuaKhamMats.Include(k => k.HoSoBenhAn);
            return View(await results.ToListAsync());
        }

        // GET: KetQuaKhamMats/Create
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        public IActionResult Create(string maHoSo)
        {
            var ketQua = new KetQuaKhamMat
            {
                MaHoSo = maHoSo
            };
            return View(ketQua);
        }

        // POST: KetQuaKhamMats/Create
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KetQuaKhamMat ketQuaKhamMat)
        {
            if (ModelState.IsValid)
            {
                // Sinh mã MaKQ tự động tăng
                var lastKQ = await _context.KetQuaKhamMats
                                    .OrderByDescending(k => k.MaKQ)
                                    .FirstOrDefaultAsync();

                int nextId = 1; // Nếu bảng trống
                if (lastKQ != null && int.TryParse(lastKQ.MaKQ?.Substring(2), out int lastId))
                {
                    nextId = lastId + 1;
                }

                ketQuaKhamMat.MaKQ = "KQ" + nextId.ToString("D4"); // Ví dụ: KQ0001, KQ0002...

                _context.Add(ketQuaKhamMat);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "HoSoBenhAns", new { id = ketQuaKhamMat.MaHoSo });
            }

            ViewData["MaHoSo"] = new SelectList(_context.HoSoBenhAns, "MaHoSo", "MaHoSo", ketQuaKhamMat.MaHoSo);
            return View(ketQuaKhamMat);
        }


        // GET: KetQuaKhamMats/Edit/5
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var ketQua = await _context.KetQuaKhamMats.FindAsync(id);
            if (ketQua == null) return NotFound();

            ViewData["MaHoSo"] = new SelectList(_context.HoSoBenhAns, "MaHoSo", "MaHoSo", ketQua.MaHoSo);
            ViewBag.MaHoSo = ketQua.MaHoSo;
            return View(ketQua);
        }

        // POST: KetQuaKhamMats/Edit/5
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, KetQuaKhamMat ketQuaKhamMat)
        {
            if (id != ketQuaKhamMat.MaKQ) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ketQuaKhamMat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KetQuaKhamMatExists(ketQuaKhamMat.MaKQ))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Details", "HoSoBenhAns", new { id = ketQuaKhamMat.MaHoSo });
            }
            ViewData["MaHoSo"] = new SelectList(_context.HoSoBenhAns, "MaHoSo", "MaHoSo", ketQuaKhamMat.MaHoSo);
            return View(ketQuaKhamMat);
        }

        // GET: KetQuaKhamMats/Details/5
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BenhNhan + "," + SD.Role_BacSi + "," + SD.Role_YTa)]

        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var ketQua = await _context.KetQuaKhamMats
                            .Include(k => k.HoSoBenhAn)
                            .FirstOrDefaultAsync(m => m.MaKQ == id);

            if (ketQua == null) return NotFound();

            return View(ketQua);
        }

        // GET: KetQuaKhamMats/Delete/5
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var ketQua = await _context.KetQuaKhamMats
                            .Include(k => k.HoSoBenhAn)
                            .FirstOrDefaultAsync(m => m.MaKQ == id);

            if (ketQua == null) return NotFound();
            ViewBag.MaHoSo = ketQua.MaHoSo;

            return View(ketQua);
        }

        // POST: KetQuaKhamMats/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var ketQua = await _context.KetQuaKhamMats.FindAsync(id);
            if (ketQua != null)
            {
                _context.KetQuaKhamMats.Remove(ketQua);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", "HoSoBenhAns", new { id = ketQua.MaHoSo });
        }

        private bool KetQuaKhamMatExists(string id)
        {
            return _context.KetQuaKhamMats.Any(e => e.MaKQ == id);
        }
    }
}

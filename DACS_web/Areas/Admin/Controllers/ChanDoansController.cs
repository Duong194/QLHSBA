using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DACS_web.Models;
using Microsoft.AspNetCore.Authorization;

namespace DACS_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ChanDoansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChanDoansController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BenhNhan + "," + SD.Role_BacSi + "," + SD.Role_YTa)]

        // GET: ChanDoans
        public async Task<IActionResult> Index()
        {
            var chanDoans = _context.ChanDoans
                .Include(c => c.HoSoBenhAn)
                .Include(c => c.DanhMucICD);
            return View(await chanDoans.ToListAsync());
        }
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BenhNhan + "," + SD.Role_BacSi + "," + SD.Role_YTa)]

        // GET: ChanDoans/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var chanDoan = await _context.ChanDoans
                .Include(c => c.HoSoBenhAn)
                .Include(c => c.DanhMucICD)
                .FirstOrDefaultAsync(m => m.MaChanDoan == id);

            if (chanDoan == null) return NotFound();

            return View(chanDoan);
        }
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        // GET: ChanDoans/Create
        public IActionResult Create()
        {
            ViewData["MaHoSo"] = new SelectList(_context.HoSoBenhAns, "MaHoSo", "MaHoSo");
            ViewData["MaICD"] = new SelectList(_context.DanhMucICDs, "MaICD", "TenBenh");
            return View();
        }

        // POST: ChanDoans/Create
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChanDoan chanDoan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chanDoan);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "HoSoBenhAns", new { id = chanDoan.MaHoSo });
            }
            ViewData["MaHoSo"] = new SelectList(_context.HoSoBenhAns, "MaHoSo", "MaHoSo", chanDoan.MaHoSo);
            ViewData["MaICD"] = new SelectList(_context.DanhMucICDs, "MaICD", "TenBenh", chanDoan.MaICD);
            return View(chanDoan);
        }

        // GET: ChanDoans/Edit/5
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var chanDoan = await _context.ChanDoans.FindAsync(id);
            if (chanDoan == null) return NotFound();

            ViewData["MaHoSo"] = new SelectList(_context.HoSoBenhAns, "MaHoSo", "MaHoSo", chanDoan.MaHoSo);
            ViewData["MaICD"] = new SelectList(_context.DanhMucICDs, "MaICD", "TenBenh", chanDoan.MaICD);
            ViewBag.MaHoSo = chanDoan.MaHoSo;
            return View(chanDoan);
        }

        // POST: ChanDoans/Edit/5
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ChanDoan chanDoan)
        {
            if (id != chanDoan.MaChanDoan) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chanDoan);
                    await _context.SaveChangesAsync();


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChanDoanExists(chanDoan.MaChanDoan)) return NotFound();
                    else throw;
                }
                return RedirectToAction("Details", "HoSoBenhAns", new { id = chanDoan.MaHoSo });
            }
            ViewData["MaHoSo"] = new SelectList(_context.HoSoBenhAns, "MaHoSo", "MaHoSo", chanDoan.MaHoSo);
            ViewData["MaICD"] = new SelectList(_context.DanhMucICDs, "MaICD", "TenBenh", chanDoan.MaICD);
            return View(chanDoan);
        }

        // GET: ChanDoans/Delete/5
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var chanDoan = await _context.ChanDoans
                .Include(c => c.HoSoBenhAn)
                .Include(c => c.DanhMucICD)
                .FirstOrDefaultAsync(m => m.MaChanDoan == id);

            if (chanDoan == null) return NotFound();
            ViewBag.MaHoSo = chanDoan.MaHoSo;
            return View(chanDoan);
        }

        // POST: ChanDoans/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var chanDoan = await _context.ChanDoans.FindAsync(id);
            if (chanDoan != null)
            {
                _context.ChanDoans.Remove(chanDoan);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", "HoSoBenhAns", new { id = chanDoan.MaHoSo });
        }

        private bool ChanDoanExists(string id)
        {
            return _context.ChanDoans.Any(e => e.MaChanDoan == id);
        }

        // ✅✅ Thêm mới:

        // GET: ChanDoans/CreateMultiple
        // GET: ChanDoans/CreateMultiple
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        public IActionResult CreateMultiple(string maHoSo)
        {
            var model = new CreateMultipleChanDoan
            {
                MaHoSo = maHoSo
            };

            ViewBag.DanhMucICD = _context.DanhMucICDs
                .Select(x => new SelectListItem
                {
                    Value = x.MaICD,
                    Text = x.TenBenh
                }).ToList();

            return View(model);
        }

        // POST: ChanDoans/CreateMultiple
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMultiple(CreateMultipleChanDoan model)
        {
            if (string.IsNullOrEmpty(model.MaHoSo))
            {
                ModelState.AddModelError("MaHoSo", "Thiếu mã hồ sơ");
                return View(model);
            }

            var lastChanDoan = await _context.ChanDoans
                                     .OrderByDescending(c => c.MaChanDoan)
                                     .FirstOrDefaultAsync();

            int nextId = 1;
            if (lastChanDoan != null && int.TryParse(lastChanDoan.MaChanDoan?.Substring(2), out int lastId))
            {
                nextId = lastId + 1;
            }

            for (int i = 0; i < model.LoaiChanDoans.Length; i++)
            {
                var chanDoan = new ChanDoan
                {
                    MaChanDoan = "CD" + nextId.ToString("D4"),
                    MaHoSo = model.MaHoSo,
                    LoaiChanDoan = model.LoaiChanDoans[i],
                    MaICD = model.MaICDs[i],
                    ViTri = model.ViTris[i],
                    MoTa = model.MoTas[i]
                };
                _context.ChanDoans.Add(chanDoan);
                nextId++;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "HoSoBenhAns", new { id = model.MaHoSo });
        }


    }
}

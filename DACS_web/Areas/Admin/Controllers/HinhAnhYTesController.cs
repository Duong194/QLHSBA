using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DACS_web.Models;
using Microsoft.AspNetCore.Authorization;

namespace DACS_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HinhAnhYTesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HinhAnhYTesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: HinhAnhYTes
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BenhNhan + "," + SD.Role_BacSi + "," + SD.Role_YTa)]

        public async Task<IActionResult> Index()
        {
            var hinhAnhs = _context.HinhAnhYTes.Include(h => h.HoSoBenhAn);
            return View(await hinhAnhs.ToListAsync());
        }

        // GET: HinhAnhYTes/Create
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        public IActionResult Create(string mahoso)
        {
            var hinhanh = new HinhAnhYTe
            {
                MaHoSo = mahoso
            };
            return View(hinhanh);
        }

        // POST: HinhAnhYTes/Create
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HinhAnhYTe hinhAnh, IFormFile uploadFile)
        {
            if (ModelState.IsValid)
            {
                if (uploadFile != null && uploadFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(uploadFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadFile.CopyToAsync(fileStream);
                    }

                    hinhAnh.FileURL = "/uploads/" + uniqueFileName;
                }

                // Sinh mã MaAnh tự động tăng dần (không random nữa)
                var lastHinhAnh = await _context.HinhAnhYTes
                                        .OrderByDescending(h => h.MaAnh)
                                        .FirstOrDefaultAsync();

                int nextId = 1;
                if (lastHinhAnh != null && int.TryParse(lastHinhAnh.MaAnh?.Substring(2), out int lastId))
                {
                    nextId = lastId + 1;
                }
                hinhAnh.MaAnh = "HA" + nextId.ToString("D4"); // Ví dụ: HA0001, HA0002, HA0003

                _context.Add(hinhAnh);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "HoSoBenhAns", new { id = hinhAnh.MaHoSo });

            }

            ViewData["MaHoSo"] = new SelectList(_context.HoSoBenhAns, "MaHoSo", "MaHoSo", hinhAnh.MaHoSo);
            return View(hinhAnh);
        }


        // GET: HinhAnhYTes/Edit/5
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var hinhAnh = await _context.HinhAnhYTes.FindAsync(id);
            if (hinhAnh == null) return NotFound();

            ViewData["MaHoSo"] = new SelectList(_context.HoSoBenhAns, "MaHoSo", "MaHoSo", hinhAnh.MaHoSo);
            ViewBag.MaHoSo = hinhAnh.MaHoSo;
            return View(hinhAnh);
        }

        // POST: HinhAnhYTes/Edit/5
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, HinhAnhYTe hinhAnh, IFormFile uploadFile)
        {
            if (id != hinhAnh.MaAnh) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (uploadFile != null && uploadFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                        Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(uploadFile.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await uploadFile.CopyToAsync(fileStream);
                        }

                        hinhAnh.FileURL = "/uploads/" + uniqueFileName;
                    }

                    _context.Update(hinhAnh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HinhAnhExists(hinhAnh.MaAnh))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Details", "HoSoBenhAns", new { id = hinhAnh.MaHoSo });
            }
            ViewData["MaHoSo"] = new SelectList(_context.HoSoBenhAns, "MaHoSo", "MaHoSo", hinhAnh.MaHoSo);
            return View(hinhAnh);
        }
        // GET: HinhAnhYTes/Delete/5
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var hinhAnh = await _context.HinhAnhYTes
                            .Include(h => h.HoSoBenhAn)
                            .FirstOrDefaultAsync(m => m.MaAnh == id);

            if (hinhAnh == null) return NotFound();
            ViewBag.MaHoSo = hinhAnh.MaHoSo;

            return View(hinhAnh);
        }

        // POST: HinhAnhYTes/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var hinhAnh = await _context.HinhAnhYTes.FindAsync(id);

            if (hinhAnh != null)
            {
                // Xóa luôn file vật lý khỏi ổ đĩa
                if (!string.IsNullOrEmpty(hinhAnh.FileURL))
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, hinhAnh.FileURL.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.HinhAnhYTes.Remove(hinhAnh);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "HoSoBenhAns", new { id = hinhAnh.MaHoSo });
        }

        private bool HinhAnhExists(string id)
        {
            return _context.HinhAnhYTes.Any(e => e.MaAnh == id);
        }
    }
}

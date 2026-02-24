using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DACS_web.Models;
using Microsoft.AspNetCore.Authorization;

namespace DACS_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
    public class XetNghiemCLSsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public XetNghiemCLSsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/XetNghiemCLSs
        public async Task<IActionResult> Index()
        {
            var xetNghiems = await _context.XetNghiemCLSs
                .Include(x => x.HoSoBenhAn)
                    .ThenInclude(h => h.BenhNhan)
                .Include(x => x.BacSiChiDinh)
                .OrderByDescending(x => x.NgayThucHien)
                .ToListAsync();

            return View(xetNghiems);
        }

        // GET: Admin/XetNghiemCLSs/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null) return NotFound();

            var xetNghiem = await _context.XetNghiemCLSs
                .Include(x => x.HoSoBenhAn)
                    .ThenInclude(h => h.BenhNhan)
                .Include(x => x.BacSiChiDinh)
                .FirstOrDefaultAsync(x => x.MaXetNghiem == id);

            if (xetNghiem == null) return NotFound();

            return View(xetNghiem);
        }

        // GET: Admin/XetNghiemCLSs/Create
        public IActionResult Create(string? maHoSo = null, string? returnUrl = null)
        {
            var xetNghiem = new XetNghiemCLS
            {
                NgayThucHien = DateTime.Now,
                GioThucHien = DateTime.Now.TimeOfDay
            };

            ViewData["ReturnUrl"] = returnUrl;

            if (!string.IsNullOrEmpty(maHoSo))
            {
                xetNghiem.MaHoSo = maHoSo;

                var hoSoInfo = _context.HoSoBenhAns
                    .Include(h => h.BenhNhan)
                    .FirstOrDefault(h => h.MaHoSo == maHoSo);

                ViewData["HoSoInfo"] = hoSoInfo;
            }

            ViewData["MaHoSo"] = new SelectList(
                _context.HoSoBenhAns.Include(h => h.BenhNhan),
                "MaHoSo",
                "BenhNhan.HoTen",
                maHoSo
            );

            var bacSiList = _context.NhanVienYTes
                .Where(nv => nv.VaiTro == VaiTroNhanVien.BacSi)
                .ToList();

            ViewData["MaBacSiChiDinh"] = new SelectList(bacSiList, "MaNhanVien", "HoTen");

            ViewData["LoaiXetNghiem"] = new SelectList(new[]
            {
                new { Value = "Xét nghiệm máu", Text = "Xét nghiệm máu" },
                new { Value = "Xét nghiệm nước tiểu", Text = "Xét nghiệm nước tiểu" },
                new { Value = "Siêu âm", Text = "Siêu âm" },
                new { Value = "X-Quang", Text = "X-Quang" },
                new { Value = "CT Scanner", Text = "CT Scanner" },
                new { Value = "MRI", Text = "MRI" },
                new { Value = "Nội soi", Text = "Nội soi" },
                new { Value = "OCT (Chụp cắt lớp võng mạc)", Text = "OCT (Chụp cắt lớp võng mạc)" },
                new { Value = "Đo thị trường", Text = "Đo thị trường" },
                new { Value = "Đo độ cong giác mạc", Text = "Đo độ cong giác mạc" },
                new { Value = "Khác", Text = "Khác" }
            }, "Value", "Text");

            return View(xetNghiem);
        }

        // POST: Admin/XetNghiemCLSs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [FromForm] XetNghiemCLS xetNghiem,
            [FromForm] string? returnUrl = null)
        {
            ModelState.Remove("HoSoBenhAn");
            ModelState.Remove("BacSiChiDinh");

            if (ModelState.IsValid)
            {
                try
                {
                    // Tự động sinh mã xét nghiệm
                    var maxMa = await _context.XetNghiemCLSs
                        .OrderByDescending(x => x.MaXetNghiem)
                        .Select(x => x.MaXetNghiem)
                        .FirstOrDefaultAsync();

                    int nextId = 1;
                    if (!string.IsNullOrEmpty(maxMa) && maxMa.StartsWith("XN"))
                    {
                        int.TryParse(maxMa.Substring(2), out nextId);
                        nextId++;
                    }

                    xetNghiem.MaXetNghiem = $"XN{nextId.ToString("D4")}";
                    _context.XetNghiemCLSs.Add(xetNghiem);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Đã thêm xét nghiệm cận lâm sàng thành công!";

                    // Redirect về Details của HoSoBenhAn hoặc URL được chỉ định
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Details", "HoSoBenhAns", new { id = xetNghiem.MaHoSo });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Lỗi lưu dữ liệu: {ex.Message}");
                }
            }

            ViewData["ReturnUrl"] = returnUrl;

            if (!string.IsNullOrEmpty(xetNghiem.MaHoSo))
            {
                var hoSoInfo = await _context.HoSoBenhAns
                    .Include(h => h.BenhNhan)
                    .FirstOrDefaultAsync(h => h.MaHoSo == xetNghiem.MaHoSo);

                ViewData["HoSoInfo"] = hoSoInfo;
            }

            ViewData["MaHoSo"] = new SelectList(
                _context.HoSoBenhAns.Include(h => h.BenhNhan),
                "MaHoSo",
                "BenhNhan.HoTen",
                xetNghiem.MaHoSo
            );

            var bacSiList = _context.NhanVienYTes
                .Where(nv => nv.VaiTro == VaiTroNhanVien.BacSi)
                .ToList();

            ViewData["MaBacSiChiDinh"] = new SelectList(bacSiList, "MaNhanVien", "HoTen", xetNghiem.MaBacSiChiDinh);

            ViewData["LoaiXetNghiem"] = new SelectList(new[]
            {
                new { Value = "Xét nghiệm máu", Text = "Xét nghiệm máu" },
                new { Value = "Xét nghiệm nước tiểu", Text = "Xét nghiệm nước tiểu" },
                new { Value = "Siêu âm", Text = "Siêu âm" },
                new { Value = "X-Quang", Text = "X-Quang" },
                new { Value = "CT Scanner", Text = "CT Scanner" },
                new { Value = "MRI", Text = "MRI" },
                new { Value = "Nội soi", Text = "Nội soi" },
                new { Value = "OCT (Chụp cắt lớp võng mạc)", Text = "OCT (Chụp cắt lớp võng mạc)" },
                new { Value = "Đo thị trường", Text = "Đo thị trường" },
                new { Value = "Đo độ cong giác mạc", Text = "Đo độ cong giác mạc" },
                new { Value = "Khác", Text = "Khác" }
            }, "Value", "Text", xetNghiem.LoaiXetNghiem);

            return View(xetNghiem);
        }

        // GET: Admin/XetNghiemCLSs/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null) return NotFound();

            var xetNghiem = await _context.XetNghiemCLSs.FindAsync(id);
            if (xetNghiem == null) return NotFound();

            ViewData["MaHoSo"] = new SelectList(
                _context.HoSoBenhAns.Include(h => h.BenhNhan),
                "MaHoSo",
                "BenhNhan.HoTen",
                xetNghiem.MaHoSo
            );

            var bacSiList = _context.NhanVienYTes
                .Where(nv => nv.VaiTro == VaiTroNhanVien.BacSi)
                .ToList();

            ViewData["MaBacSiChiDinh"] = new SelectList(bacSiList, "MaNhanVien", "HoTen", xetNghiem.MaBacSiChiDinh);

            ViewData["LoaiXetNghiem"] = new SelectList(new[]
            {
                new { Value = "Xét nghiệm máu", Text = "Xét nghiệm máu" },
                new { Value = "Xét nghiệm nước tiểu", Text = "Xét nghiệm nước tiểu" },
                new { Value = "Siêu âm", Text = "Siêu âm" },
                new { Value = "X-Quang", Text = "X-Quang" },
                new { Value = "CT Scanner", Text = "CT Scanner" },
                new { Value = "MRI", Text = "MRI" },
                new { Value = "Nội soi", Text = "Nội soi" },
                new { Value = "OCT (Chụp cắt lớp võng mạc)", Text = "OCT (Chụp cắt lớp võng mạc)" },
                new { Value = "Đo thị trường", Text = "Đo thị trường" },
                new { Value = "Đo độ cong giác mạc", Text = "Đo độ cong giác mạc" },
                new { Value = "Khác", Text = "Khác" }
            }, "Value", "Text", xetNghiem.LoaiXetNghiem);

            return View(xetNghiem);
        }

        // POST: Admin/XetNghiemCLSs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, XetNghiemCLS xetNghiem)
        {
            if (id != xetNghiem.MaXetNghiem) return NotFound();

            ModelState.Remove("HoSoBenhAn");
            ModelState.Remove("BacSiChiDinh");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(xetNghiem);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Đã cập nhật xét nghiệm thành công!";
                    return RedirectToAction("Details", "HoSoBenhAns", new { id = xetNghiem.MaHoSo });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!XetNghiemExists(xetNghiem.MaXetNghiem))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewData["MaHoSo"] = new SelectList(
                _context.HoSoBenhAns.Include(h => h.BenhNhan),
                "MaHoSo",
                "BenhNhan.HoTen",
                xetNghiem.MaHoSo
            );

            var bacSiList = _context.NhanVienYTes
                .Where(nv => nv.VaiTro == VaiTroNhanVien.BacSi)
                .ToList();

            ViewData["MaBacSiChiDinh"] = new SelectList(bacSiList, "MaNhanVien", "HoTen", xetNghiem.MaBacSiChiDinh);

            return View(xetNghiem);
        }

        // GET: Admin/XetNghiemCLSs/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null) return NotFound();

            var xetNghiem = await _context.XetNghiemCLSs
                .Include(x => x.HoSoBenhAn)
                    .ThenInclude(h => h.BenhNhan)
                .Include(x => x.BacSiChiDinh)
                .FirstOrDefaultAsync(x => x.MaXetNghiem == id);

            if (xetNghiem == null) return NotFound();

            return View(xetNghiem);
        }

        // POST: Admin/XetNghiemCLSs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var xetNghiem = await _context.XetNghiemCLSs.FindAsync(id);
            if (xetNghiem != null)
            {
                var maHoSo = xetNghiem.MaHoSo;
                _context.XetNghiemCLSs.Remove(xetNghiem);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đã xóa xét nghiệm thành công!";
                return RedirectToAction("Details", "HoSoBenhAns", new { id = maHoSo });
            }

            return RedirectToAction(nameof(Index));
        }

        private bool XetNghiemExists(string id)
        {
            return _context.XetNghiemCLSs.Any(e => e.MaXetNghiem == id);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DACS_web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace DACS_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DotKhamsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DotKhamsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DotKhams
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BenhNhan + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        public async Task<IActionResult> Index()
        {
            var dotKhams = _context.DotKhams
                            .Include(d => d.BenhNhan)
                            .Include(d => d.KhoaTiepNhan);
            return View(await dotKhams.ToListAsync());
        }

        // GET: DotKhams/Details/5
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BenhNhan + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var dotKham = await _context.DotKhams
                .Include(d => d.BenhNhan)
                .Include(d => d.KhoaTiepNhan)
                .FirstOrDefaultAsync(m => m.MaDotKham == id);

            if (dotKham == null) return NotFound();

            return View(dotKham);
        }

        // GET: DotKhams/Create
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        public IActionResult Create(string maBenhNhan)
        {
            var dotKham = new DotKham
            {
                MaBenhNhan = maBenhNhan,
                NgayVaoVien = DateTime.Today,
                GioVaoVien = DateTime.Now.TimeOfDay
            };
            ViewData["MaKhoaTiepNhan"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa");

            ViewData["MaCuocHen"] = new SelectList(_context.CuocHenKhams.Where(c => c.TrangThaiHen == "Chờ khám" && c.MaBenhNhan == maBenhNhan), "MaCuocHen", "MaCuocHen");
            return View(dotKham);
        }

        // POST: DotKhams/Create
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DotKham dotKham)
        {
            if (ModelState.IsValid)
            {
                // Tự sinh Mã đợt khám tăng dần
                var lastDotKham = await _context.DotKhams
                                        .OrderByDescending(d => d.MaDotKham)
                                        .FirstOrDefaultAsync();

                int nextId = 1; // Default ID nếu bảng đang rỗng
                if (lastDotKham != null && int.TryParse(lastDotKham.MaDotKham?.Substring(2), out int lastId)) // lấy số sau "DK"
                {
                    nextId = lastId + 1;
                }
                dotKham.MaDotKham = "DK" + nextId.ToString("D4"); // Ví dụ DK0001

                if (dotKham.NgayRaVien.HasValue)
                {
                    dotKham.TongSoNgayDieuTri = (dotKham.NgayRaVien.Value.Date - dotKham.NgayVaoVien.Date).Days;
                }
                else
                {
                    dotKham.TongSoNgayDieuTri = 0;
                }
                if (!string.IsNullOrEmpty(dotKham.MaCuocHen))
                {
                    var cuocHen = await _context.CuocHenKhams.FirstOrDefaultAsync(c => c.MaCuocHen == dotKham.MaCuocHen);
                    if (cuocHen != null)
                    {
                        cuocHen.TrangThaiHen = "Đã khám";
                        _context.Update(cuocHen);
                    }
                }
                // ❗ Ngắt liên kết navigation tránh lỗi tracking
                dotKham.BenhNhan = null;
                dotKham.KhoaTiepNhan = null;
                dotKham.KhoaRaVien = null;
                dotKham.CuocHenKham = null;

                _context.Add(dotKham);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đã tạo đợt khám thành công! Vui lòng lập hồ sơ bệnh án.";

                // ✅ Redirect sang trang tạo hồ sơ bệnh án với maBenhNhan và maDotKham
                return RedirectToAction("Create", "HoSoBenhAns", new
                {
                    maBenhNhan = dotKham.MaBenhNhan,
                    maDotKham = dotKham.MaDotKham
                });
            }

            ViewData["MaBenhNhan"] = new SelectList(_context.BenhNhans, "MaBenhNhan", "HoTen", dotKham.MaBenhNhan);
            ViewData["MaKhoaTiepNhan"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", dotKham.MaKhoaTiepNhan);
            ViewData["MaCuocHen"] = new SelectList(_context.CuocHenKhams, "MaCuocHen", "MaCuocHen", dotKham.MaCuocHen);
            return View(dotKham);
        }

        // ✅ GET: DotKhams/Edit/5 - CẬP NHẬT với returnUrl
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        public async Task<IActionResult> Edit(string id, string returnUrl = null)
        {
            if (id == null) return NotFound();

            var dotKham = await _context.DotKhams
                .Include(d => d.BenhNhan)
                .Include(d => d.KhoaTiepNhan)
                .Include(d => d.KhoaRaVien)
                .FirstOrDefaultAsync(d => d.MaDotKham == id);

            if (dotKham == null) return NotFound();

            ViewData["MaBenhNhan"] = new SelectList(_context.BenhNhans, "MaBenhNhan", "HoTen", dotKham.MaBenhNhan);
            ViewData["MaKhoaTiepNhan"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", dotKham.MaKhoaTiepNhan);
            ViewData["MaKhoaRaVien"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", dotKham.MaKhoaRaVien);
            ViewData["MaCuocHen"] = new SelectList(_context.CuocHenKhams, "MaCuocHen", "MaCuocHen", dotKham.MaCuocHen);

            // ✅ Lưu returnUrl để dùng trong View và POST
            ViewData["ReturnUrl"] = returnUrl;

            return View(dotKham);
        }

        // ✅ POST: DotKhams/Edit/5 - CẬP NHẬT với returnUrl
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, DotKham dotKham, string returnUrl = null)
        {
            if (id != dotKham.MaDotKham) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Tính số ngày điều trị
                    if (dotKham.NgayRaVien.HasValue)
                    {
                        dotKham.TongSoNgayDieuTri = (dotKham.NgayRaVien.Value.Date - dotKham.NgayVaoVien.Date).Days;
                    }
                    else
                    {
                        dotKham.TongSoNgayDieuTri = 0;
                    }

                    // ❗ Ngắt liên kết navigation tránh lỗi tracking
                    dotKham.BenhNhan = null;
                    dotKham.KhoaTiepNhan = null;
                    dotKham.KhoaRaVien = null;
                    dotKham.CuocHenKham = null;

                    _context.Update(dotKham);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Đã cập nhật đợt khám thành công!";

                    // ✅ Redirect logic mới
                    // 1. Nếu có returnUrl và hợp lệ -> Redirect về returnUrl
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    // 2. Nếu không có returnUrl, tìm HoSoBenhAn liên quan và redirect
                    var hoSoBenhAn = await _context.HoSoBenhAns
                        .FirstOrDefaultAsync(h => h.MaDotKham == dotKham.MaDotKham);

                    if (hoSoBenhAn != null)
                    {
                        return RedirectToAction("Details", "HoSoBenhAns", new { id = hoSoBenhAn.MaHoSo });
                    }

                    // 3. Fallback: Về trang Details của BenhNhan
                    return RedirectToAction("Details", "BenhNhans", new { id = dotKham.MaBenhNhan });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DotKhamExists(dotKham.MaDotKham)) return NotFound();
                    else throw;
                }
            }

            ViewData["MaBenhNhan"] = new SelectList(_context.BenhNhans, "MaBenhNhan", "HoTen", dotKham.MaBenhNhan);
            ViewData["MaKhoaTiepNhan"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", dotKham.MaKhoaTiepNhan);
            ViewData["MaKhoaRaVien"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", dotKham.MaKhoaRaVien);
            ViewData["MaCuocHen"] = new SelectList(_context.CuocHenKhams, "MaCuocHen", "MaCuocHen", dotKham.MaCuocHen);
            ViewData["ReturnUrl"] = returnUrl;

            return View(dotKham);
        }

        // GET: DotKhams/Delete/5
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var dotKham = await _context.DotKhams
                .Include(d => d.BenhNhan)
                .Include(d => d.KhoaTiepNhan)
                .FirstOrDefaultAsync(m => m.MaDotKham == id);

            if (dotKham == null) return NotFound();

            return View(dotKham);
        }

        // POST: DotKhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var dotKham = await _context.DotKhams.FindAsync(id);
            if (dotKham != null)
            {
                _context.DotKhams.Remove(dotKham);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", "BenhNhans", new { id = dotKham.MaBenhNhan });
        }

        private bool DotKhamExists(string id)
        {
            return _context.DotKhams.Any(e => e.MaDotKham == id);
        }
    }
}
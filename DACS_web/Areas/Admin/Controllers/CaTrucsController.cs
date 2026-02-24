using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DACS_web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DACS_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CaTrucsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public CaTrucsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/CaTrucs
        public async Task<IActionResult> Index(DateTime? ngayTruc, string? maNhanVien, string? tenCa)
        {
            var caTrucs = _context.CaTrucs.Include(c => c.NhanVien).ThenInclude(nv => nv.Khoa).AsQueryable();

            // Nếu không có filter ngày, mặc định hiển thị tuần hiện tại
            if (!ngayTruc.HasValue)
            {
                var today = DateTime.Today;
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                var endOfWeek = startOfWeek.AddDays(7);
                caTrucs = caTrucs.Where(c => c.NgayTruc >= startOfWeek && c.NgayTruc < endOfWeek);
            }
            else
            {
                caTrucs = caTrucs.Where(c => c.NgayTruc.Date == ngayTruc.Value.Date);
            }

            if (!string.IsNullOrEmpty(maNhanVien))
            {
                caTrucs = caTrucs.Where(c => c.MaNhanVien == maNhanVien);
            }

            if (!string.IsNullOrEmpty(tenCa))
            {
                caTrucs = caTrucs.Where(c => c.TenCa == tenCa);
            }

            ViewBag.NgayTruc = ngayTruc?.ToString("yyyy-MM-dd") ?? "";
            ViewBag.MaNhanVien = maNhanVien ?? "";
            ViewBag.TenCa = tenCa ?? "";

            var nhanVienList = await _context.NhanVienYTes.OrderBy(nv => nv.HoTen).ToListAsync();
            ViewBag.NhanVienList = new SelectList(nhanVienList, "MaNhanVien", "HoTen");
            ViewBag.CaTrucList = new SelectList(CaTrucOptions.GetTenCaTruc());

            var result = await caTrucs.OrderBy(c => c.NgayTruc).ThenBy(c => c.GioBatDau).ToListAsync();
            return View(result);
        }

        // GET: Admin/CaTrucs/Calendar
        public async Task<IActionResult> Calendar(DateTime? month)
        {
            var selectedMonth = month ?? DateTime.Today;
            var startOfMonth = new DateTime(selectedMonth.Year, selectedMonth.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1);

            var caTrucs = await _context.CaTrucs
                .Include(c => c.NhanVien)
                .Where(c => c.NgayTruc >= startOfMonth && c.NgayTruc < endOfMonth)
                .OrderBy(c => c.NgayTruc)
                .ThenBy(c => c.GioBatDau)
                .ToListAsync();

            ViewBag.SelectedMonth = selectedMonth;
            return View(caTrucs);
        }

        // GET: Admin/CaTrucs/Create
        public IActionResult Create()
        {
            ViewData["MaNhanVien"] = new SelectList(_context.NhanVienYTes.OrderBy(nv => nv.HoTen), "MaNhanVien", "HoTen");
            ViewData["TenCa"] = new SelectList(CaTrucOptions.GetTenCaTruc());
            return View();
        }

        // POST: Admin/CaTrucs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CaTruc caTruc)
        {
            if (ModelState.IsValid)
            {
                // Tự động set giờ dựa vào ca
                var gioCa = CaTrucOptions.GetGioCaTruc(caTruc.TenCa);
                caTruc.GioBatDau = gioCa.Start;
                caTruc.GioKetThuc = gioCa.End;

                // Kiểm tra trùng lịch trực
                var existed = await _context.CaTrucs
                    .AnyAsync(c => c.MaNhanVien == caTruc.MaNhanVien
                                && c.NgayTruc.Date == caTruc.NgayTruc.Date
                                && c.TenCa == caTruc.TenCa);

                if (existed)
                {
                    ModelState.AddModelError("", "Nhân viên đã có lịch trực trong ca này!");
                    ViewData["MaNhanVien"] = new SelectList(_context.NhanVienYTes.OrderBy(nv => nv.HoTen), "MaNhanVien", "HoTen", caTruc.MaNhanVien);
                    ViewData["TenCa"] = new SelectList(CaTrucOptions.GetTenCaTruc(), caTruc.TenCa);
                    return View(caTruc);
                }

                _context.Add(caTruc);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm ca trực thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaNhanVien"] = new SelectList(_context.NhanVienYTes.OrderBy(nv => nv.HoTen), "MaNhanVien", "HoTen", caTruc.MaNhanVien);
            ViewData["TenCa"] = new SelectList(CaTrucOptions.GetTenCaTruc(), caTruc.TenCa);
            return View(caTruc);
        }

        // GET: Admin/CaTrucs/CreateMultiple
        public IActionResult CreateMultiple()
        {
            ViewData["MaNhanVien"] = new SelectList(_context.NhanVienYTes.OrderBy(nv => nv.HoTen), "MaNhanVien", "HoTen");
            ViewData["TenCa"] = new SelectList(CaTrucOptions.GetTenCaTruc());
            return View();
        }

        // POST: Admin/CaTrucs/CreateMultiple
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMultiple(string[] maNhanViens, DateTime ngayBatDau, DateTime ngayKetThuc, string[] tenCas, string ghiChu)
        {
            if (maNhanViens == null || maNhanViens.Length == 0 || tenCas == null || tenCas.Length == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn nhân viên và ca trực!";
                return RedirectToAction(nameof(CreateMultiple));
            }

            var caTrucsToAdd = new List<CaTruc>();

            for (var date = ngayBatDau.Date; date <= ngayKetThuc.Date; date = date.AddDays(1))
            {
                foreach (var maNhanVien in maNhanViens)
                {
                    foreach (var tenCa in tenCas)
                    {
                        // Kiểm tra trùng lịch
                        var existed = await _context.CaTrucs
                            .AnyAsync(c => c.MaNhanVien == maNhanVien
                                        && c.NgayTruc.Date == date
                                        && c.TenCa == tenCa);

                        if (!existed)
                        {
                            var gioCa = CaTrucOptions.GetGioCaTruc(tenCa);
                            caTrucsToAdd.Add(new CaTruc
                            {
                                MaNhanVien = maNhanVien,
                                NgayTruc = date,
                                TenCa = tenCa,
                                GioBatDau = gioCa.Start,
                                GioKetThuc = gioCa.End,
                                GhiChu = ghiChu
                            });
                        }
                    }
                }
            }

            if (caTrucsToAdd.Any())
            {
                _context.CaTrucs.AddRange(caTrucsToAdd);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã thêm {caTrucsToAdd.Count} ca trực thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Tất cả ca trực đã tồn tại!";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/CaTrucs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var caTruc = await _context.CaTrucs.FindAsync(id);
            if (caTruc == null)
                return NotFound();

            ViewData["MaNhanVien"] = new SelectList(_context.NhanVienYTes.OrderBy(nv => nv.HoTen), "MaNhanVien", "HoTen", caTruc.MaNhanVien);
            ViewData["TenCa"] = new SelectList(CaTrucOptions.GetTenCaTruc(), caTruc.TenCa);
            return View(caTruc);
        }

        // POST: Admin/CaTrucs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CaTruc caTruc)
        {
            if (id != caTruc.MaCaTruc)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Tự động set giờ dựa vào ca
                    var gioCa = CaTrucOptions.GetGioCaTruc(caTruc.TenCa);
                    caTruc.GioBatDau = gioCa.Start;
                    caTruc.GioKetThuc = gioCa.End;

                    // Kiểm tra trùng lịch (ngoại trừ bản ghi hiện tại)
                    var existed = await _context.CaTrucs
                        .AnyAsync(c => c.MaCaTruc != id
                                    && c.MaNhanVien == caTruc.MaNhanVien
                                    && c.NgayTruc.Date == caTruc.NgayTruc.Date
                                    && c.TenCa == caTruc.TenCa);

                    if (existed)
                    {
                        ModelState.AddModelError("", "Nhân viên đã có lịch trực trong ca này!");
                        ViewData["MaNhanVien"] = new SelectList(_context.NhanVienYTes.OrderBy(nv => nv.HoTen), "MaNhanVien", "HoTen", caTruc.MaNhanVien);
                        ViewData["TenCa"] = new SelectList(CaTrucOptions.GetTenCaTruc(), caTruc.TenCa);
                        return View(caTruc);
                    }

                    _context.Update(caTruc);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật ca trực thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CaTrucExists(caTruc.MaCaTruc))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaNhanVien"] = new SelectList(_context.NhanVienYTes.OrderBy(nv => nv.HoTen), "MaNhanVien", "HoTen", caTruc.MaNhanVien);
            ViewData["TenCa"] = new SelectList(CaTrucOptions.GetTenCaTruc(), caTruc.TenCa);
            return View(caTruc);
        }

        // GET: Admin/CaTrucs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var caTruc = await _context.CaTrucs
                .Include(c => c.NhanVien)
                .FirstOrDefaultAsync(m => m.MaCaTruc == id);

            if (caTruc == null)
                return NotFound();

            return View(caTruc);
        }

        // POST: Admin/CaTrucs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var caTruc = await _context.CaTrucs.FindAsync(id);
            if (caTruc != null)
            {
                _context.CaTrucs.Remove(caTruc);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa ca trực thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/CaTrucs/MyShifts - Xem ca trực của bản thân
        [AllowAnonymous]
        [Authorize(Roles = $"{SD.Role_BacSi},{SD.Role_YTa}")]
        public async Task<IActionResult> MyShifts(DateTime? month)
        {
            var user = await _userManager.GetUserAsync(User);
            var maNhanVien = user?.MaNhanVien;

            if (string.IsNullOrEmpty(maNhanVien))
            {
                TempData["ErrorMessage"] = "Bạn chưa được gán mã nhân viên!";
                return RedirectToAction("Index", "Dashboard");
            }

            var selectedMonth = month ?? DateTime.Today;
            var startOfMonth = new DateTime(selectedMonth.Year, selectedMonth.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1);

            var caTrucs = await _context.CaTrucs
                .Include(c => c.NhanVien)
                .Where(c => c.MaNhanVien == maNhanVien
                         && c.NgayTruc >= startOfMonth
                         && c.NgayTruc < endOfMonth)
                .OrderBy(c => c.NgayTruc)
                .ThenBy(c => c.GioBatDau)
                .ToListAsync();

            ViewBag.SelectedMonth = selectedMonth;
            ViewBag.NhanVien = await _context.NhanVienYTes.FindAsync(maNhanVien);

            return View(caTrucs);
        }
        private bool CaTrucExists(int id)
        {
            return _context.CaTrucs.Any(e => e.MaCaTruc == id);
        }
    }
}
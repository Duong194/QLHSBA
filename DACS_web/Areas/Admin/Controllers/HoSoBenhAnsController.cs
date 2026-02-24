using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DACS_web.Models;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Identity;

namespace DACS_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HoSoBenhAnsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private IQueryable<HoSoBenhAn> query;

        public HoSoBenhAnsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            
        }

        // GET: HoSoBenhAns
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BenhNhan + "," + SD.Role_BacSi + "," + SD.Role_YTa)]

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            IQueryable<HoSoBenhAn> query = _context.HoSoBenhAns
                .Include(h => h.BenhNhan)
                .Include(h => h.BacSiLapBenhAn)
                .Include(h => h.BacSiDieuTri);
            if (User.IsInRole(SD.Role_BacSi))
            {
                if (!string.IsNullOrEmpty(user.MaNhanVien))
                {
                    query = query.Where(h => h.MaBacSiLapBenhAn == user.MaNhanVien || h.MaBacSiDieuTri == user.MaNhanVien);
                }
                else
                {
                    return View(new List<HoSoBenhAn>()); // Không có mã nhân viên, không hiển thị gì
                }
            }

            if (User.IsInRole(SD.Role_BenhNhan))
            {
                if (!string.IsNullOrEmpty(user.MaBenhNhan))
                {
                    query = query.Where(h => h.MaBenhNhan == user.MaBenhNhan);
                }
                else
                {
                    return View(new List<HoSoBenhAn>()); // Không có hồ sơ
                }
            }

            return View(await query.ToListAsync());
        }


        // GET: HoSoBenhAns/Details/
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BenhNhan + "," + SD.Role_BacSi + "," + SD.Role_YTa)]

        // Thêm phần này vào method Details trong HoSoBenhAnsController.cs

        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var hoSo = await _context.HoSoBenhAns
                .AsSplitQuery()
                .Include(h => h.BenhNhan)
                .Include(h => h.DotKham)
                    .ThenInclude(d => d.KhoaTiepNhan)
                .Include(h => h.DotKham)
                    .ThenInclude(d => d.KhoaRaVien)
                .Include(h => h.BacSiLapBenhAn)
                .Include(h => h.BacSiDieuTri)
                .Include(h => h.ChanDoans)
                    .ThenInclude(cd => cd.DanhMucICD)
                .Include(h => h.KetQuaKhamMats)
                .Include(h => h.PhuongPhapDieuTris)
                .Include(h => h.HinhAnhYTes)
                .Include(h => h.DonThuocs)
                    .ThenInclude(dt => dt.ChiTietDonThuocs)
                        .ThenInclude(ct => ct.Thuoc)
                .Include(h => h.XetNghiemCLSs)  // ✅ THÊM dòng này
                    .ThenInclude(xn => xn.BacSiChiDinh)  // ✅ THÊM dòng này
                .FirstOrDefaultAsync(h => h.MaHoSo == id);

            if (hoSo == null) return NotFound();

            if (User.IsInRole(SD.Role_BenhNhan))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user.MaBenhNhan != hoSo.MaBenhNhan)
                {
                    return Forbid();
                }
            }

            return View(hoSo);
        }

        // GET: HoSoBenhAns/Create
        // GET: HoSoBenhAns/Create
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        public async Task<IActionResult> Create(string? maBenhNhan = null, string? maDotKham = null)
        {
            var hoSoBenhAn = new HoSoBenhAn
            {
                NgayTao = DateTime.Now,
                TrangThaiRaVien = "Đang điều trị"
            };

            // ✅ Nếu có maBenhNhan và maDotKham từ DotKham
            if (!string.IsNullOrEmpty(maBenhNhan))
            {
                hoSoBenhAn.MaBenhNhan = maBenhNhan;
            }

            if (!string.IsNullOrEmpty(maDotKham))
            {
                hoSoBenhAn.MaDotKham = maDotKham;

                // Load thông tin đợt khám để hiển thị
                var dotKham = await _context.DotKhams
                    .Include(d => d.BenhNhan)
                    .Include(d => d.KhoaTiepNhan)
                    .FirstOrDefaultAsync(d => d.MaDotKham == maDotKham);

                if (dotKham != null)
                {
                    ViewData["DotKhamInfo"] = dotKham;
                }
            }

            ViewData["MaBenhNhan"] = new SelectList(_context.BenhNhans, "MaBenhNhan", "HoTen", maBenhNhan);

            var bacSiList = _context.NhanVienYTes
                .Where(nv => nv.VaiTro == VaiTroNhanVien.BacSi)
                .ToList();

            ViewData["MaBacSiLapBenhAn"] = new SelectList(bacSiList, "MaNhanVien", "HoTen");
            ViewData["MaBacSiDieuTri"] = new SelectList(bacSiList, "MaNhanVien", "HoTen");

            return View(hoSoBenhAn);
        }

        // POST: HoSoBenhAns/Create
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HoSoBenhAn hoSoBenhAn)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Tự động sinh mã hồ sơ
                    var maxMa = await _context.HoSoBenhAns
                        .OrderByDescending(h => h.MaHoSo)
                        .Select(h => h.MaHoSo)
                        .FirstOrDefaultAsync();

                    int nextId = 1;
                    if (!string.IsNullOrEmpty(maxMa) && maxMa.StartsWith("HS"))
                    {
                        int.TryParse(maxMa.Substring(2), out nextId);
                        nextId++;
                    }

                    hoSoBenhAn.MaHoSo = $"HS{nextId.ToString("D3")}";

                    _context.HoSoBenhAns.Add(hoSoBenhAn);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Đã lập hồ sơ bệnh án thành công!";

                    // ✅ Redirect về Details của HoSoBenhAn vừa tạo
                    return RedirectToAction("Details", new { id = hoSoBenhAn.MaHoSo });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Lỗi lưu dữ liệu: {ex.Message}");
                }
            }

            // Reload ViewData nếu có lỗi
            ViewData["MaBenhNhan"] = new SelectList(_context.BenhNhans, "MaBenhNhan", "HoTen", hoSoBenhAn.MaBenhNhan);

            var bacSiList = _context.NhanVienYTes
                 .Where(nv => nv.VaiTro == VaiTroNhanVien.BacSi)
                 .ToList();

            ViewData["MaBacSiLapBenhAn"] = new SelectList(bacSiList, "MaNhanVien", "HoTen", hoSoBenhAn.MaBacSiLapBenhAn);
            ViewData["MaBacSiDieuTri"] = new SelectList(bacSiList, "MaNhanVien", "HoTen", hoSoBenhAn.MaBacSiDieuTri);

            return View(hoSoBenhAn);
        }

        // GET: HoSoBenhAns/Edit/5
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var hoSo = await _context.HoSoBenhAns.FindAsync(id);
            if (hoSo == null) return NotFound();

            ViewData["MaBenhNhan"] = new SelectList(_context.BenhNhans, "MaBenhNhan", "HoTen", hoSo.MaBenhNhan);
            var bacSiList = _context.NhanVienYTes
                .Where(nv => nv.VaiTro == VaiTroNhanVien.BacSi)
                .ToList();

            ViewData["MaBacSiLapBenhAn"] = new SelectList(bacSiList, "MaNhanVien", "HoTen", hoSo.MaBacSiLapBenhAn);
            ViewData["MaBacSiDieuTri"] = new SelectList(bacSiList, "MaNhanVien", "HoTen", hoSo.MaBacSiDieuTri);

            return View(hoSo);
        }

        // POST: HoSoBenhAns/Edit/5
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, HoSoBenhAn hoSoBenhAn)
        {
            if (id != hoSoBenhAn.MaHoSo) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoSoBenhAn);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoSoBenhAnExists(hoSoBenhAn.MaHoSo))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaBenhNhan"] = new SelectList(_context.BenhNhans, "MaBenhNhan", "HoTen", hoSoBenhAn.MaBenhNhan);
            var bacSiList = _context.NhanVienYTes
                .Where(nv => nv.VaiTro == VaiTroNhanVien.BacSi)
                .ToList();

            ViewData["MaBacSiLapBenhAn"] = new SelectList(bacSiList, "MaNhanVien", "HoTen", hoSoBenhAn.MaBacSiLapBenhAn);
            ViewData["MaBacSiDieuTri"] = new SelectList(bacSiList, "MaNhanVien", "HoTen", hoSoBenhAn.MaBacSiDieuTri);


            return View(hoSoBenhAn);
        }

        // GET: HoSoBenhAns/Delete/5
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var hoSo = await _context.HoSoBenhAns
                .Include(h => h.BenhNhan)
                .FirstOrDefaultAsync(m => m.MaHoSo == id);

            if (hoSo == null) return NotFound();

            return View(hoSo);
        }

        // POST: HoSoBenhAns/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var hoSo = await _context.HoSoBenhAns.FindAsync(id);
            if (hoSo != null)
            {
                _context.HoSoBenhAns.Remove(hoSo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool HoSoBenhAnExists(string id)
        {
            return _context.HoSoBenhAns.Any(e => e.MaHoSo == id);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DACS_web.Models;
using DACS_web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace DACS_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DonThuocsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonThuocsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _context.DonThuocs
                .Include(d => d.HoSoBenhAn)
                .Include(d => d.NhanVienYTe)
                .ToListAsync();

            return View(data);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var donThuoc = await _context.DonThuocs
                .Include(d => d.HoSoBenhAn)
                .Include(d => d.NhanVienYTe)
                .Include(d => d.ChiTietDonThuocs)
                    .ThenInclude(c => c.Thuoc)
                .FirstOrDefaultAsync(d => d.MaDon == id);

            if (donThuoc == null) return NotFound();

            return View(donThuoc);
        }

        public IActionResult Create(string maHoSo)
        {
            var viewModel = new DonThuocViewModel
            {
                MaHoSo = maHoSo, 
                NgayKe = DateTime.Today,
                ChiTietThuocs = new List<ChiTietDonThuocVM> { new ChiTietDonThuocVM() }
            };


            // Lấy danh sách thuốc cho SelectList
            var danhSachThuoc = _context.Thuocs.Select(t => new { t.MaThuoc, t.TenThuoc }).ToList();
            ViewBag.MaHoSo = new SelectList(_context.HoSoBenhAns, "MaHoSo", "MaHoSo");
            ViewBag.BacSiKeDon = new SelectList(_context.NhanVienYTes.Where(nv => nv.VaiTro == "Bác sĩ"), "MaNhanVien", "HoTen");
            ViewBag.DanhSachThuoc = new SelectList(_context.Thuocs, "MaThuoc", "TenThuoc");

            // Thêm dòng này để tạo JSON cho việc thêm thuốc bằng JavaScript
            ViewBag.DanhSachThuocJson = JsonSerializer.Serialize(danhSachThuoc);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DonThuocViewModel model)
        {
            // Kiểm tra thuốc trùng lặp
            var duplicateThuocs = model.ChiTietThuocs
                .GroupBy(ct => ct.MaThuoc)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateThuocs.Any())
            {
                // Lấy tên các thuốc trùng lặp để hiển thị thông báo chi tiết
                var thuocNames = new List<string>();
                foreach (var maThuoc in duplicateThuocs)
                {
                    var thuoc = await _context.Thuocs.FirstOrDefaultAsync(t => t.MaThuoc == maThuoc);
                    if (thuoc != null)
                        thuocNames.Add(thuoc.TenThuoc);
                }

                // Thêm lỗi vào ModelState
                ModelState.AddModelError("", $"Thuốc trùng lặp trong đơn: {string.Join(", ", thuocNames)}");

                // Tải lại dữ liệu cho view
                var danhSachThuoc = _context.Thuocs.Select(t => new { t.MaThuoc, t.TenThuoc }).ToList();
                ViewBag.MaHoSo = new SelectList(_context.HoSoBenhAns, "MaHoSo", "MaHoSo", model.MaHoSo);
                ViewBag.BacSiKeDon = new SelectList(_context.NhanVienYTes.Where(nv => nv.VaiTro == "Bác sĩ"), "MaNhanVien", "HoTen", model.BacSiKeDon);
                ViewBag.DanhSachThuoc = new SelectList(_context.Thuocs, "MaThuoc", "TenThuoc");
                ViewBag.DanhSachThuocJson = JsonSerializer.Serialize(danhSachThuoc);
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                var danhSachThuoc = _context.Thuocs.Select(t => new { t.MaThuoc, t.TenThuoc }).ToList();
                ViewBag.MaHoSo = new SelectList(_context.HoSoBenhAns, "MaHoSo", "MaHoSo", model.MaHoSo);
                ViewBag.BacSiKeDon = new SelectList(_context.NhanVienYTes.Where(nv => nv.VaiTro == "Bác sĩ"), "MaNhanVien", "HoTen", model.BacSiKeDon);
                ViewBag.DanhSachThuoc = new SelectList(_context.Thuocs, "MaThuoc", "TenThuoc");
                ViewBag.DanhSachThuocJson = JsonSerializer.Serialize(danhSachThuoc);
                return View(model);
            }

            var nextMa = "DT" + (await _context.DonThuocs.CountAsync() + 1).ToString("D4");

            var donThuoc = new DonThuoc
            {
                MaDon = nextMa,
                MaHoSo = model.MaHoSo,
                BacSiKeDon = model.BacSiKeDon,
                NgayKe = model.NgayKe,
                HuongDanSuDung = model.HuongDanSuDung,
                ChiTietDonThuocs = new List<ChiTietDonThuoc>(),
                TongTien = 0
            };

            foreach (var ct in model.ChiTietThuocs)
            {
                var thuoc = await _context.Thuocs.FindAsync(ct.MaThuoc);
                if (thuoc == null) continue;

                var chiTiet = new ChiTietDonThuoc
                {
                    MaDon = nextMa,
                    MaThuoc = ct.MaThuoc,
                    SoLuong = ct.SoLuong,
                    LieuDung = ct.LieuDung,
                    ThoiGianDung = ct.ThoiGianDung,
                    GhiChu = ct.GhiChu
                };

                donThuoc.TongTien += ct.SoLuong * thuoc.DonGia;
                donThuoc.ChiTietDonThuocs.Add(chiTiet);
            }

            _context.DonThuocs.Add(donThuoc);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var donThuoc = await _context.DonThuocs
                .Include(d => d.HoSoBenhAn)
                .FirstOrDefaultAsync(m => m.MaDon == id);

            if (donThuoc == null) return NotFound();

            return View(donThuoc);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var donThuoc = await _context.DonThuocs.FindAsync(id);
            if (donThuoc != null)
            {
                var chiTiet = _context.ChiTietDonThuocs.Where(c => c.MaDon == id);
                _context.ChiTietDonThuocs.RemoveRange(chiTiet);

                _context.DonThuocs.Remove(donThuoc);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
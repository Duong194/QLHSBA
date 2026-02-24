using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DACS_web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace DACS_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class NhanVienYTesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NhanVienYTesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: NhanVienYTes
        public async Task<IActionResult> Index(string searchString, string vaiTroFilter, string khoaFilter)
        {
            var nhanviens = _context.NhanVienYTes.Include(nv => nv.Khoa).AsQueryable();

            // Tìm kiếm theo tên, mã nhân viên hoặc số điện thoại
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim().ToLower();
                nhanviens = nhanviens.Where(nv =>
                    nv.HoTen.ToLower().Contains(searchString) ||
                    nv.MaNhanVien.ToLower().Contains(searchString) ||
                    nv.SoDienThoai.Contains(searchString));
            }

            // Lọc theo vai trò
            if (!string.IsNullOrEmpty(vaiTroFilter))
            {
                nhanviens = nhanviens.Where(nv => nv.VaiTro == vaiTroFilter);
            }

            // Lọc theo khoa
            if (!string.IsNullOrEmpty(khoaFilter))
            {
                nhanviens = nhanviens.Where(nv => nv.MaKhoa == khoaFilter);
            }

            // Chuẩn bị dữ liệu cho dropdown filters
            ViewBag.SearchString = searchString ?? "";
            ViewBag.VaiTroFilter = vaiTroFilter ?? "";
            ViewBag.KhoaFilter = khoaFilter ?? "";

            var vaiTroList = await _context.NhanVienYTes
                .Where(nv => !string.IsNullOrEmpty(nv.VaiTro))
                .Select(nv => nv.VaiTro)
                .Distinct()
                .ToListAsync();
            ViewBag.VaiTroList = vaiTroList.Select(v => new SelectListItem { Value = v, Text = v }).ToList();

            var khoaList = await _context.Khoas.ToListAsync();
            ViewBag.KhoaList = khoaList.Select(k => new SelectListItem { Value = k.MaKhoa, Text = k.TenKhoa }).ToList();

            var result = await nhanviens.OrderBy(nv => nv.MaNhanVien).ToListAsync();
            return View(result);
        }


        // GET: NhanVienYTes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
                return NotFound();

            var nhanvien = await _context.NhanVienYTes
                .Include(nv => nv.Khoa)
                .FirstOrDefaultAsync(m => m.MaNhanVien == id);

            if (nhanvien == null)
                return NotFound();

            return View(nhanvien);
        }

        // GET: NhanVienYTes/Create
        public IActionResult Create()
        {
            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa");
            return View();
        }

        // POST: NhanVienYTes/Create
        // POST: NhanVienYTes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhanVienYTe nhanVienYTe, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Sinh mã tự động
                    var maxMa = await _context.NhanVienYTes
                        .OrderByDescending(nv => nv.MaNhanVien)
                        .Select(nv => nv.MaNhanVien)
                        .FirstOrDefaultAsync();

                    int nextId = 1;
                    if (!string.IsNullOrEmpty(maxMa) && maxMa.Length >= 5)
                    {
                        int.TryParse(maxMa.Substring(2), out nextId);
                        nextId++;
                    }

                    nhanVienYTe.MaNhanVien = $"NV{nextId:D3}";

                    // Xử lý upload hình ảnh
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/nhanvien");

                        // Tạo thư mục nếu chưa tồn tại
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Tạo tên file unique
                        var uniqueFileName = $"{nhanVienYTe.MaNhanVien}_{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Lưu file
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        // Lưu đường dẫn vào database
                        nhanVienYTe.HinhAnh = $"/images/nhanvien/{uniqueFileName}";
                    }

                    _context.NhanVienYTes.Add(nhanVienYTe);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Lỗi lưu database: {ex.Message}");
                }
            }

            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", nhanVienYTe.MaKhoa);
            return View(nhanVienYTe);
        }

        // GET: NhanVienYTes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
                return NotFound();

            var nhanvien = await _context.NhanVienYTes.FindAsync(id);
            if (nhanvien == null)
                return NotFound();

            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", nhanvien.MaKhoa);
            return View(nhanvien);
        }

        // POST: NhanVienYTes/Edit/5
        // POST: NhanVienYTes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, NhanVienYTe nhanVienYTe, IFormFile? imageFile)
        {
            if (id != nhanVienYTe.MaNhanVien)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Xử lý upload hình ảnh mới nếu có
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/nhanvien");

                        // Tạo thư mục nếu chưa tồn tại
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Xóa ảnh cũ nếu tồn tại
                        if (!string.IsNullOrEmpty(nhanVienYTe.HinhAnh))
                        {
                            var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", nhanVienYTe.HinhAnh.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Tạo tên file unique
                        var uniqueFileName = $"{nhanVienYTe.MaNhanVien}_{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Lưu file
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        // Cập nhật đường dẫn vào database
                        nhanVienYTe.HinhAnh = $"/images/nhanvien/{uniqueFileName}";
                    }
                    else
                    {
                        // Giữ nguyên ảnh cũ nếu không upload ảnh mới
                        var existingNhanVien = await _context.NhanVienYTes.AsNoTracking()
                            .FirstOrDefaultAsync(nv => nv.MaNhanVien == id);
                        if (existingNhanVien != null)
                        {
                            nhanVienYTe.HinhAnh = existingNhanVien.HinhAnh;
                        }
                    }

                    _context.Update(nhanVienYTe);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanVienYTeExists(nhanVienYTe.MaNhanVien))
                        return NotFound();
                    else
                        throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Lỗi cập nhật: {ex.Message}");
                }
            }

            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", nhanVienYTe.MaKhoa);
            return View(nhanVienYTe);
        }

        // GET: NhanVienYTes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
                return NotFound();

            var nhanvien = await _context.NhanVienYTes
                .Include(nv => nv.Khoa)
                .FirstOrDefaultAsync(m => m.MaNhanVien == id);

            if (nhanvien == null)
                return NotFound();

            return View(nhanvien);
        }

        // POST: NhanVienYTes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var nhanvien = await _context.NhanVienYTes.FindAsync(id);
            if (nhanvien != null)
            {
                _context.NhanVienYTes.Remove(nhanvien);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        [AllowAnonymous]
        private bool NhanVienYTeExists(string id)
        {
            return _context.NhanVienYTes.Any(e => e.MaNhanVien == id);
        }
        [AllowAnonymous]
        public async Task<IActionResult> DanhSachBacSi()
        {
            var bacSiList = await _context.NhanVienYTes
                .Where(nv => nv.VaiTro == "Bác sĩ")
                .Include(nv => nv.Khoa)
                .ToListAsync();

            return View("DanhSachBacSi", bacSiList); 
        }
    }
}

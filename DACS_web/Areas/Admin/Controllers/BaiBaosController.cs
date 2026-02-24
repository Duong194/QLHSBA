using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DACS_web.Models;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace DACS_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class BaiBaosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BaiBaosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/BaiBaos
        public async Task<IActionResult> Index()
        {
            return View(await _context.BaiBaos.ToListAsync());
        }

        // GET: Admin/BaiBaos/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var baiBao = await _context.BaiBaos.FirstOrDefaultAsync(m => m.Id == id);
            if (baiBao == null) return NotFound();

            // Truyền thêm ViewData nếu cần để điều khiển hành vi quay lại
            ViewData["ReturnToHome"] = true;

            return View(baiBao);
        }

        // GET: Admin/BaiBaos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/BaiBaos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BaiBao baiBao, IFormFile? HinhAnhFile)
        {
            if (ModelState.IsValid)
            {
                if (HinhAnhFile != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(HinhAnhFile.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await HinhAnhFile.CopyToAsync(stream);
                    }
                    baiBao.HinhAnh = "/uploads/" + fileName;
                }

                baiBao.NgayDang = DateTime.Now;
                _context.Add(baiBao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(baiBao);
        }


        // GET: Admin/BaiBaos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var baiBao = await _context.BaiBaos.FindAsync(id);
            if (baiBao == null)
            {
                return NotFound();
            }
            return View(baiBao);
        }

        // POST: Admin/BaiBaos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TieuDe,NoiDung,TacGia,NgayDang,HinhAnh,DanhMuc")] BaiBao baiBao, IFormFile? HinhAnhFile)
        {
            if (id != baiBao.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Giữ ngày đăng ban đầu nếu không thay đổi
                    var existingBaiBao = await _context.BaiBaos.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
                    baiBao.NgayDang = existingBaiBao.NgayDang;

                    // Xử lý hình ảnh mới nếu có
                    if (HinhAnhFile != null)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(HinhAnhFile.FileName);
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await HinhAnhFile.CopyToAsync(stream);
                        }

                        // Xóa hình ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(existingBaiBao.HinhAnh))
                        {
                            var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                                existingBaiBao.HinhAnh.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        baiBao.HinhAnh = "/uploads/" + fileName;
                    }
                    else
                    {
                        // Giữ nguyên hình ảnh cũ
                        baiBao.HinhAnh = existingBaiBao.HinhAnh;
                    }

                    _context.Update(baiBao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BaiBaoExists(baiBao.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(baiBao);
        }

        // GET: Admin/BaiBaos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var baiBao = await _context.BaiBaos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (baiBao == null)
            {
                return NotFound();
            }

            return View(baiBao);
        }

        // POST: Admin/BaiBaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var baiBao = await _context.BaiBaos.FindAsync(id);
            if (baiBao != null)
            {
                // Xóa hình ảnh liên quan nếu có
                if (!string.IsNullOrEmpty(baiBao.HinhAnh))
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                        baiBao.HinhAnh.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.BaiBaos.Remove(baiBao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BaiBaoExists(int id)
        {
            return _context.BaiBaos.Any(e => e.Id == id);
        }
    }
}
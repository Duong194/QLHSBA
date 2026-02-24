using DACS_web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DACS_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,YTa")] // ✅ Chỉ cho Admin và Y tá xem danh sách & đánh dấu
    public class TuVansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TuVansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Hiển thị danh sách tư vấn, có lọc theo trạng thái liên hệ
        public async Task<IActionResult> Index(bool? chuaLienHe = null)
        {
            var data = _context.TuVans.AsQueryable();

            if (chuaLienHe == true)
                data = data.Where(x => !x.DaLienHe);

            return View(await data.OrderByDescending(x => x.NgayTao).ToListAsync());
        }

        // ✅ Hiển thị form tạo (ai cũng có thể truy cập - KHÔNG thêm [Authorize])
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        // ✅ Xử lý gửi tư vấn từ người dùng
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TuVan model)
        {
            if (ModelState.IsValid)
            {
                model.NgayTao = DateTime.Now;
                model.DaLienHe = false;
                _context.TuVans.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Gửi lịch tư vấn thành công!";
                return RedirectToAction("Create");
            }

            return View(model);
        }

        // ✅ Đánh dấu đã liên hệ / chưa liên hệ (chỉ Admin hoặc Y tá mới được)
        [HttpPost]
        [Authorize(Roles = "Admin,YTa")]
        public async Task<IActionResult> ToggleLienHe(int id)
        {
            var tv = await _context.TuVans.FindAsync(id);
            if (tv == null) return NotFound();

            tv.DaLienHe = !tv.DaLienHe;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}

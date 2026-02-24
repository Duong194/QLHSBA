using System.Diagnostics;
using DACS_web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DACS_web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 6; // Số bài báo trên mỗi trang
            int totalBaiBao = await _context.BaiBaos.CountAsync();

            var baiBaos = await _context.BaiBaos
                .OrderByDescending(b => b.NgayDang)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Thông tin phân trang để truyền vào View
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalBaiBao / (double)pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalBaiBao;

            return View(baiBaos);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
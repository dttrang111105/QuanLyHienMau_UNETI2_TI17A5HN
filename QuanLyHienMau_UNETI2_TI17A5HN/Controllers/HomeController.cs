using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.Repositories;
using System.Diagnostics;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDotHienMauRepository _dotRepo;

        public HomeController(IDotHienMauRepository dotRepo)
        {
            _dotRepo = dotRepo;
        }

        public async Task<IActionResult> Index()
        {
            var openQuery = _dotRepo.ApplyFilterDangNhanDangKy(_dotRepo.GetQueryable())
                .Include(d => d.PhieuDangKyHienMaus)
                .OrderBy(d => d.HanDangKy)
                .ThenByDescending(d => d.MaDot);

            var openCampaigns = await openQuery.Take(3).ToListAsync();
            ViewBag.OpenCampaigns = openCampaigns;
            return View(openCampaigns);
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

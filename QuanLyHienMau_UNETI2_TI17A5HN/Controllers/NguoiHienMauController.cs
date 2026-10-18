using Microsoft.AspNetCore.Mvc;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers
{
    public class NguoiHienMauController : Controller
    {
        // GET: /NguoiHienMau
        public IActionResult Index()
        {
            return RedirectToAction(nameof(HoSoCaNhan));
        }

        // GET: /NguoiHienMau/HoSoCaNhan
        public IActionResult HoSoCaNhan()
        {
            ViewData["Title"] = "Hồ Sơ Người Hiến Máu & Chứng Nhận Điện Tử";
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers
{
    public class DangKyController : Controller
    {
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult TheoDoi(string? code)
        {
            ViewData["TicketCode"] = code ?? "VTL-2026-08991";
            return View();
        }
    }
}

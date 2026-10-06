using Microsoft.AspNetCore.Mvc;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers
{
    public class DotHienMauController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(string? id)
        {
            return View();
        }
    }
}

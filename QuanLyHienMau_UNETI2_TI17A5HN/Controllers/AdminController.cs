using Microsoft.AspNetCore.Mvc;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult LichKhamSangLoc()
        {
            return RedirectToAction("Index", "LichKhamSangLoc");
        }

        public IActionResult HoSoBenhSu()
        {
            return View();
        }

        public IActionResult KetQuaXetNghiem()
        {
            return RedirectToAction("Index", "KetQuaHienMau");
        }

        public IActionResult KhoLuuTruMau()
        {
            return View();
        }

        public IActionResult GiamSatTuiMau()
        {
            return View();
        }

        public IActionResult TieuHuy()
        {
            return View();
        }

        public IActionResult NguoiHienMau()
        {
            return View();
        }

        public IActionResult DonViToChuc()
        {
            return View();
        }

        public IActionResult BaoCao()
        {
            return RedirectToAction("Index", "ThongKe");
        }

        public IActionResult NhanSu()
        {
            return View();
        }

        public IActionResult RBAC()
        {
            return View();
        }

        public IActionResult KhoLanhIoT()
        {
            return View();
        }
    }
}

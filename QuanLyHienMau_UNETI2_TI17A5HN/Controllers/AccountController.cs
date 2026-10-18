using Microsoft.AspNetCore.Mvc;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Login));
        }

        // GET: /Account/Login
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["Title"] = "Đăng Nhập & Đăng Ký Cổng Hiến Máu";
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            ViewData["Title"] = "Đăng Ký Tài Khoản Tình Nguyện Viên";
            return View("Login"); // Reuses tabbed Login/Register view
        }

        // GET: /Account/VNeIDAuth
        public IActionResult VNeIDAuth()
        {
            ViewData["Title"] = "Xác Thực CCCD / VNeID Cổng Định Danh Y Tế Số";
            return View();
        }
    }
}

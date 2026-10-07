// Họ và tên: Nguyễn Tiến Nam
// Mã sinh viên: 23103100275
// Nội dung thực hiện: Đăng kí, đăng nhập, đăng xuất,
// đăng kí, dăng nhập, đăng xuất 


using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<TaiKhoan> _passwordHasher;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<TaiKhoan>();
        }

        // GET: /Account
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Login));
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToRoleHome(User.FindFirst(ClaimTypes.Role)?.Value);
            }

            ViewData["Title"] = "Đăng Nhập & Đăng Ký Cổng Hiến Máu";
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string tenDangNhap, string matKhau, bool rememberMe, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewBag.LoginTenDangNhap = tenDangNhap;

            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
            {
                ViewBag.LoginError = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return View();
            }

            var trimmedInput = tenDangNhap.Trim();
            var trimmedInputLower = trimmedInput.ToLower();

            // Tìm kiếm tài khoản theo TenDangNhap, Email hoặc SoDienThoai của hồ sơ
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.NguoiHienMau)
                .FirstOrDefaultAsync(t => t.TenDangNhap.ToLower() == trimmedInputLower ||
                                         (t.Email != null && t.Email.ToLower() == trimmedInputLower) ||
                                         (t.NguoiHienMau != null && t.NguoiHienMau.SoDienThoai == trimmedInput));

            if (taiKhoan == null)
            {
                ViewBag.LoginError = "Tài khoản hoặc mật khẩu không chính xác.";
                return View();
            }

            if (taiKhoan.TrangThai != "Hoạt động")
            {
                ViewBag.LoginError = "Tài khoản hiện đang bị khóa hoặc tạm dừng. Vui lòng liên hệ ban quản trị.";
                return View();
            }

            // Xác thực mật khẩu
            var verifyResult = _passwordHasher.VerifyHashedPassword(taiKhoan, taiKhoan.MatKhau, matKhau);
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                ViewBag.LoginError = "Tài khoản hoặc mật khẩu không chính xác.";
                return View();
            }

            // Tạo Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, taiKhoan.MaTaiKhoan.ToString()),
                new Claim(ClaimTypes.Name, taiKhoan.HoTen),
                new Claim(ClaimTypes.Role, taiKhoan.VaiTro),
                new Claim("TenDangNhap", taiKhoan.TenDangNhap)
            };

            if (!string.IsNullOrEmpty(taiKhoan.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, taiKhoan.Email));
            }

            var claimsIdentity = new ClaimsIdentity(claims, "QuanLyHienMau.Auth");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(12)
            };

            await HttpContext.SignInAsync("QuanLyHienMau.Auth", new ClaimsPrincipal(claimsIdentity), authProperties);

            TempData["SuccessMessage"] = $"Chào mừng {taiKhoan.HoTen} đã đăng nhập thành công!";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToRoleHome(taiKhoan.VaiTro);
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToRoleHome(User.FindFirst(ClaimTypes.Role)?.Value);
            }

            ViewData["Title"] = "Đăng Ký Tài Khoản Tình Nguyện Viên";
            ViewBag.ActiveTab = "register";
            return View("Login");
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string tenDangNhap,
            string matKhau,
            string xacNhanMatKhau,
            string hoTen,
            string? email,
            string? soDienThoai,
            string? nhomMau,
            double? canNang)
        {
            ViewBag.ActiveTab = "register";
            ViewBag.RegTenDangNhap = tenDangNhap;
            ViewBag.RegHoTen = hoTen;
            ViewBag.RegEmail = email;
            ViewBag.RegSoDienThoai = soDienThoai;
            ViewBag.RegNhomMau = nhomMau;
            ViewBag.RegCanNang = canNang;
            ViewBag.RegMatKhau = matKhau;
            ViewBag.RegXacNhanMatKhau = xacNhanMatKhau;
            ViewBag.RegDongYDieuKhoan = true;

            if (string.IsNullOrWhiteSpace(tenDangNhap) ||
                string.IsNullOrWhiteSpace(matKhau) ||
                string.IsNullOrWhiteSpace(hoTen))
            {
                ViewBag.RegisterError = "Vui lòng điền đầy đủ các thông tin bắt buộc (*).";
                return View("Login");
            }

            if (matKhau != xacNhanMatKhau)
            {
                ViewBag.RegisterError = "Mật khẩu xác nhận không khớp.";
                return View("Login");
            }

            if (matKhau.Length < 6)
            {
                ViewBag.RegisterError = "Mật khẩu phải có tối thiểu 6 ký tự.";
                return View("Login");
            }

            var cleanTenDangNhap = tenDangNhap.Trim();
            var cleanTenDangNhapLower = cleanTenDangNhap.ToLower();

            // Kiểm tra trùng tên đăng nhập
            var isDuplicate = await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap.ToLower() == cleanTenDangNhapLower);
            if (isDuplicate)
            {
                ViewBag.RegisterError = $"Tên đăng nhập '{cleanTenDangNhap}' đã được sử dụng. Vui lòng chọn tên khác.";
                return View("Login");
            }

            // Kiểm tra trùng email nếu có nhập
            if (!string.IsNullOrWhiteSpace(email))
            {
                var cleanEmailLower = email.Trim().ToLower();
                var isEmailDuplicate = await _context.TaiKhoans.AnyAsync(t => t.Email != null && t.Email.ToLower() == cleanEmailLower);
                if (isEmailDuplicate)
                {
                    ViewBag.RegisterError = "Email này đã được sử dụng cho tài khoản khác.";
                    return View("Login");
                }
            }

            // 1. Tạo Tài Khoản (Server quyết định VaiTro = Donor và TrangThai = Hoạt động)
            var newAccount = new TaiKhoan
            {
                TenDangNhap = cleanTenDangNhap,
                HoTen = hoTen.Trim(),
                Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
                VaiTro = "Donor",
                TrangThai = "Hoạt động"
            };

            // Hash mật khẩu
            newAccount.MatKhau = _passwordHasher.HashPassword(newAccount, matKhau);

            _context.TaiKhoans.Add(newAccount);
            await _context.SaveChangesAsync();

            // 2. Tạo Hồ sơ Người Hiến Máu liên kết
            var newDonorProfile = new NguoiHienMau
            {
                MaTaiKhoan = newAccount.MaTaiKhoan,
                HoTen = newAccount.HoTen,
                Email = newAccount.Email,
                SoDienThoai = string.IsNullOrWhiteSpace(soDienThoai) ? null : soDienThoai.Trim(),
                NhomMau = (nhomMau == "unknown" || string.IsNullOrWhiteSpace(nhomMau)) ? null : nhomMau,
                CanNang = canNang,
                TrangThai = "Hoạt động"
            };

            _context.NguoiHienMaus.Add(newDonorProfile);
            await _context.SaveChangesAsync();

            // Tự động đăng nhập sau khi đăng ký
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, newAccount.MaTaiKhoan.ToString()),
                new Claim(ClaimTypes.Name, newAccount.HoTen),
                new Claim(ClaimTypes.Role, newAccount.VaiTro),
                new Claim("TenDangNhap", newAccount.TenDangNhap)
            };

            if (!string.IsNullOrEmpty(newAccount.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, newAccount.Email));
            }

            var claimsIdentity = new ClaimsIdentity(claims, "QuanLyHienMau.Auth");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync("QuanLyHienMau.Auth", new ClaimsPrincipal(claimsIdentity), authProperties);

            TempData["SuccessMessage"] = "Đăng ký tài khoản tình nguyện viên thành công! Chào mừng bạn tham gia cộng đồng.";
            return RedirectToAction("HoSoCaNhan", "NguoiHienMau");
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("QuanLyHienMau.Auth");
            TempData["SuccessMessage"] = "Đã đăng xuất khỏi hệ thống.";
            return RedirectToAction(nameof(Login));
        }

        // GET: /Account/Logout
        [HttpGet]
        public async Task<IActionResult> LogoutGet()
        {
            await HttpContext.SignOutAsync("QuanLyHienMau.Auth");
            TempData["SuccessMessage"] = "Đã đăng xuất khỏi hệ thống.";
            return RedirectToAction(nameof(Login));
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            ViewData["Title"] = "Không Có Quyền Truy Cập (403)";
            return View();
        }

        // GET: /Account/VNeIDAuth
        [HttpGet]
        public IActionResult VNeIDAuth()
        {
            ViewData["Title"] = "Xác Thực CCCD / VNeID Cổng Định Danh Y Tế Số";
            return View();
        }

        private IActionResult RedirectToRoleHome(string? role)
        {
            return role switch
            {
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                "Medical Staff" => RedirectToAction("Dashboard", "Admin"),
                _ => RedirectToAction("HoSoCaNhan", "NguoiHienMau")
            };
        }
    }
}

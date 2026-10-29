// Họ và tên: Nguyễn Tiến Nam
// Mã sinh viên: 23103100275
// Nội dung thực hiện: Module 1 - Tài khoản, Đăng nhập, Phân quyền và Đơn vị tổ chức
// Triển khai Repository Pattern: Controller xử lý Request/Response, Repository làm việc với dữ liệu

using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.Models.ViewModels;
using QuanLyHienMau_UNETI2_TI17A5HN.Repositories;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers
{
    public class AccountController : Controller
    {
        private readonly ITaiKhoanRepository _taiKhoanRepository;
        private readonly PasswordHasher<TaiKhoan> _passwordHasher;
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex PhoneRegex = new(@"^(0[3|5|7|8|9])+([0-9]{8})$", RegexOptions.Compiled);

        public AccountController(ITaiKhoanRepository taiKhoanRepository)
        {
            _taiKhoanRepository = taiKhoanRepository;
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

            // 1. Kiểm tra dữ liệu đầu vào bắt buộc
            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
            {
                ViewBag.LoginError = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return View();
            }

            var trimmedInput = tenDangNhap.Trim();

            // 2. Gọi Repository để truy vấn dữ liệu tài khoản (Controller KHÔNG trực tiếp gọi DbContext)
            var taiKhoan = await _taiKhoanRepository.FindForLoginAsync(trimmedInput);

            if (taiKhoan == null)
            {
                ViewBag.LoginError = "Tài khoản hoặc mật khẩu không chính xác.";
                return View();
            }

            // 3. Nghiệp vụ: Tài khoản bị khóa KHÔNG được phép đăng nhập
            if (taiKhoan.TrangThai == "Bị khóa" || taiKhoan.TrangThai != "Hoạt động")
            {
                ViewBag.LoginError = "Tài khoản của bạn hiện đang bị khóa hoặc tạm dừng. Vui lòng liên hệ ban quản trị để được hỗ trợ.";
                return View();
            }

            // 4. Xác thực mật khẩu
            var verifyResult = _passwordHasher.VerifyHashedPassword(taiKhoan, taiKhoan.MatKhau, matKhau);
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                ViewBag.LoginError = "Tài khoản hoặc mật khẩu không chính xác.";
                return View();
            }

            // 5. Lưu thông tin tối thiểu vào Session theo đúng đặc tả Đề bài (Mục 5.2)
            HttpContext.Session.SetInt32("MaTaiKhoan", taiKhoan.MaTaiKhoan);
            HttpContext.Session.SetString("HoTen", taiKhoan.HoTen);
            HttpContext.Session.SetString("VaiTro", taiKhoan.VaiTro);
            HttpContext.Session.SetString("TenDangNhap", taiKhoan.TenDangNhap);

            // 6. Đăng nhập bằng Cookie Claims để hệ thống [Authorize] hoạt động
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

            // ========================================================
            // KIỂM TRA TÍNH HỢP LỆ CỦA DỮ LIỆU ĐẦU VÀO (VALIDATION)
            // ========================================================

            // 1. Kiểm tra các trường thông tin bắt buộc
            if (string.IsNullOrWhiteSpace(tenDangNhap) ||
                string.IsNullOrWhiteSpace(matKhau) ||
                string.IsNullOrWhiteSpace(hoTen))
            {
                ViewBag.RegisterError = "Vui lòng điền đầy đủ các trường thông tin bắt buộc (*).";
                return View("Login");
            }

            var cleanTenDangNhap = tenDangNhap.Trim();
            var cleanHoTen = hoTen.Trim();

            // 2. Kiểm tra định dạng tên đăng nhập
            if (cleanTenDangNhap.Length < 3 || cleanTenDangNhap.Length > 50)
            {
                ViewBag.RegisterError = "Tên đăng nhập phải có độ dài từ 3 đến 50 ký tự.";
                return View("Login");
            }

            if (cleanTenDangNhap.Contains(' '))
            {
                ViewBag.RegisterError = "Tên đăng nhập không được chứa dấu cách (khoảng trắng).";
                return View("Login");
            }

            // 3. Kiểm tra độ dài mật khẩu và khớp xác nhận
            if (matKhau.Length < 6)
            {
                ViewBag.RegisterError = "Mật khẩu phải có tối thiểu 6 ký tự.";
                return View("Login");
            }

            if (matKhau != xacNhanMatKhau)
            {
                ViewBag.RegisterError = "Mật khẩu xác nhận không khớp.";
                return View("Login");
            }

            // 4. Kiểm tra định dạng email (Bắt buộc đúng định dạng nếu có nhập)
            string? cleanEmail = null;
            if (!string.IsNullOrWhiteSpace(email))
            {
                cleanEmail = email.Trim();
                if (!EmailRegex.IsMatch(cleanEmail))
                {
                    ViewBag.RegisterError = "Địa chỉ email không đúng định dạng (Ví dụ: nguoidung@domain.com).";
                    return View("Login");
                }
            }

            // 5. Kiểm tra định dạng số điện thoại nếu có nhập
            string? cleanPhone = null;
            if (!string.IsNullOrWhiteSpace(soDienThoai))
            {
                cleanPhone = soDienThoai.Trim();
                if (!PhoneRegex.IsMatch(cleanPhone))
                {
                    ViewBag.RegisterError = "Số điện thoại không hợp lệ (Phải là số điện thoại 10 chữ số tại Việt Nam).";
                    return View("Login");
                }
            }

            // 6. Kiểm tra cân nặng hợp lý nếu có nhập
            if (canNang.HasValue && (canNang.Value < 30 || canNang.Value > 200))
            {
                ViewBag.RegisterError = "Cân nặng nhập vào không hợp lệ (khoảng hợp lệ: 30kg - 200kg).";
                return View("Login");
            }

            // 7. Kiểm tra trùng tên đăng nhập thông qua Repository
            var isDuplicateUsername = await _taiKhoanRepository.IsTenDangNhapExistsAsync(cleanTenDangNhap);
            if (isDuplicateUsername)
            {
                ViewBag.RegisterError = $"Tên đăng nhập '{cleanTenDangNhap}' đã được sử dụng. Vui lòng chọn tên khác.";
                return View("Login");
            }

            // 8. Kiểm tra trùng email thông qua Repository
            if (!string.IsNullOrWhiteSpace(cleanEmail))
            {
                var isDuplicateEmail = await _taiKhoanRepository.IsEmailExistsAsync(cleanEmail);
                if (isDuplicateEmail)
                {
                    ViewBag.RegisterError = "Email này đã được sử dụng cho tài khoản khác.";
                    return View("Login");
                }
            }

            // ========================================================
            // TẠO THỰC THỂ VÀ LƯU QUA REPOSITORY
            // ========================================================

            // Khởi tạo Entity TaiKhoan (Hệ thống thiết lập VaiTro = "Donor" và TrangThai = "Hoạt động")
            var newAccount = new TaiKhoan
            {
                TenDangNhap = cleanTenDangNhap,
                HoTen = cleanHoTen,
                Email = cleanEmail,
                VaiTro = "Donor",
                TrangThai = "Hoạt động"
            };

            // Hash mật khẩu an toàn
            newAccount.MatKhau = _passwordHasher.HashPassword(newAccount, matKhau);

            // Khởi tạo hồ sơ người hiến máu liên kết
            var newDonorProfile = new NguoiHienMau
            {
                HoTen = newAccount.HoTen,
                Email = newAccount.Email,
                SoDienThoai = cleanPhone,
                NhomMau = (nhomMau == "unknown" || string.IsNullOrWhiteSpace(nhomMau)) ? null : nhomMau,
                CanNang = canNang,
                TrangThai = "Hoạt động"
            };

            // Thực hiện lưu trữ nguyên tử thông qua Repository
            await _taiKhoanRepository.RegisterDonorAccountAsync(newAccount, newDonorProfile);

            // Lưu thông tin vào Session
            HttpContext.Session.SetInt32("MaTaiKhoan", newAccount.MaTaiKhoan);
            HttpContext.Session.SetString("HoTen", newAccount.HoTen);
            HttpContext.Session.SetString("VaiTro", newAccount.VaiTro);
            HttpContext.Session.SetString("TenDangNhap", newAccount.TenDangNhap);

            // Tự động đăng nhập Claims
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

            TempData["SuccessMessage"] = "Đăng ký tài khoản tình nguyện viên thành công! Chào mừng bạn tham gia cộng đồng hiến máu.";
            return RedirectToAction("HoSoCaNhan", "NguoiHienMau");
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // 1. Xóa toàn bộ Session đăng nhập
            HttpContext.Session.Clear();

            // 2. Đăng xuất khỏi Cookie Authentication
            await HttpContext.SignOutAsync("QuanLyHienMau.Auth");

            TempData["SuccessMessage"] = "Đã đăng xuất khỏi hệ thống thành công.";
            return RedirectToAction(nameof(Login));
        }

        // GET: /Account/Logout
        [HttpGet]
        public async Task<IActionResult> LogoutGet()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync("QuanLyHienMau.Auth");
            TempData["SuccessMessage"] = "Đã đăng xuất khỏi hệ thống thành công.";
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

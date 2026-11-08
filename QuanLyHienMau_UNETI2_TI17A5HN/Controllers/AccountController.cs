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
        private const string AuthScheme = "QuanLyHienMau.Auth";
        private readonly ITaiKhoanRepository _taiKhoanRepository;
        private readonly PasswordHasher<TaiKhoan> _passwordHasher;

        private static readonly Regex UsernameRegex = new(@"^[a-zA-Z0-9_.-]+$", RegexOptions.Compiled);
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex PhoneRegex = new(@"^(0[3|5|7|8|9])+([0-9]{8})$", RegexOptions.Compiled);

        public AccountController(ITaiKhoanRepository taiKhoanRepository)
        {
            _taiKhoanRepository = taiKhoanRepository;
            _passwordHasher = new PasswordHasher<TaiKhoan>();
        }

        // GET: /Account
        public IActionResult Index() => RedirectToAction(nameof(Login));

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToRoleHome(User.FindFirstValue(ClaimTypes.Role));
            }

            ViewData["Title"] = "Đăng Nhập & Đăng Ký Cổng Hiến Máu";
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            ViewData["Title"] = "Đăng Nhập & Đăng Ký Cổng Hiến Máu";
            ViewData["ReturnUrl"] = model.ReturnUrl;
            ViewBag.LoginTenDangNhap = model.TenDangNhap;

            // 1. Kiểm tra dữ liệu đầu vào bắt buộc
            if (string.IsNullOrWhiteSpace(model.TenDangNhap) || string.IsNullOrWhiteSpace(model.MatKhau))
            {
                ViewBag.LoginError = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return View();
            }

            var trimmedInput = model.TenDangNhap.Trim();

            // 2. Truy vấn tài khoản qua Repository
            var taiKhoan = await _taiKhoanRepository.FindForLoginAsync(trimmedInput);
            if (taiKhoan == null)
            {
                ViewBag.LoginError = "Tài khoản hoặc mật khẩu không chính xác.";
                return View();
            }

            // 3. Nghiệp vụ: Tài khoản bị khóa KHÔNG được phép đăng nhập
            if (taiKhoan.TrangThai != "Hoạt động")
            {
                ViewBag.LoginError = "Tài khoản của bạn hiện đang bị khóa hoặc tạm dừng. Vui lòng liên hệ ban quản trị để được hỗ trợ.";
                return View();
            }

            // 4. Xác thực mật khẩu băm (PasswordHasher)
            var verifyResult = _passwordHasher.VerifyHashedPassword(taiKhoan, taiKhoan.MatKhau, model.MatKhau);
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                ViewBag.LoginError = "Tài khoản hoặc mật khẩu không chính xác.";
                return View();
            }

            // 5. Thiết lập Session & Cookie Authentication
            await SignInUserAsync(taiKhoan, model.RememberMe);

            TempData["SuccessMessage"] = $"Chào mừng {taiKhoan.HoTen} đã đăng nhập thành công!";

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToRoleHome(taiKhoan.VaiTro);
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToRoleHome(User.FindFirstValue(ClaimTypes.Role));
            }

            ViewData["Title"] = "Đăng Ký Tài Khoản Tình Nguyện Viên";
            ViewBag.ActiveTab = "register";
            return View("Login");
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ViewData["Title"] = "Đăng Ký Tài Khoản Tình Nguyện Viên";
            ViewBag.ActiveTab = "register";
            ViewBag.RegTenDangNhap = model.TenDangNhap;
            ViewBag.RegHoTen = model.HoTen;
            ViewBag.RegEmail = model.Email;
            ViewBag.RegSoDienThoai = model.SoDienThoai;
            ViewBag.RegNhomMau = model.NhomMau;
            ViewBag.RegCanNang = model.CanNang;
            ViewBag.RegDongYDieuKhoan = true;

            // 1. Kiểm tra các trường thông tin bắt buộc
            if (string.IsNullOrWhiteSpace(model.TenDangNhap) ||
                string.IsNullOrWhiteSpace(model.MatKhau) ||
                string.IsNullOrWhiteSpace(model.HoTen))
            {
                ViewBag.RegisterError = "Vui lòng điền đầy đủ các trường thông tin bắt buộc (*).";
                return View("Login");
            }

            var cleanTenDangNhap = model.TenDangNhap.Trim();
            var cleanHoTen = model.HoTen.Trim();

            // 2. Kiểm tra định dạng và độ dài tên đăng nhập & họ tên
            if (cleanTenDangNhap.Length < 3 || cleanTenDangNhap.Length > 50)
            {
                ViewBag.RegisterError = "Tên đăng nhập phải có độ dài từ 3 đến 50 ký tự.";
                return View("Login");
            }

            if (!UsernameRegex.IsMatch(cleanTenDangNhap))
            {
                ViewBag.RegisterError = "Tên đăng nhập chỉ được chứa chữ cái, chữ số, dấu gạch dưới (_), gạch ngang (-) và dấu chấm (.), không chứa khoảng trắng.";
                return View("Login");
            }

            if (cleanHoTen.Length > 100)
            {
                ViewBag.RegisterError = "Họ và tên không được vượt quá 100 ký tự.";
                return View("Login");
            }

            // 3. Kiểm tra độ dài mật khẩu và khớp xác nhận
            if (model.MatKhau.Length < 6)
            {
                ViewBag.RegisterError = "Mật khẩu phải có tối thiểu 6 ký tự.";
                return View("Login");
            }

            if (model.MatKhau != model.XacNhanMatKhau)
            {
                ViewBag.RegisterError = "Mật khẩu xác nhận không khớp.";
                return View("Login");
            }

            // 4. Kiểm tra định dạng email nếu có nhập
            string? cleanEmail = null;
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                cleanEmail = model.Email.Trim();
                if (cleanEmail.Length > 100 || !EmailRegex.IsMatch(cleanEmail))
                {
                    ViewBag.RegisterError = "Địa chỉ email không đúng định dạng (Ví dụ: nguoidung@domain.com).";
                    return View("Login");
                }
            }

            // 5. Kiểm tra định dạng số điện thoại nếu có nhập
            string? cleanPhone = null;
            if (!string.IsNullOrWhiteSpace(model.SoDienThoai))
            {
                cleanPhone = model.SoDienThoai.Trim();
                if (!PhoneRegex.IsMatch(cleanPhone))
                {
                    ViewBag.RegisterError = "Số điện thoại không hợp lệ (Phải là số điện thoại 10 chữ số tại Việt Nam).";
                    return View("Login");
                }
            }

            // 6. Kiểm tra cân nặng hợp lý nếu có nhập
            if (model.CanNang.HasValue && (model.CanNang.Value < 30 || model.CanNang.Value > 200))
            {
                ViewBag.RegisterError = "Cân nặng nhập vào không hợp lệ (khoảng hợp lệ: 30kg - 200kg).";
                return View("Login");
            }

            // 7. Kiểm tra trùng tên đăng nhập qua Repository
            if (await _taiKhoanRepository.IsTenDangNhapExistsAsync(cleanTenDangNhap))
            {
                ViewBag.RegisterError = $"Tên đăng nhập '{cleanTenDangNhap}' đã được sử dụng. Vui lòng chọn tên khác.";
                return View("Login");
            }

            // 8. Kiểm tra trùng email qua Repository
            if (!string.IsNullOrWhiteSpace(cleanEmail) && await _taiKhoanRepository.IsEmailExistsAsync(cleanEmail))
            {
                ViewBag.RegisterError = "Email này đã được sử dụng cho tài khoản khác.";
                return View("Login");
            }

            // 9. Kiểm tra trùng số điện thoại qua Repository
            if (!string.IsNullOrWhiteSpace(cleanPhone) && await _taiKhoanRepository.IsSoDienThoaiExistsAsync(cleanPhone))
            {
                ViewBag.RegisterError = "Số điện thoại này đã được liên kết với một tài khoản khác.";
                return View("Login");
            }

            // 10. Khởi tạo Entity TaiKhoan & NguoiHienMau, lưu nguyên tử qua Repository
            var newAccount = new TaiKhoan
            {
                TenDangNhap = cleanTenDangNhap,
                HoTen = cleanHoTen,
                Email = cleanEmail,
                VaiTro = "Donor",
                TrangThai = "Hoạt động"
            };
            newAccount.MatKhau = _passwordHasher.HashPassword(newAccount, model.MatKhau);

            var newDonorProfile = new NguoiHienMau
            {
                HoTen = newAccount.HoTen,
                Email = newAccount.Email,
                SoDienThoai = cleanPhone,
                NhomMau = (model.NhomMau == "unknown" || string.IsNullOrWhiteSpace(model.NhomMau)) ? null : model.NhomMau.Trim(),
                CanNang = model.CanNang,
                TrangThai = "Hoạt động"
            };

            await _taiKhoanRepository.RegisterDonorAccountAsync(newAccount, newDonorProfile);

            // 11. Tự động đăng nhập Session & Claims
            await SignInUserAsync(newAccount, isPersistent: true);

            TempData["SuccessMessage"] = "Đăng ký tài khoản tình nguyện viên thành công! Chào mừng bạn tham gia cộng đồng hiến máu.";
            return RedirectToAction("HoSoCaNhan", "NguoiHienMau");
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await SignOutCurrentUserAsync();
            return RedirectToAction(nameof(Login));
        }

        // GET: /Account/Logout
        [HttpGet, ActionName("Logout")]
        public async Task<IActionResult> LogoutGet()
        {
            await SignOutCurrentUserAsync();
            return RedirectToAction(nameof(Login));
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            ViewData["Title"] = "Từ Chối Quyền Truy Cập";
            return View();
        }

        private async Task SignInUserAsync(TaiKhoan taiKhoan, bool isPersistent)
        {
            // Lưu thông tin vào Session theo đặc tả Đề bài (Mục 5.2)
            HttpContext.Session.SetInt32("MaTaiKhoan", taiKhoan.MaTaiKhoan);
            HttpContext.Session.SetString("HoTen", taiKhoan.HoTen);
            HttpContext.Session.SetString("VaiTro", taiKhoan.VaiTro);
            HttpContext.Session.SetString("TenDangNhap", taiKhoan.TenDangNhap);

            // Thiết lập Claims cho Cookie Authentication
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, taiKhoan.MaTaiKhoan.ToString()),
                new(ClaimTypes.Name, taiKhoan.HoTen),
                new(ClaimTypes.Role, taiKhoan.VaiTro),
                new("TenDangNhap", taiKhoan.TenDangNhap)
            };

            if (!string.IsNullOrEmpty(taiKhoan.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, taiKhoan.Email));
            }

            var claimsIdentity = new ClaimsIdentity(claims, AuthScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                ExpiresUtc = isPersistent ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(12)
            };

            await HttpContext.SignInAsync(AuthScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        private async Task SignOutCurrentUserAsync()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(AuthScheme);
            TempData["SuccessMessage"] = "Đã đăng xuất khỏi hệ thống thành công.";
        }

        private IActionResult RedirectToRoleHome(string? role)
        {
            return role switch
            {
                "Admin" or "Medical Staff" => RedirectToAction("Dashboard", "Admin"),
                _ => RedirectToAction("HoSoCaNhan", "NguoiHienMau")
            };
        }
    }
}

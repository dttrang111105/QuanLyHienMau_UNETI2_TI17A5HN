// Họ và tên: Nguyễn Tiến Nam
// Mã sinh viên: 23103100275
// Nội dung thực hiện: Module 1 - Controller Quản lý Tài khoản & Phân quyền
// Triển khai Repository Pattern: Controller xử lý Request/Response, Repository làm việc với dữ liệu

using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.Models.ViewModels;
using QuanLyHienMau_UNETI2_TI17A5HN.Repositories;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TaiKhoanController : Controller
    {
        private readonly ITaiKhoanRepository _taiKhoanRepository;
        private readonly PasswordHasher<TaiKhoan> _passwordHasher;

        private static readonly Regex UsernameRegex = new(@"^[a-zA-Z0-9_.-]+$", RegexOptions.Compiled);
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly HashSet<string> AllowedRoles = new(StringComparer.Ordinal) { "Admin", "Medical Staff", "Donor" };
        private static readonly HashSet<string> AllowedStatuses = new(StringComparer.Ordinal) { "Hoạt động", "Bị khóa" };

        public TaiKhoanController(ITaiKhoanRepository taiKhoanRepository)
        {
            _taiKhoanRepository = taiKhoanRepository;
            _passwordHasher = new PasswordHasher<TaiKhoan>();
        }

        // GET: /TaiKhoan
        public async Task<IActionResult> Index(string? search, string? vaiTro, string? trangThai)
        {
            var accounts = await _taiKhoanRepository.GetAllWithProfileAsync(search, vaiTro, trangThai);
            var stats = await _taiKhoanRepository.GetAccountStatsAsync();

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentVaiTro = vaiTro;
            ViewBag.CurrentTrangThai = trangThai;
            ViewBag.TotalAccounts = stats.TotalCount;
            ViewBag.AdminCount = stats.AdminCount;
            ViewBag.MedicalStaffCount = stats.MedicalStaffCount;
            ViewBag.DonorCount = stats.DonorCount;
            ViewBag.ActiveCount = stats.ActiveCount;
            ViewBag.LockedCount = stats.LockedCount;

            return View("~/Views/Admin/NhanSu.cshtml", accounts);
        }

        // POST: /TaiKhoan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAccountAdminViewModel model)
        {
            // 1. Kiểm tra các trường thông tin bắt buộc
            if (string.IsNullOrWhiteSpace(model.TenDangNhap) ||
                string.IsNullOrWhiteSpace(model.MatKhau) ||
                string.IsNullOrWhiteSpace(model.HoTen))
            {
                SetModalError("Vui lòng điền đầy đủ các thông tin bắt buộc (*).", model);
                return RedirectToAction(nameof(Index));
            }

            var cleanUsername = model.TenDangNhap.Trim();
            var cleanHoTen = model.HoTen.Trim();

            // 2. Kiểm tra độ dài và định dạng tên đăng nhập
            if (cleanUsername.Length < 3 || cleanUsername.Length > 50)
            {
                SetModalError("Tên đăng nhập phải có độ dài từ 3 đến 50 ký tự.", model);
                return RedirectToAction(nameof(Index));
            }

            if (!UsernameRegex.IsMatch(cleanUsername))
            {
                SetModalError("Tên đăng nhập chỉ được chứa chữ cái, chữ số, dấu gạch dưới (_), gạch ngang (-) và dấu chấm (.), không chứa khoảng trắng.", model);
                return RedirectToAction(nameof(Index));
            }

            // 3. Kiểm tra độ dài mật khẩu khởi tạo
            if (model.MatKhau.Length < 6)
            {
                SetModalError("Mật khẩu khởi tạo phải có tối thiểu 6 ký tự.", model);
                return RedirectToAction(nameof(Index));
            }

            // 4. Kiểm tra độ dài họ tên
            if (cleanHoTen.Length > 100)
            {
                SetModalError("Họ và tên không được vượt quá 100 ký tự.", model);
                return RedirectToAction(nameof(Index));
            }

            // 5. Kiểm tra định dạng email nếu có nhập
            string? cleanEmail = null;
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                cleanEmail = model.Email.Trim();
                if (cleanEmail.Length > 100 || !EmailRegex.IsMatch(cleanEmail))
                {
                    SetModalError("Địa chỉ email không đúng định dạng (Ví dụ: nguoidung@domain.com).", model);
                    return RedirectToAction(nameof(Index));
                }
            }

            // 6. Kiểm tra trùng tên đăng nhập qua Repository
            if (await _taiKhoanRepository.IsTenDangNhapExistsAsync(cleanUsername))
            {
                SetModalError($"Tên đăng nhập '{cleanUsername}' đã được sử dụng. Vui lòng chọn tên khác.", model);
                return RedirectToAction(nameof(Index));
            }

            // 7. Kiểm tra trùng email qua Repository nếu có nhập email
            if (!string.IsNullOrWhiteSpace(cleanEmail) && await _taiKhoanRepository.IsEmailExistsAsync(cleanEmail))
            {
                SetModalError("Địa chỉ email này đã được sử dụng cho tài khoản khác.", model);
                return RedirectToAction(nameof(Index));
            }

            // 8. Chuẩn hóa Vai trò & Trạng thái hợp lệ
            var role = (!string.IsNullOrWhiteSpace(model.VaiTro) && AllowedRoles.Contains(model.VaiTro.Trim()))
                ? model.VaiTro.Trim()
                : "Medical Staff";
            var status = (!string.IsNullOrWhiteSpace(model.TrangThai) && AllowedStatuses.Contains(model.TrangThai.Trim()))
                ? model.TrangThai.Trim()
                : "Hoạt động";

            var account = new TaiKhoan
            {
                TenDangNhap = cleanUsername,
                HoTen = cleanHoTen,
                Email = cleanEmail,
                VaiTro = role,
                TrangThai = status
            };

            account.MatKhau = _passwordHasher.HashPassword(account, model.MatKhau);

            if (account.VaiTro == "Donor")
            {
                var donorProfile = new NguoiHienMau
                {
                    HoTen = account.HoTen,
                    Email = account.Email,
                    TrangThai = account.TrangThai
                };
                await _taiKhoanRepository.RegisterDonorAccountAsync(account, donorProfile);
            }
            else
            {
                await _taiKhoanRepository.AddAsync(account);
                await _taiKhoanRepository.SaveChangesAsync();
            }

            var tenVaiTro = account.VaiTro switch
            {
                "Admin" => "Quản trị viên",
                "Medical Staff" => "Nhân viên y tế",
                _ => "Người hiến máu"
            };
            TempData["SuccessMessage"] = $"Đã tạo mới tài khoản '{account.TenDangNhap}' ({tenVaiTro}) thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /TaiKhoan/ToggleLock/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLock(int id, string? returnUrl)
        {
            var account = await _taiKhoanRepository.GetByIdAsync(id);
            if (account == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản.";
                return RedirectToLocalOrDefault(returnUrl);
            }

            // Ngăn chặn Quản trị viên tự khóa tài khoản đang đăng nhập của chính mình
            var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(currentUserIdStr, out int currentUserId) && currentUserId == id && account.TrangThai == "Hoạt động")
            {
                TempData["ErrorMessage"] = "Không thể tự khóa tài khoản Quản trị viên đang đăng nhập của chính bạn.";
                return RedirectToLocalOrDefault(returnUrl);
            }

            var newStatus = account.TrangThai == "Hoạt động" ? "Bị khóa" : "Hoạt động";
            await _taiKhoanRepository.UpdateAccountStatusAsync(id, newStatus);

            var actionName = newStatus == "Bị khóa" ? "Khóa" : "Mở khóa";
            TempData["SuccessMessage"] = $"Đã {actionName} thành công tài khoản '{account.TenDangNhap}'.";

            return RedirectToLocalOrDefault(returnUrl);
        }

        // POST: /TaiKhoan/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(int id, string matKhauMoi)
        {
            if (string.IsNullOrWhiteSpace(matKhauMoi) || matKhauMoi.Length < 6)
            {
                TempData["ErrorMessage"] = "Mật khẩu mới phải có tối thiểu 6 ký tự.";
                return RedirectToAction(nameof(Index));
            }

            var account = await _taiKhoanRepository.GetByIdAsync(id);
            if (account == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản.";
                return RedirectToAction(nameof(Index));
            }

            var hashed = _passwordHasher.HashPassword(account, matKhauMoi);
            await _taiKhoanRepository.ChangePasswordAsync(id, hashed);

            TempData["SuccessMessage"] = $"Đã đặt lại mật khẩu cho tài khoản '{account.TenDangNhap}' thành công.";
            return RedirectToAction(nameof(Index));
        }

        private void SetModalError(string errorMsg, CreateAccountAdminViewModel model)
        {
            TempData["CreateAccountError"] = errorMsg;
            TempData["ErrorMessage"] = errorMsg;
            TempData["CreateTenDangNhap"] = model.TenDangNhap;
            TempData["CreateHoTen"] = model.HoTen;
            TempData["CreateEmail"] = model.Email;
            TempData["CreateVaiTro"] = model.VaiTro;
            TempData["CreateTrangThai"] = model.TrangThai;
            TempData["OpenCreateModal"] = true;
        }

        private IActionResult RedirectToLocalOrDefault(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

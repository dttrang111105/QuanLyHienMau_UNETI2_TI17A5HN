// Họ và tên: ĐIỀN HỌ TÊN CỦA BẠN
// Mã sinh viên: ĐIỀN MSSV CỦA BẠN
// Nội dung thực hiện: bốn action Module 5; các action khác giữ nguyên bản gốc.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QuanLyHienMau_UNETI2_TI17A5HN.Services;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers
{
    public class AdminController(Module5Service module5, Module5Pages pages) : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        [Authorize(Roles = "Admin,NhanVienYTe")]
        public async Task<IActionResult> Dashboard()
        {
            return View(await module5.Dashboard());
        }

        [Authorize(Roles = "Admin,NhanVienYTe")]
        public async Task<IActionResult> LichKhamSangLoc(string? q, string? trangThai, DateTime? tuNgay, int page = 1, string view = "calendar", DateTime? ngay = null)
        {
            return await pages.Lich(this, q, trangThai, tuNgay, page, view, ngay);
        }

        public IActionResult HoSoBenhSu()
        {
            return View();
        }

        [Authorize(Roles = "Admin,NhanVienYTe")]
        public async Task<IActionResult> KetQuaXetNghiem()
        {
            return await pages.KetQua(this);
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

        [Authorize(Roles = "Admin,NhanVienYTe")]
        public async Task<IActionResult> BaoCao(int? nam)
        {
            var year = nam ?? module5.Now.Year;
            if (year < 2000 || year > 2100)
                return BadRequest("Năm không hợp lệ.");
            return View(await module5.ThongKe(year));
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

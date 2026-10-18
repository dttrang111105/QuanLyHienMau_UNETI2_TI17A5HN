// Họ và tên: ĐIỀN HỌ TÊN CỦA BẠN
// Mã sinh viên: ĐIỀN MSSV CỦA BẠN
// Nội dung: Module 5 và phần nền dữ liệu cần thiết để chạy thử.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.Services;
using QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers;
[Authorize(Roles = "Admin,NhanVienYTe")]
[Microsoft.AspNetCore.Mvc.ServiceFilter(typeof(QuanLyHienMau_UNETI2_TI17A5HN.Infrastructure.Module5DatabaseExceptionFilter))]
public class KetQuaHienMauController(ApplicationDbContext db, Module5Service service) : Controller
{
    public IActionResult Index() => RedirectToAction("KetQuaXetNghiem", "Admin");
    [HttpGet]
    public async Task<IActionResult> Create(int id)
    {
        var p = await db.PhieuDangKyHienMaus.Include(x => x.NguoiHien).Include(x => x.Dot).SingleOrDefaultAsync(x => x.MaPhieu == id);
        if (p == null)
            return NotFound();
        if (p.TrangThai != TrangThaiPhieu.DaKham)
            return BadRequest("Chỉ nhập kết quả cho phiếu Đã khám sàng lọc.");
        return View(new KetQuaFormVm { MaPhieu = id, ThongTinPhieu = $"#{id} • {p.NguoiHien.HoTen} • {p.Dot.TenDot}" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(KetQuaFormVm vm)
    {
        if (ModelState.IsValid)
        {
            var e = await service.LuuKetQua(vm);
            if (e == null)
            {
                var p = await db.PhieuDangKyHienMaus.Include(x => x.Dot).SingleAsync(x => x.MaPhieu == vm.MaPhieu);
                var n = await db.PhieuDangKyHienMaus.CountAsync(x => x.MaDot == p.MaDot && x.TrangThai == TrangThaiPhieu.ThanhCong);
                TempData["Ok"] = "Đã ghi nhận kết quả hiến máu.";
                if (n >= p.Dot.ChiTieuDonViMau)
                    TempData["Warning"] = $"Đợt {p.Dot.TenDot} đã đạt chỉ tiêu ({n}/{p.Dot.ChiTieuDonViMau}). Trao đổi với người phụ trách đợt để đóng đăng ký nếu cần.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", e);
        }

        var info = await db.PhieuDangKyHienMaus.Include(x => x.NguoiHien).SingleOrDefaultAsync(x => x.MaPhieu == vm.MaPhieu);
        vm.ThongTinPhieu = info == null ? "Phiếu không tồn tại" : $"#{info.MaPhieu} • {info.NguoiHien.HoTen}";
        return View(vm);
    }
}

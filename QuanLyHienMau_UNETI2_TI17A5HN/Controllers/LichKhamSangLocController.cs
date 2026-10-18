// Họ và tên: ĐIỀN HỌ TÊN CỦA BẠN
// Mã sinh viên: ĐIỀN MSSV CỦA BẠN
// Nội dung: Module 5 và phần nền dữ liệu cần thiết để chạy thử.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.Services;
using QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers;
[Authorize(Roles = "Admin,NhanVienYTe")]
[Microsoft.AspNetCore.Mvc.ServiceFilter(typeof(QuanLyHienMau_UNETI2_TI17A5HN.Infrastructure.Module5DatabaseExceptionFilter))]
public class LichKhamSangLocController(ApplicationDbContext db, Module5Service service) : Controller
{
    public IActionResult Index(string? q, string? trangThai, DateTime? tuNgay, int page = 1, string view = "calendar", DateTime? ngay = null) => RedirectToAction("LichKhamSangLoc", "Admin", new { q, trangThai, tuNgay, page, view, ngay });
    async Task Options(LichFormVm vm)
    {
        var ps = await db.PhieuDangKyHienMaus.AsNoTracking().Include(x => x.NguoiHien).Include(x => x.Dot).Where(x => (x.TrangThai == TrangThaiPhieu.DaXacNhan && !x.LichKhams.Any(l => l.TrangThai != TrangThaiLich.DaHuy)) || (vm.Id != 0 && x.MaPhieu == vm.MaPhieu)).OrderBy(x => x.MaPhieu).ToListAsync();
        vm.Phieus = ps.Select(x => new SelectListItem($"#{x.MaPhieu} • {x.NguoiHien.HoTen} • {x.Dot.TenDot}", x.MaPhieu.ToString())).ToList();
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new LichFormVm
        {
            ThoiGianBatDau = service.Now.Date.AddDays(1).AddHours(9),
            ThoiGianKetThuc = service.Now.Date.AddDays(1).AddHours(10)
        };
        await Options(vm);
        return View("Form", vm);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var l = await db.LichKhamSangLocs.AsNoTracking().SingleOrDefaultAsync(x => x.MaLichKhamSangLoc == id);
        if (l == null)
            return NotFound();
        if (l.TrangThai != TrangThaiLich.DaLenLich)
            return BadRequest("Lịch không được phép sửa.");
        var vm = new LichFormVm
        {
            Id = id,
            MaPhieu = l.MaPhieu,
            ThoiGianBatDau = l.ThoiGianBatDau,
            ThoiGianKetThuc = l.ThoiGianKetThuc,
            BacSiSangLoc = l.BacSiSangLoc,
            DiaDiemHoacLienKet = l.DiaDiemHoacLienKet,
            HinhThucTiepNhan = l.HinhThucTiepNhan,
            GhiChu = l.GhiChu
        };
        await Options(vm);
        return View("Form", vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(LichFormVm vm)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var error = await service.LuuLich(vm);
                if (error == null)
                {
                    TempData["Ok"] = "Đã lưu lịch và cập nhật trạng thái phiếu.";
                    return RedirectToAction("LichKhamSangLoc", "Admin", new { ngay = vm.ThoiGianBatDau, view = "calendar" });
                }

                ModelState.AddModelError("", error);
            }
            catch (Exception ex)when (ex is DbUpdateException || ex is System.Data.Common.DbException)
            {
                ModelState.AddModelError("", "Dữ liệu vừa thay đổi hoặc lịch bị trùng. Hãy tải lại và kiểm tra.");
            }
        }

        await Options(vm);
        return View("Form", vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id, string? returnUrl)
    {
        var e = await service.DoiTrangThaiLich(id, false);
        TempData[e == null ? "Ok" : "Error"] = e ?? "Đã hủy lịch. Phiếu trở về Đã xác nhận để có thể lập lịch mới.";
        return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl!) : RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id, string? returnUrl)
    {
        var e = await service.DoiTrangThaiLich(id, true);
        TempData[e == null ? "Ok" : "Error"] = e ?? "Đã hoàn thành khám và cập nhật phiếu Đã khám sàng lọc.";
        return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl!) : RedirectToAction(nameof(Index));
    }
}

using Microsoft.AspNetCore.Mvc;
using QuanLyHienMau_UNETI2_TI17A5HN.Repositories;
using QuanLyHienMau_UNETI2_TI17A5HN.Filters;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.Services;
using QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers;

[KiemTraQuyen]
public class LichKhamSangLocController(LichKhamSangLocRepository repository, KhamSangLocService service, DongHoHeThong clock) : Controller
{
    private TaiKhoan TaiKhoan => (TaiKhoan)HttpContext.Items[typeof(TaiKhoan)]!;
    private bool LaNhanVien => TrangThaiHienMau.VaiTro.LaNhanVien(TaiKhoan.VaiTro);

    [HttpGet, KiemTraQuyen(ChiNhanVien = true)]
    public async Task<IActionResult> Index(string? search, string? trangThai, int? maDot, DateTime? ngay, int page = 1) =>
        View("Index", await repository.DanhSachAsync(search, trangThai, maDot, ngay, page));

    [HttpGet]
    public async Task<IActionResult> CuaToi(string? search, string? trangThai, int? maDot, DateTime? ngay, int page = 1) =>
        View("Index", await repository.DanhSachAsync(search, trangThai, maDot, ngay, page, TaiKhoan.MaTaiKhoan));

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var lich = await repository.ChiTietAsync(id, LaNhanVien ? null : TaiKhoan.MaTaiKhoan);
        if (lich is null) return NotFound();
        ViewData["LaNhanVien"] = LaNhanVien;
        return View(lich);
    }

    [HttpGet, KiemTraQuyen(ChiNhanVien = true)]
    public async Task<IActionResult> Create(int? maPhieu)
    {
        var batDau = clock.Now.AddHours(1);
        var model = new LichKhamFormViewModel
        {
            MaPhieu = maPhieu ?? 0,
            ThoiGianBatDau = new DateTime(batDau.Year, batDau.Month, batDau.Day, batDau.Hour, batDau.Minute, 0),
            ThoiGianKetThuc = new DateTime(batDau.Year, batDau.Month, batDau.Day, batDau.Hour, batDau.Minute, 0).AddMinutes(30)
        };
        await repository.NapLuaChonAsync(model);
        return View("Form", model);
    }

    [HttpPost, ValidateAntiForgeryToken, KiemTraQuyen(ChiNhanVien = true)]
    public async Task<IActionResult> Create(LichKhamFormViewModel model)
    {
        model.MaLichKhamSangLoc = null;
        if (ModelState.IsValid)
        {
            var result = await service.LuuLichAsync(model);
            if (result.Success)
            {
                TempData["ThongBaoThanhCong"] = result.Message;
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }
            ModelState.AddModelError("", result.Message);
        }
        await repository.NapLuaChonAsync(model);
        return View("Form", model);
    }

    [HttpGet, KiemTraQuyen(ChiNhanVien = true)]
    public async Task<IActionResult> Edit(int id)
    {
        var lich = await repository.LichDeSuaAsync(id);
        if (lich is null) return NotFound();
        if (lich.TrangThai != TrangThaiHienMau.LichKham.DaLenLich ||
            lich.PhieuDangKyHienMau?.TrangThai != TrangThaiHienMau.Phieu.ChoKhamSangLoc)
        {
            TempData["ThongBaoLoi"] = "Lịch không còn đủ điều kiện chỉnh sửa.";
            return RedirectToAction(nameof(Details), new { id });
        }
        var model = new LichKhamFormViewModel
        {
            MaLichKhamSangLoc = id, MaPhieu = lich.MaPhieu,
            ThoiGianBatDau = lich.ThoiGianBatDau, ThoiGianKetThuc = lich.ThoiGianKetThuc,
            HinhThucTiepNhan = lich.HinhThucTiepNhan ?? "Trực tiếp",
            DiaDiemHoacLienKet = lich.DiaDiemHoacLienKet ?? "",
            BacSiSangLoc = lich.BacSiSangLoc ?? "", GhiChu = lich.GhiChu
        };
        await repository.NapLuaChonAsync(model);
        return View("Form", model);
    }

    [HttpPost, ValidateAntiForgeryToken, KiemTraQuyen(ChiNhanVien = true)]
    public async Task<IActionResult> Edit(int id, LichKhamFormViewModel model)
    {
        model.MaLichKhamSangLoc = id;
        if (ModelState.IsValid)
        {
            var result = await service.LuuLichAsync(model, id);
            if (result.Success)
            {
                TempData["ThongBaoThanhCong"] = result.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
            ModelState.AddModelError("", result.Message);
        }
        await repository.NapLuaChonAsync(model);
        return View("Form", model);
    }

    [HttpPost, ValidateAntiForgeryToken, KiemTraQuyen(ChiNhanVien = true)]
    public async Task<IActionResult> Huy(int id)
    {
        var result = await service.HuyLichAsync(id);
        TempData[result.Success ? "ThongBaoThanhCong" : "ThongBaoLoi"] = result.Message;
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, KiemTraQuyen(ChiNhanVien = true)]
    public async Task<IActionResult> HoanThanh(int id)
    {
        var result = await service.HoanThanhLichAsync(id);
        TempData[result.Success ? "ThongBaoThanhCong" : "ThongBaoLoi"] = result.Message;
        return RedirectToAction(nameof(Details), new { id });
    }
}

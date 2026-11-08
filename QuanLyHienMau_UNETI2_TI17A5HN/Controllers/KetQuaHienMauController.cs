using Microsoft.AspNetCore.Mvc;
using QuanLyHienMau_UNETI2_TI17A5HN.Repositories;
using QuanLyHienMau_UNETI2_TI17A5HN.Filters;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.Services;
using QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers;

[KiemTraQuyen]
public class KetQuaHienMauController(KetQuaHienMauRepository repository, KhamSangLocService service) : Controller
{
    private TaiKhoan TaiKhoan => (TaiKhoan)HttpContext.Items[typeof(TaiKhoan)]!;
    private bool LaNhanVien => TrangThaiHienMau.VaiTro.LaNhanVien(TaiKhoan.VaiTro);

    [HttpGet, KiemTraQuyen(ChiNhanVien = true)]
    public async Task<IActionResult> Index(string? search, string? ketQua, int? maDot, int page = 1) =>
        View("Index", await repository.DanhSachAsync(search, ketQua, maDot, page));

    [HttpGet]
    public async Task<IActionResult> CuaToi(string? search, string? ketQua, int? maDot, int page = 1) =>
        View("Index", await repository.DanhSachAsync(search, ketQua, maDot, page, TaiKhoan.MaTaiKhoan));

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var ketQua = await repository.ChiTietAsync(id, LaNhanVien ? null : TaiKhoan.MaTaiKhoan);
        if (ketQua is null) return NotFound();
        ViewData["LaNhanVien"] = LaNhanVien;
        return View(ketQua);
    }

    [HttpGet, KiemTraQuyen(ChiNhanVien = true)]
    public async Task<IActionResult> Create(int? maPhieu)
    {
        var model = new KetQuaFormViewModel { MaPhieu = maPhieu ?? 0 };
        await repository.NapLuaChonAsync(model);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, KiemTraQuyen(ChiNhanVien = true)]
    public async Task<IActionResult> Create(KetQuaFormViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await service.GhiKetQuaAsync(model);
            if (result.Success)
            {
                TempData["ThongBaoThanhCong"] = result.Message;
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }
            ModelState.AddModelError("", result.Message);
        }
        await repository.NapLuaChonAsync(model);
        return View(model);
    }
}

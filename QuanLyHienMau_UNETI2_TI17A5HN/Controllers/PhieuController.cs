using Microsoft.AspNetCore.Mvc;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers;

public class PhieuController(KhoPhieuDangKy kho) : Controller
{
    [HttpGet]
    public IActionResult Index(string? hoTen, string? tenDot, string? donVi, string? trangThai, DateTime? tuNgay, DateTime? denNgay, string sapXep = "ngaymoi")
    {
        var all = kho.TatCa();
        var filtered = all.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(hoTen)) filtered = filtered.Where(p => p.HoTenNguoiHien.Contains(hoTen.Trim(), StringComparison.CurrentCultureIgnoreCase));
        if (!string.IsNullOrWhiteSpace(tenDot)) filtered = filtered.Where(p => p.TenDot.Contains(tenDot.Trim(), StringComparison.CurrentCultureIgnoreCase));
        if (!string.IsNullOrWhiteSpace(donVi)) filtered = filtered.Where(p => p.DonViToChuc == donVi);
        if (!string.IsNullOrWhiteSpace(trangThai)) filtered = filtered.Where(p => p.TrangThai == trangThai);
        if (tuNgay.HasValue) filtered = filtered.Where(p => p.NgayNop.Date >= tuNgay.Value.Date);
        if (denNgay.HasValue) filtered = filtered.Where(p => p.NgayNop.Date <= denNgay.Value.Date);
        filtered = sapXep switch
        {
            "ngaycu" => filtered.OrderBy(p => p.NgayNop),
            "ten" => filtered.OrderBy(p => p.HoTenNguoiHien),
            _ => filtered.OrderByDescending(p => p.NgayNop)
        };
        return View(new PhieuDangKyViewModel
        {
            Phieu = filtered.ToList(), HoTen = hoTen, TenDot = tenDot, DonVi = donVi, TrangThai = trangThai,
            TuNgay = tuNgay, DenNgay = denNgay, SapXep = sapXep,
            DonViToChuc = all.Select(p => p.DonViToChuc).Distinct().Order().ToList()
        });
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        var phieu = kho.Tim(id);
        return phieu is null ? NotFound() : View(phieu);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult XuLy(int id, string trangThaiMoi, string? ghiChuXacNhan)
    {
        if (!TrangThaiPhieu.TatCa.Contains(trangThaiMoi)) return BadRequest("Trạng thái không hợp lệ!");
        var result = kho.ChuyenTrangThai(id, trangThaiMoi, ghiChuXacNhan);
        TempData[result.ThanhCong ? "ThongBao" : "Loi"] = result.ThongBao;
        return RedirectToAction(nameof(Details), new { id });
    }
}

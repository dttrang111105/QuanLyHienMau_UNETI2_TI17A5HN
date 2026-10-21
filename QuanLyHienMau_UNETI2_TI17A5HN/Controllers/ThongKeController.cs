using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QuanLyHienMau_UNETI2_TI17A5HN.Services;
using QuanLyHienMau_UNETI2_TI17A5HN.Infrastructure;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers;
[Authorize(Roles = "Admin,NhanVienYTe")]
public class ThongKeController(Module5Service service) : Controller
{
    public IActionResult Index() => RedirectToAction("Dashboard", "Admin");
    public IActionResult Dashboard() => RedirectToAction("Dashboard", "Admin");
    public IActionResult BaoCao(int? nam, int? maDonVi) => RedirectToAction("BaoCao", "Admin", new { nam, maDonVi });
    public async Task<IActionResult> Export(int? nam, int? maDonVi)
    {
        var year = nam ?? service.Now.Year;
        if (year < 2000 || year > 2100) return BadRequest("Năm không hợp lệ.");
        var stats = await service.ThongKe(year, maDonVi);
        var rows = new List<string[]> { new[] { "Nhóm thống kê", "Đối tượng", "Số lượng", "Tỷ lệ" } };
        foreach (var group in new[] { ("Đợt theo đơn vị", stats.DotTheoDonVi), ("Phiếu theo trạng thái", stats.PhieuTheoTrangThai), ("Thành công theo đơn vị", stats.ThanhCongTheoDonVi), ($"Phiếu theo tháng {year}", stats.PhieuTheoThang), ($"Lịch khám theo tháng {year}", stats.LichTheoThang) })
            rows.AddRange(group.Item2.Select(x => new[] { group.Item1, x.Nhom, x.SoLuong.ToString(), "" }));
        foreach (var d in stats.TheoDot)
        {
            rows.Add(new[] { "Phiếu theo đợt", d.TenDot, d.Tong.ToString(), "" });
            rows.Add(new[] { "Đã xác nhận hiện tại", d.TenDot, d.DaXacNhan.ToString(), "" });
            rows.Add(new[] { "Qua xác nhận", d.TenDot, d.QuaXacNhan.ToString(), d.TyLeXacNhan.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) + "%" });
            rows.Add(new[] { "Hiến máu thành công / đã khám", d.TenDot, $"{d.ThanhCong}/{d.DaKham}", d.TyLeThanhCong.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) + "%" });
        }
        rows.AddRange(stats.NhieuPhieuNhat.Select(x => new[] { "Đợt nhận nhiều phiếu nhất", x.TenDot, x.Tong.ToString(), "" }));
        return File(Module5Csv.Encode(rows), "text/csv; charset=utf-8", $"ThongKeHienMau_{year}.csv");
    }
}

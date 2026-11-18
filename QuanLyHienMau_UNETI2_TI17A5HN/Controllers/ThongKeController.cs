// Họ và tên: Trần Đỗ Quang Minh
// Mã sinh viên: DHTI17A5HN - Module 5
// Nội dung thực hiện: Module 5 - Controller Dashboard & Thống kê báo cáo

using System.Text;
using Microsoft.AspNetCore.Mvc;
using QuanLyHienMau_UNETI2_TI17A5HN.Filters;
using QuanLyHienMau_UNETI2_TI17A5HN.Repositories;
using QuanLyHienMau_UNETI2_TI17A5HN.Services;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers;

[KiemTraQuyen(ChiNhanVien = true)]
public class ThongKeController(ThongKeService service, DongHoHeThong clock) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Dashboard() => View(await service.DashboardAsync());

    [HttpGet]
    public async Task<IActionResult> Index() => View("Index", await service.TongHopAsync());

    [HttpGet]
    public async Task<IActionResult> XuatBaoCaoCsv()
    {
        var model = await service.TongHopAsync();
        var sb = new StringBuilder();
        sb.AppendLine("BÁO CÁO THỐNG KÊ HIẾN MÁU TÌNH NGUYỆN - HỆ THỐNG VITALIS UNETI");
        sb.AppendLine($"Thời điểm xuất báo cáo,{model.ThoiDiemThongKe:dd/MM/yyyy HH:mm}");
        sb.AppendLine($"Tổng phiếu đã khám sàng lọc,{model.TongDaKham}");
        sb.AppendLine($"Tổng hiến máu thành công,{model.TongThanhCong}");
        sb.AppendLine($"Tỷ lệ hiến máu thành công,{model.TyLeThanhCong:0.##}%");
        sb.AppendLine();

        sb.AppendLine("1. THỐNG KÊ THEO ĐỢT HIẾN MÁU");
        sb.AppendLine("Tên đợt hiến máu,Đơn vị tổ chức,Tổng phiếu,Đã xác nhận hiện tại,Đã qua xác nhận,Tỷ lệ xác nhận (%),Đã khám,Thành công,Tỷ lệ thành công (%),Chỉ tiêu");
        foreach (var dot in model.TheoDot)
        {
            sb.AppendLine($"\"{dot.TenDot.Replace("\"", "\"\"")}\",\"{dot.TenDonViToChuc.Replace("\"", "\"\"")}\",{dot.TongPhieu},{dot.DaXacNhan},{dot.DaQuaXacNhan},{dot.TyLeXacNhan:0.##},{dot.DaKham},{dot.ThanhCong},{dot.TyLeThanhCong:0.##},{dot.ChiTieu}");
        }
        sb.AppendLine();

        sb.AppendLine("2. THỐNG KÊ THEO TRẠNG THÁI PHIẾU");
        sb.AppendLine("Trạng thái phiếu,Số lượng");
        foreach (var st in model.TheoTrangThai)
        {
            sb.AppendLine($"\"{st.TrangThai}\",{st.SoLuong}");
        }
        sb.AppendLine();

        sb.AppendLine("3. THỐNG KÊ PHIẾU NỘP THEO THÁNG");
        sb.AppendLine("Tháng/Năm,Số phiếu");
        foreach (var m in model.PhieuTheoThang)
        {
            sb.AppendLine($"\"{m.Thang:00}/{m.Nam}\",{m.SoLuong}");
        }

        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        return File(bytes, "text/csv; charset=utf-8", $"BaoCaoThongKeHienMau_{clock.Now:yyyyMMdd_HHmm}.csv");
    }
}

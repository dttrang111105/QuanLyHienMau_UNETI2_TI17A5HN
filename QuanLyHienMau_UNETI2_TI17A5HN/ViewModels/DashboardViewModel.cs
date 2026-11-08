using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

public sealed class DashboardViewModel
{
    public int TongDonViToChuc { get; set; }
    public int TongDotHienMau { get; set; }
    public int SoDotDangMo { get; set; }
    public int TongNguoiHienMau { get; set; }
    public int TongPhieu { get; set; }
    public int SoPhieuChoXacNhan { get; set; }
    public int SoPhieuDaXacNhan { get; set; }
    public int SoLichSapToi { get; set; }
    public int SoPhieuThanhCong { get; set; }
    public DateTime ThoiDiemThongKe { get; set; }
    public List<LichKhamSangLoc> LichSapToi { get; set; } = [];
    public List<ThongKeDotRow> CanhBaoChiTieu { get; set; } = [];
}

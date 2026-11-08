using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

public sealed class ThongKeViewModel
{
    public List<ThongKeDonViRow> TheoDonVi { get; set; } = [];
    public List<ThongKeDotRow> TheoDot { get; set; } = [];
    public List<ThongKeTrangThaiRow> TheoTrangThai { get; set; } = [];
    public List<ThongKeThangRow> PhieuTheoThang { get; set; } = [];
    public List<ThongKeThangRow> LichTheoThang { get; set; } = [];
    public List<ThongKeDotRow> DotNhieuPhieuNhat { get; set; } = [];
    public int TongDaKham { get; set; }
    public int TongThanhCong { get; set; }
    public decimal TyLeThanhCong => TongDaKham == 0 ? 0 : Math.Round(100m * TongThanhCong / TongDaKham, 2);
    public DateTime ThoiDiemThongKe { get; set; }
}

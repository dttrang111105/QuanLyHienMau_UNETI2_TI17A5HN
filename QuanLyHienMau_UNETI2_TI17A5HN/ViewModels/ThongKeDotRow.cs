using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

public sealed class ThongKeDotRow
{
    public int MaDot { get; set; }
    public string TenDot { get; set; } = "";
    public string TenDonViToChuc { get; set; } = "";
    public int ChiTieu { get; set; }
    public int TongPhieu { get; set; }
    public int DaXacNhan { get; set; }
    public int DaQuaXacNhan { get; set; }
    public int DaKham { get; set; }
    public int ThanhCong { get; set; }
    public decimal TyLeXacNhan => TongPhieu == 0 ? 0 : Math.Round(100m * DaQuaXacNhan / TongPhieu, 2);
    public decimal TyLeThanhCong => DaKham == 0 ? 0 : Math.Round(100m * ThanhCong / DaKham, 2);
    public decimal TienDo => ChiTieu <= 0 ? 0 : Math.Round(100m * ThanhCong / ChiTieu, 2);
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

public sealed class ThongKeDonViRow
{
    public int MaDonViToChuc { get; set; }
    public string TenDonViToChuc { get; set; } = "";
    public int SoDot { get; set; }
    public int SoThanhCong { get; set; }
}

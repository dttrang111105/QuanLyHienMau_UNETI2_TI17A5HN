using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

public sealed class KetQuaListViewModel
{
    public List<KetQuaHienMau> Items { get; set; } = [];
    public string? Search { get; set; }
    public string? KetQua { get; set; }
    public int? MaDot { get; set; }
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public int SoThanhCong { get; set; }
    public int SoKhongDuDieuKien { get; set; }
    public int SoChoKetQua { get; set; }
    public bool ChiCuaToi { get; set; }
    public List<SelectListItem> DotOptions { get; set; } = [];
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

public sealed class ThongKeThangRow
{
    public int Nam { get; set; }
    public int Thang { get; set; }
    public int SoLuong { get; set; }
}

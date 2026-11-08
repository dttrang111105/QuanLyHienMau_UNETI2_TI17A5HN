using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

public sealed class KetQuaFormViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn phiếu đã khám sàng lọc.")]
    [Display(Name = "Phiếu đăng ký")]
    public int MaPhieu { get; set; }
    [Range(typeof(decimal), "0", "100", ErrorMessage = "Điểm đánh giá phải từ 0 đến 100.")]
    [Display(Name = "Điểm đánh giá")]
    public decimal? DiemDanhGia { get; set; }
    [StringLength(1000, ErrorMessage = "Nhận xét không quá 1000 ký tự.")]
    [Display(Name = "Nhận xét")]
    public string? NhanXet { get; set; }
    [Required(ErrorMessage = "Vui lòng chọn kết quả cuối cùng.")]
    [RegularExpression("^(Hiến máu thành công|Không đủ điều kiện hiến)$", ErrorMessage = "Kết quả không hợp lệ.")]
    [Display(Name = "Kết quả hiến máu")]
    public string KetQua { get; set; } = "";
    [ValidateNever]
    public List<SelectListItem> PhieuOptions { get; set; } = [];
    [ValidateNever]
    public PhieuDangKyHienMau? Phieu { get; set; }
}

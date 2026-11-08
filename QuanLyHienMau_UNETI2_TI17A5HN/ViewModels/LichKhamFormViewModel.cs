using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

public sealed class LichKhamFormViewModel : IValidatableObject
{
    public int? MaLichKhamSangLoc { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn phiếu đã xác nhận.")]
    [Display(Name = "Phiếu đăng ký")]
    public int MaPhieu { get; set; }
    [Required(ErrorMessage = "Vui lòng nhập thời gian bắt đầu.")]
    [Display(Name = "Thời gian bắt đầu")]
    public DateTime? ThoiGianBatDau { get; set; }
    [Required(ErrorMessage = "Vui lòng nhập thời gian kết thúc.")]
    [Display(Name = "Thời gian kết thúc")]
    public DateTime? ThoiGianKetThuc { get; set; }
    [Required(ErrorMessage = "Vui lòng chọn hình thức tiếp nhận.")]
    [RegularExpression("^(Trực tiếp|Trực tuyến|Trực tiếp tại điểm hiến|Xe hiến máu lưu động|Điểm cố định quận/huyện)$", ErrorMessage = "Hình thức tiếp nhận không hợp lệ.")]
    [Display(Name = "Hình thức tiếp nhận")]
    public string HinhThucTiepNhan { get; set; } = "Trực tiếp";
    [Required(ErrorMessage = "Vui lòng nhập địa điểm hoặc liên kết.")]
    [StringLength(255, ErrorMessage = "Địa điểm hoặc liên kết không quá 255 ký tự.")]
    [Display(Name = "Địa điểm hoặc liên kết")]
    public string DiaDiemHoacLienKet { get; set; } = "";
    [Required(ErrorMessage = "Vui lòng nhập bác sĩ sàng lọc.")]
    [StringLength(100, ErrorMessage = "Tên bác sĩ không quá 100 ký tự.")]
    [Display(Name = "Bác sĩ sàng lọc")]
    public string BacSiSangLoc { get; set; } = "";
    [StringLength(500, ErrorMessage = "Ghi chú không quá 500 ký tự.")]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }
    [ValidateNever]
    public List<SelectListItem> PhieuOptions { get; set; } = [];
    [ValidateNever]
    public List<string> BacSiOptions { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ThoiGianBatDau.HasValue && ThoiGianKetThuc.HasValue && ThoiGianKetThuc <= ThoiGianBatDau)
            yield return new ValidationResult("Thời gian kết thúc phải sau thời gian bắt đầu.", [nameof(ThoiGianKetThuc)]);
        if (HinhThucTiepNhan == "Trực tuyến" &&
            (!Uri.TryCreate(DiaDiemHoacLienKet, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps))
            yield return new ValidationResult("Liên kết trực tuyến phải là địa chỉ HTTPS hợp lệ.", [nameof(DiaDiemHoacLienKet)]);
    }
}

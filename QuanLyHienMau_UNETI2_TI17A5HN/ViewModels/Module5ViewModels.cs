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
    [RegularExpression("^(Trực tiếp|Trực tuyến)$", ErrorMessage = "Hình thức tiếp nhận không hợp lệ.")]
    [Display(Name = "Hình thức tiếp nhận")]
    public string HinhThucTiepNhan { get; set; } = "Trực tiếp";
    [Required(ErrorMessage = "Vui lòng nhập địa điểm hoặc liên kết.")]
    [StringLength(500, ErrorMessage = "Địa điểm hoặc liên kết không quá 500 ký tự.")]
    [Display(Name = "Địa điểm hoặc liên kết")]
    public string DiaDiemHoacLienKet { get; set; } = "";
    [Required(ErrorMessage = "Vui lòng nhập bác sĩ sàng lọc.")]
    [StringLength(150, ErrorMessage = "Tên bác sĩ không quá 150 ký tự.")]
    [Display(Name = "Bác sĩ sàng lọc")]
    public string BacSiSangLoc { get; set; } = "";
    [StringLength(2000, ErrorMessage = "Ghi chú không quá 2000 ký tự.")]
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

public sealed class KetQuaFormViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn phiếu đã khám sàng lọc.")]
    [Display(Name = "Phiếu đăng ký")]
    public int MaPhieu { get; set; }
    [Range(typeof(decimal), "0", "100", ErrorMessage = "Điểm đánh giá phải từ 0 đến 100.")]
    [Display(Name = "Điểm đánh giá")]
    public decimal? DiemDanhGia { get; set; }
    [StringLength(2000, ErrorMessage = "Nhận xét không quá 2000 ký tự.")]
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

public sealed class LichKhamListViewModel
{
    public List<LichKhamSangLoc> Items { get; set; } = [];
    public string? Search { get; set; }
    public string? TrangThai { get; set; }
    public int? MaDot { get; set; }
    public DateTime? Ngay { get; set; }
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public int SoLichHomNay { get; set; }
    public int SoLichSapToi { get; set; }
    public int SoLichHoanThanh { get; set; }
    public bool ChiCuaToi { get; set; }
    public List<SelectListItem> DotOptions { get; set; } = [];
}

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

public sealed class ThongKeDonViRow
{
    public int MaDonViToChuc { get; set; }
    public string TenDonViToChuc { get; set; } = "";
    public int SoDot { get; set; }
    public int SoThanhCong { get; set; }
}

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

public sealed class ThongKeTrangThaiRow
{
    public string TrangThai { get; set; } = "";
    public int SoLuong { get; set; }
}

public sealed class ThongKeThangRow
{
    public int Nam { get; set; }
    public int Thang { get; set; }
    public int SoLuong { get; set; }
}

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

// Họ và tên: ĐIỀN HỌ TÊN CỦA BẠN
// Mã sinh viên: ĐIỀN MSSV CỦA BẠN
// Nội dung: Module 5 và phần nền dữ liệu cần thiết để chạy thử.
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;
public class LoginVm
{
    [Required(ErrorMessage = "Nhập tên đăng nhập.")]
    public string TenDangNhap { get; set; } = "";

    [Required(ErrorMessage = "Nhập mật khẩu.")]
    public string MatKhau { get; set; } = "";
    public string? ReturnUrl { get; set; }
}

public class LichFormVm
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Chọn phiếu đã xác nhận.")]
    public int MaPhieu { get; set; }

    [Required(ErrorMessage = "Nhập thời gian bắt đầu.")]
    public DateTime? ThoiGianBatDau { get; set; }

    [Required(ErrorMessage = "Nhập thời gian kết thúc.")]
    public DateTime? ThoiGianKetThuc { get; set; }

    [Required, MaxLength(80)]
    public string HinhThucTiepNhan { get; set; } = "Trực tiếp";

    [Required(ErrorMessage = "Nhập địa điểm hoặc liên kết."), MaxLength(250)]
    public string DiaDiemHoacLienKet { get; set; } = "";

    [Required(ErrorMessage = "Nhập bác sĩ sàng lọc."), MaxLength(120)]
    public string BacSiSangLoc { get; set; } = "";

    [MaxLength(1000)]
    public string? GhiChu { get; set; }
    public List<SelectListItem> Phieus { get; set; } = [];
}

public class KetQuaFormVm
{
    public int MaPhieu { get; set; }
    public string ThongTinPhieu { get; set; } = "";

    [Range(0, 100, ErrorMessage = "Điểm phải từ 0 đến 100.")]
    public int? DiemDanhGia { get; set; }

    [Required(ErrorMessage = "Nhập nhận xét."), MaxLength(2000)]
    public string NhanXet { get; set; } = "";

    [Required(ErrorMessage = "Chọn kết quả.")]
    public string KetQua { get; set; } = "";
}

public record ThongKeDong(string Nhom, int SoLuong);
public record TheoDotDong(int MaDot, string TenDot, int Tong, int DaXacNhan, int QuaXacNhan, int DaKham, int ThanhCong, int ChiTieu)
{
    public double TyLeXacNhan => Tong == 0 ? 0 : 100d * QuaXacNhan / Tong;
    public double TyLeThanhCong => DaKham == 0 ? 0 : 100d * ThanhCong / DaKham;
}

public class DashboardVm
{
    public int DonVi { get; set; }
    public int Dot { get; set; }
    public int DotMo { get; set; }
    public int Nguoi { get; set; }
    public int Phieu { get; set; }
    public int ChoXacNhan { get; set; }
    public int DaXacNhan { get; set; }
    public int LichSapToi { get; set; }
    public int ThanhCong { get; set; }
    public List<LichKhamSangLoc> SapToi { get; set; } = [];
    public List<TheoDotDong> TienDo { get; set; } = [];
}

public class ThongKeVm
{
    public int Nam { get; set; }
    public List<ThongKeDong> DotTheoDonVi { get; set; } = [];
    public List<ThongKeDong> PhieuTheoTrangThai { get; set; } = [];
    public List<ThongKeDong> ThanhCongTheoDonVi { get; set; } = [];
    public List<ThongKeDong> PhieuTheoThang { get; set; } = [];
    public List<ThongKeDong> LichTheoThang { get; set; } = [];
    public List<TheoDotDong> TheoDot { get; set; } = [];
    public List<TheoDotDong> NhieuPhieuNhat { get; set; } = [];
}

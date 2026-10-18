using System.ComponentModel.DataAnnotations;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Models;
public class TaiKhoan
{
    [Key]
    public int MaTaiKhoan { get; set; }

    [Required, MaxLength(60)]
    public string TenDangNhap { get; set; } = "";

    [Required]
    public string MatKhau { get; set; } = "";

    [Required, MaxLength(120)]
    public string HoTen { get; set; } = "";

    [EmailAddress, MaxLength(160)]
    public string Email { get; set; } = "";

    [Required, MaxLength(30)]
    public string VaiTro { get; set; } = "NguoiHienMau";
    public bool TrangThai { get; set; } = true;
}

public class DonViToChuc
{
    [Key]
    public int MaDonViToChuc { get; set; }

    [Required, MaxLength(160)]
    public string TenDonViToChuc { get; set; } = "";
    public string MoTa { get; set; } = "";

    [EmailAddress]
    public string EmailLienHe { get; set; } = "";
    public bool TrangThai { get; set; } = true;
}

public class DotHienMau
{
    [Key]
    public int MaDot { get; set; }

    [Required, MaxLength(160)]
    public string TenDot { get; set; } = "";
    public int MaDonViToChuc { get; set; }
    public DonViToChuc DonViToChuc { get; set; } = null !;

    [Range(1, int.MaxValue)]
    public int ChiTieuDonViMau { get; set; } = 20;
    public string NhomMauUuTien { get; set; } = "O";
    public double CanNangToiThieu { get; set; } = 45;
    public DateTime NgayBatDauDangKy { get; set; }
    public DateTime HanDangKy { get; set; }
    public string MoTaDot { get; set; } = "";
    public string DieuKienNguoiHien { get; set; } = "";
    public string TrangThai { get; set; } = "Đang mở đăng ký";
}

public class NguoiHienMau
{
    [Key]
    public int MaNguoiHien { get; set; }
    public int MaTaiKhoan { get; set; }
    public TaiKhoan TaiKhoan { get; set; } = null !;
    public string HoTen { get; set; } = "";
    public DateTime NgaySinh { get; set; }
    public string GioiTinh { get; set; } = "";
    public string SoDienThoai { get; set; } = "";
    public string Email { get; set; } = "";
    public string DiaChi { get; set; } = "";
    public string NhomMau { get; set; } = "O";
    public string NgheNghiep { get; set; } = "Sinh viên";
    public double CanNang { get; set; } = 55;
    public string GhiChuSucKhoe { get; set; } = "";
    public bool TrangThai { get; set; } = true;
}

public class PhieuDangKyHienMau
{
    [Key]
    public int MaPhieu { get; set; }
    public int MaNguoiHien { get; set; }
    public NguoiHienMau NguoiHien { get; set; } = null !;
    public int MaDot { get; set; }
    public DotHienMau Dot { get; set; } = null !;
    public DateTime NgayNop { get; set; }
    public string LyDoHienMau { get; set; } = "";
    public string GhiChu { get; set; } = "";
    public string TrangThai { get; set; } = TrangThaiPhieu.ChoXacNhan;
    public DateTime? NgayXuLy { get; set; }
    public string GhiChuXacNhan { get; set; } = "";
    public List<LichKhamSangLoc> LichKhams { get; set; } = [];
    public KetQuaHienMau? KetQuaHienMau { get; set; }
}

public static class TrangThaiPhieu
{
    public const string ChoXacNhan = "Chờ xác nhận", DaXacNhan = "Đã xác nhận", TuChoi = "Từ chối", ChoKham = "Chờ khám sàng lọc", DaKham = "Đã khám sàng lọc", ThanhCong = "Hiến máu thành công", KhongDat = "Không đủ điều kiện hiến", DaHuy = "Đã hủy";
    public static readonly string[] DaQuaXacNhan = [DaXacNhan, ChoKham, DaKham, ThanhCong, KhongDat];
}

public static class TrangThaiLich
{
    public const string DaLenLich = "Đã lên lịch", HoanThanh = "Đã hoàn thành", DaHuy = "Đã hủy";
}

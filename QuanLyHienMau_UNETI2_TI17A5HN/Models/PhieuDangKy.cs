namespace QuanLyHienMau_UNETI2_TI17A5HN.Models;

public static class TrangThaiPhieu
{
    public const string ChoXacNhan = "Chờ xác nhận";
    public const string DaXacNhan = "Đã xác nhận";
    public const string TuChoi = "Từ chối";
    public const string ChoKham = "Chờ khám sàng lọc";
    public const string DaKham = "Đã khám sàng lọc";
    public const string HienMauThanhCong = "Hiến máu thành công";
    public const string KhongDuDieuKien = "Không đủ điều kiện hiến";
    public const string DaHuy = "Đã hủy";

    public static readonly string[] TatCa = [ChoXacNhan, DaXacNhan, TuChoi, ChoKham, DaKham, HienMauThanhCong, KhongDuDieuKien, DaHuy];
}

public sealed class PhieuDangKy
{
    public int Id { get; set; }
    public string MaPhieu { get; set; } = "";
    public string HoTenNguoiHien { get; set; } = "";
    public string NhomMau { get; set; } = "";
    public bool NguoiHienConHoatDong { get; set; } = true;
    public string TenDot { get; set; } = "";
    public string DonViToChuc { get; set; } = "";
    public string YeuCauDot { get; set; } = "";
    public string TrangThai { get; set; } = TrangThaiPhieu.ChoXacNhan;
    public DateTime NgayNop { get; set; }
    public DateTime? NgayXuLy { get; set; }
    public string? GhiChuXacNhan { get; set; }
}

public sealed class PhieuDangKyViewModel
{
    public IReadOnlyList<PhieuDangKy> Phieu { get; init; } = [];
    public string? HoTen { get; init; }
    public string? TenDot { get; init; }
    public string? DonVi { get; init; }
    public string? TrangThai { get; init; }
    public DateTime? TuNgay { get; init; }
    public DateTime? DenNgay { get; init; }
    public string SapXep { get; init; } = "ngaymoi";
    public IReadOnlyList<string> DonViToChuc { get; init; } = [];
    public int ChiTieuDot { get; init; }
    public int DaHienThanhCong { get; init; }
    public bool CanhBaoChiTieu => ChiTieuDot > 0 && DaHienThanhCong >= ChiTieuDot;
}

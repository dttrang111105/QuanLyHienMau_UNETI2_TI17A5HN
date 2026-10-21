namespace QuanLyHienMau_UNETI2_TI17A5HN.Models;

public static class Module5TrangThai
{
    public static class Phieu
    {
        public const string ChoXacNhan = "Chờ xác nhận";
        public const string DaXacNhan = "Đã xác nhận";
        public const string TuChoi = "Từ chối";
        public const string ChoKhamSangLoc = "Chờ khám sàng lọc";
        public const string DaKhamSangLoc = "Đã khám sàng lọc";
        public const string HienMauThanhCong = "Hiến máu thành công";
        public const string KhongDuDieuKienHien = "Không đủ điều kiện hiến";
        public const string DaHuy = "Đã hủy";

        public static readonly string[] TatCa =
        [ChoXacNhan, DaXacNhan, TuChoi, ChoKhamSangLoc, DaKhamSangLoc,
            HienMauThanhCong, KhongDuDieuKienHien, DaHuy];

        public static readonly string[] DaQuaXacNhan =
        [DaXacNhan, ChoKhamSangLoc, DaKhamSangLoc, HienMauThanhCong, KhongDuDieuKienHien];
    }

    public static class LichKham
    {
        public const string DaLenLich = "Đã lên lịch";
        public const string DaHoanThanh = "Đã hoàn thành";
        public const string DaHuy = "Đã hủy";
        public static readonly string[] TatCa = [DaLenLich, DaHoanThanh, DaHuy];
    }

    public static class DotHienMau
    {
        public const string DangMoDangKy = "Đang mở đăng ký";
    }

    public static class VaiTro
    {
        public const string Admin = "Admin";
        public const string NhanVienYTe = "Nhân viên y tế";
        public const string NguoiHienMau = "Người hiến máu";

        public static string ChuanHoa(string? value) => value?.Trim() switch
        {
            "Admin" => Admin,
            "NhanVien" or "NhanVienYTe" or NhanVienYTe => NhanVienYTe,
            "NguoiHienMau" or NguoiHienMau => NguoiHienMau,
            _ => ""
        };

        public static bool LaNhanVien(string? value) =>
            ChuanHoa(value) is Admin or NhanVienYTe;
    }
}

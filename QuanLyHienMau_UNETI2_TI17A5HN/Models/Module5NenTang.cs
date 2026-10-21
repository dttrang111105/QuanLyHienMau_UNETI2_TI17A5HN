namespace QuanLyHienMau_UNETI2_TI17A5HN.Models;

public static class TrangThaiPhieu
{
    public const string ChoXacNhan = "Chờ xác nhận", DaXacNhan = "Đã xác nhận", TuChoi = "Từ chối", ChoKham = "Chờ khám sàng lọc", DaKham = "Đã khám sàng lọc", ThanhCong = "Hiến máu thành công", KhongDat = "Không đủ điều kiện hiến", DaHuy = "Đã hủy";
    public static readonly string[] DaQuaXacNhan = [DaXacNhan, ChoKham, DaKham, ThanhCong, KhongDat];
}

public static class TrangThaiLich
{
    public const string DaLenLich = "Đã lên lịch", HoanThanh = "Đã hoàn thành", DaHuy = "Đã hủy";
}

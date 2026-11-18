// Họ và tên: Trần Đỗ Quang Minh
// Mã sinh viên: DHTI17A5HN - Module 5
// Nội dung thực hiện: Module 5 - Dịch vụ cung cấp thời gian hệ thống

namespace QuanLyHienMau_UNETI2_TI17A5HN.Services;

public sealed class DongHoHeThong(TimeProvider timeProvider)
{
    private static readonly TimeZoneInfo VietNam = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
    public DateTime Now => DateTime.SpecifyKind(
        TimeZoneInfo.ConvertTime(timeProvider.GetUtcNow(), VietNam).DateTime,
        DateTimeKind.Unspecified);
}

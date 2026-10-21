namespace QuanLyHienMau_UNETI2_TI17A5HN.Services;

public sealed class Module5Clock(TimeProvider timeProvider)
{
    private static readonly TimeZoneInfo VietNam = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
    public DateTime Now => DateTime.SpecifyKind(
        TimeZoneInfo.ConvertTime(timeProvider.GetUtcNow(), VietNam).DateTime,
        DateTimeKind.Unspecified);
}

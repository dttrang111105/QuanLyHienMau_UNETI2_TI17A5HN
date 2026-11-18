// Họ và tên: Trần Đỗ Quang Minh
// Mã sinh viên: DHTI17A5HN - Module 5
// Nội dung thực hiện: Module 5 - Service tổng hợp số liệu thống kê hiến máu

using QuanLyHienMau_UNETI2_TI17A5HN.Repositories;
using QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Services;

public sealed class ThongKeService(ThongKeRepository repository, DongHoHeThong clock)
{
    public Task<DashboardViewModel> DashboardAsync() =>
        repository.DashboardAsync(clock.Now);

    public Task<ThongKeViewModel> TongHopAsync() =>
        repository.TongHopAsync(clock.Now);
}

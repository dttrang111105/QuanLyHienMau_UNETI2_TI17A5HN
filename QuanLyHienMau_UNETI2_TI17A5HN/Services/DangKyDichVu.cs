using Microsoft.Extensions.DependencyInjection.Extensions;
using QuanLyHienMau_UNETI2_TI17A5HN.Repositories;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Services;

public static class DangKyDichVu
{
    public static IServiceCollection AddDichVuHienMau(this IServiceCollection services)
    {
        services.TryAddSingleton<TimeProvider>(TimeProvider.System);
        services.TryAddSingleton<DongHoHeThong>();
        services.AddScoped<KhamSangLocService>();
        services.AddScoped<LichKhamSangLocRepository>();
        services.AddScoped<KetQuaHienMauRepository>();
        services.AddScoped<ThongKeRepository>();
        services.AddScoped<TaiKhoanRepository>();
        return services;
    }
}

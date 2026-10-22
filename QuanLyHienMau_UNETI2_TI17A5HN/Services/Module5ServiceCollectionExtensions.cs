using Microsoft.Extensions.DependencyInjection.Extensions;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Services;

public static class Module5ServiceCollectionExtensions
{
    public static IServiceCollection AddModule5(this IServiceCollection services)
    {
        services.TryAddSingleton<TimeProvider>(TimeProvider.System);
        services.TryAddSingleton<Module5Clock>();
        services.AddScoped<Module5Service>();
        services.AddScoped<ThongKeService>();
        return services;
    }
}

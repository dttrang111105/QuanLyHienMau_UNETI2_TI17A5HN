// Họ và tên: ĐIỀN HỌ TÊN CỦA BẠN
// Mã sinh viên: ĐIỀN MSSV CỦA BẠN
// Nội dung: hoàn thiện Module 5 trên project gốc.
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Services;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Infrastructure;
public static class Module5Setup
{
    public const string Scheme = "Module5Cookie";
    public static IServiceCollection AddModule5(this IServiceCollection services, IConfiguration config)
    {
        services.AddDistributedMemoryCache();
        services.AddSession(o =>
        {
            o.Cookie.Name = ".Vitalis.Module5.Session";
            o.IdleTimeout = TimeSpan.FromHours(2);
            o.Cookie.HttpOnly = true;
            o.Cookie.IsEssential = true;
        });
        services.AddAuthentication(Scheme).AddCookie(Scheme, o =>
        {
            o.Cookie.Name = ".Vitalis.Module5.Auth";
            o.LoginPath = "/Module5Access/Login";
            o.AccessDeniedPath = "/Module5Access/AccessDenied";
            o.ExpireTimeSpan = TimeSpan.FromHours(2);
        });
        services.AddDbContext<ApplicationDbContext>(o =>
        {
            if (config["Module5:DatabaseProvider"] == "Sqlite")
                o.UseSqlite(config.GetConnectionString("Module5Sqlite"));
            else
                o.UseSqlServer(config.GetConnectionString("Module5SqlServer"));
        });
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<Module5Service>();
        services.AddScoped<Module5Pages>();
        services.AddSingleton<Module5Database>();
        services.AddScoped<Module5DatabaseExceptionFilter>();
        return services;
    }

    public static bool IsModule5(PathString path)
    {
        var s = (path.Value ?? "").TrimEnd('/');
        return s.StartsWith("/LichKhamSangLoc", StringComparison.OrdinalIgnoreCase) || s.StartsWith("/KetQuaHienMau", StringComparison.OrdinalIgnoreCase) || s.StartsWith("/ThongKe", StringComparison.OrdinalIgnoreCase) || s.StartsWith("/TheoDoiCuaToi", StringComparison.OrdinalIgnoreCase) || new[]
        {
            "/Admin/Dashboard",
            "/Admin/LichKhamSangLoc",
            "/Admin/KetQuaXetNghiem",
            "/Admin/BaoCao"
        }.Contains(s, StringComparer.OrdinalIgnoreCase);
    }
}

public class Module5Database
{
    readonly SemaphoreSlim gate = new(1, 1);
    bool initialized;
    public async Task Initialize(HttpContext context)
    {
        if (initialized)
            return;
        await gate.WaitAsync(context.RequestAborted);
        try
        {
            if (initialized)
                return;
            var db = context.RequestServices.GetRequiredService<ApplicationDbContext>();
            if (db.Database.IsSqlite())
            {
                var env = context.RequestServices.GetRequiredService<IWebHostEnvironment>();
                Directory.CreateDirectory(Path.Combine(env.ContentRootPath, "App_Data"));
                await db.Database.EnsureCreatedAsync();
            }
            else
                await db.Database.MigrateAsync();
            if (context.RequestServices.GetRequiredService<IConfiguration>().GetValue("Module5:SeedDemoData", true))
                await SeedData.Initialize(db, context.RequestServices.GetRequiredService<Module5Service>().Now);
            initialized = true;
        }
        finally
        {
            gate.Release();
        }
    }
}

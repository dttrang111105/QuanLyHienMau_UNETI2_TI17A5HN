// Họ và tên: Nguyễn Tiến Nam
// Mã sinh viên: 23103100275
// Nội dung thực hiện: Cấu hình DI Repository Pattern, Authentication và Session

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Repositories;
using QuanLyHienMau_UNETI2_TI17A5HN.Repositories.Interfaces;
using QuanLyHienMau_UNETI2_TI17A5HN.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ========================================================
// ĐĂNG KÝ DEPENDENCY INJECTION CHO REPOSITORY PATTERN
// ========================================================
builder.Services.AddScoped(typeof(QuanLyHienMau_UNETI2_TI17A5HN.Repositories.Interfaces.IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<INguoiHienMauRepository, NguoiHienMauRepository>();
builder.Services.AddScoped<QuanLyHienMau_UNETI2_TI17A5HN.Repositories.Interfaces.IPhieuDangKyRepository, PhieuDangKyRepository>();

builder.Services.AddScoped<IDotHienMauRepository, DotHienMauRepository>();

builder.Services.AddScoped<QuanLyHienMau_UNETI2_TI17A5HN.Repositories.IPhieuDangKyRepository, EfPhieuDangKyRepository>();

builder.Services.AddScoped(typeof(QuanLyHienMau_UNETI2_TI17A5HN.Repositories.IRepository<>), typeof(EfGenericRepository<>));
builder.Services.AddScoped<ITaiKhoanRepository, TaiKhoanRepository>();
builder.Services.AddScoped<IDonViToChucRepository, DonViToChucRepository>();
builder.Services.AddDichVuHienMau();

// ========================================================
// CẤU HÌNH SESSION CHO ĐĂNG NHẬP & PHÂN QUYỀN (Mục 5.2 & 5.3)
// ========================================================
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "QuanLyHienMau.Session";
});

// Configure Cookie Authentication & Real-time Account Status Validation
builder.Services.AddAuthentication("QuanLyHienMau.Auth")
    .AddCookie("QuanLyHienMau.Auth", options =>
    {
        options.Cookie.Name = "QuanLyHienMau.Auth";
        options.Cookie.HttpOnly = true;
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = async context =>
            {
                var userIdClaim = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdClaim, out int maTaiKhoan))
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync("QuanLyHienMau.Auth");
                    return;
                }

                var taiKhoanRepo = context.HttpContext.RequestServices.GetRequiredService<ITaiKhoanRepository>();
                var taiKhoan = await taiKhoanRepo.GetByIdAsync(maTaiKhoan);

                // Nếu tài khoản không còn tồn tại hoặc vừa bị khóa bởi Admin -> Đăng xuất ngay lập tức
                if (taiKhoan == null || taiKhoan.TrangThai != "Hoạt động")
                {
                    context.RejectPrincipal();
                    context.HttpContext.Session.Clear();
                    await context.HttpContext.SignOutAsync("QuanLyHienMau.Auth");
                    return;
                }

                // Tự động đồng bộ lại Session nếu Session hết hạn trước Cookie (RememberMe)
                if (context.HttpContext.Session.GetInt32("MaTaiKhoan") == null)
                {
                    context.HttpContext.Session.SetInt32("MaTaiKhoan", taiKhoan.MaTaiKhoan);
                    context.HttpContext.Session.SetString("HoTen", taiKhoan.HoTen);
                    context.HttpContext.Session.SetString("VaiTro", taiKhoan.VaiTro);
                    context.HttpContext.Session.SetString("TenDangNhap", taiKhoan.TenDangNhap);
                }
            }
        };
    });

var app = builder.Build();

// Seed initial test data for Accounts, DonViToChuc, and DotHienMau if empty
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        DbInitializer.Initialize(context);
        if (app.Environment.IsDevelopment())
        {
            DbInitializer.SeedTestData(context);
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Đã xảy ra lỗi trong quá trình khởi tạo dữ liệu mẫu.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Kích hoạt Session trước Authentication/Authorization
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

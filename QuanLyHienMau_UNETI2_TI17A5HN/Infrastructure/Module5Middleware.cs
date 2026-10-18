// Họ và tên: ĐIỀN HỌ TÊN CỦA BẠN
// Mã sinh viên: ĐIỀN MSSV CỦA BẠN
// Nội dung: hoàn thiện Module 5 trên project gốc.
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Infrastructure;
public class Module5Middleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, Module5Database database)
    {
        if (context.Request.Path.Equals("/Account/Login", StringComparison.OrdinalIgnoreCase) && HttpMethods.IsGet(context.Request.Method) && context.User.Identity?.IsAuthenticated == true)
        {
            await context.SignOutAsync(Module5Setup.Scheme);
            context.Session.Remove("Module5.MaTaiKhoan");
            context.Session.Remove("Module5.HoTen");
            context.Session.Remove("Module5.VaiTro");
            context.User = new ClaimsPrincipal(new ClaimsIdentity());
        }

        if (Module5Setup.IsModule5(context.Request.Path) || (context.Request.Path.Equals("/Module5Access/Login", StringComparison.OrdinalIgnoreCase) && HttpMethods.IsPost(context.Request.Method)))
        {
            try
            {
                await database.Initialize(context);
            }
            catch (Exception ex)when (ex is not OperationCanceledException)
            {
                context.RequestServices.GetRequiredService<ILogger<Module5Middleware>>().LogError(ex, "Cannot initialize Module 5 database");
                context.Response.StatusCode = 503;
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync("<!doctype html><html lang='vi'><meta charset='utf-8'><title>Không thể tải dữ liệu</title><body><h1>Không thể tải dữ liệu</h1><p>Vui lòng thử lại sau.</p><a href='/'>Về trang chủ</a></body></html>");
                return;
            }
        }

        await next(context);
    }
}

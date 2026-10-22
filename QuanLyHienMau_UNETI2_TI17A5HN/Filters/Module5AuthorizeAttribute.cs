using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class Module5AuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    public bool ChiNhanVien { get; set; }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var http = context.HttpContext;
        var maTaiKhoan = http.Session.GetInt32("MaTaiKhoan");
        if (!maTaiKhoan.HasValue &&
            int.TryParse(http.User.FindFirstValue(ClaimTypes.NameIdentifier), out var claimId))
        {
            maTaiKhoan = claimId;
        }

        if (!maTaiKhoan.HasValue)
        {
            context.Result = new RedirectToActionResult("Login", "Account",
                new { returnUrl = http.Request.PathBase + http.Request.Path + http.Request.QueryString });
            return;
        }

        var taiKhoan = http.Items[typeof(TaiKhoan)] as TaiKhoan;
        if (taiKhoan is null)
        {
            var db = http.RequestServices.GetRequiredService<ApplicationDbContext>();
            taiKhoan = await db.Set<TaiKhoan>().AsNoTracking()
                .SingleOrDefaultAsync(x => x.MaTaiKhoan == maTaiKhoan.Value);
        }

        var vaiTro = Module5TrangThai.VaiTro.ChuanHoa(taiKhoan?.VaiTro);
        var sessionOrClaimRole = http.Session.GetString("VaiTro") ?? http.User.FindFirstValue(ClaimTypes.Role) ?? taiKhoan?.VaiTro;
        if (taiKhoan is null || taiKhoan.TrangThai != "Hoạt động" || vaiTro == "" ||
            vaiTro != Module5TrangThai.VaiTro.ChuanHoa(sessionOrClaimRole))
        {
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            return;
        }

        http.Items[typeof(TaiKhoan)] = taiKhoan;
        if (ChiNhanVien && !Module5TrangThai.VaiTro.LaNhanVien(vaiTro))
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
    }
}

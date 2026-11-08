using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuanLyHienMau_UNETI2_TI17A5HN.Repositories;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class KiemTraQuyenAttribute : Attribute, IAsyncAuthorizationFilter
{
    public bool ChiNhanVien { get; set; }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var http = context.HttpContext;
        var maTaiKhoan = http.Session.GetInt32("MaTaiKhoan");
        if (!maTaiKhoan.HasValue && http.User.Identity?.IsAuthenticated == true &&
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
            var repository = http.RequestServices.GetRequiredService<TaiKhoanRepository>();
            taiKhoan = await repository.TimTheoMaAsync(maTaiKhoan.Value);
        }

        var vaiTro = TrangThaiHienMau.VaiTro.ChuanHoa(taiKhoan?.VaiTro);
        var sessionOrClaimRole = http.Session.GetString("VaiTro") ?? http.User.FindFirstValue(ClaimTypes.Role);
        if (taiKhoan is null || !TrangThaiHienMau.DangHoatDong(taiKhoan.TrangThai) || vaiTro == "" ||
            vaiTro != TrangThaiHienMau.VaiTro.ChuanHoa(sessionOrClaimRole))
        {
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            return;
        }

        http.Items[typeof(TaiKhoan)] = taiKhoan;
        if (ChiNhanVien && !TrangThaiHienMau.VaiTro.LaNhanVien(vaiTro))
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using QuanLyHienMau_UNETI2_TI17A5HN.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers;
public class Module5AccessController(ApplicationDbContext db) : Controller
{
    public IActionResult Index() => RedirectToAction(nameof(Login));
    [HttpGet("/Account/Login"), HttpGet("/Module5Access/Login")]
    public IActionResult Login(string? returnUrl = null) => View("~/Views/Account/Login.cshtml", new LoginVm { ReturnUrl = returnUrl });
    [HttpPost("/Account/Login"), HttpPost("/Module5Access/Login"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVm vm)
    {
        if (!ModelState.IsValid)
            return View("~/Views/Account/Login.cshtml", vm);
        var identifier = vm.TenDangNhap.Trim();
        var a = await db.TaiKhoans.SingleOrDefaultAsync(x => x.TenDangNhap == identifier)
            ?? await db.TaiKhoans.FirstOrDefaultAsync(x => x.Email == identifier);
        if (a == null || !a.TrangThai || new PasswordHasher<TaiKhoan>().VerifyHashedPassword(a, a.MatKhau, vm.MatKhau) == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Tên đăng nhập, mật khẩu không đúng hoặc tài khoản bị khóa.");
            return View("~/Views/Account/Login.cshtml", vm);
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, a.MaTaiKhoan.ToString()),
            new Claim(ClaimTypes.Name, a.HoTen),
            new Claim(ClaimTypes.Role, a.VaiTro)
        };
        await HttpContext.SignInAsync(Module5Setup.Scheme, new ClaimsPrincipal(new ClaimsIdentity(claims, Module5Setup.Scheme)), new AuthenticationProperties { IsPersistent = vm.RememberMe });
        HttpContext.Session.SetInt32("Module5.MaTaiKhoan", a.MaTaiKhoan);
        HttpContext.Session.SetString("Module5.HoTen", a.HoTen);
        HttpContext.Session.SetString("Module5.VaiTro", a.VaiTro);
        if (Url.IsLocalUrl(vm.ReturnUrl))
            return LocalRedirect(vm.ReturnUrl!);
        return Redirect(a.VaiTro == "NguoiHienMau" ? "/TheoDoiCuaToi" : "/Admin/Dashboard");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(Module5Setup.Scheme);
        HttpContext.Session.Remove("Module5.MaTaiKhoan");
        HttpContext.Session.Remove("Module5.HoTen");
        HttpContext.Session.Remove("Module5.VaiTro");
        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccessDenied()
    {
        Response.StatusCode = 403;
        return View();
    }

    public IActionResult Register() => RedirectToAction(nameof(Login));
    public IActionResult VNeIDAuth() => RedirectToAction(nameof(Login));
}

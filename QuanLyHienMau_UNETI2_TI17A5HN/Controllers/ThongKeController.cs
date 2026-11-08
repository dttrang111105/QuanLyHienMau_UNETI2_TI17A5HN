using Microsoft.AspNetCore.Mvc;
using QuanLyHienMau_UNETI2_TI17A5HN.Filters;
using QuanLyHienMau_UNETI2_TI17A5HN.Repositories;
using QuanLyHienMau_UNETI2_TI17A5HN.Services;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers;

[KiemTraQuyen(ChiNhanVien = true)]
public class ThongKeController(ThongKeRepository repository, DongHoHeThong clock) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Dashboard() => View(await repository.DashboardAsync(clock.Now));

    [HttpGet]
    public async Task<IActionResult> Index() => View("Index", await repository.TongHopAsync(clock.Now));
}

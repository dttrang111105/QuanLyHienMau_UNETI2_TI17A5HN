using Microsoft.AspNetCore.Mvc;
using QuanLyHienMau_UNETI2_TI17A5HN.Filters;
using QuanLyHienMau_UNETI2_TI17A5HN.Services;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers;

[Module5Authorize(ChiNhanVien = true)]
public class ThongKeController(ThongKeService service) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Dashboard() => View(await service.DashboardAsync());

    [HttpGet]
    public async Task<IActionResult> Index() => View("Index", await service.TongHopAsync());
}

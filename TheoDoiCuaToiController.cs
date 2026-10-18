// Họ và tên: ĐIỀN HỌ TÊN CỦA BẠN
// Mã sinh viên: ĐIỀN MSSV CỦA BẠN
// Nội dung: Module 5 và phần nền dữ liệu cần thiết để chạy thử.
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers;
[Authorize(Roles = "NguoiHienMau")]
public class TheoDoiCuaToiController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return View(await db.PhieuDangKyHienMaus.AsNoTracking().Include(x => x.Dot).Include(x => x.LichKhams).Include(x => x.KetQuaHienMau).Where(x => x.NguoiHien.MaTaiKhoan == userId).OrderByDescending(x => x.NgayNop).ToListAsync());
    }

    public async Task<IActionResult> ChiTiet(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var p = await db.PhieuDangKyHienMaus.AsNoTracking().Include(x => x.Dot).Include(x => x.LichKhams).Include(x => x.KetQuaHienMau).SingleOrDefaultAsync(x => x.MaPhieu == id && x.NguoiHien.MaTaiKhoan == userId);
        if (p == null)
            return NotFound();
        return View("Index", new[] { p }.ToList());
    }
}

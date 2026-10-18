// Họ và tên: ĐIỀN HỌ TÊN CỦA BẠN
// Mã sinh viên: ĐIỀN MSSV CỦA BẠN
// Nội dung: hoàn thiện Module 5 trên project gốc.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Services;
public class Module5Pages(ApplicationDbContext db, Module5Service service)
{
    public async Task<IActionResult> Lich(Controller c, string? q, string? trangThai, DateTime? tuNgay, int page = 1, string view = "calendar", DateTime? ngay = null)
    {
        var query = db.LichKhamSangLocs.AsNoTracking().Include(x => x.Phieu).ThenInclude(x => x.NguoiHien).Include(x => x.Phieu).ThenInclude(x => x.Dot).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(x => x.Phieu.NguoiHien.HoTen.Contains(q) || x.BacSiSangLoc.Contains(q) || x.Phieu.Dot.TenDot.Contains(q));
        if (!string.IsNullOrWhiteSpace(trangThai))
            query = query.Where(x => x.TrangThai == trangThai);
        if (tuNgay.HasValue)
            query = query.Where(x => x.ThoiGianBatDau >= tuNgay.Value.Date);
        var date = (ngay ?? service.Now).Date;
        var week = date.AddDays(-(((int)date.DayOfWeek + 6) % 7));
        var cal = view != "list";
        if (cal)
            query = query.Where(x => x.ThoiGianBatDau >= week && x.ThoiGianBatDau < week.AddDays(7));
        var count = await query.CountAsync();
        var pages = cal ? 1 : Math.Max(1, (count + 9) / 10);
        page = Math.Clamp(page, 1, pages);
        c.ViewBag.Q = q;
        c.ViewBag.TrangThai = trangThai;
        c.ViewBag.TuNgay = tuNgay?.ToString("yyyy-MM-dd");
        c.ViewBag.Page = page;
        c.ViewBag.Pages = pages;
        c.ViewBag.Count = count;
        c.ViewBag.Calendar = cal;
        c.ViewBag.Week = week;
        c.ViewBag.Total = await db.LichKhamSangLocs.CountAsync();
        c.ViewBag.Upcoming = await db.LichKhamSangLocs.CountAsync(x => x.TrangThai == TrangThaiLich.DaLenLich && x.ThoiGianBatDau >= service.Now);
        c.ViewBag.Completed = await db.LichKhamSangLocs.CountAsync(x => x.TrangThai == TrangThaiLich.HoanThanh);
        c.ViewBag.Cancelled = await db.LichKhamSangLocs.CountAsync(x => x.TrangThai == TrangThaiLich.DaHuy);
        var ordered = query.OrderBy(x => x.ThoiGianBatDau).ThenBy(x => x.MaLichKhamSangLoc);
        return c.View("~/Views/Admin/LichKhamSangLoc.cshtml", cal ? await ordered.ToListAsync() : await ordered.Skip((page - 1) * 10).Take(10).ToListAsync());
    }

    public async Task<IActionResult> KetQua(Controller c) => c.View("~/Views/Admin/KetQuaXetNghiem.cshtml", await db.PhieuDangKyHienMaus.AsNoTracking().Include(x => x.NguoiHien).Include(x => x.Dot).Include(x => x.KetQuaHienMau).Where(x => x.TrangThai == TrangThaiPhieu.DaKham || x.KetQuaHienMau != null).OrderByDescending(x => x.MaPhieu).ToListAsync());
}

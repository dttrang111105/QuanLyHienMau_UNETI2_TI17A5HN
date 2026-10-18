using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Services;
public class Module5Pages(ApplicationDbContext db, Module5Service service)
{
    public async Task<IActionResult> Lich(Controller c, string? q = null, string? trangThai = null, DateTime? tuNgay = null, int page = 1, string view = "calendar", DateTime? ngay = null, int? maDot = null, string? bacSi = null, bool newSlot = false, int? editId = null, LichFormVm? form = null)
    {
        var date = (ngay ?? service.Now).Date;
        if (date.Year < 2000 || date.Year > 2100) return c.BadRequest("Ngày không hợp lệ.");
        var query = db.LichKhamSangLocs.AsNoTracking().Include(x => x.Phieu).ThenInclude(x => x.NguoiHien).Include(x => x.Phieu).ThenInclude(x => x.Dot).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(x => x.Phieu.NguoiHien.HoTen.Contains(q) || x.BacSiSangLoc.Contains(q) || x.Phieu.Dot.TenDot.Contains(q) || x.MaPhieu.ToString() == q);
        if (!string.IsNullOrWhiteSpace(trangThai)) query = query.Where(x => x.TrangThai == trangThai);
        if (maDot.HasValue) query = query.Where(x => x.Phieu.MaDot == maDot);
        if (!string.IsNullOrWhiteSpace(bacSi)) { var doctorKey = bacSi.Trim().Normalize(System.Text.NormalizationForm.FormC).ToUpperInvariant(); query = query.Where(x => x.BacSiChuanHoa == doctorKey); }
        if (tuNgay.HasValue) query = query.Where(x => x.ThoiGianBatDau >= tuNgay.Value.Date);
        var end = date.AddDays(1);
        var dayRows = await query.Where(x => x.ThoiGianBatDau < end && x.ThoiGianKetThuc > date).OrderBy(x => x.ThoiGianBatDau).ThenBy(x => x.MaLichKhamSangLoc).ToListAsync();
        var table = view is "table" or "list";
        if (table && ngay.HasValue) query = query.Where(x => x.ThoiGianBatDau < end && x.ThoiGianKetThuc > date);
        var count = table ? await query.CountAsync() : dayRows.Count;
        var pages = table ? Math.Max(1, (count + 9) / 10) : 1;
        page = Math.Clamp(page, 1, pages);
        var doctors = await db.TaiKhoans.Where(x => x.VaiTro == "NhanVienYTe" && x.TrangThai).Select(x => x.HoTen).ToListAsync();
        doctors.AddRange(await db.LichKhamSangLocs.Select(x => x.BacSiSangLoc).Distinct().ToListAsync());
        doctors = doctors.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList();
        if (form == null && editId.HasValue)
        {
            var l = await db.LichKhamSangLocs.AsNoTracking().SingleOrDefaultAsync(x => x.MaLichKhamSangLoc == editId);
            if (l == null) return c.NotFound();
            if (l.TrangThai != TrangThaiLich.DaLenLich) return c.BadRequest("Lịch không được phép sửa.");
            form = new LichFormVm { Id = l.MaLichKhamSangLoc, MaPhieu = l.MaPhieu, ThoiGianBatDau = l.ThoiGianBatDau, ThoiGianKetThuc = l.ThoiGianKetThuc, BacSiSangLoc = l.BacSiSangLoc, HinhThucTiepNhan = l.HinhThucTiepNhan, DiaDiemHoacLienKet = l.DiaDiemHoacLienKet, GhiChu = l.GhiChu };
        }
        form ??= new LichFormVm { ThoiGianBatDau = service.Now.Date.AddDays(1).AddHours(9), ThoiGianKetThuc = service.Now.Date.AddDays(1).AddHours(10) };
        form.Phieus = await db.PhieuDangKyHienMaus.AsNoTracking().Include(x => x.NguoiHien).Include(x => x.Dot)
            .Where(x => (x.TrangThai == TrangThaiPhieu.DaXacNhan && !x.LichKhams.Any(l => l.TrangThai != TrangThaiLich.DaHuy)) || (form.Id != 0 && x.MaPhieu == form.MaPhieu))
            .OrderBy(x => x.MaPhieu).Select(x => new SelectListItem($"#{x.MaPhieu} • {x.NguoiHien.HoTen} • {x.Dot.TenDot}", x.MaPhieu.ToString())).ToListAsync();
        var effective = dayRows.Where(x => x.TrangThai != TrangThaiLich.DaHuy).ToList();
        var conflicts = effective.Where(a => effective.Any(b => a.MaLichKhamSangLoc != b.MaLichKhamSangLoc && a.ThoiGianBatDau < b.ThoiGianKetThuc && b.ThoiGianBatDau < a.ThoiGianKetThuc && (a.BacSiChuanHoa == b.BacSiChuanHoa || a.Phieu.MaNguoiHien == b.Phieu.MaNguoiHien))).ToList();
        return c.View("~/Views/Admin/LichKhamSangLoc.cshtml", new LichPageVm
        {
            Ngay = date, Now = service.Now, View = table ? "table" : "calendar", Q = q, TrangThai = trangThai, MaDot = maDot, BacSi = bacSi, Page = page, Pages = pages, Count = count,
            Total = await db.LichKhamSangLocs.CountAsync(), Completed = await db.LichKhamSangLocs.CountAsync(x => x.TrangThai == TrangThaiLich.HoanThanh), Cancelled = await db.LichKhamSangLocs.CountAsync(x => x.TrangThai == TrangThaiLich.DaHuy),
            Upcoming = await db.LichKhamSangLocs.CountAsync(x => x.TrangThai == TrangThaiLich.DaLenLich && x.ThoiGianBatDau >= service.Now),
            Rows = table ? await query.OrderBy(x => x.ThoiGianBatDau).ThenBy(x => x.MaLichKhamSangLoc).Skip((page - 1) * 10).Take(10).ToListAsync() : dayRows,
            TrongNgay = dayRows, TrungLich = conflicts,
            SapToi = await db.LichKhamSangLocs.AsNoTracking().Include(x => x.Phieu).ThenInclude(x => x.NguoiHien).Where(x => x.TrangThai == TrangThaiLich.DaLenLich && x.ThoiGianBatDau >= service.Now).OrderBy(x => x.ThoiGianBatDau).Take(2).ToListAsync(),
            BacSis = doctors, Dots = await db.DotHienMaus.Select(x => new SelectListItem(x.TenDot, x.MaDot.ToString())).ToListAsync(), Form = form, ShowForm = newSlot || editId.HasValue
        });
    }

    public async Task<IActionResult> KetQua(Controller c, int? id = null, string? q = null, int? maDot = null, string? trangThai = null, string? nhomMau = null, int page = 1, KetQuaFormVm? form = null)
    {
        var all = db.PhieuDangKyHienMaus.AsNoTracking().Include(x => x.NguoiHien).Include(x => x.Dot).Include(x => x.KetQuaHienMau).Include(x => x.LichKhams)
            .Where(x => x.TrangThai == TrangThaiPhieu.DaKham || x.KetQuaHienMau != null);
        var query = all;
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(x => x.NguoiHien.HoTen.Contains(q) || x.Dot.TenDot.Contains(q) || x.MaPhieu.ToString() == q);
        if (maDot.HasValue) query = query.Where(x => x.MaDot == maDot);
        if (!string.IsNullOrWhiteSpace(trangThai)) query = query.Where(x => x.TrangThai == trangThai);
        if (!string.IsNullOrWhiteSpace(nhomMau)) query = query.Where(x => x.NguoiHien.NhomMau == nhomMau);
        var count = await query.CountAsync();var pages = Math.Max(1, (count + 4) / 5);page = Math.Clamp(page, 1, pages);
        var selected = id.HasValue ? await all.SingleOrDefaultAsync(x => x.MaPhieu == id) : await query.OrderBy(x => x.TrangThai == TrangThaiPhieu.DaKham ? 0 : 1).ThenBy(x => x.MaPhieu).FirstOrDefaultAsync();
        if (id.HasValue && selected == null) return c.NotFound();
        return c.View("~/Views/Admin/KetQuaXetNghiem.cshtml", new KetQuaPageVm
        {
            Q = q, TrangThai = trangThai, MaDot = maDot, NhomMau = nhomMau, Page = page, Pages = pages, Count = count, Total = await all.CountAsync(), Success = await all.CountAsync(x => x.TrangThai == TrangThaiPhieu.ThanhCong), Waiting = await all.CountAsync(x => x.TrangThai == TrangThaiPhieu.DaKham), Ineligible = await all.CountAsync(x => x.TrangThai == TrangThaiPhieu.KhongDat),
            Rows = await query.OrderBy(x => x.TrangThai == TrangThaiPhieu.DaKham ? 0 : 1).ThenBy(x => x.MaPhieu).Skip((page - 1) * 5).Take(5).ToListAsync(), Selected = selected,
            Dots = await db.DotHienMaus.Select(x => new SelectListItem(x.TenDot, x.MaDot.ToString())).ToListAsync(),
            Form = form ?? new KetQuaFormVm { MaPhieu = selected?.MaPhieu ?? 0, DiemDanhGia = selected?.KetQuaHienMau?.DiemDanhGia, NhanXet = selected?.KetQuaHienMau?.NhanXet ?? "", KetQua = selected?.KetQuaHienMau?.KetQua ?? "" }
        });
    }
}

using System.Data;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Services;
public class Module5Service(ApplicationDbContext db, TimeProvider clock)
{
    public DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(clock.GetUtcNow().UtcDateTime, TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh"));

    public async Task<string?> LuuLich(LichFormVm vm)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var phieu = await db.PhieuDangKyHienMaus.FindAsync(vm.MaPhieu);
        if (phieu == null)
            return "Phiếu không tồn tại.";
        var lich = vm.Id == 0 ? null : await db.LichKhamSangLocs.FindAsync(vm.Id);
        if (vm.Id != 0 && (lich == null || lich.MaPhieu != vm.MaPhieu || lich.TrangThai != TrangThaiLich.DaLenLich))
            return "Lịch không còn được phép sửa.";
        if (vm.Id == 0 && phieu.TrangThai != TrangThaiPhieu.DaXacNhan)
            return "Chỉ phiếu Đã xác nhận mới được lập lịch.";
        if (vm.Id != 0 && phieu.TrangThai != TrangThaiPhieu.ChoKham)
            return "Trạng thái phiếu không phù hợp để sửa lịch.";
        if (vm.ThoiGianBatDau == null || vm.ThoiGianKetThuc == null)
            return "Nhập đầy đủ thời gian khám.";
        var start = vm.ThoiGianBatDau.Value;
        var end = vm.ThoiGianKetThuc.Value;
        if (end <= start)
            return "Thời gian kết thúc phải sau thời gian bắt đầu.";
        if (start < Now)
            return "Không được lập hoặc đổi lịch vào thời gian trong quá khứ.";
        if (string.IsNullOrWhiteSpace(vm.BacSiSangLoc) || string.IsNullOrWhiteSpace(vm.DiaDiemHoacLienKet))
            return "Bác sĩ và địa điểm không được để trống.";
        if (vm.HinhThucTiepNhan != "Trực tiếp" && vm.HinhThucTiepNhan != "Trực tuyến")
            return "Hình thức tiếp nhận không hợp lệ.";
        var bs = vm.BacSiSangLoc.Trim().Normalize(System.Text.NormalizationForm.FormC);
        var norm = bs.ToUpperInvariant();
        var conHieuLuc = db.LichKhamSangLocs.Where(x => x.TrangThai != TrangThaiLich.DaHuy && x.MaLichKhamSangLoc != vm.Id);
        if (await conHieuLuc.AnyAsync(x => x.MaPhieu == vm.MaPhieu))
            return "Phiếu đã có lịch khám đang hiệu lực.";
        var trung = conHieuLuc.Where(x => x.ThoiGianBatDau < end && start < x.ThoiGianKetThuc);
        if (await trung.AnyAsync(x => x.Phieu.MaNguoiHien == phieu.MaNguoiHien))
            return "Người hiến máu bị trùng khoảng thời gian khám.";
        if (await trung.AnyAsync(x => x.BacSiChuanHoa == norm))
            return "Bác sĩ sàng lọc bị trùng khoảng thời gian khám.";
        if (lich == null)
        {
            lich = new LichKhamSangLoc
            {
                MaPhieu = vm.MaPhieu
            };
            db.Add(lich);
        }

        lich.ThoiGianBatDau = start;
        lich.ThoiGianKetThuc = end;
        lich.BacSiSangLoc = bs;
        lich.BacSiChuanHoa = norm;
        lich.HinhThucTiepNhan = vm.HinhThucTiepNhan;
        lich.DiaDiemHoacLienKet = vm.DiaDiemHoacLienKet.Trim();
        lich.GhiChu = vm.GhiChu?.Trim() ?? "";
        phieu.TrangThai = TrangThaiPhieu.ChoKham;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return null;
    }

    public async Task<string?> DoiTrangThaiLich(int id, bool hoanThanh)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var l = await db.LichKhamSangLocs.Include(x => x.Phieu).SingleOrDefaultAsync(x => x.MaLichKhamSangLoc == id);
        if (l == null)
            return "Lịch không tồn tại.";
        if (l.TrangThai != TrangThaiLich.DaLenLich || l.Phieu.TrangThai != TrangThaiPhieu.ChoKham)
            return "Chỉ xử lý lịch Đã lên lịch của phiếu Chờ khám sàng lọc.";
        if (hoanThanh && l.ThoiGianBatDau > Now)
            return "Chưa đến thời gian bắt đầu khám; không thể đánh dấu hoàn thành.";
        l.TrangThai = hoanThanh ? TrangThaiLich.HoanThanh : TrangThaiLich.DaHuy;
        l.Phieu.TrangThai = hoanThanh ? TrangThaiPhieu.DaKham : TrangThaiPhieu.DaXacNhan;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return null;
    }

    public async Task<string?> LuuKetQua(KetQuaFormVm vm)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var p = await db.PhieuDangKyHienMaus.Include(x => x.KetQuaHienMau).SingleOrDefaultAsync(x => x.MaPhieu == vm.MaPhieu);
        if (p == null)
            return "Phiếu không tồn tại.";
        if (p.TrangThai != TrangThaiPhieu.DaKham || p.KetQuaHienMau != null)
            return "Chỉ nhập một kết quả cho phiếu Đã khám sàng lọc; kết quả cuối đã lưu không được ghi đè.";
        if (!await db.LichKhamSangLocs.AnyAsync(x => x.MaPhieu == vm.MaPhieu && x.TrangThai == TrangThaiLich.HoanThanh))
            return "Phiếu chưa có lịch khám đã hoàn thành.";
        if (vm.DiemDanhGia is < 0 or > 100)
            return "Điểm phải từ 0 đến 100.";
        if (string.IsNullOrWhiteSpace(vm.NhanXet))
            return "Nhận xét không được để trống.";
        if (vm.KetQua != TrangThaiPhieu.ThanhCong && vm.KetQua != TrangThaiPhieu.KhongDat)
            return "Kết quả không hợp lệ.";
        db.Add(new KetQuaHienMau { MaPhieu = vm.MaPhieu, DiemDanhGia = vm.DiemDanhGia, NhanXet = vm.NhanXet.Trim(), KetQua = vm.KetQua, NgayCapNhat = Now });
        p.TrangThai = vm.KetQua;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return null;
    }

    public Task<List<TheoDotDong>> TheoDot(int? maDonVi = null) => db.DotHienMaus.AsNoTracking().Where(d => !maDonVi.HasValue || d.MaDonViToChuc == maDonVi.Value).Select(d => new TheoDotDong(d.MaDot, d.TenDot, db.PhieuDangKyHienMaus.Count(p => p.MaDot == d.MaDot), db.PhieuDangKyHienMaus.Count(p => p.MaDot == d.MaDot && p.TrangThai == TrangThaiPhieu.DaXacNhan), db.PhieuDangKyHienMaus.Count(p => p.MaDot == d.MaDot && TrangThaiPhieu.DaQuaXacNhan.Contains(p.TrangThai)), db.PhieuDangKyHienMaus.Count(p => p.MaDot == d.MaDot && p.LichKhams.Any(l => l.TrangThai == TrangThaiLich.HoanThanh)), db.PhieuDangKyHienMaus.Count(p => p.MaDot == d.MaDot && p.TrangThai == TrangThaiPhieu.ThanhCong), d.ChiTieuDonViMau)).ToListAsync();
    public async Task<DashboardVm> Dashboard()
    {
        var now = Now;
        return new DashboardVm
        {
            DonVi = await db.DonViToChucs.CountAsync(),
            Dot = await db.DotHienMaus.CountAsync(),
            DotMo = await db.DotHienMaus.CountAsync(x => x.TrangThai == "Đang mở đăng ký"),
            Nguoi = await db.NguoiHienMaus.CountAsync(),
            Phieu = await db.PhieuDangKyHienMaus.CountAsync(),
            ChoXacNhan = await db.PhieuDangKyHienMaus.CountAsync(x => x.TrangThai == TrangThaiPhieu.ChoXacNhan),
            DaXacNhan = await db.PhieuDangKyHienMaus.CountAsync(x => x.TrangThai == TrangThaiPhieu.DaXacNhan),
            LichSapToi = await db.LichKhamSangLocs.CountAsync(x => x.TrangThai == TrangThaiLich.DaLenLich && x.ThoiGianBatDau >= now),
            ThanhCong = await db.PhieuDangKyHienMaus.CountAsync(x => x.TrangThai == TrangThaiPhieu.ThanhCong),
            SapToi = await db.LichKhamSangLocs.AsNoTracking().Include(x => x.Phieu).ThenInclude(x => x.NguoiHien).Where(x => x.TrangThai == TrangThaiLich.DaLenLich && x.ThoiGianBatDau >= now).OrderBy(x => x.ThoiGianBatDau).Take(5).ToListAsync(),
            TienDo = await TheoDot()
        };
    }

    public async Task<ThongKeVm> ThongKe(int nam, int? maDonVi = null)
    {
        var dots = await TheoDot(maDonVi);
        var max = dots.Count == 0 ? 0 : dots.Max(x => x.Tong);
        var pm = await db.PhieuDangKyHienMaus.Where(x => x.NgayNop.Year == nam && (!maDonVi.HasValue || x.Dot.MaDonViToChuc == maDonVi.Value)).GroupBy(x => x.NgayNop.Month).Select(g => new { Thang = g.Key, So = g.Count() }).ToListAsync();
        var lm = await db.LichKhamSangLocs.Where(x => x.ThoiGianBatDau.Year == nam && (!maDonVi.HasValue || x.Phieu.Dot.MaDonViToChuc == maDonVi.Value)).GroupBy(x => x.ThoiGianBatDau.Month).Select(g => new { Thang = g.Key, So = g.Count() }).ToListAsync();
        return new ThongKeVm
        {
            Nam = nam,
            MaDonVi = maDonVi,
            DonVis = await db.DonViToChucs.Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(x.TenDonViToChuc, x.MaDonViToChuc.ToString())).ToListAsync(),
            TheoDot = dots,
            NhieuPhieuNhat = max == 0 ? [] : dots.Where(x => x.Tong == max).ToList(),
            DotTheoDonVi = await db.DonViToChucs.Where(d => !maDonVi.HasValue || d.MaDonViToChuc == maDonVi.Value).Select(d => new ThongKeDong(d.TenDonViToChuc, db.DotHienMaus.Count(x => x.MaDonViToChuc == d.MaDonViToChuc))).ToListAsync(),
            PhieuTheoTrangThai = await db.PhieuDangKyHienMaus.Where(x => !maDonVi.HasValue || x.Dot.MaDonViToChuc == maDonVi.Value).GroupBy(x => x.TrangThai).OrderByDescending(g => g.Count()).Select(g => new ThongKeDong(g.Key, g.Count())).ToListAsync(),
            ThanhCongTheoDonVi = await db.DonViToChucs.Where(d => !maDonVi.HasValue || d.MaDonViToChuc == maDonVi.Value).Select(d => new ThongKeDong(d.TenDonViToChuc, db.PhieuDangKyHienMaus.Count(p => p.Dot.MaDonViToChuc == d.MaDonViToChuc && p.TrangThai == TrangThaiPhieu.ThanhCong))).ToListAsync(),
            PhieuTheoThang = Enumerable.Range(1, 12).Select(m => new ThongKeDong($"Tháng {m}", pm.FirstOrDefault(x => x.Thang == m)?.So ?? 0)).ToList(),
            LichTheoThang = Enumerable.Range(1, 12).Select(m => new ThongKeDong($"Tháng {m}", lm.FirstOrDefault(x => x.Thang == m)?.So ?? 0)).ToList()
        };
    }
}

using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Services;

public sealed class KhamSangLocService(ApplicationDbContext db, DongHoHeThong clock, ILogger<KhamSangLocService> logger)
{
    public Task<KetQuaXuLy> KhoiPhucPhieuChoKhamAsync(int maPhieu) => ThucHienAsync(async () =>
    {
        var phieu = await db.PhieuDangKyHienMaus.SingleOrDefaultAsync(p => p.MaPhieu == maPhieu);
        if (phieu?.TrangThai != TrangThaiHienMau.Phieu.ChoKhamSangLoc ||
            await db.LichKhamSangLocs.AnyAsync(l => l.MaPhieu == maPhieu && l.TrangThai != TrangThaiHienMau.LichKham.DaHuy) ||
            await db.KetQuaHienMaus.AnyAsync(k => k.MaPhieu == maPhieu))
            return KetQuaXuLy.Fail("Phiếu đã có lịch hoặc kết quả và không thể lập thêm lịch.");

        // Phiếu được chuyển sang chờ khám từ luồng cũ mà chưa tạo lịch.
        phieu.TrangThai = TrangThaiHienMau.Phieu.DaXacNhan;
        await db.SaveChangesAsync();
        return KetQuaXuLy.Ok("Phiếu đã sẵn sàng để lập lịch.", maPhieu);
    });

    public Task<KetQuaXuLy> LuuLichAsync(LichKhamFormViewModel model, int? id = null) =>
        ThucHienAsync(async () =>
        {
            var error = KiemTraForm(model);
            if (error is not null) return KetQuaXuLy.Fail(error);
            var batDau = DateTime.SpecifyKind(model.ThoiGianBatDau!.Value, DateTimeKind.Unspecified);
            var ketThuc = DateTime.SpecifyKind(model.ThoiGianKetThuc!.Value, DateTimeKind.Unspecified);
            if (batDau < clock.Now)
                return KetQuaXuLy.Fail("Thời gian bắt đầu khám không được nằm trong quá khứ.");

            LichKhamSangLoc? lich = null;
            if (id.HasValue)
            {
                lich = await db.Set<LichKhamSangLoc>().SingleOrDefaultAsync(x => x.MaLichKhamSangLoc == id.Value);
                if (lich is null) return KetQuaXuLy.Fail("Lịch khám không tồn tại.");
                if (lich.MaPhieu != model.MaPhieu)
                    return KetQuaXuLy.Fail("Không được đổi phiếu đăng ký của lịch khám.");
                if (lich.TrangThai != TrangThaiHienMau.LichKham.DaLenLich)
                    return KetQuaXuLy.Fail("Chỉ lịch đang lên lịch mới được thay đổi.");
            }

            var phieu = await db.Set<PhieuDangKyHienMau>().Include(x => x.NguoiHienMau)
                .Include(x => x.DotHienMau).SingleOrDefaultAsync(x => x.MaPhieu == model.MaPhieu);
            if (phieu is null) return KetQuaXuLy.Fail("Phiếu đăng ký không tồn tại.");
            var trangThaiCanCo = id.HasValue ? TrangThaiHienMau.Phieu.ChoKhamSangLoc : TrangThaiHienMau.Phieu.DaXacNhan;
            if (phieu.TrangThai != trangThaiCanCo)
                return KetQuaXuLy.Fail(id.HasValue
                    ? "Phiếu không còn ở trạng thái Chờ khám sàng lọc."
                    : "Chỉ phiếu Đã xác nhận mới được lập lịch khám sàng lọc.");
            if (phieu.NguoiHienMau is null || !TrangThaiHienMau.DangHoatDong(phieu.NguoiHienMau.TrangThai))
                return KetQuaXuLy.Fail("Người hiến máu hiện không hoạt động.");
            if (await db.Set<KetQuaHienMau>().AnyAsync(x => x.MaPhieu == model.MaPhieu))
                return KetQuaXuLy.Fail("Phiếu đã có kết quả cuối cùng.");

            var lichKhac = db.Set<LichKhamSangLoc>().Where(x =>
                x.TrangThai != TrangThaiHienMau.LichKham.DaHuy && (!id.HasValue || x.MaLichKhamSangLoc != id.Value));
            if (await lichKhac.AnyAsync(x => x.MaPhieu == model.MaPhieu))
                return KetQuaXuLy.Fail("Phiếu đã có một lịch khám đang hiệu lực.");

            var giaoNhau = lichKhac.Where(x => x.ThoiGianBatDau < ketThuc && batDau < x.ThoiGianKetThuc);
            if (await giaoNhau.AnyAsync(x => x.PhieuDangKyHienMau != null && x.PhieuDangKyHienMau.MaNguoiHien == phieu.MaNguoiHien))
                return KetQuaXuLy.Fail("Người hiến máu đã có lịch khám trùng khoảng thời gian này.");
            var bacSi = ChuanHoaTen(model.BacSiSangLoc);
            var bacSiSoSanh = bacSi.ToUpperInvariant();
            if (await giaoNhau.AnyAsync(x => x.BacSiSangLoc != null && x.BacSiSangLoc.ToUpper() == bacSiSoSanh))
                return KetQuaXuLy.Fail("Bác sĩ sàng lọc đã có lịch trùng khoảng thời gian này.");

            if (lich is null)
            {
                lich = new LichKhamSangLoc { MaPhieu = model.MaPhieu };
                db.Set<LichKhamSangLoc>().Add(lich);
            }
            lich.ThoiGianBatDau = batDau;
            lich.ThoiGianKetThuc = ketThuc;
            lich.HinhThucTiepNhan = model.HinhThucTiepNhan.Trim();
            lich.DiaDiemHoacLienKet = model.DiaDiemHoacLienKet.Trim();
            lich.BacSiSangLoc = bacSi;
            lich.GhiChu = model.GhiChu?.Trim();
            phieu.TrangThai = TrangThaiHienMau.Phieu.ChoKhamSangLoc;
            await db.SaveChangesAsync();
            return KetQuaXuLy.Ok(id.HasValue ? "Đã cập nhật lịch khám." : "Đã lập lịch khám sàng lọc.", lich.MaLichKhamSangLoc);
        });

    public Task<KetQuaXuLy> HuyLichAsync(int id, string? ghiChu = null) => ThucHienAsync(async () =>
    {
        var lich = await db.Set<LichKhamSangLoc>().Include(x => x.PhieuDangKyHienMau)
            .SingleOrDefaultAsync(x => x.MaLichKhamSangLoc == id);
        if (lich is null) return KetQuaXuLy.Fail("Lịch khám không tồn tại.");
        if (lich.TrangThai != TrangThaiHienMau.LichKham.DaLenLich ||
            lich.PhieuDangKyHienMau?.TrangThai != TrangThaiHienMau.Phieu.ChoKhamSangLoc)
            return KetQuaXuLy.Fail("Chỉ lịch đang lên lịch của phiếu Chờ khám sàng lọc mới được hủy.");
        if (await db.Set<KetQuaHienMau>().AnyAsync(x => x.MaPhieu == lich.MaPhieu))
            return KetQuaXuLy.Fail("Phiếu đã có kết quả cuối cùng, không được hủy lịch.");
        if (await db.Set<LichKhamSangLoc>().AnyAsync(x => x.MaPhieu == lich.MaPhieu &&
            x.MaLichKhamSangLoc != id && x.TrangThai != TrangThaiHienMau.LichKham.DaHuy))
            return KetQuaXuLy.Fail("Phiếu có lịch hiệu lực khác; vui lòng kiểm tra lại trước khi hủy.");
        if (!string.IsNullOrWhiteSpace(ghiChu))
        {
            var ghiChuHuy = $"{lich.GhiChu} [Đã hủy: {ghiChu.Trim()}]".Trim();
            if (ghiChuHuy.Length > 500)
                return KetQuaXuLy.Fail("Ghi chú lịch khám không được vượt quá 500 ký tự.");
            lich.GhiChu = ghiChuHuy;
        }
        lich.TrangThai = TrangThaiHienMau.LichKham.DaHuy;
        lich.PhieuDangKyHienMau.TrangThai = TrangThaiHienMau.Phieu.DaXacNhan;
        await db.SaveChangesAsync();
        return KetQuaXuLy.Ok("Đã hủy lịch khám. Phiếu trở về Đã xác nhận để có thể lập lịch mới.", id);
    });

    public Task<KetQuaXuLy> HoanThanhLichAsync(int id) => ThucHienAsync(async () =>
    {
        var lich = await db.Set<LichKhamSangLoc>().Include(x => x.PhieuDangKyHienMau)
            .SingleOrDefaultAsync(x => x.MaLichKhamSangLoc == id);
        if (lich is null) return KetQuaXuLy.Fail("Lịch khám không tồn tại.");
        if (lich.TrangThai != TrangThaiHienMau.LichKham.DaLenLich ||
            lich.PhieuDangKyHienMau?.TrangThai != TrangThaiHienMau.Phieu.ChoKhamSangLoc)
            return KetQuaXuLy.Fail("Lịch đã hủy, đã hoàn thành hoặc phiếu không còn chờ khám.");
        if (lich.ThoiGianBatDau > clock.Now)
            return KetQuaXuLy.Fail("Chưa đến thời gian khám, không thể đánh dấu hoàn thành.");
        if (await db.Set<KetQuaHienMau>().AnyAsync(x => x.MaPhieu == lich.MaPhieu))
            return KetQuaXuLy.Fail("Phiếu đã có kết quả cuối cùng.");
        lich.TrangThai = TrangThaiHienMau.LichKham.DaHoanThanh;
        lich.PhieuDangKyHienMau.TrangThai = TrangThaiHienMau.Phieu.DaKhamSangLoc;
        await db.SaveChangesAsync();
        return KetQuaXuLy.Ok("Đã hoàn thành khám sàng lọc. Phiếu có thể được ghi nhận kết quả.", id);
    });

    public Task<KetQuaXuLy> GhiKetQuaAsync(KetQuaFormViewModel model, int? maKetQua = null) => ThucHienAsync(async () =>
    {
        var error = KiemTraForm(model);
        if (error is not null) return KetQuaXuLy.Fail(error);
        var phieu = await db.Set<PhieuDangKyHienMau>().Include(x => x.DotHienMau)
            .SingleOrDefaultAsync(x => x.MaPhieu == model.MaPhieu);
        if (phieu is null) return KetQuaXuLy.Fail("Phiếu đăng ký không tồn tại.");
        var ketQua = await db.Set<KetQuaHienMau>().SingleOrDefaultAsync(x => x.MaPhieu == model.MaPhieu);
        if (!maKetQua.HasValue)
        {
            if (phieu.TrangThai != TrangThaiHienMau.Phieu.DaKhamSangLoc)
                return KetQuaXuLy.Fail("Chỉ phiếu Đã khám sàng lọc mới được ghi nhận kết quả cuối cùng.");
            if (ketQua is not null)
                return KetQuaXuLy.Fail("Phiếu đã có kết quả, không được tạo kết quả thứ hai.");
        }
        else if (ketQua is null || ketQua.MaKetQuaHienMau != maKetQua ||
                 phieu.TrangThai != ketQua.KetQua ||
                 phieu.TrangThai is not (TrangThaiHienMau.Phieu.HienMauThanhCong or TrangThaiHienMau.Phieu.KhongDuDieuKienHien))
        {
            return KetQuaXuLy.Fail("Kết quả và trạng thái phiếu không còn hợp lệ để cập nhật.");
        }

        if (!await db.Set<LichKhamSangLoc>().AnyAsync(x => x.MaPhieu == model.MaPhieu &&
            x.TrangThai == TrangThaiHienMau.LichKham.DaHoanThanh))
            return KetQuaXuLy.Fail("Phiếu chưa có lịch khám đã hoàn thành.");

        if (ketQua is null)
        {
            ketQua = new KetQuaHienMau { MaPhieu = model.MaPhieu };
            db.Set<KetQuaHienMau>().Add(ketQua);
        }

        ketQua.DiemDanhGia = (double?)model.DiemDanhGia;
        ketQua.NhanXet = model.NhanXet?.Trim();
        ketQua.KetQua = model.KetQua;
        ketQua.NgayCapNhat = clock.Now;
        phieu.TrangThai = model.KetQua;
        await db.SaveChangesAsync();
        var soThanhCong = await db.Set<PhieuDangKyHienMau>().CountAsync(x =>
            x.MaDot == phieu.MaDot && x.TrangThai == TrangThaiHienMau.Phieu.HienMauThanhCong);
        var canhBao = phieu.DotHienMau is not null && phieu.DotHienMau.ChiTieuDonViMau > 0 && soThanhCong >= phieu.DotHienMau.ChiTieuDonViMau
            ? $" Đợt {phieu.DotHienMau.TenDot} đã đạt chỉ tiêu ({soThanhCong}/{phieu.DotHienMau.ChiTieuDonViMau})."
            : "";
        var thongBao = maKetQua.HasValue ? "Đã cập nhật kết quả hiến máu." : "Đã ghi nhận và công bố kết quả hiến máu.";
        return KetQuaXuLy.Ok(thongBao + canhBao, ketQua.MaKetQuaHienMau);
    });

    private async Task<KetQuaXuLy> ThucHienAsync(Func<Task<KetQuaXuLy>> action)
    {
        try
        {
            await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var result = await action();
            if (result.Success) await transaction.CommitAsync();
            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            logger.LogWarning(ex, "Dữ liệu Module 5 đã thay đổi trong lúc xử lý.");
            return KetQuaXuLy.Fail("Dữ liệu vừa được người khác cập nhật. Vui lòng tải lại trang.");
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "Không thể lưu thay đổi Module 5.");
            return KetQuaXuLy.Fail("Không thể lưu do dữ liệu đã thay đổi hoặc bị trùng. Vui lòng tải lại và kiểm tra.");
        }
        catch (DbException ex)
        {
            logger.LogWarning(ex, "Xung đột giao dịch Module 5.");
            return KetQuaXuLy.Fail("Dữ liệu đang được xử lý đồng thời. Vui lòng tải lại và thử lại.");
        }
    }

    private static string? KiemTraForm(object model)
    {
        var results = new List<ValidationResult>();
        return Validator.TryValidateObject(model, new ValidationContext(model), results, true)
            ? null : string.Join(" ", results.Select(x => x.ErrorMessage));
    }

    private static string ChuanHoaTen(string value) =>
        string.Join(" ", value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
}

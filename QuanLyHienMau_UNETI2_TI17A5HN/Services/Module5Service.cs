using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Services;

public sealed record Module5Result(bool Success, string Message, int? Id = null)
{
    public static Module5Result Fail(string message) => new(false, message);
    public static Module5Result Ok(string message, int? id = null) => new(true, message, id);
}

public sealed class Module5Service(ApplicationDbContext db, Module5Clock clock, ILogger<Module5Service> logger)
{
    public Task<Module5Result> LuuLichAsync(LichKhamFormViewModel model, int? id = null) =>
        ThucHienAsync(async () =>
        {
            var error = KiemTraForm(model);
            if (error is not null) return Module5Result.Fail(error);
            var batDau = DateTime.SpecifyKind(model.ThoiGianBatDau!.Value, DateTimeKind.Unspecified);
            var ketThuc = DateTime.SpecifyKind(model.ThoiGianKetThuc!.Value, DateTimeKind.Unspecified);
            if (batDau < clock.Now)
                return Module5Result.Fail("Thời gian bắt đầu khám không được nằm trong quá khứ.");

            LichKhamSangLoc? lich = null;
            if (id.HasValue)
            {
                lich = await db.Set<LichKhamSangLoc>().SingleOrDefaultAsync(x => x.MaLichKhamSangLoc == id.Value);
                if (lich is null) return Module5Result.Fail("Lịch khám không tồn tại.");
                if (lich.MaPhieu != model.MaPhieu)
                    return Module5Result.Fail("Không được đổi phiếu đăng ký của lịch khám.");
                if (lich.TrangThai != Module5TrangThai.LichKham.DaLenLich)
                    return Module5Result.Fail("Chỉ lịch đang lên lịch mới được thay đổi.");
            }

            var phieu = await db.Set<PhieuDangKyHienMau>().Include(x => x.NguoiHienMau)
                .Include(x => x.DotHienMau).SingleOrDefaultAsync(x => x.MaPhieu == model.MaPhieu);
            if (phieu is null) return Module5Result.Fail("Phiếu đăng ký không tồn tại.");
            var trangThaiCanCo = id.HasValue ? Module5TrangThai.Phieu.ChoKhamSangLoc : Module5TrangThai.Phieu.DaXacNhan;
            if (phieu.TrangThai != trangThaiCanCo)
                return Module5Result.Fail(id.HasValue
                    ? "Phiếu không còn ở trạng thái Chờ khám sàng lọc."
                    : "Chỉ phiếu Đã xác nhận mới được lập lịch khám sàng lọc.");
            if (phieu.NguoiHienMau is null || phieu.NguoiHienMau.TrangThai != "Hoạt động")
                return Module5Result.Fail("Người hiến máu hiện không hoạt động.");
            if (await db.Set<KetQuaHienMau>().AnyAsync(x => x.MaPhieu == model.MaPhieu))
                return Module5Result.Fail("Phiếu đã có kết quả cuối cùng.");

            var lichKhac = db.Set<LichKhamSangLoc>().Where(x =>
                x.TrangThai != Module5TrangThai.LichKham.DaHuy && (!id.HasValue || x.MaLichKhamSangLoc != id.Value));
            if (await lichKhac.AnyAsync(x => x.MaPhieu == model.MaPhieu))
                return Module5Result.Fail("Phiếu đã có một lịch khám đang hiệu lực.");

            var giaoNhau = lichKhac.Where(x => x.ThoiGianBatDau < ketThuc && batDau < x.ThoiGianKetThuc);
            if (await giaoNhau.AnyAsync(x => x.PhieuDangKyHienMau != null && x.PhieuDangKyHienMau.MaNguoiHien == phieu.MaNguoiHien))
                return Module5Result.Fail("Người hiến máu đã có lịch khám trùng khoảng thời gian này.");
            var bacSi = ChuanHoaTen(model.BacSiSangLoc);
            var bacSiSoSanh = bacSi.ToUpperInvariant();
            if (await giaoNhau.AnyAsync(x => x.BacSiSangLoc != null && x.BacSiSangLoc.ToUpper() == bacSiSoSanh))
                return Module5Result.Fail("Bác sĩ sàng lọc đã có lịch trùng khoảng thời gian này.");

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
            phieu.TrangThai = Module5TrangThai.Phieu.ChoKhamSangLoc;
            await db.SaveChangesAsync();
            return Module5Result.Ok(id.HasValue ? "Đã cập nhật lịch khám." : "Đã lập lịch khám sàng lọc.", lich.MaLichKhamSangLoc);
        });

    public Task<Module5Result> HuyLichAsync(int id) => ThucHienAsync(async () =>
    {
        var lich = await db.Set<LichKhamSangLoc>().Include(x => x.PhieuDangKyHienMau)
            .SingleOrDefaultAsync(x => x.MaLichKhamSangLoc == id);
        if (lich is null) return Module5Result.Fail("Lịch khám không tồn tại.");
        if (lich.TrangThai != Module5TrangThai.LichKham.DaLenLich ||
            lich.PhieuDangKyHienMau?.TrangThai != Module5TrangThai.Phieu.ChoKhamSangLoc)
            return Module5Result.Fail("Chỉ lịch đang lên lịch của phiếu Chờ khám sàng lọc mới được hủy.");
        if (await db.Set<KetQuaHienMau>().AnyAsync(x => x.MaPhieu == lich.MaPhieu))
            return Module5Result.Fail("Phiếu đã có kết quả cuối cùng, không được hủy lịch.");
        if (await db.Set<LichKhamSangLoc>().AnyAsync(x => x.MaPhieu == lich.MaPhieu &&
            x.MaLichKhamSangLoc != id && x.TrangThai != Module5TrangThai.LichKham.DaHuy))
            return Module5Result.Fail("Phiếu có lịch hiệu lực khác; vui lòng kiểm tra lại trước khi hủy.");
        lich.TrangThai = Module5TrangThai.LichKham.DaHuy;
        lich.PhieuDangKyHienMau.TrangThai = Module5TrangThai.Phieu.DaXacNhan;
        await db.SaveChangesAsync();
        return Module5Result.Ok("Đã hủy lịch khám. Phiếu trở về Đã xác nhận để có thể lập lịch mới.", id);
    });

    public Task<Module5Result> HoanThanhLichAsync(int id) => ThucHienAsync(async () =>
    {
        var lich = await db.Set<LichKhamSangLoc>().Include(x => x.PhieuDangKyHienMau)
            .SingleOrDefaultAsync(x => x.MaLichKhamSangLoc == id);
        if (lich is null) return Module5Result.Fail("Lịch khám không tồn tại.");
        if (lich.TrangThai != Module5TrangThai.LichKham.DaLenLich ||
            lich.PhieuDangKyHienMau?.TrangThai != Module5TrangThai.Phieu.ChoKhamSangLoc)
            return Module5Result.Fail("Lịch đã hủy, đã hoàn thành hoặc phiếu không còn chờ khám.");
        if (lich.ThoiGianBatDau > clock.Now)
            return Module5Result.Fail("Chưa đến thời gian khám, không thể đánh dấu hoàn thành.");
        if (await db.Set<KetQuaHienMau>().AnyAsync(x => x.MaPhieu == lich.MaPhieu))
            return Module5Result.Fail("Phiếu đã có kết quả cuối cùng.");
        lich.TrangThai = Module5TrangThai.LichKham.DaHoanThanh;
        lich.PhieuDangKyHienMau.TrangThai = Module5TrangThai.Phieu.DaKhamSangLoc;
        await db.SaveChangesAsync();
        return Module5Result.Ok("Đã hoàn thành khám sàng lọc. Phiếu có thể được ghi nhận kết quả.", id);
    });

    public Task<Module5Result> GhiKetQuaAsync(KetQuaFormViewModel model) => ThucHienAsync(async () =>
    {
        var error = KiemTraForm(model);
        if (error is not null) return Module5Result.Fail(error);
        var phieu = await db.Set<PhieuDangKyHienMau>().Include(x => x.DotHienMau)
            .SingleOrDefaultAsync(x => x.MaPhieu == model.MaPhieu);
        if (phieu is null) return Module5Result.Fail("Phiếu đăng ký không tồn tại.");
        if (phieu.TrangThai != Module5TrangThai.Phieu.DaKhamSangLoc)
            return Module5Result.Fail("Chỉ phiếu Đã khám sàng lọc mới được ghi nhận kết quả cuối cùng.");
        if (!await db.Set<LichKhamSangLoc>().AnyAsync(x => x.MaPhieu == model.MaPhieu &&
            x.TrangThai == Module5TrangThai.LichKham.DaHoanThanh))
            return Module5Result.Fail("Phiếu chưa có lịch khám đã hoàn thành.");
        if (await db.Set<KetQuaHienMau>().AnyAsync(x => x.MaPhieu == model.MaPhieu))
            return Module5Result.Fail("Phiếu đã có kết quả, không được tạo kết quả thứ hai.");

        var ketQua = new KetQuaHienMau
        {
            MaPhieu = model.MaPhieu,
            DiemDanhGia = (double?)model.DiemDanhGia,
            NhanXet = model.NhanXet?.Trim(),
            KetQua = model.KetQua,
            NgayCapNhat = clock.Now
        };
        db.Set<KetQuaHienMau>().Add(ketQua);
        phieu.TrangThai = model.KetQua;
        await db.SaveChangesAsync();
        var soThanhCong = await db.Set<PhieuDangKyHienMau>().CountAsync(x =>
            x.MaDot == phieu.MaDot && x.TrangThai == Module5TrangThai.Phieu.HienMauThanhCong);
        var canhBao = phieu.DotHienMau is not null && phieu.DotHienMau.ChiTieuDonViMau > 0 && soThanhCong >= phieu.DotHienMau.ChiTieuDonViMau
            ? $" Đợt {phieu.DotHienMau.TenDot} đã đạt chỉ tiêu ({soThanhCong}/{phieu.DotHienMau.ChiTieuDonViMau})."
            : "";
        return Module5Result.Ok("Đã ghi nhận và công bố kết quả hiến máu." + canhBao, ketQua.MaKetQuaHienMau);
    });

    private async Task<Module5Result> ThucHienAsync(Func<Task<Module5Result>> action)
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
            return Module5Result.Fail("Dữ liệu vừa được người khác cập nhật. Vui lòng tải lại trang.");
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "Không thể lưu thay đổi Module 5.");
            return Module5Result.Fail("Không thể lưu do dữ liệu đã thay đổi hoặc bị trùng. Vui lòng tải lại và kiểm tra.");
        }
        catch (DbException ex)
        {
            logger.LogWarning(ex, "Xung đột giao dịch Module 5.");
            return Module5Result.Fail("Dữ liệu đang được xử lý đồng thời. Vui lòng tải lại và thử lại.");
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

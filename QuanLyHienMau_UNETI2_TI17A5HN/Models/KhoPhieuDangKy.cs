namespace QuanLyHienMau_UNETI2_TI17A5HN.Models;

// Dữ liệu mẫu lưu trong bộ nhớ vì dự án hiện chưa cấu hình CSDL/ORM.
public sealed class KhoPhieuDangKy
{
    private readonly object _sync = new();
    private readonly List<PhieuDangKy> _phieu =
    [
        new() { Id = 1, MaPhieu = "VTL-2026-08991", HoTenNguoiHien = "Đỗ Hoàng Quân", NhomMau = "O+", TenDot = "Giọt Hồng Thanh Niên", DonViToChuc = "Đoàn Thanh niên UNETI", YeuCauDot = "18–60 tuổi; sức khỏe ổn định; nhóm máu ưu tiên O+, A+", NgayNop = DateTime.Today.AddDays(-1) },
        new() { Id = 2, MaPhieu = "VTL-2026-08992", HoTenNguoiHien = "Nguyễn Minh Anh", NhomMau = "A+", TenDot = "Giọt Hồng Thanh Niên", DonViToChuc = "Đoàn Thanh niên UNETI", YeuCauDot = "18–60 tuổi; sức khỏe ổn định; nhóm máu ưu tiên O+, A+", NgayNop = DateTime.Today.AddDays(-2) },
        new() { Id = 3, MaPhieu = "VTL-2026-08993", HoTenNguoiHien = "Trần Thu Hà", NhomMau = "B+", TenDot = "Sắc Đỏ Tình Người", DonViToChuc = "Viện Huyết học TW", YeuCauDot = "18–60 tuổi; sức khỏe ổn định; nhóm máu ưu tiên O-, O+", NgayNop = DateTime.Today.AddDays(-3) },
        new() { Id = 4, MaPhieu = "VTL-2026-08994", HoTenNguoiHien = "Lê Quốc Bảo", NhomMau = "O-", TenDot = "Sắc Đỏ Tình Người", DonViToChuc = "Viện Huyết học TW", YeuCauDot = "18–60 tuổi; sức khỏe ổn định; nhóm máu ưu tiên O-, O+", TrangThai = TrangThaiPhieu.DaXacNhan, NgayNop = DateTime.Today.AddDays(-4), NgayXuLy = DateTime.Now.AddDays(-2), GhiChuXacNhan = "Thông tin hồ sơ hợp lệ." },
        new() { Id = 5, MaPhieu = "VTL-2026-08995", HoTenNguoiHien = "Phạm Gia Huy", NhomMau = "AB+", TenDot = "Ngày hội hiến máu", DonViToChuc = "Quận Hai Bà Trưng", YeuCauDot = "18–60 tuổi; sức khỏe ổn định", TrangThai = TrangThaiPhieu.ChoKham, NgayNop = DateTime.Today.AddDays(-5) },
        new() { Id = 6, MaPhieu = "VTL-2026-08996", HoTenNguoiHien = "Vũ Ngọc Mai", NhomMau = "A-", TenDot = "Ngày hội hiến máu", DonViToChuc = "Quận Hai Bà Trưng", YeuCauDot = "18–60 tuổi; sức khỏe ổn định", TrangThai = TrangThaiPhieu.HienMauThanhCong, NgayNop = DateTime.Today.AddDays(-8) }
    ];
    private readonly Dictionary<string, (bool TonTai, bool DangMo, int ChiTieu)> _dot = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Giọt Hồng Thanh Niên"] = (true, true, 40),
        ["Sắc Đỏ Tình Người"] = (true, true, 1),
        ["Ngày hội hiến máu"] = (true, true, 50)
    };

    public IReadOnlyList<PhieuDangKy> TatCa()
    {
        lock (_sync) return _phieu.Select(SaoChep).ToList();
    }

    public PhieuDangKy? Tim(int id)
    {
        lock (_sync) { var p = _phieu.FirstOrDefault(x => x.Id == id); return p is null ? null : SaoChep(p); }
    }

    public (bool ThanhCong, string ThongBao, int DaHien, int ChiTieu) ChuyenTrangThai(int id, string trangThaiMoi, string? ghiChu)
    {
        lock (_sync)
        {
            var p = _phieu.FirstOrDefault(x => x.Id == id);
            if (p is null) return (false, "Không tìm thấy phiếu.", 0, 0);
            if (p.TrangThai == TrangThaiPhieu.DaHuy) return (false, "Phiếu đã hủy, không thể xử lý.", 0, 0);
            var hopLe = (p.TrangThai, trangThaiMoi) switch
            {
                (TrangThaiPhieu.ChoXacNhan, TrangThaiPhieu.DaXacNhan or TrangThaiPhieu.TuChoi) => true,
                (TrangThaiPhieu.DaXacNhan, TrangThaiPhieu.ChoKham) => true,
                (TrangThaiPhieu.ChoKham, TrangThaiPhieu.DaKham) => true,
                (TrangThaiPhieu.DaKham, TrangThaiPhieu.HienMauThanhCong or TrangThaiPhieu.KhongDuDieuKien) => true,
                (_, TrangThaiPhieu.DaHuy) when p.TrangThai is not TrangThaiPhieu.HienMauThanhCong and not TrangThaiPhieu.KhongDuDieuKien and not TrangThaiPhieu.TuChoi => true,
                _ => false
            };
            if (!hopLe) return (false, $"Không thể chuyển từ '{p.TrangThai}' sang '{trangThaiMoi}'.", 0, 0);
            if (p.TrangThai == TrangThaiPhieu.ChoXacNhan && trangThaiMoi is TrangThaiPhieu.DaXacNhan or TrangThaiPhieu.TuChoi)
            {
                if (p.NgayXuLy.HasValue || p.GhiChuXacNhan is not null) return (false, "Phiếu đã có kết quả xác nhận trước đó.", 0, 0);
                if (string.IsNullOrWhiteSpace(ghiChu)) return (false, "Vui lòng nhập ghi chú xác nhận hoặc lý do từ chối.", 0, 0);
                if (!_dot.TryGetValue(p.TenDot, out var dot) || !dot.TonTai || !dot.DangMo) return (false, "Đợt hiến máu không tồn tại hoặc đã đóng.", 0, 0);
                if (!p.NguoiHienConHoatDong) return (false, "Người hiến máu không còn hoạt động.", 0, 0);
                p.NgayXuLy = DateTime.Now;
                p.GhiChuXacNhan = ghiChu.Trim();
            }
            p.TrangThai = trangThaiMoi;
            var chiTieu = _dot.GetValueOrDefault(p.TenDot).ChiTieu;
            var daHien = _phieu.Count(x => x.TenDot == p.TenDot && x.TrangThai == TrangThaiPhieu.HienMauThanhCong);
            var canhBao = trangThaiMoi == TrangThaiPhieu.HienMauThanhCong && chiTieu > 0 && daHien >= chiTieu;
            return (true, canhBao ? $"Đã đạt chỉ tiêu {daHien}/{chiTieu}. Nhân viên y tế có thể đóng đợt; các phiếu khác vẫn được giữ nguyên." : "Đã cập nhật trạng thái phiếu.", daHien, chiTieu);
        }
    }

    private static PhieuDangKy SaoChep(PhieuDangKy p) => new()
    {
        Id = p.Id, MaPhieu = p.MaPhieu, HoTenNguoiHien = p.HoTenNguoiHien, NhomMau = p.NhomMau,
        NguoiHienConHoatDong = p.NguoiHienConHoatDong, TenDot = p.TenDot, DonViToChuc = p.DonViToChuc,
        YeuCauDot = p.YeuCauDot, TrangThai = p.TrangThai, NgayNop = p.NgayNop, NgayXuLy = p.NgayXuLy, GhiChuXacNhan = p.GhiChuXacNhan
    };
}

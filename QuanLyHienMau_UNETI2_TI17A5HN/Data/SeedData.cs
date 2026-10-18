using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Data;
public static class SeedData
{
    public static async Task Initialize(ApplicationDbContext db, DateTime now)
    {
        if (await db.TaiKhoans.AnyAsync())
            return;
        var hash = new PasswordHasher<TaiKhoan>();
        TaiKhoan Account(string name, string full, string role, string pwd)
        {
            var a = new TaiKhoan
            {
                TenDangNhap = name,
                HoTen = full,
                VaiTro = role,
                Email = name + "@example.com"
            };
            a.MatKhau = hash.HashPassword(a, pwd);
            db.Add(a);
            return a;
        }

        Account("admin5", "Quản trị viên", "Admin", "Admin@12345");
        Account("yte5", "Bác sĩ Nguyễn An", "NhanVienYTe", "Yte@12345");
        Account("yte6", "Bác sĩ Trần Bình", "NhanVienYTe", "Yte@12345");
        var unitNames = new[]
        {
            "Viện Huyết học – Truyền máu Trung ương",
            "Bệnh viện Bạch Mai",
            "Đại học Quốc gia Hà Nội",
            "Hội Chữ thập đỏ Hà Nội",
            "Trung tâm Máu Quốc gia"
        };
        var donorNames = new[]
        {
            "Nguyễn Hoàng Nam",
            "Trịnh Thùy Linh",
            "Vũ Minh Trí",
            "Lê Thị Mai",
            "Đặng Văn Tuấn",
            "Nguyễn Thảo My",
            "Trần Minh Anh",
            "Phạm Đức Long",
            "Hoàng Thu Hà",
            "Đỗ Quang Huy",
            "Bùi Ngọc Mai",
            "Vũ Anh Tuấn",
            "Đặng Hải Yến",
            "Nguyễn Minh Đức",
            "Trần Ngọc Lan",
            "Lê Quốc Bảo",
            "Phạm Thanh Hương",
            "Hoàng Minh Quân",
            "Đỗ Thu Trang",
            "Bùi Tuấn Kiệt",
            "Vũ Phương Linh",
            "Đặng Nhật Minh",
            "Nguyễn Khánh Ly",
            "Trần Gia Bảo",
            "Lê Thùy Dương",
            "Phạm Hải Nam",
            "Hoàng Ngọc Ánh",
            "Đỗ Minh Châu",
            "Bùi Quốc Anh",
            "Vũ Thanh Tâm"
        };
        var campaignNames = new[]
        {
            "Giọt Hồng Thanh Niên",
            "Hành Trình Đỏ",
            "Sắc Đỏ Tình Người",
            "Chủ Nhật Đỏ",
            "Trái Tim Nhân Ái",
            "Kết Nối Dòng Máu Việt",
            "Giọt Máu Nghĩa Tình",
            "Trao Hy Vọng",
            "Sẻ Chia Sự Sống",
            "Giọt Hồng Sinh Viên",
            "Ngày Hội Tình Nguyện",
            "Xuân Hồng",
            "Mùa Hè Nhân Ái",
            "Một Giọt Máu Một Tấm Lòng",
            "Cầu Nối Yêu Thương"
        };
        var units = Enumerable.Range(1, 5).Select(i => new DonViToChuc { TenDonViToChuc = unitNames[i - 1], EmailLienHe = $"donvi{i}@example.com" }).ToList();
        db.AddRange(units);
        var dots = Enumerable.Range(1, 15).Select(i => new DotHienMau { TenDot = campaignNames[i - 1], DonViToChuc = units[(i - 1) % 5], NgayBatDauDangKy = now.Date.AddDays(-60), HanDangKy = now.Date.AddDays(i % 3 == 0 ? -1 : 60), TrangThai = i % 4 == 0 ? "Đã đóng" : "Đang mở đăng ký", ChiTieuDonViMau = i == 1 ? 1 : 20 }).ToList();
        db.AddRange(dots);
        var people = Enumerable.Range(1, 30).Select(i => new NguoiHienMau { TaiKhoan = Account($"nguoi{i:00}", donorNames[i - 1], "NguoiHienMau", "Nguoi@12345"), HoTen = donorNames[i - 1], NgaySinh = new DateTime(1998, 1, 1).AddDays(i), DiaChi = "Hà Nội", Email = $"nguoi{i:00}@example.com", SoDienThoai = $"090000{i:0000}", NhomMau = new[] { "A", "B", "AB", "O" }[i % 4], GioiTinh = i % 2 == 0 ? "Nữ" : "Nam" }).ToList();
        db.AddRange(people);
        var tickets = new List<PhieuDangKyHienMau>();
        for (var i = 1; i <= 50; i++)
        {
            var state = i <= 10 ? TrangThaiPhieu.DaXacNhan : i <= 14 ? TrangThaiPhieu.ChoXacNhan : i <= 18 ? TrangThaiPhieu.TuChoi : i <= 20 ? TrangThaiPhieu.DaHuy : i <= 28 ? TrangThaiPhieu.ChoKham : i <= 32 ? TrangThaiPhieu.DaKham : i <= 38 ? TrangThaiPhieu.ThanhCong : i <= 40 ? TrangThaiPhieu.KhongDat : TrangThaiPhieu.DaXacNhan;
            tickets.Add(new PhieuDangKyHienMau { NguoiHien = people[(i - 1) % 30], Dot = dots[(i - 1) % 15], NgayNop = now.Date.AddMonths(-(i % 9)).AddDays(-1), TrangThai = state, NgayXuLy = state == TrangThaiPhieu.ChoXacNhan ? null : now.AddDays(-2), LyDoHienMau = "Tình nguyện hiến máu cứu người" });
        }

        db.AddRange(tickets);
        for (int i = 21; i <= 40; i++)
        {
            var start = i <= 25 ? now.Date.AddDays(i - 19).AddHours(9) : now.Date.AddDays(-(i - 25)).AddHours(9);
            db.Add(new LichKhamSangLoc { Phieu = tickets[i - 1], ThoiGianBatDau = start, ThoiGianKetThuc = start.AddHours(1), BacSiSangLoc = i % 2 == 0 ? "Bác sĩ Nguyễn An" : "Bác sĩ Trần Bình", BacSiChuanHoa = (i % 2 == 0 ? "Bác sĩ Nguyễn An" : "Bác sĩ Trần Bình").ToUpperInvariant(), DiaDiemHoacLienKet = "Phòng khám sàng lọc A", TrangThai = i <= 28 ? TrangThaiLich.DaLenLich : TrangThaiLich.HoanThanh });
            if (i >= 33)
                db.Add(new KetQuaHienMau { Phieu = tickets[i - 1], DiemDanhGia = i <= 38 ? 90 : 40, NhanXet = i <= 38 ? "Đã xác nhận hiến máu thành công." : "Chưa đủ điều kiện hiến máu ở đợt này.", KetQua = tickets[i - 1].TrangThai, NgayCapNhat = start.AddHours(2) });
        }

        await db.SaveChangesAsync();
    }
}

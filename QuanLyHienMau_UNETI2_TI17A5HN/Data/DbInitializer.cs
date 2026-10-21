using Microsoft.AspNetCore.Identity;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            var passwordHasher = new PasswordHasher<TaiKhoan>();

            // 1. Seed TaiKhoan & NguoiHienMau nếu chưa có
            if (!context.TaiKhoans.Any())
            {
                var adminAccount = new TaiKhoan
                {
                    TenDangNhap = "admin",
                    HoTen = "Quản Trị Viên Hệ Thống",
                    Email = "admin@uneti.edu.vn",
                    VaiTro = "Admin",
                    TrangThai = "Hoạt động"
                };
                adminAccount.MatKhau = passwordHasher.HashPassword(adminAccount, "Admin@123");

                var staffAccount = new TaiKhoan
                {
                    TenDangNhap = "bacsi",
                    HoTen = "BS. CKI Trần Mai Linh",
                    Email = "mailinh.tran@uneti.edu.vn",
                    VaiTro = "Medical Staff",
                    TrangThai = "Hoạt động"
                };
                staffAccount.MatKhau = passwordHasher.HashPassword(staffAccount, "BacSi@123");

                var donorAccount = new TaiKhoan
                {
                    TenDangNhap = "donor",
                    HoTen = "Nguyễn Văn An",
                    Email = "nguyenvanan@gmail.com",
                    VaiTro = "Donor",
                    TrangThai = "Hoạt động"
                };
                donorAccount.MatKhau = passwordHasher.HashPassword(donorAccount, "Donor@123");

                context.TaiKhoans.AddRange(adminAccount, staffAccount, donorAccount);
                context.SaveChanges();

                // Tạo hồ sơ người hiến máu cho donor mẫu
                var donorProfile = new NguoiHienMau
                {
                    MaTaiKhoan = donorAccount.MaTaiKhoan,
                    HoTen = donorAccount.HoTen,
                    NgaySinh = new DateTime(2003, 5, 15),
                    GioiTinh = "Nam",
                    SoDienThoai = "0912345678",
                    Email = donorAccount.Email,
                    DiaChi = "218 Lĩnh Nam, Hoàng Mai, Hà Nội",
                    NhomMau = "O+",
                    NgheNghiep = "Sinh viên UNETI",
                    CanNang = 62.0,
                    TrangThai = "Hoạt động"
                };
                context.NguoiHienMaus.Add(donorProfile);
                context.SaveChanges();
            }

            // 2. Seed DonViToChuc nếu chưa có
            if (!context.DonViToChucs.Any())
            {
                var donViList = new List<DonViToChuc>
                {
                    new DonViToChuc
                    {
                        TenDonViToChuc = "Đoàn Thanh Niên & Hội Sinh Viên ĐH UNETI",
                        MoTa = "Trường Đại học Kinh tế - Kỹ thuật Công nghiệp (Cơ sở Hà Nội & Nam Định)",
                        EmailLienHe = "doanthanhnien@uneti.edu.vn",
                        TrangThai = "Hoạt động"
                    },
                    new DonViToChuc
                    {
                        TenDonViToChuc = "Viện Huyết Học - Truyền Máu Trung Ương",
                        MoTa = "Ngân hàng máu quốc gia & Trung tâm điều phối máu khu vực phía Bắc",
                        EmailLienHe = "vienhuyethoc@nihbt.org.vn",
                        TrangThai = "Hoạt động"
                    },
                    new DonViToChuc
                    {
                        TenDonViToChuc = "Hội Chữ Thập Đỏ TP. Hà Nội",
                        MoTa = "Ban Chỉ đạo Vận động Hiến máu tình nguyện thành phố Hà Nội",
                        EmailLienHe = "redcross.hanoi@redcross.org.vn",
                        TrangThai = "Hoạt động"
                    },
                    new DonViToChuc
                    {
                        TenDonViToChuc = "Bệnh Viện Hữu Nghị Việt Đức",
                        MoTa = "Trung tâm ghép tạng và cấp cứu ngoại khoa đầu ngành",
                        EmailLienHe = "vietduc.hospital@bvvd.vn",
                        TrangThai = "Hoạt động"
                    }
                };

                context.DonViToChucs.AddRange(donViList);
                context.SaveChanges();

                // 3. Seed DotHienMau
                var unetiId = donViList[0].MaDonViToChuc;
                var nihbtId = donViList[1].MaDonViToChuc;
                var redcrossId = donViList[2].MaDonViToChuc;
                var vietducId = donViList[3].MaDonViToChuc;

                var dotHienMauList = new List<DotHienMau>
                {
                    new DotHienMau
                    {
                        TenDot = "Giọt Hồng UNETI – Trao Đời Sự Sống 2026",
                        MaDonViToChuc = unetiId,
                        ChiTieuDonViMau = 200,
                        NhomMauUuTien = "O, A, B",
                        CanNangToiThieu = 45.0,
                        NgayBatDauDangKy = DateTime.Now.AddDays(-2),
                        HanDangKy = DateTime.Now.AddDays(15),
                        MoTaDot = "Chiến dịch hiến máu tình nguyện thường niên do Đoàn Thanh niên UNETI phối hợp cùng Viện Huyết học TW tổ chức tại Hội trường Nhà C, 218 Lĩnh Nam, Hà Nội.",
                        DieuKienNguoiHien = "Tình nguyện viên từ 18 - 60 tuổi, cân nặng ≥ 45kg, không mắc các bệnh truyền nhiễm (HIV, HBV, HCV, Giang mai, Sốt rét), mang theo CCCD gắn chip khi tham gia.",
                        TrangThai = "Đang mở đăng ký"
                    },
                    new DotHienMau
                    {
                        TenDot = "Trái Tim Đỏ – Chung Tay Vì Người Bệnh",
                        MaDonViToChuc = nihbtId,
                        ChiTieuDonViMau = 350,
                        NhomMauUuTien = "O- (Khẩn cấp), O+",
                        CanNangToiThieu = 45.0,
                        NgayBatDauDangKy = DateTime.Now.AddDays(-5),
                        HanDangKy = DateTime.Now.AddDays(20),
                        MoTaDot = "Đợt tiếp nhận máu lưu động đặc biệt ưu tiên bổ sung nhóm máu hiếm O- cho kho máu cấp cứu trước thềm kỳ nghỉ lễ.",
                        DieuKienNguoiHien = "Người hiến máu khỏe mạnh, khoảng cách giữa 2 lần hiến máu toàn phần tối thiểu 84 ngày, huyết sắc tố đạt chuẩn ≥ 120 g/L.",
                        TrangThai = "Đang mở đăng ký"
                    },
                    new DotHienMau
                    {
                        TenDot = "Ngày Hội Giọt Máu Hồng Cơ Sở Nam Định",
                        MaDonViToChuc = unetiId,
                        ChiTieuDonViMau = 150,
                        NhomMauUuTien = "B, AB, O",
                        CanNangToiThieu = 45.0,
                        NgayBatDauDangKy = DateTime.Now.AddDays(5),
                        HanDangKy = DateTime.Now.AddDays(25),
                        MoTaDot = "Chiến dịch dành cho cán bộ, giảng viên và sinh viên cơ sở Nam Định tại 353 Trần Hưng Đạo, TP. Nam Định.",
                        DieuKienNguoiHien = "Sinh viên và giảng viên từ 45kg trở lên, đêm hôm trước không thức khuya, ăn nhẹ trước khi hiến máu (không uống sữa/rượu bia).",
                        TrangThai = "Chưa mở"
                    },
                    new DotHienMau
                    {
                        TenDot = "Hành Trình Đỏ – Kết Nối Dòng Máu Việt",
                        MaDonViToChuc = redcrossId,
                        ChiTieuDonViMau = 500,
                        NhomMauUuTien = "O, A, B, AB",
                        CanNangToiThieu = 45.0,
                        NgayBatDauDangKy = DateTime.Now.AddDays(-30),
                        HanDangKy = DateTime.Now.AddDays(-5),
                        MoTaDot = "Chiến dịch hiến máu lớn quy mô toàn thành phố phối hợp cùng Hội Chữ Thập Đỏ.",
                        DieuKienNguoiHien = "Công dân từ 18-60 tuổi, mang theo CCCD và ứng dụng VNeID.",
                        TrangThai = "Đã đóng"
                    },
                    new DotHienMau
                    {
                        TenDot = "Blouse Trắng – Trái Tim Hồng Việt Đức",
                        MaDonViToChuc = vietducId,
                        ChiTieuDonViMau = 250,
                        NhomMauUuTien = "O, A (Khẩn cấp)",
                        CanNangToiThieu = 45.0,
                        NgayBatDauDangKy = DateTime.Now.AddDays(-1),
                        HanDangKy = DateTime.Now.AddDays(10),
                        MoTaDot = "Phục vụ cấp cứu ngoại khoa chấn thương và các ca phẫu thuật ghép tạng tại BV Hữu Nghị Việt Đức.",
                        DieuKienNguoiHien = "Người tham gia không có tiền sử bệnh lý tim mạch, huyết áp ổn định.",
                        TrangThai = "Đang mở đăng ký"
                    }
                };

                context.DotHienMaus.AddRange(dotHienMauList);
                context.SaveChanges();
            }
        }
    }
}

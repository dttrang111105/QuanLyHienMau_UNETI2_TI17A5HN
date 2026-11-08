using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.Migrate();

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

        /// <summary>
        /// Tạo dữ liệu thử bộ lọc danh sách phiếu. Phương thức chỉ được gọi trong Development
        /// và có thể chạy nhiều lần mà không tạo trùng hồ sơ hoặc phiếu mẫu.
        /// </summary>
        public static void SeedTestData(ApplicationDbContext context)
        {
            var samples = new[]
            {
                new { HoTen = "Nguyễn Minh Anh", Email = "demo.minhanh@example.test", Phone = "0981000001", Blood = "O+", Status = "Chờ xác nhận", DayOffset = 0 },
                new { HoTen = "Trần Quốc Bảo", Email = "demo.quocbao@example.test", Phone = "0981000002", Blood = "A+", Status = "Chờ xác nhận", DayOffset = 1 },
                new { HoTen = "Lê Thu Hà", Email = "demo.thuha@example.test", Phone = "0981000003", Blood = "B+", Status = "Đã xác nhận", DayOffset = 2 },
                new { HoTen = "Phạm Đức Huy", Email = "demo.duchuy@example.test", Phone = "0981000004", Blood = "AB+", Status = "Chờ khám sàng lọc", DayOffset = 3 },
                new { HoTen = "Vũ Ngọc Lan", Email = "demo.ngoclan@example.test", Phone = "0981000005", Blood = "O-", Status = "Từ chối", DayOffset = 5 },
                new { HoTen = "Đỗ Hải Nam", Email = "demo.hainam@example.test", Phone = "0981000006", Blood = "A-", Status = "Chờ xác nhận", DayOffset = 7 },
                new { HoTen = "Bùi Khánh Linh", Email = "demo.khanhlinh@example.test", Phone = "0981000007", Blood = "B-", Status = "Đã xác nhận", DayOffset = 10 },
                new { HoTen = "Hoàng Gia Phúc", Email = "demo.giaphuc@example.test", Phone = "0981000008", Blood = "O+", Status = "Chờ xác nhận", DayOffset = 14 }
            };

            var sampleEmails = samples.Select(s => s.Email).ToArray();
            var existingProfiles = context.NguoiHienMaus
                .Where(n => n.Email != null && sampleEmails.Contains(n.Email))
                .ToList();
            var profilesByEmail = existingProfiles.ToDictionary(n => n.Email!, n => n);

            foreach (var sample in samples)
            {
                if (profilesByEmail.ContainsKey(sample.Email)) continue;

                var profile = new NguoiHienMau
                {
                    HoTen = sample.HoTen,
                    Email = sample.Email,
                    SoDienThoai = sample.Phone,
                    NgaySinh = new DateTime(1995 + sample.DayOffset % 10, 3 + sample.DayOffset % 8, 5 + sample.DayOffset % 20),
                    GioiTinh = sample.DayOffset % 2 == 0 ? "Nữ" : "Nam",
                    DiaChi = "Hà Nội",
                    NhomMau = sample.Blood,
                    NgheNghiep = "Nhân viên văn phòng",
                    CanNang = 50 + sample.DayOffset % 20,
                    TrangThai = "Hoạt động"
                };
                context.NguoiHienMaus.Add(profile);
                profilesByEmail.Add(sample.Email, profile);
            }

            context.SaveChanges();

            var campaigns = context.DotHienMaus
                .OrderBy(d => d.MaDot)
                .ToList();

            if (campaigns.Count == 0)
            {
                var organization = context.DonViToChucs.FirstOrDefault();
                if (organization == null)
                {
                    organization = new DonViToChuc
                    {
                        TenDonViToChuc = "Đơn vị mẫu kiểm thử",
                        EmailLienHe = "demo.organization@example.test",
                        MoTa = "Đơn vị mẫu phục vụ kiểm thử danh sách phiếu.",
                        TrangThai = "Hoạt động"
                    };
                    context.DonViToChucs.Add(organization);
                    context.SaveChanges();
                }

                campaigns.Add(new DotHienMau
                {
                    TenDot = "Đợt hiến máu mẫu kiểm thử",
                    MaDonViToChuc = organization.MaDonViToChuc,
                    ChiTieuDonViMau = 100,
                    CanNangToiThieu = 45,
                    NgayBatDauDangKy = DateTime.Today.AddDays(-30),
                    HanDangKy = DateTime.Today.AddDays(30),
                    TrangThai = "Đang mở đăng ký"
                });
                context.DotHienMaus.Add(campaigns[0]);
                context.SaveChanges();
            }

            var sampleDonorIds = profilesByEmail.Values.Select(n => n.MaNguoiHien).ToArray();
            var donorsWithApplications = context.PhieuDangKyHienMaus
                .Where(p => sampleDonorIds.Contains(p.MaNguoiHien))
                .Select(p => p.MaNguoiHien)
                .ToHashSet();

            var newApplications = samples
                .Select((sample, index) => new { Sample = sample, Index = index, Donor = profilesByEmail[sample.Email] })
                .Where(item => !donorsWithApplications.Contains(item.Donor.MaNguoiHien))
                .Select(item => new PhieuDangKyHienMau
                {
                    MaNguoiHien = item.Donor.MaNguoiHien,
                    MaDot = campaigns[item.Index % campaigns.Count].MaDot,
                    NgayNop = DateTime.Today.AddDays(-item.Sample.DayOffset).AddHours(8 + item.Index),
                    TrangThai = item.Sample.Status,
                    LyDoHienMau = "Đăng ký hiến máu tình nguyện.",
                    GhiChu = "Dữ liệu mẫu để kiểm thử tìm kiếm, lọc và sắp xếp phiếu."
                })
                .ToList();

            if (newApplications.Count > 0)
            {
                context.PhieuDangKyHienMaus.AddRange(newApplications);
                context.SaveChanges();
            }
        }
    }
}

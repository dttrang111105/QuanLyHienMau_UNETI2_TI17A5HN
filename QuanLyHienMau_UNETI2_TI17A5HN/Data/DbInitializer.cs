// Họ và tên: Nguyễn Tiến Nam
// Mã sinh viên: 23103100275
// Nội dung thực hiện: Module 1 - Khởi tạo dữ liệu mẫu (Seed Data) toàn hệ thống

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            try
            {
                context.Database.Migrate();
            }
            catch
            {
                // Trường hợp các bảng đã tồn tại trong CSDL trước khi ghi nhận lịch sử Migration:
                // vẫn tiếp tục nạp dữ liệu mẫu bình thường.
            }

            var passwordHasher = new PasswordHasher<TaiKhoan>();

            // 1. Seed TaiKhoan & NguoiHienMau mặc định nếu chưa có
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

            SeedTestData(context);
        }

        /// <summary>
        /// Tạo dữ liệu mẫu phong phú, đa dạng cho toàn bộ 7 bảng như một hệ thống đang vận hành thực tế.
        /// Có thể chạy nhiều lần mà không tạo trùng lặp.
        /// </summary>
        public static void SeedTestData(ApplicationDbContext context)
        {
            var passwordHasher = new PasswordHasher<TaiKhoan>();

            // 1. Bổ sung thêm các Đơn vị tổ chức để hệ thống có 6 đơn vị đa dạng
            var extraOrgs = new[]
            {
                new { Ten = "Bệnh Viện Quân Y 103", MoTa = "Trung tâm huyết học truyền máu và cấp cứu quân dân y khu vực Hà Nội", Email = "huyethoc@bv103.vn", Status = "Hoạt động" },
                new { Ten = "Bệnh Viện Bạch Mai - Trung Tâm Huyết Học", MoTa = "Đơn vị tiếp nhận và điều chế chế phẩm máu tuyến cuối khu vực miền Bắc", Email = "huyethoc.bachmai@bachmai.gov.vn", Status = "Hoạt động" }
            };
            foreach (var o in extraOrgs)
            {
                if (!context.DonViToChucs.Any(d => d.TenDonViToChuc == o.Ten))
                {
                    context.DonViToChucs.Add(new DonViToChuc
                    {
                        TenDonViToChuc = o.Ten,
                        MoTa = o.MoTa,
                        EmailLienHe = o.Email,
                        TrangThai = o.Status
                    });
                }
            }
            context.SaveChanges();

            // 2. Bổ sung thêm các Đợt hiến máu (bao gồm cả đợt đạt chỉ tiêu và các đợt qua nhiều tháng)
            var firstOrg = context.DonViToChucs.OrderBy(d => d.MaDonViToChuc).First();
            var allOrgs = context.DonViToChucs.OrderBy(d => d.MaDonViToChuc).ToList();
            var extraDots = new[]
            {
                new
                {
                    TenDot = "Ngày Hội Xuân Hồng UNETI – Sẻ Chia Giọt Máu Đào",
                    MaDonVi = allOrgs[0].MaDonViToChuc,
                    ChiTieu = 6, // Chỉ tiêu 6 để minh họa tính năng Cảnh báo đạt chỉ tiêu
                    NhomMau = "O, A, B, AB",
                    CanNang = 45.0,
                    BatDau = DateTime.Today.AddDays(-120),
                    KetThuc = DateTime.Today.AddDays(-95),
                    MoTa = "Chương trình hiến máu đầu học kỳ thu hút đông đảo sinh viên khoa CNTT, Điện - Điện tử, Cơ khí và Kinh tế.",
                    DieuKien = "Sinh viên, giảng viên từ 18-60 tuổi, cân nặng ≥ 45kg, mang theo thẻ sinh viên hoặc CCCD.",
                    TrangThai = "Đã đóng"
                },
                new
                {
                    TenDot = "Mùa Hè Tình Nguyện – Giọt Máu Cứu Người 2026",
                    MaDonVi = allOrgs[Math.Min(4, allOrgs.Count - 1)].MaDonViToChuc,
                    ChiTieu = 180,
                    NhomMau = "O, B, AB",
                    CanNang = 48.0,
                    BatDau = DateTime.Today.AddDays(-75),
                    KetThuc = DateTime.Today.AddDays(-50),
                    MoTa = "Chiến dịch bổ sung nguồn máu dự trữ mùa hè phối hợp cùng Bệnh viện Quân Y 103.",
                    DieuKien = "Công dân khỏe mạnh, cân nặng từ 48kg trở lên, không sử dụng kháng sinh trong 7 ngày gần nhất.",
                    TrangThai = "Đã đóng"
                },
                new
                {
                    TenDot = "Chủ Nhật Đỏ – Hiến Máu Cứu Người Tại Bạch Mai",
                    MaDonVi = allOrgs[Math.Min(5, allOrgs.Count - 1)].MaDonViToChuc,
                    ChiTieu = 120,
                    NhomMau = "A, O, AB",
                    CanNang = 45.0,
                    BatDau = DateTime.Today.AddDays(-10),
                    KetThuc = DateTime.Today.AddDays(18),
                    MoTa = "Điểm hiến máu cố định cuối tuần hỗ trợ bệnh nhân điều trị nội trú và cấp cứu.",
                    DieuKien = "Tình nguyện viên 18-60 tuổi, huyết áp ổn định, ngủ đủ giấc trước ngày hiến máu.",
                    TrangThai = "Đang mở đăng ký"
                }
            };
            foreach (var d in extraDots)
            {
                if (!context.DotHienMaus.Any(x => x.TenDot == d.TenDot))
                {
                    context.DotHienMaus.Add(new DotHienMau
                    {
                        TenDot = d.TenDot,
                        MaDonViToChuc = d.MaDonVi,
                        ChiTieuDonViMau = d.ChiTieu,
                        NhomMauUuTien = d.NhomMau,
                        CanNangToiThieu = d.CanNang,
                        NgayBatDauDangKy = d.BatDau,
                        HanDangKy = d.KetThuc,
                        MoTaDot = d.MoTa,
                        DieuKienNguoiHien = d.DieuKien,
                        TrangThai = d.TrangThai
                    });
                }
            }
            context.SaveChanges();

            // 3. Bổ sung đội ngũ Nhân viên y tế & Quản trị viên
            var staffAccounts = new[]
            {
                new { Username = "admin_uneti", HoTen = "ThS. Nguyễn Tiến Nam", Email = "tiennam.admin@uneti.edu.vn", Role = "Admin", Status = "Hoạt động" },
                new { Username = "bacsi2", HoTen = "BS. Nguyễn Hoàng Nam", Email = "hoangnam.bs@uneti.edu.vn", Role = "Medical Staff", Status = "Hoạt động" },
                new { Username = "bacsi_huong", HoTen = "BS. CKI Lê Thu Hương", Email = "thuhuong.bs@nihbt.org.vn", Role = "Medical Staff", Status = "Hoạt động" },
                new { Username = "dieuduong_ha", HoTen = "ĐD. Phạm Ngân Hà", Email = "nganha.dd@bvvd.vn", Role = "Medical Staff", Status = "Hoạt động" }
            };
            foreach (var s in staffAccounts)
            {
                if (!context.TaiKhoans.Any(t => t.TenDangNhap == s.Username))
                {
                    var acc = new TaiKhoan
                    {
                        TenDangNhap = s.Username,
                        HoTen = s.HoTen,
                        Email = s.Email,
                        VaiTro = s.Role,
                        TrangThai = s.Status
                    };
                    acc.MatKhau = passwordHasher.HashPassword(acc, s.Role == "Admin" ? "Admin@123" : "BacSi@123");
                    context.TaiKhoans.Add(acc);
                }
            }
            context.SaveChanges();

            // 4. Danh sách 22 Người hiến máu đa dạng nhóm máu (bao gồm cả nhóm máu hiếm Rh-), độ tuổi, nghề nghiệp, địa chỉ
            var samples = new[]
            {
                new { Username = "donor_minhanh", HoTen = "Nguyễn Minh Anh", Email = "demo.minhanh@example.test", Phone = "0981000001", Blood = "O+", Weight = 54.0, Gender = "Nữ", Address = "218 Lĩnh Nam, Hoàng Mai, Hà Nội", Job = "Sinh viên UNETI", AccountStatus = "Hoạt động", DayOffset = 1 },
                new { Username = "donor_quocbao", HoTen = "Trần Quốc Bảo", Email = "demo.quocbao@example.test", Phone = "0981000002", Blood = "A+", Weight = 68.0, Gender = "Nam", Address = "454 Minh Khai, Hai Bà Trưng, Hà Nội", Job = "Kỹ sư phần mềm", AccountStatus = "Hoạt động", DayOffset = 2 },
                new { Username = "donor_thuha", HoTen = "Lê Thu Hà", Email = "demo.thuha@example.test", Phone = "0981000003", Blood = "B+", Weight = 51.0, Gender = "Nữ", Address = "120 Trương Định, Hoàng Mai, Hà Nội", Job = "Giảng viên đại học", AccountStatus = "Hoạt động", DayOffset = 3 },
                new { Username = "donor_duchuy", HoTen = "Phạm Đức Huy", Email = "demo.duchuy@example.test", Phone = "0981000004", Blood = "AB+", Weight = 70.0, Gender = "Nam", Address = "88 Giải Phóng, Đống Đa, Hà Nội", Job = "Sinh viên UNETI", AccountStatus = "Hoạt động", DayOffset = 4 },
                new { Username = "donor_ngoclan", HoTen = "Vũ Ngọc Lan", Email = "demo.ngoclan@example.test", Phone = "0981000005", Blood = "O-", Weight = 50.0, Gender = "Nữ", Address = "35 Trần Đại Nghĩa, Hai Bà Trưng, Hà Nội", Job = "Kế toán viên", AccountStatus = "Hoạt động", DayOffset = 5 },
                new { Username = "donor_hainam", HoTen = "Đỗ Hải Nam", Email = "demo.hainam@example.test", Phone = "0981000006", Blood = "A-", Weight = 66.0, Gender = "Nam", Address = "15 Tam Trinh, Hoàng Mai, Hà Nội", Job = "Chuyên viên ngân hàng", AccountStatus = "Hoạt động", DayOffset = 6 },
                new { Username = "donor_khanhlinh", HoTen = "Bùi Khánh Linh", Email = "demo.khanhlinh@example.test", Phone = "0981000007", Blood = "B-", Weight = 52.5, Gender = "Nữ", Address = "296 Lĩnh Nam, Hoàng Mai, Hà Nội", Job = "Sinh viên UNETI", AccountStatus = "Hoạt động", DayOffset = 8 },
                new { Username = "donor_giaphuc", HoTen = "Hoàng Gia Phúc", Email = "demo.giaphuc@example.test", Phone = "0981000008", Blood = "O+", Weight = 72.0, Gender = "Nam", Address = "102 Thái Hà, Đống Đa, Hà Nội", Job = "Kiến trúc sư", AccountStatus = "Hoạt động", DayOffset = 10 },
                new { Username = "donor_thanhmai", HoTen = "Đặng Thanh Mai", Email = "demo.thanhmai@example.test", Phone = "0981000009", Blood = "AB-", Weight = 53.0, Gender = "Nữ", Address = "62 Nguyễn Chí Thanh, Đống Đa, Hà Nội", Job = "Dược sĩ lâm sàng", AccountStatus = "Hoạt động", DayOffset = 12 },
                new { Username = "donor_quanghuy", HoTen = "Ngô Quang Huy", Email = "demo.quanghuy@example.test", Phone = "0981000010", Blood = "O+", Weight = 65.0, Gender = "Nam", Address = "353 Trần Hưng Đạo, TP. Nam Định", Job = "Sinh viên UNETI", AccountStatus = "Hoạt động", DayOffset = 15 },
                new { Username = "donor_thuyduong", HoTen = "Phan Thùy Dương", Email = "demo.thuyduong@example.test", Phone = "0981000011", Blood = "A+", Weight = 55.0, Gender = "Nữ", Address = "19 Bà Triệu, Hoàn Kiếm, Hà Nội", Job = "Nhân viên truyền thông", AccountStatus = "Hoạt động", DayOffset = 18 },
                new { Username = "donor_minhkhoi", HoTen = "Lý Minh Khôi", Email = "demo.minhkhoi@example.test", Phone = "0981000012", Blood = "B+", Weight = 63.0, Gender = "Nam", Address = "75 Cầu Giấy, Hà Nội", Job = "Lập trình viên .NET", AccountStatus = "Hoạt động", DayOffset = 20 },
                new { Username = "donor_baotram", HoTen = "Trịnh Bảo Trâm", Email = "demo.baotram@example.test", Phone = "0981000013", Blood = "O-", Weight = 51.5, Gender = "Nữ", Address = "144 Xuân Thủy, Cầu Giấy, Hà Nội", Job = "Giáo viên THPT", AccountStatus = "Hoạt động", DayOffset = 22 },
                new { Username = "donor_tuananh", HoTen = "Võ Tuấn Anh", Email = "demo.tuananh@example.test", Phone = "0981000014", Blood = "A+", Weight = 69.0, Gender = "Nam", Address = "250 Minh Khai, Hai Bà Trưng, Hà Nội", Job = "Kỹ sư điện tự động hóa", AccountStatus = "Hoạt động", DayOffset = 25 },
                new { Username = "donor_phuongthao", HoTen = "Nguyễn Thị Phương Thảo", Email = "demo.phuongthao@example.test", Phone = "0981000015", Blood = "O+", Weight = 50.5, Gender = "Nữ", Address = "218 Lĩnh Nam, Hoàng Mai, Hà Nội", Job = "Sinh viên UNETI", AccountStatus = "Hoạt động", DayOffset = 28 },
                new { Username = "donor_thutrang", HoTen = "Đàm Thu Trang", Email = "demo.thutrang@example.test", Phone = "0981000016", Blood = "B+", Weight = 52.0, Gender = "Nữ", Address = "189 Hoàng Hoa Thám, Ba Đình, Hà Nội", Job = "Sinh viên UNETI", AccountStatus = "Hoạt động", DayOffset = 30 },
                new { Username = "donor_vanthai", HoTen = "Vương Văn Thái", Email = "demo.vanthai@example.test", Phone = "0981000017", Blood = "A+", Weight = 67.0, Gender = "Nam", Address = "108 Lò Đúc, Hai Bà Trưng, Hà Nội", Job = "Sinh viên UNETI", AccountStatus = "Hoạt động", DayOffset = 33 },
                new { Username = "donor_quangminh", HoTen = "Trần Đỗ Quang Minh", Email = "demo.quangminh@example.test", Phone = "0981000018", Blood = "AB+", Weight = 64.0, Gender = "Nam", Address = "55 Đại Cồ Việt, Hai Bà Trưng, Hà Nội", Job = "Sinh viên UNETI", AccountStatus = "Hoạt động", DayOffset = 35 },
                new { Username = "donor_hoanglong", HoTen = "Lương Hoàng Long", Email = "demo.hoanglong@example.test", Phone = "0981000019", Blood = "O-", Weight = 71.0, Gender = "Nam", Address = "12 Kim Mã, Ba Đình, Hà Nội", Job = "Chuyên viên phân tích dữ liệu", AccountStatus = "Hoạt động", DayOffset = 40 },
                new { Username = "donor_myduyen", HoTen = "Hồ Mỹ Duyên", Email = "demo.myduyen@example.test", Phone = "0981000020", Blood = "A-", Weight = 49.5, Gender = "Nữ", Address = "81 Nguyễn Trãi, Thanh Xuân, Hà Nội", Job = "Nhân viên thiết kế đồ họa", AccountStatus = "Hoạt động", DayOffset = 42 },
                new { Username = "donor_ducmanh", HoTen = "Tạ Đức Mạnh", Email = "demo.ducmanh@example.test", Phone = "0981000021", Blood = "B-", Weight = 74.0, Gender = "Nam", Address = "301 Giải Phóng, Thanh Xuân, Hà Nội", Job = "Quản lý dự án CNTT", AccountStatus = "Hoạt động", DayOffset = 45 },
                new { Username = "donor_kimngan", HoTen = "Chu Kim Ngân", Email = "demo.kimngan@example.test", Phone = "0981000022", Blood = "O+", Weight = 48.0, Gender = "Nữ", Address = "90 Láng Hạ, Đống Đa, Hà Nội", Job = "Nhân viên hành chính", AccountStatus = "Bị khóa", DayOffset = 50 }
            };

            var sampleEmails = samples.Select(s => s.Email).ToArray();
            var existingProfiles = context.NguoiHienMaus
                .Where(n => n.Email != null && sampleEmails.Contains(n.Email))
                .ToList();
            var profilesByEmail = existingProfiles.ToDictionary(n => n.Email!, n => n);

            var existingAccounts = context.TaiKhoans
                .Where(t => t.Email != null && sampleEmails.Contains(t.Email))
                .ToDictionary(t => t.Email!, t => t);

            foreach (var sample in samples)
            {
                if (!existingAccounts.TryGetValue(sample.Email, out var account))
                {
                    account = context.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == sample.Username);
                    if (account == null)
                    {
                        account = new TaiKhoan
                        {
                            TenDangNhap = sample.Username,
                            HoTen = sample.HoTen,
                            Email = sample.Email,
                            VaiTro = "Donor",
                            TrangThai = sample.AccountStatus
                        };
                        account.MatKhau = passwordHasher.HashPassword(account, "Donor@123");
                        context.TaiKhoans.Add(account);
                        context.SaveChanges();
                    }
                    existingAccounts[sample.Email] = account;
                }

                if (profilesByEmail.TryGetValue(sample.Email, out var existingProfile))
                {
                    if (existingProfile.MaTaiKhoan == null && account != null)
                    {
                        existingProfile.MaTaiKhoan = account.MaTaiKhoan;
                    }
                    existingProfile.DiaChi = sample.Address;
                    existingProfile.NgheNghiep = sample.Job;
                    existingProfile.CanNang = sample.Weight;
                    existingProfile.TrangThai = sample.AccountStatus;
                    continue;
                }

                var profile = new NguoiHienMau
                {
                    MaTaiKhoan = account?.MaTaiKhoan,
                    HoTen = sample.HoTen,
                    Email = sample.Email,
                    SoDienThoai = sample.Phone,
                    NgaySinh = new DateTime(1996 + (sample.DayOffset % 9), 1 + (sample.DayOffset % 12), 1 + (sample.DayOffset % 27)),
                    GioiTinh = sample.Gender,
                    DiaChi = sample.Address,
                    NhomMau = sample.Blood,
                    NgheNghiep = sample.Job,
                    CanNang = sample.Weight,
                    TrangThai = sample.AccountStatus
                };
                context.NguoiHienMaus.Add(profile);
                profilesByEmail.Add(sample.Email, profile);
            }
            context.SaveChanges();

            // 5. Tạo danh sách Phiếu đăng ký hiến máu trải dài qua 6 tháng (để biểu đồ theo tháng, theo đợt, theo trạng thái hiển thị đầy đặn)
            var campaigns = context.DotHienMaus.OrderBy(d => d.MaDot).ToList();
            if (campaigns.Count == 0) return;

            var xuanHongDot = campaigns.FirstOrDefault(d => d.TenDot.Contains("Xuân Hồng")) ?? campaigns[0];
            var muaHeDot = campaigns.FirstOrDefault(d => d.TenDot.Contains("Mùa Hè")) ?? campaigns[Math.Min(1, campaigns.Count - 1)];
            var hanhTrinhDoDot = campaigns.FirstOrDefault(d => d.TenDot.Contains("Hành Trình Đỏ")) ?? campaigns[Math.Min(3, campaigns.Count - 1)];
            var giotHongUnetiDot = campaigns.FirstOrDefault(d => d.TenDot.Contains("Giọt Hồng UNETI")) ?? campaigns[0];
            var traiTimDoDot = campaigns.FirstOrDefault(d => d.TenDot.Contains("Trái Tim Đỏ")) ?? campaigns[Math.Min(1, campaigns.Count - 1)];
            var vietDucDot = campaigns.FirstOrDefault(d => d.TenDot.Contains("Việt Đức")) ?? campaigns[Math.Min(4, campaigns.Count - 1)];
            var bachMaiDot = campaigns.FirstOrDefault(d => d.TenDot.Contains("Bạch Mai")) ?? campaigns[campaigns.Count - 1];

            // Kịch bản phiếu đa dạng theo tháng (DaysAgo: 150 -> 1 ngày trước) và theo trạng thái
            var ticketSpecs = new[]
            {
                // Đợt Xuân Hồng (Cách đây ~4-5 tháng - Đạt chỉ tiêu 6/6 thành công)
                new { Email = "demo.minhanh@example.test", Dot = xuanHongDot, DaysAgo = 145, Status = "Hiến máu thành công", Reason = "Tham gia ngày hội Xuân Hồng UNETI." },
                new { Email = "demo.quocbao@example.test", Dot = xuanHongDot, DaysAgo = 142, Status = "Hiến máu thành công", Reason = "Hiến máu tình nguyện đầu năm học." },
                new { Email = "demo.thuha@example.test", Dot = xuanHongDot, DaysAgo = 140, Status = "Hiến máu thành công", Reason = "Giảng viên đồng hành cùng sinh viên hiến máu." },
                new { Email = "demo.duchuy@example.test", Dot = xuanHongDot, DaysAgo = 138, Status = "Hiến máu thành công", Reason = "Chia sẻ giọt máu hồng cứu người." },
                new { Email = "demo.ngoclan@example.test", Dot = xuanHongDot, DaysAgo = 135, Status = "Hiến máu thành công", Reason = "Hiến máu nhóm máu hiếm O- định kỳ." },
                new { Email = "demo.hainam@example.test", Dot = xuanHongDot, DaysAgo = 132, Status = "Hiến máu thành công", Reason = "Tình nguyện viên câu lạc bộ máu sống." },
                new { Email = "demo.khanhlinh@example.test", Dot = xuanHongDot, DaysAgo = 130, Status = "Không đủ điều kiện hiến", Reason = "Đăng ký hiến máu cùng chi đoàn." },

                // Đợt Mùa Hè Tình Nguyện (Cách đây ~2.5-3.5 tháng)
                new { Email = "demo.giaphuc@example.test", Dot = muaHeDot, DaysAgo = 105, Status = "Hiến máu thành công", Reason = "Hiến máu chiến dịch Mùa Hè Tình Nguyện." },
                new { Email = "demo.thanhmai@example.test", Dot = muaHeDot, DaysAgo = 100, Status = "Hiến máu thành công", Reason = "Bổ sung nguồn máu hiếm AB- cho bệnh viện." },
                new { Email = "demo.quanghuy@example.test", Dot = muaHeDot, DaysAgo = 96, Status = "Hiến máu thành công", Reason = "Sinh viên tình nguyện mùa hè xanh." },
                new { Email = "demo.thuyduong@example.test", Dot = muaHeDot, DaysAgo = 92, Status = "Hiến máu thành công", Reason = "Trao giọt máu đào - Tiếp thêm sự sống." },
                new { Email = "demo.minhkhoi@example.test", Dot = muaHeDot, DaysAgo = 88, Status = "Không đủ điều kiện hiến", Reason = "Tham gia hiến máu nhân đạo định kỳ." },
                new { Email = "demo.baotram@example.test", Dot = muaHeDot, DaysAgo = 85, Status = "Hiến máu thành công", Reason = "Ủng hộ ngân hàng máu cấp cứu hè." },
                new { Email = "demo.tuananh@example.test", Dot = muaHeDot, DaysAgo = 80, Status = "Đã hủy", Reason = "Đăng ký hiến máu lưu động." },

                // Đợt Hành Trình Đỏ (Cách đây ~1-2 tháng)
                new { Email = "demo.phuongthao@example.test", Dot = hanhTrinhDoDot, DaysAgo = 55, Status = "Hiến máu thành công", Reason = "Hưởng ứng chiến dịch Hành Trình Đỏ." },
                new { Email = "demo.thutrang@example.test", Dot = hanhTrinhDoDot, DaysAgo = 50, Status = "Hiến máu thành công", Reason = "Kết nối dòng máu Việt." },
                new { Email = "demo.vanthai@example.test", Dot = hanhTrinhDoDot, DaysAgo = 46, Status = "Hiến máu thành công", Reason = "Hiến máu tình nguyện cứu bệnh nhân cấp cứu." },
                new { Email = "demo.quangminh@example.test", Dot = hanhTrinhDoDot, DaysAgo = 42, Status = "Hiến máu thành công", Reason = "Đoàn viên ưu tú tham gia hiến máu." },
                new { Email = "demo.hoanglong@example.test", Dot = hanhTrinhDoDot, DaysAgo = 38, Status = "Hiến máu thành công", Reason = "Hiến máu nhóm máu hiếm O- theo lời kêu gọi." },
                new { Email = "demo.myduyen@example.test", Dot = hanhTrinhDoDot, DaysAgo = 34, Status = "Không đủ điều kiện hiến", Reason = "Đăng ký tham gia Hành Trình Đỏ." },
                new { Email = "demo.ducmanh@example.test", Dot = hanhTrinhDoDot, DaysAgo = 32, Status = "Hiến máu thành công", Reason = "Chia sẻ trách nhiệm cộng đồng." },
                new { Email = "demo.kimngan@example.test", Dot = hanhTrinhDoDot, DaysAgo = 30, Status = "Từ chối", Reason = "Đăng ký hiến máu tình nguyện." },

                // Các đợt hiện tại (Trong tháng này - Đầy đủ trạng thái để Admin/Nhân viên y tế thao tác trực tiếp)
                new { Email = "demo.minhanh@example.test", Dot = giotHongUnetiDot, DaysAgo = 18, Status = "Hiến máu thành công", Reason = "Đủ 84 ngày, tiếp tục hiến máu tại UNETI." },
                new { Email = "demo.quocbao@example.test", Dot = giotHongUnetiDot, DaysAgo = 15, Status = "Hiến máu thành công", Reason = "Hiến máu lần 2 trong năm tại trường." },
                new { Email = "demo.thuha@example.test", Dot = traiTimDoDot, DaysAgo = 12, Status = "Hiến máu thành công", Reason = "Ủng hộ chương trình Trái Tim Đỏ." },
                new { Email = "demo.duchuy@example.test", Dot = giotHongUnetiDot, DaysAgo = 10, Status = "Đã khám sàng lọc", Reason = "Đã hoàn thành khám sàng lọc, chờ ghi kết quả." },
                new { Email = "demo.ngoclan@example.test", Dot = traiTimDoDot, DaysAgo = 9, Status = "Đã khám sàng lọc", Reason = "Người hiến máu hiếm O- đã qua khám sàng lọc." },
                new { Email = "demo.hainam@example.test", Dot = vietDucDot, DaysAgo = 8, Status = "Đã khám sàng lọc", Reason = "Hỗ trợ nguồn máu phẫu thuật tại BV Việt Đức." },
                new { Email = "demo.khanhlinh@example.test", Dot = giotHongUnetiDot, DaysAgo = 7, Status = "Chờ khám sàng lọc", Reason = "Đã có lịch hẹn khám sàng lọc tại phòng y tế." },
                new { Email = "demo.giaphuc@example.test", Dot = traiTimDoDot, DaysAgo = 6, Status = "Chờ khám sàng lọc", Reason = "Đặt lịch khám sàng lọc buổi sáng." },
                new { Email = "demo.thanhmai@example.test", Dot = vietDucDot, DaysAgo = 5, Status = "Chờ khám sàng lọc", Reason = "Đặt lịch khám sàng lọc nhóm máu hiếm AB-." },
                new { Email = "demo.quanghuy@example.test", Dot = bachMaiDot, DaysAgo = 5, Status = "Chờ khám sàng lọc", Reason = "Tham gia Chủ Nhật Đỏ tại Bạch Mai." },
                new { Email = "demo.thuyduong@example.test", Dot = giotHongUnetiDot, DaysAgo = 4, Status = "Đã xác nhận", Reason = "Hồ sơ đã duyệt, sẵn sàng xếp lịch khám." },
                new { Email = "demo.minhkhoi@example.test", Dot = traiTimDoDot, DaysAgo = 3, Status = "Đã xác nhận", Reason = "Đã xác nhận thông tin, chờ phân công bác sĩ khám." },
                new { Email = "demo.baotram@example.test", Dot = vietDucDot, DaysAgo = 3, Status = "Đã xác nhận", Reason = "Đã duyệt phiếu đăng ký hiến máu tình nguyện." },
                new { Email = "demo.tuananh@example.test", Dot = bachMaiDot, DaysAgo = 2, Status = "Đã xác nhận", Reason = "Chờ xếp lịch khám sàng lọc cuối tuần." },
                new { Email = "demo.phuongthao@example.test", Dot = giotHongUnetiDot, DaysAgo = 2, Status = "Chờ xác nhận", Reason = "Sinh viên đăng ký mới chờ nhân viên y tế duyệt." },
                new { Email = "demo.thutrang@example.test", Dot = traiTimDoDot, DaysAgo = 1, Status = "Chờ xác nhận", Reason = "Đăng ký tham gia đợt hiến máu Trái Tim Đỏ." },
                new { Email = "demo.vanthai@example.test", Dot = vietDucDot, DaysAgo = 1, Status = "Chờ xác nhận", Reason = "Đăng ký hiến máu cứu người tại BV Việt Đức." },
                new { Email = "demo.quangminh@example.test", Dot = bachMaiDot, DaysAgo = 1, Status = "Chờ xác nhận", Reason = "Đăng ký hiến máu Chủ Nhật Đỏ." },
                new { Email = "demo.hoanglong@example.test", Dot = giotHongUnetiDot, DaysAgo = 0, Status = "Chờ xác nhận", Reason = "Tình nguyện viên máu hiếm O- đăng ký trực tuyến." },
                new { Email = "demo.myduyen@example.test", Dot = traiTimDoDot, DaysAgo = 6, Status = "Từ chối", Reason = "Chưa đủ 84 ngày kể từ lần tiếp nhận gần nhất." },
                new { Email = "demo.ducmanh@example.test", Dot = vietDucDot, DaysAgo = 4, Status = "Đã hủy", Reason = "Đi công tác đột xuất nên xin hủy lịch đợt này." }
            };

            // Nạp bổ sung lịch sử cho tài khoản mặc định "donor" (Nguyễn Văn An) để khi đăng nhập bằng tài khoản "donor" có sẵn dữ liệu đẹp
            var defaultDonor = context.NguoiHienMaus.FirstOrDefault(n => n.Email == "nguyenvanan@gmail.com");
            if (defaultDonor != null)
            {
                profilesByEmail["nguyenvanan@gmail.com"] = defaultDonor;
            }

            var existingApps = context.PhieuDangKyHienMaus
                .Select(p => new { p.MaNguoiHien, p.MaDot })
                .ToList()
                .ToHashSet();

            var addedApps = new List<PhieuDangKyHienMau>();
            int idx = 0;
            foreach (var spec in ticketSpecs)
            {
                if (!profilesByEmail.TryGetValue(spec.Email, out var donor)) continue;
                var key = new { MaNguoiHien = donor.MaNguoiHien, MaDot = spec.Dot.MaDot };
                if (existingApps.Contains(key)) continue;

                var ngayNop = DateTime.Today.AddDays(-spec.DaysAgo).AddHours(8 + (idx % 9)).AddMinutes((idx * 13) % 60);
                DateTime? ngayXuLy = spec.Status == "Chờ xác nhận"
                    ? null
                    : ngayNop.AddHours(3);
                string? ghiChuXacNhan = spec.Status switch
                {
                    "Chờ xác nhận" => null,
                    "Từ chối" => "Tình nguyện viên chưa đủ điều kiện khoảng cách giữa 2 lần hiến hoặc đang dùng thuốc điều trị.",
                    "Đã hủy" => "Đã xác nhận hồ sơ [Hủy phiếu] Người hiến máu xin hủy do lịch công tác đột xuất.",
                    _ => "Hồ sơ hợp lệ, đủ điều kiện độ tuổi và cân nặng để tham gia khám sàng lọc."
                };

                var phieu = new PhieuDangKyHienMau
                {
                    MaNguoiHien = donor.MaNguoiHien,
                    MaDot = spec.Dot.MaDot,
                    NgayNop = ngayNop,
                    NgayXuLy = ngayXuLy,
                    GhiChuXacNhan = ghiChuXacNhan,
                    TrangThai = spec.Status,
                    LyDoHienMau = spec.Reason,
                    GhiChu = "Hồ sơ đăng ký qua Cổng thông tin Hiến máu UNETI."
                };
                addedApps.Add(phieu);
                existingApps.Add(key);
                idx++;
            }

            // Tạo thêm 2 phiếu mẫu cho tài khoản mặc định "donor" (Nguyễn Văn An): 1 phiếu thành công cách đây 95 ngày + 1 phiếu đang Chờ khám sàng lọc (có lịch trong 24h tới!)
            if (defaultDonor != null)
            {
                var key1 = new { MaNguoiHien = defaultDonor.MaNguoiHien, MaDot = muaHeDot.MaDot };
                if (!existingApps.Contains(key1))
                {
                    addedApps.Add(new PhieuDangKyHienMau
                    {
                        MaNguoiHien = defaultDonor.MaNguoiHien,
                        MaDot = muaHeDot.MaDot,
                        NgayNop = DateTime.Today.AddDays(-98).AddHours(9),
                        NgayXuLy = DateTime.Today.AddDays(-95).AddHours(11),
                        GhiChuXacNhan = "Đã xác nhận đủ điều kiện tham gia đợt hiến máu.",
                        TrangThai = "Hiến máu thành công",
                        LyDoHienMau = "Hiến máu tình nguyện hè 2026.",
                        GhiChu = "Đã hoàn thành hiến 350ml máu toàn phần."
                    });
                    existingApps.Add(key1);
                }

                var key2 = new { MaNguoiHien = defaultDonor.MaNguoiHien, MaDot = giotHongUnetiDot.MaDot };
                if (!existingApps.Contains(key2))
                {
                    addedApps.Add(new PhieuDangKyHienMau
                    {
                        MaNguoiHien = defaultDonor.MaNguoiHien,
                        MaDot = giotHongUnetiDot.MaDot,
                        NgayNop = DateTime.Today.AddDays(-2).AddHours(14),
                        NgayXuLy = DateTime.Today.AddDays(-1).AddHours(9),
                        GhiChuXacNhan = "Đã duyệt phiếu và xếp lịch khám sàng lọc tại Phòng Y tế Nhà C.",
                        TrangThai = "Chờ khám sàng lọc",
                        LyDoHienMau = "Tiếp tục tham gia Giọt Hồng UNETI 2026.",
                        GhiChu = "Mong muốn hiến 350ml máu toàn phần."
                    });
                    existingApps.Add(key2);
                }
            }

            if (addedApps.Count > 0)
            {
                context.PhieuDangKyHienMaus.AddRange(addedApps);
                context.SaveChanges();
            }

            // 6. Đảm bảo tính nhất quán dữ liệu cho LichKhamSangLoc và KetQuaHienMau trên toàn bộ phiếu
            var allApps = context.PhieuDangKyHienMaus
                .Include(p => p.LichKhamSangLocs)
                .Include(p => p.KetQuaHienMau)
                .OrderBy(p => p.MaPhieu)
                .ToList();

            var doctors = new[]
            {
                "BS. CKI Trần Mai Linh",
                "BS. Nguyễn Hoàng Nam",
                "BS. CKI Lê Thu Hương",
                "BS. Đặng Quốc Tuấn"
            };
            var locations = new[]
            {
                "Phòng Y tế Nhà C - 218 Lĩnh Nam, Hoàng Mai, Hà Nội",
                "Hội trường A1 - Cơ sở 454 Minh Khai, Hai Bà Trưng, Hà Nội",
                "Khoa Tiếp nhận máu - Viện Huyết học TW",
                "Điểm hiến máu cố định BV Hữu Nghị Việt Đức"
            };

            int futureSlot = 1;
            int pastSlot = 1;
            foreach (var app in allApps)
            {
                // 6.1. Phiếu "Chờ khám sàng lọc" bắt buộc phải có lịch khám "Đã lên lịch" trong tương lai (không trùng ca bác sĩ)
                if (app.TrangThai == "Chờ khám sàng lọc" && !app.LichKhamSangLocs.Any(l => l.TrangThai == "Đã lên lịch"))
                {
                    // Lịch đầu tiên đặt vào ngày mai (trong vòng 24-48h) để hiện cảnh báo lịch sắp diễn ra
                    var start = DateTime.Today.AddDays(1 + (futureSlot / 4)).AddHours(8 + (futureSlot % 4) * 2);
                    context.LichKhamSangLocs.Add(new LichKhamSangLoc
                    {
                        MaPhieu = app.MaPhieu,
                        ThoiGianBatDau = start,
                        ThoiGianKetThuc = start.AddMinutes(45),
                        HinhThucTiepNhan = futureSlot % 3 == 0 ? "Trực tiếp tại điểm hiến" : "Trực tiếp",
                        DiaDiemHoacLienKet = locations[futureSlot % locations.Length],
                        BacSiSangLoc = doctors[futureSlot % doctors.Length],
                        TrangThai = "Đã lên lịch",
                        GhiChu = "Vui lòng mang theo CCCD gắn chip và ăn nhẹ trước khi khám sàng lọc."
                    });
                    futureSlot++;
                }
                // 6.2. Phiếu "Đã khám sàng lọc", "Hiến máu thành công", "Không đủ điều kiện hiến" phải có lịch "Đã hoàn thành"
                else if ((app.TrangThai == "Đã khám sàng lọc" || app.TrangThai == "Hiến máu thành công" || app.TrangThai == "Không đủ điều kiện hiến")
                         && !app.LichKhamSangLocs.Any(l => l.TrangThai == "Đã hoàn thành"))
                {
                    var pastStart = app.NgayNop.Date.AddDays(1).AddHours(8 + (pastSlot % 8));
                    if (pastStart >= DateTime.Now) pastStart = DateTime.Now.AddHours(-4 - (pastSlot % 6));
                    context.LichKhamSangLocs.Add(new LichKhamSangLoc
                    {
                        MaPhieu = app.MaPhieu,
                        ThoiGianBatDau = pastStart,
                        ThoiGianKetThuc = pastStart.AddMinutes(30),
                        HinhThucTiepNhan = "Trực tiếp",
                        DiaDiemHoacLienKet = locations[pastSlot % locations.Length],
                        BacSiSangLoc = doctors[pastSlot % doctors.Length],
                        TrangThai = "Đã hoàn thành",
                        GhiChu = "Đã kiểm tra huyết áp (120/80 mmHg), nhịp tim (74 l/p) và xét nghiệm Hb nhanh đạt yêu cầu."
                    });
                    pastSlot++;
                }
                // 6.3. Phiếu "Đã hủy" tạo lịch "Đã hủy" mẫu nếu chưa có
                else if (app.TrangThai == "Đã hủy" && !app.LichKhamSangLocs.Any())
                {
                    var canceledStart = app.NgayNop.Date.AddDays(1).AddHours(9);
                    context.LichKhamSangLocs.Add(new LichKhamSangLoc
                    {
                        MaPhieu = app.MaPhieu,
                        ThoiGianBatDau = canceledStart,
                        ThoiGianKetThuc = canceledStart.AddMinutes(30),
                        HinhThucTiepNhan = "Trực tiếp",
                        DiaDiemHoacLienKet = locations[0],
                        BacSiSangLoc = doctors[0],
                        TrangThai = "Đã hủy",
                        GhiChu = "[Đã hủy: Người hiến máu xin dời lịch do bận công tác đột xuất]"
                    });
                }

                // 6.4. Phiếu "Hiến máu thành công" hoặc "Không đủ điều kiện hiến" phải có KetQuaHienMau
                if ((app.TrangThai == "Hiến máu thành công" || app.TrangThai == "Không đủ điều kiện hiến") && app.KetQuaHienMau == null)
                {
                    bool isSuccess = app.TrangThai == "Hiến máu thành công";
                    double score = isSuccess ? Math.Round(9.0 + (app.MaPhieu % 11) * 0.1, 1) : Math.Round(4.5 + (app.MaPhieu % 4) * 0.5, 1);
                    context.KetQuaHienMaus.Add(new KetQuaHienMau
                    {
                        MaPhieu = app.MaPhieu,
                        KetQua = app.TrangThai,
                        DiemDanhGia = score,
                        NhanXet = isSuccess
                            ? "Sức khỏe tốt, huyết sắc tố ≥ 125 g/L, tiếp nhận 350ml máu toàn phần an toàn, không có phản ứng sau hiến."
                            : "Huyết áp thấp tạm thời hoặc nồng độ Hemoglobin chưa đạt ngưỡng tối thiểu 120 g/L tại thời điểm khám, khuyến nghị bổ sung sắt và nghỉ ngơi.",
                        NgayCapNhat = app.NgayXuLy ?? app.NgayNop.AddDays(1).AddHours(2)
                    });
                }
            }

            context.SaveChanges();
        }
    }
}

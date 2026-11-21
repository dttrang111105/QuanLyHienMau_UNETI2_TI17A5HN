using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.Repositories;

if (args.Contains("--live")) { await LiveChecks.Run(args.Contains("--keep-data")); return; }
if (args.Contains("--cleanup")) { await LiveChecks.Cleanup(); return; }
if (args.Contains("--seed"))
{
    await using var seedDb = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=QuanLyHienMau_UNETI;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True").Options);
    DbInitializer.Initialize(seedDb);
    Console.WriteLine($"SEEDED OK: TaiKhoan={await seedDb.TaiKhoans.CountAsync()}, DonVi={await seedDb.DonViToChucs.CountAsync()}, Dot={await seedDb.DotHienMaus.CountAsync()}, NguoiHien={await seedDb.NguoiHienMaus.CountAsync()}, Phieu={await seedDb.PhieuDangKyHienMaus.CountAsync()}, Lich={await seedDb.LichKhamSangLocs.CountAsync()}, KetQua={await seedDb.KetQuaHienMaus.CountAsync()}");
    return;
}
if (args.Contains("--module1"))
{
    int m1 = 0;
    void CheckM1(bool cond, string msg) { if (!cond) throw new Exception("FAIL: " + msg); m1++; Console.WriteLine("PASS [Module 1] " + msg); }
    bool ValidateObj(object o) => Validator.TryValidateObject(o, new ValidationContext(o), new List<ValidationResult>(), true);

    // 1. Validate RegisterViewModel
    QuanLyHienMau_UNETI2_TI17A5HN.Models.ViewModels.RegisterViewModel ValidReg() => new()
    {
        TenDangNhap = "nam_23103100275",
        HoTen = "Nguyễn Tiến Nam",
        Email = "nam@uneti.edu.vn",
        SoDienThoai = "0912345678",
        CanNang = 62,
        MatKhau = "Password@123",
        XacNhanMatKhau = "Password@123"
    };
    CheckM1(ValidateObj(ValidReg()), "RegisterViewModel hợp lệ được chấp nhận");
    var r1 = ValidReg(); r1.TenDangNhap = "ab"; CheckM1(!ValidateObj(r1), "Tên đăng nhập < 3 ký tự bị từ chối");
    var r2 = ValidReg(); r2.TenDangNhap = "user name!"; CheckM1(!ValidateObj(r2), "Tên đăng nhập chứa khoảng trắng/ký tự đặc biệt bị từ chối");
    var r3 = ValidReg(); r3.Email = "invalid-email"; CheckM1(!ValidateObj(r3), "Email sai định dạng bị từ chối");
    var r4 = ValidReg(); r4.SoDienThoai = "0123456789"; CheckM1(!ValidateObj(r4), "SĐT sai đầu số Việt Nam bị từ chối");
    var r5 = ValidReg(); r5.CanNang = 20; CheckM1(!ValidateObj(r5), "Cân nặng ngoài khoảng [30, 200] bị từ chối");
    var r6 = ValidReg(); r6.MatKhau = "12345"; r6.XacNhanMatKhau = "12345"; CheckM1(!ValidateObj(r6), "Mật khẩu < 6 ký tự bị từ chối");
    var r7 = ValidReg(); r7.XacNhanMatKhau = "Mismatch@123"; CheckM1(!ValidateObj(r7), "Mật khẩu xác nhận không khớp bị từ chối");

    // 2. Validate DonViToChuc
    var dvValid = new DonViToChuc { TenDonViToChuc = "Đoàn Thanh Niên UNETI", EmailLienHe = "doantn@uneti.edu.vn", TrangThai = "Hoạt động" };
    CheckM1(ValidateObj(dvValid), "DonViToChuc hợp lệ được chấp nhận");
    var dvEmpty = new DonViToChuc { TenDonViToChuc = "" }; CheckM1(!ValidateObj(dvEmpty), "DonViToChuc trống tên bị từ chối");
    var dvBadMail = new DonViToChuc { TenDonViToChuc = "Đơn vị A", EmailLienHe = "bad-email" }; CheckM1(!ValidateObj(dvBadMail), "DonViToChuc sai định dạng email bị từ chối");

    // 3. Kiểm thử Repository & Nghiệp vụ Module 1 với Database thực tế
    await using var db1 = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=QuanLyHienMau_UNETI;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True").Options);
    var tkRepo = new TaiKhoanRepository(db1);
    var dvRepo = new DonViToChucRepository(db1);

    CheckM1((await tkRepo.FindForLoginAsync("admin")) != null, "Đăng nhập tìm theo TenDangNhap hoạt động");
    CheckM1((await tkRepo.FindForLoginAsync("admin@uneti.edu.vn")) != null, "Đăng nhập tìm theo Email hoạt động");
    CheckM1((await tkRepo.FindForLoginAsync("0912345678")) != null, "Đăng nhập tìm theo Số điện thoại người hiến hoạt động");
    CheckM1(await tkRepo.IsTenDangNhapExistsAsync("ADMIN"), "Kiểm tra trùng tên đăng nhập không phân biệt hoa/thường");
    CheckM1(await tkRepo.IsEmailExistsAsync("ADMIN@UNETI.EDU.VN"), "Kiểm tra trùng Email không phân biệt hoa/thường");
    CheckM1(await tkRepo.IsSoDienThoaiExistsAsync("0912345678"), "Kiểm tra trùng Số điện thoại hoạt động");

    var accStats = await tkRepo.GetAccountStatsAsync();
    CheckM1(accStats.TotalCount == accStats.AdminCount + accStats.MedicalStaffCount + accStats.DonorCount, "Thống kê tổng số tài khoản khớp tổng 3 vai trò");
    CheckM1(accStats.TotalCount == accStats.ActiveCount + accStats.LockedCount, "Thống kê tài khoản khớp Đang hoạt động + Bị khóa");

    var firstDv = (await dvRepo.GetAllWithDotHienMauAsync()).Last();
    CheckM1(await dvRepo.IsTenDonViExistsAsync(firstDv.TenDonViToChuc.ToUpper()), "Kiểm tra trùng tên Đơn vị tổ chức không phân biệt hoa/thường");
    CheckM1(!await dvRepo.IsTenDonViExistsAsync(firstDv.TenDonViToChuc, firstDv.MaDonViToChuc), "Kiểm tra trùng tên Đơn vị tổ chức loại trừ chính nó khi Edit");
    CheckM1(await dvRepo.HasDotHienMauAsync(firstDv.MaDonViToChuc), "Kiểm tra ràng buộc chặn xóa Đơn vị tổ chức đã có Đợt hiến máu");

    Console.WriteLine($"ALL {m1} MODULE 1 CHECKS PASSED!");
    return;
}

int checks = 0;
void Check(bool condition, string name) { if (!condition) throw new Exception(name); checks++; Console.WriteLine("PASS " + name); }
DotHienMau Valid() => new() { MaDonViToChuc = 1, TenDot = "Campaign", ChiTieuDonViMau = 10, NgayBatDauDangKy = DateTime.Today, HanDangKy = DateTime.Today, TrangThai = "Đang mở đăng ký" };
bool IsValid(DotHienMau d) => Validator.TryValidateObject(d, new ValidationContext(d), new List<ValidationResult>(), true);
var d = Valid(); Check(IsValid(d), "Valid campaign and equal dates accepted");
d.TenDot = " "; Check(!IsValid(d), "Blank name rejected");
d = Valid(); d.MaDonViToChuc = 0; Check(!IsValid(d), "Missing organization rejected");
d = Valid(); d.ChiTieuDonViMau = 0; Check(!IsValid(d), "Zero quota rejected");
d = Valid(); d.CanNangToiThieu = -1; Check(!IsValid(d), "Negative weight rejected");
d = Valid(); d.HanDangKy = DateTime.Today.AddDays(-1); Check(!IsValid(d), "Reversed dates rejected");
d = Valid(); d.NgayBatDauDangKy = default; Check(!IsValid(d), "Default start date rejected");
d = Valid(); d.TrangThai = "invalid"; Check(!IsValid(d), "Unknown status rejected");
d = Valid(); Check(d.CoTheDangKy, "Inclusive start and deadline accepted");
d.NgayBatDauDangKy = DateTime.Today.AddDays(1); Check(!d.CoTheDangKy && d.ThoiHanHienThi == "Chưa đến ngày đăng ký", "Future campaign disabled");
d = Valid(); d.HanDangKy = DateTime.Today.AddDays(-1); Check(!d.CoTheDangKy && d.ThoiHanHienThi == "Hết hạn đăng ký", "Expired campaign disabled");
foreach (var state in new[] { "Chưa mở", "Tạm dừng", "Đã đóng" }) { d = Valid(); d.TrangThai = state; Check(!d.CoTheDangKy, "Registration disabled for " + state); }
using var db = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer("Server=localhost;Database=Module2Checks;Trusted_Connection=True;TrustServerCertificate=True").Options);
var repo = new DotHienMauRepository(db);
var data = new[] {
 new DotHienMau { MaDot=2, TenDot="Beta", NhomMauUuTien="AB", MaDonViToChuc=1, ChiTieuDonViMau=20, CanNangToiThieu=50, HanDangKy=DateTime.Today.AddDays(-1), TrangThai="Đã đóng" },
 new DotHienMau { MaDot=3, TenDot="Alpha", NhomMauUuTien="A", MaDonViToChuc=2, ChiTieuDonViMau=10, CanNangToiThieu=45, HanDangKy=DateTime.Today, TrangThai="Đang mở đăng ký", DonViToChuc=new() { TenDonViToChuc="UNETI" } },
 new DotHienMau { MaDot=1, TenDot="Alpha", NhomMauUuTien="O, A", MaDonViToChuc=2, ChiTieuDonViMau=10, CanNangToiThieu=45, HanDangKy=DateTime.Today, TrangThai="Đang mở đăng ký" }
}.AsQueryable();
Check(repo.ApplyFilterNhomMau(data,"A").Count()==2, "A matches single and comma-separated tokens, excludes AB");
Check(!repo.ApplyFilterNhomMau(data,"B").Any(), "B excludes AB");
Check(repo.ApplyFilterNhomMau(data,"AB").Single().MaDot==2, "AB exact match");
Check(repo.ApplySearch(data," uneti ").Single().MaDot==3, "Organization search trims keyword");
Check(repo.ApplySort(data,"name_asc").Select(x=>x.MaDot).SequenceEqual(new[]{1,3,2}), "Stable name sort with unique tie breaker");
var combined = repo.ApplySearch(data,"Alpha"); combined=repo.ApplyFilterDonVi(combined,2); combined=repo.ApplyFilterNhomMau(combined,"A"); combined=repo.ApplyFilterTrangThai(combined,"Đang mở đăng ký"); combined=repo.ApplyFilterCanNang(combined,45); combined=repo.ApplyFilterThoiHan(combined,"conhan");
Check(repo.ApplySort(combined,"quota_desc").Skip(1).Take(1).Single().MaDot==3, "Combined search, all filters, sort and paging");
foreach (var sort in new[]{"name_asc","name_desc","deadline_asc","deadline_desc","quota_asc","quota_desc"}) {
 var q=repo.ApplySort(repo.ApplyFilterNhomMau(repo.GetQueryable(),"A"),sort).Skip(0).Take(6);
 var sql=q.ToQueryString(); Check(sql.Contains("OFFSET") && sql.Contains("FETCH NEXT") && sql.Contains("MaDot"), "SQL translation and server paging: "+sort);
}

var dates = new[] { Valid(), Valid(), Valid(), Valid(), Valid() };
dates[1].NgayBatDauDangKy = DateTime.Today.AddDays(1);
dates[2].HanDangKy = DateTime.Today.AddDays(-1);
dates[3].TrangThai = "Tạm dừng";
dates[4].TrangThai = "Đã đóng";
Check(repo.ApplyFilterDangNhanDangKy(dates.AsQueryable()).SequenceEqual(dates.Where(x => x.CoTheDangKy)), "Public open filter agrees with registration eligibility, including boundary dates");
Check(repo.ApplyFilterTrangThai(dates.AsQueryable(), "Đang mở đăng ký").Count() == 3, "Admin stored status filter remains independent of dates");
Check(repo.ApplyFilterDangNhanDangKy(repo.GetQueryable()).ToQueryString().Contains("CONVERT(date"), "Receiving filter translates date boundaries to SQL");
Console.WriteLine($"All {checks} module 2 checks passed. No database connection or mutation performed.");

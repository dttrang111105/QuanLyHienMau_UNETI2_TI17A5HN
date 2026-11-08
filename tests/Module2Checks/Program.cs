using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.Repositories;

if (args.Contains("--live")) { await LiveChecks.Run(args.Contains("--keep-data")); return; }
if (args.Contains("--cleanup")) { await LiveChecks.Cleanup(); return; }

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

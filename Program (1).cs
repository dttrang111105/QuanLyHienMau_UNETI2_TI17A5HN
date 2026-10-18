// Họ và tên: ĐIỀN HỌ TÊN CỦA BẠN
// Mã sinh viên: ĐIỀN MSSV CỦA BẠN
// Nội dung: Module 5 - kiểm tra nghiệp vụ trên database quan hệ.
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.Services;
using QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

var passed = 0;
var failed = 0;
void Check(bool value, string message = "Assertion failed")
{
    if (!value)
        throw new Exception(message);
}

async Task Test(string title, Func<ApplicationDbContext, Module5Service, FixedClock, Task> fn, bool seedDemo = false)
{
    try
    {
        await using var c = new SqliteConnection("Data Source=:memory:");
        var sql = Environment.GetEnvironmentVariable("MODULE5_SQLSERVER_TEST");
        var options = new DbContextOptionsBuilder<ApplicationDbContext>();
        if (string.IsNullOrEmpty(sql))
        {
            await c.OpenAsync();
            options.UseSqlite(c);
        }
        else
        {
            var cs = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(sql)
            {
                InitialCatalog = "M5Case_" + Guid.NewGuid().ToString("N")
            };
            options.UseSqlServer(cs.ConnectionString);
        }

        await using var db = new ApplicationDbContext(options.Options);
        await db.Database.EnsureCreatedAsync();
        var clock = new FixedClock();
        var s = new Module5Service(db, clock);
        if (seedDemo)
            await SeedData.Initialize(db, s.Now);
        else
        {
            var unit = new DonViToChuc
            {
                TenDonViToChuc = "Đơn vị A"
            };
            var dot = new DotHienMau
            {
                DonViToChuc = unit,
                TenDot = "Đợt A",
                ChiTieuDonViMau = 1
            };
            var p1 = new NguoiHienMau
            {
                HoTen = "Người 1",
                TaiKhoan = new TaiKhoan
                {
                    TenDangNhap = "u1",
                    MatKhau = "test"
                }
            };
            var p2 = new NguoiHienMau
            {
                HoTen = "Người 2",
                TaiKhoan = new TaiKhoan
                {
                    TenDangNhap = "u2",
                    MatKhau = "test"
                }
            };
            db.AddRange(Enumerable.Range(1, 8).Select(i => new PhieuDangKyHienMau { NguoiHien = i <= 2 || i >= 5 ? p1 : p2, Dot = dot, NgayNop = s.Now, TrangThai = i == 5 ? TrangThaiPhieu.ChoXacNhan : i == 6 ? TrangThaiPhieu.TuChoi : i == 7 ? TrangThaiPhieu.DaHuy : i == 8 ? TrangThaiPhieu.DaKham : TrangThaiPhieu.DaXacNhan }));
            await db.SaveChangesAsync();
        }

        await fn(db, s, clock);
        if (!string.IsNullOrEmpty(sql))
        {
            db.ChangeTracker.Clear();
            await db.Database.EnsureDeletedAsync();
        }

        Console.WriteLine("PASS " + title);
        passed++;
    }
    catch (Exception ex)
    {
        Console.WriteLine("FAIL " + title + " :: " + ex);
        failed++;
    }
}

LichFormVm Form(Module5Service s, int p = 1, string bs = "BS A", int hours = 24) => new()
{
    MaPhieu = p,
    ThoiGianBatDau = s.Now.AddHours(hours),
    ThoiGianKetThuc = s.Now.AddHours(hours + 1),
    BacSiSangLoc = bs,
    DiaDiemHoacLienKet = "Phòng A"
};
KetQuaFormVm Result(int p = 1, string k = TrangThaiPhieu.ThanhCong, int? score = 90) => new()
{
    MaPhieu = p,
    KetQua = k,
    DiemDanhGia = score,
    NhanXet = "Đã được nhân viên y tế xác nhận"
};
async Task Screen(ApplicationDbContext db, Module5Service s, FixedClock clock)
{
    Check(await s.LuuLich(Form(s)) == null);
    clock.Advance(TimeSpan.FromHours(26));
    Check(await s.DoiTrangThaiLich((await db.LichKhamSangLocs.SingleAsync()).MaLichKhamSangLoc, true) == null);
}

await Test("valid schedule persists and transitions ticket", async (db, s, c) =>
{
    Check(await s.LuuLich(Form(s)) == null);
    Check((await db.PhieuDangKyHienMaus.FindAsync(1))!.TrangThai == TrangThaiPhieu.ChoKham);
    Check(await db.LichKhamSangLocs.CountAsync() == 1);
});
foreach (var p in new[]
{
    5,
    6,
    7,
    8
}

)
    await Test("reject scheduling ticket state " + p, async (db, s, c) =>
    {
        Check(await s.LuuLich(Form(s, p)) != null);
        Check(await db.LichKhamSangLocs.CountAsync() == 0);
    });
await Test("reject end before start", async (db, s, c) =>
{
    var v = Form(s);
    v.ThoiGianKetThuc = v.ThoiGianBatDau.GetValueOrDefault().AddMinutes(-1);
    Check(await s.LuuLich(v) != null);
});
await Test("reject zero duration", async (db, s, c) =>
{
    var v = Form(s);
    v.ThoiGianKetThuc = v.ThoiGianBatDau;
    Check(await s.LuuLich(v) != null);
});
await Test("reject past and missing date", async (db, s, c) =>
{
    Check(await s.LuuLich(Form(s, hours: -2)) != null);
    var v = Form(s);
    v.ThoiGianBatDau = null;
    Check(await s.LuuLich(v) != null);
});
await Test("reject duplicate effective schedule", async (db, s, c) =>
{
    Check(await s.LuuLich(Form(s)) == null);
    Check(await s.LuuLich(Form(s, hours: 48)) != null);
    Check(await db.LichKhamSangLocs.CountAsync() == 1);
});
await Test("detect donor overlap across tickets", async (db, s, c) =>
{
    Check(await s.LuuLich(Form(s)) == null);
    var v = Form(s, 2, "BS B");
    v.ThoiGianBatDau = v.ThoiGianBatDau.GetValueOrDefault().AddMinutes(30);
    v.ThoiGianKetThuc = v.ThoiGianKetThuc.GetValueOrDefault().AddMinutes(30);
    Check((await s.LuuLich(v))!.Contains("Người hiến"));
});
await Test("detect doctor overlap ignoring case and outside whitespace", async (db, s, c) =>
{
    Check(await s.LuuLich(Form(s)) == null);
    Check((await s.LuuLich(Form(s, 3, " bs a ")))!.Contains("Bác sĩ"));
});
await Test("Vietnamese doctor case-insensitive overlap", async (db, s, c) =>
{
    Check(await s.LuuLich(Form(s, 1, "Bác sĩ Nguyễn An")) == null);
    Check((await s.LuuLich(Form(s, 3, "  bác sĩ nguyễn an  ")))!.Contains("Bác sĩ"));
});
await Test("Vietnamese doctor Unicode normalization", async (db, s, c) =>
{
    Check(await s.LuuLich(Form(s, 1, "Bác sĩ Nguyễn An")) == null);
    Check((await s.LuuLich(Form(s, 3, "Bác sĩ Nguyễn An".Normalize(System.Text.NormalizationForm.FormD))))!.Contains("Bác sĩ"));
});
await Test("allow back-to-back appointments", async (db, s, c) =>
{
    Check(await s.LuuLich(Form(s)) == null);
    Check(await s.LuuLich(Form(s, 3, hours: 25)) == null);
});
await Test("cancel preserves history and restores ticket", async (db, s, c) =>
{
    Check(await s.LuuLich(Form(s)) == null);
    Check(await s.DoiTrangThaiLich((await db.LichKhamSangLocs.SingleAsync()).MaLichKhamSangLoc, false) == null);
    Check((await db.PhieuDangKyHienMaus.FindAsync(1))!.TrangThai == TrangThaiPhieu.DaXacNhan);
    Check(await s.LuuLich(Form(s)) == null);
    Check(await db.LichKhamSangLocs.CountAsync() == 2);
});
await Test("edit excludes itself from conflict queries", async (db, s, c) =>
{
    var v = Form(s);
    Check(await s.LuuLich(v) == null);
    v.Id = (await db.LichKhamSangLocs.SingleAsync()).MaLichKhamSangLoc;
    Check(await s.LuuLich(v) == null);
});
await Test("edit cannot reassign ticket", async (db, s, c) =>
{
    var v = Form(s);
    Check(await s.LuuLich(v) == null);
    v.Id = (await db.LichKhamSangLocs.SingleAsync()).MaLichKhamSangLoc;
    v.MaPhieu = 3;
    Check(await s.LuuLich(v) != null);
});
await Test("future schedule cannot complete early", async (db, s, c) =>
{
    Check(await s.LuuLich(Form(s)) == null);
    Check(await s.DoiTrangThaiLich((await db.LichKhamSangLocs.SingleAsync()).MaLichKhamSangLoc, true) != null);
});
await Test("complete updates schedule and ticket atomically", async (db, s, c) =>
{
    await Screen(db, s, c);
    Check((await db.PhieuDangKyHienMaus.FindAsync(1))!.TrangThai == TrangThaiPhieu.DaKham);
    Check((await db.LichKhamSangLocs.SingleAsync()).TrangThai == TrangThaiLich.HoanThanh);
});
await Test("cancelled schedule cannot complete", async (db, s, c) =>
{
    Check(await s.LuuLich(Form(s)) == null);
    var id = (await db.LichKhamSangLocs.SingleAsync()).MaLichKhamSangLoc;
    Check(await s.DoiTrangThaiLich(id, false) == null);
    c.Advance(TimeSpan.FromHours(26));
    Check(await s.DoiTrangThaiLich(id, true) != null);
});
await Test("nonexistent schedule rejected", async (db, s, c) => Check(await s.DoiTrangThaiLich(999, true) != null));
await Test("reject final result before screening", async (db, s, c) => Check(await s.LuuKetQua(Result()) != null));
await Test("require completed schedule, not only ticket status", async (db, s, c) => Check(await s.LuuKetQua(Result(8)) != null));
foreach (var n in new[]
{
    -1,
    101
}

)
    await Test("reject score " + n, async (db, s, c) =>
    {
        await Screen(db, s, c);
        Check(await s.LuuKetQua(Result(score: n)) != null);
        Check(await db.KetQuaHienMaus.CountAsync() == 0);
    });
await Test("reject arbitrary outcome", async (db, s, c) =>
{
    await Screen(db, s, c);
    Check(await s.LuuKetQua(Result(k: "Tùy ý")) != null);
});
foreach (var outcome in new[]
{
    TrangThaiPhieu.ThanhCong,
    TrangThaiPhieu.KhongDat
}

)
    await Test("save final outcome " + outcome, async (db, s, c) =>
    {
        await Screen(db, s, c);
        Check(await s.LuuKetQua(Result(k: outcome)) == null);
        Check((await db.PhieuDangKyHienMaus.FindAsync(1))!.TrangThai == outcome);
        Check((await db.KetQuaHienMaus.SingleAsync()).NgayCapNhat == s.Now);
    });
await Test("no duplicate or overwrite of final outcome", async (db, s, c) =>
{
    await Screen(db, s, c);
    Check(await s.LuuKetQua(Result()) == null);
    Check(await s.LuuKetQua(Result(k: TrangThaiPhieu.KhongDat)) != null);
    Check(await db.KetQuaHienMaus.CountAsync() == 1);
});
await Test("dashboard updates immediately after schedule and outcome", async (db, s, c) =>
{
    var before = await s.Dashboard();
    Check(before.DaXacNhan == 4);
    Check(await s.LuuLich(Form(s)) == null);
    var scheduled = await s.Dashboard();
    Check(scheduled.LichSapToi == before.LichSapToi + 1);
    c.Advance(TimeSpan.FromHours(26));
    Check(await s.DoiTrangThaiLich((await db.LichKhamSangLocs.SingleAsync()).MaLichKhamSangLoc, true) == null);
    Check(await s.LuuKetQua(Result()) == null);
    Check((await s.Dashboard()).ThanhCong == before.ThanhCong + 1);
});
await Test("all statistics and zero denominators", async (db, s, c) =>
{
    var stats = await s.ThongKe(2026);
    Check(stats.TheoDot.Single().Tong == 8);
    Check(stats.TheoDot.Single().DaXacNhan == 4);
    Check(stats.TheoDot.Single().TyLeThanhCong == 0);
    Check(stats.PhieuTheoThang.Sum(x => x.SoLuong) == 8);
    Check(stats.PhieuTheoTrangThai.Sum(x => x.SoLuong) == 8);
    Check(stats.LichTheoThang.Count == 12);
    Check(stats.NhieuPhieuNhat.Count == 1);
});
await Test("screened denominator includes final outcomes", async (db, s, c) =>
{
    await Screen(db, s, c);
    Check(await s.LuuKetQua(Result()) == null);
    var row = (await s.TheoDot()).Single();
    Check(row.DaKham == 1 && row.ThanhCong == 1 && row.TyLeThanhCong == 100);
});
await Test("sample dataset and statistics agree", async (db, s, c) =>
{
    Check(await db.DonViToChucs.CountAsync() == 5);
    Check(await db.DotHienMaus.CountAsync() == 15);
    Check(await db.NguoiHienMaus.CountAsync() == 30);
    Check(await db.PhieuDangKyHienMaus.CountAsync() == 50);
    Check(await db.LichKhamSangLocs.CountAsync() == 20);
    Check(await db.KetQuaHienMaus.CountAsync() == 8);
    var d = await s.Dashboard();
    Check(d.Phieu == 50 && d.ThanhCong == 6);
    var r = await s.ThongKe(2026);
    Check(r.TheoDot.Sum(x => x.Tong) == 50);
    Check(r.ThanhCongTheoDonVi.Sum(x => x.SoLuong) == 6);
    Check(r.PhieuTheoTrangThai.Sum(x => x.SoLuong) == 50);
    Check(r.LichTheoThang.Sum(x => x.SoLuong) == 20);
}, true);
foreach (var item in new[]
{
    (-30, 30, true),
    (30, 90, true),
    (15, 45, true),
    (-30, 90, true),
    (0, 60, true),
    (-60, 0, false),
    (60, 120, false),
    (120, 180, false)
}

)
{
    foreach (var sameDoctor in new[]
    {
        false,
        true
    }

    )
    {
        await Test($"interval matrix {item.Item1}:{item.Item2} / doctor={sameDoctor}", async (db, sv, clock) =>
        {
            Check(await sv.LuuLich(Form(sv)) == null);
            var v = Form(sv, sameDoctor ? 3 : 2, sameDoctor ? "BS A" : "BS B");
            var origin = sv.Now.AddHours(24);
            v.ThoiGianBatDau = origin.AddMinutes(item.Item1);
            v.ThoiGianKetThuc = origin.AddMinutes(item.Item2);
            var error = await sv.LuuLich(v);
            Check((error != null) == item.Item3);
        });
    }
}

await Test("reject nonexistent ticket", async (db, sv, clock) => Check(await sv.LuuLich(Form(sv, 999)) != null));
await Test("reject unsupported reception type", async (db, sv, clock) =>
{
    var v = Form(sv);
    v.HinhThucTiepNhan = "Tùy ý";
    Check(await sv.LuuLich(v) != null);
});
await Test("reject blank doctor and location", async (db, sv, clock) =>
{
    var v = Form(sv);
    v.BacSiSangLoc = "  ";
    Check(await sv.LuuLich(v) != null);
    v.BacSiSangLoc = "BS A";
    v.DiaDiemHoacLienKet = "  ";
    Check(await sv.LuuLich(v) != null);
});
await Test("cancelled appointment frees doctor for another donor", async (db, sv, clock) =>
{
    Check(await sv.LuuLich(Form(sv)) == null);
    Check(await sv.DoiTrangThaiLich((await db.LichKhamSangLocs.SingleAsync()).MaLichKhamSangLoc, false) == null);
    Check(await sv.LuuLich(Form(sv, 3)) == null);
});
await Test("editing detects another appointment conflict", async (db, sv, clock) =>
{
    Check(await sv.LuuLich(Form(sv)) == null);
    Check(await sv.LuuLich(Form(sv, 3, "BS B")) == null);
    var v = Form(sv, 3, "BS A");
    v.Id = (await db.LichKhamSangLocs.SingleAsync(x => x.MaPhieu == 3)).MaLichKhamSangLoc;
    Check(await sv.LuuLich(v) != null);
});
await Test("cannot edit or cancel completed appointment", async (db, sv, clock) =>
{
    await Screen(db, sv, clock);
    var id = (await db.LichKhamSangLocs.SingleAsync()).MaLichKhamSangLoc;
    var v = Form(sv);
    v.Id = id;
    Check(await sv.LuuLich(v) != null);
    Check(await sv.DoiTrangThaiLich(id, false) != null);
    Check(await sv.DoiTrangThaiLich(id, true) != null);
});
await Test("empty clinical comment rejected", async (db, sv, clock) =>
{
    await Screen(db, sv, clock);
    var v = Result();
    v.NhanXet = "  ";
    Check(await sv.LuuKetQua(v) != null);
});
await Test("optional score can remain null", async (db, sv, clock) =>
{
    await Screen(db, sv, clock);
    Check(await sv.LuuKetQua(Result(score: null)) == null);
    Check((await db.KetQuaHienMaus.SingleAsync()).DiemDanhGia == null);
});
foreach (var score in new[]
{
    0,
    100
}

)
    await Test("score boundary does not override clinician " + score, async (db, sv, clock) =>
    {
        await Screen(db, sv, clock);
        Check(await sv.LuuKetQua(Result(k: score == 0 ? TrangThaiPhieu.ThanhCong : TrangThaiPhieu.KhongDat, score: score)) == null);
    });
await Test("cancel updates dashboard counters", async (db, sv, clock) =>
{
    var before = await sv.Dashboard();
    Check(await sv.LuuLich(Form(sv)) == null);
    var id = (await db.LichKhamSangLocs.SingleAsync()).MaLichKhamSangLoc;
    Check(await sv.DoiTrangThaiLich(id, false) == null);
    var after = await sv.Dashboard();
    Check(after.LichSapToi == before.LichSapToi && after.DaXacNhan == before.DaXacNhan);
});
await Test("month reports filter year correctly", async (db, sv, clock) =>
{
    var a = await sv.ThongKe(2026);
    var b = await sv.ThongKe(2025);
    Check(a.PhieuTheoThang.Sum(x => x.SoLuong) == 8);
    Check(b.PhieuTheoThang.Sum(x => x.SoLuong) == 0);
    Check(a.TheoDot.Sum(x => x.Tong) == b.TheoDot.Sum(x => x.Tong));
});
await Test("database unique result constraint", async (db, sv, clock) =>
{
    await Screen(db, sv, clock);
    Check(await sv.LuuKetQua(Result()) == null);
    db.ChangeTracker.Clear();
    db.Add(new KetQuaHienMau { MaPhieu = 1, NhanXet = "Duplicate", KetQua = TrangThaiPhieu.ThanhCong, NgayCapNhat = sv.Now });
    try
    {
        await db.SaveChangesAsync();
        throw new Exception("Duplicate result was accepted");
    }
    catch (DbUpdateException)
    {
    }
});
Console.WriteLine($"RESULT: {passed} passed, {failed} failed");
return failed == 0 ? 0 : 1;
class FixedClock : TimeProvider
{
    DateTimeOffset current = new(2026, 10, 18, 5, 0, 0, TimeSpan.Zero);
    public override DateTimeOffset GetUtcNow() => current;
    public void Advance(TimeSpan d) => current += d;
}

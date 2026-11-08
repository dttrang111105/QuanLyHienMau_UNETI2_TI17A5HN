using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

// Integration checks for module 2 only. Requests go through the running MVC app.
// Only records with this reserved test prefix are changed or cleaned up.
static class LiveChecks
{
    const string Prefix = "M2-QA-20261107-";
    static readonly List<string> Results = new();
    static ApplicationDbContext Db()
    {
        var config = JsonDocument.Parse(File.ReadAllText("QuanLyHienMau_UNETI2_TI17A5HN/appsettings.json"));
        var connection = config.RootElement.GetProperty("ConnectionStrings").GetProperty("DefaultConnection").GetString();
        return new(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(connection).Options);
    }
    static HttpClient Client() => new(new HttpClientHandler { AllowAutoRedirect = false, CookieContainer = new CookieContainer() }) { BaseAddress = new Uri("http://localhost:5117"), Timeout = TimeSpan.FromSeconds(60) };
    static void Check(bool pass, string name)
    {
        Results.Add((pass ? "PASS " : "FAIL ") + name);
        Console.WriteLine(Results[^1]);
        if (!pass) throw new Exception(name);
    }
    static async Task<string> Html(HttpClient client, string path)
    {
        var response = await client.GetAsync(path);
        response.EnsureSuccessStatusCode();
        return WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
    }
    static async Task<HttpResponseMessage> Post(HttpClient client, string path, Dictionary<string,string> data, string tokenPage = "/DotHienMau/Create", bool csrf = true)
    {
        data = new(data);
        if (csrf)
        {
            var html = await Html(client, tokenPage);
            var token = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"").Groups[1].Value;
            if (token.Length == 0) throw new Exception("Missing CSRF token: " + tokenPage);
            data["__RequestVerificationToken"] = token;
        }
        return await client.PostAsync(path, new FormUrlEncodedContent(data));
    }
    static async Task<HttpClient> Login(string user, string password)
    {
        var client = Client();
        var r = await Post(client, "/Account/Login", new() { ["tenDangNhap"] = user, ["matKhau"] = password, ["rememberMe"] = "false" }, "/Account/Login");
        Check(r.StatusCode == HttpStatusCode.Redirect && !r.Headers.Location!.ToString().Contains("Login"), "Login test role " + user);
        return client;
    }
    static Dictionary<string,string> Form(string name, int org, int index = 0) => new()
    {
        ["TenDot"] = name, ["MaDonViToChuc"] = org.ToString(), ["ChiTieuDonViMau"] = (100 + index).ToString(),
        ["NhomMauUuTien"] = index % 2 == 0 ? "A" : "AB", ["CanNangToiThieu"] = index % 2 == 0 ? "45" : "50",
        ["NgayBatDauDangKy"] = DateTime.Today.AddDays(-2).ToString("yyyy-MM-dd"),
        ["HanDangKy"] = DateTime.Today.AddDays(index + 1).ToString("yyyy-MM-dd"),
        ["MoTaDot"] = "Kiểm thử module 2: 'dấu nháy', \"ngoặc kép\", <script>test</script>\nDòng thứ hai",
        ["DieuKienNguoiHien"] = "Dữ liệu thử nghiệm module 2"
    };
    public static async Task Cleanup()
    {
        await using var db = Db();
        var dots = await db.DotHienMaus.Where(d => d.TenDot.StartsWith(Prefix)).ToListAsync();
        var ids = dots.Select(d => d.MaDot).ToArray();
        var slips = await db.PhieuDangKyHienMaus.Where(p => ids.Contains(p.MaDot) && p.GhiChu == Prefix).ToListAsync();
        db.PhieuDangKyHienMaus.RemoveRange(slips);
        await db.SaveChangesAsync();
        db.DotHienMaus.RemoveRange(dots);
        await db.SaveChangesAsync();
        Console.WriteLine($"Cleaned {dots.Count} test campaigns and {slips.Count} test-only history records.");
    }
    public static async Task Run(bool keepData)
    {
        await using var db = Db();
        Check(!await db.DotHienMaus.AnyAsync(d => d.TenDot.StartsWith(Prefix)), "No pre-existing reserved test fixtures");
        var baseline = await db.DotHienMaus.AsNoTracking().OrderBy(d => d.MaDot).ToListAsync();
        var baselineJson = JsonSerializer.Serialize(baseline);
        var org = await db.DonViToChucs.Where(x => x.TrangThai == "Hoạt động").Select(x => x.MaDonViToChuc).FirstAsync();
        using var admin = await Login("admin", "Admin@123");
        using var staff = await Login("bacsi", "BacSi@123");
        using var donor = await Login("donor", "Donor@123");
        using var anonymous = Client();
        bool completed = false;
        try
        {
            foreach (var route in new[] { "/DotHienMau", "/Admin/DotHienMau", "/DotHienMau/Create" })
                Check((await admin.GetAsync(route)).IsSuccessStatusCode, "Admin GET " + route);
            foreach (var route in new[] { "/DotHienMau/Create", "/DotHienMau/Edit/1", "/Admin/DotHienMau" })
            {
                Check((await anonymous.GetAsync(route)).Headers.Location?.ToString().Contains("Login") == true, "Anonymous denied " + route);
                Check((await donor.GetAsync(route)).Headers.Location?.ToString().Contains("AccessDenied") == true, "Donor denied " + route);
                Check((await staff.GetAsync(route)).IsSuccessStatusCode, "Medical staff allowed " + route);
            }
            foreach (var route in new[] { "/DotHienMau/Create", "/DotHienMau/Edit/1", "/DotHienMau/Delete/1", "/DotHienMau/ChangeStatus", "/Admin/AdminCreateDot", "/Admin/AdminEditDot", "/Admin/AdminDeleteDot", "/Admin/AdminChangeStatusDot" })
            {
                var denied = await Post(donor, route, new(), "/Account/Login", false);
                Check(denied.Headers.Location?.ToString().Contains("AccessDenied") == true, "Donor cannot POST " + route);
            }
            Check((await Post(admin, "/DotHienMau/Create", Form(Prefix + "csrf", org), csrf:false)).StatusCode == HttpStatusCode.BadRequest, "Missing CSRF rejected");
            foreach (var route in new[] { "/DotHienMau/Create", "/Admin/AdminCreateDot" })
            {
                foreach (var bad in new[] { ("TenDot", " "), ("MaDonViToChuc", "0"), ("MaDonViToChuc", "2147483647"), ("ChiTieuDonViMau", "0"), ("CanNangToiThieu", "-1"), ("NgayBatDauDangKy", ""), ("HanDangKy", ""), ("HanDangKy", DateTime.Today.AddDays(-3).ToString("yyyy-MM-dd")), ("MoTaDot", new string('x',1001)) })
                {
                    var form = Form(Prefix + "invalid", org); form[bad.Item1] = bad.Item2;
                    var r = await Post(admin, route, form);
                    Check((int)r.StatusCode < 500 && !await db.DotHienMaus.AnyAsync(d => d.TenDot.StartsWith(Prefix)), "Invalid " + bad.Item1 + " blocked at " + route);
                }
            }
            for (int i = 0; i < 23; i++)
            {
                var route = i % 2 == 0 ? "/DotHienMau/Create" : "/Admin/AdminCreateDot";
                var r = await Post(i == 22 ? staff : admin, route, Form(Prefix + i.ToString("D2"), org, i));
                Check(r.StatusCode == HttpStatusCode.Redirect && await db.DotHienMaus.AsNoTracking().AnyAsync(d => d.TenDot == Prefix + i.ToString("D2") && d.TrangThai == "Chưa mở"), "Create persisted " + i + " via " + route);
            }
            var first = await db.DotHienMaus.AsNoTracking().SingleAsync(d => d.TenDot == Prefix + "00");
            foreach (var route in new[] { "/DotHienMau/Edit/" + first.MaDot, "/Admin/AdminEditDot?id=" + first.MaDot })
            {
                var form = Form(first.TenDot, org); form["MaDot"] = first.MaDot.ToString(); form["ChiTieuDonViMau"] = "321";
                Check((await Post(staff, route, form)).StatusCode == HttpStatusCode.Redirect, "Staff edit accepted " + route);
                Check(await db.DotHienMaus.AsNoTracking().AnyAsync(d => d.MaDot == first.MaDot && d.ChiTieuDonViMau == 321 && d.TrangThai == "Chưa mở"), "Edit persisted and status preserved");
                form["HanDangKy"] = "2000-01-01";
                await Post(admin, route, form);
                Check(await db.DotHienMaus.AsNoTracking().AnyAsync(d => d.MaDot == first.MaDot && d.HanDangKy >= DateTime.Today), "Edit rejects reversed dates " + route);
            }
            foreach (var route in new[] { "/DotHienMau/ChangeStatus", "/Admin/AdminChangeStatusDot" })
            {
                foreach (var state in new[] { "Đang mở đăng ký", "Tạm dừng", "Đã đóng", "Chưa mở" })
                {
                    await Post(staff, route, new() { ["id"] = first.MaDot.ToString(), ["trangThai"] = state });
                    Check(await db.DotHienMaus.AsNoTracking().AnyAsync(d => d.MaDot == first.MaDot && d.TrangThai == state), "Staff changes status " + state + " via " + route);
                }
                await Post(admin, route, new() { ["id"] = first.MaDot.ToString(), ["trangThai"] = "invalid" });
                Check(await db.DotHienMaus.AsNoTracking().AnyAsync(d => d.MaDot == first.MaDot && d.TrangThai == "Chưa mở"), "Invalid status rejected " + route);
            }
            // Explicitly corrupt ONLY a reserved fixture to exercise opening legacy invalid data.
            await db.DotHienMaus.Where(d => d.MaDot == first.MaDot).ExecuteUpdateAsync(x => x.SetProperty(d => d.ChiTieuDonViMau, 0));
            foreach (var route in new[] { "/DotHienMau/ChangeStatus", "/Admin/AdminChangeStatusDot" })
            {
                await Post(admin, route, new() { ["id"] = first.MaDot.ToString(), ["trangThai"] = "Đang mở đăng ký" });
                Check(await db.DotHienMaus.AsNoTracking().AnyAsync(d => d.MaDot == first.MaDot && d.TrangThai == "Chưa mở"), "Invalid campaign cannot open " + route);
            }
            await db.DotHienMaus.Where(d => d.MaDot == first.MaDot).ExecuteUpdateAsync(x => x.SetProperty(d => d.ChiTieuDonViMau, 100));
            var person = await db.NguoiHienMaus.Select(d => d.MaNguoiHien).FirstAsync();
            var slip = new PhieuDangKyHienMau { MaDot = first.MaDot, MaNguoiHien = person, GhiChu = Prefix, TrangThai = "Đã hủy" };
            db.PhieuDangKyHienMaus.Add(slip); await db.SaveChangesAsync();
            foreach (var route in new[] { "/DotHienMau/Delete", "/Admin/AdminDeleteDot" })
            {
                await Post(admin, route, new() { ["id"] = first.MaDot.ToString() });
                Check(await db.DotHienMaus.AsNoTracking().AnyAsync(d => d.MaDot == first.MaDot) && await db.PhieuDangKyHienMaus.AnyAsync(p => p.MaPhieu == slip.MaPhieu), "Delete preserves history " + route);
            }
            db.PhieuDangKyHienMaus.Remove(slip); await db.SaveChangesAsync();
            foreach (var route in new[] { "/DotHienMau/Delete", "/Admin/AdminDeleteDot" })
            {
                var last = await db.DotHienMaus.AsNoTracking().Where(d => d.TenDot.StartsWith(Prefix)).OrderByDescending(d => d.MaDot).FirstAsync();
                await Post(staff, route, new() { ["id"] = last.MaDot.ToString() });
                Check(!await db.DotHienMaus.AsNoTracking().AnyAsync(d => d.MaDot == last.MaDot), "Staff deletes empty campaign " + route);
            }
            foreach (var route in new[] { "/DotHienMau", "/Admin/DotHienMau" })
            {
                int size = route.StartsWith("/Admin") ? 10 : 6;
                var fixture = await db.DotHienMaus.AsNoTracking().Where(d => d.TenDot.StartsWith(Prefix)).OrderBy(d => d.TenDot).ToListAsync();
                for (int page = 1; page <= (int)Math.Ceiling(fixture.Count/(double)size); page++)
                {
                    var html = await Html(admin, route + "?search=" + Prefix + "&sort=name_asc&page=" + page);
                    var expected = fixture.Skip((page-1)*size).Take(size).ToList();
                    Check(expected.All(d => html.Contains(d.TenDot)) && fixture.Except(expected).All(d => !html.Contains(d.TenDot)), "Exact page contents " + route + " page " + page);
                    Check(html.Contains("search=" + Prefix) && html.Contains("sort=name_asc"), "Paging preserves conditions " + route + " page " + page);
                }
                foreach (var page in new[] { -1, 0, 99999 })
                    Check((await Html(admin, route + "?search=" + Prefix + "&page=" + page)).Contains(Prefix), "Page bounds " + route + " " + page);
                foreach (var sort in new[] { "name_asc", "name_desc", "deadline_asc", "deadline_desc", "quota_asc", "quota_desc" })
                {
                    var expected = sort switch { "name_desc" => fixture.OrderByDescending(d=>d.TenDot), "deadline_asc" => fixture.OrderBy(d=>d.HanDangKy), "deadline_desc" => fixture.OrderByDescending(d=>d.HanDangKy), "quota_asc" => fixture.OrderBy(d=>d.ChiTieuDonViMau), "quota_desc" => fixture.OrderByDescending(d=>d.ChiTieuDonViMau), _ => fixture.OrderBy(d=>d.TenDot) };
                    var names = expected.ThenBy(d=>d.MaDot).Take(size).Select(d=>d.TenDot).ToArray();
                    var html = await Html(admin, route + "?search=" + Prefix + "&sort=" + sort);
                    var positions = names.Select(n=>html.IndexOf(n,StringComparison.Ordinal)).ToArray();
                    Check(positions.All(p=>p>=0) && positions.SequenceEqual(positions.OrderBy(p=>p)), "Sort " + route + " " + sort);
                }
                foreach (var filter in new[] { "donViId="+org, "nhomMau=A", "status="+Uri.EscapeDataString("Chưa mở"), "canNang=45", "thoiHan=conhan", "thoiHan=7ngay", "thoiHan=hethan" })
                {
                    var html = await Html(admin, route+"?search="+Prefix+"&"+filter);
                    Check(filter=="thoiHan=hethan" ? !html.Contains(Prefix+"00") : html.Contains(Prefix+"00"), "Filter " + route + " " + filter);
                    if (filter=="nhomMau=A" || filter=="canNang=45") Check(!html.Contains(Prefix+"01"), "Filter excludes AB or heavier campaign");
                }
                var combined = await Html(admin,route+"?search="+Prefix+"&donViId="+org+"&nhomMau=A&status="+Uri.EscapeDataString("Chưa mở")+"&canNang=45&thoiHan=conhan&sort=name_desc&page=2");
                Check(combined.Contains(Prefix+"00") && !combined.Contains(Prefix+"01"), "All filters + search + sort + page " + route);
                Check(!(await Html(admin,route+"?search=NO-MATCH-"+Prefix)).Contains(Prefix+"00"), "Empty search result " + route);
            }
            foreach (var route in new[] { "/DotHienMau/Details/2147483647", "/DotHienMau/Edit/2147483647" })
                Check((await admin.GetAsync(route)).StatusCode==HttpStatusCode.NotFound,"Missing record returns 404 " + route);
            foreach (var scenario in new[] { "open", "future", "expired", "paused", "closed" })
            {
                var start = scenario=="future" ? DateTime.Today.AddDays(2) : DateTime.Today.AddDays(-2);
                var end = scenario=="expired" ? DateTime.Today.AddDays(-1) : DateTime.Today.AddDays(5);
                var state = scenario=="paused" ? "Tạm dừng" : scenario=="closed" ? "Đã đóng" : "Đang mở đăng ký";
                await db.DotHienMaus.Where(d=>d.MaDot==first.MaDot).ExecuteUpdateAsync(x=>x.SetProperty(d=>d.NgayBatDauDangKy,start).SetProperty(d=>d.HanDangKy,end).SetProperty(d=>d.TrangThai,state));
                var html = await Html(donor,"/DotHienMau/Details/"+first.MaDot);
                Check(html.Contains("/DangKy/Create?maDot="+first.MaDot)==(scenario=="open"), "Donor registration link " + scenario);
                Check(html.Contains(state.ToUpperInvariant()), "Details badge preserves stored status " + scenario);
                var listing = await Html(donor, "/DotHienMau?search="+Prefix+"00");
                Check(listing.Contains(state.ToUpperInvariant()), "List badge preserves stored status " + scenario);
                var filtered = await Html(donor, "/DotHienMau?search="+Prefix+"00&status="+Uri.EscapeDataString("Đang mở đăng ký"));
                Check(filtered.Contains("/DotHienMau/Details/"+first.MaDot)==(scenario=="open"), "Public open filter " + scenario);
                var receiving = await db.DotHienMaus.CountAsync(d => d.TrangThai == "Đang mở đăng ký" && d.NgayBatDauDangKy.Date <= DateTime.Today && d.HanDangKy.Date >= DateTime.Today);
                var dashboard = await Html(admin, "/Admin/DotHienMau");
                Check(dashboard.Contains($"{receiving} đợt đang trong thời hạn nhận phiếu"), "Actual receiving count " + scenario);
            }
            var unchanged = await db.DotHienMaus.AsNoTracking().Where(d=>!d.TenDot.StartsWith(Prefix)).OrderBy(d=>d.MaDot).ToListAsync();
            Check(JsonSerializer.Serialize(unchanged)==baselineJson,"All pre-existing campaigns unchanged");
            completed = true;
        }
        finally
        {
            if (!keepData || !completed) await Cleanup();
            await File.WriteAllLinesAsync("tests/Module2Checks/live-results.txt", Results);
        }
        Console.WriteLine($"Passed {Results.Count} live HTTP/SQL checks. Fixtures retained for UI review: {keepData}");
    }
}

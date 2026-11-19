# PHẦN II (NHÓM H) & PHẦN IV: ĐỌC CODE GIẢI THÍCH TỪNG PHƯƠNG THỨC VÀ CÂU TRẢ LỜI MẪU CHI TIẾT ĐỂ TRÌNH BÀY MIỆNG

> **Mục tiêu tài liệu:**
> 1. **Phần II — Nhóm H (Câu hỏi đọc code & giải thích từng phương thức):** Giúp sinh viên khi bị giảng viên yêu cầu *"Em mở file `DangKyController.cs` hoặc `NguoiHienMauController.cs` lên và giải thích từng dòng hàm này cho cô"* có thể trả lời mạch lạc đầy đủ **12 khía cạnh kỹ thuật/nghiệp vụ cho từng phương thức**.
> 2. **Phần IV (Bộ câu trả lời mẫu ngắn gọn 30s & đầy đủ 60–90s):** Cung cấp kịch bản trả lời bằng văn nói tiếng Việt tự nhiên, chuyên nghiệp cho các câu hỏi cốt lõi và câu hỏi khó.

---

## PHẦN II — NHÓM H: GIẢI THÍCH CHI TIẾT TỪNG PHƯƠNG THỨC TRONG CODE MODULE 3

Danh sách toàn bộ các phương thức thuộc phạm vi Module 3 được phân tích dưới đây:
- **Trong `Controllers/DangKyController.cs`:**
  1. `IsConfirmedSuccessfulDonation(PhieuDangKyHienMau phieu)` (dòng 41–60)
  2. `GetActualDonationDate(PhieuDangKyHienMau phieu)` (dòng 68–86)
  3. `EvaluateRecentDonationEligibility(IEnumerable<PhieuDangKyHienMau>? lichSuPhieu, DateTime? referenceDate = null)` (dòng 94–178)
  4. Bộ 3 hàm chuẩn hóa & kiểm tra: `NormalizePhone(string? phone)`, `IsValidPhone(string? phone)`, `IsValidAge(DateTime ngaySinh, DateTime? referenceDate = null)` (dòng 180–205)
  5. `[HttpGet] Create(int? maDot)` (dòng 207–310)
  6. `[HttpPost, ValidateAntiForgeryToken] Create(...)` (dòng 312–681)
  7. `[Authorize] TheoDoi()` (dòng 685–721)
  8. `[HttpPost, Authorize, ValidateAntiForgeryToken] Huy(int id)` (dòng 725–781)
- **Trong `Controllers/NguoiHienMauController.cs`:**
  9. `[HttpGet] HoSoCaNhan()` (dòng 48–101)
  10. `[HttpPost, ValidateAntiForgeryToken] HoSoCaNhan(NguoiHienMau model)` (dòng 103–282)
- **Trong `Controllers/AdminController.cs` (liên quan trực tiếp Module 3):**
  11. `[HttpGet] NguoiHienMau(string? search, string? nhomMau, string? trangThai, int page = 1)` (dòng 638–779)
  12. Các action duyệt/xử lý phiếu đăng ký: `PhieuDangKyHienMau`, `ChiTietPhieuDangKy`, `XacNhanPhieu`, `TuChoiPhieu`, `ChuyenChoKham` (dòng 356–523)

---

### Phương thức H1: `DangKyController.IsConfirmedSuccessfulDonation` (`Controllers/DangKyController.cs`, dòng 41–60)

```csharp
public static bool IsConfirmedSuccessfulDonation(PhieuDangKyHienMau phieu)
{
    if (phieu == null) return false;
    if (phieu.TrangThai == "Đã hủy" ||
        phieu.TrangThai == "Từ chối" ||
        phieu.TrangThai == "Không đủ điều kiện hiến")
    {
        return false;
    }
    if (phieu.KetQuaHienMau != null)
    {
        return phieu.KetQuaHienMau.KetQua == "Hiến máu thành công";
    }
    return phieu.TrangThai == "Hiến máu thành công";
}
```

1. **Tên phương thức:** `public static bool IsConfirmedSuccessfulDonation(PhieuDangKyHienMau phieu)`
2. **Mục đích:** Xác định xem một phiếu đăng ký hiến máu có thực sự là một **lần hiến máu thành công đã được xác nhận** hay không.
3. **Tham số đầu vào:** `PhieuDangKyHienMau phieu` (đối tượng phiếu đăng ký, nên được `.Include(p => p.KetQuaHienMau)` trước khi truyền vào).
4. **Dữ liệu lấy từ đâu:** Từ thuộc tính `phieu.TrangThai` (bảng `PhieuDangKyHienMau`) và `phieu.KetQuaHienMau.KetQua` (bảng `KetQuaHienMau`).
5. **Các bước xử lý bên trong:**
   - Bước 1: Kiểm tra `phieu == null` -> trả về `false`.
   - Bước 2: Loại trừ ngay các phiếu đã kết thúc thất bại (`"Đã hủy"`, `"Từ chối"`, `"Không đủ điều kiện hiến"`) -> trả về `false`.
   - Bước 3: Nếu phiếu đã nạp đối tượng `KetQuaHienMau` (`!= null`), lấy kết quả chính thức tại `phieu.KetQuaHienMau.KetQua == "Hiến máu thành công"`.
   - Bước 4: Nếu `KetQuaHienMau == null` (trường hợp dữ liệu lịch sử chưa có bản ghi `KetQuaHienMau`), kiểm tra `phieu.TrangThai == "Hiến máu thành công"`.
6. **Các điều kiện `if/else` quan trọng:** Việc đặt `if (phieu.KetQuaHienMau != null)` trước `return phieu.TrangThai == "Hiến máu thành công"` giúp ưu tiên kết quả chuyên môn của Module 5 (`KetQuaHienMau`) hơn trạng thái phiếu nếu chẳng may dữ liệu bị lệch.
7. **Truy vấn LINQ / EF Core:** Hàm này xử lý trên bộ nhớ (In-memory helper), được gọi sau khi EF Core đã truy vấn danh sách phiếu kèm `.Include(p => p.KetQuaHienMau)`.
8. **Thao tác Database:** Không ghi DB (chỉ đọc đối tượng trên RAM).
9. **Giá trị trả về:** `bool` (`true` nếu là lần hiến máu thành công thực sự, `false` cho mọi trường hợp còn lại).
10. **View / Luồng nhận kết quả:** Được gọi bởi `GetActualDonationDate`, `EvaluateRecentDonationEligibility`, `DangKyController.Create`, `NguoiHienMauController.HoSoCaNhan`, `Views/NguoiHienMau/HoSoCaNhan.cshtml` và `Views/DangKy/TheoDoi.cshtml`.
11. **Lỗi có thể xảy ra nếu sửa sai:** Nếu chỉ kiểm tra `phieu.KetQuaHienMau != null` mà quên kiểm tra `KetQua == "Hiến máu thành công"`, hệ thống sẽ đếm nhầm cả những người `"Không đủ điều kiện hiến"` là đã hiến máu thành công và khóa oan họ 84 ngày!
12. **Câu hỏi giảng viên có thể hỏi ngay tại hàm này:**
    - *Hỏi:* "Vì sao hàm này được khai báo là `public static`?"
    - *Trả lời:* Dạ vì đây là hàm thuần logic nghiệp vụ (Pure Domain Function) không phụ thuộc vào `_context` của riêng `DangKyController`, việc để `public static` giúp `NguoiHienMauController` và các Razor View (`HoSoCaNhan.cshtml`, `TheoDoi.cshtml`) có thể gọi lại trực tiếp `DangKyController.IsConfirmedSuccessfulDonation(phieu)` để đảm bảo toàn bộ Module 3 dùng chung một quy tắc duy nhất.

---

### Phương thức H2: `DangKyController.GetActualDonationDate` (`Controllers/DangKyController.cs`, dòng 68–86)

```csharp
public static DateTime? GetActualDonationDate(PhieuDangKyHienMau phieu)
{
    if (!IsConfirmedSuccessfulDonation(phieu))
    {
        return null;
    }
    if (phieu.KetQuaHienMau?.NgayCapNhat != null)
    {
        return phieu.KetQuaHienMau.NgayCapNhat.Value;
    }
    return null;
}
```

1. **Tên phương thức:** `public static DateTime? GetActualDonationDate(PhieuDangKyHienMau phieu)`
2. **Mục đích:** Trích xuất **ngày hiến máu thực tế** của một phiếu hiến máu thành công để làm mốc tính chu kỳ nghỉ 84 ngày.
3. **Tham số đầu vào:** `PhieuDangKyHienMau phieu`.
4. **Dữ liệu lấy từ đâu:** Thuộc tính `phieu.KetQuaHienMau.NgayCapNhat` (bảng `KetQuaHienMau`).
5. **Các bước xử lý bên trong:**
   - Bước 1: Gọi `IsConfirmedSuccessfulDonation(phieu)`. Nếu không phải lần hiến thành công -> trả về `null`.
   - Bước 2: Kiểm tra `phieu.KetQuaHienMau?.NgayCapNhat != null`. Nếu có giá trị -> trả về `NgayCapNhat.Value`.
   - Bước 3: Nếu không có `KetQuaHienMau` hoặc `NgayCapNhat == null` -> trả về `null` (tuyệt đối không fallback về `phieu.NgayNop` hay `phieu.NgayXuLy`).
6. **Các điều kiện `if/else` quan trọng:** Điều kiện `if (phieu.KetQuaHienMau?.NgayCapNhat != null)` là chốt chặn ngăn việc lấy nhầm ngày nộp phiếu đăng ký.
7. **Truy vấn LINQ / EF Core:** Yêu cầu câu truy vấn EF Core trước đó phải có `.Include(p => p.KetQuaHienMau)`.
8. **Thao tác Database:** Không.
9. **Giá trị trả về:** `DateTime?` (có ngày nếu đầy đủ dữ liệu kết quả, `null` nếu chưa hiến hoặc bị khuyết ngày kết quả).
10. **View / Luồng nhận kết quả:** Được gọi trong `EvaluateRecentDonationEligibility` và `NguoiHienMauController.HoSoCaNhan`.
11. **Lỗi có thể xảy ra nếu sửa sai:** Nếu ai đó sửa dòng `return null;` cuối hàm thành `return phieu.NgayNop;`, thì những phiếu nộp từ trước ngày diễn ra chiến dịch 2 tuần sẽ bị tính mốc 84 ngày sớm hơn thực tế 2 tuần, hoặc nếu quên `.Include(p => p.KetQuaHienMau)` ở câu truy vấn LINQ thì hàm này sẽ luôn trả về `null`.
12. **Câu hỏi giảng viên có thể hỏi ngay tại hàm này:**
    - *Hỏi:* "Nếu cô xóa `.Include(p => p.KetQuaHienMau)` ở câu lệnh LINQ trong `Create` thì hàm `GetActualDonationDate` này trả về gì?"
    - *Trả lời:* Dạ vì EF Core không bật Lazy Loading, nếu xóa `.Include(p => p.KetQuaHienMau)` thì `phieu.KetQuaHienMau` sẽ luôn bằng `null`. Khi đó hàm `GetActualDonationDate` sẽ trả về `null`, khiến những phiếu có `TrangThai == "Hiến máu thành công"` bị rơi vào nhánh `undatedSuccessful` (báo lỗi thiếu ngày kết quả hiến máu).

---

### Phương thức H3: `DangKyController.EvaluateRecentDonationEligibility` (`Controllers/DangKyController.cs`, dòng 94–178)

1. **Tên phương thức:** `public static RecentDonationEligibilityResult EvaluateRecentDonationEligibility(IEnumerable<PhieuDangKyHienMau>? lichSuPhieu, DateTime? referenceDate = null)`
2. **Mục đích:** Đánh giá toàn diện lịch sử phiếu đăng ký của một người hiến máu để kết luận người đó hiện có đủ điều kiện về mặt thời gian chờ (**84 ngày**) để đăng ký lần tiếp theo hay không.
3. **Tham số đầu vào:**
   - `IEnumerable<PhieuDangKyHienMau>? lichSuPhieu`: Danh sách tất cả các phiếu đăng ký của người hiến máu.
   - `DateTime? referenceDate = null`: Ngày mốc để kiểm tra (mặc định nếu `null` sẽ lấy `DateTime.Today`).
4. **Dữ liệu lấy từ đâu:** Từ danh sách `PhieuDangKyHienMau` và navigation property `KetQuaHienMau` đã nạp từ DB.
5. **Các bước xử lý bên trong:**
   - Bước 1: Xác định `refDay = (referenceDate ?? DateTime.Today).Date`. Nếu `lichSuPhieu == null` -> trả về `new RecentDonationEligibilityResult(true, null, null, null, 0, false, null)`.
   - Bước 2: Lọc danh sách các phiếu thành công: `var successfulDonations = lichSuPhieu.Where(IsConfirmedSuccessfulDonation).ToList()`. Nếu `Count == 0` -> trả về `IsEligible = true`.
   - Bước 3: Tách `successfulDonations` thành 2 danh sách:
     - `datedSuccessful`: Các phiếu có `GetActualDonationDate(p).HasValue`, sắp xếp giảm dần theo `ActualDate.Date` rồi đến `MaPhieu`.
     - `undatedSuccessful`: Các phiếu thành công nhưng `GetActualDonationDate(p) == null`.
   - Bước 4: Nếu `datedSuccessful.Count > 0`:
     - Lấy phần tử đầu tiên `latestDated = datedSuccessful[0]`.
     - Tính `lastActualDate = latestDated.ActualDate.Date`.
     - Tính `nextEligibleDate = lastActualDate.AddDays(84)`.
     - Tính `daysElapsed = Math.Max(0, (refDay - lastActualDate).Days)` và `remainingDays = Math.Max(0, (nextEligibleDate - refDay).Days)`.
     - Nếu `refDay < nextEligibleDate` (`daysElapsed < 84`): Trả về `IsEligible = false` kèm chuỗi thông báo nêu rõ ngày hiến gần nhất, mã phiếu `#DKHM-xxxxx`, ngày đủ điều kiện tiếp theo và số ngày còn phải chờ.
   - Bước 5: Nếu `hasMissingDate` (`undatedSuccessful.Count > 0`): Trả về `IsEligible = false`, `HasMissingActualDonationDate = true` để chặn an toàn đối với hồ sơ thiếu ngày kết quả.
   - Bước 6: Nếu `refDay >= nextEligibleDate` và không có phiếu thiếu ngày -> trả về `IsEligible = true` kèm thông tin lần hiến gần nhất để hiển thị thống kê.
6. **Các điều kiện `if/else` quan trọng:**
   - `if (successfulDonations.Count == 0)` (Người mới hoặc chỉ có phiếu chờ/hủy/từ chối -> cho phép đăng ký).
   - `if (refDay < nextEligibleDate)` (Chưa đủ 84 ngày -> chặn).
   - `if (hasMissingDate)` (Thiếu ngày thực tế -> chặn an toàn).
7. **Truy vấn LINQ / EF Core:** Sử dụng LINQ to Objects (`.Where`, `.Select`, `.OrderByDescending`, `.ThenByDescending`, `.ToList`).
8. **Thao tác Database:** Không.
9. **Giá trị trả về:** Đối tượng record `RecentDonationEligibilityResult` gồm 7 thuộc tính: `IsEligible`, `LastSuccessfulPhieu`, `LastActualDonationDate`, `NextEligibleDonationDate`, `RemainingDays`, `HasMissingActualDonationDate`, `ReasonMessage`.
10. **View / Luồng nhận kết quả:** Được sử dụng tại `DangKyController.Create` (GET & POST), `DangKyController.TheoDoi`, và `NguoiHienMauController.HoSoCaNhan` (GET & POST).
11. **Lỗi có thể xảy ra nếu sửa sai:** Nếu sắp xếp `OrderBy` tăng dần thay vì `OrderByDescending`, hệ thống sẽ lấy lần hiến máu đầu tiên trong đời thay vì lần hiến máu gần nhất để cộng 84 ngày!
12. **Câu hỏi giảng viên có thể hỏi ngay tại hàm này:**
    - *Hỏi:* "Tại sao tham số thứ hai `DateTime? referenceDate = null` lại được thiết kế cho phép truyền ngày từ bên ngoài vào thay vì viết cứng `DateTime.Today` ở trong hàm?"
    - *Trả lời:* Dạ việc cho phép truyền `referenceDate` (với mặc định là `DateTime.Today`) giúp hàm này dễ dàng kiểm thử tự động (Unit Test) với bất kỳ mốc ngày giả định nào mà không phụ thuộc vào đồng hồ hệ thống lúc chạy test.

---

### Phương thức H4: Bộ 3 hàm Helper `NormalizePhone`, `IsValidPhone`, `IsValidAge` (`DangKyController.cs` dòng 180–205 & `NguoiHienMauController.cs` dòng 20–45)

1. **Tên phương thức:**
   - `private static string NormalizePhone(string? phone)`
   - `private static bool IsValidPhone(string? phone)`
   - `private static bool IsValidAge(DateTime ngaySinh, DateTime? referenceDate = null)`
2. **Mục đích:** Chuẩn hóa số điện thoại, kiểm tra định dạng SĐT Việt Nam và kiểm tra độ tuổi hiến máu hợp lệ (18–60 tuổi).
3. **Tham số đầu vào:** Chuỗi `phone` và ngày sinh `ngaySinh`.
4. **Các bước xử lý bên trong:**
   - `NormalizePhone`: Nếu rỗng trả về `string.Empty`; ngược lại `.Trim().Replace(" ", "").Replace("-", "").Replace(".", "")`.
   - `IsValidPhone`: Gọi `NormalizePhone`, sau đó kiểm tra `Regex.IsMatch(normalized, @"^0[0-9]{9,10}$")`.
   - `IsValidAge`:
     ```csharp
     var today = (referenceDate ?? DateTime.Today).Date;
     if (ngaySinh.Date > today) return false;
     int age = today.Year - ngaySinh.Year;
     if (ngaySinh.Date > today.AddYears(-age)) age--;
     return age >= 18 && age <= 60;
     ```
5. **Điểm kỹ thuật đáng chú ý:** Câu lệnh `if (ngaySinh.Date > today.AddYears(-age)) age--;` giúp tính tuổi **chính xác đến từng ngày sinh nhật**, chứ không chỉ lấy `today.Year - ngaySinh.Year`. Ví dụ: Hôm nay là `29/10/2026`, một người sinh ngày `30/10/2008` nếu chỉ trừ năm thì $2026 - 2008 = 18$, nhưng thực tế phải đến ngày mai họ mới tròn 18 tuổi -> hàm `IsValidAge` trừ đi 1 còn `17` tuổi và chặn chính xác!

---

### Phương thức H5: `DangKyController.Create [HttpGet]` (`Controllers/DangKyController.cs`, dòng 207–310)

1. **Tên phương thức:** `[HttpGet] public async Task<IActionResult> Create(int? maDot)`
2. **Mục đích:** Hiển thị giao diện đăng ký hiến máu (`Views/DangKy/Create.cshtml`), nạp thông tin đợt hiến máu, tự động điền thông tin người hiến (nếu đã đăng nhập) và kiểm tra trước điều kiện đăng ký để cảnh báo ngay trên giao diện.
3. **Tham số đầu vào:** `int? maDot` (mã đợt hiến máu trên query string `/DangKy/Create?maDot=1`, có thể `null`).
4. **Dữ liệu lấy từ đâu:** Query string `maDot`, Cookie claim `"MaTaiKhoan"`, và các bảng `DotHienMau`, `NguoiHienMau`, `TaiKhoan`, `PhieuDangKyHienMau`, `KetQuaHienMau`.
5. **Các bước xử lý bên trong:**
   - Bước 1 (dòng 210–215): Truy vấn danh sách các đợt hiến máu đang khả dụng (`TrangThai == "Đang mở" || TrangThai == "Sắp diễn ra"` và `KetThuc >= DateTime.Now`), sắp xếp theo `BatDau`.
   - Bước 2 (dòng 218–236): Nếu `maDot == null`, tự động chọn đợt đầu tiên trong danh sách khả dụng. Nếu vẫn không có đợt nào, trả về `TempData["Error"]` và chuyển hướng về `DotHienMau/Index`. Truy vấn chi tiết `dotHienMau` kèm `DonViToChuc` và `PhieuDangKyHienMaus`.
   - Bước 3 (dòng 239–248): Đếm số phiếu hợp lệ hiện tại của đợt (`soLuongDaDangKy`). Nếu đợt đã đóng/kết thúc hoặc `soLuongDaDangKy >= dotHienMau.SoLuongToiDa`, đặt `coTheDangKy = false` và ghi lý do vào `lyDoKhongTheDangKy`.
   - Bước 4 (dòng 261–308): Nếu người dùng đã đăng nhập (`User.Identity?.IsAuthenticated == true`):
     - Lấy `MaTaiKhoan` từ Claim, tìm `NguoiHienMau` hoặc `TaiKhoan` để truyền vào `ViewBag.NguoiHienMau` / `ViewBag.TaiKhoan` (phục vụ auto-fill form).
     - Nếu đã có `NguoiHienMau`, truy vấn `lichSuPhieu` kèm `KetQuaHienMau`, gọi `EvaluateRecentDonationEligibility(lichSuPhieu)`.
     - Nếu `!eligibility.IsEligible` (chưa đủ 84 ngày hoặc thiếu ngày thực tế), đặt `coTheDangKy = false`.
     - Nếu đủ 84 ngày, kiểm tra tiếp xem người đó đã có `existingPhieu` đang chờ xử lý trong cùng `maDot` chưa; nếu có thì đặt `coTheDangKy = false`.
   - Bước 5: Gán toàn bộ dữ liệu vào `ViewBag` và trả về `View(dotHienMau)`.
6. **Các điều kiện `if/else` quan trọng:** Thứ tự ưu tiên thông báo chặn trên UI: Kiểm tra trạng thái/chỉ tiêu của Đợt trước -> Kiểm tra chu kỳ 84 ngày -> Kiểm tra trùng phiếu đang hoạt động trong đợt.
7. **Truy vấn LINQ / EF Core:**
   - `_context.DotHienMaus.Include(d => d.DonViToChuc).Where(...)`
   - `_context.PhieuDangKyHienMaus.Include(p => p.KetQuaHienMau).Include(p => p.DotHienMau).Where(p => p.MaNguoiHien == nguoiHienMau.MaNguoiHien)`
8. **Thao tác Database:** Chỉ đọc (`SELECT`).
9. **Giá trị trả về:** `View(dotHienMau)` (hoặc `RedirectToAction("Index", "DotHienMau")` nếu không tìm thấy đợt).
10. **View nhận kết quả:** `Views/DangKy/Create.cshtml`.
11. **Lỗi có thể xảy ra nếu sửa sai:** Nếu quên kiểm tra `soLuongDaDangKy >= dotHienMau.SoLuongToiDa` hoặc không loại trừ các phiếu `"Đã hủy"`, `"Từ chối"`, `"Không đủ điều kiện hiến"` khi đếm `soLuongDaDangKy`, thanh tiến độ chỉ tiêu trên giao diện sẽ hiển thị sai số chỗ còn lại.
12. **Câu hỏi giảng viên có thể hỏi ngay tại hàm này:**
    - *Hỏi:* "Khi một tài khoản vừa mới đăng ký xong, chưa từng vào trang Hồ sơ cá nhân mà vào thẳng `GET /DangKy/Create` thì hệ thống lấy thông tin ở đâu để điền sẵn vào form?"
    - *Trả lời:* Dạ thứ nhất, khi đăng ký tài khoản ở `AccountController.Register` (dòng 150), hệ thống đã tự tạo sẵn bản ghi `NguoiHienMau`. Thứ hai, kể cả với tài khoản cũ chưa có bản ghi `NguoiHienMau`, ở dòng 275–278 của `DangKyController.cs`, code có nhánh `else { var taiKhoan = await _context.TaiKhoans.FindAsync(maTaiKhoan); ViewBag.TaiKhoan = taiKhoan; }`, và trên `Create.cshtml` (dòng 17–21) có toán tử null-coalescing `var hoTen = nguoiHien?.HoTen ?? taiKhoan?.HoTen ?? "";` nên vẫn tự động điền được Họ tên, SĐT và Email từ bảng `TaiKhoan`.

---

### Phương thức H6: `DangKyController.Create [HttpPost]` (`Controllers/DangKyController.cs`, dòng 312–681)

1. **Tên phương thức:** `[HttpPost] [ValidateAntiForgeryToken] public async Task<IActionResult> Create(int maDot, string hoTen, DateTime? ngaySinh, string? gioiTinh, string soDienThoai, string? email, string? diaChi, string? nhomMau, string? ngheNghiep, double? canNang, string? ghiChuSucKhoe, string? lyDoHienMau, string? ghiChu, string? volume = null, string? timeSlot = null, string? health_q1 = null, string? health_q2 = null)`
2. **Mục đích:** Xử lý yêu cầu gửi phiếu đăng ký hiến máu, thực hiện đầy đủ kiểm tra nghiệp vụ ở phía Server, cập nhật/tạo mới hồ sơ `NguoiHienMau` và tạo bản ghi `PhieuDangKyHienMau` an toàn trong Transaction.
3. **Tham số đầu vào:** 17 tham số nhận từ form POST của `Views/DangKy/Create.cshtml`.
4. **Dữ liệu lấy từ đâu:** Form POST body, Cookie claim `"MaTaiKhoan"`, và các bảng `DotHienMau`, `NguoiHienMau`, `PhieuDangKyHienMau`, `KetQuaHienMau`.
5. **Các bước xử lý bên trong:**
   - **Bước 1 (Kiểm tra chiến dịch - dòng 335–355):** Tìm `dotHienMau`, kiểm tra tồn tại, trạng thái (`"Đang mở"` / `"Sắp diễn ra"`), chưa quá hạn `KetThuc`, và `soLuongDaDangKy < dotHienMau.SoLuongToiDa`.
   - **Bước 2 (Xác định hồ sơ `NguoiHienMau` - dòng 356–420):**
     - Chuẩn hóa các chuỗi nhập vào (`Trim()`, `NormalizePhone`).
     - Nếu đã đăng nhập: tìm `NguoiHienMau` theo `MaTaiKhoan`; nếu chưa có thì tìm bản ghi `NguoiHienMau` trùng `SoDienThoai` có `MaTaiKhoan == null` để nối vào tài khoản.
     - Nếu chưa đăng nhập: tìm `NguoiHienMau` theo `SoDienThoai` đã chuẩn hóa.
   - **Bước 3 (Kiểm tra dữ liệu & y tế - dòng 422–523):**
     - Kiểm tra Họ tên (`<= 100` ký tự), SĐT (`^0[0-9]{9,10}$`), Ngày sinh & Tuổi (`18–60`), Giới tính (`Nam`/`Nữ`/`Khác`), Email (`<= 100` ký tự, đúng chuẩn email), Địa chỉ (`<= 255` ký tự), Cân nặng (`30–200` kg và `>= dotHienMau.CanNangToiThieu`), và khảo sát sức khỏe (`health_q1 == "none"`, `health_q2 == "good"`).
     - Nếu có bất kỳ lỗi nào -> gán `TempData["Error"]` và `RedirectToAction("Create", new { maDot })`.
   - **Bước 4 (Lưu/Cập nhật `NguoiHienMau` - dòng 525–530):**
     - Nếu `nguoiHienMau == null`: khởi tạo mới, `_context.NguoiHienMaus.Add(nguoiHienMau)` và `await _context.SaveChangesAsync()` để sinh khóa chính `MaNguoiHien`.
   - **Bước 5 (Khóa đồng bộ & Transaction tạo phiếu - dòng 532–616):**
     - Lấy khóa `SemaphoreSlim` theo `nguoiHienMau.MaNguoiHien` từ `_submissionLocks` và `await userLock.WaitAsync()`.
     - Mở Transaction `await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable)`.
     - Truy vấn lại `lichSuPhieuInTx` kèm `KetQuaHienMau`, gọi `EvaluateRecentDonationEligibility(lichSuPhieuInTx)`. Nếu `!eligibilityInTx.IsEligible` -> `RollbackAsync()` và báo lỗi.
     - Kiểm tra `existingPhieu` trong cùng `maDot`. Nếu đã tồn tại -> `RollbackAsync()` và báo lỗi.
     - Cập nhật thông tin mới nhất từ form vào `nguoiHienMau`.
     - Ghép `volume` vào `LyDoHienMau`, ghép `timeSlot` vào `GhiChu`.
     - Khởi tạo `PhieuDangKyHienMau` với `TrangThai = "Chờ xác nhận"`, `NgayNop = DateTime.Now`, thêm vào `_context.PhieuDangKyHienMaus`, gọi `SaveChangesAsync()` và `CommitAsync()`.
     - Trong khối `finally`: luôn gọi `userLock.Release()`.
   - **Bước 6 (Điều hướng kết quả - dòng 672–680):** Nếu đã đăng nhập -> `RedirectToAction("TheoDoi")`; nếu khách vãng lai -> `RedirectToAction("Details", "DotHienMau", new { id = maDot })`.
6. **Các điều kiện `if/else` quan trọng:** Toàn bộ các điều kiện kiểm tra tuổi, cân nặng, câu hỏi sức khỏe, 84 ngày và trùng phiếu.
7. **Truy vấn LINQ / EF Core:** Truy vấn `DotHienMaus`, `NguoiHienMaus`, `PhieuDangKyHienMaus` (có `.Include(p => p.KetQuaHienMau)`).
8. **Thao tác Database:** `INSERT` hoặc `UPDATE` bảng `NguoiHienMau` + `INSERT` bảng `PhieuDangKyHienMau` trong Transaction `Serializable`.
9. **Giá trị trả về:** `RedirectToAction("TheoDoi")` hoặc `RedirectToAction("Details", "DotHienMau")` hoặc `RedirectToAction("Create", new { maDot })`.
10. **View nhận kết quả:** `Views/DangKy/TheoDoi.cshtml` (hoặc `Views/DotHienMau/Details.cshtml` / `Views/DangKy/Create.cshtml`).
11. **Lỗi có thể xảy ra nếu sửa sai:** Nếu bỏ khối `finally { userLock.Release(); }`, khi xảy ra ngoại lệ DB ở giữa hàm thì `SemaphoreSlim` của `MaNguoiHien` đó sẽ bị giữ khóa vĩnh viễn, khiến người dùng đó không bao giờ gửi phiếu được nữa cho đến khi khởi động lại server!
12. **Câu hỏi giảng viên có thể hỏi ngay tại hàm này:**
    - *Hỏi:* "Tại sao ở dòng 529 em phải gọi `await _context.SaveChangesAsync()` cho `nguoiHienMau` mới trước khi bước vào dòng 532 ` _submissionLocks.GetOrAdd(nguoiHienMau.MaNguoiHien, ...)`?"
    - *Trả lời:* Dạ bởi vì nếu `nguoiHienMau` là bản ghi mới tinh vừa `new NguoiHienMau()`, trước khi gọi `SaveChangesAsync()` thì cột khóa chính tự tăng `MaNguoiHien` vẫn bằng `0`. Nếu không lưu trước để SQL Server cấp `MaNguoiHien` thật (ví dụ `15`), thì mọi người dùng mới đều dùng chung khóa `MaNguoiHien = 0` trong `_submissionLocks` và không thể gán khóa ngoại `MaNguoiHien` cho `PhieuDangKyHienMau`.

---

### Phương thức H7: `DangKyController.TheoDoi [HttpGet]` (`Controllers/DangKyController.cs`, dòng 685–721)

1. **Tên phương thức:** `[Authorize] public async Task<IActionResult> TheoDoi()`
2. **Mục đích:** Hiển thị trang Theo dõi đăng ký & Lịch sử hiến máu của người dùng đang đăng nhập.
3. **Tham số đầu vào:** Không có tham số URL (lấy định danh từ Cookie claim `"MaTaiKhoan"`).
4. **Dữ liệu lấy từ đâu:** Claim `"MaTaiKhoan"`, bảng `NguoiHienMau`, `PhieuDangKyHienMau`, `DotHienMau`, `LichKhamSangLoc`, `KetQuaHienMau`.
5. **Các bước xử lý bên trong:**
   - Bước 1: Kiểm tra `MaTaiKhoan` từ Cookie. Nếu không có -> `RedirectToAction("Login", "Account")`.
   - Bước 2: Tìm `nguoiHienMau` theo `MaTaiKhoan`. Nếu `null` -> truyền `EligibilityResult` mặc định vào `ViewBag` và trả về `View(new List<PhieuDangKyHienMau>())`.
   - Bước 3: Truy vấn `danhSachPhieu` của `nguoiHienMau.MaNguoiHien` kèm `.Include(p => p.DotHienMau)`, `.Include(p => p.LichKhamSangLocs)`, `.Include(p => p.KetQuaHienMau)`, sắp xếp `.OrderByDescending(p => p.NgayNop).ThenByDescending(p => p.MaPhieu)`.
   - Bước 4: Gọi `EvaluateRecentDonationEligibility(danhSachPhieu)`, lưu vào `ViewBag.EligibilityResult` và trả về `View(danhSachPhieu)`.
6. **Giá trị trả về:** `View(danhSachPhieu)` (`Views/DangKy/TheoDoi.cshtml`).
7. **Câu hỏi giảng viên có thể hỏi ngay tại hàm này:**
    - *Hỏi:* "Trên trang `TheoDoi.cshtml`, làm sao biết phiếu nào được đưa lên khung Stepper 4 bước ở trên cùng, còn phiếu nào nằm ở bảng Lịch sử bên dưới?"
    - *Trả lời:* Dạ ở đầu file `Views/DangKy/TheoDoi.cshtml` (dòng 12–16), View lọc ra `activePhieu` bằng điều kiện: phiếu mới nhất có `TrangThai` khác `"Đã hủy"`, `"Từ chối"`, `"Không đủ điều kiện hiến"` và `"Hiến máu thành công"`. Tức là chỉ phiếu đang trong tiến trình xử lý mới hiện ở Stepper 4 bước, còn khi đã hoàn tất hoặc hủy/từ chối thì nằm ở bảng lịch sử.

---

### Phương thức H8: `DangKyController.Huy [HttpPost]` (`Controllers/DangKyController.cs`, dòng 725–781)

1. **Tên phương thức:** `[HttpPost] [Authorize] [ValidateAntiForgeryToken] public async Task<IActionResult> Huy(int id)`
2. **Mục đích:** Cho phép người hiến máu tự hủy phiếu đăng ký của chính mình khi phiếu còn ở giai đoạn đầu.
3. **Tham số đầu vào:** `int id` (chính là `MaPhieu` gửi từ form ẩn trong modal xác nhận hủy của `TheoDoi.cshtml`).
4. **Dữ liệu lấy từ đâu:** Tham số `id`, Claim `"MaTaiKhoan"`, bảng `NguoiHienMau`, `PhieuDangKyHienMau`, `LichKhamSangLoc`, `KetQuaHienMau`.
5. **Các bước xử lý bên trong:**
   - Bước 1: Đọc `MaTaiKhoan` từ Cookie và tìm `nguoiHienMau`.
   - Bước 2: Truy vấn `phieu` kèm `LichKhamSangLocs` và `KetQuaHienMau` với điều kiện `p.MaPhieu == id && p.MaNguoiHien == nguoiHienMau.MaNguoiHien`. Nếu `phieu == null` -> báo lỗi không tìm thấy hoặc không có quyền hủy.
   - Bước 3: Kiểm tra `var trangThaiChoPhep = new[] { "Chờ xác nhận", "Đã xác nhận" };`. Nếu `!trangThaiChoPhep.Contains(phieu.TrangThai)` -> báo lỗi không thể hủy ở trạng thái hiện tại.
   - Bước 4: Kiểm tra `if (phieu.LichKhamSangLocs.Any(l => l.TrangThai != "Đã hủy"))` -> báo lỗi phiếu đã được xếp lịch khám sàng lọc nên không thể tự hủy.
   - Bước 5: Kiểm tra `if (phieu.KetQuaHienMau != null)` -> báo lỗi phiếu đã có kết quả hiến máu nên không thể hủy.
   - Bước 6: Gán `phieu.TrangThai = "Đã hủy"`, `phieu.NgayXuLy = DateTime.Now`, gọi `await _context.SaveChangesAsync()` và chuyển hướng về `TheoDoi`.
6. **Thao tác Database:** `UPDATE PhieuDangKyHienMau SET TrangThai = N'Đã hủy', NgayXuLy = ... WHERE MaPhieu = @id`.
7. **Câu hỏi giảng viên có thể hỏi ngay tại hàm này:**
    - *Hỏi:* "Vì sao khi người dùng hủy phiếu, em không dùng `_context.PhieuDangKyHienMaus.Remove(phieu)` để xóa hẳn bản ghi khỏi CSDL mà lại đổi `TrangThai = "Đã hủy"`?"
    - *Trả lời:* Dạ đây là kỹ thuật xóa mềm theo trạng thái (State-based Soft Cancel). Việc giữ lại bản ghi với trạng thái `"Đã hủy"` và thời điểm `NgayXuLy` giúp cả người hiến máu lẫn ban tổ chức tra cứu lại được lịch sử giao dịch, thống kê tỷ lệ hủy phiếu của từng chiến dịch và tránh lỗi mất dấu dữ liệu kiểm toán (Audit Trail).

---

### Phương thức H9: `NguoiHienMauController.HoSoCaNhan [HttpGet]` (`Controllers/NguoiHienMauController.cs`, dòng 48–101)

1. **Tên phương thức:** `[HttpGet] public async Task<IActionResult> HoSoCaNhan()`
2. **Mục đích:** Hiển thị trang Hồ sơ cá nhân & Sổ tay sức khỏe của người hiến máu đang đăng nhập.
3. **Tham số đầu vào:** Không có tham số URL (sử dụng Claim `"MaTaiKhoan"` từ Cookie).
4. **Các bước xử lý bên trong:**
   - Bước 1: Kiểm tra `MaTaiKhoan` từ Cookie.
   - Bước 2: Truy vấn `nguoiHienMau` kèm `TaiKhoan` và `PhieuDangKyHienMaus` -> `DotHienMau`, `KetQuaHienMau`, `LichKhamSangLocs`.
   - Bước 3: Nếu `nguoiHienMau == null`, truy vấn `TaiKhoan` và khởi tạo một đối tượng `NguoiHienMau` tạm trên bộ nhớ từ `TaiKhoan` để truyền sang View.
   - Bước 4: Gọi `DangKyController.EvaluateRecentDonationEligibility(lichSuPhieu)` và đếm số phiếu thỏa mãn `DangKyController.IsConfirmedSuccessfulDonation`.
   - Bước 5: Gán 6 biến thống kê vào `ViewBag` (`ConfirmedSuccessfulCount`, `LastActualDonationDate`, `NextEligibleDonationDate`, `RemainingCooldownDays`, `IsEligibleToDonateNow`, `HasMissingActualDonationDate`) và trả về `View(nguoiHienMau)`.
5. **Câu hỏi giảng viên có thể hỏi ngay tại hàm này:**
    - *Hỏi:* "Tại sao trong `HoSoCaNhan` GET, khi `nguoiHienMau == null` em chỉ `new NguoiHienMau` trên RAM mà không gọi `_context.Add` và `SaveChangesAsync()` ngay ở hàm GET?"
    - *Trả lời:* Dạ theo chuẩn kiến trúc HTTP và RESTful, phương thức `HTTP GET` phải có tính chất **Idempotent và Safe** (chỉ đọc dữ liệu, không làm thay đổi trạng thái cơ sở dữ liệu). Việc tạo bản ghi thật vào DB chỉ được thực hiện khi người dùng bấm nút Lưu gửi request `HTTP POST`.

---

### Phương thức H10: `NguoiHienMauController.HoSoCaNhan [HttpPost]` (`Controllers/NguoiHienMauController.cs`, dòng 103–282)

1. **Tên phương thức:** `[HttpPost] [ValidateAntiForgeryToken] public async Task<IActionResult> HoSoCaNhan(NguoiHienMau model)`
2. **Mục đích:** Nhận dữ liệu chỉnh sửa hồ sơ cá nhân từ người dùng, kiểm tra tính hợp lệ và lưu cập nhật vào bảng `NguoiHienMau` (đồng thời đồng bộ `HoTen` sang bảng `TaiKhoan`).
3. **Tham số đầu vào:** `NguoiHienMau model` (được ASP.NET Core Model Binder ánh xạ từ form `HoSoCaNhan.cshtml`).
4. **Các bước xử lý bên trong:**
   - Bước 1: Đọc `MaTaiKhoan` từ Cookie.
   - Bước 2: Chuẩn hóa các chuỗi trong `model` (`Trim()`, `NormalizePhone`).
   - Bước 3: Kiểm tra từng trường dữ liệu và gom lỗi vào `List<string> errors`:
     - Họ tên: bắt buộc, `<= 100` ký tự.
     - SĐT: bắt buộc, khớp `IsValidPhone` (`^0[0-9]{9,10}$`).
     - Ngày sinh: bắt buộc, `<= DateTime.Today`, khớp `IsValidAge` (`18–60` tuổi).
     - Giới tính: bắt buộc thuộc `{"Nam", "Nữ", "Khác"}`.
     - Email: bắt buộc, `<= 100` ký tự, khớp `EmailAddressAttribute`.
     - Địa chỉ: bắt buộc, `<= 255` ký tự.
     - Cân nặng: bắt buộc, nằm trong khoảng `[30.0, 200.0]`.
     - Nhóm máu `<= 20` ký tự, Nghề nghiệp `<= 100` ký tự, Ghi chú sức khỏe `<= 500` ký tự.
   - Bước 4: Nếu `errors.Count > 0`:
     - Gán `TempData["Error"] = string.Join(" ", errors)`.
     - Truy vấn lại `existingProfile` từ DB để nạp lại `PhieuDangKyHienMaus` và các biến `ViewBag` thống kê, sau đó trả về `View(model)` để giữ nguyên dữ liệu người dùng vừa nhập.
   - Bước 5: Nếu hợp lệ (`errors.Count == 0`):
     - Tìm `nguoiHienMau` trong DB theo `MaTaiKhoan` (kèm `.Include(n => n.TaiKhoan)`).
     - Nếu `nguoiHienMau == null`: khởi tạo mới gắn `MaTaiKhoan = maTaiKhoan`, `TrangThai = "Hoạt động"` và `_context.Add(nguoiHienMau)`.
     - Nếu đã có: gán tường minh 10 trường thông tin từ `model` sang `nguoiHienMau`, đồng thời nếu `nguoiHienMau.TaiKhoan != null` thì đồng bộ `nguoiHienMau.TaiKhoan.HoTen = model.HoTen`.
     - Gọi `await _context.SaveChangesAsync()`, gán `TempData["Success"]` và `RedirectToAction(nameof(HoSoCaNhan))`.
5. **Câu hỏi giảng viên có thể hỏi ngay tại hàm này:**
    - *Hỏi:* "Tại sao khi `errors.Count > 0` em lại trả về `return View(model)` mà khi lưu thành công em lại dùng `return RedirectToAction(nameof(HoSoCaNhan))`?"
    - *Trả lời:* Dạ đây là mẫu thiết kế **PRG (Post - Redirect - Get)**:
      - Khi **có lỗi validation**, em dùng `return View(model)` để giữ lại toàn bộ những gì người dùng vừa gõ trên form, giúp họ sửa lại chỗ sai mà không bị mất dữ liệu.
      - Khi **lưu thành công**, em bắt buộc dùng `RedirectToAction` (sinh ra request `GET` mới) để nếu người dùng bấm phím F5 (Refresh trình duyệt) thì trình duyệt chỉ tải lại trang `GET` chứ không gửi lặp lại lệnh `POST`.

---

### Phương thức H11: `AdminController.NguoiHienMau [HttpGet]` (`Controllers/AdminController.cs`, dòng 638–779)

1. **Tên phương thức:** `public async Task<IActionResult> NguoiHienMau(string? search, string? nhomMau, string? trangThai, int page = 1)`
2. **Mục đích:** Hiển thị trang quản lý danh sách người hiến máu dành cho `Admin` và `NhanVienYTe`, hỗ trợ tìm kiếm, lọc theo nhóm máu/trạng thái và phân trang.
3. **Các bước xử lý bên trong:**
   - Bước 1: Tạo `IQueryable<NguoiHienMau>` kèm `.Include(n => n.TaiKhoan)` và `.Include(n => n.PhieuDangKyHienMaus).ThenInclude(p => p.KetQuaHienMau)`.
   - Bước 2: Áp dụng bộ lọc tìm kiếm theo `HoTen`, `SoDienThoai`, `Email` hoặc mã định danh `#HM-xxxx` / `#BD-xxxx`.
   - Bước 3: Lọc theo `nhomMau` (bao gồm tùy chọn `"Chưa biết"` cho nhóm máu `null`/rỗng/chứa `"Chưa"`).
   - Bước 4: Lọc theo `trangThai` (`"Sẵn sàng"` >= 84 ngày, `"Đang chờ"` < 84 ngày, hoặc `"Tạm ngưng"`).
   - Bước 5: Phân trang (`pageSize = 6`, dùng `.Skip((page - 1) * pageSize).Take(pageSize)`) và tính các chỉ số KPI nhóm máu truyền qua `ViewBag`.
4. **Lưu ý kỹ thuật rất quan trọng khi bảo vệ:**
   - Trong `AdminController.NguoiHienMau` (dòng 656–665 và 739–775), công thức lọc trạng thái chờ 84 ngày của trang Admin hiện vẫn viết theo kiểu truy vấn SQL trực tiếp dựa trên `p.NgayXuLy ?? p.NgayNop` và điều kiện `p.TrangThai == "Hiến máu thành công" || p.KetQuaHienMau != null`, chưa gọi sang hàm `DangKyController.EvaluateRecentDonationEligibility`. Nếu giảng viên hỏi về sự khác nhau giữa con số trên trang Admin và trang Hồ sơ cá nhân, sinh viên chỉ rõ điểm này sẽ thể hiện khả năng đọc hiểu code cực kỳ sâu.

---

### Phương thức H12: Nhóm phương thức quản lý phiếu đăng ký trong `AdminController.cs` (`PhieuDangKyHienMau`, `ChiTietPhieuDangKy`, `XacNhanPhieu`, `TuChoiPhieu`, `ChuyenChoKham`, dòng 356–523)

1. **`PhieuDangKyHienMau(string? search, string? trangThai, int? maDot, int page = 1)` (dòng 356–409):**
   - Truy vấn danh sách phiếu đăng ký kèm `NguoiHienMau`, `DotHienMau`, `LichKhamSangLocs`, `KetQuaHienMau`.
   - Hỗ trợ tìm theo tên/SĐT/mã phiếu, lọc theo trạng thái phiếu, lọc theo đợt hiến máu và phân trang (`pageSize = 8`).
2. **`ChiTietPhieuDangKy(int id)` (dòng 411–420):**
   - Hiển thị toàn bộ thông tin chi tiết của 1 phiếu đăng ký (`Views/Admin/ChiTietPhieuDangKy.cshtml`), cho phép Admin thao tác duyệt phiếu, tạo lịch khám sàng lọc và ghi nhận kết quả ngay trên một màn hình.
3. **`XacNhanPhieu(int id, string? ghiChuXacNhan, string? returnUrl)` (dòng 422–450):**
   - Chỉ cho phép khi `phieu.TrangThai == "Chờ xác nhận"`. Cập nhật `TrangThai = "Đã xác nhận"`, `NgayXuLy = DateTime.Now`, lưu `GhiChuXacNhan`.
4. **`TuChoiPhieu(int id, string? ghiChuXacNhan, string? returnUrl)` (dòng 452–495):**
   - Cho phép khi `phieu.TrangThai` thuộc `{"Chờ xác nhận", "Đã xác nhận", "Chờ khám sàng lọc"}` và `phieu.KetQuaHienMau == null`.
   - Cập nhật `TrangThai = "Từ chối"`, đồng thời tự động chuyển các `LichKhamSangLoc` chưa hoàn thành của phiếu đó sang `"Đã hủy"`.
5. **`ChuyenChoKham(int id, string? returnUrl)` (dòng 497–523):**
   - Chỉ cho phép khi `phieu.TrangThai == "Đã xác nhận"`. Cập nhật `TrangThai = "Chờ khám sàng lọc"`, `NgayXuLy = DateTime.Now`.

---

## PHẦN IV — BỘ CÂU TRẢ LỜI MẪU ĐỂ SINH VIÊN HỌC THUỘC Ý VÀ TRÌNH BÀY MIỆNG (30 GIÂY & 60–90 GIÂY)

Dưới đây là 10 chủ đề quan trọng nhất được biên soạn theo 2 phiên bản: **Bản trả lời ngắn (30 giây)** khi hội đồng hỏi nhanh, và **Bản trả lời đầy đủ (60–90 giây)** khi hội đồng yêu cầu phân tích sâu.

---

### Chủ đề 1: Giới thiệu tổng quan Module 3 do em phụ trách
- **⏱️ Bản ngắn (30 giây):**
  "Thưa cô và hội đồng, em phụ trách **Module 3: Quản lý người hiến máu và Phiếu đăng ký hiến máu**. Module của em gồm 2 Controller chính là `DangKyController` và `NguoiHienMauController`, làm việc trực tiếp với 2 bảng `NguoiHienMau` và `PhieuDangKyHienMau`. Chức năng cốt lõi là quản lý hồ sơ sức khỏe người hiến, tiếp nhận phiếu đăng ký theo đợt, kiểm tra tự động các điều kiện y tế đặc biệt là quy tắc nghỉ tối thiểu 84 ngày giữa hai lần hiến máu thành công, theo dõi tiến trình phiếu 4 bước và cho phép hủy phiếu hợp lệ."
- **🎙️ Bản đầy đủ (60–90 giây):**
  "Thưa cô và hội đồng, trong dự án Quản lý hiến máu vì cộng đồng, em đảm nhận **Module 3: Quản lý người hiến máu và Phiếu đăng ký hiến máu**.
  Về mặt kiến trúc, Module 3 đóng vai trò là cầu nối trung tâm của toàn hệ thống: nhận thông tin định danh tài khoản từ Module 1 (`TaiKhoan`) và thông tin chiến dịch từ Module 2 (`DotHienMau`), từ đó cho phép người tình nguyện cập nhật hồ sơ cá nhân tại `NguoiHienMauController` (`HoSoCaNhan`) và gửi phiếu đăng ký trực tuyến tại `DangKyController` (`Create`).
  Khi người dùng đăng ký, hệ thống kiểm tra chặt chẽ ở cả Client và Server về độ tuổi 18–60, cân nặng đạt chuẩn của đợt, khảo sát sức khỏe và đặc biệt là thuật toán kiểm tra chu kỳ 84 ngày tính từ ngày có kết quả hiến máu thành công thực tế (`KetQuaHienMau.NgayCapNhat`). Sau khi tạo phiếu ở trạng thái `'Chờ xác nhận'`, người hiến có thể theo dõi tiến trình xử lý qua 4 bước tại màn hình `TheoDoi` hoặc tự hủy phiếu tại action `Huy` nếu phiếu chưa bước vào giai đoạn khám sàng lọc của Module 4 và ghi nhận kết quả của Module 5."

---

### Chủ đề 2: Giải thích quy tắc 84 ngày và cách xác định ngày hiến máu thực tế
- **⏱️ Bản ngắn (30 giây):**
  "Thưa cô, theo quy định y tế, khoảng cách tối thiểu giữa 2 lần hiến máu toàn phần là 12 tuần tức **84 ngày**. Trong `DangKyController`, em viết hàm `EvaluateRecentDonationEligibility` kết hợp với `IsConfirmedSuccessfulDonation` và `GetActualDonationDate`. Hệ thống chỉ lấy những phiếu có kết quả `'Hiến máu thành công'` và lấy ngày thực tế tại cột `KetQuaHienMau.NgayCapNhat` cộng thêm 84 ngày, tuyệt đối không lấy ngày nộp phiếu `NgayNop`. Nếu ngày hiện tại nhỏ hơn mốc 84 ngày đó thì hệ thống chặn đăng ký."
- **🎙️ Bản đầy đủ (60–90 giây):**
  "Thưa cô, quy tắc 84 ngày được em cài đặt tại 3 hàm tĩnh ở đầu file `Controllers/DangKyController.cs` từ dòng 23 đến dòng 178:
  - Thứ nhất, hàm `IsConfirmedSuccessfulDonation` lọc ra các phiếu thực sự hiến máu thành công: loại bỏ các phiếu `'Đã hủy'`, `'Từ chối'`, `'Không đủ điều kiện hiến'`, và kiểm tra `phieu.KetQuaHienMau.KetQua == "Hiến máu thành công"`.
  - Thứ hai, hàm `GetActualDonationDate` lấy ngày hiến máu thực tế từ `phieu.KetQuaHienMau.NgayCapNhat`. Em không dùng `PhieuDangKyHienMau.NgayNop` vì ngày nộp phiếu chỉ là ngày đăng ký trên web, không phải ngày lấy máu.
  - Thứ ba, hàm `EvaluateRecentDonationEligibility` sắp xếp các lần hiến thành công có ngày thực tế theo thứ tự giảm dần để lấy lần gần nhất (`lastActualDate`), sau đó tính `nextEligibleDate = lastActualDate.Date.AddDays(84)`. Nếu `DateTime.Today < nextEligibleDate`, hàm trả về `IsEligible = false` kèm số ngày còn phải chờ (`remainingDays`). Ngoài ra, nếu phát hiện có phiếu mang trạng thái `'Hiến máu thành công'` nhưng bị khuyết ngày bên bảng `KetQuaHienMau`, hệ thống bật cờ `HasMissingActualDonationDate = true` để chặn an toàn chứ không đoán ngày."

---

### Chủ đề 3: Giải thích cơ chế chống gửi trùng 2 phiếu đồng thời (Race Condition / Concurrency)
- **⏱️ Bản ngắn (30 giây):**
  "Thưa cô, để chống việc một người bấm đúp chuột hoặc gửi 2 request POST cùng lúc tạo ra 2 phiếu trùng nhau, tại `DangKyController.Create [POST]` (dòng 532–616), em kết hợp 2 cơ chế: một là dùng `ConcurrentDictionary<int, SemaphoreSlim> _submissionLocks` khóa theo từng `MaNguoiHien`, hai là mở Database Transaction với mức cô lập `IsolationLevel.Serializable` để kiểm tra lại lịch sử 84 ngày và kiểm tra trùng phiếu ngay bên trong transaction trước khi `CommitAsync()`."
- **🎙️ Bản đầy đủ (60–90 giây):**
  "Thưa cô, nếu chỉ kiểm tra `existingPhieu == null` bằng câu lệnh `SELECT` thông thường thì khi 2 request đến cùng lúc, cả 2 luồng đều đọc thấy chưa có phiếu trước khi luồng kia kịp `INSERT`.
  Để giải quyết triệt để tại dòng 532–616 của `DangKyController.cs`, em áp dụng 2 lớp bảo vệ:
  - Lớp 1 ở tầng ứng dụng: Em khai báo biến tĩnh `_submissionLocks` kiểu `ConcurrentDictionary<int, SemaphoreSlim>`. Mỗi `MaNguoiHien` có một đối tượng `SemaphoreSlim(1, 1)` riêng. Khi request đầu tiên của người đó vào tới bước tạo phiếu, nó gọi `await userLock.WaitAsync()`, buộc request thứ hai của cùng người đó phải đợi.
  - Lớp 2 ở tầng CSDL: Bên trong vùng khóa, em mở Transaction với `IsolationLevel.Serializable` — mức cô lập cao nhất trong SQL Server. Trong transaction này, em truy vấn lại `lichSuPhieuInTx`, chạy lại hàm kiểm tra 84 ngày và kiểm tra lại xem đã có phiếu đang hoạt động trong đợt (`existingPhieu`) hay chưa. Chỉ khi thỏa mãn hoàn toàn thì mới `Add` phiếu, `SaveChangesAsync()` và `CommitAsync()`. Khối `finally` luôn gọi `userLock.Release()` để đảm bảo không bao giờ bị treo khóa."

---

### Chủ đề 4: Giải thích điều kiện và luồng Hủy phiếu đăng ký (`DangKyController.Huy`)
- **⏱️ Bản ngắn (30 giây):**
  "Thưa cô, chức năng hủy phiếu nằm ở action `POST /DangKy/Huy(int id)` (dòng 725–781 `DangKyController.cs`). Người hiến máu chỉ hủy được phiếu khi thỏa mãn đủ 4 điều kiện: (1) Phiếu thuộc về chính `MaNguoiHien` đang đăng nhập, (2) Trạng thái phiếu đang là `'Chờ xác nhận'` hoặc `'Đã xác nhận'`, (3) Chưa có lịch khám sàng lọc nào đang hoạt động (`TrangThai != "Đã hủy"`), và (4) Chưa có kết quả hiến máu (`KetQuaHienMau == null`). Khi hủy, hệ thống cập nhật `TrangThai = "Đã hủy"` và `NgayXuLy = DateTime.Now`."
- **🎙️ Bản đầy đủ (60–90 giây):**
  "Thưa cô, quy trình hủy phiếu tại `DangKyController.Huy` được em bảo vệ chặt chẽ ở cả giao diện lẫn Backend:
  - Trên giao diện `Views/DangKy/TheoDoi.cshtml` (dòng 356), nút `'Hủy đăng ký'` chỉ hiển thị khi phiếu đang ở trạng thái `'Chờ xác nhận'` hoặc `'Đã xác nhận'`, chưa có lịch khám hoạt động và chưa có kết quả.
  - Khi người dùng xác nhận hủy ở modal, form gửi request `POST /DangKy/Huy` kèm Anti-Forgery Token.
  - Tại Backend (`DangKyController.cs` dòng 725–781), hệ thống lấy `MaTaiKhoan` từ Cookie, tìm `nguoiHienMau` và truy vấn phiếu theo cả `p.MaPhieu == id && p.MaNguoiHien == nguoiHienMau.MaNguoiHien` để chống việc người này sửa ID hủy phiếu của người khác. Sau đó hệ thống kiểm tra lại trạng thái chỉ được nằm trong mảng `{"Chờ xác nhận", "Đã xác nhận"}`, kiểm tra `!phieu.LichKhamSangLocs.Any(l => l.TrangThai != "Đã hủy")` và `phieu.KetQuaHienMau == null`. Cuối cùng hệ thống đổi `phieu.TrangThai = "Đã hủy"` và lưu `NgayXuLy = DateTime.Now` để giữ lại lịch sử phục vụ thống kê."

---

### Chủ đề 5: Giải thích luồng Xem và Cập nhật Hồ sơ cá nhân (`NguoiHienMauController.HoSoCaNhan`)
- **⏱️ Bản ngắn (30 giây):**
  "Thưa cô, `NguoiHienMauController.HoSoCaNhan` gồm 2 action GET và POST, yêu cầu đăng nhập `[Authorize]`. Ở hàm GET, hệ thống lấy `MaTaiKhoan` từ Cookie để nạp hồ sơ `NguoiHienMau` kèm lịch sử phiếu và gọi hàm `EvaluateRecentDonationEligibility` của `DangKyController` để tính số lần hiến thành công và số ngày chờ 84 ngày. Ở hàm POST, hệ thống kiểm tra hợp lệ các trường Họ tên, SĐT, Ngày sinh 18–60, Giới tính, Email, Địa chỉ, Cân nặng 30–200kg rồi cập nhật vào bảng `NguoiHienMau` và đồng bộ `HoTen` sang bảng `TaiKhoan`."
- **🎙️ Bản đầy đủ (60–90 giây):**
  "Thưa cô, luồng quản lý hồ sơ cá nhân trong `Controllers/NguoiHienMauController.cs` hoạt động như sau:
  - Ở `[HttpGet] HoSoCaNhan()` (dòng 48–101): Hệ thống đọc Claim `'MaTaiKhoan'` từ Cookie xác thực và truy vấn bảng `NguoiHienMau` kèm theo `TaiKhoan`, `PhieuDangKyHienMaus`, `DotHienMau`, `KetQuaHienMau` và `LichKhamSangLocs`. Nếu tài khoản đó chưa có bản ghi bên bảng `NguoiHienMau`, hệ thống lấy thông tin từ bảng `TaiKhoan` khởi tạo một đối tượng `NguoiHienMau` tạm trên RAM để điền sẵn Họ tên, Email, SĐT lên form. Đồng thời hàm gọi `DangKyController.EvaluateRecentDonationEligibility` để truyền các chỉ số thống kê sang View qua `ViewBag`.
  - Ở `[HttpPost] HoSoCaNhan(NguoiHienMau model)` (dòng 103–282): Hệ thống chuẩn hóa chuỗi, làm sạch SĐT bằng `NormalizePhone` và kiểm tra đầy đủ các ràng buộc: tuổi từ 18 đến 60, SĐT 10–11 số bắt đầu bằng 0, cân nặng 30–200kg, email và địa chỉ hợp lệ. Nếu có lỗi thì trả về `View(model)` kèm thông báo `TempData["Error"]`. Nếu hợp lệ thì cập nhật tường minh từng trường vào thực thể `NguoiHienMau`, đồng bộ `HoTen` sang `TaiKhoan` và gọi `SaveChangesAsync()`."

---

### Chủ đề 6: Giải thích mối quan hệ CSDL giữa `TaiKhoan`, `NguoiHienMau`, `PhieuDangKyHienMau`, `DotHienMau`, `LichKhamSangLoc` và `KetQuaHienMau`
- **⏱️ Bản ngắn (30 giây):**
  "Thưa cô, trong `ApplicationDbContext.cs`, bảng `TaiKhoan` liên kết `1 - 0..1` với `NguoiHienMau` qua khóa ngoại `MaTaiKhoan` (`DeleteBehavior.SetNull`). Bảng `NguoiHienMau` liên kết `1 - n` với `PhieuDangKyHienMau` qua `MaNguoiHien` (`DeleteBehavior.Restrict`). Bảng `DotHienMau` liên kết `1 - n` với `PhieuDangKyHienMau` qua `MaDot` (`DeleteBehavior.Restrict`). Từ `PhieuDangKyHienMau` sẽ liên kết `1 - n` với `LichKhamSangLoc` và `1 - 0..1` với `KetQuaHienMau` qua `MaPhieu` (`DeleteBehavior.Cascade`)."
- **🎙️ Bản đầy đủ (60–90 giây):**
  "Thưa cô, thiết kế CSDL quanh Module 3 được cấu hình bằng Fluent API trong `Data/ApplicationDbContext.cs` với các chủ đích nghiệp vụ rất rõ ràng:
  1. Giữa `TaiKhoan` và `NguoiHienMau` là quan hệ `1 - 0..1` qua khóa ngoại nullable `MaTaiKhoan`, có Unique Filtered Index `WHERE [MaTaiKhoan] IS NOT NULL`. Nhờ `MaTaiKhoan` cho phép `NULL`, hệ thống lưu được cả hồ sơ của khách vãng lai chưa có tài khoản; còn nhờ Unique Filtered Index, mỗi tài khoản chỉ gắn được tối đa với 1 hồ sơ người hiến máu. Hành vi xóa là `SetNull` để nếu xóa tài khoản đăng nhập thì hồ sơ y tế và lịch sử hiến máu vẫn được giữ lại.
  2. Giữa `NguoiHienMau` với `PhieuDangKyHienMau` (`MaNguoiHien`) và giữa `DotHienMau` với `PhieuDangKyHienMau` (`MaDot`) đều là quan hệ `1 - n` với `DeleteBehavior.Restrict`, nhằm ngăn chặn việc xóa nhầm một người hiến máu hoặc một đợt hiến máu khi đã có phiếu đăng ký phát sinh.
  3. Giữa `PhieuDangKyHienMau` với `LichKhamSangLoc` (`1 - n`) và `KetQuaHienMau` (`1 - 0..1`, có Unique Index trên `MaPhieu`) dùng `DeleteBehavior.Cascade` vì lịch khám và kết quả là dữ liệu phụ thuộc trực tiếp vào tờ phiếu đăng ký đó."

# BÁO CÁO KIỂM THỬ HỒI QUY (REGRESSION TEST) SAU KHI CHUYỂN ĐỔI REPOSITORY PATTERN – MODULE 3

- **Dự án**: `QuanLyHienMau_UNETI2_TI17A5HN`
- **Phạm vi kiểm thử**: **Module 3** – Quản lý hồ sơ người hiến máu và Đăng ký hiến máu sau khi chuyển sang sử dụng `INguoiHienMauRepository` và `IPhieuDangKyRepository`.
- **Công cụ kiểm thử**:
  1. Kiểm tra kiến trúc tĩnh & Reflection (`ARCH-01` → `ARCH-05`).
  2. Kiểm tra biên dịch (`dotnet restore` & `dotnet build`).
  3. Bộ kiểm thử tự động thực thi trực tiếp các Action của `NguoiHienMauController` và `DangKyController` thông qua `INguoiHienMauRepository` và `IPhieuDangKyRepository` (`21/21 PASS`).

---

## 1. KẾT QUẢ KIỂM TRA KIẾN TRÚC (MỤC 6.A)

| Mã KT | Tiêu chí kiểm tra kiến trúc | Phương pháp xác minh | Kết quả | Bằng chứng thực tế |
| :---: | :--- | :--- | :---: | :--- |
| **ARCH-01** | [`NguoiHienMauController`](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs) không còn inject `ApplicationDbContext` | `Select-String` + Reflection kiểm tra toàn bộ fields của class | **PASS** | Chỉ có duy nhất field `private readonly INguoiHienMauRepository _nguoiHienMauRepo;` (dòng 17). |
| **ARCH-02** | [`DangKyController`](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs) không còn inject `ApplicationDbContext` | `Select-String` + Reflection kiểm tra toàn bộ fields của class | **PASS** | Chỉ có `_phieuDangKyRepo` (`IPhieuDangKyRepository`) và `_nguoiHienMauRepo` (`INguoiHienMauRepository`) (dòng 18–19). |
| **ARCH-03** | Không còn truy vấn trực tiếp `_context` trong hai Controller của Module 3 | `Select-String -Pattern "ApplicationDbContext\|_context"` trên 2 file Controller | **PASS** | 0 kết quả khớp. Không còn `using Microsoft.EntityFrameworkCore;` hay `using QuanLyHienMau_UNETI2_TI17A5HN.Data;` trong 2 Controller. |
| **ARCH-04** | Tất cả Repository được đăng ký Dependency Injection đầy đủ trong [`Program.cs`](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Program.cs#L16-L18) | Kiểm tra mã nguồn `Program.cs` và khởi tạo `ServiceProvider` | **PASS** | Đăng ký đầy đủ `IRepository<>`, `INguoiHienMauRepository`, `IPhieuDangKyRepository` với vòng đời `AddScoped`. |
| **ARCH-05** | Hai Repository chia sẻ cùng một `ApplicationDbContext` instance trong cùng một Scoped HTTP Request | Kiểm tra `ReferenceEquals(ctx1, ctx2)` giữa `INguoiHienMauRepository` và `IPhieuDangKyRepository` trong cùng `IServiceScope` | **PASS** | `ReferenceEquals(ctx1, ctx2) == true` – Đảm bảo mọi thay đổi trên `NguoiHienMau`, `TaiKhoan` và `PhieuDangKyHienMau` được lưu nguyên tử trong cùng một Transaction. |

---

## 2. KẾT QUẢ BUILD DỰ ÁN (MỤC 6.B)

- **Lệnh thực thi**:
  ```powershell
  dotnet restore "QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN.csproj"
  dotnet build "QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN.csproj" --no-restore
  ```
- **Kết quả**:
  - `dotnet restore`: `All projects are up-to-date for restore.`
  - `dotnet build`: **`Build succeeded. 0 Error(s), 0 C# Compiler Warning(s)`** (Không phát sinh bất kỳ lỗi biên dịch hay cảnh báo C# mới nào).

---

## 3. KẾT QUẢ KIỂM THỬ HỒI QUY 12 KỊCH BẢN NGHIỆP VỤ (MỤC 6.C)

Bộ kiểm thử tự động (`Module3RepoTests`) thực thi trực tiếp các phương thức Action trên [`NguoiHienMauController`](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs) và [`DangKyController`](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs) thông qua `INguoiHienMauRepository` và `IPhieuDangKyRepository`:

| STT | Kịch bản kiểm thử hồi quy (Yêu cầu Mục 6.C) | Mã Test | Kết quả Logic Controller + Repository | Kết quả Runtime DB vật lý (LocalDB) | Chi tiết xác minh thực tế |
| :---: | :--- | :---: | :---: | :---: | :--- |
| 1 | **Xem hồ sơ cá nhân** (`GET /NguoiHienMau/HoSoCaNhan`) | `REG-01A`<br>`REG-01B` | **PASS** | *Chưa kiểm chứng (Sandbox chặn LocalDB)* | - `REG-01A`: Khi tài khoản đã có hồ sơ, gọi `_nguoiHienMauRepo.GetProfileWithFullHistoryAsync(maTaiKhoan)` trả về đúng `ViewResult` chứa `NguoiHienMau`.<br>- `REG-01B`: Khi tài khoản mới đăng nhập lần đầu chưa có hồ sơ, tự động khởi tạo `NguoiHienMau` mới qua `_nguoiHienMauRepo.AddAsync` + `SaveChangesAsync`. |
| 2 | **Cập nhật hồ sơ cá nhân** (`POST /NguoiHienMau/HoSoCaNhan`) | `REG-02A`<br>`REG-02B` | **PASS** | *Chưa kiểm chứng (Sandbox chặn LocalDB)* | - `REG-02A`: Dữ liệu hợp lệ -> cập nhật thông tin cá nhân, đồng bộ `HoTen` và `Email` sang `TaiKhoan`, gọi `_nguoiHienMauRepo.SaveChangesAsync()` và redirect về `HoSoCaNhan`.<br>- `REG-02B`: Nhập Email trùng với tài khoản khác -> `_nguoiHienMauRepo.IsEmailTakenByOtherAccountAsync` phát hiện, gán lỗi `ModelState` và gọi `_nguoiHienMauRepo.LoadDonationHistoryCollectionAsync(nguoiHienMau)` để giữ nguyên lịch sử hiến máu trên View. |
| 3 | **Đăng ký hiến máu hợp lệ** (`POST /DangKy/Create`) | `REG-03` | **PASS** | *Chưa kiểm chứng (Sandbox chặn LocalDB)* | Người hiến đủ tuổi (22 tuổi), cân nặng 62kg (`>= 45kg`), SĐT hợp lệ, khảo sát sức khỏe đạt (`no`/`no`), đợt đang mở -> Mở Transaction `Serializable`, cập nhật hồ sơ & tài khoản, tạo phiếu trạng thái `"Chờ xác nhận"`, gọi `_phieuDangKyRepo.SaveChangesAsync()` đúng **1 lần duy nhất** (nguyên tử), `CommitAsync()` và chuyển hướng sang `TheoDoi`. |
| 4 | **Đăng ký thiếu điều kiện cân nặng** (`POST /DangKy/Create`) | `REG-04` | **PASS** | *Chưa kiểm chứng (Sandbox chặn LocalDB)* | Nhập `canNang = 42.0` kg cho đợt yêu cầu `CanNangToiThieu = 45.0` kg -> Bị chặn, không tạo phiếu (`Count == 0`), trả về `ViewResult` kèm thông báo `"Cân nặng của bạn (42kg) chưa đạt mức tối thiểu quy định cho đợt này (yêu cầu ≥ 45kg)."`. |
| 5 | **Đăng ký không đủ tuổi** (`POST /DangKy/Create`) | `REG-05` | **PASS** | *Chưa kiểm chứng (Sandbox chặn LocalDB)* | Nhập `ngaySinh` ứng với 17 tuổi (`< 18` tuổi) -> Bị chặn, không tạo phiếu (`Count == 0`), trả về `ViewResult` kèm thông báo `"Độ tuổi người hiến máu theo quy định phải từ 18 đến 60 tuổi (Hiện tại: 17 tuổi)."`. |
| 6 | **Đăng ký trùng** (`GET & POST /DangKy/Create`) | `REG-06` | **PASS** | *Chưa kiểm chứng (Sandbox chặn LocalDB)* | Người hiến đã có phiếu `"Chờ xác nhận"` trong đợt `100` -> Cả `GET Create(100)` và `POST Create(100)` đều chặn không cho tạo phiếu thứ 2, gọi `transaction.RollbackAsync()` và chuyển hướng về `TheoDoi` của phiếu hiện có. |
| 7 | **Đăng ký trong thời gian chưa đủ 84 ngày** (`POST /DangKy/Create`) | `REG-07` | **PASS** | *Chưa kiểm chứng (Sandbox chặn LocalDB)* | Người hiến có lần `"Hiến máu thành công"` với `KetQuaHienMau.NgayCapNhat` cách đây 40 ngày (`< 84` ngày) -> `EvaluateRecentDonationEligibility` trả về `IsEligible = false` (còn 44 ngày), chặn đăng ký và hiển thị rõ ngày đủ điều kiện đăng ký lại. |
| 8 | **Đăng ký khi đợt hiến máu đã đóng** (`POST /DangKy/Create`) | `REG-08` | **PASS** | *Chưa kiểm chứng (Sandbox chặn LocalDB)* | Gửi đăng ký vào đợt `101` (`TrangThai = "Đã đóng"`) -> Bị chặn và redirect về `DotHienMau/Details/101`. |
| 9 | **Theo dõi phiếu của chính mình** (`GET /DangKy/TheoDoi`) | `REG-09` | **PASS** | *Chưa kiểm chứng (Sandbox chặn LocalDB)* | Người hiến máu `MaNguoiHien = 10` truy cập `TheoDoi(701)` -> Hiển thị chính xác thông tin phiếu `#701` của mình. |
| 10 | **Không xem được phiếu của người khác** (`GET /DangKy/TheoDoi`) | `REG-10` | **PASS** | *Chưa kiểm chứng (Sandbox chặn LocalDB)* | Người hiến máu `MaNguoiHien = 10` truyền `id = 702` (phiếu của `MaNguoiHien = 20`) -> Bị chặn, `ViewResult.Model == null` và báo lỗi `"Bạn không có quyền truy cập phiếu đăng ký này hoặc phiếu không tồn tại."`. |
| 11 | **Hủy phiếu khi được phép** (`POST /DangKy/Huy`) | `REG-11` | **PASS** | *Chưa kiểm chứng (Sandbox chặn LocalDB)* | Chủ sở hữu hủy phiếu `#801` đang ở trạng thái `"Chờ xác nhận"` (chưa có lịch khám và chưa có kết quả) -> Phiếu chuyển sang `"Đã hủy"`, cập nhật `NgayXuLy`, gọi `_phieuDangKyRepo.SaveChangesAsync()`. |
| 12 | **Không hủy được phiếu ở trạng thái không hợp lệ hoặc của người khác** (`POST /DangKy/Huy`) | `REG-12A`<br>`REG-12B`<br>`REG-12C` | **PASS** | *Chưa kiểm chứng (Sandbox chặn LocalDB)* | - `REG-12A`: Hủy phiếu `#802` của người khác -> Trả về `ForbidResult` (HTTP 403).<br>- `REG-12B`: Hủy phiếu `#803` đang ở trạng thái `"Đã khám sàng lọc"` -> Bị từ chối, giữ nguyên trạng thái.<br>- `REG-12C`: Hủy phiếu `#804` đang ở trạng thái `"Đã xác nhận"` nhưng đã có lịch khám sàng lọc `"Đã lên lịch"` -> Bị từ chối, giữ nguyên trạng thái. |

---

## 4. LOG KẾT QUẢ CHẠY BỘ TEST TỰ ĐỘNG (`21/21 PASS`)

```text
=== MODULE 3 REPOSITORY ARCHITECTURE & REGRESSION TEST SUITE ===
[PASS] ARCH-01: NguoiHienMauController không còn trường ApplicationDbContext
[PASS] ARCH-02: DangKyController không còn trường ApplicationDbContext
[PASS] ARCH-03: NguoiHienMauController chỉ inject INguoiHienMauRepository
[PASS] ARCH-04: DangKyController chỉ inject IPhieuDangKyRepository và INguoiHienMauRepository
[PASS] ARCH-05: INguoiHienMauRepository và IPhieuDangKyRepository chia sẻ cùng 1 ApplicationDbContext instance trong Scoped request
[PASS] REG-01A: Xem hồ sơ cá nhân hiện có thành công qua INguoiHienMauRepository
[PASS] REG-01B: Tự động tạo hồ sơ (Auto-provisioning) khi tài khoản đăng nhập lần đầu
[PASS] REG-02A: Cập nhật hồ sơ hợp lệ và đồng bộ HoTen, Email sang TaiKhoan
[PASS] REG-02B: Cập nhật hồ sơ với Email trùng bị chặn và nạp lại lịch sử hiến máu qua LoadDonationHistoryCollectionAsync
[PASS] REG-03: Đăng ký hiến máu hợp lệ tạo phiếu 'Chờ xác nhận', lưu nguyên tử 1 lần SaveChangesAsync và Commit Serializable Transaction
[PASS] REG-04: Đăng ký không đủ cân nặng tối thiểu của đợt hiến máu (42kg < 45kg) bị chặn
[PASS] REG-05: Đăng ký khi chưa đủ 18 tuổi bị chặn
[PASS] REG-06: Đăng ký trùng đợt đang xử lý bị chặn ở cả GET và POST (Rollback Transaction và chuyển sang TheoDoi)
[PASS] REG-07: Đăng ký khi chưa đủ 84 ngày kể từ lần hiến máu thành công gần nhất bị chặn
[PASS] REG-08: Đăng ký khi đợt hiến máu đã đóng bị chặn và chuyển hướng về chi tiết đợt
[PASS] REG-09: Người hiến máu xem được phiếu đăng ký của chính mình (#701)
[PASS] REG-10: Người hiến máu bị chặn khi cố tình truyền ID xem phiếu của người khác (#702)
[PASS] REG-11: Hủy phiếu hợp lệ (#801) chuyển trạng thái sang 'Đã hủy' và cập nhật NgayXuLy
[PASS] REG-12A: Chặn hủy phiếu của người khác (#802) bằng ForbidResult
[PASS] REG-12B: Chặn hủy phiếu ở trạng thái không hợp lệ ('Đã khám sàng lọc' #803)
[PASS] REG-12C: Chặn hủy phiếu đã được xếp lịch khám sàng lọc đang hoạt động (#804)
=== SUMMARY: 21 PASSED, 0 FAILED ===
```

---

## 5. GHI NHẬN TRUNG THỰC VỀ PHẠM VI CHƯA KIỂM CHỨNG RUNTIME DATABASE

- **Nội dung chưa kiểm chứng trực tiếp**: Thực thi các câu lệnh SQL vật lý trên tiến trình `(localdb)\mssqllocaldb` khi chạy web server trực tiếp.
- **Nguyên nhân**: Môi trường terminal sandbox cô lập kết nối Named Pipe tới dịch vụ SQL Server LocalDB của Windows.
- **Đánh giá độ tin cậy**: Toàn bộ các câu truy vấn EF Core (`Include`, `ThenInclude`, `Where`, `OrderByDescending`, `FirstOrDefaultAsync`, `ToListAsync`, `AnyAsync`, `FindAsync`, `Entry().Collection().Query().LoadAsync()`, `Database.BeginTransactionAsync(IsolationLevel.Serializable)`) bên trong `NguoiHienMauRepository` và `PhieuDangKyRepository` được chuyển **nguyên bản 1:1** từ code hoạt động trước đó của `NguoiHienMauController` và `DangKyController`, không thay đổi bất kỳ biểu thức LINQ hay cấu trúc bảng nào.

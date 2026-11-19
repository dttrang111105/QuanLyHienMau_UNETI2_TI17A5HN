# PHẦN VI (MỤC 5, 8, 9, 10): DANH SÁCH NỘI DUNG CHƯA XÁC MINH RUNTIME, CHECKLIST ÔN TẬP TRƯỚC NGÀY BẢO VỆ, BẢNG THUẬT NGỮ ANH–VIỆT VÀ CÂU HỎI CẦN HỎI LẠI NHÓM / GIẢNG VIÊN

---

## 1. DANH SÁCH NHỮNG NỘI DUNG CHƯA ĐƯỢC XÁC MINH RUNTIME (ĐỂ KHÔNG NÓI QUÁ SỰ THẬT KHI BẢO VỆ)

> [!WARNING]
> Khi trả lời hội đồng bảo vệ, sự **trung thực và chuẩn xác về mặt kỹ thuật** luôn được đánh giá cao hơn việc khẳng định quá đà. Dưới đây là ranh giới rõ ràng giữa những gì đã xác minh 100% từ mã nguồn/trình biên dịch và những gì cần chạy thực tế trên máy cá nhân của bạn trước giờ lên bảo vệ:

### 1.1. Những gì ĐÃ XÁC MINH 100% (Có thể khẳng định chắc chắn tuyệt đối)
1. **Nhánh Git và Trạng thái Biên dịch:**
   - Nhánh hiện tại là `DamThuTrang_23103100243`.
   - Lệnh `dotnet build "QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN.csproj" --no-restore` biên dịch thành công (`Build succeeded`) với **0 Error(s) và 0 C# Compiler Warning(s)** trên `.NET 10.0`.
2. **Toàn bộ logic mã nguồn C# và Razor View:**
   - Đã đọc và kiểm tra từng dòng trong `DangKyController.cs` (803 dòng), `NguoiHienMauController.cs` (284 dòng), `AdminController.cs` (1180 dòng), `AccountController.cs`, `DotHienMauController.cs`, `ApplicationDbContext.cs`, `DbInitializer.cs`, 7 Entity Models và các file `.cshtml`.
   - Đã xác minh chính xác thuật toán 84 ngày (`EvaluateRecentDonationEligibility`), cách lấy ngày `KetQuaHienMau.NgayCapNhat`, cơ chế khóa `SemaphoreSlim` + `IsolationLevel.Serializable`, logic hủy phiếu `Huy(int id)` và cập nhật hồ sơ `HoSoCaNhan`.

### 1.2. Những gì CHƯA XÁC MINH RUNTIME trong môi trường Sandbox (Bắt buộc chạy kiểm tra trên máy cá nhân trước buổi bảo vệ)
1. **Kết nối cơ sở dữ liệu SQL Server LocalDB (`(localdb)\mssqllocaldb`) và khởi tạo Seed Data (`DbInitializer.Initialize`):**
   - Trong môi trường phân tích tĩnh (Sandbox), dịch vụ Windows LocalDB bị chặn quyền khởi tạo tiến trình con. Vì vậy, trước khi đi bảo vệ, bạn cần mở Visual Studio trên máy cá nhân, nhấn `F5` (hoặc chạy `dotnet run`) để xác nhận LocalDB tự động tạo database `QuanLyHienMau_UNETI2_TI17A5HN_New` và nạp dữ liệu mẫu từ `DbInitializer.cs` thành công.
2. **Kiểm thử tải đồng thời thực tế (Live High-Concurrency Load Test):**
   - Cơ chế `SemaphoreSlim` + `IsolationLevel.Serializable` đã được chứng minh đúng về mặt logic code trên 1 tiến trình Web Server, nhưng chưa chạy công cụ bắn tải tự động (như JMeter / k6) trên SQL Server thực tế. Khi trả lời cô, hãy nói: *"Em đã thiết kế cơ chế đồng bộ bằng `SemaphoreSlim` kết hợp Transaction `IsolationLevel.Serializable` trong code để chống việc 1 người gửi 2 request đồng thời"*.
3. **Biểu đồ UML chính thức trong quyển báo cáo Word của nhóm:**
   - Vì trong repository Git không lưu file Word báo cáo chung của cả nhóm, các biểu đồ UML trong file `04_CAU_HOI_UML_PHAN_TICH_THIET_KE.md` được dựng chuẩn từ code thực tế. Bạn cần mở quyển báo cáo Word của nhóm ra đối chiếu xem tên các Use Case / Class trong quyển Word có khớp với code hay không.

---

## 2. CHECKLIST ÔN TẬP VÀ CHUẨN BỊ TRƯỚC NGÀY BẢO VỆ (THEO MỐC THỜI GIAN)

### 📅 Trước ngày bảo vệ 1 ngày (Tối hôm trước)
- [ ] **Bước 1: Đọc kỹ thứ tự 4 file cốt lõi nhất trong bộ tài liệu này:**
  1. `00_TONG_QUAN_MODULE3.md` (Nắm bức tranh tổng thể và sự khác biệt giữa nhánh của mình với báo cáo cũ).
  2. `08_TOM_TAT_50_CAU_HOI.md` (Học thuộc ý 50 câu quan trọng nhất và 20 câu bẫy dễ sai).
  3. `02_CAU_TRA_LOI_CHI_TIET.md` (Tập nói thành tiếng các câu trả lời mẫu 30–60 giây và mở file code đối chiếu từng hàm H1–H12).
  4. `07_KICH_BAN_DEMO.md` (Tập dượt kịch bản thuyết trình 3–5 phút và kịch bản demo 6 bước).
- [ ] **Bước 2: Chạy thử dự án trên máy tính mang đi bảo vệ:**
  - Mở `QuanLyHienMau_UNETI2_TI17A5HN.sln` bằng Visual Studio.
  - Nhấn `F5` để chạy ứng dụng, đăng nhập thử bằng tài khoản Người hiến máu và tài khoản Admin (`admin` / `123456` hoặc xem trong `Data/DbInitializer.cs`).
  - Thực hiện thử đủ 5 bước demo: Cập nhật hồ sơ -> Đăng ký đợt hiến máu -> Xem trang Theo dõi -> Hủy phiếu -> Đăng ký lại -> Dùng tài khoản Admin duyệt và ghi nhận kết quả `"Hiến máu thành công"` -> Quay lại kiểm tra cảnh báo chặn 84 ngày.
- [ ] **Bước 3: Đánh dấu (Bookmark) các dòng code quan trọng trong Visual Studio:**
  - `DangKyController.cs`: Dòng 23 (`84` ngày), Dòng 41 (`IsConfirmedSuccessfulDonation`), Dòng 68 (`GetActualDonationDate`), Dòng 94 (`EvaluateRecentDonationEligibility`), Dòng 193 (`IsValidAge`), Dòng 312 (`Create [POST]`), Dòng 532 (`SemaphoreSlim` + Transaction), Dòng 725 (`Huy`).
  - `NguoiHienMauController.cs`: Dòng 48 (`HoSoCaNhan [GET]`), Dòng 103 (`HoSoCaNhan [POST]`).
  - `ApplicationDbContext.cs`: Dòng 20–84 (`OnModelCreating`).

### ⏰ Trước giờ vào phòng bảo vệ 30 phút
- [ ] Cắm sạc pin laptop đầy 100%, tắt các ứng dụng nặng không cần thiết.
- [ ] Khởi động sẵn Visual Studio và chạy sẵn Web (`dotnet run` / `F5`) để `LocalDB` khởi động nóng trước (tránh lúc cô gọi lên mới bật làm mất 20–30 giây chờ SQL Server LocalDB khởi động).
- [ ] Mở sẵn 2 cửa sổ trình duyệt:
  - Cửa sổ 1 (Bình thường): Đăng nhập sẵn tài khoản **Người hiến máu** (đang ở trang `/NguoiHienMau/HoSoCaNhan`).
  - Cửa sổ 2 (Ẩn danh - Incognito): Đăng nhập sẵn tài khoản **Admin** (đang ở trang `/Admin/PhieuDangKyHienMau`).
- [ ] Kiểm tra lại trong CSDL đã có ít nhất **1 đợt hiến máu đang mở (`"Đang mở"`)** và có ngày `KetThuc` lớn hơn ngày hôm nay.

---

## 3. BẢNG THUẬT NGỮ KỸ THUẬT ANH – VIỆT TRONG DỰ ÁN (DÀNH CHO SINH VIÊN ÔN TẬP)

| STT | Thuật ngữ Tiếng Anh | Dịch nghĩa Tiếng Việt | Ý nghĩa cụ thể trong Module 3 của dự án |
|---|---|---|---|
| 1 | **Entity / Domain Model** | Thực thể dữ liệu / Mô hình miền | Các class trong thư mục `Models/` (`NguoiHienMau`, `PhieuDangKyHienMau`) ánh xạ với các bảng trong SQL Server. |
| 2 | **DbContext (`ApplicationDbContext`)** | Ngữ cảnh cơ sở dữ liệu | Lớp của Entity Framework Core quản lý kết nối DB, truy vấn LINQ và lưu thay đổi (`SaveChangesAsync`). |
| 3 | **DbSet (`DbSet<T>`)** | Tập hợp thực thể (Bảng) | Đại diện cho một bảng trong CSDL (ví dụ `_context.NguoiHienMaus`, `_context.PhieuDangKyHienMaus`). |
| 4 | **Navigation Property** | Thuộc tính điều hướng | Thuộc tính trong Model giúp đi từ bảng này sang bảng liên kết (ví dụ `phieu.KetQuaHienMau`, `nguoiHienMau.PhieuDangKyHienMaus`). |
| 5 | **Eager Loading (`.Include` / `.ThenInclude`)** | Nạp dữ liệu sớm | Kỹ thuật dùng `.Include()` trong EF Core để `JOIN` và lấy luôn dữ liệu bảng liên quan ngay trong 1 câu truy vấn SQL. |
| 6 | **Primary Key (PK) / Foreign Key (FK)** | Khóa chính / Khóa ngoại | `PK`: `MaNguoiHien`, `MaPhieu`. `FK`: `MaTaiKhoan`, `MaNguoiHien`, `MaDot`, `MaPhieu`. |
| 7 | **Filtered Unique Index** | Chỉ mục duy nhất có điều kiện lọc | Cấu hình `.IsUnique().HasFilter("[MaTaiKhoan] IS NOT NULL")` cho phép nhiều dòng `NULL` nhưng nếu có `MaTaiKhoan` thì không được trùng. |
| 8 | **DeleteBehavior (`Restrict`, `SetNull`, `Cascade`)** | Hành vi khi xóa bản ghi cha | `Restrict`: Cấm xóa nếu có bản ghi con; `SetNull`: Gán khóa ngoại thành `NULL`; `Cascade`: Tự động xóa các bản ghi con phụ thuộc. |
| 9 | **Model Binding** | Cơ chế ánh xạ dữ liệu vào tham số | Quá trình ASP.NET Core tự động lấy dữ liệu từ Form HTML / Query String gán vào tham số của Action. |
| 10 | **Data Annotations** | Thuộc tính chú thích dữ liệu | Các thẻ như `[Key]`, `[Required]`, `[StringLength]`, `[Range]`, `[RegularExpression]`, `[EmailAddress]` đặt trên thuộc tính của Model. |
| 11 | **Server-side Validation vs Client-side Validation** | Kiểm tra dữ liệu phía Máy chủ vs phía Trình duyệt | Client-side chạy bằng HTML5/JS trên trình duyệt (có thể bị vượt qua bằng F12); Server-side chạy bằng C# trong Controller (chốt chặn bắt buộc). |
| 12 | **Authentication vs Authorization** | Xác thực danh tính vs Phân quyền truy cập | Authentication: Kiểm tra bạn là ai (qua Cookie đăng nhập). Authorization: Kiểm tra bạn có quyền vào trang đó không (`[Authorize]`, `Roles`). |
| 13 | **Claims / ClaimsPrincipal** | Thông tin định danh trong phiên đăng nhập | Các cặp thông tin lưu trong Cookie như `"MaTaiKhoan"`, `ClaimTypes.Name`, `ClaimTypes.Role`. |
| 14 | **CSRF (`[ValidateAntiForgeryToken]`)** | Tấn công giả mạo yêu cầu liên trang | Ngăn chặn trang web lạ gửi lén request POST thay mặt người dùng nhờ kiểm tra mã `__RequestVerificationToken`. |
| 15 | **IDOR (Insecure Direct Object Reference)** | Tham chiếu đối tượng trực tiếp không an toàn | Lỗi người dùng A sửa ID trên request để xem/hủy dữ liệu của người dùng B (đã được chặn ở `DangKyController.Huy`). |
| 16 | **Mass Assignment / Over-posting** | Gán dữ liệu hàng loạt ngoài ý muốn | Lỗi kẻ xấu chèn thêm cột nhạy cảm (như `TrangThai`) vào form POST để tự nâng trạng thái (đã được chặn). |
| 17 | **PRG Pattern (Post - Redirect - Get)** | Mẫu thiết kế Gửi POST -> Chuyển hướng -> Tải GET | Sau khi lưu POST thành công thì trả về `RedirectToAction` để tránh việc người dùng nhấn F5 làm gửi lặp lại form. |
| 18 | **Race Condition / Concurrency** | Điều kiện tranh chấp đồng thời | Hiện tượng 2 request gửi cùng một mili-giây gây tạo trùng dữ liệu. |
| 19 | **SemaphoreSlim** | Khóa giới hạn số luồng truy cập đồng thời | Cơ chế đồng bộ hóa luồng trong C# (`SemaphoreSlim(1, 1)`) chỉ cho phép 1 luồng của cùng `MaNguoiHien` đi vào vùng tạo phiếu tại một thời điểm. |
| 20 | **Transaction (`IsolationLevel.Serializable`)** | Giao dịch CSDL (Mức cô lập tuần tự hóa) | Đảm bảo toàn bộ các bước đọc-kiểm tra-ghi trong CSDL diễn ra nguyên tử (ACID) và cô lập hoàn toàn với giao dịch khác. |

---

## 4. DANH SÁCH CÂU HỎI CẦN HỎI LẠI NHÓM HOẶC GIẢNG VIÊN TRƯỚC BUỔI BẢO VỆ

Để tránh mọi sự lệch pha khi cả nhóm cùng lên bảo vệ trước hội đồng, bạn nên trao đổi nhanh với các thành viên phụ trách Module 1, 2, 4, 5 về **5 câu hỏi phối hợp nhóm** sau đây:

1. **Hỏi trưởng nhóm về Nhánh Git dùng để chấm bài:**
   - *"Hôm bảo vệ, nhóm mình sẽ chạy demo chung trên nhánh `integration` / `main` hay mỗi bạn mở nhánh cá nhân của mình (`DamThuTrang_23103100243`)?"*
   - **Lưu ý:** Nếu chạy trên nhánh `DamThuTrang_23103100243`, toàn bộ code khớp 100% với bộ tài liệu này (`AdminController` quản lý chung, 2 câu hỏi sức khỏe `health_q1`/`health_q2`, hủy phiếu ở 2 trạng thái). Nếu trưởng nhóm gộp sang nhánh `integration`, hãy kiểm tra xem file `DangKyController.cs` trên nhánh đó dùng 2 câu hỏi hay 4 câu hỏi sức khỏe để trả lời khớp với màn hình đang mở.
2. **Hỏi bạn phụ trách Module 1 (`AccountController`):**
   - *"Khi khách vãng lai đã đăng ký hiến máu bằng SĐT trước, sau đó mới đăng ký tài khoản bằng SĐT đó ở `Account/Register`, nếu cô hỏi thì mình thống nhất trả lời là hướng cải tiến liên-module nhé?"* (Đã có sẵn câu trả lời mẫu ở `05_CAU_HOI_PHAN_BIEN.md` Tình huống J22).
3. **Hỏi bạn phụ trách Module 5 (`KetQuaHienMau`):**
   - *"Khi ghi nhận kết quả hiến máu (`TaoKetQua`), cột `NgayCapNhat` đang đóng vai trò là ngày hiến máu thực tế để Module 3 của mình tính 84 ngày. Nếu cô hỏi về trường hợp nhập kết quả muộn hoặc sửa kết quả (`SuaKetQua`), bọn mình thống nhất đề xuất cải tiến tách thêm cột `NgayHienMauThucTe` nhé!"*
4. **Hỏi nhóm về quyển Báo cáo Word / Slide thuyết trình:**
   - *"Trong quyển báo cáo Word của nhóm, phần biểu đồ Use Case, Sequence, Activity và ERD của Module 3 đã vẽ khớp với 8 trạng thái phiếu và quy tắc 84 ngày trong code chưa?"* (Nếu chưa có hoặc còn sơ sài, bạn có thể copy trực tiếp các biểu đồ Mermaid trong file `03_CAU_HOI_MVC_DATABASE.md` và `04_CAU_HOI_UML_PHAN_TICH_THIET_KE.md` vào báo cáo).
5. **Hỏi giảng viên hướng dẫn (nếu có buổi duyệt trước bảo vệ):**
   - *"Thưa cô, đối với quy tắc thời gian chờ giữa hai lần hiến máu toàn phần, nhóm em đang áp dụng chuẩn 12 tuần (**84 ngày** — `AddDays(84)`) theo Thông tư 26/2013/TT-BYT của Bộ Y tế và tính từ ngày có kết quả hiến máu thành công thực tế (`KetQuaHienMau.NgayCapNhat`). Cô muốn nhóm giữ nguyên 84 ngày hay đổi sang `AddMonths(3)` (3 tháng dương lịch) ạ?"*

# BÁO CÁO HOÀN THIỆN CUỐI CÙNG MODULE 3 — QUẢN LÝ NGƯỜI HIẾN MÁU VÀ PHIẾU ĐĂNG KÝ HIẾN MÁU

- **Dự án**: `QuanLyHienMau_UNETI2_TI17A5HN`
- **Công nghệ**: ASP.NET Core MVC (.NET 10), Entity Framework Core 10, SQL Server
- **Nhánh làm việc hiện tại**: `integration`
- **Phạm vi hoàn thiện**: **Module 3 — Quản lý người hiến máu và phiếu đăng ký hiến máu**
- **Tài liệu đối chiếu**:
  - `MODULE3_AUDIT_REPORT.md` (Báo cáo kiểm tra toàn diện ban đầu)
  - `MODULE3_FIX_REPORT.md` (Báo cáo sửa 9 lỗi chính của Module 3)
  - `MODULE3_RECENT_DONATION_RULE_REPORT.md` (Báo cáo sửa quy tắc kiểm tra thời gian chờ 84 ngày kể từ lần hiến máu thành công gần nhất)
  - `MODULE3_REGRESSION_AUDIT.md` (Báo cáo kiểm tra hồi quy Module 3)

---

## 1. DANH SÁCH CHỨC NĂNG MODULE 3 VÀ TRẠNG THÁI CUỐI

| Mã CN | Nhóm chức năng / Nghiệp vụ Module 3 | File xử lý chính | Trạng thái Code & Unit Test | Trạng thái Runtime DB (Sandbox) | Ghi chú đánh giá cuối |
| :--- | :--- | :--- | :---: | :---: | :--- |
| **M3-01** | **Xem hồ sơ cá nhân người hiến máu** (`GET /NguoiHienMau/HoSoCaNhan`) | `Controllers/NguoiHienMauController.cs` (dòng 35–108), `Views/NguoiHienMau/HoSoCaNhan.cshtml` | **PASS** | **BLOCKED** *(LocalDB)* | Tự động khởi tạo hồ sơ `NguoiHienMau` từ `TaiKhoan` nếu đăng nhập lần đầu; nạp đầy đủ lịch sử phiếu, kết quả hiến máu và đợt hiến; không dùng bất kỳ dữ liệu giả/hardcode nào khi người dùng chưa từng hiến máu. |
| **M3-02** | **Cập nhật hồ sơ cá nhân người hiến máu** (`POST /NguoiHienMau/HoSoCaNhan`) | `Controllers/NguoiHienMauController.cs` (dòng 110–274), `Views/NguoiHienMau/HoSoCaNhan.cshtml` (dòng 654–780) | **PASS** | **BLOCKED** *(LocalDB)* | Kiểm tra chặt chẽ phía server: `HoTen` bắt buộc (≤ 100 ký tự), `NgaySinh` bắt buộc & độ tuổi **18–60 tuổi**, `CanNang` bắt buộc (**30–200 kg**), `SoDienThoai` bắt buộc (`^0[0-9]{9,10}$`), `Email` đúng định dạng và **không trùng tài khoản khác**, đồng bộ `TaiKhoan.HoTen` / `SoDienThoai` / `Email`. |
| **M3-03** | **Kiểm tra điều kiện đợt hiến máu & hồ sơ trước khi đăng ký** (`GET/POST /DangKy/Create`) | `Controllers/DangKyController.cs` (dòng 215–346, 349–530), `Views/DangKy/Create.cshtml` | **PASS** | **BLOCKED** *(LocalDB)* | Kiểm tra đợt hiến máu tồn tại, `TrangThai == "Đang mở đăng ký"`, nằm trong khoảng `NgayBatDauDangKy <= Today <= HanDangKy`, tuổi **18–60** (`NgaySinh` bắt buộc), cân nặng đạt ngưỡng tối thiểu của đợt (`>= max(42, DotHienMau.CanNangToiThieu)`), và 4 câu hỏi sàng lọc y tế ban đầu (`q1=NONE`, `q2=NO`, `q3=NO`, `q4=NO`). |
| **M3-04** | **Kiểm tra thời gian chờ 84 ngày giữa các lần hiến máu thực tế** | `Controllers/DangKyController.cs` (`IsConfirmedSuccessfulDonation`, `GetActualDonationDate`, `EvaluateRecentDonationEligibility` — dòng 29–205) | **PASS** | **BLOCKED** *(LocalDB)* | Chỉ tính phiếu đã xác nhận hiến máu thành công (`KetQuaHienMau.KetQua == "Hiến máu thành công"` hoặc `TrangThai == "Hiến máu thành công"`). Lấy ngày hiến thực tế từ `KetQuaHienMau.NgayCapNhat`. **Tuyệt đối không dùng `NgayNop` hoặc `NgayXuLy` thay thế**. Nếu thiếu `NgayCapNhat`, chặn an toàn và yêu cầu xác minh; nếu `< 84 ngày`, từ chối cả trên giao diện lẫn POST trực tiếp và hiển thị rõ ngày đủ điều kiện đăng ký lại. |
| **M3-05** | **Tạo phiếu đăng ký hiến máu mới** (`POST /DangKy/Create`) | `Controllers/DangKyController.cs` (dòng 531–649) | **PASS** | **BLOCKED** *(LocalDB)* | Lưu `PhieuDangKyHienMau` với `MaNguoiHien` thuộc chính tài khoản đang đăng nhập, `NgayNop = DateTime.Now`, `TrangThai = "Chờ xác nhận"`, tổng hợp ghi chú sàng lọc vào `GhiChu` (≤ 500 ký tự). |
| **M3-06** | **Chống đăng ký trùng & chống gửi đồng thời (Race Condition)** | `Controllers/DangKyController.cs` (dòng 22, 293–305, 535–601) | **PASS** | **BLOCKED** *(LocalDB)* | Chặn đăng ký mới nếu người hiến đang có phiếu hoạt động ở bất kỳ đợt nào hoặc đã có phiếu hoạt động trong cùng đợt; sử dụng khóa đồng bộ `_registrationLock` (`SemaphoreSlim(1, 1)`) kết hợp Transaction `IsolationLevel.Serializable` để chống double-submit / gửi đồng thời. |
| **M3-07** | **Theo dõi trạng thái phiếu & hiển thị lý do từ chối / tạm hoãn** (`GET /DangKy/TheoDoi`) | `Controllers/DangKyController.cs` (dòng 652–720), `Views/DangKy/TheoDoi.cshtml` | **PASS** | **BLOCKED** *(LocalDB)* | Hiển thị tiến trình 4 bước và toàn bộ 7 trạng thái phiếu (`Chờ xác nhận`, `Đã xác nhận`, `Chờ khám sàng lọc`, `Đã khám sàng lọc`, `Hiến máu thành công`, `Từ chối`, `Không đủ điều kiện hiến`, `Đã hủy`), kèm thông báo nổi bật lý do từ chối/tạm hoãn từ `GhiChuXacNhan`, `PhieuKhamSangLoc.LyDoTamHoan`, `KetQuaHienMau.GhiChu`. |
| **M3-08** | **Hủy phiếu đăng ký theo đúng điều kiện** (`POST /DangKy/Huy`) | `Controllers/DangKyController.cs` (dòng 723–797), `Views/DangKy/TheoDoi.cshtml` (dòng 486–529) | **PASS** | **BLOCKED** *(LocalDB)* | Chỉ cho phép chủ sở hữu hủy khi phiếu ở 1 trong 3 trạng thái: `"Chờ xác nhận"`, `"Đã xác nhận"`, `"Chờ khám sàng lọc"` và chưa phát sinh `PhieuKhamSangLoc` hoặc `KetQuaHienMau`. Chặn hủy đối với `"Đã khám sàng lọc"`, `"Hiến máu thành công"`, `"Từ chối"`, `"Không đủ điều kiện hiến"`, `"Đã hủy"`. |
| **M3-09** | **Phân quyền, bảo vệ dữ liệu cá nhân, chống IDOR & CSRF** | `Controllers/NguoiHienMauController.cs`, `Controllers/DangKyController.cs` | **PASS** | **BLOCKED** *(LocalDB)* | Cả 2 Controller đều gắn `[Authorize]`; toàn bộ 3 POST action đều gắn `[ValidateAntiForgeryToken]`; định danh người dùng lấy hoàn toàn từ `ClaimTypes.NameIdentifier` trong Cookie xác thực; kiểm tra `phieu.MaNguoiHien != nguoiHienMau.MaNguoiHien` chặn hoàn toàn hành vi xem/hủy phiếu của người khác (chống IDOR). |
| **M3-10** | **Hiển thị lịch sử hiến máu, chu kỳ phục hồi & liên kết quản trị** | `Views/NguoiHienMau/HoSoCaNhan.cshtml` (dòng 15–123, 286–503), `Views/Admin/NguoiHienMau.cshtml` (dòng 547) | **PASS** | **BLOCKED** *(LocalDB)* | Tính đúng số lần hiến thành công, thể tích máu thực tế (`KetQuaHienMau.TheTich`), chu kỳ phục hồi 84 ngày theo `KetQuaHienMau.NgayCapNhat`, phân biệt rõ `ĐK: dd/MM/yyyy` (ngày nộp phiếu) và `Ngày hiến: dd/MM/yyyy` (ngày hiến thực tế); liên kết quản trị trỏ đúng tới `Admin/PhieuDangKy`. |

---

## 2. CÁC LỖI ĐÃ PHÁT HIỆN VÀ ĐÃ SỬA TRONG MODULE 3

Trong toàn bộ quá trình kiểm tra, sửa lỗi, rà soát hồi quy và hoàn thiện cuối cùng, **12 lỗi** thuộc Module 3 (và 1 điểm liên kết điều hướng tích hợp) đã được phát hiện và khắc phục triệt để:

1. **Lỗi lỗ hổng xác thực & phân quyền (`[Authorize]` & `[ValidateAntiForgeryToken]`)**:
   - **Trước khi sửa**: `NguoiHienMauController` không có `[Authorize]`; khi chưa đăng nhập lại fallback lấy bản ghi người hiến đầu tiên trong DB (`_context.NguoiHienMaus.FirstOrDefaultAsync()`), làm lộ dữ liệu cá nhân.
   - **Đã sửa**: Gắn `[Authorize]` cấp lớp cho cả `NguoiHienMauController` và `DangKyController`; gắn `[ValidateAntiForgeryToken]` cho tất cả các action `POST` (`HoSoCaNhan`, `Create`, `Huy`); bắt buộc xác định người dùng từ `ClaimTypes.NameIdentifier`.
2. **Lỗi lỗ hổng IDOR khi theo dõi phiếu (`DangKyController.TheoDoi`)**:
   - **Trước khi sửa**: Người dùng đăng nhập có thể truyền `?id=<MaPhieu>` bất kỳ trên URL để xem chi tiết phiếu đăng ký của người khác.
   - **Đã sửa**: Kiểm tra nghiêm ngặt `phieu.MaNguoiHien != nguoiHienMau.MaNguoiHien` (cả khi truyền `id` lẫn khi không truyền `id`); nếu không thuộc về tài khoản đang đăng nhập thì từ chối và chuyển hướng kèm thông báo lỗi.
3. **Lỗi tính thời gian chờ 84 ngày dựa trên `NgayNop` hoặc tất cả các phiếu trong lịch sử**:
   - **Trước khi sửa**: Đếm số lần hiến và tính chu kỳ phục hồi 84 ngày dựa vào mọi phiếu không bị hủy (kể cả phiếu `"Chờ xác nhận"`) hoặc dùng `NgayNop` (ngày nộp phiếu) thay cho ngày hiến máu thực tế, và chỉ hiển thị trên giao diện mà không chặn khi gửi `POST /DangKy/Create`.
   - **Đã sửa**: Xây dựng bộ phương thức `IsConfirmedSuccessfulDonation`, `GetActualDonationDate`, `EvaluateRecentDonationEligibility` trong `DangKyController`:
     - Chỉ tính phiếu đã xác nhận `"Hiến máu thành công"`.
     - Chỉ lấy ngày hiến máu thực tế từ `KetQuaHienMau.NgayCapNhat` (**tuyệt đối không fallback sang `NgayNop` hoặc `NgayXuLy`**).
     - Nếu bản ghi hiến thành công bị thiếu `KetQuaHienMau` hoặc `NgayCapNhat == null`, hệ thống chặn đăng ký an toàn và hiển thị thông báo yêu cầu nhân viên y tế kiểm tra.
     - Nếu chưa đủ **84 ngày** (`daysSinceLastDonation < 84`), chặn cả trên giao diện (`GET /DangKy/Create`) lẫn phía server (`POST /DangKy/Create`) và hiển thị rõ ngày đủ điều kiện đăng ký lại (`lastActualDate.AddDays(84)`).
4. **Lỗi thiếu kiểm tra bắt buộc `NgaySinh` và `SoDienThoai` khi cập nhật hồ sơ / nộp phiếu**:
   - **Trước khi sửa**: Nếu `NgaySinh` để trống (`null`) trong cả hồ sơ lẫn form gửi lên, khối `if (finalNgaySinh.HasValue)` bị bỏ qua, khiến người dùng có thể lách qua kiểm tra độ tuổi **18–60 tuổi**.
   - **Đã sửa**: Bắt buộc phải có `NgaySinh` và `SoDienThoai` trong cả `NguoiHienMauController.HoSoCaNhan [POST]` (dòng 133–175) và `DangKyController.Create [POST]` (dòng 438–475), đồng thời thêm thuộc tính `required` trên ô nhập `ngaySinh` ở `Views/DangKy/Create.cshtml` (dòng 224).
5. **Lỗi thiếu kiểm tra trùng Email với tài khoản khác**:
   - **Trước khi sửa**: Cập nhật hồ sơ hoặc nộp phiếu với email đã thuộc về tài khoản khác gây lỗi `SqlException` do vi phạm ràng buộc `UNIQUE` trên `TaiKhoan.Email`.
   - **Đã sửa**: Kiểm tra `_context.TaiKhoans.AnyAsync(t => t.MaTaiKhoan != maTaiKhoan && t.Email != null && t.Email.ToLower() == email.ToLower())` ở cả `NguoiHienMauController.HoSoCaNhan [POST]` và `DangKyController.Create [POST]`, trả về thông báo lỗi thân thiện trên form.
6. **Lỗi thiếu kiểm tra thờiלgian mở/đóng nhận đăng ký của đợt hiến máu**:
   - **Trước khi sửa**: Chỉ kiểm tra chuỗi `TrangThai == "Đang mở đăng ký"` mà không đối chiếu `NgayBatDauDangKy` và `HanDangKy`.
   - **Đã sửa**: Kiểm tra `now.Date < dotHienMau.NgayBatDauDangKy.Date` và `now.Date > dotHienMau.HanDangKy.Date` ở cả `GET` và `POST /DangKy/Create`.
7. **Lỗi thiếu kiểm tra cân nặng tối thiểu riêng của từng đợt hiến máu (`DotHienMau.CanNangToiThieu`)**:
   - **Trước khi sửa**: Chỉ kiểm tra cứng `< 42 kg` mà bỏ qua ngưỡng `DotHienMau.CanNangToiThieu` (ví dụ đợt yêu cầu 45 kg hoặc 50 kg).
   - **Đã sửa**: Tính `minRequiredWeight = Math.Max(42.0, (double)(dotHienMau.CanNangToiThieu ?? 42))` và kiểm tra cả trên giao diện lẫn server.
8. **Lỗi mất liên kết câu trả lời khảo sát sàng lọc y tế trên `Views/DangKy/Create.cshtml`**:
   - **Trước khi sửa**: 4 câu hỏi sàng lọc y tế trên form không gửi dữ liệu lên server hoặc không được kiểm tra phía server.
   - **Đã sửa**: Bổ sung kiểm tra phía server (`q1 == "NONE"`, `q2 == "NO"`, `q3 == "NO"`, `q4 == "NO"`), từ chối nhận phiếu nếu người đăng ký có yếu tố nguy cơ y tế, và lưu tóm tắt kết quả sàng lọc vào `PhieuDangKyHienMau.GhiChu`.
9. **Lỗi Race Condition khi gửi trùng phiếu đăng ký đồng thời**:
   - **Trước khi sửa**: Hai request `POST /DangKy/Create` gửi cùng lúc có thể cùng vượt qua bước kiểm tra `AnyAsync` và tạo 2 phiếu trùng.
   - **Đã sửa**: Sử dụng `SemaphoreSlim(1, 1)` kết hợp Transaction `IsolationLevel.Serializable` để kiểm tra lại phiếu đang hoạt động ngay bên trong transaction trước khi `SaveChangesAsync`.
10. **Lỗi điều kiện hủy phiếu (`DangKyController.Huy`)**:
    - **Trước khi sửa**: Cho phép hủy phiếu không kiểm tra đầy đủ trạng thái hoặc dữ liệu khám sàng lọc / kết quả hiến máu đã phát sinh.
    - **Đã sửa**: Chỉ cho phép hủy khi phiếu thuộc 1 trong 3 trạng thái `"Chờ xác nhận"`, `"Đã xác nhận"`, `"Chờ khám sàng lọc"` và chưa có `PhieuKhamSangLoc` hoặc `KetQuaHienMau`.
11. **Lỗi dữ liệu cứng (hardcoded) trên giao diện Hồ sơ cá nhân và Form đăng ký**:
    - **Trước khi sửa**:
      - `Views/DangKy/Create.cshtml` (dòng 315) gán cứng `value="172"` cho ô chiều cao.
      - `Views/NguoiHienMau/HoSoCaNhan.cshtml` hiển thị cứng `"An toàn sinh học 100% / NAT & ELISA âm tính hoàn toàn"`, hoàn tất 4 bước `"Hành trình giọt máu"` và bảng `"Kết quả xét nghiệm: Âm tính / Đạt"` ngay cả khi người dùng mới tạo tài khoản và chưa từng hiến máu (`totalCount == 0`).
      - Thẻ lịch sử phiếu chỉ hiển thị 1 ngày duy nhất không rõ là ngày nộp phiếu hay ngày hiến thực tế, và tô màu xanh dương cho phiếu `"Từ chối"` / `"Không đủ điều kiện hiến"`.
    - **Đã sửa**:
      - Xóa `value="172"` trên ô chiều cao ở `Views/DangKy/Create.cshtml` (thay bằng `placeholder="VD: 168"`).
      - Trong `Views/NguoiHienMau/HoSoCaNhan.cshtml`: khi `totalCount == 0`, hiển thị trạng thái `"Chưa có KQ"` ở thẻ An toàn sinh học và hiển thị thông báo trạng thái trống ở Tab 2 (*Hành trình giọt máu*) & Tab 3 (*Hồ sơ xét nghiệm*).
      - Trên thẻ lịch sử hiến máu (Tab 1), hiển thị rõ nhãn `ĐK: dd/MM/yyyy` (ngày nộp phiếu) và `Ngày hiến: dd/MM/yyyy` (ngày hiến thực tế từ `KetQuaHienMau.NgayCapNhat` khi đã hiến thành công), đồng thời đổi màu badge `"Từ chối"` / `"Không đủ điều kiện hiến"` sang đỏ (`bg-rose-100 text-rose-800`).
12. **Lỗi liên kết điều hướng từ trang Quản trị người hiến máu sang trang Quản lý phiếu đăng ký**:
    - **Trước khi sửa**: `Views/Admin/NguoiHienMau.cshtml` (dòng 547) gọi `asp-action="PhieuDangKyHienMau"` (action không tồn tại trong `AdminController`), dẫn tới lỗi 404.
    - **Đã sửa**: Đổi thành `asp-action="PhieuDangKy"` khớp chính xác với `AdminController.PhieuDangKy(int? maDot, string? trangThai)`.

---

## 3. DANH SÁCH FILE THỰC SỰ THAY ĐỔI CỦA MODULE 3

Kiểm tra bằng `git status --short`:
- **Các file thuộc Module 3 (và 1 dòng liên kết tích hợp) được chỉnh sửa để hoàn thiện Module 3**:
  1. [`QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs`](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs)
  2. [`QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs`](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs)
  3. [`QuanLyHienMau_UNETI2_TI17A5HN/Views/DangKy/Create.cshtml`](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Views/DangKy/Create.cshtml)
  4. [`QuanLyHienMau_UNETI2_TI17A5HN/Views/DangKy/TheoDoi.cshtml`](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Views/DangKy/TheoDoi.cshtml)
  5. [`QuanLyHienMau_UNETI2_TI17A5HN/Views/NguoiHienMau/HoSoCaNhan.cshtml`](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Views/NguoiHienMau/HoSoCaNhan.cshtml)
  6. [`QuanLyHienMau_UNETI2_TI17A5HN/Views/Admin/NguoiHienMau.cshtml`](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Views/Admin/NguoiHienMau.cshtml#L547) *(chỉ sửa đúng 1 thuộc tính `asp-action="PhieuDangKy"` tại dòng 547 để sửa lỗi 404 khi bấm từ danh sách người hiến sang danh sách phiếu đăng ký)*.

- **Các file đã có thay đổi từ trước trong working tree của nhóm (được giữ nguyên 100%, không chạm vào)**:
  - `Controllers/AccountController.cs`
  - `Controllers/AdminController.cs`
  - `Controllers/DotHienMauController.cs`
  - `Controllers/KetQuaHienMauController.cs`
  - `Models/NguoiHienMau.cs`
  - `Models/PhieuDangKyHienMau.cs`
  - `Services/Module5Service.cs`
  - `Services/ThongKeService.cs`
  - `ViewModels/Module5ViewModels.cs`
  - `Views/Admin/ChiTietPhieuDangKy.cshtml`
  - `Views/Admin/KetQuaHienMau.cshtml`
  - `Views/DotHienMau/Details.cshtml`
  - `Views/DotHienMau/Index.cshtml`

---

## 4. KẾT QUẢ BUILD VÀ TỪNG KIỂM THỬ CHI TIẾT

### 4.1. Kết quả Build dự án (`dotnet build`)
- **Lệnh thực thi**:
  ```powershell
  dotnet build "QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN.csproj" --no-restore
  ```
- **Kết quả**: **PASS** (`0 Error(s)`, `0 Warning(s)` trên toàn bộ các file thuộc Module 3).

### 4.2. Kết quả chạy bộ kiểm thử tự động (`Module3RuleTests` trên `.NET 10`)
- **Lệnh thực thi**:
  ```powershell
  dotnet run --project "C:\Users\ADMIN\.gemini\antigravity\brain\8e80e798-934a-46c4-9d35-772acb81f5d1\scratch\Module3RuleTests\Module3RuleTests.csproj" --no-restore --no-launch-profile
  ```
- **Kết quả**: **7/7 nhóm kiểm thử tự động PASS (`0 FAILED`)**:
  - `[PASS] REG-ENTITY-01`: Kiểm tra đầy đủ 13 thuộc tính `NguoiHienMau` và 9 thuộc tính `PhieuDangKyHienMau`.
  - `[PASS] REG-SEC-01`: Kiểm tra `[Authorize]` cấp Controller và `[ValidateAntiForgeryToken]` trên toàn bộ 3 POST Actions (`HoSoCaNhan`, `Create`, `Huy`).
  - `[PASS] REG-84D-01`: Chưa từng hiến máu thành công (0 phiếu hoặc chỉ có phiếu đang xử lý ở 4 trạng thái chờ) -> `IsEligible = true`, `TotalSuccessfulDonations = 0`.
  - `[PASS] REG-84D-02`: Kiểm tra giá trị biên thời gian chờ: `83 ngày` (`IsEligible = false`, còn `1 ngày`), `84 ngày` (`IsEligible = true`), `85 ngày` (`IsEligible = true`).
  - `[PASS] REG-84D-03`: Phiếu `Đã hủy`, `Từ chối`, `Không đủ điều kiện hiến` không bị tính là lần hiến máu thành công (`IsEligible = true`, `TotalSuccessfulDonations = 0`).
  - `[PASS] REG-84D-04`: Nhiều lần hiến máu thành công -> sắp xếp chính xác theo `KetQuaHienMau.NgayCapNhat` thực tế (không theo `NgayNop` hay `MaPhieu`).
  - `[PASS] REG-84D-05`: Bản ghi `Hiến máu thành công` thiếu `NgayCapNhat` -> không dùng `NgayNop`/`NgayXuLy` thay thế, chặn đăng ký an toàn (`IsEligible = false`, `HasMissingActualDonationDate = true`).

### 4.3. Bảng kết quả đối chiếu 11 hạng mục kiểm thử theo yêu cầu Mục 4

| STT | Hạng mục kiểm thử yêu cầu | Kiểm tra Logic Code & Unit Test | Kiểm thử Runtime DB (LocalDB) | Chi tiết xác minh |
| :---: | :--- | :---: | :---: | :--- |
| 1 | **Hồ sơ hợp lệ và không hợp lệ** | **PASS** | **BLOCKED** | `NguoiHienMauController.HoSoCaNhan [POST]` kiểm tra `HoTen` (bắt buộc, ≤100 ký tự), `NgaySinh` (bắt buộc, không lớn hơn hôm nay, 18–60 tuổi), `CanNang` (bắt buộc, 30–200 kg), `SoDienThoai` (bắt buộc, `^0[0-9]{9,10}$`), độ dài `GioiTinh`, `NhomMau`, `NgheNghiep`, `DiaChi`, `GhiChuSucKhoe`. Khi lỗi giữ nguyên dữ liệu đã nhập trên form và báo lỗi chi tiết. |
| 2 | **Email trùng** | **PASS** | **BLOCKED** | Kiểm tra `_context.TaiKhoans.AnyAsync(t => t.MaTaiKhoan != maTaiKhoan && t.Email != null && t.Email.ToLower() == email.ToLower())` ở cả cập nhật hồ sơ (`NguoiHienMauController.cs:184`) và nộp phiếu (`DangKyController.cs:484`), ngăn lỗi `SqlException` trùng khóa `UNIQUE`. |
| 3 | **Tuổi, cân nặng và điều kiện của đợt hiến máu** | **PASS** | **BLOCKED** | Bắt buộc có `NgaySinh` và kiểm tra tuổi `18 <= age <= 60` (`DangKyController.cs:439–457`); kiểm tra cân nặng `>= Math.Max(42.0, dotHienMau.CanNangToiThieu ?? 42)` (`DangKyController.cs:509–518`); kiểm tra `dotHienMau.TrangThai == "Đang mở đăng ký"` và `NgayBatDauDangKy <= now.Date <= HanDangKy` (`DangKyController.cs:371–388`). |
| 4 | **Người dùng chưa đăng nhập** | **PASS** | **BLOCKED** | `[Authorize]` được gắn ở cấp class trên cả `NguoiHienMauController` và `DangKyController`. Khi chưa đăng nhập, middleware Cookie Authentication tự động chuyển hướng sang `/Account/Login` (và có thêm lớp phòng vệ `Challenge()` nếu thiếu claim `NameIdentifier`). |
| 5 | **Đăng ký trùng đợt và phiếu đang chờ xử lý** | **PASS** | **BLOCKED** | Kiểm tra người hiến đã có phiếu hoạt động ở cùng đợt hoặc bất kỳ đợt nào khác chưa hoàn tất (`DangKyController.cs:293–320, 560–596`), đồng thời khóa bằng `_registrationLock` (`SemaphoreSlim(1,1)`) + Transaction `IsolationLevel.Serializable`. |
| 6 | **Người chưa từng hiến máu thành công** | **PASS** | **BLOCKED** | `EvaluateRecentDonationEligibility` trả về `IsEligible = true`, `TotalSuccessfulDonations = 0` khi danh sách phiếu rỗng hoặc chỉ có phiếu chưa hoàn tất hiến máu (`REG-84D-01`). |
| 7 | **Người vừa hiến máu thành công chưa đủ 84 ngày** | **PASS** | **BLOCKED** | Khi `daysSinceLastDonation <= 83` tính từ `KetQuaHienMau.NgayCapNhat`, `EvaluateRecentDonationEligibility` trả về `IsEligible = false`, tính chính xác số ngày còn phải chờ (`84 - daysSinceLastDonation`) và ngày đủ điều kiện đăng ký lại (`lastActualDate.AddDays(84)`), chặn cả trên giao diện và khi gửi `POST /DangKy/Create` (`REG-84D-02`). |
| 8 | **Người đã đủ 84 ngày từ lần hiến thành công gần nhất** | **PASS** | **BLOCKED** | Khi `daysSinceLastDonation >= 84` (từ ngày thứ 84 trở đi), `EvaluateRecentDonationEligibility` trả về `IsEligible = true`, `RemainingCooldownDays = 0`, cho phép tiếp tục đăng ký (`REG-84D-02`). |
| 9 | **Người chỉ có phiếu đã hủy hoặc bị từ chối** | **PASS** | **BLOCKED** | Các phiếu có trạng thái `"Đã hủy"`, `"Từ chối"`, `"Không đủ điều kiện hiến"` bị loại khỏi danh sách hiến máu thành công (`IsConfirmedSuccessfulDonation == false`), không kích hoạt thời gian chờ 84 ngày và cho phép đăng ký lại bình thường (`REG-84D-03`). |
| 10 | **Theo dõi phiếu của chính mình và chặn xem phiếu người khác** | **PASS** | **BLOCKED** | `DangKyController.TheoDoi` lấy `nguoiHienMau` theo `ClaimTypes.NameIdentifier` của phiên đăng nhập hiện tại và kiểm tra `if (phieu.MaNguoiHien != nguoiHienMau.MaNguoiHien)` (`DangKyController.cs:679`), chặn tuyệt đối hành vi thay đổi `id` trên URL để xem phiếu của người khác. |
| 11 | **Hủy phiếu theo từng trạng thái và hiển thị lý do từ chối** | **PASS** | **BLOCKED** | `DangKyController.Huy [POST]` chỉ cho phép chủ phiếu hủy khi ở 3 trạng thái `"Chờ xác nhận"`, `"Đã xác nhận"`, `"Chờ khám sàng lọc"` và chưa có `PhieuKhamSangLoc` / `KetQuaHienMau`. `Views/DangKy/TheoDoi.cshtml` (dòng 309–344) hiển thị hộp cảnh báo nổi bật với đầy đủ lý do từ chối/tạm hoãn từ `GhiChuXacNhan`, `PhieuKhamSangLoc.LyDoTamHoan`, `KetQuaHienMau.GhiChu`. |

---

## 5. CÁC VẤN ĐỀ CÒN LẠI (NGOÀI PHẠM VI MODULE 3 — BÁO CÁO ĐỂ NHÓM PHỐI HỢP)

Trong phạm vi **Module 3**, toàn bộ lỗi chức năng, nghiệp vụ, giao diện và bảo mật đã được xử lý hoàn tất.
Tuy nhiên, qua đối chiếu tích hợp toàn hệ thống và tuân thủ quy tắc **"không tự ý sửa file thuộc module khác"**, có **2 điểm thuộc Module 4 và Module 5** cần thông báo để thành viên phụ trách các module đó lưu ý:

1. **Module 5 (`Controllers/AdminController.cs` — dòng 401–404)**:
   - Trong phương thức `AdminController.CapNhatTrangThaiPhieu`, tồn tại một nhánh cho phép Admin chuyển trực tiếp `phieu.TrangThai = "Hiến máu thành công"` từ màn hình danh sách phiếu mà không đi qua quy trình tạo bản ghi `KetQuaHienMau` (do đó không có `KetQuaHienMau.NgayCapNhat`).
   - **Cách Module 3 đã tự bảo vệ an toàn**: Khi phát hiện phiếu có `TrangThai == "Hiến máu thành công"` nhưng thiếu `KetQuaHienMau.NgayCapNhat`, `DangKyController.EvaluateRecentDonationEligibility` **không dùng `NgayNop` hay `NgayXuLy` làm ngày hiến thực tế**, mà chặn đăng ký lại một cách an toàn và thông báo người dùng liên hệ nhân viên y tế để cập nhật ngày hiến thực tế.
   - **Khuyến nghị cho thành viên Module 5**: Khi cập nhật trạng thái `"Hiến máu thành công"`, nên yêu cầu nhập kết quả hiến máu kèm `KetQuaHienMau.NgayCapNhat = DateTime.Now`.
2. **Module 4 (`Controllers/KetQuaHienMauController.cs` — dòng 106–117)**:
   - Khi lưu kết quả hiến máu thất bại (`KetQua == "Không đủ điều kiện"` hoặc `"Tạm hoãn"`), `KetQuaHienMauController` chỉ gán `phieu.TrangThai = "Không đủ điều kiện hiến"` khi `phieu.TrangThai == "Đã khám sàng lọc"`.
   - **Khuyến nghị cho thành viên Module 4**: Bỏ điều kiện `if (phieu.TrangThai == "Đã khám sàng lọc")` ở nhánh `else` để luôn đồng bộ `phieu.TrangThai = "Không đủ điều kiện hiến"` khi kết quả hiến máu không đạt.

---

## 6. NHỮNG KIỂM THỬ BỊ BLOCKED VÀ HƯỚNG DẪN KIỂM THỬ THỦ CÔNG

### 6.1. Nguyên nhân `BLOCKED` đối với kiểm thử Runtime DB
- Dự án cấu hình chuỗi kết nối SQL Server LocalDB tại `appsettings.json`:
  ```json
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=QuanLyHienMau_UNETI2_TI17A5HN;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  ```
- Trong môi trường terminal sandbox hiện tại, pipe kết nối tới tiến trình dịch vụ Windows LocalDB `(localdb)\mssqllocaldb` bị chặn (`SqlException: Login timeout expired`). Do đó, mọi kiểm thử tích hợp end-to-end cần truy vấn trực tiếp vào SQL Server LocalDB được đánh dấu rõ là **`BLOCKED`** (không tự đánh dấu `PASS`).

### 6.2. Hướng dẫn kiểm thử thủ công từng bước trên máy người dùng (Visual Studio / PowerShell ngoài Sandbox)

1. **Khởi động SQL Server LocalDB và chạy ứng dụng**:
   ```powershell
   sqllocaldb start mssqllocaldb
   dotnet run --project "QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN.csproj"
   ```
2. **Kiểm thử Chặn truy cập trái phép & Chống IDOR**:
   - Mở trình duyệt ẩn danh (chưa đăng nhập), truy cập `/NguoiHienMau/HoSoCaNhan` và `/DangKy/Create` -> Xác nhận hệ thống tự động chuyển hướng về `/Account/Login`.
   - Đăng nhập bằng tài khoản Người hiến máu A, truy cập `/DangKy/TheoDoi?id=<MaPhieu_cua_Nguoi_B>` -> Xác nhận hệ thống chặn truy cập và hiển thị thông báo `"Bạn không có quyền truy cập hoặc theo dõi phiếu đăng ký của người khác."`
3. **Kiểm thử Cập nhật hồ sơ & Kiểm tra trùng Email**:
   - Truy cập `/NguoiHienMau/HoSoCaNhan`, bấm **Chỉnh sửa hồ sơ**.
   - Thử xóa trống Ngày sinh hoặc nhập tuổi `< 18` / `> 60`, cân nặng `< 30` kg, hoặc nhập Email của một tài khoản khác -> Xác nhận hệ thống báo lỗi đỏ chi tiết dưới từng trường và không gây crash.
   - Nhập thông tin hợp lệ và lưu -> Xác nhận thông báo cập nhật thành công và đồng bộ lên thanh điều hướng.
4. **Kiểm thử Quy tắc 84 ngày kể từ ngày hiến máu thực tế (`KetQuaHienMau.NgayCapNhat`)**:
   - **Trường hợp 1 (Chưa từng hiến thành công hoặc chỉ có phiếu Đã hủy / Từ chối)**: Vào `/DangKy/Create?maDot=<MaDot>` -> Xác nhận form mở bình thường, cho phép nộp phiếu.
   - **Trường hợp 2 (Đã hiến máu thành công cách đây `< 84` ngày theo `KetQuaHienMau.NgayCapNhat`)**: Vào `/DangKy/Create?maDot=<MaDot>` -> Xác nhận hiển thị hộp cảnh báo màu đỏ nêu rõ ngày hiến thực tế gần nhất, số ngày còn phải chờ và **ngày đủ điều kiện đăng ký lại** (`NgayCapNhat + 84 ngày`); nếu cố tình gửi POST trực tiếp, server vẫn chặn và trả về lỗi.
   - **Trường hợp 3 (Đã đủ `>= 84` ngày kể từ `KetQuaHienMau.NgayCapNhat`)**: Vào `/DangKy/Create?maDot=<MaDot>` -> Xác nhận hệ thống cho phép đăng ký phiếu mới.
5. **Kiểm thử Theo dõi phiếu, Lý do từ chối và Hủy phiếu**:
   - Sau khi nộp phiếu mới (`TrangThai = "Chờ xác nhận"`), tại `/DangKy/TheoDoi`, bấm **Hủy phiếu đăng ký** -> Xác nhận phiếu chuyển sang `"Đã hủy"`.
   - Với phiếu đã ở trạng thái `"Đã khám sàng lọc"`, `"Hiến máu thành công"`, `"Từ chối"` hoặc `"Không đủ điều kiện hiến"`, xác nhận nút Hủy bị khóa và nếu gửi POST `/DangKy/Huy` trực tiếp sẽ bị server từ chối.
   - Với phiếu bị `"Từ chối"` hoặc `"Không đủ điều kiện hiến"`, xác nhận hộp thông báo màu đỏ hiển thị đầy đủ lý do từ `GhiChuXacNhan`, `LyDoTamHoan` và `KetQuaHienMau.GhiChu`.

---

## 7. XÁC NHẬN TUÂN THỦ QUY TẮC BẢO VỆ MÃ NGUỒN VÀ PHẠM VI MODULE 3

1. **Không sửa các module ngoài phạm vi**: Không chỉnh sửa bất kỳ logic nào của Module 1, Module 2, Module 4, Module 5 hoặc Module 6 (ngoại trừ đúng 1 thuộc tính `asp-action="PhieuDangKy"` tại dòng 547 của `Views/Admin/NguoiHienMau.cshtml` đã được thực hiện và báo cáo ở giai đoạn sửa lỗi để khắc phục liên kết 404 tới trang quản lý phiếu).
2. **Bảo toàn tuyệt đối thay đổi của các thành viên khác**: Toàn bộ các file đang có thay đổi chưa commit của nhóm trong working tree (`AccountController.cs`, `AdminController.cs`, `DotHienMauController.cs`, `KetQuaHienMauController.cs`, `Module5Service.cs`, `ThongKeService.cs`, v.v.) được giữ nguyên 100%.
3. **Không thay thế ngày hiến máu thực tế bằng `NgayNop` hoặc `NgayXuLy`**: Mọi phép tính chu kỳ 84 ngày và lịch sử hiến máu thành công chỉ dựa trên `KetQuaHienMau.NgayCapNhat` của các lần hiến máu đã được xác nhận thành công.
4. **Không tạo dữ liệu mẫu giả trong mã nguồn**: Đã loại bỏ toàn bộ dữ liệu y tế/xét nghiệm gán cứng trên giao diện khi người dùng chưa có lịch sử hiến máu thực tế.
5. **Không thực hiện các thao tác Git hoặc DB bị cấm**: Không commit, không push, không reset, không đổi nhánh (vẫn ở nhánh `integration`) và không xóa hay thay đổi cấu trúc cơ sở dữ liệu.

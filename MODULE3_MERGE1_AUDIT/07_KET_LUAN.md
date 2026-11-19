# 07. KẾT LUẬN CUỐI CÙNG & ĐÁNH GIÁ MỨC ĐỘ HOÀN THIỆN MODULE 3

Tài liệu này tổng hợp đánh giá mức độ hoàn thiện của **Module 3 (Quản lý Người hiến máu & Phiếu đăng ký hiến máu)** trên nhánh `origin/merge1` và trả lời trực tiếp 8 câu hỏi kết luận bắt buộc.

---

## Trả lời 8 Câu hỏi Kết luận

### 1. Đã lấy đúng code mới nhất từ `origin/merge1` chưa? Commit hash nào?
- **Đã lấy đúng 100% code mới nhất từ `origin/merge1`.**
- **Commit hash hiện tại (`HEAD` == `origin/merge1`):**
  `0b0d7d6926094f398cdbfd88e18658570650be63` (*"Merge"*, Author: `vutrunganh27012005`, Date: `2026-04-16 09:05:31 +0700`).
- Toàn bộ thay đổi của nhánh `DamThuTrang_23103100243` (commit `2434c70631c534c08888b7b3069dd7a0b6f34f03`) đã nằm trọn vẹn trong lịch sử của `origin/merge1` (tại commit merge `f5a20cf`). Không có file chưa commit nào bị mất trong quá trình cập nhật.

---

### 2. Module 3 hiện có bao nhiêu chức năng?
Đối chiếu yêu cầu đề bài với source code thực tế trên nhánh `merge1`, Module 3 bao gồm **12 chức năng / nghiệp vụ chính** chia thành 2 phân hệ:
- **Phân hệ Cổng Người hiến máu (9 chức năng):**
  1. Xem và tự động khởi tạo hồ sơ cá nhân người hiến máu (`GET /NguoiHienMau/HoSoCaNhan`).
  2. Cập nhật thông tin hồ sơ cá nhân & đồng bộ tài khoản (`POST /NguoiHienMau/HoSoCaNhan`).
  3. Đăng ký tham gia hiến máu theo đợt (`GET & POST /DangKy/Create`).
  4. Kiểm tra điều kiện mở đăng ký của đợt hiến máu (trạng thái đợt & thời hạn đăng ký).
  5. Kiểm tra điều kiện độ tuổi (18–60), cân nặng (30–200kg và `>= CanNangToiThieu` của đợt) và khảo sát sức khỏe (`health_q1`, `health_q2`).
  6. Kiểm tra khoảng cách tối thiểu **84 ngày** kể từ ngày hiến máu thành công thực tế gần nhất (`KetQuaHienMau.NgayCapNhat`).
  7. Kiểm tra chống đăng ký trùng đợt hiến máu (bảo vệ đồng thời bằng `SemaphoreSlim` + `Serializable` Transaction).
  8. Theo dõi trạng thái phiếu đăng ký, lịch khám sàng lọc và kết quả hiến máu theo mã `#DKHM-xxxxx` (`GET /DangKy/TheoDoi`).
  9. Chủ động hủy phiếu đăng ký hiến máu trực tuyến (`POST /DangKy/Huy`).
- **Phân hệ Quản trị Người hiến máu phía Admin (3 chức năng):**
  10. Quản lý danh sách người hiến máu, tìm kiếm đa tiêu chí, lọc theo nhóm máu ABO/Rh/trạng thái/mức đóng góp và phân trang (`GET /Admin/NguoiHienMau`).
  11. Khóa / mở khóa người hiến máu và thu hồi phiên đăng nhập tức thời (thực hiện thông qua `POST /Admin/KhoaMoKhoaTaiKhoan` tại `/Admin/NhanSu`, tự động đồng bộ `NguoiHienMau.TrangThai`).
  12. Thống kê người hiến máu và các chỉ số KPI liên quan (trên `/Admin/NguoiHienMau` và `/NguoiHienMau/HoSoCaNhan`).

---

### 3. Bao nhiêu chức năng đã kiểm thử thành công?
- **09 / 12 chức năng hoạt động đúng và vượt qua toàn bộ các bài kiểm thử chức năng + bảo mật** (bao gồm các chức năng: `1, 2, 4, 5, 6 (cổng Donor), 7, 8, 9` và phần tìm kiếm/lọc/phân quyền của chức năng `10`).
- Cụ thể trên tổng số **30 Test Cases** thực thi trực tiếp: **28/30 Test Cases đạt PASS** (`93.3%`), **02/30 Test Cases phát hiện lỗi FAIL** (`BUG-M3-01` và `BUG-M3-02`).

---

### 4. Bao nhiêu chức năng bị lỗi?
Có **03 / 12 chức năng** đang có lỗi logic hoặc lỗi giao diện cần khắc phục:
1. **Chức năng 3 (Đăng ký tham gia hiến máu – Cập nhật nhóm máu):** Mắc lỗi **`BUG-M3-01` (High)** trên `Views/DangKy/Create.cshtml` (dòng 274–296) làm người hiến máu có nhóm máu hiếm `Rh(-)` (`O-`, `A-`, `B-`, `AB-`) bị tự động chọn nhầm thành `Rh(+)` và bị ghi đè mất nhóm máu `Rh(-)` khi nộp phiếu.
2. **Chức năng 10 & 12 (Quản lý danh sách & Thống kê KPI người hiến máu phía Admin):** Mắc lỗi **`BUG-M3-02` (High)** tại `AdminController.NguoiHienMau` (dòng 681–691, 764–800) và `Views/Admin/NguoiHienMau.cshtml` (dòng 391, 401–404) do dùng điều kiện `p.TrangThai == "Hiến máu thành công" || p.KetQuaHienMau != null`, khiến phiếu có kết quả `"Không đủ điều kiện hiến"` bị tính nhầm là đã hiến máu thành công và bị đưa vào danh sách chờ 84 ngày; ngoài ra nút `"Thêm Hồ Sơ Mới"` trên `/Admin/NguoiHienMau` trỏ nhầm sang `/DangKy/Create` (**`BUG-M3-04` - Medium**).

---

### 5. Có chức năng nào chưa hoàn thiện không?
Có **01 chức năng hoàn thiện một phần (Partial)**:
- **Chức năng 11 (Khóa / mở khóa người hiến máu phía Admin – `BUG-M3-03`):**
  - Đã có chức năng khóa/mở khóa tài khoản tại `/Admin/NhanSu` (`POST /Admin/KhoaMoKhoaTaiKhoan`), tự động đồng bộ `NguoiHienMau.TrangThai = "Bị khóa" / "Hoạt động"` và tự động đá văng (sign-out) phiên đăng nhập của người hiến máu ngay lập tức.
  - Tuy nhiên, ngay trên bảng `/Admin/NguoiHienMau` chưa có nút bấm Khóa/Mở khóa trực tiếp từng hồ sơ người hiến máu (do đó Admin phải chuyển qua trang `/Admin/NhanSu` để khóa, và chưa khóa được các hồ sơ mẫu không gắn `MaTaiKhoan`).

---

### 6. Có lỗi nào ảnh hưởng đến Database hoặc các Module khác không?
- **Cấu trúc Database, Migration và `ApplicationDbContext` hoàn toàn ổn định**, không có lỗi biên dịch hay lỗi миграции (migration). Bất kỳ thành viên nào clone nhánh `merge1` về máy đều chạy được ngay (`Clone & Run` đạt 100%).
- **Ảnh hưởng liên Module cần lưu ý:**
  1. **`BUG-M3-01` & `BUG-M3-05`:** Khi người hiến máu nộp phiếu tại `/DangKy/Create` (Module 3), `NguoiHienMau.NhomMau` bị lưu theo chuỗi `"O Rh(+)"` thay vì `"O+"` (chuẩn của Module 1 `AccountController` và `HoSoCaNhan.cshtml`), đồng thời làm mất nhóm máu âm `Rh(-)` nếu người hiến có nhóm máu hiếm. Điều này ảnh hưởng tới bộ lọc nhóm máu `Rh(-)` ở trang Admin và hiển thị ở Module 4/5.
  2. **`BUG-M3-02`:** Khi Bác sĩ ở Module 5 nhập `KetQuaHienMau` với kết quả `"Không đủ điều kiện hiến"`, trang `/Admin/NguoiHienMau` (Module 3 Admin) lại đếm bản ghi `KetQuaHienMau != null` đó thành 1 lần hiến máu thành công.
  3. **`BUG-M3-07`:** Sau khi merge nhánh `merge1`, trong project đang tồn tại song song 2 cặp interface `IRepository<T>` và `IPhieuDangKyRepository` ở 2 namespace (`Repositories` của Module 1/4 và `Repositories.Interfaces` của Module 3). Dù `Program.cs` đã đăng ký DI đầy đủ cho cả hai và không gây lỗi chạy, nhóm nên lưu ý khi giải thích kiến trúc trước giảng viên.

---

### 7. Module 3 đã đủ điều kiện để nghiệm thu và bảo vệ trước giảng viên chưa?
- **Đánh giá tổng thể: Đạt khoảng 88% – 90% yêu cầu nghiệm thu.**
  - **Điểm mạnh rất lớn khi bảo vệ:**
    - Toàn bộ luồng cốt lõi phía Người hiến máu (`HoSoCaNhan`, `DangKy/Create`, `DangKy/TheoDoi`, `DangKy/Huy`) đã chuyển sang **Repository Pattern** (`INguoiHienMauRepository`, `IPhieuDangKyRepository`) sạch sẽ, rõ ràng.
    - Các quy tắc nghiệp vụ khó mà giảng viên thường hỏi xoáy (như **quy tắc 84 ngày chỉ tính trên lần hiến máu thành công thực tế `KetQuaHienMau.NgayCapNhat`**, **phiếu hủy/từ chối không bị tính 84 ngày**, **chống đăng ký trùng đợt bằng `SemaphoreSlim` + `Serializable` Transaction**, **chặn IDOR khi xem/hủy phiếu của người khác**, **thu hồi phiên đăng nhập tức thời khi bị khóa**) đều đã được xử lý rất chắc chắn và vượt qua kiểm thử thực tế.
  - **Khuyến nghị trước buổi bảo vệ:**
    - Nên dành khoảng **15–20 phút** để sửa dứt điểm 2 lỗi mức **High** (`BUG-M3-01` và `BUG-M3-02`) trước khi bảo vệ chính thức để tránh trường hợp giảng viên test nhóm máu hiếm `O-` trên form đăng ký hoặc đối chiếu số lần hiến máu giữa trang Admin `/Admin/NguoiHienMau` và trang Hồ sơ cá nhân.

---

### 8. Những lỗi nào cần ưu tiên sửa trước?
Thứ tự ưu tiên khắc phục đề xuất (từ cao xuống thấp):

1. **Ưu tiên số 1 (`BUG-M3-02` – High):** Đồng bộ điều kiện tính lần hiến máu thành công và ngày chờ 84 ngày trong `AdminController.NguoiHienMau` (dòng 681–691, 764–800) và `Views/Admin/NguoiHienMau.cshtml` (dòng 391, 401–404) sang dùng `DangKyController.IsConfirmedSuccessfulDonation(p)` và `DangKyController.GetActualDonationDate(p)`.
2. **Ưu tiên số 2 (`BUG-M3-01` & `BUG-M3-05` – High/Medium):** Chuẩn hóa giá trị nhóm máu trên `Views/DangKy/Create.cshtml` (dòng 274–296) về đúng 8 nhóm máu (`O+`, `A+`, `B+`, `AB+`, `O-`, `A-`, `B-`, `AB-`) + `UNKNOWN`, và sửa điều kiện `checked` từ `.Contains(...)` sang so sánh bằng chính xác (`==`) để không bao giờ ghi đè mất nhóm máu hiếm `Rh(-)`.
3. **Ưu tiên số 3 (`BUG-M3-03` & `BUG-M3-04` – Medium):** Sửa link nút `"Thêm Hồ Sơ Mới"` trên `Views/Admin/NguoiHienMau.cshtml` (dòng 62) và bổ sung nút Khóa/Mở khóa trực tiếp hồ sơ người hiến máu trên bảng `/Admin/NguoiHienMau`.
4. **Ưu tiên số 4 (`BUG-M3-06`, `BUG-M3-07`, `BUG-M3-08` – Low):** Kiểm tra bắt buộc không rỗng đối với `ngaySinh`/`canNang` gửi từ form `/DangKy/Create`, sửa phép so sánh `.Date` trong `GetLatestOpenDotHienMauAsync`, và chuyển `AdminController.NguoiHienMau` sang dùng `INguoiHienMauRepository`.

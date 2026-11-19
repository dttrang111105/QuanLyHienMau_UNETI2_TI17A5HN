# BÁO CÁO SỬA LỖI LUỒNG ĐIỀU HƯỚNG ĐĂNG KÝ HIẾN MÁU – MODULE 3

- **Dự án:** `QuanLyHienMau_UNETI2_TI17A5HN` (ASP.NET Core MVC - .NET 10.0)
- **Nhánh làm việc:** `merge1` (đồng bộ với `origin/merge1` tại commit `0b0d7d6926094f398cdbfd88e18658570650be63`)
- **Phạm vi thực hiện:** Sửa lỗi điều hướng luồng đăng ký hiến máu (từ nút **“ĐĂNG KÝ HIẾN MÁU”** trên thanh điều hướng góc trên bên phải -> danh sách đợt hiến máu -> chọn đợt đang mở -> biểu mẫu đăng ký tương ứng với đợt đã chọn).

---

## 1. Nguyên nhân lỗi ban đầu

Qua kiểm tra source code thực tế trên nhánh `merge1`, luồng đăng ký hiến máu gặp 4 nguyên nhân gốc rễ sau:

1. **Nút “ĐĂNG KÝ HIẾN MÁU” trên thanh điều hướng (`Views/Shared/_Layout.cshtml`) và trang chủ (`Views/Home/Index.cshtml`) trỏ thẳng vào `DangKy/Create` không kèm mã đợt:**
   - Tại `Views/Shared/_Layout.cshtml` (dòng 161): Nút **“ĐĂNG KÝ HIẾN MÁU”** ở góc trên bên phải được gắn `asp-controller="DangKy" asp-action="Create"` mà không đi qua danh sách đợt hiến máu (`DotHienMau/Index`). Tương tự, liên kết ở Footer (dòng 197) và mục “Quy trình” trên thanh menu (dòng 106) cũng trỏ trực tiếp vào `DangKy/Create`.
   - Tại `Views/Home/Index.cshtml` (các dòng 31, 124, 652): Các nút kêu gọi đăng ký chung trên trang chủ trỏ thẳng tới `DangKy/Create` không có `maDot`, và 3 thẻ đợt hiến máu tiêu biểu trên trang chủ (dòng 281, 334, 386) cũng thiếu `asp-route-maDot`.

2. **Nút “Đăng ký ngay” trên trang danh sách đợt hiến máu (`Views/DotHienMau/Index.cshtml`) điều hướng sai sang trang Chi tiết:**
   - Tại `Views/DotHienMau/Index.cshtml` (dòng 530–533): Nút hành động chính của từng thẻ đợt hiến máu hiển thị nhãn `"Đăng ký ngay"` khi `coTheDangKy == true`, nhưng thuộc tính điều hướng lại viết là:
     ```html
     <a asp-controller="DotHienMau" asp-action="Details" asp-route-id="@item.MaDot" ...>
         <span>@(coTheDangKy ? "Đăng ký ngay" : "Xem chi tiết")</span>
     </a>
     ```
   - Khi `coTheDangKy == false` (đợt `"Chưa mở"`, `"Tạm dừng"`, `"Đã đóng"`, chưa tới hạn hoặc đã hết hạn đăng ký), giao diện lại hiển thị thêm một nút `"Xem chi tiết"` thứ hai nằm cạnh nút `"Xem chi tiết"` sẵn có (dòng 436) thay vì khóa nút đăng ký.

3. **Action `GET /DangKy/Create` tự động lấy đại một đợt hiến máu khi người dùng truy cập trực tiếp không có `maDot`:**
   - Tại `Controllers/DangKyController.cs` (dòng 232–240): Khi `maDot` bị bỏ trống (`null`), action `Create` gọi `_phieuDangKyRepo.GetLatestOpenDotHienMauAsync(DateTime.Today)` để tự động gán một đợt hiến máu bất kỳ đang mở, khiến người dùng có thể vào thẳng trang lập phiếu mà chưa hề chọn đợt hiến máu cụ thể.

4. **Trang chi tiết đợt hiến máu (`Views/DotHienMau/Details.cshtml`) chưa hiển thị thông báo `TempData["ErrorMessage"]`:**
   - Khi Backend chặn người dùng truy cập `DangKy/Create?maDot=...` đối với đợt đã đóng/chưa mở/hết hạn và chuyển hướng về `DotHienMau/Details/{id}`, view `Details.cshtml` chưa có vùng hiển thị `TempData["ErrorMessage"]` để thông báo rõ lý do không thể đăng ký.

---

## 2. Những file đã chỉnh sửa

Chỉ chỉnh sửa **5 file** thuộc đúng luồng điều hướng đăng ký hiến máu, hoàn toàn không can thiệp vào các chức năng khác của Module 3 hay các module của thành viên khác:

1. `QuanLyHienMau_UNETI2_TI17A5HN/Views/Shared/_Layout.cshtml`
2. `QuanLyHienMau_UNETI2_TI17A5HN/Views/Home/Index.cshtml`
3. `QuanLyHienMau_UNETI2_TI17A5HN/Views/DotHienMau/Index.cshtml`
4. `QuanLyHienMau_UNETI2_TI17A5HN/Views/DotHienMau/Details.cshtml`
5. `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs` & `QuanLyHienMau_UNETI2_TI17A5HN/Views/DangKy/Create.cshtml`

---

## 3. Những thay đổi cụ thể

### 3.1. `Views/Shared/_Layout.cshtml`
- **Nút “ĐĂNG KÝ HIẾN MÁU” góc trên bên phải (dòng 161):** Đổi từ `asp-controller="DangKy" asp-action="Create"` sang `asp-controller="DotHienMau" asp-action="Index"`, đồng thời tối ưu class responsive (`text-xs sm:text-label-md px-3.5 sm:px-5 py-2 sm:py-2.5 whitespace-nowrap`) để hiển thị gọn đẹp trên cả màn hình mobile và desktop.
- **Liên kết Footer “Đăng ký hiến máu” (dòng 197):** Đổi từ `asp-controller="DangKy" asp-action="Create"` sang `asp-controller="DotHienMau" asp-action="Index"`.
- **Liên kết menu “Quy trình” (dòng 106):** Đổi từ `asp-controller="DangKy" asp-action="Create"` sang `asp-controller="Home" asp-action="Index" asp-fragment="process"`.

### 3.2. `Views/Home/Index.cshtml`
- **Các nút CTA chung (Hero dòng 31, Quick Action Card 2 dòng 124, Bottom CTA dòng 652):** Đổi từ `asp-controller="DangKy" asp-action="Create"` sang `asp-controller="DotHienMau" asp-action="Index"`.
- **Các thẻ đợt hiến máu tiêu biểu trên trang chủ (dòng 281, 334, 386):** Bổ sung `asp-route-maDot="1"`, `asp-route-maDot="2"`, `asp-route-maDot="3"` tương ứng với mã đợt của từng thẻ.

### 3.3. `Views/DotHienMau/Index.cshtml`
- **Nút đăng ký trên từng thẻ đợt hiến máu (dòng 530–543):**
  - Khi `coTheDangKy == true` (`item.CoTheDangKy`, tức `TrangThai == "Đang mở đăng ký"` và `NgayBatDauDangKy.Date <= DateTime.Today <= HanDangKy.Date`): Hiển thị nút `<a asp-controller="DangKy" asp-action="Create" asp-route-maDot="@item.MaDot" ...>Đăng ký ngay</a>` chuyển thẳng sang biểu mẫu đăng ký của đợt đó.
  - Khi `coTheDangKy == false`: Hiển thị nút `<button type="button" disabled ...>` kèm biểu tượng khóa (`block`) và trạng thái thực tế (`@(item.TrangThai != "Đang mở đăng ký" ? item.TrangThai : item.ThoiHanHienThi)`), không cho phép nhấn đăng ký trên giao diện (nút `"Xem chi tiết"` bên cạnh vẫn hoạt động bình thường).

### 3.4. `Views/DotHienMau/Details.cshtml`
- Bổ sung khối hiển thị thông báo `TempData["SuccessMessage"]` và `TempData["ErrorMessage"]` ngay dưới thanh Breadcrumb (dòng 29–42) để hiển thị thông báo lỗi khi người dùng cố tình truy cập URL đăng ký của đợt không đủ điều kiện.

### 3.5. `Controllers/DangKyController.cs` & `Views/DangKy/Create.cshtml`
- **`GET /DangKy/Create(int? maDot, int? maDotHienMau = null, int? id = null)`:**
  - Loại bỏ việc tự động gọi `GetLatestOpenDotHienMauAsync` khi không có `maDot`.
  - Bắt buộc kiểm tra `selectedMaDot = maDot ?? maDotHienMau ?? id`. Nếu `!selectedMaDot.HasValue || selectedMaDot.Value <= 0`, gán `TempData["ErrorMessage"] = "Vui lòng chọn một đợt hiến máu đang mở đăng ký từ danh sách trước khi lập phiếu."` và `RedirectToAction("Index", "DotHienMau")`.
  - Nếu `dotHienMau == null`, gán `TempData["ErrorMessage"]` và `RedirectToAction("Index", "DotHienMau")`.
  - Giữ nguyên đầy đủ các bước kiểm tra chặt chẽ ở Backend: trạng thái đợt phải là `"Đang mở đăng ký"`, ngày hiện tại phải nằm trong khoảng `[NgayBatDauDangKy.Date, HanDangKy.Date]`, kiểm tra tài khoản/hồ sơ hoạt động, kiểm tra khoảng cách 84 ngày và kiểm tra trùng phiếu.
- **`POST /DangKy/Create` & `Views/DangKy/Create.cshtml`:**
  - Truyền đồng thời `asp-route-maDot="@Model.MaDot"`, `<input type="hidden" name="maDot" value="@Model.MaDot" />` và `<input type="hidden" name="maDotHienMau" value="@Model.MaDot" />`.
  - Tại Backend `POST /DangKy/Create`, kiểm tra chặt chẽ `selectedMaDot <= 0` hoặc `dotHienMau == null` (chuyển hướng an toàn về `DotHienMau/Index`), kiểm tra lại trạng thái đợt và thời hạn đăng ký trước khi lưu `PhieuDangKyHienMau` với `MaDot = dotHienMau.MaDot`.

---

## 4. Luồng hoạt động trước và sau khi sửa

| Bước | Trước khi sửa | Sau khi sửa |
| :--- | :--- | :--- |
| **Bước 1:** Nhấn nút “ĐĂNG KÝ HIẾN MÁU” góc trên bên phải | Chuyển thẳng tới `/DangKy/Create` (không có mã đợt). | Chuyển đến trang danh sách đợt hiến máu `/DotHienMau` (`DotHienMau/Index`). |
| **Bước 2 & 3:** Xem danh sách đợt hiến máu (`/DotHienMau`) | Nút `"Đăng ký ngay"` trên thẻ đợt hiến máu lại trỏ về `/DotHienMau/Details/{id}`. Đợt đóng hiển thị 2 nút `"Xem chi tiết"` trùng nhau. | Hiển thị đầy đủ tên đợt, thời gian, địa điểm, trạng thái. Đợt đủ điều kiện hiển thị nút `"Đăng ký ngay"` trỏ đúng tới `/DangKy/Create?maDot={MaDot}`. Đợt đóng/chưa mở/hết hạn bị vô hiệu hóa nút đăng ký (`disabled`). |
| **Bước 4 & 5:** Chọn một đợt đang mở đăng ký | Phải qua trang Chi tiết mới thấy link `DangKy/Create`. | Chuyển trực tiếp từ danh sách (hoặc trang Chi tiết) sang `/DangKy/Create?maDot={MaDot}` của đúng đợt đã chọn. |
| **Bước 6:** Gửi biểu mẫu đăng ký | Có nguy cơ đăng ký nhầm đợt tự động lấy khi thiếu `maDot`. | `MaDot` được truyền chính xác vào biểu mẫu và lưu vào `PhieuDangKyHienMau.MaDot` liên kết đúng đợt đã chọn. |
| **Truy cập trực tiếp `/DangKy/Create` không có `maDot` hợp lệ** | Tự động lấy một đợt bất kỳ đang mở để hiển thị form. | Chặn an toàn, thông báo yêu cầu chọn đợt hiến máu và chuyển hướng về `/DotHienMau`. |

---

## 5. Kết quả từng trường hợp kiểm thử

Đã thực hiện kiểm thử tự động tích hợp (HTTP Request + SQL Verification) trên cơ sở dữ liệu kiểm thử độc lập `QuanLyHienMau_UNETI_AuditM3` (tự động dọn dẹp sau khi test, bảo toàn 100% database thật `QuanLyHienMau_UNETI`):

| STT | Trường hợp kiểm thử | Kết quả thực tế | Đánh giá |
| :---: | :--- | :--- | :---: |
| **1** | Nhấn nút “Đăng ký” (`ĐĂNG KÝ HIẾN MÁU`) trên thanh điều hướng trang chủ | Nút điều hướng chính xác tới `href="/DotHienMau"` (`HTTP 200`). Không còn bất kỳ link `/DangKy/Create` trống mã đợt nào trên trang chủ và Layout. | **PASS** |
| **2** | Danh sách đợt hiến máu hiển thị đúng dữ liệu từ database | Trang `/DotHienMau` hiển thị đầy đủ các đợt từ DB (*Giọt Hồng UNETI*, *Trái Tim Đỏ*, ...), gồm tên đợt, thời gian tổ chức, địa điểm/đơn vị, trạng thái đăng ký và các nút hành động. | **PASS** |
| **3** | Chọn một đợt đang mở đăng ký | Nút `"Đăng ký ngay"` của đợt đang mở (`MaDot = 2`) điều hướng chính xác đến `/DangKy/Create?maDot=2` (`HTTP 200`). | **PASS** |
| **4** | Kiểm tra `MaDotHienMau` (`maDot`) được truyền chính xác | Biểu mẫu `/DangKy/Create?maDot=2` hiển thị đúng tiêu đề/mã đợt `#DOT-0002` và chứa `<input type="hidden" name="maDot" value="2" />`. | **PASS** |
| **5** | Gửi đăng ký: phiếu được lưu và liên kết đúng đợt | Gửi `POST /DangKy/Create?maDot=2` với thông tin hợp lệ -> tạo phiếu thành công, kiểm tra trong bảng `PhieuDangKyHienMau` ghi nhận đúng `MaDot = 2`, `TrangThai = N'Chờ xác nhận'`. | **PASS** |
| **6** | Đợt đã đóng đăng ký (`MaDot = 3`, `TrangThai = "Đã đóng"`): không thể đăng ký | Trên `/DotHienMau`, nút đăng ký bị `disabled` (không sinh link `DangKy/Create?maDot=3`). Truy cập trực tiếp `GET` hoặc `POST /DangKy/Create?maDot=3` bị Backend chặn (`HTTP 302` về `/DotHienMau/Details/3`), số phiếu tạo trong DB = `0`. | **PASS** |
| **7** | Đợt chưa đến thời gian (`MaDot = 4`) hoặc đã hết hạn đăng ký (`MaDot = 5`): không thể đăng ký | Trên `/DotHienMau`, cả hai đợt đều có `CoTheDangKy == false`, nút đăng ký bị `disabled`. Truy cập trực tiếp `GET /DangKy/Create?maDot=4` và `GET /DangKy/Create?maDot=5` đều bị Backend chặn (`HTTP 302` về `/DotHienMau/Details/4` và `/5`), số phiếu tạo trong DB = `0`. | **PASS** |
| **8** | Truy cập trực tiếp `DangKy/Create` mà không có mã đợt hợp lệ | Thử nghiệm `GET /DangKy/Create` (thiếu `maDot`), `GET /DangKy/Create?maDot=0`, và `GET /DangKy/Create?maDot=9999` (không tồn tại) đều trả về `HTTP 302` chuyển hướng an toàn về `/DotHienMau` kèm thông báo lỗi, không tạo phiếu sai. | **PASS** |
| **9** | Kiểm tra giao diện trên cả desktop và mobile | Nút `"ĐĂNG KÝ HIẾN MÁU"` trên header có padding/text co giãn hợp lý (`text-xs sm:text-label-md px-3.5 sm:px-5 py-2 sm:py-2.5 whitespace-nowrap`), lưới danh sách đợt hiến máu đáp ứng tốt trên mobile/tablet/desktop (`grid-cols-1 md:grid-cols-2 lg:grid-cols-3`). | **PASS** |
| **10** | Kiểm tra các chức năng Module 3 hiện có không bị ảnh hưởng | Kiểm tra hồi quy: Hồ sơ cá nhân (`GET /NguoiHienMau/HoSoCaNhan` -> `HTTP 200`), Theo dõi & Hủy phiếu (`GET /DangKy/TheoDoi` -> `HTTP 200`), Quản lý người hiến máu phía Admin (`GET /Admin/NguoiHienMau` -> `HTTP 200`) đều hoạt động bình thường. | **PASS** |

---

## 6. Kết quả Build

Thực hiện biên dịch toàn bộ solution `QuanLyHienMau_UNETI2_TI17A5HN.slnx`:
- **Trạng thái biên dịch:** `Build succeeded`
- **Số lỗi biên dịch (Compiler Errors):** `0 Error(s)`
- **Số cảnh báo C# (C# Compiler Warnings):** `0 Warning(s)`

---

## 7. Các vấn đề còn tồn tại

- **Không còn lỗi tồn tại** trong luồng điều hướng và kiểm tra tính hợp lệ của đợt hiến máu khi đăng ký.
- Tuân thủ đầy đủ quy định Git: Không tự ý `commit`, không `push`, giữ nguyên nhánh `merge1`, bảo toàn toàn bộ file hiện có của dự án và không làm thay đổi dữ liệu trên database thật `QuanLyHienMau_UNETI`.

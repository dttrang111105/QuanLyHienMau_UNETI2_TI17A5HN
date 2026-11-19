# BÁO CÁO AUDIT TÍCH HỢP TOÀN HỆ THỐNG (5 MODULE) VÀ CẤU TRÚC KỸ THUẬT

- **Dự án:** Hệ thống Quản lý Đăng ký Hiến máu Tình nguyện và Lịch khám Sàng lọc (`QuanLyHienMau_UNETI2_TI17A5HN` – Đề tài 29)
- **Phạm vi kiểm tra:** Khả năng tích hợp xuyên suốt 5 Module (`Module 1` -> `Module 2` -> `Module 3` -> `Module 4` -> `Module 5`), lược đồ CSDL (`ApplicationDbContext` & Migrations), cấu hình Dependency Injection (`Program.cs`, `DangKyDichVu.cs`), tuân thủ Repository Pattern, và dữ liệu mẫu (`DbInitializer.cs`).

---

## 1. KIỂM TRA LUỒNG NGHIỆP VỤ END-TO-END XUYÊN SUỐT 5 MODULE

Luồng nghiệp vụ từ đầu đến cuối đã được kiểm thử thực tế trên SQL Server LocalDB (`QuanLyHienMau_UNETI`):

```
[Module 1] Donor đăng ký tài khoản / đăng nhập -> Tự động khởi tạo TaiKhoan(Donor) + NguoiHienMau
     │
     ▼
[Module 3] Donor cập nhật Hồ sơ cá nhân (Ngày sinh, Giới tính, Nhóm máu, Cân nặng >= 45kg, SĐT, Email)
     │
     ▼
[Module 2] Admin/Medical Staff tạo Đợt hiến máu gắn với Đơn vị tổ chức đang hoạt động -> Mở đăng ký ("Đang mở đăng ký")
     │
     ▼
[Module 3] Donor xem đợt đang mở -> Nộp phiếu đăng ký -> Tạo PhieuDangKyHienMau ("Chờ xác nhận")
     │
     ▼
[Module 4] Medical Staff kiểm tra hồ sơ -> Xác nhận phiếu -> PhieuDangKyHienMau ("Đã xác nhận", NgayXuLy, GhiChuXacNhan)
     │
     ▼
[Module 5A] Medical Staff lập lịch khám sàng lọc (kiểm tra không trùng lịch người hiến & bác sĩ)
            -> LichKhamSangLoc ("Đã lên lịch") + PhieuDangKyHienMau ("Chờ khám sàng lọc")
     │
     ▼
[Module 5A] Medical Staff đánh dấu hoàn thành khám sàng lọc
            -> LichKhamSangLoc ("Đã hoàn thành") + PhieuDangKyHienMau ("Đã khám sàng lọc")
     │
     ▼
[Module 5B] Medical Staff ghi nhận kết quả hiến máu
            -> KetQuaHienMau ("Hiến máu thành công" / "Không đủ điều kiện hiến", NgayCapNhat = Server Now)
            + PhieuDangKyHienMau đồng bộ trạng thái cuối
     │
     ▼
[Module 3 & 5C] Hồ sơ người hiến máu tự động tính số lần hiến thành công, thể tích máu, chu kỳ 84 ngày từ KetQuaHienMau.NgayCapNhat;
                Dashboard và Thống kê LINQ cập nhật theo thời gian thực.
```

---

## 2. BẢNG ĐỐI CHIẾU TÍCH HỢP KỸ THUẬT TOÀN HỆ THỐNG

| STT | Hạng mục tích hợp | Trạng thái | Vị trí code | Đánh giá chi tiết |
| :-: | :--- | :---: | :--- | :--- |
| 1 | Primary Key, Foreign Key và Navigation Property giữa 7 Entity | **PASS** | [ApplicationDbContext.cs](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Data/ApplicationDbContext.cs#L15-L70), [CauHinhKhamSangLoc.cs](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Data/CauHinhKhamSangLoc.cs#L10-L37) | 7 Entity (`TaiKhoan`, `NguoiHienMau`, `DonViToChuc`, `DotHienMau`, `PhieuDangKyHienMau`, `LichKhamSangLoc`, `KetQuaHienMau`) khớp 100% khóa chính, khóa ngoại, Unique Index và DeleteBehavior (`SetNull`, `Restrict`, `Cascade`). |
| 2 | `ApplicationDbContext` và Migrations | **PASS** | `Migrations/20260409195209_InitialCreate.cs`, `Migrations/20260417014756_Module5HardeningConstraints.cs` | Migration khởi tạo đầy đủ 7 bảng và Migration thứ hai bổ sung Check Constraints + Filtered Unique Index cho Module 5. Không có lỗi lệch schema. |
| 3 | Dependency Injection trong `Program.cs` & `DangKyDichVu.cs` | **PASS** | [Program.cs](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Program.cs#L24-L35), [DangKyDichVu.cs](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Services/DangKyDichVu.cs#L8-L18) | Tất cả Repository Interface, Repository cụ thể và Service (`IDotHienMauRepository`, `IDonViToChucRepository`, `ITaiKhoanRepository`, `INguoiHienMauRepository`, cả 2 `IPhieuDangKyRepository`, `LichKhamSangLocRepository`, `KetQuaHienMauRepository`, `ThongKeRepository`, `KhamSangLocService`, `DongHoHeThong`) đều đã được đăng ký DI đầy đủ. |
| 4 | Tuân thủ Repository Pattern (Controller không gọi trực tiếp `ApplicationDbContext`) | **PASS** | Toàn bộ 11 Controller trong `Controllers/` | Sau khi đồng bộ commit `a963aa5` và sửa lỗi Module 3, **0/11 Controller** còn inject trực tiếp `ApplicationDbContext`. Mọi truy vấn dữ liệu đều đi qua tầng Repository/Service. |
| 5 | Không xung đột tên hằng số trạng thái và vai trò giữa 5 Module | **PASS** | [TrangThaiHienMau.cs](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Models/TrangThaiHienMau.cs#L3-L32) | 3 vai trò (`"Admin"`, `"Medical Staff"`, `"Donor"`), 4 trạng thái đợt, 8 trạng thái phiếu, 3 trạng thái lịch khám và 2 kết quả hiến máu khớp chính xác từng ký tự tiếng Việt có dấu giữa cả 5 Module. |
| 6 | Tích hợp chu kỳ 84 ngày và thống kê giữa Module 3 và Module 4/5 | **PASS** | [DangKyController.cs](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L39-L178), [NguoiHienMauController.cs](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs#L32-L75) | Module 3 chỉ tính lần hiến thành công khi `Phieu.TrangThai == "Hiến máu thành công"` (hoặc `KetQuaHienMau.KetQua == "Hiến máu thành công"`) và lấy ngày hiến thực tế từ `KetQuaHienMau.NgayCapNhat` do Module 5 ghi nhận. |
| 7 | Hai Interface trùng tên `IPhieuDangKyRepository` ở 2 namespace khác nhau | **PARTIAL** | `Repositories/IPhieuDangKyRepository.cs` (Module 4) vs `Repositories/Interfaces/IPhieuDangKyRepository.cs` (Module 3) | Cả hai đều được đăng ký DI tường minh trong `Program.cs` và sử dụng alias/full-name ở `AdminController`, `DangKyController`, `NguoiHienMauController` nên biên dịch và chạy hoàn toàn bình thường (0 lỗi). Tuy nhiên về lâu dài nên gộp hoặc đổi tên để code dễ bảo trì hơn. |
| 8 | Dữ liệu mẫu trong `DbInitializer.cs` (Yêu cầu chung Mục 6) | **PARTIAL** | [DbInitializer.cs](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Data/DbInitializer.cs#L1-L296) | Xem phân tích chi tiết ở Mục 3 bên dưới về sự khác biệt giữa `DbInitializer.cs` tại `HEAD` (`0b0d7d6`) và `origin/merge1` (`a963aa5`). |

---

## 3. KIỂM TRA DỮ LIỆU MẪU (`DbInitializer.cs`) THEO YÊU CẦU ĐỀ TÀI 29

Đối chiếu định mức dữ liệu mẫu tối thiểu của Đề tài 29 với file `Data/DbInitializer.cs`:

| Bảng dữ liệu | Yêu cầu tối thiểu của Đề bài | Bản local hiện tại (`HEAD 0b0d7d6`) | Bản trên `origin/merge1` (`a963aa5`) | Trạng thái trong DB LocalDB hiện tại |
| :--- | :---: | :---: | :---: | :---: |
| `TaiKhoan` (đủ 3 vai trò) | >= 3 (Admin, Staff, Donor) | 3 tài khoản (`admin`, `bacsi`, `donor`) | 17 tài khoản (1 Admin, 4 Staff, 12 Donor) | **15** tài khoản |
| `DonViToChuc` | >= 3 đơn vị | 4 đơn vị | 5 đơn vị | **4** đơn vị |
| `DotHienMau` | >= 6 đợt | 4 đợt (`SeedTestData`) | **8 đợt** | **9** đợt |
| `NguoiHienMau` | >= 12 người hiến | 6 người hiến (`1 + 5`) | **15 người hiến** | **15** người hiến |
| `PhieuDangKyHienMau` | >= 20 phiếu | 5 phiếu | **25 phiếu** | **22** phiếu |
| `LichKhamSangLoc` | >= 10 lịch khám | 1 lịch khám | **>= 18 lịch khám** | **13** lịch khám |
| `KetQuaHienMau` | >= 10 kết quả | 1 kết quả | **>= 11 kết quả** | **11** kết quả |

> **Phát hiện quan trọng:**
> - Trong cơ sở dữ liệu SQL Server LocalDB (`QuanLyHienMau_UNETI`) trên máy hiện tại, số lượng bản ghi đã đạt và vượt toàn bộ định mức của đề bài (`15 NguoiHienMau`, `4 DonViToChuc`, `9 DotHienMau`, `22 PhieuDangKyHienMau`, `13 LichKhamSangLoc`, `11 KetQuaHienMau`).
> - Tuy nhiên, file `QuanLyHienMau_UNETI2_TI17A5HN/Data/DbInitializer.cs` trên nhánh local (`HEAD = 0b0d7d6`) vẫn đang ở phiên bản cũ (296 dòng), trong khi commit mới nhất trên `origin/merge1` (`a963aa5`) đã nâng cấp `DbInitializer.cs` lên 800 dòng để tự động nạp đủ `5 DonViToChuc`, `8 DotHienMau`, `15 NguoiHienMau`, `25 PhieuDangKyHienMau`, `>= 10 LichKhamSangLoc`, `>= 10 KetQuaHienMau` khi khởi tạo trên một máy mới hoàn toàn.
> - **Khuyến nghị:** Khi đồng bộ với `origin/merge1`, nên lấy luôn bản cập nhật `QuanLyHienMau_UNETI2_TI17A5HN/Data/DbInitializer.cs` từ `origin/merge1` (`a963aa5`) để đảm bảo khi giảng viên chạy project trên máy chấm thi mới (DB trống), hệ thống tự động seed đủ 100% số lượng bản ghi theo yêu cầu.

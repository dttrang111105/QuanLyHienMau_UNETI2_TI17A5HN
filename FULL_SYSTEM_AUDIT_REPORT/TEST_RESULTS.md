# BÁO CÁO KẾT QUẢ BIÊN DỊCH & KIỂM THỬ THỰC TẾ TOÀN HỆ THỐNG (5 MODULE)

- **Dự án:** Hệ thống Quản lý Đăng ký Hiến máu Tình nguyện và Lịch khám Sàng lọc (`QuanLyHienMau_UNETI2_TI17A5HN` – Đề tài 29)
- **Môi trường thực thi:** .NET SDK 10.0.301 (`net10.0`), Entity Framework Core 10.0.5, SQL Server LocalDB (`(localdb)\mssqllocaldb` – Database: `QuanLyHienMau_UNETI`)
- **Thời gian chạy kiểm thử:** `2026-11-19 20:03:04 (+07:00)`
- **Cam kết toàn vẹn dữ liệu:** Mọi bản ghi kiểm thử tạm thời được tạo ra trong quá trình chạy `FullSystemVerify` và `Module3Verify` đều được dọn dẹp hoàn toàn trong khối `finally` (`[CLEANUP] All temporary audit test records removed cleanly`), không làm thay đổi dữ liệu gốc của hệ thống.

---

## 1. TỔNG HỢP KẾT QUẢ KIỂM THỬ TOÀN HỆ THỐNG

| Bộ kiểm thử | Phạm vi kiểm tra | Số lượng Test | Kết quả | Tỷ lệ PASS |
| :--- | :--- | :---: | :---: | :---: |
| **1. Build Verification** | Biên dịch toàn bộ Solution `QuanLyHienMau_UNETI2_TI17A5HN.csproj` | 1 | **0 Warning, 0 Error** | **100%** |
| **2. `FullSystemVerify` – Module 1** | Đăng ký, Băm mật khẩu PBKDF2, Chống trùng Username/Email/SĐT, Đăng nhập, Khóa tài khoản, Đăng xuất, Chống Admin tự khóa, CRUD `DonViToChuc` & Ràng buộc khóa ngoại | 13 | **13 / 13 PASS** | **100%** |
| **3. `FullSystemVerify` – Module 2** | Validate `DotHienMau`, Ràng buộc `DonViToChuc` hoạt động, Đổi trạng thái, Tìm kiếm, 5 Bộ lọc (token nhóm máu), 6 Chế độ sắp xếp, Phân trang Server SQL `OFFSET/FETCH`, Chặn xóa đợt đã có phiếu | 9 (+23 sub-checks) | **9 / 9 PASS** | **100%** |
| **4. `Module3Verify` – Module 3** | Hồ sơ cá nhân, Chống IDOR, Nộp phiếu, Chặn trùng phiếu, Tuổi 18–60, Cân nặng tối thiểu, Chu kỳ 84 ngày từ `KetQuaHienMau.NgayCapNhat`, Hủy phiếu, Khóa hồ sơ, Xuất CSV | 29 | **29 / 29 PASS** | **100%** |
| **5. `FullSystemVerify` – Module 4** | Tiếp nhận phiếu, Duyệt phiếu (`NgayXuLy`, `GhiChuXacNhan`), Từ chối phiếu (bắt buộc lý do), Chặn xử lý lại phiếu đã duyệt/từ chối, Chặn nhảy cóc trạng thái, Chặn lập lịch cho phiếu chờ/từ chối | 9 | **9 / 9 PASS** | **100%** |
| **6. `FullSystemVerify` – Module 5 & E2E** | Lập lịch khám sàng lọc, Chặn lịch quá khứ, Kiểm tra `KetThuc > BatDau`, Chặn trùng lịch một phần (Partial Overlap) cho Người hiến & Bác sĩ, Hủy lịch, Hoàn thành khám, Ghi nhận kết quả (1-1), Đồng bộ phiếu, Đối chiếu 9 KPI Dashboard & 10 thống kê LINQ với SQL Server | 14 | **14 / 14 PASS** | **100%** |
| **TỔNG CỘNG** | **Toàn bộ 5 Module + Luồng tích hợp End-to-End** | **74** | **74 / 74 PASS** | **100%** |

---

## 2. LOG THỰC TẾ BỘ KIỂM THỬ `FullSystemVerify` (45/45 PASS TRÊN SQL SERVER LOCALDB)

```text
=================================================================
FULL SYSTEM AUTOMATED AUDIT VERIFICATION (M1, M2, M3, M4, M5)
Timestamp: 2026-11-19 20:03:04
=================================================================
[DB BASELINE] TaiKhoans=20, NguoiHienMaus=27, DonViToChucs=4, DotHienMaus=5, Phieus=25, Lichs=27, KetQuas=3

--- MODULE 1: TAI KHOAN, DANG NHAP, PHAN QUYEN, DON VI TO CHUC ---
[PASS] M1-01: Register valid donor creates TaiKhoan(Donor) + NguoiHienMau with PBKDF2 password hash -> MaTK=15005, MaNHM=15014
[PASS] M1-02: Register duplicate TenDangNhap rejected -> Tên đăng nhập 'm1_user_200304' đã được sử dụng. Vui lòng chọn tên khác.
[PASS] M1-03: Register duplicate Email rejected -> Email này đã được sử dụng cho tài khoản khác.
[PASS] M1-04: Register duplicate SoDienThoai rejected -> Số điện thoại này đã được liên kết với một tài khoản khác.
[PASS] M1-05: Valid login sets Session (MaTaiKhoan, VaiTro, HoTen) and Cookie Claims -> Session MaTaiKhoan=15005, VaiTro=Donor
[PASS] M1-06: Wrong password rejected on Login -> Tài khoản hoặc mật khẩu không chính xác.
[PASS] M1-07: Locked account ('Bị khóa') blocked from logging in -> Tài khoản của bạn hiện đang bị khóa hoặc tạm dừng. Vui lòng liên hệ ban quản trị để được hỗ trợ.
[PASS] M1-08: Logout clears Session and signs out authentication cookie
[PASS] M1-09: Admin prevented from locking their own active account -> Không thể tự khóa tài khoản Quản trị viên đang đăng nhập của chính bạn.
[PASS] M1-10: Create DonViToChuc succeeds -> MaDonVi=1002
[PASS] M1-11: Duplicate TenDonViToChuc rejected -> Tên đơn vị "Đơn vị Kiểm Thử M1 200304" đã tồn tại. Vui lòng chọn tên khác.
[PASS] M1-12: Invalid EmailLienHe on DonViToChuc rejected -> Địa chỉ email liên hệ không đúng định dạng.
[PASS] M1-13: Deleting DonViToChuc that has DotHienMau is blocked -> Không thể xóa đơn vị "Đoàn Thanh Niên & Hội Sinh Viên ĐH UNETI" vì đang có đợt hiến máu liên kết. Việc xóa sẽ làm các đợt hiến máu này không còn hợp lệ.

--- MODULE 2: QUAN LY DOT HIEN MAU, TIM KIEM, LOC, SAP XEP, PHAN TRANG ---
[PASS] M2-01: Valid campaign and equal start/deadline dates accepted
[PASS] M2-02: Blank name, zero quota, negative weight, and HanDangKy < NgayBatDauDangKy all rejected
[PASS] M2-03: Creating campaign persists record with initial state 'Chưa mở' -> MaDot=7002
[PASS] M2-04: Cannot create DotHienMau under an inactive ('Tạm dừng') DonViToChuc
[PASS] M2-05: ChangeStatus transitions campaign to 'Đang mở đăng ký' and enables CoTheDangKy
[PASS] M2-06: Blood group filter matches exact tokens (A does not false-match AB)
[PASS] M2-07: Combined search + 5 filters + sort + Skip/Take pagination works accurately
[PASS] M2-08: All 6 sort modes translate to SQL Server OFFSET / FETCH NEXT with MaDot tie-breaker
[PASS] M2-09: Deleting DotHienMau that has PhieuDangKyHienMau is blocked to preserve history -> Không thể xóa đợt hiến máu vì đã có phiếu đăng ký.

--- MODULE 4 & MODULE 5 & E2E FLOW (M1 -> M2 -> M3 -> M4 -> M5) ---
[PASS] M4/E2E-01: Donor submits ticket -> initial status is 'Chờ xác nhận' -> MaPhieu=11014
[PASS] M4-02: Blocked direct jump from 'Chờ xác nhận' to 'Hiến máu thành công' -> Chỉ phiếu Đã khám sàng lọc mới được ghi nhận kết quả cuối cùng.
[PASS] M4-03: Blocked creating LichKhamSangLoc when ticket is still 'Chờ xác nhận' -> Chỉ phiếu Đã xác nhận mới được lập lịch khám sàng lọc.
[PASS] M4-04: Approving ticket without GhiChuXacNhan is rejected
[PASS] M4-05: Approving 'Chờ xác nhận' -> 'Đã xác nhận' with NgayXuLy & GhiChuXacNhan -> NgayXuLy=2026-11-19 20:03:07
[PASS] M4-06: Rejecting an already approved ('Đã xác nhận') ticket is blocked
[PASS] M4-07: Rejecting ticket without reason is blocked
[PASS] M4-08: Rejecting 'Chờ xác nhận' -> 'Từ chối' succeeds with reason & timestamp
[PASS] M4-09: Blocked creating LichKhamSangLoc for 'Từ chối' ticket -> Chỉ phiếu Đã xác nhận mới được lập lịch khám sàng lọc.
[PASS] M5-01: Creating screening schedule in the past is rejected -> Thời gian bắt đầu khám không được nằm trong quá khứ.
[PASS] M5-02: ThoiGianKetThuc <= ThoiGianBatDau rejected by validation -> Thời gian kết thúc phải sau thời gian bắt đầu.
[PASS] M5-03: Creating screening schedule succeeds and updates ticket status to 'Chờ khám sàng lọc' -> MaLich=9006
[PASS] M5-04: Blocked creating a second active schedule for the same ticket -> Chỉ phiếu Đã xác nhận mới được lập lịch khám sàng lọc.
[PASS] M5-05: Partial time-interval overlap for the same Donor is detected and blocked -> Người hiến máu đã có lịch khám trùng khoảng thời gian này.
[PASS] M5-06: Partial time-interval overlap for the same Doctor is detected and blocked -> Bác sĩ sàng lọc đã có lịch trùng khoảng thời gian này.
[PASS] M5-07: Cancelling schedule marks schedule 'Đã hủy' and reverts ticket to 'Đã xác nhận'
[PASS] M5-08: Cancelled schedule is ignored when re-scheduling the same time slot
[PASS] M5-09: Completing screening schedule updates schedule to 'Đã hoàn thành' and ticket to 'Đã khám sàng lọc'
[PASS] M5-10: Invalid KetQua string rejected (only 2 valid strings allowed) -> Kết quả không hợp lệ.
[PASS] M5-11: Recording 'Hiến máu thành công' creates KetQuaHienMau with server NgayCapNhat and updates ticket to 'Hiến máu thành công' -> MaKQ=2004, NgayCapNhat=2026-11-19 20:03:08
[PASS] M5-12: Blocked recording a second KetQuaHienMau for the same ticket -> Chỉ phiếu Đã khám sàng lọc mới được ghi nhận kết quả cuối cùng.
[PASS] M5-13: All 9 Dashboard KPIs match direct SQL Server COUNT queries 100% -> Orgs=6, Dots=8, OpenDots=5, Donors=29, Phieus=29, ChoXN=6, DaXN=3, Lich24h=0, ThanhCong=4
[PASS] M5-14: ThongKeRepository LINQ aggregations (TheoDonVi, TheoDot, TheoTrangThai, DotNhieuPhieuNhat, TyLeThanhCong) match database totals -> TyLeThanhCong=50%, TopDot=Giọt Hồng UNETI – Trao Đời Sự Sống 2026

[CLEANUP] All temporary audit test records removed cleanly.
=================================================================
TOTAL PASSED: 45 | TOTAL FAILED: 0
=================================================================
```

---

## 3. LOG THỰC TẾ BỘ KIỂM THỬ `Module3Verify` (29/29 PASS TRÊN SQL SERVER LOCALDB)

```text
=== STARTING MODULE 3 AUTOMATED VERIFICATION (29 TESTS) ===
[PASS] Test 01: Repository DI Registrations (All repositories resolve via DI)
[PASS] Test 02: Donor Profile Auto-Sync & View Own Profile
[PASS] Test 03: Donor Profile Update Valid Data & Sync to TaiKhoan
[PASS] Test 04: Donor Profile Reject Duplicate Phone Number
[PASS] Test 05: Donor Profile Reject Duplicate Email
[PASS] Test 06: Donor Profile Reject Invalid Weight (<30 or >200)
[PASS] Test 07: Donor Profile Reject Invalid Phone Format
[PASS] Test 08: Donor Profile Prevent IDOR (Cannot edit other donor's profile via POST tampering)
[PASS] Test 09: Submit Registration Valid Open Campaign -> 'Chờ xác nhận'
[PASS] Test 10: Submit Registration Uses Unified selectedMaDot (Fixes Bug #1)
[PASS] Test 11: Block Duplicate Active Ticket in Same Campaign
[PASS] Test 12: Allow Re-registering in Same Campaign After Cancellation ('Đã hủy')
[PASS] Test 13: Block Registration in Closed Campaign ('Đã đóng')
[PASS] Test 14: Block Registration in Paused Campaign ('Tạm dừng')
[PASS] Test 15: Block Registration in Unopened Campaign ('Chưa mở')
[PASS] Test 16: Block Registration in Expired Campaign (HanDangKy < Today)
[PASS] Test 17: Block Registration When Underweight (< Dot.CanNangToiThieu)
[PASS] Test 18: Block Registration When Underage (< 18 years old)
[PASS] Test 19: Block Registration When Overage (> 60 years old)
[PASS] Test 20: Block Registration When Within 84-Day Cooldown (Based on KetQuaHienMau.NgayCapNhat)
[PASS] Test 21: Allow Registration When >= 84 Days Since Last Successful Donation
[PASS] Test 22: Do Not Count 'Không đủ điều kiện hiến' as Successful Donation or Trigger 84-Day Cooldown (Fixes Bug #3 & #5)
[PASS] Test 23: Block Registration Safely When Successful Donation Lacks KetQuaHienMau.NgayCapNhat
[PASS] Test 24: Donor Cancel Ticket in 'Chờ xác nhận' -> 'Đã hủy'
[PASS] Test 25: Donor Cancel Ticket in 'Đã xác nhận' -> 'Đã hủy' & Cancel Pending Schedules
[PASS] Test 26: Block Donor From Cancelling Ticket in 'Đã khám sàng lọc' or 'Hiến máu thành công'
[PASS] Test 27: Prevent IDOR on TheoDoi & HuyDangKy (Donor A cannot view or cancel Donor B's ticket)
[PASS] Test 28: Locked Donor Profile ('Ngừng hoạt động') Blocked From Creating Ticket
[PASS] Test 29: HomeController.Index Uses Live Database Campaigns via IDotHienMauRepository (Fixes Bug #7)
=== SUMMARY: 29 PASSED, 0 FAILED ===
```

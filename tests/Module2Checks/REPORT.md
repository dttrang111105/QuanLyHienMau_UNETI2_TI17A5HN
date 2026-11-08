# Báo cáo kiểm thử module 2

Ngày hoàn tất: 08/11/2026. Website chạy thực tế tại http://localhost:5117, ASP.NET Core 10 MVC và SQL Server LocalDB theo cấu hình project.

## Kết quả

| Nhóm | Kết quả và phạm vi |
|---|---|
| Build | Thành công, 0 lỗi. Còn cảnh báo NU1902/NU1903 của dependency có sẵn; không đổi package ngoài phạm vi. |
| Model, LINQ và SQL translation | 29/29 đạt; kiểm tra validation, nhóm máu, sắp xếp ổn định, bộ lọc kết hợp, OFFSET/FETCH. |
| HTTP + SQL thực tế | 160/160 đạt; chi tiết từng ca trong `live-results.txt`. |
| Phân quyền | Admin, Medical Staff, Donor, người chưa đăng nhập; GET/POST và CSRF. |
| Thêm/sửa | Cả tuyến cộng đồng và quản trị; dữ liệu hợp lệ, thiếu tên/đơn vị/ngày, chỉ tiêu 0, cân nặng âm, ngày đảo ngược, mô tả quá dài; đối chiếu lưu DB. |
| Trạng thái | Bốn trạng thái qua cả hai tuyến; từ chối trạng thái lạ và mở đợt không hợp lệ. |
| Xóa | HTTP/SQL: xóa đợt rỗng; chặn xóa đợt có lịch sử phiếu, giữ nguyên phiếu. |
| Tìm/lọc/sắp xếp/phân trang | Tìm tên và đơn vị; 5 nhóm lọc kết hợp; 6 kiểu sắp xếp; giữ query qua trang; trang âm/0/quá lớn; kết quả rỗng. |
| Chi tiết và CTA đăng ký | Còn hạn, chưa tới ngày, hết hạn, tạm dừng, đóng; bản ghi không tồn tại trả 404. |
| UI desktop/mobile | Đã xem trực tiếp danh sách cộng đồng, chi tiết, thêm/sửa và quản trị; kiểm tra ở 1440 px và 390 px. |

## Kiểm tra trực tiếp trên trình duyệt

- Thêm/sửa qua form cộng đồng thành công, dữ liệu tiếng Việt và xuống dòng được giữ.
- Thêm đợt bằng modal quản trị trên mobile thành công; sửa chỉ tiêu 130 thành 135 trên desktop và chuyển trạng thái sang Đang mở đăng ký thành công. Trang cộng đồng hiển thị đúng kết quả.
- Form rỗng hiển thị lỗi tiếng Việt; ngày kết thúc trước ngày bắt đầu bị chặn.
- Tìm kiếm giữ bộ lọc/sắp xếp, chuyển trang và trạng thái không có kết quả được kiểm tra.
- Đã sửa lỗi sidebar chiếm chỗ trên mobile quản trị; trang rộng 390 px, không tràn ngang toàn trang. Bảng được cuộn ngang trong vùng bảng.
- Đã sửa việc xuống dòng các nút trên thẻ đợt và bỏ thanh tiến độ phần trăm giả ở danh sách/chi tiết.
- Đã thay confirm mặc định bằng hộp thoại module 2 trên cả hai trang. Đã kiểm tra Hủy giữ nguyên dữ liệu và Xác nhận xóa thành công trên trình duyệt: quản trị desktop (ID 35) và cộng đồng mobile (ID 36), với sự cho phép của người dùng. Hộp thoại hiển thị tên đợt, cảnh báo không thể hoàn tác, mặc định focus Hủy, chặn bấm xác nhận lặp. Server vẫn kiểm tra lịch sử phiếu.

## Dữ liệu và phạm vi thay đổi

- Chỉ sửa mã module 2 và thêm bộ kiểm thử module 2.
- Đã so sánh với HEAD: toàn bộ phần AdminController trước marker `// GET: /Admin/DotHienMau` giữ nguyên.
- Không sửa DangKyController, shared layout, Program, cấu hình DB, migration hoặc module khác.
- Suite đối chiếu dữ liệu đợt gốc trước/sau và đạt. Phiếu lịch sử thử đã được dọn; sau kiểm tra UI đã dọn thêm 23 đợt mang tiền tố `M2-QA-20261107-`. Trang quản trị xác nhận còn 6 đợt gốc.
- Việc tạo bản ghi thử có sử dụng giá trị identity; các khoảng trống ID sau khi dọn là bình thường.

## Giới hạn cần phân biệt

- Điều kiện thời hạn đã áp dụng ở UI module 2. Chặn POST đăng ký trực tiếp trong DangKyController thuộc module 3 vẫn nằm ngoài phạm vi được phép sửa; vì vậy không kết luận quy tắc đăng ký đã được bảo vệ xuyên suốt toàn hệ thống.
- Logo sidebar quản trị lỗi ảnh và nút đăng ký trên header mobile xuống nhiều dòng thuộc shared layout, được giữ nguyên theo giới hạn chỉ sửa module 2.
- Thẻ Đang mở đăng ký giữ số đợt theo trạng thái lưu, kèm số đợt thực sự trong hạn nhận phiếu (ngày bắt đầu <= hôm nay <= hạn đăng ký). Đã bổ sung 5 ca HTTP/SQL cho còn hạn, chưa tới ngày, hết hạn, tạm dừng, đóng; tất cả đạt. Dữ liệu gốc: 3 đợt có trạng thái mở nhưng 0 đợt còn trong hạn. Không tự ý thay đổi trạng thái lưu của dữ liệu gốc.

## Chạy lại

Từ thư mục repository, chạy website ở một terminal:

```powershell
dotnet run --project QuanLyHienMau_UNETI2_TI17A5HN --launch-profile http
```

Terminal khác:

```powershell
dotnet run --project tests/Module2Checks
dotnet run --project tests/Module2Checks -- --live
```

Suite live dùng các tài khoản demo hiện có và tạo dữ liệu thử có tiền tố riêng, tự dọn khi kết thúc. Chỉ chạy trên database phát triển của project. Có thể dùng `--live --keep-data` để giữ fixture phục vụ kiểm tra UI, sau đó chạy `--cleanup` để dọn.

## Ảnh kiểm chứng

- `screenshots/admin-desktop.png`: quản trị desktop sau khi dọn dữ liệu.
- `screenshots/admin-mobile.png`: quản trị mobile sau khi dọn dữ liệu.
- `screenshots/public-desktop.png`: danh sách trong quá trình thử.
- `screenshots/details-mobile.png`: chi tiết đợt thử trên mobile.
- `screenshots/validation-mobile.png`: thông báo validation tiếng Việt.

## Kiểm tra lại trạng thái và xác nhận xóa

Build 0 lỗi; chạy lại 155/155 ca HTTP/SQL đạt. Sau khi xóa hai đợt thử bằng UI, đã dọn 19 đợt còn lại, trở về 6 đợt gốc. Ảnh mới: `delete-confirm-desktop.png`, `delete-confirm-mobile.png`, `delete-success-public.png`, `delete-success-admin.png` trong thư mục screenshots.

## Sửa bộ lọc đang mở trên trang cộng đồng (08/11/2026)

- Bộ lọc Đang mở đăng ký tại /DotHienMau gọi repository để yêu cầu đồng thời trạng thái mở, ngày bắt đầu <= hôm nay và hạn đăng ký >= hôm nay. Điều kiện chạy trong SQL trước Count/Skip/Take.
- Bộ lọc quản trị vẫn theo trạng thái lưu để quản lý lịch sử; không tự sửa trạng thái hoặc ngày trong DB.
- Đã chạy lại 29/29 model/LINQ checks và 160/160 HTTP/SQL checks, gồm 5 ca kiểm tra kết quả bộ lọc cộng đồng. Dọn 21 fixture, dữ liệu đợt gốc không thay đổi.
- Theo xác nhận riêng của người dùng, khôi phục duy nhất dòng app.Run(); ở cuối Program.cs bị thiếu để website khởi động.

## Sửa nhãn trạng thái riêng với thời hạn

Đã sửa Index.cshtml và Details.cshtml: nhãn trạng thái lấy TrangThai (4 trạng thái), nhãn thời hạn lấy ThoiHanHienThi riêng. CoTheDangKy vẫn điều khiển CTA, không dùng để thay nhãn trạng thái. Bổ sung 10 ca HTTP kiểm tra nhãn danh sách/chi tiết ở các tình huống thời hạn và trạng thái. Build thành công, 0 lỗi. Lượt chạy live mới bị chặn ngay trước tạo fixture bởi lỗi LocalDB startup; chưa xác nhận các ca mới hay giao diện trình duyệt. Kết quả 160 ca trong live-results.txt thuộc lượt trước, không phải xác nhận bản sửa nhãn này. Đã dừng server thử để giải phóng exe.

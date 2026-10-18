// Họ và tên: ĐIỀN HỌ TÊN CỦA BẠN
// Mã sinh viên: ĐIỀN MSSV CỦA BẠN
// Nội dung: Module 5 và phần nền dữ liệu cần thiết để chạy thử.
using System.ComponentModel.DataAnnotations;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Models;
public class KetQuaHienMau
{
    [Key]
    public int MaKetQuaHienMau { get; set; }
    public int MaPhieu { get; set; }
    public PhieuDangKyHienMau Phieu { get; set; } = null !;

    [Range(0, 100)]
    public int? DiemDanhGia { get; set; }

    [Required, MaxLength(2000)]
    public string NhanXet { get; set; } = "";

    [Required, MaxLength(60)]
    public string KetQua { get; set; } = "";
    public DateTime NgayCapNhat { get; set; }
}

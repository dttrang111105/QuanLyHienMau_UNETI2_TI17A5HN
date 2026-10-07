// Họ và tên: NGUYỄN THỊ PHƯƠNG THẢO
// Mã sinh viên: 23103100272
// Nội dung thực hiện: Quản lý đợt hiến máu,
// tìm kiếm, lọc, sắp xếp và phân trang.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Models
{
    [Table("DotHienMau")]
    public class DotHienMau
    {
        [Key]
        public int MaDot { get; set; }

        [Required(ErrorMessage = "Tên đợt hiến máu không được để trống")]
        [StringLength(200, ErrorMessage = "Tên đợt hiến máu tối đa 200 ký tự")]
        public string TenDot { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã đơn vị tổ chức không được để trống")]
        public int MaDonViToChuc { get; set; }

        [Range(1, 10000, ErrorMessage = "Chỉ tiêu đơn vị máu phải lớn hơn 0")]
        public int ChiTieuDonViMau { get; set; }

        [StringLength(50)]
        public string? NhomMauUuTien { get; set; }

        [Range(0, 300,ErrorMessage = "Cân nặng tối thiểu phải lớn hơn hoặc bằng 0")]
        public double CanNangToiThieu { get; set; } = 45.0;

        [Required(ErrorMessage = "Ngày bắt đầu đăng ký không được để trống")]
        public DateTime NgayBatDauDangKy { get; set; }

        [Required(ErrorMessage = "Hạn đăng ký không được để trống")]
        public DateTime HanDangKy { get; set; }

        [StringLength(1000)]
        public string? MoTaDot { get; set; }

        [StringLength(1000)]
        public string? DieuKienNguoiHien { get; set; }

        [Required]
        [StringLength(50)]
        public string TrangThai { get; set; } = "Chưa mở"; // Chưa mở, Đang mở đăng ký, Tạm dừng, Đã đóng

        // Navigation Properties
        [ForeignKey("MaDonViToChuc")]
        public virtual DonViToChuc? DonViToChuc { get; set; }

        public virtual ICollection<PhieuDangKyHienMau> PhieuDangKyHienMaus { get; set; } = new List<PhieuDangKyHienMau>();
    }
}

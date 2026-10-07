using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Models
{
    [Table("NguoiHienMau")]
    public class NguoiHienMau
    {
        [Key]
        public int MaNguoiHien { get; set; }

        public int? MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Họ tên người hiến máu không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        public string HoTen { get; set; } = string.Empty;

        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        public string? GioiTinh { get; set; }

        [StringLength(20)]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? DiaChi { get; set; }

        [StringLength(20)]
        public string? NhomMau { get; set; }

        [StringLength(100)]
        public string? NgheNghiep { get; set; }

        [Range(30.0, 200.0, ErrorMessage = "Cân nặng không hợp lệ")]
        public double? CanNang { get; set; }

        [StringLength(500)]
        public string? GhiChuSucKhoe { get; set; }

        [StringLength(50)]
        public string? TrangThai { get; set; } = "Hoạt động";

        // Navigation Properties
        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoan? TaiKhoan { get; set; }

        public virtual ICollection<PhieuDangKyHienMau> PhieuDangKyHienMaus { get; set; } = new List<PhieuDangKyHienMau>();
    }
}

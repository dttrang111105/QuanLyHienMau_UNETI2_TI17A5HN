// ============================================================================
// DỰ ÁN QUẢN LÝ HIẾN MÁU NHÂN ĐẠO VITALIS - UNETI (TI17A5HN)
// MODULE 3: QUẢN LÝ NGƯỜI HIẾN MÁU & PHIẾU ĐĂNG KÝ HIẾN MÁU
// Phụ trách: Nhóm thực hiện Module 3 - Lớp DHTI17A5HN
// ============================================================================

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

        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }

        [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự")]
        [RegularExpression(@"^0[0-9]{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ (gồm 10-11 chữ số, bắt đầu bằng 0)")]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [Display(Name = "Email")]
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

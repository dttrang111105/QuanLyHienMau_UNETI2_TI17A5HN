using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Models
{
    [Table("TaiKhoan")]
    public class TaiKhoan
    {
        [Key]
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập tối đa 50 ký tự")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(255, ErrorMessage = "Mật khẩu tối đa 255 ký tự")]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        public string HoTen { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Vai trò không được để trống")]
        [StringLength(50, ErrorMessage = "Vai trò tối đa 50 ký tự")]
        public string VaiTro { get; set; } = "Donor"; // Admin, Medical Staff, Donor

        [StringLength(50)]
        public string? TrangThai { get; set; } = "Hoạt động";

        // Navigation Property: 1 - 0..1 với NguoiHienMau
        public virtual NguoiHienMau? NguoiHienMau { get; set; }
    }
}

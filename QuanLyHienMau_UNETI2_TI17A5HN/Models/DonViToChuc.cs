using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Models
{
    [Table("DonViToChuc")]
    public class DonViToChuc
    {
        [Key]
        public int MaDonViToChuc { get; set; }

        [Required(ErrorMessage = "Tên đơn vị tổ chức không được để trống")]
        [StringLength(200, ErrorMessage = "Tên đơn vị tổ chức tối đa 200 ký tự")]
        public string TenDonViToChuc { get; set; } = string.Empty;

        [StringLength(500)]
        public string? MoTa { get; set; }

        [EmailAddress(ErrorMessage = "Email liên hệ không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email liên hệ tối đa 100 ký tự")]
        public string? EmailLienHe { get; set; }

        [StringLength(50)]
        public string? TrangThai { get; set; } = "Hoạt động";

        // Navigation Property: DonViToChuc 1 - n DotHienMau
        public virtual ICollection<DotHienMau> DotHienMaus { get; set; } = new List<DotHienMau>();
    }
}

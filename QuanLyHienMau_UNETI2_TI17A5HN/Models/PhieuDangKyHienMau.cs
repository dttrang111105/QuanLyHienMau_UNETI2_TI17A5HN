// ============================================================================
// DỰ ÁN QUẢN LÝ HIẾN MÁU NHÂN ĐẠO VITALIS - UNETI (TI17A5HN)
// MODULE 3: QUẢN LÝ NGƯỜI HIẾN MÁU & PHIẾU ĐĂNG KÝ HIẾN MÁU
// Phụ trách: Nhóm thực hiện Module 3 - Lớp DHTI17A5HN
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Models
{
    [Table("PhieuDangKyHienMau")]
    public class PhieuDangKyHienMau
    {
        [Key]
        public int MaPhieu { get; set; }

        [Required(ErrorMessage = "Mã người hiến không được để trống")]
        public int MaNguoiHien { get; set; }

        [Required(ErrorMessage = "Mã đợt hiến không được để trống")]
        public int MaDot { get; set; }

        [Required]
        public DateTime NgayNop { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? LyDoHienMau { get; set; }

        [StringLength(500)]
        public string? GhiChu { get; set; }

        [Required]
        [StringLength(50)]
        public string TrangThai { get; set; } = "Chờ xác nhận";
        // Luồng trạng thái: Chờ xác nhận -> Đã xác nhận -> Chờ khám sàng lọc -> Đã khám sàng lọc -> Hiến máu thành công
        // Hoặc: Không đủ điều kiện hiến, Từ chối, Đã hủy

        public DateTime? NgayXuLy { get; set; }

        [StringLength(500)]
        public string? GhiChuXacNhan { get; set; }

        // Navigation Properties
        [ForeignKey("MaNguoiHien")]
        public virtual NguoiHienMau? NguoiHienMau { get; set; }
        [NotMapped]
        public virtual NguoiHienMau? NguoiHien { get => NguoiHienMau; set => NguoiHienMau = value; }

        [ForeignKey("MaDot")]
        public virtual DotHienMau? DotHienMau { get; set; }
        [NotMapped]
        public virtual DotHienMau? Dot { get => DotHienMau; set => DotHienMau = value; }

        public virtual ICollection<LichKhamSangLoc> LichKhamSangLocs { get; set; } = new List<LichKhamSangLoc>();
        [NotMapped]
        public virtual ICollection<LichKhamSangLoc> LichKhams { get => LichKhamSangLocs; set => LichKhamSangLocs = value; }

        public virtual KetQuaHienMau? KetQuaHienMau { get; set; }
    }
}

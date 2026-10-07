using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Models
{
    [Table("LichKhamSangLoc")]
    public class LichKhamSangLoc
    {
        [Key]
        public int MaLichKhamSangLoc { get; set; }

        [Required(ErrorMessage = "Mã phiếu không được để trống")]
        public int MaPhieu { get; set; }

        [Required(ErrorMessage = "Thời gian bắt đầu không được để trống")]
        public DateTime ThoiGianBatDau { get; set; }

        [Required(ErrorMessage = "Thời gian kết thúc không được để trống")]
        public DateTime ThoiGianKetThuc { get; set; }

        [StringLength(100)]
        public string? HinhThucTiepNhan { get; set; } // Trực tiếp tại điểm hiến, Lưu động, Trực tuyến

        [StringLength(255)]
        public string? DiaDiemHoacLienKet { get; set; }

        [StringLength(100)]
        public string? BacSiSangLoc { get; set; }

        [StringLength(500)]
        public string? GhiChu { get; set; }

        [Required]
        [StringLength(50)]
        public string TrangThai { get; set; } = "Đã lên lịch"; // Đã lên lịch, Đã hoàn thành, Đã hủy

        // Navigation Property
        [ForeignKey("MaPhieu")]
        public virtual PhieuDangKyHienMau? PhieuDangKyHienMau { get; set; }
    }
}

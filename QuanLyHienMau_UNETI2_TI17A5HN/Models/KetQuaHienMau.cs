using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Models
{
    [Table("KetQuaHienMau")]
    public class KetQuaHienMau
    {
        [Key]
        public int MaKetQuaHienMau { get; set; }

        [Required(ErrorMessage = "Mã phiếu đăng ký không được để trống")]
        public int MaPhieu { get; set; }

        [Range(0.0, 10.0,
            ErrorMessage = "Điểm đánh giá từ 0 đến 10")]
        public double? DiemDanhGia { get; set; }

        [StringLength(1000)]
        public string? NhanXet { get; set; }

        [Required(ErrorMessage = "Kết quả không được để trống")]
        [StringLength(100)]
        public string KetQua { get; set; } = "Hiến máu thành công";

        public DateTime? NgayCapNhat { get; set; } = DateTime.Now;

        // Quan hệ với PhieuDangKyHienMau
        [ForeignKey(nameof(MaPhieu))]
        public virtual PhieuDangKyHienMau? PhieuDangKyHienMau
        {
            get;
            set;
        }

        [NotMapped]
        public virtual PhieuDangKyHienMau? Phieu
        {
            get => PhieuDangKyHienMau;
            set => PhieuDangKyHienMau = value;
        }
    }
}
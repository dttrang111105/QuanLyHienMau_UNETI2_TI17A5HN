// Họ và tên: NGUYỄN THỊ PHƯƠNG THẢO
// Mã sinh viên: 23103100272
// Nội dung thực hiện: Quản lý đợt hiến máu,
// tìm kiếm, lọc, sắp xếp và phân trang.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using Microsoft.AspNetCore.Authorization;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers
{
    public class DotHienMauController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DotHienMauController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /DotHienMau
        public async Task<IActionResult> Index(string? search, string? status, int? donViId, string? nhomMau, double? canNang, string? thoiHan, string? sort,int page = 1)
        {
            const int pageSize = 6;

            var query = _context.DotHienMaus
                .Include(d => d.DonViToChuc)
                .AsQueryable();

            //SEARCH Tìm kiếm theo tên đợt, mô tả hoặc tên đơn vị tổ chức
            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim().ToLower();

                query = query.Where(d =>
                    d.TenDot.ToLower().Contains(keyword) ||
                    (
                        d.DonViToChuc != null &&
                        d.DonViToChuc.TenDonViToChuc
                            .ToLower()
                            .Contains(keyword)
                    ));
            }

            //FILTER ĐƠN VỊ: Lọc theo đơn vị tổ chức
            if (donViId.HasValue && donViId.Value > 0)
            {
                query = query.Where(d => d.MaDonViToChuc == donViId.Value);
            }

            //FILTER NHÓM MÁU: Lọc theo nhóm máu ưu tiên
            if (!string.IsNullOrWhiteSpace(nhomMau))
            {
                query = query.Where(d => d.NhomMauUuTien != null && d.NhomMauUuTien.Contains(nhomMau));
            }

            //FILTER TRẠNG THÁI: Lọc theo trạng thái
            if (!string.IsNullOrWhiteSpace(status) && status != "all")
            {
                query = query.Where(d => d.TrangThai == status);
            }

            //FILTER CÂN NẶNG: Lọc theo cân nặng tối thiểu
            if (canNang.HasValue && canNang > 0)
            {
                query = query.Where(d => d.CanNangToiThieu <= canNang.Value);
            }

            //CÒN HẠN/ HẾT HẠN
            var today = DateTime.Today;

            if (thoiHan == "conhan")
            {
                query = query.Where(d =>
                    d.HanDangKy >= today);
            }
            else if (thoiHan == "hethan")
            {
                query = query.Where(d =>
                    d.HanDangKy < today);
            }

            //SORT SẮP XẾP 
            query = sort switch
            {
                "name_asc" =>
                    query.OrderBy(d => d.TenDot),

                "name_desc" =>
                    query.OrderByDescending(d => d.TenDot),

                "deadline_asc" =>
                    query.OrderBy(d => d.HanDangKy),

                "deadline_desc" =>
                    query.OrderByDescending(d => d.HanDangKy),

                "quota_asc" =>
                    query.OrderBy(d => d.ChiTieuDonViMau),

                "quota_desc" =>
                    query.OrderByDescending(d => d.ChiTieuDonViMau),

                _ =>
                    query.OrderByDescending(d =>
                        d.NgayBatDauDangKy)
            };

            // PAGINATION
            var totalItems = await query.CountAsync();
            
            var totalPages =(int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1)
            {
                page = 1;
            }

            if (totalPages > 0 && page > totalPages)
            {
                page = totalPages;
            }
            var result = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            
            //LOAD ĐƠN VỊ: Danh sách đơn vị phục vụ dropdown bộ lọc
            ViewBag.DonViList = await _context.DonViToChucs
                .Where(dv => dv.TrangThai == "Hoạt động")
                 .OrderBy(d =>d.TenDonViToChuc)
                .ToListAsync();

            //GIỮ FILTER: Giữ lại tham số tìm kiếm/lọc
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentDonViId = donViId;
            ViewBag.CurrentNhomMau = nhomMau;
            ViewBag.CurrentCanNang = canNang;
            ViewBag.CurrentThoiHan = thoiHan;
            ViewBag.CurrentSort = sort;

            // PAGINATION
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            //var result = await query
            //    .OrderByDescending(d => d.NgayBatDauDangKy)
            //    .ToListAsync();

            return View(result);
        }

        // GET: /DotHienMau/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dotHienMau = await _context.DotHienMaus
                .Include(d => d.DonViToChuc)
                .FirstOrDefaultAsync(m => m.MaDot == id);

            if (dotHienMau == null)
            {
                return NotFound();
            }

            return View(dotHienMau);
        }
    }
}

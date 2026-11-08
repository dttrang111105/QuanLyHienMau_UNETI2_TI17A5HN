using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
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
        public async Task<IActionResult> Index(string? search, string? status, int? donViId, string? nhomMau, double? canNang)
        {
            var query = _context.DotHienMaus
                .Include(d => d.DonViToChuc)
                .AsQueryable();

            // Tìm kiếm theo tên đợt, mô tả hoặc tên đơn vị tổ chức
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLower();
                query = query.Where(d => d.TenDot.ToLower().Contains(searchLower) ||
                                         (d.MoTaDot != null && d.MoTaDot.ToLower().Contains(searchLower)) ||
                                         (d.DonViToChuc != null && d.DonViToChuc.TenDonViToChuc.ToLower().Contains(searchLower)));
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrWhiteSpace(status) && status != "all")
            {
                query = query.Where(d => d.TrangThai == status);
            }

            // Lọc theo đơn vị tổ chức
            if (donViId.HasValue && donViId > 0)
            {
                query = query.Where(d => d.MaDonViToChuc == donViId.Value);
            }

            // Lọc theo nhóm máu ưu tiên
            if (!string.IsNullOrWhiteSpace(nhomMau))
            {
                query = query.Where(d => d.NhomMauUuTien != null && d.NhomMauUuTien.Contains(nhomMau));
            }

            // Lọc theo cân nặng tối thiểu
            if (canNang.HasValue && canNang > 0)
            {
                query = query.Where(d => d.CanNangToiThieu <= canNang.Value);
            }

            // Danh sách đơn vị phục vụ dropdown bộ lọc
            ViewBag.DonViList = await _context.DonViToChucs
                .Where(dv => dv.TrangThai == "Hoạt động")
                .ToListAsync();

            // Giữ lại tham số tìm kiếm/lọc
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentDonViId = donViId;
            ViewBag.CurrentNhomMau = nhomMau;
            ViewBag.CurrentCanNang = canNang;

            var result = await query
                .OrderByDescending(d => d.NgayBatDauDangKy)
                .ToListAsync();

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

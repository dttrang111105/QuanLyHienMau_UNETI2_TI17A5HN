# -*- coding: utf-8 -*-
import os, sys, math, subprocess

sys.stdout.reconfigure(encoding="utf-8")

output_dir = r"D:\vs\thnet\QuanLyHienMau_UNETI2_TI17A5HN\Diagrams_Module1"
artifact_dir = r"C:\Users\XPS\.gemini\antigravity\brain\36ffa63d-76ea-4470-9d00-03f312d760a3"
os.makedirs(output_dir, exist_ok=True)
chrome_path = r"C:\Program Files\Google\Chrome\Application\chrome.exe"

def render_svg_to_png(svg_content, png_filename, width, height, bg_color="#ffffff"):
    png_path = os.path.join(output_dir, png_filename)
    html_content = f"""<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<style>
  * {{ box-sizing: border-box; margin: 0; padding: 0; }}
  body {{
    margin: 0;
    padding: 0;
    background: {bg_color};
    font-family: Arial, "Segoe UI", sans-serif;
    overflow: hidden;
  }}
  svg {{
    display: block;
    background: {bg_color};
  }}
</style>
</head>
<body>
{svg_content}
</body>
</html>"""
    temp_html = os.path.join(output_dir, f"temp_{png_filename}.html")
    with open(temp_html, "w", encoding="utf-8") as f:
        f.write(html_content)
    
    cmd = [
        chrome_path,
        "--headless=new",
        f"--screenshot={png_path}",
        f"--window-size={width},{height}",
        "--default-background-color=ffffff",
        "--hide-scrollbars",
        temp_html
    ]
    subprocess.run(cmd, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
    if os.path.exists(temp_html):
        os.remove(temp_html)
    if os.path.exists(png_path):
        # Also copy to artifact_dir
        import shutil
        shutil.copy2(png_path, os.path.join(artifact_dir, png_filename))
        print(f"Created {png_filename} ({width}x{height}) successfully!")
    else:
        print(f"Error creating {png_filename}")

# ==============================================================================
# COMMON SVG HELPERS (100% CLEAN GEOMETRY & NO CODE JARGON)
# ==============================================================================
def svg_wrapper(width, height, content, bg_color="#ffffff"):
    return f"""<svg width="{width}" height="{height}" viewBox="0 0 {width} {height}" xmlns="http://www.w3.org/2000/svg">
  <defs>
    <marker id="arrow" viewBox="0 0 10 10" refX="9.5" refY="5" markerWidth="6.5" markerHeight="6.5" orient="auto">
      <path d="M 0 0.5 L 9.5 5 L 0 9.5 z" fill="#222222" />
    </marker>
    <marker id="open_arrow" viewBox="0 0 10 10" refX="9.5" refY="5" markerWidth="7.5" markerHeight="7.5" orient="auto">
      <path d="M 0.5 1 L 9 5 L 0.5 9" fill="none" stroke="#222222" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round" />
    </marker>
  </defs>
  <rect width="{width}" height="{height}" fill="{bg_color}" />
  {content}
</svg>"""

def draw_actor(x, y, name):
    lines = name.split("\n")
    t_spans = ""
    for i, line in enumerate(lines):
        t_spans += f'<text x="{x}" y="{y + 48 + i*16}" font-size="13" font-weight="bold" fill="#000000" text-anchor="middle" font-family="Arial, sans-serif">{line}</text>\n'
    return f"""
    <circle cx="{x}" cy="{y - 34}" r="13" fill="#dae8fc" stroke="#6c8ebf" stroke-width="1.5" />
    <line x1="{x}" y1="{y - 21}" x2="{x}" y2="{y + 8}" stroke="#6c8ebf" stroke-width="1.5" />
    <line x1="{x - 20}" y1="{y - 10}" x2="{x + 20}" y2="{y - 10}" stroke="#6c8ebf" stroke-width="1.5" />
    <line x1="{x}" y1="{y + 8}" x2="{x - 16}" y2="{y + 32}" stroke="#6c8ebf" stroke-width="1.5" />
    <line x1="{x}" y1="{y + 8}" x2="{x + 16}" y2="{y + 32}" stroke="#6c8ebf" stroke-width="1.5" />
    {t_spans}
    """

def draw_ellipse(cx, cy, rx, ry, lines, fill="#d5e8d4", stroke="#82b366", font_weight="normal", font_size=12):
    t_spans = ""
    start_y = cy - (len(lines) - 1) * (font_size + 3) / 2
    for i, line in enumerate(lines):
        ly = start_y + i * (font_size + 3)
        t_spans += f'<text x="{cx}" y="{ly}" font-size="{font_size}" font-weight="{font_weight}" fill="#000000" text-anchor="middle" dominant-baseline="central" font-family="Arial, sans-serif">{line}</text>\n'
    return f"""
    <ellipse cx="{cx}" cy="{cy}" rx="{rx}" ry="{ry}" fill="{fill}" stroke="{stroke}" stroke-width="1.3" />
    {t_spans}
    """

def ellipse_border_point(cx, cy, rx, ry, target_x, target_y):
    dx = target_x - cx
    dy = target_y - cy
    angle = math.atan2(dy, dx)
    # Parametric angle on ellipse
    t = math.atan2(rx * dy, ry * dx)
    px = cx + rx * math.cos(t)
    py = cy + ry * math.sin(t)
    return px, py

def draw_uc_relation(cx1, cy1, rx1, ry1, cx2, cy2, rx2, ry2, rel_type="include", bg_color="#f5f5f5"):
    # For include: arrow from UC1 -> UC2
    # For extend: arrow from UC1 (extending) -> UC2 (base)
    x1, y1 = ellipse_border_point(cx1, cy1, rx1, ry1, cx2, cy2)
    x2, y2 = ellipse_border_point(cx2, cy2, rx2, ry2, cx1, cy1)
    mx = (x1 + x2) / 2
    my = (y1 + y2) / 2
    label = f"&lt;&lt;{rel_type}&gt;&gt;"
    return f"""
    <line x1="{x1:.1f}" y1="{y1:.1f}" x2="{x2:.1f}" y2="{y2:.1f}" stroke="#222222" stroke-width="1.2" stroke-dasharray="4,3" marker-end="url(#open_arrow)" />
    <rect x="{mx - 34:.1f}" y="{my - 8:.1f}" width="68" height="16" fill="{bg_color}" />
    <text x="{mx:.1f}" y="{my:.1f}" font-size="11" fill="#000000" text-anchor="middle" dominant-baseline="central" font-family="Arial, sans-serif">{label}</text>
    """, (mx, my)

def draw_uc_note(x, y, w, h, lines, target_mx=None, target_my=None):
    fold = 12
    path = f"M {x} {y} L {x+w-fold} {y} L {x+w} {y+fold} L {x+w} {y+h} L {x} {y+h} Z"
    fold_path = f"M {x+w-fold} {y} L {x+w-fold} {y+fold} L {x+w} {y+fold} Z"
    t_spans = ""
    start_y = y + 16
    for i, line in enumerate(lines):
        ly = start_y + i * 15
        t_spans += f'<text x="{x+8}" y="{ly}" font-size="11" fill="#000000" font-family="Arial, sans-serif">{line}</text>\n'
    conn = ""
    if target_mx is not None and target_my is not None:
        # Connect from closest edge center of note to (target_mx, target_my)
        ncx = x + w / 2
        ncy = y + h / 2
        if target_my > y + h:
            nx, ny = ncx, y + h
        elif target_my < y:
            nx, ny = ncx, y
        elif target_mx > x + w:
            nx, ny = x + w, ncy
        else:
            nx, ny = x, ncy
        conn = f'<line x1="{nx:.1f}" y1="{ny:.1f}" x2="{target_mx:.1f}" y2="{target_my:.1f}" stroke="#b5a642" stroke-width="1.2" stroke-dasharray="3,3" />'
    return f"""
    {conn}
    <path d="{path}" fill="#ffffc0" stroke="#b5a642" stroke-width="1.1" />
    <path d="{fold_path}" fill="#f2f2a6" stroke="#b5a642" stroke-width="1.1" />
    {t_spans}
    """

# ==============================================================================
# ACTIVITY DIAGRAM HELPERS
# ==============================================================================
def draw_swimlane(x, y, w, h, col1_w, title1, title2):
    hdr_h = 28
    return f"""
    <!-- Outer Swimlane Box -->
    <rect x="{x}" y="{y}" width="{w}" height="{h}" fill="#ffffff" stroke="#666666" stroke-width="1.2" />
    <!-- Left Header -->
    <rect x="{x}" y="{y}" width="{col1_w}" height="{hdr_h}" fill="#f5f5f5" stroke="#666666" stroke-width="1.2" />
    <rect x="{x+6}" y="{y+6}" width="9" height="9" fill="#ffffff" stroke="#888888" stroke-width="1" />
    <line x1="{x+8}" y1="{y+10.5}" x2="{x+13}" y2="{y+10.5}" stroke="#888888" stroke-width="1" />
    <text x="{x + col1_w/2}" y="{y + 18}" font-size="13" font-weight="bold" fill="#000000" text-anchor="middle" font-family="Arial, sans-serif">{title1}</text>
    <!-- Right Header -->
    <rect x="{x+col1_w}" y="{y}" width="{w - col1_w}" height="{hdr_h}" fill="#f5f5f5" stroke="#666666" stroke-width="1.2" />
    <rect x="{x+col1_w+6}" y="{y+6}" width="9" height="9" fill="#ffffff" stroke="#888888" stroke-width="1" />
    <line x1="{x+col1_w+8}" y1="{y+10.5}" x2="{x+col1_w+13}" y2="{y+10.5}" stroke="#888888" stroke-width="1" />
    <text x="{x + col1_w + (w - col1_w)/2}" y="{y + 18}" font-size="13" font-weight="bold" fill="#000000" text-anchor="middle" font-family="Arial, sans-serif">{title2}</text>
    <!-- Center Divider -->
    <line x1="{x+col1_w}" y1="{y+hdr_h}" x2="{x+col1_w}" y2="{y+h}" stroke="#666666" stroke-width="1.2" />
    """

def draw_act_box(cx, cy, w, h, lines, style="system", font_size=12):
    # style: "user" (blue), "system" (green), "error" (red), "warn" (yellow)
    palette = {
        "user": ("#dae8fc", "#6c8ebf"),
        "system": ("#d5e8d4", "#82b366"),
        "error": ("#f8cecc", "#b85450"),
        "warn": ("#fff2cc", "#d6b656")
    }
    fill, stroke = palette.get(style, ("#d5e8d4", "#82b366"))
    x = cx - w / 2
    y = cy - h / 2
    rx = min(18, h / 2)
    t_spans = ""
    start_y = cy - (len(lines) - 1) * (font_size + 3) / 2
    for i, line in enumerate(lines):
        ly = start_y + i * (font_size + 3)
        t_spans += f'<text x="{cx}" y="{ly}" font-size="{font_size}" fill="#000000" text-anchor="middle" dominant-baseline="central" font-family="Arial, sans-serif">{line}</text>\n'
    return f"""
    <rect x="{x:.1f}" y="{y:.1f}" width="{w}" height="{h}" rx="{rx:.1f}" ry="{rx:.1f}" fill="{fill}" stroke="{stroke}" stroke-width="1.2" />
    {t_spans}
    """

def draw_act_diamond(cx, cy, w, h, lines, font_size=12):
    pts = f"{cx},{cy - h/2} {cx + w/2},{cy} {cx},{cy + h/2} {cx - w/2},{cy}"
    t_spans = ""
    start_y = cy - (len(lines) - 1) * (font_size + 2) / 2
    for i, line in enumerate(lines):
        ly = start_y + i * (font_size + 2)
        t_spans += f'<text x="{cx}" y="{ly}" font-size="{font_size}" font-weight="bold" fill="#000000" text-anchor="middle" dominant-baseline="central" font-family="Arial, sans-serif">{line}</text>\n'
    return f"""
    <polygon points="{pts}" fill="#fff2cc" stroke="#d6b656" stroke-width="1.2" />
    {t_spans}
    """

def draw_start_node(cx, cy):
    return f'<circle cx="{cx}" cy="{cy}" r="11" fill="#000000" />'

def draw_end_node(cx, cy):
    return f"""
    <circle cx="{cx}" cy="{cy}" r="13" fill="#ffffff" stroke="#000000" stroke-width="1.5" />
    <circle cx="{cx}" cy="{cy}" r="8.5" fill="#000000" />
    """

def draw_flow_arrow(pts, label=None, label_pos=None):
    pts_str = " ".join([f"{p[0]:.1f},{p[1]:.1f}" for p in pts])
    out = f'<polyline points="{pts_str}" fill="none" stroke="#222222" stroke-width="1.2" marker-end="url(#arrow)" />\n'
    if label and label_pos:
        lx, ly = label_pos
        tw = len(label) * 6.2 + 8
        out += f'<rect x="{lx - tw/2:.1f}" y="{ly - 8:.1f}" width="{tw:.1f}" height="16" fill="#ffffff" />\n'
        out += f'<text x="{lx:.1f}" y="{ly:.1f}" font-size="11.5" fill="#000000" text-anchor="middle" dominant-baseline="central" font-family="Arial, sans-serif">{label}</text>\n'
    return out

# ==============================================================================
# SEQUENCE DIAGRAM HELPERS (EXACT STYLE OF HÌNH 2.20 / IMAGE8.PNG)
# ==============================================================================
def draw_seq_header(cx, y, w, h, lines, kind="box_blue"):
    x = cx - w / 2
    if kind == "cylinder":
        rx = w / 2
        ry = 7
        return f"""
        <path d="M {x} {y+ry} A {rx} {ry} 0 0 0 {x+w} {y+ry} L {x+w} {y+h-ry} A {rx} {ry} 0 0 1 {x} {y+h-ry} Z" fill="#f5f5f5" stroke="#666666" stroke-width="1.2" />
        <ellipse cx="{cx}" cy="{y+ry}" rx="{rx}" ry="{ry}" fill="#f5f5f5" stroke="#666666" stroke-width="1.2" />
        <text x="{cx}" y="{y + h/2 + 4}" font-size="12.5" font-weight="bold" fill="#000000" text-anchor="middle" font-family="Arial, sans-serif">{lines[0]}</text>
        """
    fill, stroke = ("#fff2cc", "#d6b656") if kind == "actor" else ("#dae8fc", "#6c8ebf")
    t_spans = ""
    start_y = y + h/2 - (len(lines) - 1) * 15 / 2
    for i, line in enumerate(lines):
        ly = start_y + i * 15
        t_spans += f'<text x="{cx}" y="{ly}" font-size="12.5" font-weight="bold" fill="#000000" text-anchor="middle" dominant-baseline="central" font-family="Arial, sans-serif">{line}</text>\n'
    return f"""
    <rect x="{x:.1f}" y="{y:.1f}" width="{w}" height="{h}" rx="6" ry="6" fill="{fill}" stroke="{stroke}" stroke-width="1.2" />
    {t_spans}
    """

def draw_seq_msg(x1, x2, y, text, dashed=False):
    mx = (x1 + x2) / 2
    dash_attr = 'stroke-dasharray="4,3"' if dashed else ''
    marker = "open_arrow" if dashed else "arrow"
    return f"""
    <text x="{mx:.1f}" y="{y - 6}" font-size="12" fill="#000000" text-anchor="middle" font-family="Arial, sans-serif">{text}</text>
    <line x1="{x1:.1f}" y1="{y}" x2="{x2:.1f}" y2="{y}" stroke="#222222" stroke-width="1.2" {dash_attr} marker-end="url(#{marker})" />
    """

def draw_seq_self(x, y, text):
    w_loop = 28
    h_loop = 18
    return f"""
    <polyline points="{x},{y} {x+w_loop},{y} {x+w_loop},{y+h_loop} {x},{y+h_loop}" fill="none" stroke="#222222" stroke-width="1.2" marker-end="url(#arrow)" />
    <text x="{x + w_loop + 7}" y="{y + h_loop/2 + 1}" font-size="12" fill="#000000" text-anchor="start" dominant-baseline="central" font-family="Arial, sans-serif">{text}</text>
    """

def draw_seq_condition(x, y, text, divider_y=None, div_x1=150, div_x2=880):
    out = ""
    if divider_y is not None:
        out += f'<line x1="{div_x1}" y1="{divider_y}" x2="{div_x2}" y2="{divider_y}" stroke="#82b366" stroke-width="1.1" stroke-dasharray="4,3" />\n'
    out += f'<text x="{x}" y="{y}" font-size="12" font-style="italic" fill="#000000" text-anchor="start" font-family="Arial, sans-serif">{text}</text>\n'
    return out
# ==============================================================================
# 0. BIỂU ĐỒ CA SỬ DỤNG TỔNG QUAN MODULE 1
# ==============================================================================
def generate_uc_tongquan():
    w, h = 860, 580
    bg = "#f5f5f5"
    c = ""
    # Boundary box
    c += '<rect x="195" y="25" width="470" height="530" fill="#ffffff" stroke="#666666" stroke-width="1.2" />'
    c += '<text x="430" y="50" font-size="14" font-weight="bold" fill="#000000" text-anchor="middle" font-family="Arial, sans-serif">Hệ thống quản lý hiến máu</text>'

    # 3 Actors
    c += draw_actor(85, 190, "Quản trị viên")
    c += draw_actor(85, 420, "Nhân viên y tế")
    c += draw_actor(775, 290, "Người hiến máu")

    # Use Cases inside system
    ucs = [
        (430, 105, 105, 32, ["Đăng ký tài khoản"]),
        (430, 185, 105, 32, ["Đăng nhập hệ thống"]),
        (430, 265, 100, 30, ["Đăng xuất hệ thống"]),
        (430, 345, 110, 34, ["Quản lý tài khoản", "và phân quyền"]),
        (430, 430, 115, 34, ["Quản lý danh mục", "đơn vị tổ chức"]),
        (430, 505, 110, 32, ["Tra cứu thông tin", "đơn vị tổ chức"])
    ]
    for cx, cy, rx, ry, lines in ucs:
        c += draw_ellipse(cx, cy, rx, ry, lines, fill="#dae8fc", stroke="#6c8ebf")

    # Actor lines
    # Quản trị viên -> Đăng nhập, Đăng xuất, Quản lý tài khoản, Quản lý đơn vị, Tra cứu đơn vị
    for target_cy, target_rx in [(185, 105), (265, 100), (345, 110), (430, 115), (505, 110)]:
        c += f'<line x1="115" y1="185" x2="{430 - target_rx}" y2="{target_cy}" stroke="#222222" stroke-width="1.2" />'

    # Nhân viên y tế -> Đăng nhập, Đăng xuất, Tra cứu đơn vị
    for target_cy, target_rx in [(185, 105), (265, 100), (505, 110)]:
        c += f'<line x1="115" y1="415" x2="{430 - target_rx}" y2="{target_cy}" stroke="#222222" stroke-width="1.2" />'

    # Người hiến máu -> Đăng ký, Đăng nhập, Đăng xuất
    for target_cy, target_rx in [(105, 105), (185, 105), (265, 100)]:
        c += f'<line x1="745" y1="285" x2="{430 + target_rx}" y2="{target_cy}" stroke="#222222" stroke-width="1.2" />'

    render_svg_to_png(svg_wrapper(w, h, c, bg), "Hinh2_17_UC_TongQuan_Module1.png", w, h, bg)

# ==============================================================================
# 1. BỘ 1: ĐĂNG NHẬP HỆ THỐNG VÀ PHÂN QUYỀN (HÌNH 2.18, 2.19, 2.20)
# ==============================================================================
def generate_uc_dangnhap():
    w, h = 760, 560
    bg = "#f5f5f5"
    c = ""

    # Actor
    c += draw_actor(75, 275, "Người dùng")

    # Main UC
    mcx, mcy, mrx, mry = 255, 270, 95, 38
    c += draw_ellipse(mcx, mcy, mrx, mry, ["Đăng nhập hệ thống"], fill="#dae8fc", stroke="#6c8ebf", font_weight="bold", font_size=13)
    c += f'<line x1="95" y1="270" x2="{mcx - mrx}" y2="270" stroke="#222222" stroke-width="1.3" />'

    # Included UCs (radial layout like image6.png)
    inc_ucs = [
        (415, 70, 105, 34, ["Kiểm tra thông tin", "tài khoản"]),
        (575, 140, 105, 34, ["Kiểm tra trạng thái", "hoạt động"]),
        (615, 240, 105, 34, ["Đối chiếu mật khẩu"]),
        (615, 340, 105, 34, ["Khởi tạo phiên", "đăng nhập"]),
        (570, 435, 105, 34, ["Phân quyền và điều", "hướng theo vai trò"]),
        (395, 495, 105, 34, ["Thông báo kết quả", "đăng nhập"])
    ]
    for scx, scy, srx, sry, lines in inc_ucs:
        c += draw_ellipse(scx, scy, srx, sry, lines, fill="#d5e8d4", stroke="#82b366", font_size=12)
        rel_svg, _ = draw_uc_relation(mcx, mcy, mrx, mry, scx, scy, srx, sry, "include", bg)
        c += rel_svg

    render_svg_to_png(svg_wrapper(w, h, c, bg), "Hinh2_18_UC_DangNhap.png", w, h, bg)

def generate_ad_dangnhap():
    w, h = 760, 780
    col1_w = 310
    c = ""
    c += draw_swimlane(12, 12, w - 24, h - 24, col1_w, "Người dùng", "Hệ thống")

    ux = 12 + col1_w / 2           # 167
    sx = 12 + col1_w + (w - 24 - col1_w) / 2  # 535

    # User Lane
    c += draw_start_node(ux, 62)
    c += draw_act_box(ux, 118, 200, 42, ["Truy cập chức năng", "đăng nhập"], "user")
    c += draw_flow_arrow([(ux, 73), (ux, 97)])

    c += draw_act_box(ux, 190, 210, 42, ["Nhập tên đăng nhập", "và mật khẩu"], "user")
    c += draw_flow_arrow([(ux, 139), (ux, 169)])

    c += draw_act_box(ux, 260, 180, 38, ["Gửi yêu cầu đăng nhập"], "user")
    c += draw_flow_arrow([(ux, 211), (ux, 241)])

    # Arrow from User (260) to System first box (118)
    c += draw_flow_arrow([(ux + 90, 260), (355, 260), (355, 118), (sx - 115, 118)])

    # System Lane
    c += draw_act_box(sx, 118, 230, 42, ["Kiểm tra tính hợp lệ của", "thông tin đầu vào"], "system")
    c += draw_act_box(sx, 190, 230, 42, ["Tra cứu thông tin tài khoản", "trong cơ sở dữ liệu"], "system")
    c += draw_flow_arrow([(sx, 139), (sx, 169)])

    c += draw_act_box(sx, 265, 230, 44, ["Kiểm tra sự tồn tại và", "đối chiếu mật khẩu"], "system")
    c += draw_flow_arrow([(sx, 211), (sx, 243)])

    c += draw_act_box(sx, 342, 230, 44, ["Kiểm tra trạng thái hoạt động", "của tài khoản (Không bị khóa)"], "system")
    c += draw_flow_arrow([(sx, 287), (sx, 320)])

    # Decision: Thông tin hợp lệ?
    c += draw_act_diamond(sx, 425, 165, 52, ["Đăng nhập", "hợp lệ?"])
    c += draw_flow_arrow([(sx, 364), (sx, 399)])

    # Left branch: [Không hợp lệ] -> Error box in User lane
    c += draw_act_box(ux, 425, 215, 48, ["Thông báo lý do từ chối", "(Sai thông tin hoặc bị khóa)"], "error")
    c += draw_flow_arrow([(sx - 82.5, 425), (ux + 107.5, 425)], "[Không hợp lệ]", (360, 425))

    # Down branch: [Hợp lệ]
    c += draw_act_box(sx, 515, 235, 44, ["Khởi tạo phiên làm việc và", "ghi nhận thông tin người dùng"], "system")
    c += draw_flow_arrow([(sx, 451), (sx, 493)], "[Hợp lệ]", (sx, 471))

    c += draw_act_box(sx, 590, 235, 44, ["Xác định quyền hạn theo vai trò", "(Quản trị / Nhân viên y tế / Người hiến)"], "system")
    c += draw_flow_arrow([(sx, 537), (sx, 568)])

    c += draw_act_box(sx, 665, 220, 40, ["Điều hướng đến giao diện", "chức năng tương ứng"], "system")
    c += draw_flow_arrow([(sx, 612), (sx, 645)])

    # End node at sx, 730
    c += draw_end_node(sx, 730)
    c += draw_flow_arrow([(sx, 685), (sx, 717)])
    # Connect Error box to End node
    c += draw_flow_arrow([(ux, 449), (ux, 730), (sx - 13, 730)])

    render_svg_to_png(svg_wrapper(w, h, c), "Hinh2_19_AD_DangNhap.png", w, h)

def generate_sd_dangnhap():
    w, h = 1040, 740
    c = ""

    x1, x2, x3, x4 = 95, 345, 620, 915
    c += draw_seq_header(x1, 15, 130, 42, ["Người dùng"], "actor")
    c += draw_seq_header(x2, 15, 145, 42, ["Giao diện", "đăng nhập"], "box_blue")
    c += draw_seq_header(x3, 15, 150, 42, ["Bộ điều khiển", "xác thực"], "box_blue")
    c += draw_seq_header(x4, 15, 135, 42, ["Cơ sở dữ liệu"], "cylinder")

    # Lifelines
    for lx in [x1, x2, x3, x4]:
        c += f'<line x1="{lx}" y1="57" x2="{lx}" y2="{h - 15}" stroke="#666666" stroke-width="1.2" stroke-dasharray="4,4" />'

    c += draw_seq_msg(x1, x2, 90, "1. Truy cập chức năng đăng nhập")
    c += draw_seq_msg(x2, x3, 122, "2. Gửi yêu cầu mở biểu mẫu đăng nhập")
    c += draw_seq_msg(x3, x2, 154, "3. Trả về biểu mẫu đăng nhập", dashed=True)
    c += draw_seq_msg(x2, x1, 186, "4. Hiển thị biểu mẫu đăng nhập", dashed=True)

    c += draw_seq_msg(x1, x2, 222, "5. Nhập tên đăng nhập, mật khẩu &amp; gửi yêu cầu")
    c += draw_seq_msg(x2, x3, 254, "6. Chuyển dữ liệu đăng nhập")
    c += draw_seq_self(x3, 276, "7. Kiểm tra tính đầy đủ của dữ liệu đầu vào")

    c += draw_seq_msg(x3, x4, 324, "8. Truy vấn tài khoản theo tên đăng nhập")
    c += draw_seq_msg(x4, x3, 356, "9. Trả về thông tin tài khoản", dashed=True)
    c += draw_seq_self(x3, 378, "10. Kiểm tra tồn tại, đối chiếu mật khẩu &amp; trạng thái tài khoản")

    # Branch 1: Hợp lệ
    c += draw_seq_condition(x1 - 15, 422, "[Thông tin hợp lệ và tài khoản đang hoạt động]")
    c += draw_seq_self(x3, 438, "11. Khởi tạo phiên đăng nhập &amp; lưu thông tin vai trò")
    c += draw_seq_self(x3, 476, "12. Xác định trang chức năng tương ứng với vai trò")
    c += draw_seq_msg(x3, x2, 522, "13. Trả kết quả xác thực thành công &amp; điều hướng", dashed=True)
    c += draw_seq_msg(x2, x1, 554, "14. Hiển thị giao diện theo quyền hạn người dùng", dashed=True)

    # Branch 2: Không hợp lệ
    c += draw_seq_condition(x1 - 15, 596, "[Sai tài khoản, sai mật khẩu hoặc tài khoản bị khóa]", divider_y=576, div_x1=x1+10, div_x2=x4-10)
    c += draw_seq_self(x3, 612, "15. Từ chối xác thực &amp; xác định nguyên nhân lỗi")
    c += draw_seq_msg(x3, x2, 658, "16. Trả kết quả đăng nhập thất bại kèm lý do", dashed=True)
    c += draw_seq_msg(x2, x1, 690, "17. Hiển thị thông báo lỗi đăng nhập", dashed=True)

    render_svg_to_png(svg_wrapper(w, h, c), "Hinh2_20_SD_DangNhap.png", w, h)

# ==============================================================================
# 2. BỘ 2: QUẢN LÝ TÀI KHOẢN VÀ PHÂN QUYỀN (HÌNH 2.21, 2.22, 2.23)
# ==============================================================================
def generate_uc_quanty_taikhoan():
    w, h = 900, 580
    bg = "#f5f5f5"
    c = ""

    c += draw_actor(65, 285, "Quản trị viên")

    # Main UC
    mcx, mcy, mrx, mry = 215, 280, 90, 38
    c += draw_ellipse(mcx, mcy, mrx, mry, ["Quản lý", "tài khoản"], fill="#dae8fc", stroke="#6c8ebf", font_weight="bold", font_size=13)
    c += f'<line x1="85" y1="280" x2="{mcx - mrx}" y2="280" stroke="#222222" stroke-width="1.3" />'

    # Included UCs (exact layout of image15.png)
    inc_ucs = [
        (345, 115, 95, 34, ["Xác thực quyền", "quản trị viên"]),
        (485, 280, 95, 34, ["Xem danh sách", "tài khoản"]),
        (345, 445, 100, 34, ["Xem thống kê số lượng", "theo vai trò"])
    ]
    for scx, scy, srx, sry, lines in inc_ucs:
        c += draw_ellipse(scx, scy, srx, sry, lines, fill="#d5e8d4", stroke="#82b366", font_size=12)
        rel_svg, _ = draw_uc_relation(mcx, mcy, mrx, mry, scx, scy, srx, sry, "include", bg)
        c += rel_svg

    # 3 Extended UCs from "Xem danh sách tài khoản" (485, 280) - exact layout of image15.png
    ext_ucs = [
        (705, 95, 98, 34, ["Tìm kiếm & lọc", "tài khoản"], (385, 25, 165, 44, ["Điều kiện mở rộng:", "[Quản trị viên nhập từ khóa", "hoặc chọn điều kiện lọc]"])),
        (790, 280, 98, 34, ["Thêm mới & đặt lại", "mật khẩu tài khoản"], (695, 172, 165, 44, ["Điều kiện mở rộng:", "[Quản trị viên chọn thêm mới", "hoặc cấp lại mật khẩu]"])),
        (735, 455, 98, 34, ["Khóa / Mở khóa", "tài khoản"], (435, 490, 165, 44, ["Điều kiện mở rộng:", "[Quản trị viên chọn thay đổi", "trạng thái hoạt động]"]))
    ]
    for ecx, ecy, erx, ery, lines, note_info in ext_ucs:
        c += draw_ellipse(ecx, ecy, erx, ery, lines, fill="#fff2cc", stroke="#d6b656", font_size=12)
        rel_svg, (mx, my) = draw_uc_relation(ecx, ecy, erx, ery, 485, 280, 95, 34, "extend", bg)
        c += rel_svg
        nx, ny, nw, nh, nlines = note_info
        c += draw_uc_note(nx, ny, nw, nh, nlines, mx, my)

    render_svg_to_png(svg_wrapper(w, h, c, bg), "Hinh2_21_UC_QuanLyTaiKhoan.png", w, h, bg)

def generate_ad_quanty_taikhoan():
    w, h = 840, 790
    col1_w = 310
    c = ""
    c += draw_swimlane(12, 12, w - 24, h - 24, col1_w, "Quản trị viên", "Hệ thống quản trị")

    ux = 12 + col1_w / 2           # 167
    sx = 12 + col1_w + 195         # 517 (leaves room on right for rejection box)

    # 1. Start -> Truy cập màn hình quản lý tài khoản
    c += draw_start_node(ux, 62)
    c += draw_act_box(ux, 118, 210, 44, ["Truy cập chức năng", "quản lý tài khoản"], "user")
    c += draw_flow_arrow([(ux, 73), (ux, 96)])

    # Arrow to System: Kiểm tra đăng nhập và quyền quản trị
    c += draw_act_box(sx, 118, 225, 44, ["Kiểm tra đăng nhập và", "quyền quản trị viên"], "system")
    c += draw_flow_arrow([(ux + 105, 118), (sx - 112.5, 118)])

    # Decision: Có quyền truy cập?
    c += draw_act_diamond(sx, 198, 145, 50, ["Có quyền", "truy cập?"])
    c += draw_flow_arrow([(sx, 140), (sx, 173)])

    # Branch [Không có quyền] -> Red box on right (center at 740)
    c += draw_act_box(742, 198, 130, 44, ["Từ chối truy cập", "khu vực quản trị"], "error", font_size=11)
    c += draw_flow_arrow([(sx + 72.5, 198), (742 - 65, 198)], "[Không có quyền]", ((sx + 72.5 + 677)/2, 198))

    # Branch [Có quyền] -> Truy vấn danh sách tài khoản
    c += draw_act_box(sx, 282, 230, 42, ["Truy vấn danh sách tài khoản", "và thống kê theo vai trò"], "system")
    c += draw_flow_arrow([(sx, 223), (sx, 261)], "[Có quyền]", (sx, 241))

    c += draw_act_box(sx, 350, 235, 42, ["Hiển thị danh sách tài khoản", "và số liệu thống kê vai trò"], "system")
    c += draw_flow_arrow([(sx, 303), (sx, 329)])

    # Back to Admin lane: Chọn thao tác
    c += draw_act_box(ux, 418, 220, 50, ["Chọn thao tác: Thêm tài khoản,", "Khóa/Mở khóa hoặc Đặt lại", "mật khẩu và gửi dữ liệu"], "user")
    c += draw_flow_arrow([(sx - 117.5, 350), (ux, 350), (ux, 393)])

    # To System: Kiểm tra dữ liệu và điều kiện nghiệp vụ
    c += draw_act_box(sx, 438, 235, 46, ["Kiểm tra tính hợp lệ của dữ liệu", "(Trùng tên đăng nhập, định dạng)"], "system")
    c += draw_flow_arrow([(ux + 110, 418), (355, 418), (355, 438), (sx - 117.5, 438)])

    # Decision: Dữ liệu hợp lệ?
    c += draw_act_diamond(sx, 522, 155, 50, ["Dữ liệu", "hợp lệ?"])
    c += draw_flow_arrow([(sx, 461), (sx, 497)])

    # [Không hợp lệ] -> Error box in Admin lane
    c += draw_act_box(ux, 522, 210, 46, ["Hiển thị thông báo lỗi", "(Từ chối lưu thay đổi)"], "error")
    c += draw_flow_arrow([(sx - 77.5, 522), (ux + 105, 522)], "[Không hợp lệ]", (355, 522))

    # [Hợp lệ] -> Lưu thông tin
    c += draw_act_box(sx, 608, 235, 46, ["Cập nhật thông tin tài khoản,", "trạng thái hoặc mật khẩu vào CSDL"], "system")
    c += draw_flow_arrow([(sx, 547), (sx, 585)], "[Hợp lệ]", (sx, 565))

    c += draw_act_box(sx, 678, 225, 40, ["Thông báo kết quả xử lý và", "cập nhật danh sách hiển thị"], "system")
    c += draw_flow_arrow([(sx, 631), (sx, 658)])

    # End node
    c += draw_end_node(sx, 740)
    c += draw_flow_arrow([(sx, 698), (sx, 727)])
    c += draw_flow_arrow([(ux, 545), (ux, 740), (sx - 13, 740)])
    c += draw_flow_arrow([(807, 198), (816, 198), (816, 740), (sx + 13, 740)])

    render_svg_to_png(svg_wrapper(w, h, c), "Hinh2_22_AD_QuanLyTaiKhoan.png", w, h)

def generate_sd_quanty_taikhoan():
    w, h = 1040, 770
    c = ""

    x1, x2, x3, x4 = 95, 345, 620, 915
    c += draw_seq_header(x1, 15, 130, 42, ["Quản trị viên"], "actor")
    c += draw_seq_header(x2, 15, 155, 42, ["Giao diện quản lý", "tài khoản"], "box_blue")
    c += draw_seq_header(x3, 15, 160, 42, ["Bộ điều khiển", "quản lý tài khoản"], "box_blue")
    c += draw_seq_header(x4, 15, 135, 42, ["Cơ sở dữ liệu"], "cylinder")

    for lx in [x1, x2, x3, x4]:
        c += f'<line x1="{lx}" y1="57" x2="{lx}" y2="{h - 15}" stroke="#666666" stroke-width="1.2" stroke-dasharray="4,4" />'

    c += draw_seq_msg(x1, x2, 88, "1. Truy cập chức năng quản lý tài khoản")
    c += draw_seq_msg(x2, x3, 118, "2. Gửi yêu cầu lấy danh sách tài khoản")
    c += draw_seq_self(x3, 138, "3. Kiểm tra quyền quản trị viên")

    c += draw_seq_condition(x1 - 15, 178, "[Không có quyền truy cập]")
    c += draw_seq_msg(x3, x2, 200, "4. Từ chối truy cập &amp; trả kết quả lỗi phân quyền", dashed=True)
    c += draw_seq_msg(x2, x1, 230, "5. Hiển thị thông báo từ chối truy cập", dashed=True)

    c += draw_seq_condition(x1 - 15, 265, "[Quản trị viên có quyền truy cập]", divider_y=248, div_x1=x1+10, div_x2=x4-10)
    c += draw_seq_msg(x3, x4, 288, "6. Truy vấn danh sách tài khoản &amp; thống kê theo vai trò")
    c += draw_seq_msg(x4, x3, 318, "7. Trả kết quả danh sách &amp; số liệu thống kê", dashed=True)
    c += draw_seq_msg(x3, x2, 348, "8. Trả dữ liệu danh sách tài khoản", dashed=True)
    c += draw_seq_msg(x2, x1, 378, "9. Hiển thị danh sách tài khoản &amp; công cụ quản trị", dashed=True)

    c += draw_seq_msg(x1, x2, 415, "10. Gửi yêu cầu thêm tài khoản / khóa tài khoản / đặt lại mật khẩu")
    c += draw_seq_msg(x2, x3, 445, "11. Chuyển dữ liệu thao tác quản lý tài khoản")
    c += draw_seq_msg(x3, x4, 475, "12. Truy vấn kiểm tra tài khoản &amp; tên đăng nhập hiện có")
    c += draw_seq_msg(x4, x3, 505, "13. Trả kết quả kiểm tra dữ liệu", dashed=True)
    c += draw_seq_self(x3, 525, "14. Kiểm tra tính hợp lệ dữ liệu, trạng thái &amp; mã hóa mật khẩu")

    c += draw_seq_condition(x1 - 15, 568, "[Dữ liệu hợp lệ]")
    c += draw_seq_msg(x3, x4, 590, "15. Lưu tài khoản mới hoặc cập nhật trạng thái / mật khẩu")
    c += draw_seq_msg(x4, x3, 620, "16. Xác nhận lưu dữ liệu thành công", dashed=True)
    c += draw_seq_msg(x3, x2, 650, "17. Trả kết quả xử lý thành công", dashed=True)
    c += draw_seq_msg(x2, x1, 680, "18. Hiển thị thông báo thành công &amp; danh sách mới", dashed=True)

    c += draw_seq_condition(x1 - 15, 712, "[Dữ liệu không hợp lệ hoặc trùng tên đăng nhập]", divider_y=696, div_x1=x1+10, div_x2=x4-10)
    c += draw_seq_msg(x3, x2, 732, "19. Trả kết quả thất bại kèm nguyên nhân lỗi", dashed=True)
    c += draw_seq_msg(x2, x1, 756, "20. Hiển thị thông báo lỗi để chỉnh sửa", dashed=True)

    render_svg_to_png(svg_wrapper(w, h, c), "Hinh2_23_SD_QuanLyTaiKhoan.png", w, h)

# ==============================================================================
# 3. BỘ 3: QUẢN LÝ ĐƠN VỊ TỔ CHỨC (HÌNH 2.24, 2.25, 2.26)
# ==============================================================================
def generate_uc_quanty_donvitochuc():
    w, h = 900, 580
    bg = "#f5f5f5"
    c = ""

    c += draw_actor(65, 285, "Quản trị viên")

    # Main UC
    mcx, mcy, mrx, mry = 215, 280, 90, 38
    c += draw_ellipse(mcx, mcy, mrx, mry, ["Quản lý", "đơn vị tổ chức"], fill="#dae8fc", stroke="#6c8ebf", font_weight="bold", font_size=13)
    c += f'<line x1="85" y1="280" x2="{mcx - mrx}" y2="280" stroke="#222222" stroke-width="1.3" />'

    # Included UCs (exact layout of image15.png)
    inc_ucs = [
        (345, 115, 95, 34, ["Xác thực quyền", "người dùng"]),
        (485, 280, 95, 34, ["Xem danh sách", "đơn vị tổ chức"]),
        (345, 445, 100, 34, ["Kiểm tra ràng buộc", "đợt hiến máu"])
    ]
    for scx, scy, srx, sry, lines in inc_ucs:
        c += draw_ellipse(scx, scy, srx, sry, lines, fill="#d5e8d4", stroke="#82b366", font_size=12)
        rel_svg, _ = draw_uc_relation(mcx, mcy, mrx, mry, scx, scy, srx, sry, "include", bg)
        c += rel_svg

    # 3 Extended UCs from "Xem danh sách đơn vị tổ chức" (485, 280)
    ext_ucs = [
        (705, 95, 98, 34, ["Tìm kiếm & xem", "chi tiết đơn vị"], (385, 25, 165, 44, ["Điều kiện mở rộng:", "[Chọn tra cứu hoặc xem", "chi tiết đơn vị tổ chức]"])),
        (790, 280, 98, 34, ["Thêm mới & cập nhật", "đơn vị tổ chức"], (695, 172, 165, 44, ["Điều kiện mở rộng:", "[Quản trị viên chọn thêm", "hoặc sửa thông tin đơn vị]"])),
        (735, 455, 98, 34, ["Xóa đơn vị", "tổ chức"], (435, 490, 165, 44, ["Điều kiện mở rộng:", "[Quản trị viên chọn xóa", "đơn vị tổ chức]"]))
    ]
    for ecx, ecy, erx, ery, lines, note_info in ext_ucs:
        c += draw_ellipse(ecx, ecy, erx, ery, lines, fill="#fff2cc", stroke="#d6b656", font_size=12)
        rel_svg, (mx, my) = draw_uc_relation(ecx, ecy, erx, ery, 485, 280, 95, 34, "extend", bg)
        c += rel_svg
        nx, ny, nw, nh, nlines = note_info
        c += draw_uc_note(nx, ny, nw, nh, nlines, mx, my)

    render_svg_to_png(svg_wrapper(w, h, c, bg), "Hinh2_24_UC_QuanLyDonViToChuc.png", w, h, bg)

def generate_ad_quanty_donvitochuc():
    w, h = 780, 800
    col1_w = 310
    c = ""
    c += draw_swimlane(12, 12, w - 24, h - 24, col1_w, "Quản trị viên", "Hệ thống")

    ux = 12 + col1_w / 2           # 167
    sx = 12 + col1_w + (w - 24 - col1_w) / 2  # 545

    c += draw_start_node(ux, 62)
    c += draw_act_box(ux, 118, 210, 44, ["Truy cập danh sách", "đơn vị tổ chức"], "user")
    c += draw_flow_arrow([(ux, 73), (ux, 96)])

    c += draw_act_box(sx, 118, 230, 44, ["Kiểm tra phân quyền và truy vấn", "danh sách đơn vị tổ chức"], "system")
    c += draw_flow_arrow([(ux + 105, 118), (sx - 115, 118)])

    c += draw_act_box(sx, 188, 230, 42, ["Hiển thị danh sách đơn vị tổ chức", "và thông tin đợt hiến máu"], "system")
    c += draw_flow_arrow([(sx, 140), (sx, 167)])

    # Back to User lane: Chọn thao tác
    c += draw_act_box(ux, 255, 215, 44, ["Lựa chọn thao tác: Thêm mới,", "Cập nhật hoặc Xóa đơn vị"], "user")
    c += draw_flow_arrow([(sx - 115, 188), (ux, 188), (ux, 233)])

    # Decision in System lane: Loại thao tác xử lý?
    c += draw_act_diamond(sx, 255, 165, 50, ["Yêu cầu", "xóa đơn vị?"])
    c += draw_flow_arrow([(ux + 107.5, 255), (sx - 82.5, 255)])

    # Branch 1: [Thêm / Cập nhật] -> Goes to User input box at ux, 350
    c += draw_act_box(ux, 350, 215, 46, ["Nhập thông tin đơn vị tổ chức", "(Tên, mô tả, email, trạng thái)"], "user")
    c += draw_flow_arrow([(sx, 280), (sx, 305), (ux, 305), (ux, 327)], "[Thêm / Cập nhật]", (360, 305))

    c += draw_act_box(sx, 350, 230, 46, ["Kiểm tra tên đơn vị bắt buộc,", "không trùng lặp và định dạng email"], "system")
    c += draw_flow_arrow([(ux + 107.5, 350), (sx - 115, 350)])

    # Branch 2: [Yêu cầu xóa] -> Goes right then down to box at sx, 435
    c += draw_act_box(sx, 435, 235, 46, ["Kiểm tra ràng buộc dữ liệu:", "Đơn vị đã có đợt hiến máu liên kết?"], "system")
    c += draw_flow_arrow([(sx + 82.5, 255), (sx + 165, 255), (sx + 165, 435), (sx + 117.5, 435)], "[Xóa đơn vị]", (sx + 165, 345))

    # Decision: Đủ điều kiện thực hiện?
    c += draw_act_diamond(sx, 525, 175, 52, ["Thỏa mãn", "điều kiện?"])
    c += draw_flow_arrow([(sx, 458), (sx, 499)])
    # Connect Thêm/Cập nhật check box bottom-left to left vertex of Decision
    c += draw_flow_arrow([(sx - 95, 373), (sx - 145, 373), (sx - 145, 525), (sx - 87.5, 525)])

    # [Vi phạm điều kiện / Đã có đợt hiến máu] -> Error box in User lane
    c += draw_act_box(ux, 525, 220, 56, ["Thông báo từ chối thao tác", "(Trùng tên đơn vị hoặc không được", "xóa đơn vị đã có đợt hiến máu)"], "error", font_size=11)
    c += draw_flow_arrow([(sx, 551), (sx, 570), (350, 570), (350, 525), (ux + 110, 525)], "[Không hợp lệ]", (350, 548))

    # [Hợp lệ] -> Lưu hoặc xóa trong CSDL
    c += draw_act_box(sx, 625, 235, 44, ["Lưu thông tin đơn vị hoặc xóa", "đơn vị chưa có đợt hiến máu"], "system")
    c += draw_flow_arrow([(sx + 87.5, 525), (sx + 140, 525), (sx + 140, 625), (sx + 117.5, 625)], "[Hợp lệ]", (sx + 140, 575))

    c += draw_act_box(sx, 692, 225, 40, ["Thông báo thực hiện thành công", "và cập nhật danh sách"], "system")
    c += draw_flow_arrow([(sx, 647), (sx, 672)])

    # End node
    c += draw_end_node(sx, 750)
    c += draw_flow_arrow([(sx, 712), (sx, 737)])
    c += draw_flow_arrow([(ux, 553), (ux, 750), (sx - 13, 750)])

    render_svg_to_png(svg_wrapper(w, h, c), "Hinh2_25_AD_QuanLyDonViToChuc.png", w, h)

def generate_sd_quanty_donvitochuc():
    w, h = 1040, 760
    c = ""

    x1, x2, x3, x4 = 95, 345, 620, 915
    c += draw_seq_header(x1, 15, 130, 42, ["Quản trị viên"], "actor")
    c += draw_seq_header(x2, 15, 155, 42, ["Giao diện quản lý", "đơn vị tổ chức"], "box_blue")
    c += draw_seq_header(x3, 15, 160, 42, ["Bộ điều khiển", "đơn vị tổ chức"], "box_blue")
    c += draw_seq_header(x4, 15, 135, 42, ["Cơ sở dữ liệu"], "cylinder")

    for lx in [x1, x2, x3, x4]:
        c += f'<line x1="{lx}" y1="57" x2="{lx}" y2="{h - 15}" stroke="#666666" stroke-width="1.2" stroke-dasharray="4,4" />'

    c += draw_seq_msg(x1, x2, 88, "1. Truy cập chức năng quản lý đơn vị tổ chức")
    c += draw_seq_msg(x2, x3, 118, "2. Gửi yêu cầu lấy danh sách đơn vị tổ chức")
    c += draw_seq_self(x3, 138, "3. Kiểm tra quyền truy cập của người dùng")
    c += draw_seq_msg(x3, x4, 178, "4. Truy vấn danh sách đơn vị tổ chức &amp; đợt hiến máu")
    c += draw_seq_msg(x4, x3, 208, "5. Trả dữ liệu danh sách đơn vị tổ chức", dashed=True)
    c += draw_seq_msg(x3, x2, 238, "6. Trả kết quả danh sách cần hiển thị", dashed=True)
    c += draw_seq_msg(x2, x1, 268, "7. Hiển thị danh sách đơn vị tổ chức", dashed=True)

    # Thêm / Sửa
    c += draw_seq_msg(x1, x2, 305, "8. Nhập thông tin thêm mới / cập nhật đơn vị &amp; gửi yêu cầu")
    c += draw_seq_msg(x2, x3, 335, "9. Chuyển dữ liệu đơn vị tổ chức")
    c += draw_seq_msg(x3, x4, 365, "10. Kiểm tra trùng tên đơn vị &amp; lưu thông tin hợp lệ")
    c += draw_seq_msg(x4, x3, 395, "11. Xác nhận lưu dữ liệu thành công", dashed=True)
    c += draw_seq_msg(x3, x2, 425, "12. Trả kết quả cập nhật thành công", dashed=True)
    c += draw_seq_msg(x2, x1, 455, "13. Hiển thị thông báo thành công &amp; danh sách mới", dashed=True)

    # Xóa có kiểm tra ràng buộc đợt hiến máu
    c += draw_seq_msg(x1, x2, 495, "14. Chọn đơn vị tổ chức cần xóa &amp; xác nhận xóa")
    c += draw_seq_msg(x2, x3, 525, "15. Gửi yêu cầu xóa đơn vị tổ chức (kèm mã đơn vị)")
    c += draw_seq_msg(x3, x4, 555, "16. Truy vấn kiểm tra các đợt hiến máu thuộc đơn vị")
    c += draw_seq_msg(x4, x3, 585, "17. Trả thông tin đợt hiến máu liên quan", dashed=True)
    c += draw_seq_self(x3, 603, "18. Kiểm tra ràng buộc toàn vẹn dữ liệu đợt hiến máu")

    c += draw_seq_condition(x1 - 15, 642, "[Đơn vị chưa có đợt hiến máu liên kết - Được phép xóa]")
    c += draw_seq_msg(x3, x4, 662, "19. Thực hiện xóa đơn vị tổ chức khỏi cơ sở dữ liệu")
    c += draw_seq_msg(x3, x1, 690, "20. Thông báo xóa đơn vị tổ chức thành công", dashed=True)

    c += draw_seq_condition(x1 - 15, 720, "[Đơn vị đã có đợt hiến máu liên kết - Không được phép xóa]", divider_y=704, div_x1=x1+10, div_x2=x4-10)
    c += draw_seq_msg(x3, x1, 742, "21. Từ chối xóa &amp; hiển thị cảnh báo bảo toàn dữ liệu đợt hiến máu", dashed=True)

    render_svg_to_png(svg_wrapper(w, h, c), "Hinh2_26_SD_QuanLyDonViToChuc.png", w, h)

# ==============================================================================
# 4. BỘ 4: ĐĂNG KÝ TÀI KHOẢN NGƯỜI HIẾN MÁU (HÌNH 2.27, 2.28, 2.29)
# ==============================================================================
def generate_uc_dangky():
    w, h = 760, 560
    bg = "#f5f5f5"
    c = ""

    c += draw_actor(75, 275, "Người hiến máu")

    mcx, mcy, mrx, mry = 255, 270, 95, 38
    c += draw_ellipse(mcx, mcy, mrx, mry, ["Đăng ký tài khoản"], fill="#dae8fc", stroke="#6c8ebf", font_weight="bold", font_size=13)
    c += f'<line x1="95" y1="270" x2="{mcx - mrx}" y2="270" stroke="#222222" stroke-width="1.3" />'

    inc_ucs = [
        (415, 70, 105, 34, ["Nhập thông tin", "tài khoản & cá nhân"]),
        (575, 140, 105, 34, ["Kiểm tra định dạng", "dữ liệu đăng ký"]),
        (615, 240, 105, 34, ["Kiểm tra trùng", "tên đăng nhập"]),
        (615, 340, 105, 34, ["Tạo tài khoản mới", "(Vai trò Người hiến)"]),
        (570, 435, 105, 34, ["Khởi tạo hồ sơ", "người hiến máu"]),
        (395, 495, 105, 34, ["Thông báo kết quả", "đăng ký"])
    ]
    for scx, scy, srx, sry, lines in inc_ucs:
        c += draw_ellipse(scx, scy, srx, sry, lines, fill="#d5e8d4", stroke="#82b366", font_size=12)
        rel_svg, _ = draw_uc_relation(mcx, mcy, mrx, mry, scx, scy, srx, sry, "include", bg)
        c += rel_svg

    render_svg_to_png(svg_wrapper(w, h, c, bg), "Hinh2_27_UC_DangKyTaiKhoan.png", w, h, bg)

def generate_ad_dangky():
    w, h = 760, 760
    col1_w = 310
    c = ""
    c += draw_swimlane(12, 12, w - 24, h - 24, col1_w, "Người hiến máu", "Hệ thống")

    ux = 12 + col1_w / 2           # 167
    sx = 12 + col1_w + (w - 24 - col1_w) / 2  # 535

    c += draw_start_node(ux, 62)
    c += draw_act_box(ux, 118, 200, 42, ["Mở biểu mẫu đăng ký", "tài khoản"], "user")
    c += draw_flow_arrow([(ux, 73), (ux, 97)])

    c += draw_act_box(sx, 118, 220, 42, ["Hiển thị biểu mẫu đăng ký", "tài khoản người hiến máu"], "system")
    c += draw_flow_arrow([(ux + 100, 118), (sx - 110, 118)])

    c += draw_act_box(ux, 195, 215, 46, ["Nhập thông tin cá nhân, tên", "đăng nhập và mật khẩu"], "user")
    c += draw_flow_arrow([(sx, 139), (sx, 158), (ux, 158), (ux, 172)])

    c += draw_act_box(ux, 270, 190, 38, ["Gửi biểu mẫu đăng ký"], "user")
    c += draw_flow_arrow([(ux, 218), (ux, 251)])

    c += draw_act_box(sx, 270, 230, 44, ["Kiểm tra tính đầy đủ và định dạng", "(Họ tên, email, SĐT, mật khẩu)"], "system")
    c += draw_flow_arrow([(ux + 95, 270), (sx - 115, 270)])

    c += draw_act_box(sx, 348, 230, 44, ["Kiểm tra trùng tên đăng nhập", "trong cơ sở dữ liệu"], "system")
    c += draw_flow_arrow([(sx, 292), (sx, 326)])

    c += draw_act_diamond(sx, 430, 165, 52, ["Dữ liệu", "hợp lệ?"])
    c += draw_flow_arrow([(sx, 370), (sx, 404)])

    # [Không hợp lệ]
    c += draw_act_box(ux, 430, 215, 48, ["Thông báo lỗi dữ liệu", "(Không tạo tài khoản mới)"], "error")
    c += draw_flow_arrow([(sx - 82.5, 430), (ux + 107.5, 430)], "[Không hợp lệ]", (355, 430))

    # [Hợp lệ]
    c += draw_act_box(sx, 520, 235, 44, ["Tạo bản ghi tài khoản mới với", "vai trò Người hiến máu"], "system")
    c += draw_flow_arrow([(sx, 456), (sx, 498)], "[Hợp lệ]", (sx, 476))

    c += draw_act_box(sx, 592, 235, 44, ["Khởi tạo hồ sơ người hiến máu", "liên kết với tài khoản vừa tạo"], "system")
    c += draw_flow_arrow([(sx, 542), (sx, 570)])

    c += draw_act_box(sx, 662, 220, 40, ["Thông báo đăng ký tài khoản", "thành công"], "system")
    c += draw_flow_arrow([(sx, 614), (sx, 642)])

    c += draw_end_node(sx, 720)
    c += draw_flow_arrow([(sx, 682), (sx, 707)])
    c += draw_flow_arrow([(ux, 454), (ux, 720), (sx - 13, 720)])

    render_svg_to_png(svg_wrapper(w, h, c), "Hinh2_28_AD_DangKyTaiKhoan.png", w, h)

def generate_sd_dangky():
    w, h = 1040, 740
    c = ""

    x1, x2, x3, x4 = 95, 345, 620, 915
    c += draw_seq_header(x1, 15, 130, 42, ["Người hiến máu"], "actor")
    c += draw_seq_header(x2, 15, 145, 42, ["Giao diện", "đăng ký"], "box_blue")
    c += draw_seq_header(x3, 15, 150, 42, ["Bộ điều khiển", "tài khoản"], "box_blue")
    c += draw_seq_header(x4, 15, 135, 42, ["Cơ sở dữ liệu"], "cylinder")

    for lx in [x1, x2, x3, x4]:
        c += f'<line x1="{lx}" y1="57" x2="{lx}" y2="{h - 15}" stroke="#666666" stroke-width="1.2" stroke-dasharray="4,4" />'

    c += draw_seq_msg(x1, x2, 90, "1. Chọn chức năng đăng ký tài khoản")
    c += draw_seq_msg(x2, x3, 122, "2. Gửi yêu cầu mở biểu mẫu đăng ký")
    c += draw_seq_msg(x3, x2, 154, "3. Trả về biểu mẫu đăng ký", dashed=True)
    c += draw_seq_msg(x2, x1, 186, "4. Hiển thị biểu mẫu đăng ký", dashed=True)

    c += draw_seq_msg(x1, x2, 222, "5. Nhập thông tin tài khoản, hồ sơ cá nhân &amp; gửi biểu mẫu")
    c += draw_seq_msg(x2, x3, 254, "6. Gửi dữ liệu đăng ký tài khoản")
    c += draw_seq_self(x3, 276, "7. Kiểm tra định dạng email, số điện thoại, mật khẩu &amp; cân nặng")

    c += draw_seq_msg(x3, x4, 324, "8. Truy vấn kiểm tra tên đăng nhập trùng lặp")
    c += draw_seq_msg(x4, x3, 356, "9. Trả kết quả kiểm tra tên đăng nhập", dashed=True)

    # Hợp lệ
    c += draw_seq_condition(x1 - 15, 396, "[Tất cả dữ liệu hợp lệ và tên đăng nhập chưa tồn tại]")
    c += draw_seq_self(x3, 412, "10. Mã hóa mật khẩu &amp; khởi tạo đối tượng tài khoản mới")
    c += draw_seq_msg(x3, x4, 458, "11. Lưu tài khoản mới &amp; khởi tạo hồ sơ người hiến máu")
    c += draw_seq_msg(x4, x3, 490, "12. Trả kết quả lưu thành công", dashed=True)
    c += draw_seq_msg(x3, x2, 522, "13. Trả kết quả đăng ký tài khoản thành công", dashed=True)
    c += draw_seq_msg(x2, x1, 554, "14. Thông báo đăng ký thành công &amp; chuyển đến trang chủ", dashed=True)

    # Không hợp lệ
    c += draw_seq_condition(x1 - 15, 596, "[Dữ liệu sai định dạng hoặc tên đăng nhập đã tồn tại]", divider_y=576, div_x1=x1+10, div_x2=x4-10)
    c += draw_seq_self(x3, 612, "15. Không tạo tài khoản &amp; xác định thông báo lỗi")
    c += draw_seq_msg(x3, x2, 658, "16. Trả kết quả thất bại kèm nguyên nhân lỗi", dashed=True)
    c += draw_seq_msg(x2, x1, 690, "17. Hiển thị thông báo lỗi trên biểu mẫu đăng ký", dashed=True)

    render_svg_to_png(svg_wrapper(w, h, c), "Hinh2_29_SD_DangKyTaiKhoan.png", w, h)

if __name__ == "__main__":
    print("=== REGENERATING ALL MODULE 1 DIAGRAMS (PURE DESIGN, ZERO OVERLAP) ===")
    generate_uc_tongquan()
    generate_uc_dangnhap()
    generate_ad_dangnhap()
    generate_sd_dangnhap()
    generate_uc_quanty_taikhoan()
    generate_ad_quanty_taikhoan()
    generate_sd_quanty_taikhoan()
    generate_uc_quanty_donvitochuc()
    generate_ad_quanty_donvitochuc()
    generate_sd_quanty_donvitochuc()
    generate_uc_dangky()
    generate_ad_dangky()
    generate_sd_dangky()
    print("=== COMPLETED ===")

using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyQuanAn.Forms
{
    public class FrmMain : Form
    {
        public FrmMain()
        {
            Text = "HỆ THỐNG QUẢN LÝ QUÁN CAFE - TRÀ SỮA (TRANG CHỦ)";
            Size = new Size(1040, 520);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10F);
            BackColor = Color.WhiteSmoke;

            // Thanh tiêu đề phía trên (tự căn chỉnh khoảng cách, không bao giờ bị đè chữ)
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = Color.FromArgb(78, 52, 46),
                Padding = new Padding(20, 0, 20, 0)
            };

            var lblApp = new Label
            {
                Text = "QUẢN LÝ QUÁN CAFE & TRÀ SỮA",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Dock = DockStyle.Left,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = true
            };

            var pnlRight = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                AutoSize = true,
                WrapContents = false,
                Padding = new Padding(0, 16, 0, 0),
                BackColor = Color.Transparent
            };

            var lblUser = new Label
            {
                Text = $"Xin chào: {FrmDangNhap.TenNhanVienHienTai} ({FrmDangNhap.ChucVuHienTai})",
                ForeColor = Color.Bisque,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 6, 15, 0)
            };

            var btnDangXuat = new Button
            {
                Text = "Đăng xuất",
                BackColor = Color.IndianRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(105, 32),
                Cursor = Cursors.Hand
            };
            btnDangXuat.Click += (_, _) => Application.Restart();

            pnlRight.Controls.Add(lblUser);
            pnlRight.Controls.Add(btnDangXuat);
            pnlHeader.Controls.Add(pnlRight);
            pnlHeader.Controls.Add(lblApp);

            // ==================== CỘT 1: PHẦN 1 (TV01 - POS & CRM) ====================
            var grpPhan1 = TaoNhomChucNang("PHẦN 1: BÁN HÀNG & KHÁCH HÀNG (TV01)", 25);
            var btnP1_1 = TaoNut("1. Lập Hóa đơn & Order (POS)", 45, Color.SteelBlue);
            var btnP1_2 = TaoNut("2. Quản lý Hóa đơn & Thanh toán", 110, Color.SteelBlue);
            var btnP1_3 = TaoNut("3. Khách hàng & Tích điểm", 175, Color.SteelBlue);

            // TODO (TV01): Thay MessageBox bằng lệnh mở Form của TV01 (VD: new FrmBanHang().ShowDialog();)
            btnP1_1.Click += (_, _) => MessageBox.Show("Chức năng của Thành viên 01 (Đang chờ ghép Form).", "Thông báo");
            btnP1_2.Click += (_, _) => MessageBox.Show("Chức năng của Thành viên 01 (Đang chờ ghép Form).", "Thông báo");
            btnP1_3.Click += (_, _) => MessageBox.Show("Chức năng của Thành viên 01 (Đang chờ ghép Form).", "Thông báo");
            grpPhan1.Controls.AddRange(new Control[] { btnP1_1, btnP1_2, btnP1_3 });

            // ==================== CỘT 2: PHẦN 2 (TV02 - CỦA BẠN) ====================
            var grpPhan2 = TaoNhomChucNang("PHẦN 2: THỰC ĐƠN & KHO (TV02)", 360);
            var btnP2_1 = TaoNut("1. Quản lý Danh mục Đồ uống", 45, Color.SeaGreen);
            var btnP2_2 = TaoNut("2. Quản lý Thực đơn & Giá bán", 110, Color.SeaGreen);
            var btnP2_3 = TaoNut("3. Quản lý Kho & Nhập nguyên liệu", 175, Color.SeaGreen);

            btnP2_1.Click += (_, _) => new FrmDanhMucMon().ShowDialog();
            btnP2_2.Click += (_, _) => new FrmQuanLyMonAn().ShowDialog();
            btnP2_3.Click += (_, _) => new FrmKhoNguyenLieu().ShowDialog();
            grpPhan2.Controls.AddRange(new Control[] { btnP2_1, btnP2_2, btnP2_3 });

            // ==================== CỘT 3: PHẦN 3 (TV03 - NHÂN SỰ & THỐNG KÊ) ====================
            var grpPhan3 = TaoNhomChucNang("PHẦN 3: BÀN, NHÂN SỰ & BÁO CÁO (TV03)", 695);
            var btnP3_1 = TaoNut("1. Quản lý Sơ đồ Bàn Cafe", 45, Color.DarkSlateBlue);
            var btnP3_2 = TaoNut("2. Quản lý Nhân viên & Tài khoản", 110, Color.DarkSlateBlue);
            var btnP3_3 = TaoNut("3. Báo cáo Thống kê Doanh thu", 175, Color.DarkSlateBlue);

            // TODO (TV03): Thay MessageBox bằng lệnh mở Form của TV03 (VD: new FrmThongKe().ShowDialog();)
            btnP3_1.Click += (_, _) => MessageBox.Show("Chức năng của Thành viên 03 (Đang chờ ghép Form).", "Thông báo");
            btnP3_2.Click += (_, _) => MessageBox.Show("Chức năng của Thành viên 03 (Đang chờ ghép Form).", "Thông báo");
            btnP3_3.Click += (_, _) => MessageBox.Show("Chức năng của Thành viên 03 (Đang chờ ghép Form).", "Thông báo");
            grpPhan3.Controls.AddRange(new Control[] { btnP3_1, btnP3_2, btnP3_3 });

            Controls.AddRange(new Control[] { pnlHeader, grpPhan1, grpPhan2, grpPhan3 });
        }

        private static GroupBox TaoNhomChucNang(string title, int x)
        {
            return new GroupBox
            {
                Text = title,
                Location = new Point(x, 90),
                Size = new Size(310, 350),
                BackColor = Color.White,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
        }

        private static Button TaoNut(string text, int y, Color bgColor)
        {
            return new Button
            {
                Text = text,
                Location = new Point(20, y),
                Size = new Size(270, 48),
                BackColor = bgColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }
    }
}
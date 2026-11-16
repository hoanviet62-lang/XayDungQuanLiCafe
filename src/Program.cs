using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyQuanAn.Forms;

namespace QuanLyQuanAn
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var menu = new Form
            {
                Text = "PHẦN 2 (THÀNH VIÊN 02) - THỰC ĐƠN & KHO NGUYÊN LIỆU",
                Size = new Size(460, 280),
                StartPosition = FormStartPosition.CenterScreen,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold)
            };

            var b1 = new Button { Text = "1. Quản lý Danh mục Món (FrmDanhMucMon)", Location = new Point(35, 25), Size = new Size(375, 48), BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var b2 = new Button { Text = "2. Quản lý Thực đơn Món ăn (FrmQuanLyMonAn)", Location = new Point(35, 90), Size = new Size(375, 48), BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var b3 = new Button { Text = "3. Quản lý Kho & Nhập kho (FrmKhoNguyenLieu)", Location = new Point(35, 155), Size = new Size(375, 48), BackColor = Color.DarkSlateBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            b1.Click += (_, _) => new FrmDanhMucMon().ShowDialog();
            b2.Click += (_, _) => new FrmQuanLyMonAn().ShowDialog();
            b3.Click += (_, _) => new FrmKhoNguyenLieu().ShowDialog();

            menu.Controls.AddRange(new Control[] { b1, b2, b3 });
            Application.Run(menu);
        }
    }
}

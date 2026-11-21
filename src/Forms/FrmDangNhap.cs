using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using QuanLyQuanAn.DAL;

namespace QuanLyQuanAn.Forms
{
    public class FrmDangNhap : Form
    {
        public static string TenNhanVienHienTai { get; private set; } = string.Empty;
        public static string ChucVuHienTai { get; private set; } = string.Empty;

        private TextBox txtTaiKhoan = new() { Text = "admin" };
        private TextBox txtMatKhau = new() { Text = "123456", UseSystemPasswordChar = true };

        public FrmDangNhap()
        {
            Text = "Đăng nhập Hệ thống Quản lý Quán Cafe";
            Size = new Size(420, 270);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Font = new Font("Segoe UI", 10F);

            var lblTitle = new Label
            {
                Text = "ĐĂNG NHẬP HỆ THỐNG CAFE",
                Dock = DockStyle.Top,
                Height = 52,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(78, 52, 46),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold)
            };

            var lblTK = new Label { Text = "Tài khoản:", Location = new Point(35, 78), AutoSize = true };
            txtTaiKhoan.SetBounds(130, 75, 235, 27);

            var lblMK = new Label { Text = "Mật khẩu:", Location = new Point(35, 120), AutoSize = true };
            txtMatKhau.SetBounds(130, 117, 235, 27);

            var btnDangNhap = new Button
            {
                Text = "Đăng nhập",
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnDangNhap.SetBounds(130, 165, 115, 36);

            var btnThoat = new Button
            {
                Text = "Thoát",
                BackColor = Color.DimGray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnThoat.SetBounds(255, 165, 110, 36);

            Controls.AddRange(new Control[] { lblTitle, lblTK, txtTaiKhoan, lblMK, txtMatKhau, btnDangNhap, btnThoat });
            AcceptButton = btnDangNhap;

            btnThoat.Click += (_, _) => Application.Exit();
            btnDangNhap.Click += (_, _) => XuLyDangNhap();
        }

        private void XuLyDangNhap()
        {
            if (string.IsNullOrWhiteSpace(txtTaiKhoan.Text) || string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tài khoản và mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using var conn = DatabaseHelper.GetConnection();
                conn.Open();
                string sql = "SELECT HoTen, ChucVu FROM NhanVien WHERE TaiKhoan = @tk AND MatKhau = @mk AND TrangThai = 1";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@tk", txtTaiKhoan.Text.Trim());
                cmd.Parameters.AddWithValue("@mk", txtMatKhau.Text.Trim());

                using var rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    TenNhanVienHienTai = rd.GetString(0);
                    ChucVuHienTai = rd.GetString(1);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Sai tài khoản hoặc mật khẩu (hoặc tài khoản đã bị khóa)!", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối CSDL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

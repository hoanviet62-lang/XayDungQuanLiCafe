
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using QuanLyQuanAn.DAL;

namespace QuanLyQuanAn.Forms
{
    public class FrmThongKe : Form
    {
        private DateTimePicker dtTuNgay = new()
        {
            Format = DateTimePickerFormat.Short
        };

        private DateTimePicker dtDenNgay = new()
        {
            Format = DateTimePickerFormat.Short
        };

        private Label lblTongDoanhThu = new()
        {
            Text = "Tổng doanh thu: 0 VNĐ",
            AutoSize = true,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.DarkGreen
        };

        private DataGridView dgvDoanhThu = TaoGrid();
        private DataGridView dgvNguyenLieu = TaoGrid();
        private DataGridView dgvNhanVien = TaoGrid();

        public FrmThongKe()
        {
            Text = "Phần 3 - Báo cáo thống kê";
            Size = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10);

            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.WhiteSmoke
            };

            panel.Controls.Add(new Label
            {
                Text = "Từ ngày:",
                Location = new Point(20, 20),
                AutoSize = true
            });

            dtTuNgay.SetBounds(100, 17, 130, 28);

            panel.Controls.Add(new Label
            {
                Text = "Đến ngày:",
                Location = new Point(260, 20),
                AutoSize = true
            });

            dtDenNgay.SetBounds(340, 17, 130, 28);

            var btnThongKe = new Button
            {
                Text = "Thống kê",
                Location = new Point(500, 15),
                Size = new Size(120, 35),
                BackColor = Color.DarkSlateBlue,
                ForeColor = Color.White
            };

            lblTongDoanhThu.Location = new Point(20, 60);

            panel.Controls.AddRange(new Control[]
            {
                dtTuNgay, dtDenNgay, btnThongKe, lblTongDoanhThu
            });

            var tabs = new TabControl
            {
                Dock = DockStyle.Fill
            };

            var tabDoanhThu = new TabPage("Doanh thu");
            var tabNguyenLieu = new TabPage("Nguyên liệu cần nhập");
            var tabNhanVien = new TabPage("Doanh thu theo nhân viên");

            tabDoanhThu.Controls.Add(dgvDoanhThu);
            tabNguyenLieu.Controls.Add(dgvNguyenLieu);
            tabNhanVien.Controls.Add(dgvNhanVien);

            tabs.TabPages.Add(tabDoanhThu);
            tabs.TabPages.Add(tabNguyenLieu);
            tabs.TabPages.Add(tabNhanVien);

            Controls.Add(tabs);
            Controls.Add(panel);

            btnThongKe.Click += (_, _) => ThongKe();

            Load += (_, _) =>
            {
                dtTuNgay.Value = DateTime.Today.AddDays(-30);
                dtDenNgay.Value = DateTime.Today;
                ThongKe();
            };
        }

        private static DataGridView TaoGrid()
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect
            };
        }

        private void ThongKe()
        {
            if (dtTuNgay.Value.Date > dtDenNgay.Value.Date)
            {
                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc!");
                return;
            }

            ThongKeDoanhThu();
            ThongKeNguyenLieu();
            ThongKeTheoNhanVien();
        }

        private void ThongKeDoanhThu()
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                conn.Open();

                string sql = @"
                    SELECT
                        CONVERT(date, NgayLap) AS Ngay,
                        COUNT(MaHD) AS SoHoaDon,
                        SUM(TongTien - GiamGia) AS DoanhThu,
                        COUNT(DISTINCT MaNV) AS SoNhanVien
                    FROM HoaDon
                    WHERE TrangThai = N'Đã thanh toán'
                      AND NgayLap >= @tu
                      AND NgayLap < DATEADD(day, 1, @den)
                    GROUP BY CONVERT(date, NgayLap)
                    ORDER BY Ngay";

                using var cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@tu", dtTuNgay.Value.Date);
                cmd.Parameters.AddWithValue("@den", dtDenNgay.Value.Date);

                using var da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvDoanhThu.DataSource = dt;

                decimal tong = 0;

                foreach (DataRow row in dt.Rows)
                {
                    if (row["DoanhThu"] != DBNull.Value)
                        tong += Convert.ToDecimal(row["DoanhThu"]);
                }

                lblTongDoanhThu.Text =
                    "Tổng doanh thu: " + tong.ToString("N0") + " VNĐ";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thống kê doanh thu: " + ex.Message);
            }
        }

        private void ThongKeNguyenLieu()
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                conn.Open();

                string sql = @"
                    SELECT
                        MaNL,
                        TenNL,
                        DonViTinh,
                        SoLuongTon,
                        MucCanhBao
                    FROM NguyenLieu
                    WHERE SoLuongTon <= MucCanhBao
                    ORDER BY SoLuongTon ASC";

                using var da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvNguyenLieu.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thống kê nguyên liệu: " + ex.Message);
            }
        }

        private void ThongKeTheoNhanVien()
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                conn.Open();
                const string sql = @"
                    SELECT nv.MaNV AS [Mã nhân viên], nv.HoTen AS [Nhân viên],
                           nv.ChucVu AS [Chức vụ], COUNT(hd.MaHD) AS [Số hóa đơn],
                           SUM(hd.TongTien - hd.GiamGia) AS [Doanh thu thực nhận]
                    FROM NhanVien nv
                    LEFT JOIN HoaDon hd ON hd.MaNV = nv.MaNV
                        AND hd.TrangThai = N'Đã thanh toán'
                        AND hd.NgayLap >= @tu AND hd.NgayLap < DATEADD(day, 1, @den)
                    GROUP BY nv.MaNV, nv.HoTen, nv.ChucVu
                    ORDER BY SUM(hd.TongTien - hd.GiamGia) DESC";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@tu", dtTuNgay.Value.Date);
                cmd.Parameters.AddWithValue("@den", dtDenNgay.Value.Date);
                using var da = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                dgvNhanVien.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thống kê theo nhân viên: " + ex.Message);
            }
        }

    }
}


using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using QuanLyQuanAn.DAL;

namespace QuanLyQuanAn.Forms
{
    public class FrmQuanLyNhanVien : Form
    {
        private DataGridView dgv = new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AllowUserToAddRows = false
        };

        private TextBox txtHoTen = new();
        private TextBox txtSDT = new();
        private TextBox txtTaiKhoan = new();
        private TextBox txtMatKhau = new();

        private ComboBox cboChucVu = new()
        {
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        private NumericUpDown numLuong = new()
        {
            Minimum = 0,
            Maximum = 100000000,
            ThousandsSeparator = true
        };

        private CheckBox chkHoatDong = new()
        {
            Text = "Đang làm việc",
            Checked = true,
            AutoSize = true
        };

        private int maNV = 0;

        public FrmQuanLyNhanVien()
        {
            Text = "Phần 3 - Quản lý nhân viên & tài khoản";
            Size = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10);

            cboChucVu.Items.AddRange(new object[]
            {
                "Quản lý",
                "Thu ngân",
                "Phục vụ"
            });
            cboChucVu.SelectedIndex = 0;

            txtMatKhau.UseSystemPasswordChar = true;

            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 230,
                BackColor = Color.WhiteSmoke
            };

            AddLabel(panel, "Họ tên:", 20, 20);
            txtHoTen.SetBounds(110, 17, 220, 28);

            AddLabel(panel, "Số điện thoại:", 370, 20);
            txtSDT.SetBounds(480, 17, 200, 28);

            AddLabel(panel, "Chức vụ:", 20, 65);
            cboChucVu.SetBounds(110, 62, 220, 28);

            AddLabel(panel, "Lương cơ bản:", 370, 65);
            numLuong.SetBounds(480, 62, 200, 28);

            AddLabel(panel, "Tài khoản:", 20, 110);
            txtTaiKhoan.SetBounds(110, 107, 220, 28);

            AddLabel(panel, "Mật khẩu:", 370, 110);
            txtMatKhau.SetBounds(480, 107, 200, 28);

            chkHoatDong.Location = new Point(110, 150);

            var btnThem = TaoNut("Thêm", 20, 185);
            var btnSua = TaoNut("Cập nhật", 140, 185);
            var btnXoa = TaoNut("Xóa", 260, 185);
            var btnTai = TaoNut("Tải lại", 380, 185);
            var btnMoi = TaoNut("Làm mới", 500, 185);

            panel.Controls.AddRange(new Control[]
            {
                txtHoTen, txtSDT, cboChucVu, numLuong,
                txtTaiKhoan, txtMatKhau, chkHoatDong,
                btnThem, btnSua, btnXoa, btnTai, btnMoi
            });

            Controls.Add(dgv);
            Controls.Add(panel);

            Load += (_, _) => TaiDanhSach();

            dgv.SelectionChanged += (_, _) => ChonNhanVien();

            btnThem.Click += (_, _) => ThemNhanVien();
            btnSua.Click += (_, _) => SuaNhanVien();
            btnXoa.Click += (_, _) => XoaNhanVien();
            btnTai.Click += (_, _) => TaiDanhSach();
            btnMoi.Click += (_, _) => LamMoi();
        }

        private void AddLabel(Control p, string text, int x, int y)
        {
            p.Controls.Add(new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true
            });
        }

        private Button TaoNut(string text, int x, int y)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(105, 32),
                BackColor = Color.DarkSlateBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
        }

        private void TaiDanhSach()
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                conn.Open();

                string sql = @"SELECT MaNV, HoTen, ChucVu,
                    SoDienThoai, TaiKhoan, LuongCoBan,
                    TrangThai
                    FROM NhanVien
                    ORDER BY MaNV";

                using var da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgv.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải nhân viên: " + ex.Message);
            }
        }

        private bool KiemTra()
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                string.IsNullOrWhiteSpace(txtTaiKhoan.Text) ||
                string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                MessageBox.Show("Hãy nhập họ tên, tài khoản và mật khẩu!");
                return false;
            }

            return true;
        }

        private void ThemNhanVien()
        {
            if (!KiemTra()) return;

            try
            {
                using var conn = DatabaseHelper.GetConnection();
                conn.Open();

                string sql = @"INSERT INTO NhanVien
                    (HoTen, ChucVu, SoDienThoai, TaiKhoan,
                     MatKhau, LuongCoBan, TrangThai)
                    VALUES
                    (@ten, @cv, @sdt, @tk, @mk, @luong, @tt)";

                using var cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@ten", txtHoTen.Text.Trim());
                cmd.Parameters.AddWithValue("@cv", cboChucVu.Text);
                cmd.Parameters.AddWithValue("@sdt",
                    string.IsNullOrWhiteSpace(txtSDT.Text)
                    ? DBNull.Value : txtSDT.Text.Trim());
                cmd.Parameters.AddWithValue("@tk", txtTaiKhoan.Text.Trim());
                cmd.Parameters.AddWithValue("@mk", txtMatKhau.Text);
                cmd.Parameters.AddWithValue("@luong", numLuong.Value);
                cmd.Parameters.AddWithValue("@tt", chkHoatDong.Checked);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Thêm nhân viên thành công!");
                TaiDanhSach();
                LamMoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm nhân viên: " + ex.Message);
            }
        }

        private void ChonNhanVien()
        {
            if (dgv.CurrentRow == null ||
                dgv.CurrentRow.Cells["MaNV"].Value == null)
                return;

            var r = dgv.CurrentRow;

            maNV = Convert.ToInt32(r.Cells["MaNV"].Value);
            txtHoTen.Text = r.Cells["HoTen"].Value?.ToString();
            txtSDT.Text = r.Cells["SoDienThoai"].Value?.ToString();
            txtTaiKhoan.Text = r.Cells["TaiKhoan"].Value?.ToString();

            cboChucVu.Text = r.Cells["ChucVu"].Value?.ToString();

            numLuong.Value = Convert.ToDecimal(
                r.Cells["LuongCoBan"].Value ?? 0);

            chkHoatDong.Checked = Convert.ToBoolean(
                r.Cells["TrangThai"].Value ?? true);

            // Không hiển thị mật khẩu đã lưu.
            txtMatKhau.Clear();
        }

        private void SuaNhanVien()
        {
            if (maNV == 0)
            {
                MessageBox.Show("Hãy chọn nhân viên cần sửa!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                string.IsNullOrWhiteSpace(txtTaiKhoan.Text))
            {
                MessageBox.Show("Hãy nhập họ tên và tài khoản!");
                return;
            }

            try
            {
                using var conn = DatabaseHelper.GetConnection();
                conn.Open();

                string sql = @"UPDATE NhanVien SET
                    HoTen=@ten, ChucVu=@cv, SoDienThoai=@sdt,
                    TaiKhoan=@tk, LuongCoBan=@luong,
                    TrangThai=@tt
                    WHERE MaNV=@ma";

                if (!string.IsNullOrWhiteSpace(txtMatKhau.Text))
                    sql = sql.Replace(
                        "LuongCoBan=@luong,",
                        "LuongCoBan=@luong, MatKhau=@mk,");

                using var cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@ten", txtHoTen.Text.Trim());
                cmd.Parameters.AddWithValue("@cv", cboChucVu.Text);
                cmd.Parameters.AddWithValue("@sdt",
                    string.IsNullOrWhiteSpace(txtSDT.Text)
                    ? DBNull.Value : txtSDT.Text.Trim());
                cmd.Parameters.AddWithValue("@tk", txtTaiKhoan.Text.Trim());
                cmd.Parameters.AddWithValue("@luong", numLuong.Value);
                cmd.Parameters.AddWithValue("@tt", chkHoatDong.Checked);
                cmd.Parameters.AddWithValue("@ma", maNV);

                if (!string.IsNullOrWhiteSpace(txtMatKhau.Text))
                    cmd.Parameters.AddWithValue("@mk", txtMatKhau.Text);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Cập nhật nhân viên thành công!");
                TaiDanhSach();
                LamMoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật: " + ex.Message);
            }
        }


        private void XoaNhanVien()
        {
            if (maNV == 0)
            {
                MessageBox.Show(
                    "Hãy chọn nhân viên cần xóa!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa nhân viên này không?\n" +
                "Nhân viên sẽ bị xóa hoàn toàn khỏi cơ sở dữ liệu!",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes)
                return;

            try
            {
                using var conn = DatabaseHelper.GetConnection();
                conn.Open();

                string sql = "DELETE FROM NhanVien WHERE MaNV = @ma";

                using var cmd = new SqlCommand(sql, conn);

                cmd.Parameters.Add("@ma", SqlDbType.Int).Value = maNV;

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    MessageBox.Show(
                        "Xóa nhân viên thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    maNV = 0;

                    LamMoi();
                    TaiDanhSach();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy nhân viên cần xóa!");
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Không thể xóa nhân viên!\n" +
                    "Nhân viên có thể đang được sử dụng trong hóa đơn " +
                    "hoặc phiếu nhập.\n\n" +
                    ex.Message,
                    "Lỗi SQL Server",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi xóa nhân viên: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        private void LamMoi()
        {
            maNV = 0;
            txtHoTen.Clear();
            txtSDT.Clear();
            txtTaiKhoan.Clear();
            txtMatKhau.Clear();
            cboChucVu.SelectedIndex = 0;
            numLuong.Value = 0;
            chkHoatDong.Checked = true;
            dgv.ClearSelection();
        }
    }
}

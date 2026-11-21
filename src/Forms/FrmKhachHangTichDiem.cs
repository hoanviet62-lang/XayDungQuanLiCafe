using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyQuanAn.BUS;
using QuanLyQuanAn.DTO;

namespace QuanLyQuanAn.Forms
{
    public class FrmKhachHangTichDiem : Form
    {
        private readonly BanHangBUS _bus = new();
        private TextBox txtMaKh = null!;
        private TextBox txtTenKh = null!;
        private TextBox txtSdt = null!;
        private NumericUpDown numDiem = null!;
        private ComboBox cboHang = null!;
        private DataGridView dgvKhachHang = null!;
        private Button btnThem = null!;
        private Button btnSua = null!;
        private Button btnXoa = null!;
        private Button btnLamMoi = null!;

        public FrmKhachHangTichDiem()
        {
            Text = "Quản Lý Khách Hàng & Tích Điểm";
            Size = new Size(880, 560);
            StartPosition = FormStartPosition.CenterScreen;
            KhoiTaoGiaoDien();
            TaiDanhSachKhachHang();
        }

        private void KhoiTaoGiaoDien()
        {
            var lblMa = new Label { Text = "Mã KH:", Location = new Point(20, 22), AutoSize = true };
            txtMaKh = new TextBox { Location = new Point(95, 19), Width = 80, ReadOnly = true };

            var lblTen = new Label { Text = "Tên KH:", Location = new Point(195, 22), AutoSize = true };
            txtTenKh = new TextBox { Location = new Point(255, 19), Width = 220 };

            var lblSdt = new Label { Text = "SĐT:", Location = new Point(495, 22), AutoSize = true };
            txtSdt = new TextBox { Location = new Point(535, 19), Width = 150 };

            var lblDiem = new Label { Text = "Điểm:", Location = new Point(20, 62), AutoSize = true };
            numDiem = new NumericUpDown { Location = new Point(95, 59), Width = 100, Maximum = 1000000 };

            var lblHang = new Label { Text = "Hạng:", Location = new Point(215, 62), AutoSize = true };
            cboHang = new ComboBox { Location = new Point(260, 59), Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
            cboHang.Items.AddRange(new object[] { "Thường", "Bạc", "Vàng", "Kim cương" });
            cboHang.SelectedIndex = 0;

            btnThem = new Button { Text = "Thêm", Location = new Point(425, 56), Size = new Size(90, 32) };
            btnThem.Click += BtnThem_Click;

            btnSua = new Button { Text = "Sửa", Location = new Point(525, 56), Size = new Size(90, 32) };
            btnSua.Click += BtnSua_Click;

            btnXoa = new Button { Text = "Xóa", Location = new Point(625, 56), Size = new Size(90, 32) };
            btnXoa.Click += BtnXoa_Click;

            btnLamMoi = new Button { Text = "Làm mới", Location = new Point(725, 56), Size = new Size(95, 32) };
            btnLamMoi.Click += (_, _) => XoaTrang();

            dgvKhachHang = new DataGridView
            {
                Location = new Point(20, 105),
                Size = new Size(825, 395),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            dgvKhachHang.CellClick += DgvKhachHang_CellClick;

            Controls.AddRange(new Control[]
            {
                lblMa, txtMaKh, lblTen, txtTenKh, lblSdt, txtSdt,
                lblDiem, numDiem, lblHang, cboHang,
                btnThem, btnSua, btnXoa, btnLamMoi, dgvKhachHang
            });
        }

        private void TaiDanhSachKhachHang()
        {
            try
            {
                dgvKhachHang.DataSource = null;
                dgvKhachHang.DataSource = _bus.LayDanhSachKhachHang();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách khách hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvKhachHang_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (dgvKhachHang.CurrentRow?.DataBoundItem is not KhachHangDTO kh)
                return;

            txtMaKh.Text = kh.MaKH.ToString();
            txtTenKh.Text = kh.TenKH;
            txtSdt.Text = kh.SoDienThoai;
            numDiem.Value = kh.DiemTichLuy;
            cboHang.SelectedItem = string.IsNullOrWhiteSpace(kh.HangThanhVien) ? "Thường" : kh.HangThanhVien;
        }

        private void XoaTrang()
        {
            txtMaKh.Clear();
            txtTenKh.Clear();
            txtSdt.Clear();
            numDiem.Value = 0;
            cboHang.SelectedIndex = 0;
        }

        private void BtnThem_Click(object? sender, EventArgs e)
        {
            try
            {
                var kh = new KhachHangDTO
                {
                    TenKH = txtTenKh.Text.Trim(),
                    SoDienThoai = txtSdt.Text.Trim(),
                    DiemTichLuy = (int)numDiem.Value,
                    HangThanhVien = cboHang.SelectedItem?.ToString() ?? "Thường"
                };
                _bus.ThemKhachHang(kh);
                TaiDanhSachKhachHang();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi thêm khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnSua_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtMaKh.Text, out int maKh) || maKh <= 0)
                    throw new Exception("Vui lòng chọn khách hàng cần sửa!");

                var kh = new KhachHangDTO
                {
                    MaKH = maKh,
                    TenKH = txtTenKh.Text.Trim(),
                    SoDienThoai = txtSdt.Text.Trim(),
                    DiemTichLuy = (int)numDiem.Value,
                    HangThanhVien = cboHang.SelectedItem?.ToString() ?? "Thường"
                };
                _bus.SuaKhachHang(kh);
                TaiDanhSachKhachHang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi sửa khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnXoa_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtMaKh.Text, out int maKh) || maKh <= 0)
                    throw new Exception("Vui lòng chọn khách hàng cần xóa!");

                _bus.XoaKhachHang(maKh);
                TaiDanhSachKhachHang();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi xóa khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
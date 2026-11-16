using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QuanLyQuanAn.BUS;
using QuanLyQuanAn.DTO;

namespace QuanLyQuanAn.Forms
{
    public class FrmKhoNguyenLieu : Form
    {
        private readonly MenuInventoryBUS _bus = new();
        private readonly List<ChiTietNhapKhoDTO> _gioNhap = new();

        private DataGridView dgvKho = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
        private CheckBox chkSapHet = new() { Text = "Chỉ hiện nguyên liệu sắp hết (Tồn <= Mức cảnh báo)", AutoSize = true, ForeColor = Color.Firebrick, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };

        private ComboBox cboNguyenLieu = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private NumericUpDown numSoLuong = new() { DecimalPlaces = 1, Maximum = 10000, Minimum = 0, Value = 5 };
        private NumericUpDown numDonGia = new() { Maximum = 100000000, Increment = 10000, Value = 100000, ThousandsSeparator = true };
        private TextBox txtNhaCungCap = new() { Text = "Công ty Thực phẩm Sạch Hà Nội" };
        private TextBox txtGhiChu = new();
        private Label lblTongTien = new() { Text = "Tổng tiền nhập: 0 VNĐ", AutoSize = true, Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.DarkBlue };
        private DataGridView dgvChiTietNhap = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private DataGridView dgvLichSu = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

        public FrmKhoNguyenLieu()
        {
            Text = "Form 3 (TV02): Quản lý Kho Nguyên liệu & Nhập kho (SqlTransaction)";
            Size = new Size(1020, 640);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10F);

            var tabs = new TabControl { Dock = DockStyle.Fill };

            // --- TAB 1: TỒN KHO & CẢNH BÁO ---
            var tabTon = new TabPage("1. Tồn kho & Cảnh báo sắp hết");
            var pnlTonTop = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(12) };
            chkSapHet.Location = new Point(15, 14);
            var btnRefresh = new Button { Text = "Tải lại kho", Location = new Point(450, 10), Size = new Size(110, 30) };
            pnlTonTop.Controls.AddRange(new Control[] { chkSapHet, btnRefresh });
            tabTon.Controls.Add(dgvKho);
            tabTon.Controls.Add(pnlTonTop);

            // --- TAB 2: LẬP PHIẾU NHẬP KHO (TRANSACTION) ---
            var tabNhap = new TabPage("2. Lập Phiếu Nhập Kho (SqlTransaction)");
            var pnlNhapTop = new Panel { Dock = DockStyle.Top, Height = 165, Padding = new Padding(12) };
            pnlNhapTop.Controls.AddRange(new Control[]
            {
                new Label { Text = "Nhà cung cấp:", Location = new Point(15, 16), AutoSize = true }, txtNhaCungCap,
                new Label { Text = "Ghi chú:", Location = new Point(470, 16), AutoSize = true }, txtGhiChu,
                new Label { Text = "Nguyên liệu:", Location = new Point(15, 58), AutoSize = true }, cboNguyenLieu,
                new Label { Text = "SL nhập:", Location = new Point(360, 58), AutoSize = true }, numSoLuong,
                new Label { Text = "Đơn giá nhập:", Location = new Point(520, 58), AutoSize = true }, numDonGia,
                lblTongTien
            });

            txtNhaCungCap.SetBounds(125, 13, 320, 27);
            txtGhiChu.SetBounds(540, 13, 430, 27);
            cboNguyenLieu.SetBounds(125, 55, 220, 27);
            numSoLuong.SetBounds(425, 55, 80, 27);
            numDonGia.SetBounds(620, 55, 140, 27);

            var btnThemDong = new Button { Text = "+ Thêm vào phiếu", BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var btnLuuPhieu = new Button { Text = "Lưu Phiếu Nhập (Transaction)", BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnThemDong.SetBounds(775, 53, 195, 32);
            lblTongTien.Location = new Point(15, 110);
            btnLuuPhieu.SetBounds(740, 102, 230, 38);

            pnlNhapTop.Controls.AddRange(new Control[] { btnThemDong, btnLuuPhieu });
            tabNhap.Controls.Add(dgvChiTietNhap);
            tabNhap.Controls.Add(pnlNhapTop);

            // --- TAB 3: LỊCH SỬ PHIẾU NHẬP ---
            var tabLichSu = new TabPage("3. Lịch sử Phiếu Nhập Kho");
            tabLichSu.Controls.Add(dgvLichSu);

            tabs.TabPages.AddRange(new[] { tabTon, tabNhap, tabLichSu });
            Controls.Add(tabs);

            Load += (_, _) => LoadAll();
            chkSapHet.CheckedChanged += (_, _) => LoadKho();
            btnRefresh.Click += (_, _) => LoadKho();

            dgvKho.RowPrePaint += (_, e) =>
            {
                if (e.RowIndex >= 0 && dgvKho.Rows[e.RowIndex].DataBoundItem is NguyenLieuDTO nl && nl.SoLuongTon <= nl.MucCanhBao)
                    dgvKho.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.MistyRose;
            };

            btnThemDong.Click += (_, _) =>
            {
                if (cboNguyenLieu.SelectedItem is not NguyenLieuDTO nl) return;
                _gioNhap.Add(new ChiTietNhapKhoDTO
                {
                    MaNL = nl.MaNL,
                    TenNL = nl.TenNL,
                    SoLuongNhap = (double)numSoLuong.Value,
                    DonGiaNhap = numDonGia.Value
                });
                RefreshGioNhap();
            };

            btnLuuPhieu.Click += (_, _) =>
            {
                try
                {
                    _bus.NhapKho(new PhieuNhapKhoDTO
                    {
                        NhaCungCap = txtNhaCungCap.Text.Trim(),
                        GhiChu = txtGhiChu.Text.Trim(),
                        ChiTiet = new List<ChiTietNhapKhoDTO>(_gioNhap)
                    });
                    MessageBox.Show("Lưu phiếu nhập kho & cập nhật tồn kho thành công!", "Transaction Commit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _gioNhap.Clear();
                    RefreshGioNhap();
                    LoadAll();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Giao dịch đã Rollback!\nLỗi: {ex.Message}", "Transaction Rollback", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }

        private void LoadAll()
        {
            LoadKho();
            cboNguyenLieu.DataSource = _bus.LayDanhSachNguyenLieu(false);
            cboNguyenLieu.DisplayMember = "TenNL";
            cboNguyenLieu.ValueMember = "MaNL";
            dgvLichSu.DataSource = _bus.LayLichSuPhieuNhap();
        }

        private void LoadKho() => dgvKho.DataSource = _bus.LayDanhSachNguyenLieu(chkSapHet.Checked);

        private void RefreshGioNhap()
        {
            dgvChiTietNhap.DataSource = null;
            dgvChiTietNhap.DataSource = _gioNhap.ToList();
            decimal tong = _gioNhap.Sum(x => x.ThanhTien);
            lblTongTien.Text = $"Tổng tiền nhập: {tong:N0} VNĐ";
        }
    }
}

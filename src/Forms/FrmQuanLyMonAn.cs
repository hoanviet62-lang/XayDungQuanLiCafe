using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyQuanAn.BUS;
using QuanLyQuanAn.DTO;

namespace QuanLyQuanAn.Forms
{
    public class FrmQuanLyMonAn : Form
    {
        private readonly MenuInventoryBUS _bus = new();
        private TextBox txtMa = new() { ReadOnly = true, BackColor = Color.WhiteSmoke };
        private TextBox txtTen = new();
        private ComboBox cboDanhMuc = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private NumericUpDown numGia = new() { Maximum = 100000000, Increment = 5000, ThousandsSeparator = true };
        private TextBox txtDVT = new();
        private ComboBox cboTrangThai = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private ComboBox cboLocDanhMuc = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private TextBox txtTimKiem = new() { PlaceholderText = "Nhập tên món cần tìm..." };
        private DataGridView dgv = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

        public FrmQuanLyMonAn()
        {
            Text = "Form 2 (TV02): Quản lý Thực đơn & Giá bán";
            Size = new Size(980, 600);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10F);

            cboTrangThai.Items.AddRange(new object[] { "Còn bán", "Hết món" });
            cboTrangThai.SelectedIndex = 0;

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 205, Padding = new Padding(12) };
            pnlTop.Controls.AddRange(new Control[]
            {
                new Label { Text = "Mã món:", Location = new Point(15, 18), AutoSize = true }, txtMa,
                new Label { Text = "Tên món:", Location = new Point(200, 18), AutoSize = true }, txtTen,
                new Label { Text = "Danh mục:", Location = new Point(560, 18), AutoSize = true }, cboDanhMuc,
                new Label { Text = "Đơn giá:", Location = new Point(15, 58), AutoSize = true }, numGia,
                new Label { Text = "ĐVT:", Location = new Point(250, 58), AutoSize = true }, txtDVT,
                new Label { Text = "Trạng thái:", Location = new Point(450, 58), AutoSize = true }, cboTrangThai
            });

            txtMa.SetBounds(85, 15, 95, 27);
            txtTen.SetBounds(275, 15, 265, 27);
            cboDanhMuc.SetBounds(645, 15, 290, 27);
            numGia.SetBounds(85, 55, 145, 27);
            txtDVT.SetBounds(295, 55, 130, 27);
            cboTrangThai.SetBounds(535, 55, 160, 27);

            var btnThem = new Button { Text = "Thêm món", BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var btnSua = new Button { Text = "Cập nhật", BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var btnXoa = new Button { Text = "Xóa món", BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var btnLamMoi = new Button { Text = "Làm mới", BackColor = Color.DimGray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            btnThem.SetBounds(85, 98, 110, 34);
            btnSua.SetBounds(205, 98, 110, 34);
            btnXoa.SetBounds(325, 98, 110, 34);
            btnLamMoi.SetBounds(445, 98, 110, 34);

            var lblLoc = new Label { Text = "Lọc danh mục:", Location = new Point(15, 155), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            cboLocDanhMuc.SetBounds(125, 151, 220, 27);
            txtTimKiem.SetBounds(360, 151, 280, 27);
            var btnTim = new Button { Text = "Tìm kiếm", BackColor = Color.DarkSlateBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnTim.SetBounds(650, 149, 105, 31);

            pnlTop.Controls.AddRange(new Control[] { btnThem, btnSua, btnXoa, btnLamMoi, lblLoc, cboLocDanhMuc, txtTimKiem, btnTim });

            Controls.Add(dgv);
            Controls.Add(pnlTop);

            Load += (_, _) => { LoadDanhMuc(); LoadMonAn(); };
            cboLocDanhMuc.SelectedIndexChanged += (_, _) => LoadMonAn();
            btnTim.Click += (_, _) => LoadMonAn();

            dgv.CellClick += (_, e) =>
            {
                if (e.RowIndex >= 0 && dgv.Rows[e.RowIndex].DataBoundItem is MonAnDTO m)
                {
                    txtMa.Text = m.MaMon.ToString();
                    txtTen.Text = m.TenMon;
                    cboDanhMuc.SelectedValue = m.MaDanhMuc;
                    numGia.Value = m.DonGia;
                    txtDVT.Text = m.DonViTinh;
                    cboTrangThai.SelectedItem = m.TrangThai;
                }
            };

            btnThem.Click += (_, _) => Execute(() =>
            {
                _bus.ThemMonAn(new MonAnDTO
                {
                    TenMon = txtTen.Text.Trim(),
                    MaDanhMuc = (int)(cboDanhMuc.SelectedValue ?? 1),
                    DonGia = numGia.Value,
                    DonViTinh = txtDVT.Text.Trim(),
                    TrangThai = cboTrangThai.Text
                });
                ClearInput();
                LoadMonAn();
            });

            btnSua.Click += (_, _) => Execute(() =>
            {
                int.TryParse(txtMa.Text, out int ma);
                _bus.SuaMonAn(new MonAnDTO
                {
                    MaMon = ma,
                    TenMon = txtTen.Text.Trim(),
                    MaDanhMuc = (int)(cboDanhMuc.SelectedValue ?? 1),
                    DonGia = numGia.Value,
                    DonViTinh = txtDVT.Text.Trim(),
                    TrangThai = cboTrangThai.Text
                });
                LoadMonAn();
            });

            btnXoa.Click += (_, _) => Execute(() =>
            {
                int.TryParse(txtMa.Text, out int ma);
                if (MessageBox.Show("Xóa món ăn này khỏi thực đơn?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _bus.XoaMonAn(ma);
                    ClearInput();
                    LoadMonAn();
                }
            });

            btnLamMoi.Click += (_, _) => { ClearInput(); txtTimKiem.Clear(); cboLocDanhMuc.SelectedIndex = 0; LoadMonAn(); };
        }

        private void LoadDanhMuc()
        {
            var list1 = _bus.LayDanhSachDanhMuc();
            cboDanhMuc.DataSource = list1;
            cboDanhMuc.DisplayMember = "TenDanhMuc";
            cboDanhMuc.ValueMember = "MaDanhMuc";

            var list2 = _bus.LayDanhSachDanhMuc();
            list2.Insert(0, new DanhMucMonDTO { MaDanhMuc = 0, TenDanhMuc = "-- Tất cả danh mục --" });
            cboLocDanhMuc.DataSource = list2;
            cboLocDanhMuc.DisplayMember = "TenDanhMuc";
            cboLocDanhMuc.ValueMember = "MaDanhMuc";
        }

        private void LoadMonAn()
        {
            int maDM = cboLocDanhMuc.SelectedValue is int id ? id : 0;
            dgv.DataSource = _bus.LayDanhSachMonAn(maDM, txtTimKiem.Text);
            if (dgv.Columns["MaDanhMuc"] != null) dgv.Columns["MaDanhMuc"]!.Visible = false;
        }

        private void ClearInput() { txtMa.Clear(); txtTen.Clear(); txtDVT.Clear(); numGia.Value = 0; cboTrangThai.SelectedIndex = 0; }
        private void Execute(Action action)
        {
            try { action(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
    }
}

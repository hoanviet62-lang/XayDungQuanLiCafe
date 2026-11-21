using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyQuanAn.BUS;
using QuanLyQuanAn.DTO;

namespace QuanLyQuanAn.Forms
{
    public class FrmDanhMucMon : Form
    {
        private readonly MenuInventoryBUS _bus = new();
        private TextBox txtMa = new() { ReadOnly = true, BackColor = Color.WhiteSmoke };
        private TextBox txtTen = new();
        private TextBox txtMoTa = new();
        private DataGridView dgv = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

        public FrmDanhMucMon()
        {
            Text = "Form 1 (TV02): Quản lý Danh mục Món ăn";
            Size = new Size(850, 520);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10F);

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 150, Padding = new Padding(12) };
            pnlTop.Controls.AddRange(new Control[]
            {
                new Label { Text = "Mã DM:", Location = new Point(15, 18), AutoSize = true },
                txtMa,
                new Label { Text = "Tên danh mục:", Location = new Point(220, 18), AutoSize = true },
                txtTen,
                new Label { Text = "Mô tả:", Location = new Point(15, 58), AutoSize = true },
                txtMoTa
            });
            txtMa.SetBounds(85, 15, 110, 27);
            txtTen.SetBounds(330, 15, 480, 27);
            txtMoTa.SetBounds(85, 55, 725, 27);

            var btnThem = new Button { Text = "Thêm mới", BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var btnSua = new Button { Text = "Cập nhật", BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var btnXoa = new Button { Text = "Xóa", BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var btnLamMoi = new Button { Text = "Làm mới", BackColor = Color.DimGray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            btnThem.SetBounds(85, 98, 110, 34);
            btnSua.SetBounds(205, 98, 110, 34);
            btnXoa.SetBounds(325, 98, 110, 34);
            btnLamMoi.SetBounds(445, 98, 110, 34);
            pnlTop.Controls.AddRange(new Control[] { btnThem, btnSua, btnXoa, btnLamMoi });

            Controls.Add(dgv);
            Controls.Add(pnlTop);

            Load += (_, _) => LoadData();
            dgv.CellClick += (_, e) =>
            {
                if (e.RowIndex >= 0 && dgv.Rows[e.RowIndex].DataBoundItem is DanhMucMonDTO item)
                {
                    txtMa.Text = item.MaDanhMuc.ToString();
                    txtTen.Text = item.TenDanhMuc;
                    txtMoTa.Text = item.MoTa;
                }
            };

            btnThem.Click += (_, _) => Execute(() =>
            {
                _bus.ThemDanhMuc(new DanhMucMonDTO { TenDanhMuc = txtTen.Text.Trim(), MoTa = txtMoTa.Text.Trim() });
                ClearInput();
                LoadData();
            });

            btnSua.Click += (_, _) => Execute(() =>
            {
                int.TryParse(txtMa.Text, out int ma);
                _bus.SuaDanhMuc(new DanhMucMonDTO { MaDanhMuc = ma, TenDanhMuc = txtTen.Text.Trim(), MoTa = txtMoTa.Text.Trim() });
                LoadData();
            });

            btnXoa.Click += (_, _) => Execute(() =>
            {
                int.TryParse(txtMa.Text, out int ma);
                if (MessageBox.Show("Xóa danh mục này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _bus.XoaDanhMuc(ma);
                    ClearInput();
                    LoadData();
                }
            });

            btnLamMoi.Click += (_, _) => { ClearInput(); LoadData(); };
        }

        private void LoadData() => dgv.DataSource = _bus.LayDanhSachDanhMuc();
        private void ClearInput() { txtMa.Clear(); txtTen.Clear(); txtMoTa.Clear(); }
        private void Execute(Action action)
        {
            try { action(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
    }
}

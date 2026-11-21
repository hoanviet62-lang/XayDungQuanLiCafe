using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QuanLyQuanAn.BUS;
using QuanLyQuanAn.DTO;

namespace QuanLyQuanAn.Forms
{
    public class FrmLapHoaDonPOS : Form
    {
        private readonly BanHangBUS _bus = new();
        private readonly MenuInventoryBUS _menuBus = new();
        private readonly List<ChiTietHoaDonDTO> _gioHang = new();

        private ComboBox cboMon = null!;
        private NumericUpDown numSoLuong = null!;
        private TextBox txtMaBan = null!;
        private TextBox txtMaKhachHang = null!;
        private DataGridView dgvGioHang = null!;
        private Label lblTongTien = null!;
        private Button btnThemMon = null!;
        private Button btnXoaMon = null!;
        private Button btnLapHoaDon = null!;

        public FrmLapHoaDonPOS()
        {
            Text = "Lập Hóa Đơn / Order (POS)";
            Size = new Size(860, 540);
            StartPosition = FormStartPosition.CenterScreen;
            KhoiTaoGiaoDien();
            TaiDanhSachMon();
        }

        private void KhoiTaoGiaoDien()
        {
            var lblMaBan = new Label { Text = "Mã bàn:", Location = new Point(20, 20), AutoSize = true };
            txtMaBan = new TextBox { Location = new Point(85, 17), Width = 80, Text = "1" };

            var lblMaKh = new Label { Text = "Mã KH (tùy chọn):", Location = new Point(185, 20), AutoSize = true };
            txtMaKhachHang = new TextBox { Location = new Point(300, 17), Width = 80 };

            var lblMon = new Label { Text = "Chọn món:", Location = new Point(20, 60), AutoSize = true };
            cboMon = new ComboBox { Location = new Point(85, 57), Width = 295, DropDownStyle = ComboBoxStyle.DropDownList };

            var lblSl = new Label { Text = "Số lượng:", Location = new Point(400, 60), AutoSize = true };
            numSoLuong = new NumericUpDown { Location = new Point(465, 57), Width = 70, Minimum = 1, Maximum = 999, Value = 1 };

            btnThemMon = new Button { Text = "Thêm món", Location = new Point(555, 55), Size = new Size(110, 30) };
            btnThemMon.Click += BtnThemMon_Click;

            btnXoaMon = new Button { Text = "Xóa dòng", Location = new Point(675, 55), Size = new Size(110, 30) };
            btnXoaMon.Click += BtnXoaMon_Click;

            dgvGioHang = new DataGridView
            {
                Location = new Point(20, 100),
                Size = new Size(805, 310),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false
            };

            lblTongTien = new Label
            {
                Text = "Tổng tiền: 0 VNĐ",
                Location = new Point(20, 430),
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };

            btnLapHoaDon = new Button
            {
                Text = "Lập Hóa Đơn",
                Location = new Point(665, 425),
                Size = new Size(160, 40)
            };
            btnLapHoaDon.Click += BtnLapHoaDon_Click;

            Controls.AddRange(new Control[]
            {
                lblMaBan, txtMaBan, lblMaKh, txtMaKhachHang,
                lblMon, cboMon, lblSl, numSoLuong,
                btnThemMon, btnXoaMon, dgvGioHang, lblTongTien, btnLapHoaDon
            });
        }

        private void TaiDanhSachMon()
        {
            try
            {
                var dsMon = _menuBus.LayDanhSachMonAn();
                cboMon.DataSource = dsMon;
                cboMon.DisplayMember = "TenMon";
                cboMon.ValueMember = "MaMon";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách món: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnThemMon_Click(object? sender, EventArgs e)
        {
            if (cboMon.SelectedItem is not MonAnDTO mon)
                return;

            int soLuong = (int)numSoLuong.Value;
            var tonTai = _gioHang.FirstOrDefault(x => x.MaMon == mon.MaMon);
            if (tonTai != null)
            {
                tonTai.SoLuong += soLuong;
            }
            else
            {
                _gioHang.Add(new ChiTietHoaDonDTO
                {
                    MaMon = mon.MaMon,
                    TenMon = mon.TenMon,
                    SoLuong = soLuong,
                    DonGiaBan = mon.DonGia
                });
            }

            CapNhatBangVaTongTien();
        }

        private void BtnXoaMon_Click(object? sender, EventArgs e)
        {
            if (dgvGioHang.CurrentRow == null || dgvGioHang.CurrentRow.Index < 0)
                return;

            int idx = dgvGioHang.CurrentRow.Index;
            if (idx < _gioHang.Count)
            {
                _gioHang.RemoveAt(idx);
                CapNhatBangVaTongTien();
            }
        }

        private void CapNhatBangVaTongTien()
        {
            dgvGioHang.DataSource = null;
            dgvGioHang.DataSource = _gioHang.Select(x => new
            {
                x.MaMon,
                x.TenMon,
                x.SoLuong,
                x.DonGiaBan,
                x.ThanhTien
            }).ToList();

            decimal tong = _gioHang.Sum(x => x.ThanhTien);
            lblTongTien.Text = $"Tổng tiền: {tong:N0} VNĐ";
        }

        private void BtnLapHoaDon_Click(object? sender, EventArgs e)
        {
            try
            {
                int maBan = int.TryParse(txtMaBan.Text.Trim(), out int b) && b > 0 ? b : 1;
                int? maKh = int.TryParse(txtMaKhachHang.Text.Trim(), out int kh) && kh > 0 ? kh : null;
                decimal tongTien = _gioHang.Sum(x => x.ThanhTien);

                var hd = new HoaDonDTO
                {
                    MaBan = maBan,
                    MaNV = 1,
                    MaKH = maKh,
                    NgayLap = DateTime.Now,
                    TongTien = tongTien,
                    GiamGia = 0,
                    TrangThai = "Chưa thanh toán"
                };

                int maHdMoi = _bus.LapHoaDon(hd, _gioHang);
                MessageBox.Show($"Lập hóa đơn thành công! Mã hóa đơn: {maHdMoi}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _gioHang.Clear();
                CapNhatBangVaTongTien();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi lập hóa đơn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
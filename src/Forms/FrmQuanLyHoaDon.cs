using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyQuanAn.BUS;

namespace QuanLyQuanAn.Forms
{
    public class FrmQuanLyHoaDon : Form
    {
        private readonly BanHangBUS _bus = new();
        private DataGridView dgvHoaDon = null!;
        private DataGridView dgvChiTiet = null!;
        private Button btnTaiLai = null!;
        private Button btnThanhToan = null!;

        public FrmQuanLyHoaDon()
        {
            Text = "Quản Lý Hóa Đơn & Thanh Toán";
            Size = new Size(920, 600);
            StartPosition = FormStartPosition.CenterScreen;
            KhoiTaoGiaoDien();
            TaiDanhSachHoaDon();
        }

        private void KhoiTaoGiaoDien()
        {
            btnTaiLai = new Button { Text = "Tải lại", Location = new Point(20, 15), Size = new Size(100, 32) };
            btnTaiLai.Click += (_, _) => TaiDanhSachHoaDon();

            btnThanhToan = new Button { Text = "Thanh toán hóa đơn", Location = new Point(135, 15), Size = new Size(165, 32) };
            btnThanhToan.Click += BtnThanhToan_Click;

            dgvHoaDon = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(865, 250),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            dgvHoaDon.SelectionChanged += DgvHoaDon_SelectionChanged;

            var lblChiTiet = new Label { Text = "Chi tiết hóa đơn:", Location = new Point(20, 322), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };

            dgvChiTiet = new DataGridView
            {
                Location = new Point(20, 348),
                Size = new Size(865, 195),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false
            };

            Controls.AddRange(new Control[] { btnTaiLai, btnThanhToan, dgvHoaDon, lblChiTiet, dgvChiTiet });
        }

        private void TaiDanhSachHoaDon()
        {
            try
            {
                dgvHoaDon.DataSource = null;
                dgvHoaDon.DataSource = _bus.LayDanhSachHoaDon();
                TaiChiTietHoaDonDangChon();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvHoaDon_SelectionChanged(object? sender, EventArgs e)
        {
            TaiChiTietHoaDonDangChon();
        }

        private void TaiChiTietHoaDonDangChon()
        {
            if (dgvHoaDon.CurrentRow?.DataBoundItem is not DTO.HoaDonDTO hd)
            {
                dgvChiTiet.DataSource = null;
                return;
            }

            try
            {
                dgvChiTiet.DataSource = null;
                dgvChiTiet.DataSource = _bus.LayChiTietHoaDon(hd.MaHD);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải chi tiết hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnThanhToan_Click(object? sender, EventArgs e)
        {
            if (dgvHoaDon.CurrentRow?.DataBoundItem is not DTO.HoaDonDTO hd)
            {
                MessageBox.Show("Vui lòng chọn hóa đơn cần thanh toán!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _bus.ThanhToanHoaDon(hd.MaHD);
                MessageBox.Show($"Đã thanh toán hóa đơn #{hd.MaHD}!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TaiDanhSachHoaDon();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi thanh toán", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
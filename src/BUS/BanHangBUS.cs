using System;
using System.Collections.Generic;
using QuanLyQuanAn.DAL;
using QuanLyQuanAn.DTO;

namespace QuanLyQuanAn.BUS
{
    public class BanHangBUS
    {
        private readonly BanHangDAL _dal = new();

        public List<KhachHangDTO> LayDanhSachKhachHang() => _dal.GetAllKhachHang();

        public void ThemKhachHang(KhachHangDTO kh)
        {
            if (string.IsNullOrWhiteSpace(kh.TenKH))
                throw new Exception("Tên khách hàng không được để trống!");
            if (kh.DiemTichLuy < 0)
                throw new Exception("Điểm tích lũy không được nhỏ hơn 0!");
            _dal.InsertKhachHang(kh);
        }

        public void SuaKhachHang(KhachHangDTO kh)
        {
            if (kh.MaKH <= 0)
                throw new Exception("Chưa chọn khách hàng hợp lệ!");
            if (string.IsNullOrWhiteSpace(kh.TenKH))
                throw new Exception("Tên khách hàng không được để trống!");
            _dal.UpdateKhachHang(kh);
        }

        public void XoaKhachHang(int maKH)
        {
            if (maKH <= 0)
                throw new Exception("Chưa chọn khách hàng cần xóa!");
            _dal.DeleteKhachHang(maKH);
        }

        public int LapHoaDon(HoaDonDTO hd, List<ChiTietHoaDonDTO> chiTiet)
        {
            if (chiTiet == null || chiTiet.Count == 0)
                throw new Exception("Hóa đơn phải có ít nhất 1 món!");
            return _dal.TaoHoaDonTransaction(hd, chiTiet);
        }

        public List<HoaDonDTO> LayDanhSachHoaDon() => _dal.GetAllHoaDon();

        public List<ChiTietHoaDonDTO> LayChiTietHoaDon(int maHD)
        {
            if (maHD <= 0)
                throw new Exception("Mã hóa đơn không hợp lệ!");
            return _dal.GetChiTietHoaDon(maHD);
        }

        public void ThanhToanHoaDon(int maHD)
        {
            if (maHD <= 0)
                throw new Exception("Chưa chọn hóa đơn cần thanh toán!");
            if (!_dal.ThanhToanHoaDon(maHD))
                throw new Exception("Không thể cập nhật trạng thái thanh toán hóa đơn!");
        }
    }
}
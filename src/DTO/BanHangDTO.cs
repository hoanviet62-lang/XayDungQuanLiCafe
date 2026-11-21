using System;

namespace QuanLyQuanAn.DTO
{
    public class KhachHangDTO
    {
        public int MaKH { get; set; }
        public string TenKH { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public int DiemTichLuy { get; set; }
        public string HangThanhVien { get; set; } = "Thường";
    }

    public class HoaDonDTO
    {
        public int MaHD { get; set; }
        public DateTime NgayLap { get; set; }
        public int MaBan { get; set; }
        public int MaNV { get; set; } = 1;
        public int? MaKH { get; set; }
        public decimal TongTien { get; set; }
        public decimal GiamGia { get; set; }
        public string TrangThai { get; set; } = "Chưa thanh toán";
    }

    public class ChiTietHoaDonDTO
    {
        public int MaHD { get; set; }
        public int MaMon { get; set; }
        public string TenMon { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public decimal DonGiaBan { get; set; }
        public decimal ThanhTien => SoLuong * DonGiaBan;
    }
}
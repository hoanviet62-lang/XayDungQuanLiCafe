using System;
using System.Collections.Generic;

namespace QuanLyQuanAn.DTO
{
    public class DanhMucMonDTO
    {
        public int MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
    }

    public class MonAnDTO
    {
        public int MaMon { get; set; }
        public string TenMon { get; set; } = string.Empty;
        public int MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
        public string DonViTinh { get; set; } = string.Empty;
        public string TrangThai { get; set; } = "Còn bán";
    }

    public class NguyenLieuDTO
    {
        public int MaNL { get; set; }
        public string TenNL { get; set; } = string.Empty;
        public string DonViTinh { get; set; } = string.Empty;
        public double SoLuongTon { get; set; }
        public double MucCanhBao { get; set; }
    }

    public class ChiTietNhapKhoDTO
    {
        public int MaNL { get; set; }
        public string TenNL { get; set; } = string.Empty;
        public double SoLuongNhap { get; set; }
        public decimal DonGiaNhap { get; set; }
        public decimal ThanhTien => (decimal)SoLuongNhap * DonGiaNhap;
    }

    public class PhieuNhapKhoDTO
    {
        public int MaPhieuNhap { get; set; }
        public DateTime NgayNhap { get; set; } = DateTime.Now;
        public string NhaCungCap { get; set; } = string.Empty;
        public int MaNV { get; set; } = 3; // Mặc định nhân viên Kho & Bếp (MaNV = 3)
        public decimal TongTienNhap { get; set; }
        public string GhiChu { get; set; } = string.Empty;
        public List<ChiTietNhapKhoDTO> ChiTiet { get; set; } = new();
    }
}
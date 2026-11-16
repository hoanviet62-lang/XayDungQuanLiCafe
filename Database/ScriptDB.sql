USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'QuanLyQuanAnDB')
BEGIN
    ALTER DATABASE QuanLyQuanAnDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanLyQuanAnDB;
END
GO

CREATE DATABASE QuanLyQuanAnDB;
GO

USE QuanLyQuanAnDB;
GO

-- =========================================================================
-- PHẦN 3 (THÀNH VIÊN 03): NHÂN SỰ & SƠ ĐỒ BÀN QUÁN CAFE
-- =========================================================================
CREATE TABLE NhanVien (
    MaNV INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    ChucVu NVARCHAR(50) NOT NULL,
    SoDienThoai VARCHAR(15),
    TaiKhoan VARCHAR(50) UNIQUE NOT NULL,
    MatKhau VARCHAR(100) NOT NULL,
    LuongCoBan DECIMAL(18,0) DEFAULT 0,
    TrangThai BIT DEFAULT 1
);

CREATE TABLE BanAn (
    MaBan INT IDENTITY(1,1) PRIMARY KEY,
    TenBan NVARCHAR(50) NOT NULL,
    KhuVuc NVARCHAR(50) NOT NULL,
    SucChua INT DEFAULT 4,
    TrangThai NVARCHAR(30) DEFAULT N'Trống'
);

-- =========================================================================
-- PHẦN 2 (THÀNH VIÊN 02 - CỦA BẠN): THỰC ĐƠN CAFE & KHO NGUYÊN LIỆU
-- =========================================================================
CREATE TABLE DanhMucMon (
    MaDanhMuc INT IDENTITY(1,1) PRIMARY KEY,
    TenDanhMuc NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(255)
);

CREATE TABLE MonAn (
    MaMon INT IDENTITY(1,1) PRIMARY KEY,
    TenMon NVARCHAR(150) NOT NULL,
    MaDanhMuc INT NOT NULL FOREIGN KEY REFERENCES DanhMucMon(MaDanhMuc) ON DELETE CASCADE,
    DonGia DECIMAL(18,0) NOT NULL CHECK (DonGia >= 0),
    DonViTinh NVARCHAR(30) NOT NULL,
    TrangThai NVARCHAR(30) DEFAULT N'Còn bán'
);

CREATE TABLE NguyenLieu (
    MaNL INT IDENTITY(1,1) PRIMARY KEY,
    TenNL NVARCHAR(150) NOT NULL,
    DonViTinh NVARCHAR(30) NOT NULL,
    SoLuongTon FLOAT DEFAULT 0 CHECK (SoLuongTon >= 0),
    MucCanhBao FLOAT DEFAULT 5
);

CREATE TABLE PhieuNhapKho (
    MaPhieuNhap INT IDENTITY(1,1) PRIMARY KEY,
    NgayNhap DATETIME DEFAULT GETDATE(),
    NhaCungCap NVARCHAR(150) NOT NULL,
    MaNV INT NOT NULL FOREIGN KEY REFERENCES NhanVien(MaNV),
    TongTienNhap DECIMAL(18,0) DEFAULT 0,
    GhiChu NVARCHAR(255)
);

CREATE TABLE ChiTietNhapKho (
    MaPhieuNhap INT NOT NULL FOREIGN KEY REFERENCES PhieuNhapKho(MaPhieuNhap) ON DELETE CASCADE,
    MaNL INT NOT NULL FOREIGN KEY REFERENCES NguyenLieu(MaNL) ON DELETE CASCADE,
    SoLuongNhap FLOAT NOT NULL CHECK (SoLuongNhap > 0),
    DonGiaNhap DECIMAL(18,0) NOT NULL CHECK (DonGiaNhap >= 0),
    PRIMARY KEY (MaPhieuNhap, MaNL)
);

-- =========================================================================
-- PHẦN 1 (THÀNH VIÊN 01): BÁN HÀNG & KHÁCH HÀNG (POS & CRM)
-- =========================================================================
CREATE TABLE KhachHang (
    MaKH INT IDENTITY(1,1) PRIMARY KEY,
    TenKH NVARCHAR(100) NOT NULL,
    SoDienThoai VARCHAR(15) UNIQUE,
    DiemTichLuy INT DEFAULT 0,
    HangThanhVien NVARCHAR(30) DEFAULT N'Thường'
);

CREATE TABLE HoaDon (
    MaHD INT IDENTITY(1,1) PRIMARY KEY,
    NgayLap DATETIME DEFAULT GETDATE(),
    MaBan INT NOT NULL FOREIGN KEY REFERENCES BanAn(MaBan),
    MaNV INT NOT NULL FOREIGN KEY REFERENCES NhanVien(MaNV),
    MaKH INT NULL FOREIGN KEY REFERENCES KhachHang(MaKH),
    TongTien DECIMAL(18,0) DEFAULT 0,
    GiamGia DECIMAL(18,0) DEFAULT 0,
    TrangThai NVARCHAR(30) DEFAULT N'Chưa thanh toán'
);

CREATE TABLE ChiTietHoaDon (
    MaHD INT NOT NULL FOREIGN KEY REFERENCES HoaDon(MaHD) ON DELETE CASCADE,
    MaMon INT NOT NULL FOREIGN KEY REFERENCES MonAn(MaMon) ON DELETE CASCADE,
    SoLuong INT NOT NULL CHECK (SoLuong > 0),
    DonGiaBan DECIMAL(18,0) NOT NULL,
    ThanhTien AS (SoLuong * DonGiaBan),
    PRIMARY KEY (MaHD, MaMon)
);
GO

-- =========================================================================
-- DỮ LIỆU MẪU 100% QUÁN CAFE - TRÀ SỮA - BÁNH NGỌT
-- =========================================================================
INSERT INTO NhanVien (HoTen, ChucVu, SoDienThoai, TaiKhoan, MatKhau, LuongCoBan) VALUES
(N'Nguyễn Văn An', N'Quản lý', '0901111222', 'admin', '123456', 12000000),
(N'Trần Thị Bình', N'Thu ngân', '0903333444', 'thungan01', '123456', 7500000),
(N'Lê Hoàng Cường', N'Pha chế & Kho', '0905555666', 'barista01', '123456', 8500000);

INSERT INTO BanAn (TenBan, KhuVuc, SucChua, TrangThai) VALUES
(N'Bàn 01', N'Trong nhà', 2, N'Trống'),
(N'Bàn 02', N'Trong nhà', 4, N'Có khách'),
(N'Sân vườn 01', N'Sân vườn', 4, N'Trống'),
(N'Sân vườn 02', N'Sân vườn', 6, N'Trống'),
(N'Ban công 01', N'Tầng 2', 2, N'Có khách'),
(N'Phòng lạnh 01', N'Tầng 2', 8, N'Trống');

INSERT INTO DanhMucMon (TenDanhMuc, MoTa) VALUES
(N'Cà phê', N'Cà phê phin truyền thống, Espresso, Cà phê máy'),
(N'Trà trái cây & Trà sữa', N'Trà đào cam sả, trà vải, trà sữa trân châu'),
(N'Nước ép & Sinh tố', N'Nước ép trái cây tươi nguyên chất và sinh tố'),
(N'Bánh ngọt & Ăn vặt', N'Bánh Tiramisu, Croissant, hạt hướng dương');

INSERT INTO MonAn (TenMon, MaDanhMuc, DonGia, DonViTinh, TrangThai) VALUES
(N'Cà phê sữa đá', 1, 29000, N'Ly', N'Còn bán'),
(N'Bạc xỉu cốt dừa', 1, 35000, N'Ly', N'Còn bán'),
(N'Americano đá', 1, 32000, N'Ly', N'Còn bán'),
(N'Trà đào cam sả', 2, 42000, N'Ly', N'Còn bán'),
(N'Trà sữa trân châu đường đen', 2, 45000, N'Ly', N'Còn bán'),
(N'Sinh tố bơ sáp', 3, 48000, N'Ly', N'Còn bán'),
(N'Nước ép cam nguyên chất', 3, 38000, N'Ly', N'Còn bán'),
(N'Bánh Tiramisu', 4, 45000, N'Đĩa', N'Còn bán'),
(N'Bánh Croissant trứng muối', 4, 39000, N'Cái', N'Hết món');

INSERT INTO NguyenLieu (TenNL, DonViTinh, SoLuongTon, MucCanhBao) VALUES
(N'Hạt cà phê Robusta rang mộc', N'Kg', 18.5, 5),
(N'Sữa đặc Ngôi Sao Phương Nam', N'Hộp', 4.0, 6),
(N'Sữa tươi thanh trùng Dalatmilk', N'Lít', 15.0, 4),
(N'Trà đen túi lọc', N'Hộp', 12.0, 3),
(N'Đào ngâm hộp lớn', N'Hộp', 2.0, 5),
(N'Trân châu đen', N'Gói', 10.0, 3);

INSERT INTO PhieuNhapKho (NgayNhap, NhaCungCap, MaNV, TongTienNhap, GhiChu) VALUES
('2026-11-10 08:30:00', N'Công ty Cà phê Nguyên Chất Tây Nguyên', 3, 2700000, N'Nhập hạt cà phê định kỳ'),
('2026-11-15 09:15:00', N'Nhà phân phối Nguyên liệu Pha chế Hà Nội', 3, 1850000, N'Nhập sữa và trà cuối tuần');

INSERT INTO ChiTietNhapKho (MaPhieuNhap, MaNL, SoLuongNhap, DonGiaNhap) VALUES
(1, 1, 15, 180000),
(2, 2, 20, 25000),
(2, 4, 15, 90000);

INSERT INTO KhachHang (TenKH, SoDienThoai, DiemTichLuy, HangThanhVien) VALUES
(N'Phạm Minh Tuấn', '0912345678', 120, N'Vàng'),
(N'Đỗ Thu Hà', '0987654321', 45, N'Bạc'),
(N'Khách vãng lai', '0000000000', 0, N'Thường');

INSERT INTO HoaDon (NgayLap, MaBan, MaNV, MaKH, TongTien, GiamGia, TrangThai) VALUES
('2026-11-15 19:30:00', 1, 2, 1, 116000, 10000, N'Đã thanh toán'),
(GETDATE(), 2, 2, 2, 74000, 0, N'Chưa thanh toán');

INSERT INTO ChiTietHoaDon (MaHD, MaMon, SoLuong, DonGiaBan) VALUES
(1, 4, 2, 42000),
(1, 3, 1, 32000),
(2, 1, 1, 29000),
(2, 8, 1, 45000);
GO
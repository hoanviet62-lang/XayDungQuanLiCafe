using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using QuanLyQuanAn.DTO;

namespace QuanLyQuanAn.DAL
{
    public class BanHangDAL
    {
        public List<KhachHangDTO> GetAllKhachHang()
        {
            var list = new List<KhachHangDTO>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("SELECT MaKH, TenKH, SoDienThoai, DiemTichLuy, HangThanhVien FROM KhachHang ORDER BY MaKH DESC", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new KhachHangDTO
                {
                    MaKH = reader.GetInt32(reader.GetOrdinal("MaKH")),
                    TenKH = reader.IsDBNull(reader.GetOrdinal("TenKH")) ? string.Empty : reader.GetString(reader.GetOrdinal("TenKH")),
                    SoDienThoai = reader.IsDBNull(reader.GetOrdinal("SoDienThoai")) ? string.Empty : reader.GetString(reader.GetOrdinal("SoDienThoai")),
                    DiemTichLuy = reader.IsDBNull(reader.GetOrdinal("DiemTichLuy")) ? 0 : reader.GetInt32(reader.GetOrdinal("DiemTichLuy")),
                    HangThanhVien = reader.IsDBNull(reader.GetOrdinal("HangThanhVien")) ? "Thường" : reader.GetString(reader.GetOrdinal("HangThanhVien"))
                });
            }
            return list;
        }

        public void InsertKhachHang(KhachHangDTO kh)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "INSERT INTO KhachHang (TenKH, SoDienThoai, DiemTichLuy, HangThanhVien) VALUES (@TenKH, @SoDienThoai, @DiemTichLuy, @HangThanhVien)",
                conn);
            cmd.Parameters.AddWithValue("@TenKH", kh.TenKH);
            cmd.Parameters.AddWithValue("@SoDienThoai", string.IsNullOrWhiteSpace(kh.SoDienThoai) ? DBNull.Value : kh.SoDienThoai);
            cmd.Parameters.AddWithValue("@DiemTichLuy", kh.DiemTichLuy);
            cmd.Parameters.AddWithValue("@HangThanhVien", string.IsNullOrWhiteSpace(kh.HangThanhVien) ? "Thường" : kh.HangThanhVien);
            cmd.ExecuteNonQuery();
        }

        public void UpdateKhachHang(KhachHangDTO kh)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "UPDATE KhachHang SET TenKH = @TenKH, SoDienThoai = @SoDienThoai, DiemTichLuy = @DiemTichLuy, HangThanhVien = @HangThanhVien WHERE MaKH = @MaKH",
                conn);
            cmd.Parameters.AddWithValue("@MaKH", kh.MaKH);
            cmd.Parameters.AddWithValue("@TenKH", kh.TenKH);
            cmd.Parameters.AddWithValue("@SoDienThoai", string.IsNullOrWhiteSpace(kh.SoDienThoai) ? DBNull.Value : kh.SoDienThoai);
            cmd.Parameters.AddWithValue("@DiemTichLuy", kh.DiemTichLuy);
            cmd.Parameters.AddWithValue("@HangThanhVien", string.IsNullOrWhiteSpace(kh.HangThanhVien) ? "Thường" : kh.HangThanhVien);
            cmd.ExecuteNonQuery();
        }

        public void DeleteKhachHang(int maKH)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM KhachHang WHERE MaKH = @MaKH", conn);
            cmd.Parameters.AddWithValue("@MaKH", maKH);
            cmd.ExecuteNonQuery();
        }

        public int TaoHoaDonTransaction(HoaDonDTO hd, List<ChiTietHoaDonDTO> chiTiet)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                string sqlHd = @"
                    INSERT INTO HoaDon (NgayLap, MaBan, MaNV, MaKH, TongTien, GiamGia, TrangThai)
                    VALUES (@NgayLap, @MaBan, @MaNV, @MaKH, @TongTien, @GiamGia, @TrangThai);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using var cmdHd = new SqlCommand(sqlHd, conn, tran);
                cmdHd.Parameters.AddWithValue("@NgayLap", hd.NgayLap == default ? DateTime.Now : hd.NgayLap);
                cmdHd.Parameters.AddWithValue("@MaBan", hd.MaBan > 0 ? hd.MaBan : 1);
                cmdHd.Parameters.AddWithValue("@MaNV", hd.MaNV > 0 ? hd.MaNV : 1);
                cmdHd.Parameters.AddWithValue("@MaKH", hd.MaKH.HasValue && hd.MaKH.Value > 0 ? hd.MaKH.Value : DBNull.Value);
                cmdHd.Parameters.AddWithValue("@TongTien", hd.TongTien);
                cmdHd.Parameters.AddWithValue("@GiamGia", hd.GiamGia);
                cmdHd.Parameters.AddWithValue("@TrangThai", string.IsNullOrWhiteSpace(hd.TrangThai) ? "Chưa thanh toán" : hd.TrangThai);

                int maHdMoi = Convert.ToInt32(cmdHd.ExecuteScalar());

                string sqlCt = @"
                    INSERT INTO ChiTietHoaDon (MaHD, MaMon, SoLuong, DonGiaBan)
                    VALUES (@MaHD, @MaMon, @SoLuong, @DonGiaBan);";

                foreach (var item in chiTiet)
                {
                    using var cmdCt = new SqlCommand(sqlCt, conn, tran);
                    cmdCt.Parameters.AddWithValue("@MaHD", maHdMoi);
                    cmdCt.Parameters.AddWithValue("@MaMon", item.MaMon);
                    cmdCt.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                    cmdCt.Parameters.AddWithValue("@DonGiaBan", item.DonGiaBan);
                    cmdCt.ExecuteNonQuery();
                }

                tran.Commit();
                return maHdMoi;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public List<HoaDonDTO> GetAllHoaDon()
        {
            var list = new List<HoaDonDTO>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("SELECT MaHD, NgayLap, MaBan, MaNV, MaKH, TongTien, GiamGia, TrangThai FROM HoaDon ORDER BY MaHD DESC", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new HoaDonDTO
                {
                    MaHD = reader.GetInt32(reader.GetOrdinal("MaHD")),
                    NgayLap = reader.IsDBNull(reader.GetOrdinal("NgayLap")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("NgayLap")),
                    MaBan = reader.IsDBNull(reader.GetOrdinal("MaBan")) ? 0 : reader.GetInt32(reader.GetOrdinal("MaBan")),
                    MaNV = reader.IsDBNull(reader.GetOrdinal("MaNV")) ? 0 : reader.GetInt32(reader.GetOrdinal("MaNV")),
                    MaKH = reader.IsDBNull(reader.GetOrdinal("MaKH")) ? null : reader.GetInt32(reader.GetOrdinal("MaKH")),
                    TongTien = reader.IsDBNull(reader.GetOrdinal("TongTien")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TongTien")),
                    GiamGia = reader.IsDBNull(reader.GetOrdinal("GiamGia")) ? 0 : reader.GetDecimal(reader.GetOrdinal("GiamGia")),
                    TrangThai = reader.IsDBNull(reader.GetOrdinal("TrangThai")) ? "Chưa thanh toán" : reader.GetString(reader.GetOrdinal("TrangThai"))
                });
            }
            return list;
        }

        public List<ChiTietHoaDonDTO> GetChiTietHoaDon(int maHD)
        {
            var list = new List<ChiTietHoaDonDTO>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            string sql = @"
                SELECT ct.MaHD, ct.MaMon, m.TenMon, ct.SoLuong, ct.DonGiaBan
                FROM ChiTietHoaDon ct
                INNER JOIN MonAn m ON ct.MaMon = m.MaMon
                WHERE ct.MaHD = @MaHD";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaHD", maHD);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new ChiTietHoaDonDTO
                {
                    MaHD = reader.GetInt32(reader.GetOrdinal("MaHD")),
                    MaMon = reader.GetInt32(reader.GetOrdinal("MaMon")),
                    TenMon = reader.IsDBNull(reader.GetOrdinal("TenMon")) ? string.Empty : reader.GetString(reader.GetOrdinal("TenMon")),
                    SoLuong = reader.GetInt32(reader.GetOrdinal("SoLuong")),
                    DonGiaBan = reader.GetDecimal(reader.GetOrdinal("DonGiaBan"))
                });
            }
            return list;
        }

        public bool ThanhToanHoaDon(int maHD)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("UPDATE HoaDon SET TrangThai = N'Đã thanh toán' WHERE MaHD = @MaHD", conn);
            cmd.Parameters.AddWithValue("@MaHD", maHD);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
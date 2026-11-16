using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using QuanLyQuanAn.DTO;

namespace QuanLyQuanAn.DAL
{
    public class MenuInventoryDAL
    {
        // ==================== 1. DANH MỤC MÓN ====================
        public List<DanhMucMonDTO> GetAllDanhMuc()
        {
            var list = new List<DanhMucMonDTO>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            string sql = "SELECT MaDanhMuc, TenDanhMuc, ISNULL(MoTa, '') AS MoTa FROM DanhMucMon ORDER BY MaDanhMuc";
            using var cmd = new SqlCommand(sql, conn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new DanhMucMonDTO
                {
                    MaDanhMuc = rd.GetInt32(0),
                    TenDanhMuc = rd.GetString(1),
                    MoTa = rd.GetString(2)
                });
            }
            return list;
        }

        public bool InsertDanhMuc(DanhMucMonDTO dm)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            string sql = "INSERT INTO DanhMucMon (TenDanhMuc, MoTa) VALUES (@Ten, @MoTa)";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Ten", dm.TenDanhMuc);
            cmd.Parameters.AddWithValue("@MoTa", dm.MoTa);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool UpdateDanhMuc(DanhMucMonDTO dm)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            string sql = "UPDATE DanhMucMon SET TenDanhMuc = @Ten, MoTa = @MoTa WHERE MaDanhMuc = @Ma";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Ma", dm.MaDanhMuc);
            cmd.Parameters.AddWithValue("@Ten", dm.TenDanhMuc);
            cmd.Parameters.AddWithValue("@MoTa", dm.MoTa);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DeleteDanhMuc(int maDanhMuc)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            string sql = "DELETE FROM DanhMucMon WHERE MaDanhMuc = @Ma";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Ma", maDanhMuc);
            return cmd.ExecuteNonQuery() > 0;
        }

        // ==================== 2. MÓN ĂN (THỰC ĐƠN) ====================
        public List<MonAnDTO> GetMonAn(int maDanhMuc = 0, string tuKhoa = "")
        {
            var list = new List<MonAnDTO>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            string sql = @"
                SELECT m.MaMon, m.TenMon, m.MaDanhMuc, d.TenDanhMuc, m.DonGia, m.DonViTinh, m.TrangThai
                FROM MonAn m
                INNER JOIN DanhMucMon d ON m.MaDanhMuc = d.MaDanhMuc
                WHERE (@MaDM = 0 OR m.MaDanhMuc = @MaDM)
                  AND (@TuKhoa = N'' OR m.TenMon LIKE N'%' + @TuKhoa + N'%')
                ORDER BY m.MaMon DESC";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaDM", maDanhMuc);
            cmd.Parameters.AddWithValue("@TuKhoa", tuKhoa.Trim());
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new MonAnDTO
                {
                    MaMon = rd.GetInt32(0),
                    TenMon = rd.GetString(1),
                    MaDanhMuc = rd.GetInt32(2),
                    TenDanhMuc = rd.GetString(3),
                    DonGia = rd.GetDecimal(4),
                    DonViTinh = rd.GetString(5),
                    TrangThai = rd.GetString(6)
                });
            }
            return list;
        }

        public bool InsertMonAn(MonAnDTO m)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            string sql = @"INSERT INTO MonAn (TenMon, MaDanhMuc, DonGia, DonViTinh, TrangThai)
                           VALUES (@Ten, @MaDM, @Gia, @DVT, @TT)";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Ten", m.TenMon);
            cmd.Parameters.AddWithValue("@MaDM", m.MaDanhMuc);
            cmd.Parameters.AddWithValue("@Gia", m.DonGia);
            cmd.Parameters.AddWithValue("@DVT", m.DonViTinh);
            cmd.Parameters.AddWithValue("@TT", m.TrangThai);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool UpdateMonAn(MonAnDTO m)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            string sql = @"UPDATE MonAn
                           SET TenMon = @Ten, MaDanhMuc = @MaDM, DonGia = @Gia, DonViTinh = @DVT, TrangThai = @TT
                           WHERE MaMon = @Ma";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Ma", m.MaMon);
            cmd.Parameters.AddWithValue("@Ten", m.TenMon);
            cmd.Parameters.AddWithValue("@MaDM", m.MaDanhMuc);
            cmd.Parameters.AddWithValue("@Gia", m.DonGia);
            cmd.Parameters.AddWithValue("@DVT", m.DonViTinh);
            cmd.Parameters.AddWithValue("@TT", m.TrangThai);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DeleteMonAn(int maMon)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            string sql = "DELETE FROM MonAn WHERE MaMon = @Ma";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Ma", maMon);
            return cmd.ExecuteNonQuery() > 0;
        }

        // ==================== 3. KHO NGUYÊN LIỆU & PHIẾU NHẬP ====================
        public List<NguyenLieuDTO> GetAllNguyenLieu(bool chiLaySapHet = false)
        {
            var list = new List<NguyenLieuDTO>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            string sql = "SELECT MaNL, TenNL, DonViTinh, SoLuongTon, MucCanhBao FROM NguyenLieu";
            if (chiLaySapHet)
                sql += " WHERE SoLuongTon <= MucCanhBao";
            sql += " ORDER BY MaNL";

            using var cmd = new SqlCommand(sql, conn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new NguyenLieuDTO
                {
                    MaNL = rd.GetInt32(0),
                    TenNL = rd.GetString(1),
                    DonViTinh = rd.GetString(2),
                    SoLuongTon = rd.GetDouble(3),
                    MucCanhBao = rd.GetDouble(4)
                });
            }
            return list;
        }

        public bool InsertNguyenLieu(NguyenLieuDTO nl)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            string sql = @"INSERT INTO NguyenLieu (TenNL, DonViTinh, SoLuongTon, MucCanhBao)
                           VALUES (@Ten, @DVT, @Ton, @CanhBao)";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Ten", nl.TenNL);
            cmd.Parameters.AddWithValue("@DVT", nl.DonViTinh);
            cmd.Parameters.AddWithValue("@Ton", nl.SoLuongTon);
            cmd.Parameters.AddWithValue("@CanhBao", nl.MucCanhBao);
            return cmd.ExecuteNonQuery() > 0;
        }

        public DataTable GetLichSuPhieuNhap()
        {
            using var conn = DatabaseHelper.GetConnection();
            string sql = @"
                SELECT p.MaPhieuNhap AS [Mã Phiếu],
                       p.NgayNhap AS [Ngày Nhập],
                       p.NhaCungCap AS [Nhà Cung Cấp],
                       nv.HoTen AS [Nhân Viên],
                       p.TongTienNhap AS [Tổng Tiền],
                       p.GhiChu AS [Ghi Chú]
                FROM PhieuNhapKho p
                INNER JOIN NhanVien nv ON p.MaNV = nv.MaNV
                ORDER BY p.MaPhieuNhap DESC";
            using var da = new SqlDataAdapter(sql, conn);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        // Lưu Phiếu Nhập Kho + Chi Tiết + Cộng Tồn Kho bằng SqlTransaction
        public bool LuuPhieuNhapKho(PhieuNhapKhoDTO phieu)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                string sqlPhieu = @"
                    INSERT INTO PhieuNhapKho (NgayNhap, NhaCungCap, MaNV, TongTienNhap, GhiChu)
                    OUTPUT INSERTED.MaPhieuNhap
                    VALUES (GETDATE(), @NCC, @MaNV, @TongTien, @GhiChu)";
                using var cmdPhieu = new SqlCommand(sqlPhieu, conn, tran);
                cmdPhieu.Parameters.AddWithValue("@NCC", phieu.NhaCungCap);
                cmdPhieu.Parameters.AddWithValue("@MaNV", phieu.MaNV);
                cmdPhieu.Parameters.AddWithValue("@TongTien", phieu.TongTienNhap);
                cmdPhieu.Parameters.AddWithValue("@GhiChu", phieu.GhiChu);

                int maPhieuMoi = (int)cmdPhieu.ExecuteScalar();

                foreach (var ct in phieu.ChiTiet)
                {
                    if (ct.SoLuongNhap <= 0 || ct.DonGiaNhap < 0)
                        throw new Exception($"Số lượng hoặc đơn giá nhập của '{ct.TenNL}' không hợp lệ!");

                    string sqlCT = @"
                        INSERT INTO ChiTietNhapKho (MaPhieuNhap, MaNL, SoLuongNhap, DonGiaNhap)
                        VALUES (@MaPN, @MaNL, @SL, @Gia)";
                    using var cmdCT = new SqlCommand(sqlCT, conn, tran);
                    cmdCT.Parameters.AddWithValue("@MaPN", maPhieuMoi);
                    cmdCT.Parameters.AddWithValue("@MaNL", ct.MaNL);
                    cmdCT.Parameters.AddWithValue("@SL", ct.SoLuongNhap);
                    cmdCT.Parameters.AddWithValue("@Gia", ct.DonGiaNhap);
                    cmdCT.ExecuteNonQuery();

                    string sqlTon = "UPDATE NguyenLieu SET SoLuongTon = SoLuongTon + @SL WHERE MaNL = @MaNL";
                    using var cmdTon = new SqlCommand(sqlTon, conn, tran);
                    cmdTon.Parameters.AddWithValue("@SL", ct.SoLuongNhap);
                    cmdTon.Parameters.AddWithValue("@MaNL", ct.MaNL);
                    cmdTon.ExecuteNonQuery();
                }

                tran.Commit();
                return true;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }
    }
}
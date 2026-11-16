using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using QuanLyQuanAn.DAL;
using QuanLyQuanAn.DTO;

namespace QuanLyQuanAn.BUS
{
    public class MenuInventoryBUS
    {
        private readonly MenuInventoryDAL _dal = new();

        // Danh mục món
        public List<DanhMucMonDTO> LayDanhSachDanhMuc() => _dal.GetAllDanhMuc();

        public void ThemDanhMuc(DanhMucMonDTO dm)
        {
            if (string.IsNullOrWhiteSpace(dm.TenDanhMuc))
                throw new Exception("Tên danh mục món không được để trống!");
            _dal.InsertDanhMuc(dm);
        }

        public void SuaDanhMuc(DanhMucMonDTO dm)
        {
            if (dm.MaDanhMuc <= 0) throw new Exception("Vui lòng chọn danh mục cần sửa!");
            if (string.IsNullOrWhiteSpace(dm.TenDanhMuc))
                throw new Exception("Tên danh mục món không được để trống!");
            _dal.UpdateDanhMuc(dm);
        }

        public void XoaDanhMuc(int maDM)
        {
            if (maDM <= 0) throw new Exception("Vui lòng chọn danh mục cần xóa!");
            _dal.DeleteDanhMuc(maDM);
        }

        // Món ăn
        public List<MonAnDTO> LayDanhSachMonAn(int maDM = 0, string tuKhoa = "") => _dal.GetMonAn(maDM, tuKhoa);

        public void ThemMonAn(MonAnDTO m)
        {
            if (string.IsNullOrWhiteSpace(m.TenMon)) throw new Exception("Tên món ăn không được để trống!");
            if (m.DonGia < 0) throw new Exception("Đơn giá bán phải >= 0!");
            if (string.IsNullOrWhiteSpace(m.DonViTinh)) throw new Exception("Đơn vị tính không được để trống!");
            _dal.InsertMonAn(m);
        }

        public void SuaMonAn(MonAnDTO m)
        {
            if (m.MaMon <= 0) throw new Exception("Vui lòng chọn món ăn cần cập nhật!");
            if (string.IsNullOrWhiteSpace(m.TenMon)) throw new Exception("Tên món ăn không được để trống!");
            if (m.DonGia < 0) throw new Exception("Đơn giá bán phải >= 0!");
            _dal.UpdateMonAn(m);
        }

        public void XoaMonAn(int maMon)
        {
            if (maMon <= 0) throw new Exception("Vui lòng chọn món ăn cần xóa!");
            _dal.DeleteMonAn(maMon);
        }

        // Kho & Nhập kho
        public List<NguyenLieuDTO> LayDanhSachNguyenLieu(bool chiLaySapHet = false) => _dal.GetAllNguyenLieu(chiLaySapHet);

        public void ThemNguyenLieu(NguyenLieuDTO nl)
        {
            if (string.IsNullOrWhiteSpace(nl.TenNL)) throw new Exception("Tên nguyên liệu không được để trống!");
            if (string.IsNullOrWhiteSpace(nl.DonViTinh)) throw new Exception("Đơn vị tính không được để trống!");
            if (nl.SoLuongTon < 0 || nl.MucCanhBao < 0) throw new Exception("Số lượng tồn và mức cảnh báo phải >= 0!");
            _dal.InsertNguyenLieu(nl);
        }

        public DataTable LayLichSuPhieuNhap() => _dal.GetLichSuPhieuNhap();

        public void NhapKho(PhieuNhapKhoDTO phieu)
        {
            if (string.IsNullOrWhiteSpace(phieu.NhaCungCap))
                throw new Exception("Vui lòng nhập tên Nhà cung cấp!");
            if (phieu.ChiTiet == null || phieu.ChiTiet.Count == 0)
                throw new Exception("Phiếu nhập kho phải có ít nhất 1 nguyên liệu!");

            phieu.TongTienNhap = phieu.ChiTiet.Sum(x => x.ThanhTien);
            _dal.LuuPhieuNhapKho(phieu);
        }
    }
}

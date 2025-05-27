using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WH.DataContext;
using System.Data;
using Npgsql;

namespace WH.Controllers
{
    public class DashboardAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        [Route("api/DashboardAPI/TongGiaTriNhapTheoNgay")]
        public IHttpActionResult TongGiaTriNhapTheoNgay()
        {
            try
            {
                var query = @"
                    SELECT TO_CHAR(pn.ngay_ct, 'YYYY-MM-DD') AS ngay,
                           SUM(ct.so_luong_nhap * ct.don_gia_nhap) AS tong_gia_tri
                    FROM phieu_nhap pn
                    JOIN ct_phieu_nhap ct ON pn.so_ct = ct.so_ct AND pn.ma_ncc = ct.ma_ncc
                    GROUP BY TO_CHAR(pn.ngay_ct, 'YYYY-MM-DD')
                    ORDER BY ngay;
                ";

                var result = db.Database.SqlQuery<DashboardNhapTheoNgayVM>(query).ToList();

                return Ok(new { success = true, result });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/DashboardAPI/TongGiaTriXuatTheoNgay")]
        public IHttpActionResult TongGiaTriXuatTheoNgay()
        {
            try
            {
                var query = @"
            SELECT TO_CHAR(px.ngay_ct, 'YYYY-MM-DD') AS ngay,
                   SUM(ct.so_luong_xuat * ct.don_gia_xuat) AS tong_gia_tri
            FROM phieu_xuat px
            JOIN ct_phieu_xuat ct ON px.so_ct = ct.so_ct
            GROUP BY TO_CHAR(px.ngay_ct, 'YYYY-MM-DD')
            ORDER BY ngay;
        ";

                var result = db.Database.SqlQuery<DashboardNhapTheoNgayVM>(query).ToList();
                return Ok(new { success = true, result });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/DashboardAPI/TonKhoTheoKho")]
        public IHttpActionResult TonKhoTheoKho()
        {
            try
            {
                var query = @"
            SELECT tk.ma_kho,
                   dk.ten_kho,
                   SUM(tk.so_luong_ton) AS tong_so_luong
            FROM tonkho tk
            JOIN dmkho dk ON tk.ma_kho = dk.ma_kho
            GROUP BY tk.ma_kho, dk.ten_kho
            ORDER BY tong_so_luong DESC;
        ";

                var result = db.Database.SqlQuery<TonKhoTheoKhoVM>(query).ToList();
                return Ok(new { success = true, result });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        [Route("api/DashboardAPI/VatTuSapHetHang")]
        public IHttpActionResult VatTuSapHetHang()
        {
            try
            {
                var query = @"
            SELECT vt.ma_vt,
                   vt.ten_vt,
                   SUM(tk.so_luong_ton) AS tong_so_luong
            FROM tonkho tk
            JOIN dmvt vt ON tk.ma_vt = vt.ma_vt
            GROUP BY vt.ma_vt, vt.ten_vt
            HAVING SUM(tk.so_luong_ton) <= 10
            ORDER BY tong_so_luong ASC;
        ";

                var result = db.Database.SqlQuery<VatTuHetHangVM>(query).ToList();
                return Ok(new { success = true, result });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/DashboardAPI/Top5NCCNhapCaoNhat")]
        public IHttpActionResult Top5NCCNhapCaoNhat()
        {
            try
            {
                var query = @"
            SELECT ncc.ma_ncc,
                   ncc.ten_ncc,
                   SUM(ct.so_luong_nhap * ct.don_gia_nhap) AS tong_gia_tri
            FROM phieu_nhap pn
            JOIN ct_phieu_nhap ct ON pn.so_ct = ct.so_ct AND pn.ma_ncc = ct.ma_ncc
            JOIN dmncc ncc ON pn.ma_ncc = ncc.ma_ncc
            GROUP BY ncc.ma_ncc, ncc.ten_ncc
            ORDER BY tong_gia_tri DESC
            LIMIT 5;
        ";

                var result = db.Database.SqlQuery<TopNCCVM>(query).ToList();
                return Ok(new { success = true, result });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/DashboardAPI/TyTrongNhapTheoLoaiVT")]
        public IHttpActionResult TyTrongNhapTheoLoaiVT()
        {
            try
            {
                var query = @"
            SELECT lvt.ten_loai_vt,
                   SUM(ct.so_luong_nhap * ct.don_gia_nhap) AS tong_gia_tri
            FROM phieu_nhap pn
            JOIN ct_phieu_nhap ct ON pn.so_ct = ct.so_ct
            JOIN dmvt vt ON ct.ma_vt = vt.ma_vt
            JOIN loai_vat_tu lvt ON vt.ma_loai_vt = lvt.ma_loai_vt
            GROUP BY lvt.ten_loai_vt
            ORDER BY tong_gia_tri DESC;
        ";

                var result = db.Database.SqlQuery<TyTrongLoaiVM>(query).ToList();
                return Ok(new { success = true, result });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

  


    }
    public class TonKhoTheoKhoVM
    {
        public string ma_kho { get; set; }
        public string ten_kho { get; set; }
        public decimal tong_so_luong { get; set; }
    }
    public class DashboardNhapTheoNgayVM
    {
        public string ngay { get; set; }
        public decimal tong_gia_tri { get; set; }
    }
    public class VatTuHetHangVM
    {
        public string ma_vt { get; set; }
        public string ten_vt { get; set; }
        public decimal tong_so_luong { get; set; }
    }

    public class TopNCCVM
    {
        public string ma_ncc { get; set; }
        public string ten_ncc { get; set; }
        public decimal tong_gia_tri { get; set; }
    }
    public class TyTrongLoaiVM
    {
        public string ten_loai_vt { get; set; }
        public decimal tong_gia_tri { get; set; }
    }
}

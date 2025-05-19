using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using WH.DataContext;
using WH.Models;
using ApplicationDbContext = WH.DataContext.ApplicationDbContext;

namespace WH.Controllers
{
    public class PhieuXuatAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET all PhieuXuat
        public IHttpActionResult Get()
        {
            var query = @"
                SELECT 
                    p.so_ct, p.ngay_ct, p.dien_giai, p.ma_kh, p.trang_thai, 
    p.bien_so_xe, p.ngay_van_chuyen, p.tt_thanhtoan, 
    p.chietkhau, p.ma_so_thue, p.thue, 
    p.tong_thanh_toan, p.tong_chiet_khau, p.tong_thue,
    p.nguoi_giao, p.dia_diem_giao,
    b.ten_kh
                FROM phieu_xuat p left join dmkh b on p.ma_kh = b.ma_kh
                ORDER BY p.so_ct DESC";

            var phieuXuatList = db.Database.SqlQuery<PhieuXuatViewModel>(query).ToList();
            return Ok(phieuXuatList);
        }

        // GET ChiTiet PhieuXuat by so_ct
        [HttpGet]
        [Route("api/PhieuXuatAPI/GET_CT")]
        public IHttpActionResult GET_CT(string so_ct)
        {
            var query = @"
        SELECT 
            a.ma_vt, 
            a.ma_kho, 
            a.so_ct, 
            a.ma_kh, 
            a.ma_dvt, 
            a.so_luong_xuat, 
            a.don_gia_xuat, 
            a.ghi_chu,
             COALESCE(b.so_luong_ton, 0) AS ton
        FROM ct_phieu_xuat a
        LEFT JOIN tonkho b 
            ON a.ma_kho = b.ma_kho AND a.ma_vt = b.ma_vt AND a.ma_dvt = b.ma_dvt
        WHERE a.so_ct = @p0";

            var ctPhieuXuatList = db.Database.SqlQuery<ChiTietPhieuXuatViewClass>(query, so_ct).ToList();
            return Ok(ctPhieuXuatList);
        }

        // SEARCH with filters
        [HttpPost]
        [Route("api/PhieuXuatAPI/SEARCH")]
        public IHttpActionResult SEARCH(JObject data)
        {
            var keyword = (string)data["keyword"];
            var so_ct = (string)data["so_ct"];
            var ma_kh = (string)data["ma_kh"];
            var tu_ngay = data["tu_ngay"]?.ToObject<DateTime?>()?.Date;
            var den_ngay = data["den_ngay"]?.ToObject<DateTime?>()?.Date;

            var query = @"
                SELECT 
                       p.so_ct, p.ngay_ct, p.dien_giai, p.ma_kh, p.trang_thai, 
    p.bien_so_xe, p.ngay_van_chuyen, p.tt_thanhtoan, 
    p.chietkhau, p.ma_so_thue, p.thue, 
    p.tong_thanh_toan, p.tong_chiet_khau, p.tong_thue,
    p.nguoi_giao, p.dia_diem_giao,
    b.ten_kh
                FROM phieu_xuat p left join dmkh b on p.ma_kh = b.ma_kh
           
                WHERE 1=1";

            if (!string.IsNullOrEmpty(keyword))
                query += $" AND (p.so_ct ILIKE '%{keyword}%' OR p.dien_giai ILIKE '%{keyword}%' OR p.ma_kh ILIKE '%{keyword}%' OR p.ngay_ct::text ILIKE '%{keyword}%')";

            if (!string.IsNullOrEmpty(so_ct))
                query += $" AND p.so_ct ILIKE '%{so_ct}%'";

            if (!string.IsNullOrEmpty(ma_kh))
                query += $" AND p.ma_kh ILIKE '%{ma_kh}%'";

            if (tu_ngay.HasValue)
                query += $" AND CAST(p.ngay_ct AS date) >= '{tu_ngay.Value:yyyy-MM-dd}'";

            if (den_ngay.HasValue)
                query += $" AND CAST(p.ngay_ct AS date) <= '{den_ngay.Value:yyyy-MM-dd}'";

            query += " ORDER BY p.so_ct DESC";
            var result = db.Database.SqlQuery<PhieuXuatViewModel>(query).ToList();

            return Ok(new { success = true, result });
        }

        // DELETE selected PhieuXuat
        [HttpPost]
        [Route("api/PhieuXuatAPI/DeleteAll")]
        public IHttpActionResult DeleteAll([FromBody] List<PhieuXuatClass> list)
        {
            if (list == null || !list.Any())
                return BadRequest("Danh sách rỗng");

            foreach (var item in list)
            {
                var phieu = db.PhieuXuatObj.FirstOrDefault(p => p.so_ct == item.so_ct);
                if (phieu != null)
                {
                    db.PhieuXuatObj.Remove(phieu);
                }
            }
            db.SaveChanges();
            return Ok("Đã xóa các phiếu được chọn");
        }

        // POST Save All ChiTiet PhieuXuat
        [HttpPost]
        [Route("api/PhieuXuatAPI/SaveAll")]
        public IHttpActionResult SaveAll(List<ChiTietPhieuXuatClass> list)
        {
            try
            {
                if (list == null || !list.Any())
                    return BadRequest("Danh sách trống");

                var so_ct = list.First().so_ct;

                // Xóa các bản ghi cũ theo so_ct
                var oldItems = db.ChiTietPhieuXuatObj.Where(x => x.so_ct == so_ct).ToList();
                db.ChiTietPhieuXuatObj.RemoveRange(oldItems);
                db.SaveChanges();

                // Thêm mới các bản ghi
                foreach (var item in list)
                {
                    db.ChiTietPhieuXuatObj.Add(item);
                }
                db.SaveChanges();

                return Ok(new { success = true, message = "Lưu thành công" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        // POST Save new PhieuXuat
        [HttpPost]
        [Route("api/PhieuXuatAPI/SaveAdd")]
        public IHttpActionResult SaveAdd(JObject data)
        {
            try
            {
                var phieu = data.ToObject<PhieuXuatClass>();
                if (phieu == null || string.IsNullOrEmpty(phieu.so_ct))
                    return BadRequest("Dữ liệu không hợp lệ");

                if (db.PhieuXuatObj.Any(x => x.so_ct == phieu.so_ct))
                {
                    return Ok(new { success = false, message = "Số chứng từ đã tồn tại." });
                }

                db.PhieuXuatObj.Add(phieu);
                db.SaveChanges();

                return Ok(new { success = true, result = phieu });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        // PUT update existing PhieuXuat
        [HttpPut]
        [Route("api/PhieuXuatAPI/SaveEdit")]
        public IHttpActionResult SaveEdit(JObject data)
        {
            try
            {
                var phieu = data.ToObject<PhieuXuatClass>();
                if (phieu == null || string.IsNullOrEmpty(phieu.so_ct))
                    return BadRequest("Dữ liệu không hợp lệ");

                var existing = db.PhieuXuatObj.FirstOrDefault(x => x.so_ct == phieu.so_ct);
                if (existing == null)
                {
                    return Ok(new { success = false, message = "Phiếu xuất không tồn tại." });
                }

                existing.ngay_ct = phieu.ngay_ct;
                existing.dien_giai = phieu.dien_giai;
                existing.trang_thai = phieu.trang_thai;
                existing.bien_so_xe = phieu.bien_so_xe;
                existing.ngay_van_chuyen = phieu.ngay_van_chuyen;
                existing.tt_thanhtoan = phieu.tt_thanhtoan;
                existing.chietkhau = phieu.chietkhau;
                existing.ma_so_thue = phieu.ma_so_thue;
                existing.thue = phieu.thue;
                existing.tong_thanh_toan = phieu.tong_thanh_toan;
                existing.tong_chiet_khau = phieu.tong_chiet_khau;
                existing.tong_thue = phieu.tong_thue;
                existing.nguoi_giao = phieu.nguoi_giao;
                existing.dia_diem_giao = phieu.dia_diem_giao;

                db.Entry(existing).State = EntityState.Modified;
                db.SaveChanges();

                return Ok(new { success = true, result = existing });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        [Route("api/phieuxuat/tao_soct")]
        public IHttpActionResult TaoSoChungTu()
        {
            // Lấy username từ cookie
            var cookie = HttpContext.Current.Request.Cookies["UserSessionCookie"];

            // Kiểm tra nếu cookie tồn tại và lấy giá trị UserName từ cookie
            var username = cookie?.Value ?? "000000";  // Nếu cookie không tồn tại, dùng giá trị mặc định "000000"

            // Đảm bảo chuỗi số có đủ 6 ký tự, thêm '0' ở đầu nếu cần
            string paddedUsername = username.PadLeft(6, '0');

            // Lấy thời gian hiện tại theo định dạng ddMMyyHHmmss
            string timestamp = DateTime.Now.ToString("ddMMyyHHmmss");

            // Tạo số chứng từ theo định dạng PX+user+timestamp
            string so_ct = "PX" + paddedUsername + "-" + timestamp;

            // Trả về kết quả JSON
            return Ok(new { so_ct });
        }
        [HttpGet]
        [Route("api/PhieuXuatAPI/GetTonTheoKho")]
        public IHttpActionResult GetTonTheoKho(string ma_vt, string ma_kho, string ma_dvt)
        {
            try
            {
                if (string.IsNullOrEmpty(ma_vt) || string.IsNullOrEmpty(ma_kho) || string.IsNullOrEmpty(ma_dvt))
                {
                    return Ok(0); // Trả về 0 nếu thiếu dữ liệu
                }

                var tonKho = db.Database.SqlQuery<decimal?>(@"
            SELECT so_luong_ton 
            FROM tonkho 
            WHERE ma_vt = @p0 AND ma_kho = @p1 AND ma_dvt = @p2
            LIMIT 1
        ", ma_vt, ma_kho, ma_dvt).FirstOrDefault();

                return Ok(tonKho ?? 0); // Nếu null thì trả về 0
            }
            catch (Exception ex)
            {
                return Ok(0); // Trong trường hợp lỗi cũng trả về 0
            }
        }


    }
}

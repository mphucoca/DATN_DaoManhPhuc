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
    public class PhieuNhapAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
  
        public IHttpActionResult Get()
        {
            var query = @"
        SELECT 
              p.so_ct, 
    p.ma_ncc, 
    p.ngay_ct, 
    p.dien_giai, 
    p.trang_thai, 
    p.bien_so_xe,
    p.ngay_van_chuyen,
    p.tt_thanhtoan,
    p.chietkhau,
    p.ma_so_thue,p.tong_thanh_toan,
p.tong_chiet_khau,
p.tong_thue,
    p.thue,
    n.ten_ncc as ten_ncc
        FROM phieu_nhap p
        LEFT JOIN dmncc n ON p.ma_ncc = n.ma_ncc order by p.so_ct desc;";
            // Sử dụng View Model chỉ để hiển thị dũ liệu
            var phieuNhapList = db.Database.SqlQuery<PhieuNhapViewModel>(query).ToList();
            return Ok(phieuNhapList);
        }
        [HttpGet]
        [Route("api/PhieuNhapAPI/GET_CT")]
        public IHttpActionResult GET_CT(string so_ct)
        {
            var query = @"
        SELECT * 
        FROM ct_phieu_nhap 
        WHERE so_ct = @p0";

            // Sử dụng View Model chỉ để hiển thị dữ liệu
            var ctphieuNhapList = db.Database.SqlQuery<ChiTietPhieuNhapClass>(query, so_ct).ToList();

            return Ok(ctphieuNhapList);
        }

        [HttpPost]
        [Route("api/PhieuNhapAPI/SEARCH")]
        public IHttpActionResult SEARCH(JObject data)
        {
            var keyword = (string)data["keyword"];
            var so_ct = (string)data["so_ct"];
            var ma_ncc = (string)data["ma_ncc"];
            var tu_ngay = data["tu_ngay"]?.ToObject<DateTime?>()?.Date;
            var den_ngay = data["den_ngay"]?.ToObject<DateTime?>()?.Date;


            var query = @"
        SELECT 
                p.so_ct, 
    p.ma_ncc, 
    p.ngay_ct, 
    p.dien_giai, 
    p.trang_thai, 
    p.bien_so_xe,
    p.ngay_van_chuyen,
    p.tt_thanhtoan,
    p.chietkhau,
    p.ma_so_thue,
    p.thue,p.tong_thanh_toan,
p.tong_chiet_khau,
p.tong_thue,
    n.ten_ncc as ten_ncc
        FROM phieu_nhap p
        LEFT JOIN dmncc n ON p.ma_ncc = n.ma_ncc
        WHERE 1=1";

            if (!string.IsNullOrEmpty(keyword))
                query += $" AND (p.so_ct ILIKE '%{keyword}%' OR p.dien_giai ILIKE '%{keyword}%'OR n.ten_ncc ILIKE '%{keyword}%'OR p.ma_ncc ILIKE '%{keyword}%'OR p.ngay_ct::text ILIKE '%{keyword}%')";

            if (!string.IsNullOrEmpty(so_ct))
                query += $" AND p.so_ct ILIKE '%{so_ct}%'";

            if (!string.IsNullOrEmpty(ma_ncc))
                query += $" AND p.ma_ncc ILIKE '%{ma_ncc}%'";

            if (tu_ngay.HasValue)
                query += $" AND CAST(p.ngay_ct AS date) >= '{tu_ngay.Value:yyyy-MM-dd}'";

            if (den_ngay.HasValue)
                query += $" AND CAST(p.ngay_ct AS date) <= '{den_ngay.Value:yyyy-MM-dd}'";
            query += $"order by p.so_ct desc";
            var result = db.Database.SqlQuery<PhieuNhapViewModel>(query).ToList();

            return Ok(new { success = true, result });
        }

        [HttpPost]
        [Route("api/PhieuNhapAPI/DeleteAll")]
        public IHttpActionResult DeleteAll([FromBody] List<PhieuNhapClass> list)
        {
            if (list == null || !list.Any())
                return BadRequest("Danh sách rỗng");

            foreach (var item in list)
            {
                var phieu = db.PhieuNhapObj.FirstOrDefault(p => p.so_ct == item.so_ct.ToString());
                if (phieu != null)
                {
                    db.PhieuNhapObj.Remove(phieu);
                }
            } 
            db.SaveChanges();
            return Ok("Đã xóa các phiếu được chọn");
        }
        [HttpGet]
        [Route("api/PhieuNhapAPI/GetDVTByMaVt")]
        public IHttpActionResult GetDVTByMaVt(string ma_vt)
        {
            var query = @"
    SELECT 
        qd.ma_vt, 
        qd.ma_dvt, 
        dvt.ten_dvt, 
        dvt.mo_ta,
        dvt.trangthai,
        qd.ty_le_quy_doi
    FROM dmqddvt qd
    LEFT JOIN dmdvt dvt ON qd.ma_dvt = dvt.ma_dvt
    WHERE qd.ma_vt = @p0

    UNION

    SELECT 
        dmvt.ma_vt, 
        dmvt.ma_dvt, 
        dvt.ten_dvt, 
        dvt.mo_ta,
        dvt.trangthai,
        1 AS ty_le_quy_doi
    FROM dmvt
    LEFT JOIN dmdvt dvt ON dmvt.ma_dvt = dvt.ma_dvt
    WHERE dmvt.ma_vt = @p0
      AND NOT EXISTS (
          SELECT 1 
          FROM dmqddvt 
          WHERE dmqddvt.ma_vt = dmvt.ma_vt 
            AND dmqddvt.ma_dvt = dmvt.ma_dvt
      );";

            var result = db.Database.SqlQuery<DmdvtQddvtViewModel>(query, ma_vt).ToList();
            return Ok(result);
        }

        [HttpPost]
        [Route("api/PhieuNhapAPI/SaveAll")]
        public IHttpActionResult SaveAll(List<ChiTietPhieuNhapClass> list)
        {
            try
            {
                if (list == null || !list.Any())
                    return BadRequest("Danh sách trống");

                var so_ct = list.First().so_ct;  // Lấy so_ct từ bản ghi đầu tiên

                // Bước 1: Xóa hết các bản ghi cũ trong bảng ct_phieu_nhap theo so_ct
                var oldItems = db.ChiTietPhieuNhapObj.Where(x => x.so_ct == so_ct).ToList();
                db.ChiTietPhieuNhapObj.RemoveRange(oldItems);
                db.SaveChanges();  // Lưu thay đổi sau khi xóa

                // Bước 2: Thêm mới các bản ghi gửi lên
                foreach (var item in list)
                {
                    db.ChiTietPhieuNhapObj.Add(item);  // Thêm từng bản ghi vào bảng
                    db.SaveChanges();
                }

                  // Lưu thay đổi sau khi thêm mới

                return Ok(new { success = true, message = "Lưu thành công" });
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có
                return Ok(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        [Route("api/PhieuNhapAPI/SaveAdd")]
        public IHttpActionResult SaveAdd(JObject data)
        {
            try
            {
                var phieu = data.ToObject<PhieuNhapClass>();
                if (phieu == null || string.IsNullOrEmpty(phieu.so_ct))
                    return BadRequest("Dữ liệu không hợp lệ");

                if (db.PhieuNhapObj.Any(x => x.so_ct == phieu.so_ct))
                {
                    return Ok(new { success = false, message = "Số chứng từ đã tồn tại." });
                }

                db.PhieuNhapObj.Add(phieu);
                db.SaveChanges();

                return Ok(new { success = true, result = phieu });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
        [HttpPut]
        [Route("api/PhieuNhapAPI/SaveEdit")]
        public IHttpActionResult SaveEdit(JObject data)
        {
            try
            {
                var phieu = data.ToObject<PhieuNhapClass>();
                if (phieu == null || string.IsNullOrEmpty(phieu.so_ct))
                    return BadRequest("Dữ liệu không hợp lệ");

                var existing = db.PhieuNhapObj.FirstOrDefault(x => x.so_ct == phieu.so_ct);
                if (existing == null)
                {
                    return Ok(new { success = false, message = "Phiếu nhập không tồn tại." });
                }

                existing.ma_ncc = phieu.ma_ncc;
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
        [Route("api/phieunhap/tao_soct")]
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
            string so_ct = "PN" + paddedUsername + "-" + timestamp;

            // Trả về kết quả JSON
            return Ok(new { so_ct });
        }



    }
}

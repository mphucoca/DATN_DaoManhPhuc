using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http;
using WH.DataContext;
using WH.Models;
using ApplicationDbContext = WH.DataContext.ApplicationDbContext;

namespace WH.Controllers
{
    public class DmkhoAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public IHttpActionResult Get()
        {
            var query = @"SELECT * FROM dmkho ORDER BY ma_kho ASC;";
            var dmkhoList = db.Database.SqlQuery<DmkhoClass>(query).ToList();
            return Ok(dmkhoList);
        }
        // API lấy ra tồn kho đối với mỗi kho
        [HttpGet]
        [Route("api/DmkhoAPI/GetTonTheoKho")]
        public IHttpActionResult GetTonTheoKho(string ma_kho)
        {
            var query = @"
        SELECT 
            tk.ma_vt, 
            tk.ma_kho, 
            tk.ma_dvt, 
            dvt.ten_dvt, 
            dvt.mo_ta, 
            dvt.trangthai, 
            tk.so_luong_ton, 
            tk.ngay_cap_nhat
        FROM tonkho tk
        INNER JOIN dmdvt dvt ON tk.ma_dvt = dvt.ma_dvt
        WHERE tk.ma_kho = @p0
        ORDER BY tk.ma_vt ASC, tk.ma_dvt ASC;
    ";

            var result = db.Database.SqlQuery<TonKhoViewModel>(query, ma_kho).ToList();

            return Ok(result);
        }
        // lấy ra danh mục đơn vị tính dựa trên từng mã vật tư

        [HttpGet]
        [Route("api/DmdvtAPI/GetDVTByMaVT")]
        public IHttpActionResult GetDVTByMaVT(string ma_vt)
        {
            var query = @"
        SELECT 
            a.ma_dvt,
            b.ten_dvt,
            b.mo_ta,
            b.trangthai
        FROM (
            SELECT ma_dvt 
            FROM dmvt 
            WHERE ma_vt = @p0
            UNION ALL
            SELECT ma_dvt 
            FROM dmqddvt 
            WHERE ma_vt = @p0
        ) a 
        LEFT JOIN dmdvt b ON a.ma_dvt = b.ma_dvt where b.trangthai ='1';
    ";

            var result = db.Database.SqlQuery<DmdvtClass>(query, ma_vt).ToList();

            return Ok(result);
        }

        [HttpPost]
        [Route("api/DmkhoAPI/SEARCH")]
        public IHttpActionResult SEARCH(JObject data)
        {
            var keyword = (string)data["keyword"];
            var ma_kho = (string)data["ma_kho"];
            var ten_kho = (string)data["ten_kho"];
            var mo_ta = (string)data["mo_ta"];
            var dia_chi = (string)data["dia_chi"];

            var query = @"SELECT ma_kho, ten_kho, mo_ta, dia_chi FROM dmkho WHERE 1=1";

            if (!string.IsNullOrEmpty(keyword))
                query += $@" AND (
                        ma_kho ILIKE '%{keyword}%' OR 
                        ten_kho ILIKE '%{keyword}%' OR 
                        mo_ta ILIKE '%{keyword}%' OR 
                        dia_chi ILIKE '%{keyword}%')";

            if (!string.IsNullOrEmpty(ma_kho))
                query += $" AND ma_kho ILIKE '%{ma_kho}%'";
            if (!string.IsNullOrEmpty(ten_kho))
                query += $" AND ten_kho ILIKE '%{ten_kho}%'";
            if (!string.IsNullOrEmpty(mo_ta))
                query += $" AND mo_ta ILIKE '%{mo_ta}%'";
            if (!string.IsNullOrEmpty(dia_chi))
                query += $" AND dia_chi ILIKE '%{dia_chi}%'";

            query += " ORDER BY ma_kho ASC";

            var result = db.Database.SqlQuery<DmkhoClass>(query).ToList();
            return Ok(new { success = true, result });
        }

        [HttpPost]
        [Route("api/DmkhoAPI/DeleteAll")]
        public IHttpActionResult DeleteAll([FromBody] List<DmkhoClass> list)
        {
            if (list == null || !list.Any())
                return BadRequest("Danh sách rỗng");

            foreach (var item in list)
            {
                var kho = db.DmkhoObj.FirstOrDefault(p => p.ma_kho == item.ma_kho);
                if (kho != null)
                {
                    db.DmkhoObj.Remove(kho);
                }
            }
            db.SaveChanges();
            return Ok("Đã xóa các kho được chọn");
        }

        [HttpPost]
        [Route("api/DmkhoAPI/SaveAdd")]
        public IHttpActionResult SaveAdd(JObject data)
        {
            try
            {
                var kho = data.ToObject<DmkhoClass>();

                if (kho == null)
                    return BadRequest("Dữ liệu không hợp lệ");

                if (db.DmkhoObj.Any(k => k.ma_kho == kho.ma_kho))
                {
                    return Ok(new { success = false, message = "Mã kho đã tồn tại." });
                }

                db.DmkhoObj.Add(kho);
                db.SaveChanges();

                return Ok(new { success = true, result = kho });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("api/DmkhoAPI/SaveEdit")]
        public IHttpActionResult SaveEdit(JObject data)
        {
            try
            {
                var kho = data.ToObject<DmkhoClass>();

                if (kho == null || string.IsNullOrEmpty(kho.ma_kho))
                    return BadRequest("Dữ liệu không hợp lệ");

                var existingKho = db.DmkhoObj.FirstOrDefault(k => k.ma_kho == kho.ma_kho);
                if (existingKho == null)
                    return NotFound();

                existingKho.ten_kho = kho.ten_kho;
                existingKho.mo_ta = kho.mo_ta;
                existingKho.dia_chi = kho.dia_chi;

                db.Entry(existingKho).State = EntityState.Modified;
                db.SaveChanges();

                return Ok(new { success = true, result = existingKho });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPost]
        [Route("api/TonKhoAPI/SaveTonKho")]
        public IHttpActionResult SaveTonKho(JObject data)
        {
            try
            {
                var ma_kho = (string)data["ma_kho"];
                var ds_ton_kho = data["ds_ton_kho"].ToObject<List<TonKhoClass>>();

                if (string.IsNullOrEmpty(ma_kho) || ds_ton_kho == null)
                    return BadRequest("Dữ liệu không hợp lệ");

                // Xóa dữ liệu cũ theo mã kho
                var tonKhoCu = db.TonKhoObj.Where(tk => tk.ma_kho == ma_kho).ToList();
                db.TonKhoObj.RemoveRange(tonKhoCu);
                db.SaveChanges();

                // Thêm dữ liệu mới
                foreach (var item in ds_ton_kho)
                {
                    item.ma_kho = ma_kho; // đảm bảo mã kho đúng
                    db.TonKhoObj.Add(item);
                }
                db.SaveChanges();

                return Ok(new { success = true, message = "Lưu tồn kho thành công" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Có lỗi xảy ra khi lưu tồn kho: " + ex.Message });
            }
        }
    }
}

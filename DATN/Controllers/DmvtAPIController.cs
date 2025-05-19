using Newtonsoft.Json.Linq;
using Npgsql;
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
    public class DmvtAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public IHttpActionResult Get()
        {
            var query = @"SELECT * FROM dmvt ORDER BY ma_vt ASC;";
            var dmvtList = db.Database.SqlQuery<DmvtClass>(query).ToList();
            return Ok(dmvtList);
        }

        [HttpPost]
        [Route("api/DmvtAPI/SEARCH")]
        public IHttpActionResult SEARCH(JObject data)
        {
            try
            {
                var keyword = (string)data["keyword"];
                var ma_vt = (string)data["ma_vt"];
                var ten_vt = (string)data["ten_vt"];
                var mo_ta = (string)data["mo_ta"];
                var trangthaiStr = (string)data["trangthai"];

                var query = @"SELECT * FROM dmvt WHERE 1=1";
                var paramList = new List<NpgsqlParameter>();

                if (!string.IsNullOrEmpty(keyword))
                {
                    query += @" AND (
        ma_vt ILIKE @keyword OR
        ten_vt ILIKE @keyword OR
        min_ton::text ILIKE @keyword OR
        max_ton::text ILIKE @keyword OR
        rong::text ILIKE @keyword OR
        cao::text ILIKE @keyword OR
        khoi_luong::text ILIKE @keyword OR
        mau_sac ILIKE @keyword OR
        kieu_dang ILIKE @keyword OR
        mo_ta ILIKE @keyword OR
        (CASE 
            WHEN trangthai = 0 THEN 'Không sử dụng' 
            WHEN trangthai = 1 THEN 'Sử dụng' 
            ELSE ''
         END) ILIKE @keyword
    )";
        
                    paramList.Add(new NpgsqlParameter("@keyword", $"%{keyword}%"));
                }

                if (!string.IsNullOrEmpty(ma_vt))
                {
                    query += " AND ma_vt ILIKE @ma_vt";
                    paramList.Add(new NpgsqlParameter("@ma_vt", $"%{ma_vt}%"));
                }

                if (!string.IsNullOrEmpty(ten_vt))
                {
                    query += " AND ten_vt ILIKE @ten_vt";
                    paramList.Add(new NpgsqlParameter("@ten_vt", $"%{ten_vt}%"));
                }

                if (!string.IsNullOrEmpty(mo_ta))
                {
                    query += " AND mo_ta ILIKE @mo_ta";
                    paramList.Add(new NpgsqlParameter("@mo_ta", $"%{mo_ta}%"));
                }

                if (trangthaiStr == "0" || trangthaiStr == "1")
                {
                    query += " AND trangthai = @trangthai";
                    paramList.Add(new NpgsqlParameter("@trangthai", int.Parse(trangthaiStr)));
                }

                query += " ORDER BY ma_vt ASC";

                var result = db.Database.SqlQuery<DmvtClass>(query, paramList.ToArray()).ToList();
                return Ok(new { success = true, result });
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi xử lý tìm kiếm: " + ex.Message);
            }
        }


        [HttpPost]
        [Route("api/DmvtAPI/DeleteAll")]
        public IHttpActionResult DeleteAll([FromBody] List<DmvtClass> list)
        {
            if (list == null || !list.Any())
                return BadRequest("Danh sách rỗng");

            foreach (var item in list)
            {
                var dmvt = db.DmvtObj.FirstOrDefault(p => p.ma_vt == item.ma_vt);
                if (dmvt != null)
                {
                    db.DmvtObj.Remove(dmvt);
                }
            }
            db.SaveChanges();
            return Ok("Đã xóa các vật tư được chọn");
        }

        [HttpPost]
        [Route("api/DmvtAPI/SaveAdd")]
        public IHttpActionResult SaveAdd(JObject data)
        {
            try
            {
                var dmvt = data.ToObject<DmvtClass>();

                if (dmvt == null)
                    return BadRequest("Dữ liệu không hợp lệ");

                if (db.DmvtObj.Any(u => u.ma_vt == dmvt.ma_vt))
                {
                    return Ok(new { success = false, message = "Mã vật tư đã tồn tại." });
                }
                if (dmvt.khoi_luong != null)
                    dmvt.khoi_luong = Convert.ToDecimal(dmvt.khoi_luong);

                if (dmvt.rong != null)
                    dmvt.rong = Convert.ToDecimal(dmvt.rong);

                if (dmvt.cao != null)
                    dmvt.cao = Convert.ToDecimal(dmvt.cao);
               
                dmvt.url = "1";
                dmvt.barcode = "1";
                db.DmvtObj.Add(dmvt);
                db.SaveChanges();

                return Ok(new { success = true, result = dmvt });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.ToString() });
            }
        }

        [HttpPut]
        [Route("api/DmvtAPI/SaveEdit")]
        public IHttpActionResult SaveEdit(JObject data)
        {
            try
            {
                var dmvt = data.ToObject<DmvtClass>();

                if (dmvt == null || string.IsNullOrEmpty(dmvt.ma_vt))
                    return BadRequest("Dữ liệu không hợp lệ");

                var existingDmvt = db.DmvtObj.FirstOrDefault(u => u.ma_vt == dmvt.ma_vt);
                if (existingDmvt == null)
                    return NotFound();

                // Cập nhật thông tin
                existingDmvt.ma_loai_vt = dmvt.ma_loai_vt;
                existingDmvt.ten_vt = dmvt.ten_vt;
                existingDmvt.min_ton = dmvt.min_ton;
                existingDmvt.max_ton = dmvt.max_ton;
                existingDmvt.barcode = dmvt.barcode;
                existingDmvt.url = dmvt.url; 
                existingDmvt.rong = dmvt.rong;
                existingDmvt.cao = dmvt.cao;
                existingDmvt.khoi_luong = dmvt.khoi_luong;
                existingDmvt.mau_sac = dmvt.mau_sac;
                existingDmvt.kieu_dang = dmvt.kieu_dang;
                existingDmvt.trangthai = dmvt.trangthai;
                existingDmvt.mo_ta = dmvt.mo_ta;
                existingDmvt.ma_dvt = dmvt.ma_dvt;
                db.Entry(existingDmvt).State = EntityState.Modified;
                db.SaveChanges();

                return Ok(new { success = true, result = existingDmvt });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}

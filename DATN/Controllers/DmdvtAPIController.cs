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
    public class DmdvtAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public IHttpActionResult Get()
        {
            var query = "SELECT * FROM dmdvt ORDER BY ma_dvt ASC;";
            var dmdvtList = db.Database.SqlQuery<DmdvtClass>(query).ToList();
            return Ok(dmdvtList);
        }

        [HttpPost]
        [Route("api/DmdvtAPI/SEARCH")]
        public IHttpActionResult SEARCH(JObject data)
        {
            var keyword = (string)data["keyword"];
            var ma_dvt = (string)data["ma_dvt"];
            var ten_dvt = (string)data["ten_dvt"];
            var mo_ta = (string)data["mo_ta"];
            var trangthai = (string)data["trangthai"]; 

            var query = @"
                SELECT ma_dvt, ten_dvt, mo_ta,   trangthai
                FROM dmdvt
                WHERE 1=1";

            if (!string.IsNullOrEmpty(keyword))
                query += $@" AND (
                        ma_dvt ILIKE '%{keyword}%' OR 
                        ten_dvt ILIKE '%{keyword}%' OR 
                        mo_ta ILIKE '%{keyword}%' OR 
                       
                        trangthai ILIKE '%{keyword}%')";

            if (!string.IsNullOrEmpty(ma_dvt))
                query += $" AND ma_dvt ILIKE '%{ma_dvt}%'";

            if (!string.IsNullOrEmpty(ten_dvt))
                query += $" AND ten_dvt ILIKE '%{ten_dvt}%'";

            if (!string.IsNullOrEmpty(mo_ta))
                query += $" AND mo_ta ILIKE '%{mo_ta}%'";
             
            if (!string.IsNullOrEmpty(trangthai))
                query += $" AND trangthai = '{trangthai}'";

            query += " ORDER BY ma_dvt ASC";

            var result = db.Database.SqlQuery<DmdvtClass>(query).ToList();
            return Ok(new { success = true, result });
        }

        [HttpPost]
        [Route("api/DmdvtAPI/DeleteAll")]
        public IHttpActionResult DeleteAll([FromBody] List<DmdvtClass> list)
        {
            if (list == null || !list.Any())
                return BadRequest("Danh sách rỗng");

            foreach (var item in list)
            {
                var dmdvt = db.DmdvtObj.FirstOrDefault(p => p.ma_dvt == item.ma_dvt);
                if (dmdvt != null)
                {
                    db.DmdvtObj.Remove(dmdvt);
                }
            }
            db.SaveChanges();
            return Ok("Đã xóa các đơn vị tính được chọn");
        }

        [HttpPost]
        [Route("api/DmdvtAPI/SaveAdd")]
        public IHttpActionResult SaveAdd(JObject data)
        {
            try
            {
                var dmdvt = data.ToObject<DmdvtClass>();

                if (dmdvt == null)
                    return BadRequest("Dữ liệu không hợp lệ");

                if (db.DmdvtObj.Any(u => u.ma_dvt == dmdvt.ma_dvt))
                {
                    return Ok(new { success = false, message = "Mã đơn vị tính đã tồn tại." });
                }

                db.DmdvtObj.Add(dmdvt);
                db.SaveChanges();

                return Ok(new { success = true, result = dmdvt });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("api/DmdvtAPI/SaveEdit")]
        public IHttpActionResult SaveEdit(JObject data)
        {
            try
            {
                var dmdvt = data.ToObject<DmdvtClass>();

                if (dmdvt == null || string.IsNullOrEmpty(dmdvt.ma_dvt))
                    return BadRequest("Dữ liệu không hợp lệ");

                var existing = db.DmdvtObj.FirstOrDefault(u => u.ma_dvt == dmdvt.ma_dvt);
                if (existing == null)
                    return NotFound();

                existing.ten_dvt = dmdvt.ten_dvt;
                existing.mo_ta = dmdvt.mo_ta; 
                existing.trangthai = dmdvt.trangthai;

                db.Entry(existing).State = EntityState.Modified;
                db.SaveChanges();

                return Ok(new { success = true, result = existing });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}

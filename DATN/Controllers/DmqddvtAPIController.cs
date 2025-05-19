using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using WH.DataContext;
using WH.Models;
using ApplicationDbContext = WH.DataContext.ApplicationDbContext;

namespace WH.Controllers
{
    public class DmqddvtAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public IHttpActionResult Get()
        {
            var query = "SELECT * FROM dmqddvt ORDER BY ma_vt ASC, ma_dvt ASC;";
            var dmqddvtList = db.Database.SqlQuery<DmqddvtClass>(query).ToList();
            return Ok(dmqddvtList);
        }


        [HttpPost]
        [Route("api/DmqddvtAPI/SEARCH")]
        public IHttpActionResult SEARCH(JObject data)
        {
            var keyword = (string)data["keyword"];
            var ma_vt = (string)data["ma_vt"];
            var ma_dvt = (string)data["ma_dvt"];

            var query = @"
                SELECT ma_vt, ma_dvt, ty_le_quy_doi
                FROM dmqddvt
                WHERE 1=1";

            if (!string.IsNullOrEmpty(keyword))
                query += $@" AND (
                        ma_vt ILIKE '%{keyword}%' OR 
                        ma_dvt ILIKE '%{keyword}%')";

            if (!string.IsNullOrEmpty(ma_vt))
                query += $" AND ma_vt ILIKE '%{ma_vt}%'";

            if (!string.IsNullOrEmpty(ma_dvt))
                query += $" AND ma_dvt ILIKE '%{ma_dvt}%'";

            query += " ORDER BY ma_vt ASC, ma_dvt ASC";

            var result = db.Database.SqlQuery<DmqddvtClass>(query).ToList();
            return Ok(new { success = true, result });
        }

        [HttpPost]
        [Route("api/DmqddvtAPI/DeleteAll")]
        public IHttpActionResult DeleteAll([FromBody] List<DmqddvtClass> list)
        {
            if (list == null || !list.Any())
                return BadRequest("Danh sách rỗng");

            foreach (var item in list)
            {
                var entity = db.DmqddvtObj.FirstOrDefault(p => p.ma_vt == item.ma_vt && p.ma_dvt == item.ma_dvt);
                if (entity != null)
                {
                    db.DmqddvtObj.Remove(entity);
                }
            }
            db.SaveChanges();
            return Ok("Đã xóa các quy đổi đơn vị được chọn");
        }

        [HttpPost]
        [Route("api/DmqddvtAPI/SaveAdd")]
        public IHttpActionResult SaveAdd(JObject data)
        {
            try
            {
                var dmqddvt = data.ToObject<DmqddvtClass>();

                if (dmqddvt == null)
                    return BadRequest("Dữ liệu không hợp lệ");

                if (db.DmqddvtObj.Any(u => u.ma_vt == dmqddvt.ma_vt && u.ma_dvt == dmqddvt.ma_dvt))
                {
                    return Ok(new { success = false, message = "Mã vật tư + Mã đơn vị tính đã tồn tại." });
                }

                db.DmqddvtObj.Add(dmqddvt);
                db.SaveChanges();

                return Ok(new { success = true, result = dmqddvt });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPost]
        [Route("api/DmqddvtAPI/SaveAll")]
        public IHttpActionResult SaveAll(List<DmqddvtClass> list)
        {
            try
            {
                if (list == null || !list.Any())
                    return BadRequest("Danh sách trống");

                var ma_vt = list.First().ma_vt;

                // Xóa hết các bản ghi cũ theo ma_vt
                var oldItems = db.DmqddvtObj.Where(x => x.ma_vt == ma_vt).ToList();
                db.DmqddvtObj.RemoveRange(oldItems);
                db.SaveChanges();

                // Thêm mới các bản ghi gửi lên
                foreach (var item in list)
                {
                    db.DmqddvtObj.Add(item);
                }

                db.SaveChanges();

                return Ok(new { success = true, message = "Lưu thành công" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
        [HttpPut]
        [Route("api/DmqddvtAPI/SaveEdit")]
        public IHttpActionResult SaveEdit(JObject data)
        {
            try
            {
                var dmqddvt = data.ToObject<DmqddvtClass>();

                if (dmqddvt == null || string.IsNullOrEmpty(dmqddvt.ma_vt) || string.IsNullOrEmpty(dmqddvt.ma_dvt))
                    return BadRequest("Dữ liệu không hợp lệ");

                var existing = db.DmqddvtObj.FirstOrDefault(u => u.ma_vt == dmqddvt.ma_vt && u.ma_dvt == dmqddvt.ma_dvt);
                if (existing == null)
                    return NotFound();

                existing.ty_le_quy_doi = dmqddvt.ty_le_quy_doi;

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

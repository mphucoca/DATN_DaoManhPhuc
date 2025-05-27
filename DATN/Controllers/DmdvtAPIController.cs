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
using System.Threading.Tasks;
using WH.Helpers;

namespace WH.Controllers
{
    public class DmdvtAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private AuditHelper auditHelper;

        public DmdvtAPIController()
        {
            auditHelper = new AuditHelper(db);
        }

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
                SELECT ma_dvt, ten_dvt, mo_ta, trangthai
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
        public async Task<IHttpActionResult> DeleteAll([FromBody] List<DmdvtClass> list)
        {
            if (list == null || !list.Any())
                return BadRequest("Danh sách rỗng");

            foreach (var item in list)
            {
                var dmdvt = db.DmdvtObj.FirstOrDefault(p => p.ma_dvt == item.ma_dvt);
                if (dmdvt != null)
                {
                    var primaryKeyData = new { ma_dvt = dmdvt.ma_dvt };

                    // Ghi log trước khi xóa
                    await auditHelper.SaveAuditLogAsync(
                        tableName: "dmdvt",
                        operation: "DELETE",
                        primaryKeyData: primaryKeyData,
                        oldData: dmdvt,
                        newData: null
                    );

                    db.DmdvtObj.Remove(dmdvt);
                }
            }
            await db.SaveChangesAsync();
            return Ok("Đã xóa các đơn vị tính được chọn");
        }

        [HttpPost]
        [Route("api/DmdvtAPI/SaveAdd")]
        public async Task<IHttpActionResult> SaveAdd(JObject data)
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
                await db.SaveChangesAsync();

                // Ghi log INSERT
                var primaryKeyData = new { ma_dvt = dmdvt.ma_dvt };
                await auditHelper.SaveAuditLogAsync(
                    tableName: "dmdvt",
                    operation: "INSERT",
                    primaryKeyData: primaryKeyData,
                    oldData: null,
                    newData: dmdvt
                );

                return Ok(new { success = true, result = dmdvt });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("api/DmdvtAPI/SaveEdit")]
        public async Task<IHttpActionResult> SaveEdit(JObject data)
        {
            try
            {
                var dmdvt = data.ToObject<DmdvtClass>();

                if (dmdvt == null || string.IsNullOrEmpty(dmdvt.ma_dvt))
                    return BadRequest("Dữ liệu không hợp lệ");

                var existing = db.DmdvtObj.FirstOrDefault(u => u.ma_dvt == dmdvt.ma_dvt);
                if (existing == null)
                    return NotFound();

                // Lưu dữ liệu cũ để log
                var oldData = new DmdvtClass
                {
                    ma_dvt = existing.ma_dvt,
                    ten_dvt = existing.ten_dvt,
                    mo_ta = existing.mo_ta,
                    trangthai = existing.trangthai
                };

                existing.ten_dvt = dmdvt.ten_dvt;
                existing.mo_ta = dmdvt.mo_ta;
                existing.trangthai = dmdvt.trangthai;

                db.Entry(existing).State = EntityState.Modified;
                await db.SaveChangesAsync();

                // Ghi log UPDATE
                var primaryKeyData = new { ma_dvt = dmdvt.ma_dvt };
                await auditHelper.SaveAuditLogAsync(
                    tableName: "dmdvt",
                    operation: "UPDATE",
                    primaryKeyData: primaryKeyData,
                    oldData: oldData,
                    newData: existing
                );

                return Ok(new { success = true, result = existing });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        [Route("api/LOGAPI/GetAuditLogByTable")]
        public IHttpActionResult GetAuditLogByTable()
        {

            var query = @"
        SELECT 
            a.id AS id,
            a.table_name AS table_name,
            a.operation AS operation,
            a.primary_key_data AS primary_key_data,
            a.old_data AS old_data,
            a.new_data AS new_data,
            a.changed_by AS changed_by,
            a.changed_at AS changed_at,
            b.username AS username
        FROM audit_log a
        LEFT JOIN userinfo b ON a.changed_by = b.id
        WHERE a.table_name = 'dmdvt'
        ORDER BY a.changed_at DESC;
    ";

            var result = db.Database.SqlQuery<AuditLogView>(query).ToList();

            return Ok(result);
        }
        [HttpGet]
        [Route("api/LOGAPI/GetAuditLogByTableCT")]
        public IHttpActionResult GetAuditLogByTableCT()
        {

            var query = @"
        SELECT 
            a.id AS id,
            a.table_name AS table_name,
            a.operation AS operation,
            a.primary_key_data AS primary_key_data,
            a.old_data AS old_data,
            a.new_data AS new_data,
            a.changed_by AS changed_by,
            a.changed_at AS changed_at,
            b.username AS username
        FROM audit_log a
        LEFT JOIN userinfo b ON a.changed_by = b.id
        WHERE a.table_name = 'dmdvt'
        ORDER BY a.changed_at DESC;
    ";

            var result = db.Database.SqlQuery<AuditLogView>(query).ToList();

            return Ok(result);
        }
    }
}

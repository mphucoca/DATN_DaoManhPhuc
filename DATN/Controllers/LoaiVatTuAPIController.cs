using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WH.DataContext;
using WH.Helpers;
using WH.Models;
using ApplicationDbContext = WH.DataContext.ApplicationDbContext;

namespace WH.Controllers
{
    public class LoaiVatTuAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
  
        private AuditHelper auditHelper;
        public LoaiVatTuAPIController()
        {
            auditHelper = new AuditHelper(db);
        }
        public IHttpActionResult Get()
        {
            var query = @"SELECT * FROM loai_vat_tu ORDER BY ma_loai_vt ASC;";
            var list = db.Database.SqlQuery<LoaiVatTuClass>(query).ToList();
            return Ok(list);
        }

        [HttpPost]
        [Route("api/LoaiVatTuAPI/SEARCH")]
        public IHttpActionResult SEARCH(JObject data)
        {
            try
            {
                var keyword = (string)data["keyword"];
                var ma_loai_vt = (string)data["ma_loai_vt"];
                var ten_loai_vt = (string)data["ten_loai_vt"];
                var mo_ta = (string)data["mo_ta"];
                var trangthaiStr = (string)data["trangthai"];

                var query = @"SELECT * FROM loai_vat_tu WHERE 1=1";
                var paramList = new List<NpgsqlParameter>();

                if (!string.IsNullOrEmpty(keyword))
                {
                    query += @" AND (
                        ma_loai_vt ILIKE @keyword OR
                        ten_loai_vt ILIKE @keyword OR
                        mo_ta ILIKE @keyword OR
                        (CASE 
                            WHEN trangthai = 0 THEN 'Không sử dụng' 
                            WHEN trangthai = 1 THEN 'Sử dụng' 
                            ELSE ''
                         END) ILIKE @keyword
                    )";
                    paramList.Add(new NpgsqlParameter("@keyword", $"%{keyword}%"));
                }

                if (!string.IsNullOrEmpty(ma_loai_vt))
                {
                    query += " AND ma_loai_vt ILIKE @ma_loai_vt";
                    paramList.Add(new NpgsqlParameter("@ma_loai_vt", $"%{ma_loai_vt}%"));
                }

                if (!string.IsNullOrEmpty(ten_loai_vt))
                {
                    query += " AND ten_loai_vt ILIKE @ten_loai_vt";
                    paramList.Add(new NpgsqlParameter("@ten_loai_vt", $"%{ten_loai_vt}%"));
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

                query += " ORDER BY ma_loai_vt ASC";

                var result = db.Database.SqlQuery<LoaiVatTuClass>(query, paramList.ToArray()).ToList();
                return Ok(new { success = true, result });
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi xử lý tìm kiếm: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("api/LoaiVatTuAPI/DeleteAll")]
        public async Task<IHttpActionResult> DeleteAll([FromBody] List<LoaiVatTuClass> list)
        {
            if (list == null || !list.Any())
                return BadRequest("Danh sách rỗng");

            foreach (var item in list)
            {
                var obj = db.LoaiVatTuObj.FirstOrDefault(p => p.ma_loai_vt == item.ma_loai_vt);
                if (obj != null)
                {
                    var primaryKeyData = new { ma_loai_vt = obj.ma_loai_vt };

                    // Ghi log trước khi xóa
                    await auditHelper.SaveAuditLogAsync(
                        tableName: "loai_vat_tu",
                        operation: "DELETE",
                        primaryKeyData: primaryKeyData,
                        oldData: obj,
                        newData: null
                    );

                    db.LoaiVatTuObj.Remove(obj);
                }
            }

            await db.SaveChangesAsync();
            return Ok("Đã xóa các loại vật tư được chọn");
        }


        [HttpPost]
        [Route("api/LoaiVatTuAPI/SaveAdd")]
        public async Task<IHttpActionResult> SaveAdd(JObject data)
        {
            try
            {
                var obj = data.ToObject<LoaiVatTuClass>();

                if (obj == null)
                    return BadRequest("Dữ liệu không hợp lệ");

                if (db.LoaiVatTuObj.Any(u => u.ma_loai_vt == obj.ma_loai_vt))
                {
                    return Ok(new { success = false, message = "Mã loại vật tư đã tồn tại." });
                }

                db.LoaiVatTuObj.Add(obj);
                await db.SaveChangesAsync();

                var primaryKeyData = new { ma_loai_vt = obj.ma_loai_vt };

                // Ghi log INSERT
                await auditHelper.SaveAuditLogAsync(
                    tableName: "loai_vat_tu",
                    operation: "INSERT",
                    primaryKeyData: primaryKeyData,
                    oldData: null,
                    newData: obj
                );

                return Ok(new { success = true, result = obj });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.ToString() });
            }
        }
        [HttpPut]
        [Route("api/LoaiVatTuAPI/SaveEdit")]
        public async Task<IHttpActionResult> SaveEdit(JObject data)
        {
            try
            {
                var obj = data.ToObject<LoaiVatTuClass>();

                if (obj == null || string.IsNullOrEmpty(obj.ma_loai_vt))
                    return BadRequest("Dữ liệu không hợp lệ");

                var existingObj = db.LoaiVatTuObj.FirstOrDefault(u => u.ma_loai_vt == obj.ma_loai_vt);
                if (existingObj == null)
                    return NotFound();

                // Lưu dữ liệu cũ để log
                var oldData = new LoaiVatTuClass
                {
                    ma_loai_vt = existingObj.ma_loai_vt,
                    ten_loai_vt = existingObj.ten_loai_vt,
                    mo_ta = existingObj.mo_ta,
                    trangthai = existingObj.trangthai
                };

                existingObj.ten_loai_vt = obj.ten_loai_vt;
                existingObj.mo_ta = obj.mo_ta;
                existingObj.trangthai = obj.trangthai;

                db.Entry(existingObj).State = EntityState.Modified;
                await db.SaveChangesAsync();

                var primaryKeyData = new { ma_loai_vt = obj.ma_loai_vt };

                // Ghi log UPDATE
                await auditHelper.SaveAuditLogAsync(
                    tableName: "loai_vat_tu",
                    operation: "UPDATE",
                    primaryKeyData: primaryKeyData,
                    oldData: oldData,
                    newData: existingObj
                );

                return Ok(new { success = true, result = existingObj });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        [Route("api/LOG5API/GetAuditLogByTable")]
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
        WHERE a.table_name = 'loai_vat_tu'
        ORDER BY a.changed_at DESC;
    ";

            var result = db.Database.SqlQuery<AuditLogView>(query).ToList();

            return Ok(result);
        }
        [HttpGet]
        [Route("api/LOG5API/GetAuditLogByTableCT")]
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
        WHERE a.table_name = 'loai_vat_tu'
        ORDER BY a.changed_at DESC;
    ";

            var result = db.Database.SqlQuery<AuditLogView>(query).ToList();

            return Ok(result);
        }

    }
}

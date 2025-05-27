using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WH.DataContext;
using WH.Models;
using WH.Helpers;
using ApplicationDbContext = WH.DataContext.ApplicationDbContext;

namespace WH.Controllers
{
    public class DMNCCAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private AuditHelper auditHelper;

        public DMNCCAPIController()
        {
            auditHelper = new AuditHelper(db);
        }

        public IHttpActionResult Get()
        {
            var query = @"SELECT ma_ncc, ten_ncc, dia_chi, dien_thoai, ghi_chu, email, ma_so_thue FROM dmncc ORDER BY ma_ncc ASC;";
            var nccList = db.Database.SqlQuery<DmnccClass>(query).ToList();
            return Ok(nccList);
        }

        [HttpPost]
        [Route("api/DMNCCAPI/SEARCH")]
        public IHttpActionResult SEARCH(JObject data)
        {
            var keyword = (string)data["keyword"];
            var ma_ncc = (string)data["ma_ncc"];
            var ten_ncc = (string)data["ten_ncc"];
            var dia_chi = (string)data["dia_chi"];
            var dien_thoai = (string)data["dien_thoai"];
            var ghi_chu = (string)data["ghi_chu"];
            var email = (string)data["email"];
            var query = @"SELECT * FROM dmncc WHERE 1=1";

            if (!string.IsNullOrEmpty(keyword))
                query += $@" AND (
                    ma_ncc ILIKE '%{keyword}%' OR 
                    ten_ncc ILIKE '%{keyword}%' OR 
                    dia_chi ILIKE '%{keyword}%' OR 
                    ghi_chu ILIKE '%{keyword}%' OR 
                    dien_thoai ILIKE '%{keyword}%' OR 
                    ma_so_thue ILIKE '%{keyword}%' OR 
                    email ILIKE '%{keyword}%')";

            if (!string.IsNullOrEmpty(ma_ncc)) query += $" AND ma_ncc ILIKE '%{ma_ncc}%'";
            if (!string.IsNullOrEmpty(ten_ncc)) query += $" AND ten_ncc ILIKE '%{ten_ncc}%'";
            if (!string.IsNullOrEmpty(dia_chi)) query += $" AND dia_chi ILIKE '%{dia_chi}%'";
            if (!string.IsNullOrEmpty(dien_thoai)) query += $" AND dien_thoai ILIKE '%{dien_thoai}%'";
            if (!string.IsNullOrEmpty(ghi_chu)) query += $" AND ghi_chu ILIKE '%{ghi_chu}%'";
            if (!string.IsNullOrEmpty(email)) query += $" AND email ILIKE '%{email}%'";
            query += " ORDER BY ma_ncc ASC";

            var result = db.Database.SqlQuery<DmnccClass>(query).ToList();

            return Ok(new { success = true, result });
        }

        [HttpPost]
        [Route("api/DMNCCAPI/DeleteAll")]
        public async Task<IHttpActionResult> DeleteAll([FromBody] List<DmnccClass> list)
        {
            if (list == null || !list.Any())
                return BadRequest("Danh sách rỗng");

            foreach (var item in list)
            {
                var ncc = db.DmnccObj.FirstOrDefault(p => p.ma_ncc == item.ma_ncc);
                if (ncc != null)
                {
                    var primaryKeyData = new { ma_ncc = ncc.ma_ncc };

                    await auditHelper.SaveAuditLogAsync(
                        tableName: "dmncc",
                        operation: "DELETE",
                        primaryKeyData: primaryKeyData,
                        oldData: ncc,
                        newData: null
                    );

                    db.DmnccObj.Remove(ncc);
                }
            }
            await db.SaveChangesAsync();
            return Ok("Đã xóa các nhà cung cấp được chọn");
        }

        [HttpPost]
        [Route("api/DMNCCAPI/SaveAdd")]
        public async Task<IHttpActionResult> SaveAdd(JObject data)
        {
            try
            {
                var _dmncc = data.ToObject<DmnccClass>();

                if (_dmncc == null || string.IsNullOrEmpty(_dmncc.ten_ncc))
                    return BadRequest("Dữ liệu không hợp lệ");

                if (string.IsNullOrEmpty(_dmncc.ma_ncc))
                {
                    var lastNcc = db.DmnccObj
                        .AsEnumerable()
                        .Where(u => int.TryParse(u.ma_ncc, out _))
                        .OrderByDescending(u => int.Parse(u.ma_ncc))
                        .Select(u => u.ma_ncc)
                        .FirstOrDefault();
                    _dmncc.ma_ncc = lastNcc == null ? "1" : (int.Parse(lastNcc) + 1).ToString();
                }
                else if (db.DmnccObj.Any(u => u.ma_ncc == _dmncc.ma_ncc))
                {
                    return Ok(new { success = false, message = "Mã nhà cung cấp đã tồn tại." });
                }

                db.DmnccObj.Add(_dmncc);
                await db.SaveChangesAsync();

                var primaryKeyData = new { ma_ncc = _dmncc.ma_ncc };

                await auditHelper.SaveAuditLogAsync(
                    tableName: "dmncc",
                    operation: "INSERT",
                    primaryKeyData: primaryKeyData,
                    oldData: null,
                    newData: _dmncc
                );

                return Ok(new { success = true, result = _dmncc });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("api/DMNCCAPI/SaveEdit")]
        public async Task<IHttpActionResult> SaveEdit(JObject data)
        {
            try
            {
                var _dmncc = data.ToObject<DmnccClass>();

                if (_dmncc == null || string.IsNullOrEmpty(_dmncc.ma_ncc) || string.IsNullOrEmpty(_dmncc.ten_ncc))
                    return BadRequest("Dữ liệu không hợp lệ");

                var existing = db.DmnccObj.FirstOrDefault(u => u.ma_ncc == _dmncc.ma_ncc);
                if (existing == null)
                    return NotFound();

                var oldData = new DmnccClass
                {
                    ma_ncc = existing.ma_ncc,
                    ten_ncc = existing.ten_ncc,
                    dia_chi = existing.dia_chi,
                    dien_thoai = existing.dien_thoai,
                    ghi_chu = existing.ghi_chu,
                    email = existing.email,
                    ma_so_thue = existing.ma_so_thue
                };

                existing.ten_ncc = _dmncc.ten_ncc;
                existing.dia_chi = _dmncc.dia_chi;
                existing.dien_thoai = _dmncc.dien_thoai;
                existing.ghi_chu = _dmncc.ghi_chu;
                existing.email = _dmncc.email;
                existing.ma_so_thue = _dmncc.ma_so_thue;

                await db.SaveChangesAsync();

                var primaryKeyData = new { ma_ncc = _dmncc.ma_ncc };

                await auditHelper.SaveAuditLogAsync(
                    tableName: "dmncc",
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
        [Route("api/DMNCCAPI/GetMaSoThue")]
        public IHttpActionResult GetMaSoThue(string ma_ncc)
        {
            if (string.IsNullOrEmpty(ma_ncc))
                return BadRequest("Mã nhà cung cấp không được để trống");

            var ncc = db.DmnccObj
                .Where(x => x.ma_ncc == ma_ncc)
                .Select(x => new { x.ma_ncc, x.ma_so_thue })
                .FirstOrDefault();

            if (ncc == null)
                return NotFound();

            return Ok(new { success = true, result = ncc });
        }
        [HttpGet]
        [Route("api/LOG2API/GetAuditLogByTable")]
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
        WHERE a.table_name = 'dmncc'
        ORDER BY a.changed_at DESC;
    ";

            var result = db.Database.SqlQuery<AuditLogView>(query).ToList();

            return Ok(result);
        }
        [HttpGet]
        [Route("api/LOG2API/GetAuditLogByTableCT")]
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
        WHERE a.table_name = 'dmncc'
        ORDER BY a.changed_at DESC;
    ";

            var result = db.Database.SqlQuery<AuditLogView>(query).ToList();

            return Ok(result);
        }
    }
}

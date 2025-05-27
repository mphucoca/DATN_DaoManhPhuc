using Newtonsoft.Json.Linq;
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
    public class DMKhachHangAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private AuditHelper auditHelper;

        public DMKhachHangAPIController()
        {
            auditHelper = new AuditHelper(db);
        }

        public IHttpActionResult Get()
        {
            var query = @"SELECT ma_kh, ten_kh, dia_chi, dien_thoai, mo_ta, ma_so_thue FROM dmkh ORDER BY ma_kh ASC;";
            var khachHangList = db.Database.SqlQuery<DmkhClass>(query).ToList();
            return Ok(khachHangList);
        }

        [HttpPost]
        [Route("api/DMKhachHangAPI/SEARCH")]
        public IHttpActionResult SEARCH(JObject data)
        {
            var keyword = (string)data["keyword"];
            var ma_kh = (string)data["ma_kh"];
            var ten_kh = (string)data["ten_kh"];
            var dia_chi = (string)data["dia_chi"];
            var dien_thoai = (string)data["dien_thoai"];
            var mo_ta = (string)data["mo_ta"];
            var ma_so_thue = (string)data["ma_so_thue"];
            var query = @"SELECT * FROM dmkh WHERE 1=1";

            if (!string.IsNullOrEmpty(keyword))
                query += $@" AND (
                    ma_kh ILIKE '%{keyword}%' OR 
                    ten_kh ILIKE '%{keyword}%' OR 
                    dia_chi ILIKE '%{keyword}%' OR 
                    mo_ta ILIKE '%{keyword}%' OR 
                    ma_so_thue ILIKE '%{keyword}%' OR 
                    dien_thoai ILIKE '%{keyword}%')";

            if (!string.IsNullOrEmpty(ma_kh))
                query += $" AND ma_kh ILIKE '%{ma_kh}%'";
            if (!string.IsNullOrEmpty(ten_kh))
                query += $" AND ten_kh ILIKE '%{ten_kh}%'";
            if (!string.IsNullOrEmpty(dia_chi))
                query += $" AND dia_chi ILIKE '%{dia_chi}%'";
            if (!string.IsNullOrEmpty(dien_thoai))
                query += $" AND dien_thoai ILIKE '%{dien_thoai}%'";
            if (!string.IsNullOrEmpty(mo_ta))
                query += $" AND mo_ta ILIKE '%{mo_ta}%'";
            if (!string.IsNullOrEmpty(ma_so_thue))
                query += $" AND ma_so_thue ILIKE '%{ma_so_thue}%'";

            query += " ORDER BY ma_kh ASC";

            var result = db.Database.SqlQuery<DmkhClass>(query).ToList();

            return Ok(new { success = true, result });
        }

        [HttpPost]
        [Route("api/DMKhachHangAPI/DeleteAll")]
        public async Task<IHttpActionResult> DeleteAll([FromBody] List<DmkhClass> list)
        {
            if (list == null || !list.Any())
                return BadRequest("Danh sách rỗng");

            foreach (var item in list)
            {
                var kh = db.DmkhObj.FirstOrDefault(p => p.ma_kh == item.ma_kh);
                if (kh != null)
                {
                    var primaryKeyData = new { ma_kh = kh.ma_kh };

                    // Ghi log trước khi xóa
                    await auditHelper.SaveAuditLogAsync(
                        tableName: "dmkh",
                        operation: "DELETE",
                        primaryKeyData: primaryKeyData,
                        oldData: kh,
                        newData: null
                    );

                    db.DmkhObj.Remove(kh);
                }
            }
            await db.SaveChangesAsync();
            return Ok("Đã xóa các khách hàng được chọn");
        }

        [HttpPost]
        [Route("api/DMKhachHangAPI/SaveAdd")]
        public async Task<IHttpActionResult> SaveAdd(JObject data)
        {
            try
            {
                var _dmkh = data.ToObject<DmkhClass>();

                if (_dmkh == null)
                    return BadRequest("Dữ liệu không hợp lệ");

                if (string.IsNullOrEmpty(_dmkh.ten_kh))
                    return BadRequest("Tên khách hàng không được để trống");

                if (string.IsNullOrEmpty(_dmkh.ma_kh))
                {
                    var lastKh = db.DmkhObj
                        .AsEnumerable()
                        .Where(u => int.TryParse(u.ma_kh, out _))
                        .OrderByDescending(u => int.Parse(u.ma_kh))
                        .Select(u => u.ma_kh)
                        .FirstOrDefault();
                    _dmkh.ma_kh = lastKh == null ? "1" : (int.Parse(lastKh) + 1).ToString();
                }
                else if (db.DmkhObj.Any(u => u.ma_kh == _dmkh.ma_kh))
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Mã khách hàng đã tồn tại."
                    });
                }

                db.DmkhObj.Add(_dmkh);
                await db.SaveChangesAsync();

                var primaryKeyData = new { ma_kh = _dmkh.ma_kh };

                // Ghi log INSERT
                await auditHelper.SaveAuditLogAsync(
                    tableName: "dmkh",
                    operation: "INSERT",
                    primaryKeyData: primaryKeyData,
                    oldData: null,
                    newData: _dmkh
                );

                return Ok(new
                {
                    success = true,
                    result = _dmkh
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("api/DMKhachHangAPI/SaveEdit")]
        public async Task<IHttpActionResult> SaveEdit(JObject data)
        {
            try
            {
                var _dmkh = data.ToObject<DmkhClass>();

                if (_dmkh == null || string.IsNullOrEmpty(_dmkh.ma_kh))
                    return BadRequest("Dữ liệu không hợp lệ");

                if (string.IsNullOrEmpty(_dmkh.ten_kh))
                    return BadRequest("Tên khách hàng không được để trống");

                var existingKh = db.DmkhObj.FirstOrDefault(u => u.ma_kh == _dmkh.ma_kh);
                if (existingKh == null)
                    return NotFound();

                var oldData = new DmkhClass
                {
                    ma_kh = existingKh.ma_kh,
                    ten_kh = existingKh.ten_kh,
                    dia_chi = existingKh.dia_chi,
                    dien_thoai = existingKh.dien_thoai,
                    mo_ta = existingKh.mo_ta,
                    ma_so_thue = existingKh.ma_so_thue
                };

                existingKh.ten_kh = _dmkh.ten_kh;
                existingKh.dia_chi = _dmkh.dia_chi;
                existingKh.dien_thoai = _dmkh.dien_thoai;
                existingKh.mo_ta = _dmkh.mo_ta;
                existingKh.ma_so_thue = _dmkh.ma_so_thue;

                await db.SaveChangesAsync();

                var primaryKeyData = new { ma_kh = _dmkh.ma_kh };

                // Ghi log UPDATE
                await auditHelper.SaveAuditLogAsync(
                    tableName: "dmkh",
                    operation: "UPDATE",
                    primaryKeyData: primaryKeyData,
                    oldData: oldData,
                    newData: existingKh
                );

                return Ok(new
                {
                    success = true,
                    result = existingKh
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/DMKhachHangAPI/GetMaSoThue")]
        public IHttpActionResult GetMaSoThue(string ma_kh)
        {
            if (string.IsNullOrEmpty(ma_kh))
                return BadRequest("Mã khách hàng không được để trống");

            var ncc = db.DmkhObj
                .Where(x => x.ma_kh == ma_kh)
                .Select(x => new { x.ma_kh, x.ma_so_thue })
                .FirstOrDefault();

            if (ncc == null)
                return NotFound();

            return Ok(new
            {
                success = true,
                result = ncc
            });
        }
        [HttpGet]
        [Route("api/LOG1API/GetAuditLogByTable")]
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
        WHERE a.table_name = 'dmkh'
        ORDER BY a.changed_at DESC;
    ";

            var result = db.Database.SqlQuery<AuditLogView>(query).ToList();

            return Ok(result);
        }
        [HttpGet]
        [Route("api/LOG1API/GetAuditLogByTableCT")]
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
        WHERE a.table_name = 'dmkh'
        ORDER BY a.changed_at DESC;
    ";

            var result = db.Database.SqlQuery<AuditLogView>(query).ToList();

            return Ok(result);
        }
    }
}

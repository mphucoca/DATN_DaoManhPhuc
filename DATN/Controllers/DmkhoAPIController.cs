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
    public class DmkhoAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private AuditHelper auditHelper;

        public DmkhoAPIController()
        {
            auditHelper = new AuditHelper(db);
        }

        public IHttpActionResult Get()
        {
            var query = @"SELECT * FROM dmkho ORDER BY ma_kho ASC;";
            var dmkhoList = db.Database.SqlQuery<DmkhoClass>(query).ToList();
            return Ok(dmkhoList);
        }

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
        public async Task<IHttpActionResult> DeleteAll([FromBody] List<DmkhoClass> list)
        {
            if (list == null || !list.Any())
                return BadRequest("Danh sách rỗng");

            foreach (var item in list)
            {
                var kho = db.DmkhoObj.FirstOrDefault(p => p.ma_kho == item.ma_kho);
                if (kho != null)
                {
                    db.DmkhoObj.Remove(kho);

                    // Log xóa từng kho
                    await auditHelper.SaveAuditLogAsync(
                        tableName: "dmkho",
                        operation: "DELETE",
                        primaryKeyData: new { ma_kho = kho.ma_kho },
                        oldData: kho,
                        newData: null
                    );
                }
            }
            await db.SaveChangesAsync();
            return Ok("Đã xóa các kho được chọn");
        }

        [HttpPost]
        [Route("api/DmkhoAPI/SaveAdd")]
        public async Task<IHttpActionResult> SaveAdd(JObject data)
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
                await db.SaveChangesAsync();

                // Log thêm mới kho
                await auditHelper.SaveAuditLogAsync(
                    tableName: "dmkho",
                    operation: "INSERT",
                    primaryKeyData: new { ma_kho = kho.ma_kho },
                    oldData: null,
                    newData: kho
                );

                return Ok(new { success = true, result = kho });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("api/DmkhoAPI/SaveEdit")]
        public async Task<IHttpActionResult> SaveEdit(JObject data)
        {
            try
            {
                var kho = data.ToObject<DmkhoClass>();

                if (kho == null || string.IsNullOrEmpty(kho.ma_kho))
                    return BadRequest("Dữ liệu không hợp lệ");

                var existingKho = db.DmkhoObj.FirstOrDefault(k => k.ma_kho == kho.ma_kho);
                if (existingKho == null)
                    return NotFound();

                // Lưu bản sao để log old data
                var oldKho = new DmkhoClass
                {
                    ma_kho = existingKho.ma_kho,
                    ten_kho = existingKho.ten_kho,
                    mo_ta = existingKho.mo_ta,
                    dia_chi = existingKho.dia_chi
                };

                existingKho.ten_kho = kho.ten_kho;
                existingKho.mo_ta = kho.mo_ta;
                existingKho.dia_chi = kho.dia_chi;

                db.Entry(existingKho).State = EntityState.Modified;
                await db.SaveChangesAsync();

                // Log sửa kho
                await auditHelper.SaveAuditLogAsync(
                    tableName: "dmkho",
                    operation: "UPDATE",
                    primaryKeyData: new { ma_kho = existingKho.ma_kho },
                    oldData: oldKho,
                    newData: existingKho
                );

                return Ok(new { success = true, result = existingKho });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
     
        [HttpPost]
        [Route("api/TonKhoAPI/SaveTonKho")]
        public async Task<IHttpActionResult> SaveTonKho(JObject data)
        {
            try
            {
                var ma_kho = (string)data["ma_kho"];
                var ds_ton_kho = data["ds_ton_kho"].ToObject<List<TonKhoClass>>();

                if (string.IsNullOrEmpty(ma_kho) || ds_ton_kho == null)
                    return BadRequest("Dữ liệu không hợp lệ");

                // Lấy tồn kho cũ để kiểm tra và log
                var tonKhoCu = db.TonKhoObj.Where(tk => tk.ma_kho == ma_kho).ToList();
                bool isInsert = !tonKhoCu.Any(); // Không có bản ghi cũ => là INSERT

                // Xóa tồn kho cũ
                db.TonKhoObj.RemoveRange(tonKhoCu);
                await db.SaveChangesAsync();
               
                // Thêm và ghi log cho mỗi bản ghi tồn kho mới
                foreach (var item in ds_ton_kho)
                {
                    item.ma_kho = ma_kho;
                    db.TonKhoObj.Add(item);
                    await db.SaveChangesAsync();
                    db.Entry(item).State = EntityState.Detached;
                    // Tìm bản ghi cũ tương ứng (nếu có) để log UPDATE, ngược lại là INSERT
                    var old = tonKhoCu.FirstOrDefault(x => x.ma_vt == item.ma_vt && x.ma_dvt == item.ma_dvt);
                    string action = old == null ? "INSERT" : "UPDATE";

                    await auditHelper.SaveAuditLogAsync(
                        tableName: "ton_kho",
                        operation: action,
                        primaryKeyData: new { ma_kho = item.ma_kho, ma_vt = item.ma_vt, ma_dvt = item.ma_dvt },
                        oldData: old,
                        newData: item
                    );
                }

             

                return Ok(new { success = true, message = "Lưu tồn kho thành công" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Có lỗi xảy ra khi lưu tồn kho: " + ex.Message });
            }
        }
        [HttpGet]
        [Route("api/TonKhoAPI/GetAuditLogByTable")]
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
        WHERE a.table_name = 'dmkho'
        ORDER BY a.changed_at DESC;
    ";

            var result = db.Database.SqlQuery<AuditLogView>(query).ToList();

            return Ok(result);
        }
        [HttpGet]
        [Route("api/TonKhoAPI/GetAuditLogByTableCT")]
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
        WHERE a.table_name = 'ton_kho'
        ORDER BY a.changed_at DESC;
    ";

            var result = db.Database.SqlQuery<AuditLogView>(query).ToList();

            return Ok(result);
        }



    }
}

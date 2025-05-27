using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WH.DataContext;
using WH.Helpers;
using WH.Models;
using ApplicationDbContext = WH.DataContext.ApplicationDbContext;

namespace WH.Controllers
{
    public class DmvtAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private AuditHelper auditHelper;

        public DmvtAPIController()
        {
            auditHelper = new AuditHelper(db);
        }
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
        public async Task<IHttpActionResult> DeleteAll([FromBody] List<DmvtClass> list)
        {
            if (list == null || !list.Any())
                return BadRequest("Danh sách rỗng");

            foreach (var item in list)
            {
                var dmvt = db.DmvtObj.FirstOrDefault(p => p.ma_vt == item.ma_vt);
                if (dmvt != null)
                {
                    var primaryKeyData = new { ma_vt = dmvt.ma_vt };

                    // Ghi log trước khi xóa
                    await auditHelper.SaveAuditLogAsync(
                        tableName: "dmvt",
                        operation: "DELETE",
                        primaryKeyData: primaryKeyData,
                        oldData: dmvt,
                        newData: null
                    );

                    db.DmvtObj.Remove(dmvt);
                }
            }
            await db.SaveChangesAsync();
            return Ok("Đã xóa các vật tư được chọn");
        }

        [HttpPost]
        [Route("api/DmvtAPI/SaveAdd")]
        public async Task<IHttpActionResult> SaveAdd(JObject data)
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

                dmvt.khoi_luong = dmvt.khoi_luong ?? 0;
                dmvt.rong = dmvt.rong ?? 0;
                dmvt.cao = dmvt.cao ?? 0;
               
                dmvt.barcode = "1";

                db.DmvtObj.Add(dmvt);
                await db.SaveChangesAsync();

                var primaryKeyData = new { ma_vt = dmvt.ma_vt };
                await auditHelper.SaveAuditLogAsync(
                    tableName: "dmvt",
                    operation: "INSERT",
                    primaryKeyData: primaryKeyData,
                    oldData: null,
                    newData: dmvt
                );

                return Ok(new { success = true, result = dmvt });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.ToString() });
            }
        }

        [HttpPut]
        [Route("api/DmvtAPI/SaveEdit")]
        public async Task<IHttpActionResult> SaveEdit(JObject data)
        {
            try
            {
                var dmvt = data.ToObject<DmvtClass>();

                if (dmvt == null || string.IsNullOrEmpty(dmvt.ma_vt))
                    return BadRequest("Dữ liệu không hợp lệ");

                var existingDmvt = db.DmvtObj.FirstOrDefault(u => u.ma_vt == dmvt.ma_vt);
                if (existingDmvt == null)
                    return NotFound();

                // Lưu dữ liệu cũ để log
                var oldData = new DmvtClass
                {
                    ma_vt = existingDmvt.ma_vt,
                    ma_loai_vt = existingDmvt.ma_loai_vt,
                    ten_vt = existingDmvt.ten_vt,
                    min_ton = existingDmvt.min_ton,
                    max_ton = existingDmvt.max_ton,
                    barcode = existingDmvt.barcode,
                    url = existingDmvt.url,
                    rong = existingDmvt.rong,
                    cao = existingDmvt.cao,
                    khoi_luong = existingDmvt.khoi_luong,
                    mau_sac = existingDmvt.mau_sac,
                    kieu_dang = existingDmvt.kieu_dang,
                    trangthai = existingDmvt.trangthai,
                    mo_ta = existingDmvt.mo_ta,
                    ma_dvt = existingDmvt.ma_dvt
                };

                // Cập nhật dữ liệu mới
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
                await db.SaveChangesAsync();

                var primaryKeyData = new { ma_vt = existingDmvt.ma_vt };
                await auditHelper.SaveAuditLogAsync(
                    tableName: "dmvt",
                    operation: "UPDATE",
                    primaryKeyData: primaryKeyData,
                    oldData: oldData,
                    newData: existingDmvt
                );

                return Ok(new { success = true, result = existingDmvt });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/DmvtAPI/UploadImage")]
        public async Task<IHttpActionResult> UploadImage()
        {
            if (!Request.Content.IsMimeMultipartContent())
                return BadRequest("Unsupported media type");

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                var buffer = await file.ReadAsByteArrayAsync();

                // Tạo đường dẫn lưu file
                var filePath = HttpContext.Current.Server.MapPath("~/Image/" + filename);
                File.WriteAllBytes(filePath, buffer);

                // Trả về URL tương đối để lưu vào DB (dùng để hiển thị ảnh trong web)
                var relativeUrl = "/Image/" + filename;

                return Ok(new { success = true, url = relativeUrl });
            }

            return BadRequest("Không có file nào được upload");
        }
        [HttpGet]
        [Route("api/LOG4API/GetAuditLogByTable")]
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
        WHERE a.table_name = 'dmvt'
        ORDER BY a.changed_at DESC;
    ";

            var result = db.Database.SqlQuery<AuditLogView>(query).ToList();

            return Ok(result);
        }
        [HttpGet]
        [Route("api/LOG4API/GetAuditLogByTableCT")]
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
        WHERE a.table_name = 'dmvt'
        ORDER BY a.changed_at DESC;
    ";

            var result = db.Database.SqlQuery<AuditLogView>(query).ToList();

            return Ok(result);
        }
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WH.DataContext;
using WH.Helpers;
using WH.Models;
using ApplicationDbContext = WH.DataContext.ApplicationDbContext;
namespace WH.Controllers
{
    public class UserInfoAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private AuditHelper auditHelper;

        public UserInfoAPIController()
        {
            auditHelper = new AuditHelper(db);
        }
        [HttpGet]
        [Route("api/UserInfoAPI/GetSessionInfo")]
        public IHttpActionResult GetSessionInfo()
        {
            var (username, role) = auditHelper.GetSessionInfo();

            return Ok(new
            {
                success = true,
                username,
                role
            });
        }


        public IHttpActionResult Get()
        {
            var query = @"
        SELECT  * from userinfo ORDER BY id ASC;";
            // Sử dụng View Model chỉ để hiển thị dũ liệu
            var userinfolist = db.Database.SqlQuery<UserInfoClass>(query).ToList();
            return Ok(userinfolist);
        }
        [HttpPost]
        [Route("api/UserInfoAPI/SEARCH")]
        public IHttpActionResult SEARCH(JObject data)
        {
            var keyword = (string)data["keyword"];
            var id = (string)data["id"];
            var username = (string)data["username"];
            var fullname = (string)data["fullname"];
            var email = (string)data["email"];
            var role = (string)data["role"];
            var trangthai = (string)data["trangthai"];

            var query = @"
        SELECT id, username, fullname,password, email, role, trangthai
        FROM userinfo
        WHERE 1=1";

            if (!string.IsNullOrEmpty(keyword))
                query += $@" AND (
                        username ILIKE '%{keyword}%' OR 
                        fullname ILIKE '%{keyword}%' OR 
                        email ILIKE '%{keyword}%' OR 
                         (CASE 
                        WHEN trangthai = '0' THEN 'Không sử dụng' 
                        WHEN trangthai = '1' THEN 'Sử dụng' 
                        ELSE ''
                         END) ILIKE '%{keyword}%' OR 
                          (CASE 
                        WHEN role = '0' THEN 'Admin' 
                        WHEN role = '1' THEN 'Người dùng' 
                        ELSE ''
                         END) ILIKE '%{keyword}%')";

            if (!string.IsNullOrEmpty(id))
                query += $" AND CAST(id AS TEXT) ILIKE '%{id}%'";

            if (!string.IsNullOrEmpty(username))
                query += $" AND username ILIKE '%{username}%'";

            if (!string.IsNullOrEmpty(fullname))
                query += $" AND fullname ILIKE '%{fullname}%'";

            if (!string.IsNullOrEmpty(email))
                query += $" AND email ILIKE '%{email}%'";

            if (!string.IsNullOrEmpty(role))
                query += $" AND role = {role}";

            if (!string.IsNullOrEmpty(trangthai))
                query += $" AND trangthai = {trangthai}";

            query += " ORDER BY id ASC";

            var result = db.Database.SqlQuery<UserInfoClass>(query).ToList();

            return Ok(new { success = true, result });
        }

        [HttpPost]
        [Route("api/UserInfoAPI/DeleteAll")]
        public async Task<IHttpActionResult> DeleteAll([FromBody] List<UserInfoClass> list)
        {
            if (list == null || !list.Any())
                return BadRequest("Danh sách rỗng");

            foreach (var item in list)
            {
                var _userinfo = db.UserInfoObj.FirstOrDefault(p => p.id == item.id.ToString());
                if (_userinfo != null)
                {
                    var primaryKeyData = new { id = _userinfo.id };

                    // Ghi log trước khi xóa
                    await auditHelper.SaveAuditLogAsync(
                        tableName: "userinfo",
                        operation: "DELETE",
                        primaryKeyData: primaryKeyData,
                        oldData: _userinfo,
                        newData: null
                    );

                    db.UserInfoObj.Remove(_userinfo);
                }
            }
            await db.SaveChangesAsync();
            return Ok("Đã xóa các tài khoản được chọn");
        }

        [HttpPost]
        [Route("api/UserInfoAPI/SaveAdd")]
        public async Task<IHttpActionResult> SaveAdd(JObject data)
        {
            try
            {
                var userInfo = data.ToObject<UserInfoClass>();

                if (userInfo == null)
                    return BadRequest("Dữ liệu không hợp lệ");

                if (db.UserInfoObj.Any(u => u.username == userInfo.username))
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Tên đăng nhập đã tồn tại."
                    });
                }

                var lastUser = db.UserInfoObj
                    .AsEnumerable()
                    .Where(u => int.TryParse(u.id, out _))
                    .OrderByDescending(u => int.Parse(u.id))
                    .Select(u => u.id)
                    .FirstOrDefault();

                userInfo.id = lastUser == null ? "0" : (int.Parse(lastUser) + 1).ToString();
                userInfo.password = PasswordHelper.HashPassword(userInfo.password);
                db.UserInfoObj.Add(userInfo);
                await db.SaveChangesAsync();

                // Ghi log INSERT
                var primaryKeyData = new { id = userInfo.id };
                await auditHelper.SaveAuditLogAsync(
                    tableName: "userinfo",
                    operation: "INSERT",
                    primaryKeyData: primaryKeyData,
                    oldData: null,
                    newData: userInfo
                );

                return Ok(new
                {
                    success = true,
                    result = userInfo
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("api/UserInfoAPI/SaveEdit")]
        public async Task<IHttpActionResult> SaveEdit(JObject data)
        {
            try
            {
                var userInfo = data.ToObject<UserInfoClass>();

                if (userInfo == null || string.IsNullOrEmpty(userInfo.id))
                    return BadRequest("Dữ liệu không hợp lệ");

                var existingUser = db.UserInfoObj.FirstOrDefault(u => u.id == userInfo.id);
                if (existingUser == null)
                    return NotFound();

                if (db.UserInfoObj.Any(u => u.username == userInfo.username && u.id != userInfo.id))
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Tên đăng nhập đã tồn tại."
                    });
                }

                // Lưu dữ liệu cũ để log
                var oldData = new UserInfoClass
                {
                    id = existingUser.id,
                    username = existingUser.username,
                    fullname = existingUser.fullname,
                    password = existingUser.password,
                    email = existingUser.email,
                    role = existingUser.role,
                    trangthai = existingUser.trangthai
                };

                // Cập nhật thông tin
                existingUser.username = userInfo.username;
                existingUser.fullname = userInfo.fullname;
                if (existingUser.password != userInfo.password)
                {
                    existingUser.password = PasswordHelper.HashPassword(userInfo.password);
                }
                existingUser.email = userInfo.email;
                existingUser.role = userInfo.role;
                existingUser.trangthai = userInfo.trangthai;

                db.Entry(existingUser).State = EntityState.Modified;
                await db.SaveChangesAsync();

                // Ghi log UPDATE
                var primaryKeyData = new { id = userInfo.id };
                await auditHelper.SaveAuditLogAsync(
                    tableName: "userinfo",
                    operation: "UPDATE",
                    primaryKeyData: primaryKeyData,
                    oldData: oldData,
                    newData: existingUser
                );

                return Ok(new
                {
                    success = true,
                    result = existingUser
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpGet]
        [Route("api/UserAPI/GetAuditLogByTable")]
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
        WHERE a.table_name = 'userinfo'
        ORDER BY a.changed_at DESC;
    ";

            var result = db.Database.SqlQuery<AuditLogView>(query).ToList();

            return Ok(result);
        }
        [HttpGet]
        [Route("api/UserAPI/GetAuditLogByTableCT")]
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
        WHERE a.table_name = 'userinfo'
        ORDER BY a.changed_at DESC;
    ";

            var result = db.Database.SqlQuery<AuditLogView>(query).ToList();

            return Ok(result);
        }


    }
}

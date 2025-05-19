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
    public class UserInfoAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

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
        public IHttpActionResult DeleteAll([FromBody] List<UserInfoClass> list)
        {
            if (list == null || !list.Any())
                return BadRequest("Danh sách rỗng");

            foreach (var item in list)
            {
                var _userinfo = db.UserInfoObj.FirstOrDefault(p => p.id == item.id.ToString());
                if (_userinfo != null)
                {
                    db.UserInfoObj.Remove(_userinfo);
                }
            }
            db.SaveChanges();
            return Ok("Đã xóa các tài khoản được chọn");
        }
        [HttpPost]
        [Route("api/UserInfoAPI/SaveAdd")]
        public IHttpActionResult SaveAdd(JObject data)
        {
            try
            {
                var userInfo = data.ToObject<UserInfoClass>();

                if (userInfo == null)
                    return BadRequest("Dữ liệu không hợp lệ");

                // Kiểm tra tên đăng nhập đã tồn tại
                if (db.UserInfoObj.Any(u => u.username == userInfo.username))
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Tên đăng nhập đã tồn tại."
                    });
                }

                // Sinh ID tự động (dạng số nhưng lưu dưới dạng chuỗi)
                var lastUser = db.UserInfoObj
                    .AsEnumerable()
                    .Where(u => int.TryParse(u.id, out _))
                    .OrderByDescending(u => int.Parse(u.id))
                    .Select(u => u.id)
                    .FirstOrDefault();

                userInfo.id = lastUser == null ? "0" : (int.Parse(lastUser) + 1).ToString(); 
                db.UserInfoObj.Add(userInfo);
                db.SaveChanges();

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
        public IHttpActionResult SaveEdit(JObject data)
        {
            try
            {
                var userInfo = data.ToObject<UserInfoClass>();

                if (userInfo == null || string.IsNullOrEmpty(userInfo.id))
                    return BadRequest("Dữ liệu không hợp lệ");

                // Tìm bản ghi cũ trong DB
                var existingUser = db.UserInfoObj.FirstOrDefault(u => u.id == userInfo.id);
                if (existingUser == null)
                    return NotFound();

                // Kiểm tra tên đăng nhập đã tồn tại (ngoại trừ bản ghi đang chỉnh sửa)
                if (db.UserInfoObj.Any(u => u.username == userInfo.username && u.id != userInfo.id))
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Tên đăng nhập đã tồn tại."
                    });
                }

                // Cập nhật thông tin
                existingUser.username = userInfo.username;
                existingUser.fullname = userInfo.fullname;
                existingUser.password = userInfo.password;
                existingUser.email = userInfo.email;
                existingUser.role = userInfo.role;
                existingUser.trangthai = userInfo.trangthai;

                db.Entry(existingUser).State = EntityState.Modified;
                db.SaveChanges();

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




    }
}

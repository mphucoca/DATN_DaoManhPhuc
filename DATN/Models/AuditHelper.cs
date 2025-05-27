using System;
using System.Web; // để lấy HttpContext.Current
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using WH.Models;
using ApplicationDbContext = WH.DataContext.ApplicationDbContext;

namespace WH.Helpers
{
    public class AuditHelper
    {
        private readonly ApplicationDbContext _db;

        public AuditHelper(ApplicationDbContext dbContext)
        {
            _db = dbContext;
        }

        // Hàm lấy username từ cookie
        private string GetUsernameFromCookie()
        {
            var cookie = HttpContext.Current?.Request?.Cookies["UserSessionCookie"];
            return cookie?.Value ?? "000000";
        }
        private string GetRoleFromCookie()
        {
            var cookie = HttpContext.Current?.Request?.Cookies["role_session_cookie"];
            return cookie?.Value ?? "1";
        }
        public (string username, string role) GetSessionInfo()
        {
            var username = GetUsernameFromCookie();
            var role = GetRoleFromCookie();
            return (username, role);
        }

        public async Task SaveAuditLogAsync(string tableName, string operation, object primaryKeyData, object oldData, object newData, string changedBy = null)
        {
            // Nếu không truyền changedBy, lấy tự động từ cookie
            if (string.IsNullOrEmpty(changedBy))
            {
                changedBy = GetUsernameFromCookie();
            }

            var audit = new AuditLog
            {
                table_name = tableName,
                operation = operation,
                primary_key_data = JObject.FromObject(primaryKeyData).ToString(),
                old_data = oldData != null ? JObject.FromObject(oldData).ToString() : null,
                new_data = newData != null ? JObject.FromObject(newData).ToString() : null,
                changed_by = changedBy,
                changed_at = DateTime.Now
            };

            _db.AuditLogObj.Add(audit);
            await _db.SaveChangesAsync();
        }
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using WH.DataContext;
using WH.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.Net.Http;
using System.Net.Http.Headers;
using ApplicationDbContext = WH.DataContext.ApplicationDbContext;
using System.IO;
using iText.Kernel.Font;
using iText.IO.Font;
using System.Windows;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using WH.Helpers;
using System.Threading.Tasks;

namespace WH.Controllers
{
    public class AuditLogController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        [Route("api/AuditLog/GetAll")]
        public IHttpActionResult GetAllAuditLogs()
        {
            try
            {
                var result = (from audit in db.AuditLogObj
                              join user in db.UserInfoObj on audit.changed_by equals user.id into userJoin
                              from user in userJoin.DefaultIfEmpty() // LEFT JOIN
                              orderby audit.changed_at descending
                              select new
                              {
                                  audit.id,
                                  audit.table_name,
                                  audit.operation,
                                  audit.primary_key_data,
                                  audit.old_data,
                                  audit.new_data,
                                  audit.changed_at,
                                  changed_by = audit.changed_by,
                                  username = user != null ? user.username : null,
                                  fullname = user != null ? user.fullname : null,
                                  email = user != null ? user.email : null
                              }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }







    }
}

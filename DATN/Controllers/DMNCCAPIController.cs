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
    public class DMNCCAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

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

            if (!string.IsNullOrEmpty(ma_ncc))
                query += $" AND ma_ncc ILIKE '%{ma_ncc}%'";
            if (!string.IsNullOrEmpty(ten_ncc))
                query += $" AND ten_ncc ILIKE '%{ten_ncc}%'";
            if (!string.IsNullOrEmpty(dia_chi))
                query += $" AND dia_chi ILIKE '%{dia_chi}%'";
            if (!string.IsNullOrEmpty(dien_thoai))
                query += $" AND dien_thoai ILIKE '%{dien_thoai}%'";
            if (!string.IsNullOrEmpty(ghi_chu))
                query += $" AND ghi_chu ILIKE '%{ghi_chu}%'";
            if (!string.IsNullOrEmpty(email))
                query += $" AND email ILIKE '%{email}%'";
            query += " ORDER BY ma_ncc ASC";

            var result = db.Database.SqlQuery<DmnccClass>(query).ToList();

            return Ok(new { success = true, result });
        }

        [HttpPost]
        [Route("api/DMNCCAPI/DeleteAll")]
        public IHttpActionResult DeleteAll([FromBody] List<DmnccClass> list)
        {
            if (list == null || !list.Any())
                return BadRequest("Danh sách rỗng");

            foreach (var item in list)
            {
                var ncc = db.DmnccObj.FirstOrDefault(p => p.ma_ncc == item.ma_ncc);
                if (ncc != null)
                {
                    db.DmnccObj.Remove(ncc);
                }
            }
            db.SaveChanges();
            return Ok("Đã xóa các nhà cung cấp được chọn");
        }

        [HttpPost]
        [Route("api/DMNCCAPI/SaveAdd")]
        public IHttpActionResult SaveAdd(JObject data)
        {
            try
            {
                var _dmncc = data.ToObject<DmnccClass>();

                if (_dmncc == null)
                    return BadRequest("Dữ liệu không hợp lệ");

                if (string.IsNullOrEmpty(_dmncc.ten_ncc))
                    return BadRequest("Tên nhà cung cấp không được để trống");

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
                    return Ok(new
                    {
                        success = false,
                        message = "Mã nhà cung cấp đã tồn tại."
                    });
                }

                db.DmnccObj.Add(_dmncc);
                db.SaveChanges();

                return Ok(new
                {
                    success = true,
                    result = _dmncc
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("api/DMNCCAPI/SaveEdit")]
        public IHttpActionResult SaveEdit(JObject data)
        {
            try
            {
                var _dmncc = data.ToObject<DmnccClass>();

                if (_dmncc == null || string.IsNullOrEmpty(_dmncc.ma_ncc))
                    return BadRequest("Dữ liệu không hợp lệ");

                if (string.IsNullOrEmpty(_dmncc.ten_ncc))
                    return BadRequest("Tên nhà cung cấp không được để trống");

                var existingNcc = db.DmnccObj.FirstOrDefault(u => u.ma_ncc == _dmncc.ma_ncc);
                if (existingNcc == null)
                    return NotFound();

                existingNcc.ten_ncc = _dmncc.ten_ncc;
                existingNcc.dia_chi = _dmncc.dia_chi;
                existingNcc.dien_thoai = _dmncc.dien_thoai;
                existingNcc.ghi_chu = _dmncc.ghi_chu;
                existingNcc.email = _dmncc.email; 
                existingNcc.ma_so_thue = _dmncc.ma_so_thue;
                db.SaveChanges();

                return Ok(new
                {
                    success = true,
                    result = existingNcc
                });
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

            return Ok(new
            {
                success = true,
                result = ncc
            });
        }
    }
}

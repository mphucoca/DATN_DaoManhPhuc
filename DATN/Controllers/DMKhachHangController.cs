using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WH.DataContext;
using WH.Models;
using ApplicationDbContext = WH.DataContext.ApplicationDbContext;
namespace WH.Controllers
{
    public class DMKhachHangController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DMKhachHang
        public ActionResult DMKhachHang()
        {
            if (Session["UserName"] == null)
                return RedirectToAction("Login", "LogInOut");

            return View();
        }
 
    }
}

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
    public class BCTonKhoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BCTonKho
        public ActionResult BCTonKho()
        {
            if (Session["UserName"] == null)
                return RedirectToAction("Login", "LogInOut");

            return View();
        }
 
    }
}

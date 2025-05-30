using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WH.Controllers
{
    public class PhieuNhapController : Controller
    {
        // GET: PhieuNhap
        public ActionResult PhieuNhap()
        {
            if (Session["UserName"] == null)
                return RedirectToAction("Login", "LogInOut");

            return View();
        }
    }
}
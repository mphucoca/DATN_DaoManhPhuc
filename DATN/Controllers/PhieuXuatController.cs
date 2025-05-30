using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WH.Controllers
{
    public class PhieuXuatController : Controller
    {
        // GET: PhieuXuat
        public ActionResult PhieuXuat()
        {
            if (Session["UserName"] == null)
                return RedirectToAction("Login", "LogInOut");

            return View();
        }
    }
}
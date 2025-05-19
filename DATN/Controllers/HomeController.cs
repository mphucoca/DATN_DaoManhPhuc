using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WH.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["UserName"] == null) return RedirectToAction("Login", "LogInOut");
            // Lưu session vào cookie
            var userName = Session["ID"].ToString();
            HttpCookie userSessionCookie = new HttpCookie("UserSessionCookie", userName);
            userSessionCookie.Expires = DateTime.Now.AddHours(1); // Đặt thời gian hết hạn cho cookie nếu cần
            Response.Cookies.Add(userSessionCookie);
            return View();
        }

        public ActionResult About()
        {
            if (Session["UserName"] == null) return RedirectToAction("Login", "LogInOut");
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            if (Session["UserName"] == null) return RedirectToAction("Login", "LogInOut");
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
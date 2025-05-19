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
    public class UserInfoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserInfo
        public ActionResult UserInfo()
        {
            if (Session["UserName"] == null || (int)Session["Role"] != 0)
                return RedirectToAction("Login", "LogInOut");

            ViewBag.Total = db.UserInfoObj.Count();
            return View(db.UserInfoObj.ToList());
        }

        // GET: UserInfo/Details/id
        public ActionResult Details(string id)
        {
            if (Session["UserName"] == null || (int)Session["Role"] != 0)
                return RedirectToAction("Login", "LogInOut");

            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var userInfo = db.UserInfoObj.Find(id);
            if (userInfo == null)
                return HttpNotFound();

            return View(userInfo);
        }

        // GET: UserInfo/Create
        public ActionResult Create()
        {
            if (Session["UserName"] == null || (int)Session["Role"] != 0)
                return RedirectToAction("Login", "LogInOut");

            return View();
        }

        // POST: UserInfo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,username,fullname,password,role")] UserInfoClass userInfo)
        {
            if (Session["UserName"] == null || (int)Session["Role"] != 0)
                return RedirectToAction("Login", "LogInOut");

            if (ModelState.IsValid)
            {
                db.UserInfoObj.Add(userInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(userInfo);
        }

        // GET: UserInfo/Edit/id
        public ActionResult Edit(string id)
        {
            if (Session["UserName"] == null || (int)Session["Role"] != 0)
                return RedirectToAction("Login", "LogInOut");

            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var userInfo = db.UserInfoObj.Find(id);
            if (userInfo == null)
                return HttpNotFound();

            return View(userInfo);
        }

        // POST: UserInfo/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,username,fullname,password,role")] UserInfoClass userInfo)
        {
            if (Session["UserName"] == null || (int)Session["Role"] != 0)
                return RedirectToAction("Login", "LogInOut");

            if (ModelState.IsValid)
            {
                db.Entry(userInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userInfo);
        }

        // GET: UserInfo/Delete/id
        public ActionResult Delete(string id)
        {
            if (Session["UserName"] == null || (int)Session["Role"] != 0)
                return RedirectToAction("Login", "LogInOut");

            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var userInfo = db.UserInfoObj.Find(id);
            if (userInfo == null)
                return HttpNotFound();

            return View(userInfo);
        }

        // POST: UserInfo/DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if (Session["UserName"] == null || (int)Session["Role"] != 0)
                return RedirectToAction("Login", "LogInOut");

            var userInfo = db.UserInfoObj.Find(id);
            db.UserInfoObj.Remove(userInfo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

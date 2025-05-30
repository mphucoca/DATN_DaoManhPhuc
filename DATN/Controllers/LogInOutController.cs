using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WH.DataContext;
using WH.Models;
using System.Web.Security; // Thư viện cho Authentication
using ApplicationDbContext = WH.DataContext.ApplicationDbContext;

namespace WH.Controllers
{
    public class LogInOutController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: LogInOut/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: LogInOut/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (Session["UserName"] != null) return RedirectToAction("Index", "Home") ;
            if (ModelState.IsValid)
            {
                string hashedPassword = PasswordHelper.HashPassword(password);

                var user = db.UserInfoObj
                            .FirstOrDefault(u => u.username == username && u.password == hashedPassword);

                if (user != null)
                {
                    Session["UserName"] = user.username;
                    Session["Role"] = user.role;
                    Session["ID"] = user.id;
                    Session["FullName"] = user.fullname; 
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }
            return View();
        }

        // GET: LogInOut/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: LogInOut/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "Id,UserName,FullName,Password,Role")] UserInfoClass userInfo)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tên đăng nhập đã tồn tại chưa
                if (db.UserInfoObj.Any(u => u.username == userInfo.username))
                {
                    ModelState.AddModelError("", "Tên đăng nhập đã tồn tại.");
                    return View(userInfo);
                }
                var lastUser = db.UserInfoObj
                       .AsEnumerable()  // Chuyển dữ liệu về bộ nhớ
                       .Where(u => int.TryParse(u.id, out _))  // Lọc Id có thể chuyển thành số
                       .OrderByDescending(u => int.Parse(u.id))  // Chuyển Id thành số để sắp xếp
                       .Select(u => u.id)  // Chọn cột Id dưới dạng chuỗi
                       .FirstOrDefault();


                if (lastUser==null)
                {
                    userInfo.id = "0";
                }
                else
                {
                    var new_id = int.Parse(lastUser) + 1;
                    userInfo.id = new_id.ToString();
                }
                userInfo.role = 1;// Mặc định phân quyền là nhân viên
                db.UserInfoObj.Add(userInfo);
                db.SaveChanges();

                // Đăng nhập sau khi đăng ký thành công
                System.Web.Security.FormsAuthentication.SetAuthCookie(userInfo.username, false);
                return RedirectToAction("Index", "Home");
            }

            return View(userInfo);
        }

 
        // GET: LogInOut/Logout
        public ActionResult Logout()
        {
            // Xoá mọi Session
            Session.Clear();
            Session.Abandon();

            // Xoá cookie FormsAuthentication
            FormsAuthentication.SignOut();

            // Xoá toàn bộ cookie còn tồn tại
            if (Request.Cookies != null)
            {
                foreach (string cookieName in Request.Cookies.AllKeys)
                {
                    var cookie = new HttpCookie(cookieName)
                    {
                        Expires = DateTime.Now.AddDays(-1),
                        Value = null
                    };
                    Response.Cookies.Add(cookie);
                }
            }

            return RedirectToAction("Login", "LogInOut");
        }

    }
}

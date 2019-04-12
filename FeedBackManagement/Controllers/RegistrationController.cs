using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FeedBackManagement.Models;
using System.Threading;
using System.Web.Security;



namespace FeedBackManagement.Controllers
{
    public class RegistrationController : Controller
    {
        FeedbackEntities db = new FeedbackEntities();



        // GET: Registration

        [Authorize(Roles = "admin")]
        public ActionResult Index(string name, string roles)
        {

            var Lst = new List<string>();

            var Qry = from d in db.users
                      orderby d.role
                      select d.role;
            Lst.AddRange(Qry.Distinct());
            ViewBag.roles = new SelectList(Lst);

            var urlt = from m in db.users
                       select m;
            if (!String.IsNullOrEmpty(name))
            {
                urlt = urlt.Where(s => s.user_name.Contains(name));
            }
            if (!string.IsNullOrEmpty(roles))
            {
                urlt = urlt.Where(x => x.role == roles);
            }



            return View(urlt);

        }
        public ActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(user use)
        {
            //var mail = db.users.Where(x => x.user_name == use.user_name).Where(x => x.password == use.password).Select(x => x.email_id).Max();
            //if (use.password ) 
            //{
            //    ViewBag.Pw = "Password not same";
            //}
            Thread.Sleep(200);
            var precheck = db.users.Where(x => x.user_name == use.user_name).FirstOrDefault();

            if (precheck != null)
            {
                ViewBag.chk = "User Already Exist";
                return View(use);

            }
            else if (ModelState.IsValid)
            {

                use.role = "assign role";
                db.users.Add(use);
                db.SaveChanges();

                //return this.RedirectToAction("Login", "Registration");
            }
            ViewBag.Message = "Contact admin to assign role";
            return View();


        }
        public ActionResult Login()
        {
            //var Fieldlist = new List<string>();
            //var FieldQry = from d in db.fields
            //               orderby d.Id
            //               select d.field_name;
            //Fieldlist.AddRange(FieldQry.Distinct());
            //ViewBag.Fieldview = new SelectList(Fieldlist);

            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(user user, string Fieldview)
        {
            var assgrole = db.users
                 .Where(x => x.user_name == user.user_name)
                 .Where(x => x.password == user.password)
                 .Select(x => x.role).Max();
            if (Fieldview == "-1")
            {
                ViewBag.FieldSelect = "Please Select a Field";
                return View(user);
            }

            else if (assgrole == "assign role")
            {
                ViewBag.assgnRole = "Contact admin to assign role";
                return View(user);

            }

            else if (IsValid(user.user_name, user.password))
            {
                var mail = db.users
                  .Where(x => x.user_name == user.user_name)
                  .Where(x => x.password == user.password)
                  .Select(x => x.email_id).Max();
                string m = mail;



                Session["field"] = Fieldview.ToString();
                //user.email_id = (user.user_name && user.password);


                FormsAuthentication.SetAuthCookie(user.user_name, false);


                return RedirectToAction("ReportEorror", "Feedback");


            }
            else
            {

                //   ModelState.AddModelError("", "Login details are wrong.");
                ViewBag.Mg = "Login details are wrong.";
                return View(user);

            }

            //var userDetail = db.user_account.Where(x => x.user_name == user.user_name && x.password == user.password).Select(x => x.email_id).Max();

            //    ViewBag.Message = "Helle";

            //return View();
        }


        public ActionResult Logout(string returnUrl = null)
        {

            FormsAuthentication.SignOut();

            if (string.IsNullOrWhiteSpace(returnUrl))
                return RedirectToAction("Login", "Registration");

            return Redirect(returnUrl);
        }


        private bool IsValid(string name, string passwords)
        {

            bool IsValid = false;


            var user = db.users.FirstOrDefault(u => u.user_name == name);
            if (user != null)
            {
                if (user.password == passwords)
                {
                    IsValid = true;
                }
            }

            return IsValid;
        }

        public JsonResult Fields()
        {
            try
            {
                return Json(db.fields.Select(x => new
                {
                    DepartmentID = x.Id,
                    DepartmentName = x.field_name
                }).ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return ViewBag.error = ex.Message;
            }
        }

        public ActionResult AssignRole(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user urse = db.users.Find(id);
            if (urse == null)
            {
                return HttpNotFound();
            }

            return View(urse);
        }
        [HttpPost, ActionName("AssignRole")]
        [ValidateAntiForgeryToken]
        public ActionResult AssignRole([Bind(Include = "Id,user_name,email_id,password,role")]Models.user users , string Roles,int? id)
        {
            if (ModelState.IsValid)
            {
                var result = db.users.SingleOrDefault(b => b.Id == id);

                result.role = Roles;
                db.SaveChanges();

            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user urse = db.users.Find(id);
            if (urse == null)
            {
                return HttpNotFound();
            }

            return View(urse);
        }
    }
}
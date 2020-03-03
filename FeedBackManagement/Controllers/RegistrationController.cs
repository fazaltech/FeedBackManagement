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
        [HttpGet]
        public ActionResult Jsonindex(int? id, string roles)
        {



            var urlt = from m in db.users
                       select m;
            if (id != null)
            {
                urlt = urlt.Where(s => s.Id == id);
            }

            if (!string.IsNullOrEmpty(roles))
            {
                urlt = urlt.Where(s => s.role == roles);
            }

            return View(urlt);

        }

        [HttpPost, ActionName("Jsonindex")]
        [ValidateAntiForgeryToken]
        public ActionResult Jsonindex([Bind(Include = "Id,user_name,email_id,password,role")]Models.user users, string Roles, int? id)
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





        public ActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(user use)
        {

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


            }
            ViewBag.Message = "Contact admin to assign role";
            return View();


        }
        public ActionResult Login()
        {


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



                FormsAuthentication.SetAuthCookie(user.user_name, false);


                return RedirectToAction("ReportEorror", "Feedback");


            }
            else
            {


                ViewBag.Mg = "Login details are wrong.";
                return View(user);

            }


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
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult AssignRole([Bind(Include = "Id,user_name,email_id,password,role")]Models.user users, string Roles, int? id)
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

        //public ActionResult JsAssignRole([Bind(Include = "Id,user_name,email_id,password,role")]Models.user users, string Roles, int? id)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = db.users.SingleOrDefault(b => b.Id == id);

        //        result.role = Roles;
        //        db.SaveChanges();

        //    }

        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    user urse = db.users.Find(id);
        //    if (urse == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    return View(urse);
        //}

        public JsonResult Search(string query)
        {


            

            List<Locations> locations = new List<Locations>()
        { 
                
                

            new Locations() {Id = 1, Label = "Common Blackbird",Value = "Common Blackbird",},
            new Locations() {Id = 2, Label = "Locustella naevia",Value = "Locustella naevia",},
            new Locations() {Id = 3, Label = "Locustella naevia",Value = "Locustella naevia",},
            new Locations() {Id = 3, Label = "cat",Value = "cat",}
            //new Locations() {Id = 2, Name = "Walles"},
            //new Locations() {Id = 3, Name = "Birmingham"},
            //new Locations() {Id = 4, Name = "Edinburgh"},
            //new Locations() {Id = 5, Name = "Glasgow"},
            //new Locations() {Id = 6, Name = "Liverpool"},
            //new Locations() {Id = 7, Name = "Bristol"},
            //new Locations() {Id = 8, Name = "Manchester"},
            //new Locations() {Id = 9, Name = "NewCastle"},
            //new Locations() {Id = 10, Name = "Leeds"},
            //new Locations() {Id = 11, Name = "Sheffield"},
            //new Locations() {Id = 12, Name = "Nottingham"},
            //new Locations() {Id = 13, Name = "Cardif"},
            //new Locations() {Id = 14, Name = "Cambridge"},
            //new Locations() {Id = 15, Name = "Bradford"},
            //new Locations() {Id = 16, Name = "Kingston Upon Hall"},
            //new Locations() {Id = 17, Name = "Norwich"},
            //new Locations() {Id = 18, Name = "Conventory"}

            };
            //var b = (from a in locations
            //                 where a.Label.StartsWith(query)
            //                 select new
            //                 {
            //                     label = a.Label,
            //                     val = a.Id
            //                 }).ToList();

            var b = (from N in locations
                            where N.Label.StartsWith(query)
                            select new { N.Label,N.Id });


            // var Loc = locations.Where(x => x.Label.StartsWith(query.ToLower())).Select(x => x.Label).ToList();
            return Json(b.ToList(),JsonRequestBehavior.AllowGet);
        }



    }


}

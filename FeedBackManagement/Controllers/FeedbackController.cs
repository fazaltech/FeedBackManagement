
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using FeedBackManagement.Models;
using System.IO;
using System.Net;

namespace FeedBackManagement.Controllers
{
    public class FeedbackController : Controller
    {

        FeedbackEntities db = new FeedbackEntities();

        // GET: Feedback
        [Authorize]
        public ActionResult ReportEorror()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportEorror(feedback post, HttpPostedFileBase Content)
        {
            var field = Session["field"];
            if (Content == null)
            {
                post.date_time = DateTime.Now;
                post.status = "submitted";
                post.emp_field = field.ToString();
                post.employee_name = User.Identity.Name;
                post.type = "ReportError";
                db.feedbacks.Add(post);
                db.SaveChanges();

            }
            else if (Content != null)
            {
                String FileExt = Path.GetExtension(Content.FileName).ToUpper();

                if (FileExt == ".PNG" || FileExt == ".JPG" && ModelState.IsValid)
                {

                    var fileName = Path.GetFileName(Content.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Upload"), fileName);
                    var con = ("~/Content/Upload/" + fileName);
                    Content.SaveAs(path);


                    post.date_time = DateTime.Now;
                    post.status = "submitted";
                    post.emp_field = field.ToString();
                    post.employee_name = User.Identity.Name;
                    post.filename = fileName;
                    post.filepath = con;
                    post.type = "ReportError";
                    db.feedbacks.Add(post);
                    db.SaveChanges();

                }
            }

            return View();
        }
        [Authorize]
        public ActionResult RequestNewFunction()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestNewFunction(feedback post)
        {
            var field = Session["field"];


            if (ModelState.IsValid)
            {
                post.date_time = DateTime.Now;
                post.status = "submitted";
                post.employee_name = User.Identity.Name;
                post.emp_field = field.ToString();
                post.type = "RequestNewFunction";
                db.feedbacks.Add(post);
                db.SaveChanges();
            }
            return View();
        }
        [Authorize]
        public ActionResult AskQuestion()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AskQuestion(feedback post, HttpPostedFileBase Content)
        {

            var field = Session["field"];
            if (Content == null && ModelState.IsValid)
            {
                post.date_time = DateTime.Now;
                post.status = "submitted";
                post.emp_field = field.ToString();
                post.employee_name = User.Identity.Name;
                post.type = "AskQuestion";
                db.feedbacks.Add(post);
                db.SaveChanges();

            }
            else if (Content != null)
            {
                String FileExt = Path.GetExtension(Content.FileName).ToUpper();

                if (FileExt == ".PNG" || FileExt == ".JPEG" && ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(Content.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Upload"), fileName);
                    var con = ("~/Content/Upload/" + fileName);
                    Content.SaveAs(path);

                    post.date_time = DateTime.Now;
                    post.status = "submitted";
                    post.emp_field = field.ToString();
                    post.filename = fileName;
                    post.employee_name = User.Identity.Name;
                    post.filepath = con;
                    post.type = "AskQuestion";
                    db.feedbacks.Add(post);
                    db.SaveChanges();

                }
            }
            return View();


        }
        [Authorize(Roles = "admin")]
        public ActionResult Index(string typerpt, string searchString, string empname)
        {

            string Username = User.Identity.Name;
            var TypeLst = new List<string>();

            var GenreQry = from d in db.feedbacks
                           orderby d.type
                           select d.type;
            TypeLst.AddRange(GenreQry.Distinct());
            ViewBag.typerpt = new SelectList(TypeLst);

            var fbrpt = from m in db.feedbacks
                        select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                fbrpt = fbrpt.Where(s => s.summary.Contains(searchString));
            }
            if (!String.IsNullOrEmpty(empname))
            {
                fbrpt = fbrpt.Where(s => s.employee_name.Contains(empname));
            }
            if (!string.IsNullOrEmpty(typerpt))
            {
                fbrpt = fbrpt.Where(x => x.type == typerpt);
            }

            return View(fbrpt);
        }
        [Authorize]
        public ActionResult User_Index(string typerpt, string searchString)
        {
            string Username = User.Identity.Name;
            var TypeLst = new List<string>();

            var GenreQry = from d in db.feedbacks
                           orderby d.type
                           select d.type;
            TypeLst.AddRange(GenreQry.Distinct());
            ViewBag.typerpt = new SelectList(TypeLst);


            var fbrpt = from m in db.feedbacks
                        where m.employee_name == Username
                        select m;


            if (!String.IsNullOrEmpty(searchString))
            {
                fbrpt = fbrpt.Where(s => s.summary.Contains(searchString));
            }
            
            if (!string.IsNullOrEmpty(typerpt))
            {
                fbrpt = fbrpt.Where(x => x.type == typerpt);
            }

            return View(fbrpt);
        }
        [Authorize(Roles = "admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            feedback employee = db.feedbacks.Find(id);

            if (employee == null)
            {
                return HttpNotFound();
            }
            var mail = db.feedbacks
               .Where(x => x.Id == employee.Id)
               .Select(x => x.filename).Max();
            var filepath = db.feedbacks
               .Where(x => x.Id == employee.Id)
               .Select(x => x.filepath).Max();
            if (mail != null)
            {
                string path = Server.MapPath(filepath);
                byte[] imageByteData = System.IO.File.ReadAllBytes(path);
                string imageBase64Data = Convert.ToBase64String(imageByteData);
                string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                ViewBag.ImageData = imageDataURL;

            }

            ViewBag.filecontent = mail;

            

            return View(employee);
        }
        [Authorize(Roles = "admin")]
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public ActionResult statuschange([Bind(Include = "Id,type,summary,description,filename,filepath,employee_name,emp_field,date_time,status")]Models.feedback feedbacks, int? id,string stat)
        {
            if (ModelState.IsValid)
            {
                var result = db.feedbacks.SingleOrDefault(b => b.Id == id);

                result.status = stat;
                db.SaveChanges();

            }



            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            feedback employee = db.feedbacks.Find(id);

            if (employee == null)
            {
                return HttpNotFound();
            }
            var mail = db.feedbacks
               .Where(x => x.Id == employee.Id)
               .Select(x => x.filename).Max();
            var filepath = db.feedbacks
               .Where(x => x.Id == employee.Id)
               .Select(x => x.filepath).Max();
            if (mail != null)
            {
                string path = Server.MapPath(filepath);
                byte[] imageByteData = System.IO.File.ReadAllBytes(path);
                string imageBase64Data = Convert.ToBase64String(imageByteData);
                string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                ViewBag.ImageData = imageDataURL;

            }

            ViewBag.filecontent = mail;



            return View(employee);

        }

        public ActionResult UserDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            feedback employee = db.feedbacks.Find(id);

            if (employee == null)
            {
                return HttpNotFound();
            }
            var mail = db.feedbacks
               .Where(x => x.Id == employee.Id)
               .Select(x => x.filename).Max();
            var filepath = db.feedbacks
               .Where(x => x.Id == employee.Id)
               .Select(x => x.filepath).Max();
            if (mail != null)
            {
                string path = Server.MapPath(filepath);
                byte[] imageByteData = System.IO.File.ReadAllBytes(path);
                string imageBase64Data = Convert.ToBase64String(imageByteData);
                string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                ViewBag.ImageData = imageDataURL;

            }

            ViewBag.filecontent = mail;



            return View(employee);
        }
        public ActionResult DownLoad(int? id)
        {
            feedback pic = db.feedbacks.Find(id);
            var content = pic.filepath;
            var name = pic.filename;

            if (content != null)
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(content));

                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, name);
            }
            return null;

        }


    }
}
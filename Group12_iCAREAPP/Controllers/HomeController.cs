using Group12_iCAREAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Group12_iCAREAPP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserPassword objUser)
        {
            if (ModelState.IsValid)
            {
                using (Group12_iCAREDBEntities db = new Group12_iCAREDBEntities())
                {
                    var obj = db.UserPassword.Where(a => a.ID.Equals(objUser.ID) && a.password.Equals(objUser.password)).FirstOrDefault();
                    if (obj != null)
                    {
                        Session["UserID"] = obj.ID.ToString();
                        // Session["UserName"] = obj.userAccountName.ToString();
                        // return RedirectToAction("WelcomeUser");
                        return RedirectToAction("Index", "PatientRecords");
                    }
                }
            }
            return View(objUser);
        }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string UserID, string Password)
        {
            // Check if the user ID and password are provided
            if (!string.IsNullOrEmpty(UserID) && !string.IsNullOrEmpty(Password))
            {
                using (Group12_iCAREDBEntities db = new Group12_iCAREDBEntities())
                {
                    // Attempt to find the user by ID in iCAREUser
                    var user = db.iCAREUser.FirstOrDefault(u => u.ID == UserID);

                    if (user != null)
                    {
                        // Retrieve the password from UserPassword based on the user's passwordID
                        var userPassword = db.UserPassword.FirstOrDefault(up => up.ID == user.passwordID);

                        // Validate the password
                        if (userPassword != null && userPassword.password.Equals(Password))
                        {
                            // Store user information in the session
                            Session["UserID"] = user.ID;
                            //Session["UserName"] = user.name;

                            // Check if the user is an admin or worker
                            bool isAdmin = db.iCAREAdmin.Any(a => a.ID == user.ID);
                            bool isWorker = db.iCAREWorker.Any(w => w.ID == user.ID);

                            // Redirect based on user role
                            if (isAdmin)
                            {
                                return RedirectToAction("Index", "iCAREUsers");
                            }
                            else if (isWorker)
                            {
                                return RedirectToAction("Index", "PatientRecords");
                            }
                        }
                    }
                }
            }

            // If we got this far, something failed; redisplay the form with the current userId
            ViewBag.Message = "Invalid user ID or password.";
            return View();
        }
    }
}
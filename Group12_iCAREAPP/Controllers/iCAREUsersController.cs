using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Group12_iCAREAPP.Models;
using Microsoft.Ajax.Utilities;

namespace Group12_iCAREAPP.Controllers
{
    public class iCAREUsersController : Controller
    {
        private Group12_iCAREDBEntities db = new Group12_iCAREDBEntities();

        // GET: iCAREUsers
        public ActionResult Index()
        {
            /* var users = db.iCAREUser
                          .Where(u => !db.iCAREAdmin.Any(a => a.ID == u.ID))
                          .ToList();*/

            // Retrieve the WORKER users that have not been deleted from iCARE.
            var users = db.iCAREUser
                              .Where(u => !db.iCAREAdmin.Any(a => a.ID == u.ID) && u.name != "_DELETED_")
                              .ToList();

            return View(users);
        }


        // GET: iCAREUsers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            iCAREUser iCAREUser = db.iCAREUser.Find(id);
            if (iCAREUser == null)
            {
                return HttpNotFound();
            }
            return View(iCAREUser);
        }

        // GET: iCAREUsers/Create
        public ActionResult Create()
        {
            //ViewBag.creatorID = new SelectList(db.iCAREAdmin, "ID", "ID");
            ViewBag.creatorID = new SelectList(db.iCAREAdmin
                                    .Where(a => a.iCAREUser.name != "_DELETED_"),
                                    "ID", "ID");

            //ViewBag.roleID = new SelectList(db.iCAREWorker, "ID", "profession");
            ViewBag.roleID = new SelectList(db.UserRole
    .Where(b => b.roleName != "admin")
    .OrderBy(b => b.roleName), "ID", "roleName");
            return View();
        }

        // POST: iCAREUsers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,name,password,passwordID,profession,creatorID,roleID")] iCAREUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //create the UserPassword
                        var userPassword = new UserPassword
                        {
                            password = viewModel.password,

                            //ID = viewModel.passwordID
                            ID = viewModel.ID
                        };

                        db.UserPassword.Add(userPassword);
                        db.SaveChanges();

                        //create the iCAREUser
                        var iCAREUser = new iCAREUser
                        {
                            ID = viewModel.ID,
                            name = viewModel.name,

                            //passwordID = userPassword.ID
                            passwordID = viewModel.ID
                        };

                        db.iCAREUser.Add(iCAREUser);
                        db.SaveChanges();

                        string professionStr;
                        if (viewModel.roleID == "1") professionStr = "doctor";
                        else professionStr = "nurse";

                        //create the iCAREWorker
                        var iCAREWorker = new iCAREWorker
                        {
                            ID = iCAREUser.ID,

                            //profession = viewModel.profession,
                            profession = professionStr,

                            creatorID = viewModel.creatorID,
                            roleID = viewModel.roleID
                        };

                        db.iCAREWorker.Add(iCAREWorker);
                        db.SaveChanges();

                        transaction.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", "An error occurred while creating the user and worker.");
                    }
                }
            }

            //ViewBag.creatorID = new SelectList(db.iCAREAdmin, "ID", "ID", viewModel.creatorID);
            ViewBag.creatorID = new SelectList(db.iCAREAdmin.Where(a => a.iCAREUser.name != "_DELETED_"),"ID", "ID", viewModel.creatorID);
            //ViewBag.roleID = new SelectList(db.iCAREWorker, "ID", "profession", viewModel.roleID);
            ViewBag.roleID = new SelectList(db.UserRole
    .Where(b => b.roleName != "admin")
    .OrderBy(b => b.roleName), "ID", "roleName");
            return View(viewModel);
        }

        // GET: iCAREUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var iCAREUser = db.iCAREUser.Find(id);
            if (iCAREUser == null)
            {
                return HttpNotFound();
            }

            var userPassword = db.UserPassword.Find(iCAREUser.passwordID);
            var iCAREWorker = db.iCAREWorker.Find(iCAREUser.ID);

            var viewModel = new iCAREUserViewModel
            {
                ID = iCAREUser.ID,
                name = iCAREUser.name,
                password = userPassword?.password,
                passwordID = iCAREUser.passwordID,

                //profession = iCAREWorker?.profession,
                profession = iCAREWorker?.UserRole.roleName,

                creatorID = iCAREWorker?.creatorID,
                roleID = iCAREWorker?.roleID
            };

            // Get distinct roles
            var distinctRoles = db.iCAREWorker.Select(w => w.profession).Distinct().ToList();

            //ViewBag.creatorID = new SelectList(db.iCAREAdmin, "ID", "ID", viewModel.creatorID);
            ViewBag.creatorID = new SelectList(db.iCAREAdmin.Where(a => a.iCAREUser.name != "_DELETED_"), "ID", "ID", viewModel.creatorID);
            //ViewBag.roleID = new SelectList(distinctRoles, viewModel.roleID);
            ViewBag.roleID = new SelectList(db.UserRole
    .Where(b => b.roleName != "admin")
    .OrderBy(b => b.roleName), "ID", "roleName");
            return View(viewModel);
        }



        // POST: iCAREUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,name,password,passwordID,profession,creatorID,roleID")] iCAREUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //find the existing iCAREUser
                        var iCAREUser = db.iCAREUser.Find(viewModel.ID);
                        if (iCAREUser == null)
                        {
                            return HttpNotFound();
                        }

                        //update the UserPassword
                        var userPassword = db.UserPassword.Find(iCAREUser.passwordID);
                        if (userPassword != null)
                        {
                            userPassword.password = viewModel.password;
                            db.Entry(userPassword).State = EntityState.Modified;
                        }

                        //update the iCAREUser
                        iCAREUser.name = viewModel.name;
                        db.Entry(iCAREUser).State = EntityState.Modified;

                        //update the iCAREWorker
                        var iCAREWorker = db.iCAREWorker.Find(iCAREUser.ID);
                        if (iCAREWorker != null)
                        {
                            //iCAREWorker.profession = viewModel.profession;

                            if (viewModel.roleID == "1")
                                iCAREWorker.profession = "doctor";
                            else
                                iCAREWorker.profession = "nurse";

                            iCAREWorker.creatorID = viewModel.creatorID;
                            iCAREWorker.roleID = viewModel.roleID;
                            db.Entry(iCAREWorker).State = EntityState.Modified;
                        }

                        //save changes
                        db.SaveChanges();

                        //commit transaction
                        transaction.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        //if any error occurs
                        transaction.Rollback();
                        ModelState.AddModelError("", "An error occurred while updating the user and associated records.");
                    }
                }
            }

            //ViewBag.creatorID = new SelectList(db.iCAREAdmin, "ID", "ID", viewModel.creatorID);
            ViewBag.creatorID = new SelectList(db.iCAREAdmin.Where(a => a.iCAREUser.name != "_DELETED_"), "ID", "ID", viewModel.creatorID);
            //ViewBag.roleID = new SelectList(db.iCAREWorker, "ID", "profession", viewModel.roleID);
            ViewBag.roleID = new SelectList(db.UserRole
    .Where(b => b.roleName != "admin")
    .OrderBy(b => b.roleName), "ID", "roleName");
            return View(viewModel);
        }


        // GET: iCAREUsers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            iCAREUser iCAREUser = db.iCAREUser.Find(id);
            if (iCAREUser == null)
            {
                return HttpNotFound();
            }
            return View(iCAREUser);
        }

        // POST: iCAREUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //find the iCAREUser
                    iCAREUser iCAREUser = db.iCAREUser.Find(id);
                    if (iCAREUser == null)
                    {
                        return HttpNotFound();
                    }

                    //find the associated iCAREWorker
                    /*iCAREWorker iCAREWorker = db.iCAREWorker.Find(id);
                    if (iCAREWorker != null)
                    {
                        db.iCAREWorker.Remove(iCAREWorker);
                    }

                    //find the associated UserPassword
                    UserPassword userPassword = db.UserPassword.Find(iCAREUser.passwordID);
                    if (userPassword != null)
                    {
                        db.UserPassword.Remove(userPassword);
                    }

                    //remove the iCAREUser
                    db.iCAREUser.Remove(iCAREUser);*/


                    // Shadow delete the worker by changing their name.
                    iCAREUser.name = "_DELETED_";  // Mark as deleted
                    db.Entry(iCAREUser).State = EntityState.Modified;


                    //save changes
                    db.SaveChanges();

                    //commit transaction
                    transaction.Commit();
                }
                catch (Exception)
                {
                    //if any error occurs
                    transaction.Rollback();
                    ModelState.AddModelError("", "An error occurred while deleting the user and associated records.");
                    return RedirectToAction("Delete", new { id });
                }
            }

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


// ****** OLD *********
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using Group12_iCAREAPP.Models;

//namespace Group12_iCAREAPP.Controllers
//{
//    public class iCAREUsersController : Controller
//    {
//        private Group12_iCAREDBEntities db = new Group12_iCAREDBEntities();

//        // GET: iCAREUsers
//        public ActionResult Index()
//        {
//            var iCAREUser = db.iCAREUser.Include(i => i.iCAREAdmin).Include(i => i.UserPassword).Include(i => i.iCAREWorker);
//            return View(iCAREUser.ToList());
//        }

//        // GET: iCAREUsers/Details/5
//        public ActionResult Details(string id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            iCAREUser iCAREUser = db.iCAREUser.Find(id);
//            if (iCAREUser == null)
//            {
//                return HttpNotFound();
//            }
//            return View(iCAREUser);
//        }

//        // GET: iCAREUsers/Create
//        public ActionResult Create()
//        {
//            ViewBag.ID = new SelectList(db.iCAREAdmin, "ID", "ID");
//            ViewBag.passwordID = new SelectList(db.UserPassword, "ID", "password");
//            ViewBag.ID = new SelectList(db.iCAREWorker, "ID", "profession");
//            return View();
//        }

//        // POST: iCAREUsers/Create
//        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
//        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create([Bind(Include = "ID,name,passwordID")] iCAREUser iCAREUser)
//        {
//            if (ModelState.IsValid)
//            {
//                db.iCAREUser.Add(iCAREUser);
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }

//            ViewBag.ID = new SelectList(db.iCAREAdmin, "ID", "ID", iCAREUser.ID);
//            ViewBag.passwordID = new SelectList(db.UserPassword, "ID", "password", iCAREUser.passwordID);
//            ViewBag.ID = new SelectList(db.iCAREWorker, "ID", "profession", iCAREUser.ID);
//            return View(iCAREUser);
//        }

//        // GET: iCAREUsers/Edit/5
//        public ActionResult Edit(string id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            iCAREUser iCAREUser = db.iCAREUser.Find(id);
//            if (iCAREUser == null)
//            {
//                return HttpNotFound();
//            }
//            ViewBag.ID = new SelectList(db.iCAREAdmin, "ID", "ID", iCAREUser.ID);
//            ViewBag.passwordID = new SelectList(db.UserPassword, "ID", "password", iCAREUser.passwordID);
//            ViewBag.ID = new SelectList(db.iCAREWorker, "ID", "profession", iCAREUser.ID);
//            return View(iCAREUser);
//        }

//        // POST: iCAREUsers/Edit/5
//        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
//        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit([Bind(Include = "ID,name,passwordID")] iCAREUser iCAREUser)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Entry(iCAREUser).State = EntityState.Modified;
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }
//            ViewBag.ID = new SelectList(db.iCAREAdmin, "ID", "ID", iCAREUser.ID);
//            ViewBag.passwordID = new SelectList(db.UserPassword, "ID", "password", iCAREUser.passwordID);
//            ViewBag.ID = new SelectList(db.iCAREWorker, "ID", "profession", iCAREUser.ID);
//            return View(iCAREUser);
//        }

//        // GET: iCAREUsers/Delete/5
//        public ActionResult Delete(string id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            iCAREUser iCAREUser = db.iCAREUser.Find(id);
//            if (iCAREUser == null)
//            {
//                return HttpNotFound();
//            }
//            return View(iCAREUser);
//        }

//        // POST: iCAREUsers/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(string id)
//        {
//            iCAREUser iCAREUser = db.iCAREUser.Find(id);
//            db.iCAREUser.Remove(iCAREUser);
//            db.SaveChanges();
//            return RedirectToAction("Index");
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}

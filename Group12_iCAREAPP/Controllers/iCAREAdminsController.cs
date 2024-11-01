// ****** NEW *******
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Group12_iCAREAPP.Models;

namespace Group12_iCAREAPP.Controllers
{
    public class iCAREAdminsController : Controller
    {
        private Group12_iCAREDBEntities db = new Group12_iCAREDBEntities();

        // GET: iCAREAdmins
        public ActionResult Index()
        {
            var iCAREAdmin = db.iCAREAdmin.Include(i => i.iCAREUser);
            return View(iCAREAdmin.ToList());
        }

        // GET: iCAREAdmins/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            iCAREAdmin iCAREAdmin = db.iCAREAdmin.Find(id);
            if (iCAREAdmin == null)
            {
                return HttpNotFound();
            }
            return View(iCAREAdmin);
        }

        // GET: iCAREAdmins/Create
        public ActionResult Create()
        {
            ViewBag.ID = new SelectList(db.iCAREUser, "ID", "name");
            return View();
        }


        // POST: iCAREAdmins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,name,password,passwordID,dateHired,dateFinished")] iCAREAdminViewModel viewModel)
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
                            ID = viewModel.passwordID
                        };

                        db.UserPassword.Add(userPassword);
                        db.SaveChanges();

                        //create the iCAREUser
                        var iCAREUser = new iCAREUser
                        {
                            ID = viewModel.ID,
                            name = viewModel.name,
                            passwordID = userPassword.ID
                        };

                        db.iCAREUser.Add(iCAREUser);
                        db.SaveChanges();

                        //create the iCAREAdmin
                        var iCAREAdmin = new iCAREAdmin
                        {
                            ID = iCAREUser.ID,
                            dateHired = viewModel.dateHired,
                            dateFinished = viewModel.dateFinished
                        };

                        db.iCAREAdmin.Add(iCAREAdmin);
                        db.SaveChanges();

                        transaction.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", "An error occurred while creating the admin, user, and password.");
                    }
                }
            }

            ViewBag.ID = new SelectList(db.iCAREUser, "ID", "name", viewModel.ID);
            return View(viewModel);
        }


        // GET: iCAREAdmins/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var iCAREAdmin = db.iCAREAdmin.Find(id);
            if (iCAREAdmin == null)
            {
                return HttpNotFound();
            }

            var iCAREUser = db.iCAREUser.Find(id);
            var userPassword = db.UserPassword.Find(iCAREUser.passwordID);

            var viewModel = new iCAREAdminViewModel
            {
                ID = iCAREAdmin.ID,
                name = iCAREUser.name,
                password = userPassword.password,
                passwordID = iCAREUser.passwordID,
                dateHired = iCAREAdmin.dateHired,
                dateFinished = iCAREAdmin.dateFinished
            };

            ViewBag.ID = new SelectList(db.iCAREUser, "ID", "name", viewModel.ID);
            return View(viewModel);
        }

        // POST: iCAREAdmins/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,name,password,passwordID,dateFinished")] iCAREAdminViewModel viewModel)
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
                        iCAREUser.passwordID = viewModel.passwordID;
                        db.Entry(iCAREUser).State = EntityState.Modified;

                        //update the iCAREAdmin
                        var iCAREAdmin = db.iCAREAdmin.Find(viewModel.ID);
                        if (iCAREAdmin != null)
                        {
                            //do not update dateHired
                            iCAREAdmin.dateFinished = viewModel.dateFinished;
                            db.Entry(iCAREAdmin).State = EntityState.Modified;
                        }

                        //save changes
                        db.SaveChanges();

                        //commit transaction
                        transaction.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        //if any error occurs
                        transaction.Rollback();
                        ModelState.AddModelError("", "An error occurred while updating the admin, user, and password.");
                    }
                }
            }

            ViewBag.ID = new SelectList(db.iCAREUser, "ID", "name", viewModel.ID);
            return View(viewModel);
        }



        // GET: iCAREAdmins/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            iCAREAdmin iCAREAdmin = db.iCAREAdmin.Find(id);
            if (iCAREAdmin == null)
            {
                return HttpNotFound();
            }
            return View(iCAREAdmin);
        }


        // POST: iCAREAdmins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //find the iCAREAdmin
                    var iCAREAdmin = db.iCAREAdmin.Find(id);
                    if (iCAREAdmin == null)
                    {
                        return HttpNotFound();
                    }

                    //find the associated iCAREUser
                    var iCAREUser = db.iCAREUser.Find(id);
                    if (iCAREUser == null)
                    {
                        return HttpNotFound();
                    }

                    //find the associated UserPassword
                    var userPassword = db.UserPassword.Find(iCAREUser.passwordID);
                    if (userPassword != null)
                    {
                        db.UserPassword.Remove(userPassword);
                    }

                    //remove the iCAREUser
                    db.iCAREUser.Remove(iCAREUser);

                    //remove the iCAREAdmin
                    db.iCAREAdmin.Remove(iCAREAdmin);

                    //save changes
                    db.SaveChanges();

                    //commit transaction
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    //if any error occurs
                    transaction.Rollback();
                    ModelState.AddModelError("", "An error occurred while deleting the admin, user, and password.");
                    //log the exception details
                    System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                    System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
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



// ******* OLD *******
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
//    public class iCAREAdminsController : Controller
//    {
//        private Group12_iCAREDBEntities db = new Group12_iCAREDBEntities();

//        // GET: iCAREAdmins
//        public ActionResult Index()
//        {
//            var iCAREAdmin = db.iCAREAdmin.Include(i => i.iCAREUser);
//            return View(iCAREAdmin.ToList());
//        }

//        // GET: iCAREAdmins/Details/5
//        public ActionResult Details(string id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            iCAREAdmin iCAREAdmin = db.iCAREAdmin.Find(id);
//            if (iCAREAdmin == null)
//            {
//                return HttpNotFound();
//            }
//            return View(iCAREAdmin);
//        }

//        // GET: iCAREAdmins/Create
//        public ActionResult Create()
//        {
//            ViewBag.ID = new SelectList(db.iCAREUser, "ID", "name");
//            return View();
//        }

//        // POST: iCAREAdmins/Create
//        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
//        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create([Bind(Include = "ID,dateHired,dateFinished")] iCAREAdmin iCAREAdmin)
//        {
//            if (ModelState.IsValid)
//            {
//                db.iCAREAdmin.Add(iCAREAdmin);
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }

//            ViewBag.ID = new SelectList(db.iCAREUser, "ID", "name", iCAREAdmin.ID);
//            return View(iCAREAdmin);
//        }

//        // GET: iCAREAdmins/Edit/5
//        public ActionResult Edit(string id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            iCAREAdmin iCAREAdmin = db.iCAREAdmin.Find(id);
//            if (iCAREAdmin == null)
//            {
//                return HttpNotFound();
//            }
//            ViewBag.ID = new SelectList(db.iCAREUser, "ID", "name", iCAREAdmin.ID);
//            return View(iCAREAdmin);
//        }

//        // POST: iCAREAdmins/Edit/5
//        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
//        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit([Bind(Include = "ID,dateHired,dateFinished")] iCAREAdmin iCAREAdmin)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Entry(iCAREAdmin).State = EntityState.Modified;
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }
//            ViewBag.ID = new SelectList(db.iCAREUser, "ID", "name", iCAREAdmin.ID);
//            return View(iCAREAdmin);
//        }

//        // GET: iCAREAdmins/Delete/5
//        public ActionResult Delete(string id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            iCAREAdmin iCAREAdmin = db.iCAREAdmin.Find(id);
//            if (iCAREAdmin == null)
//            {
//                return HttpNotFound();
//            }
//            return View(iCAREAdmin);
//        }

//        // POST: iCAREAdmins/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(string id)
//        {
//            iCAREAdmin iCAREAdmin = db.iCAREAdmin.Find(id);
//            db.iCAREAdmin.Remove(iCAREAdmin);
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

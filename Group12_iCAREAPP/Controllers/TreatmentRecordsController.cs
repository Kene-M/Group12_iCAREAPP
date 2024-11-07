using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Group12_iCAREAPP.Models;

namespace Group12_iCAREAPP.Controllers
{
    public class TreatmentRecordsController : Controller
    {
        private Group12_iCAREDBEntities db = new Group12_iCAREDBEntities();

        // GET: TreatmentRecords
        /*public ActionResult Index()
        {
            var treatmentRecord = db.TreatmentRecord.Include(t => t.iCAREWorker).Include(t => t.PatientRecord);
            return View(treatmentRecord.ToList());
        }*/

        public ActionResult Index()
        {
            // Get the current user's ID from the session
            var userId = Session["UserID"]?.ToString();

            // Fetch treatment records that correspond to the user ID
            var treatmentRecord = db.TreatmentRecord
                .Include(t => t.iCAREWorker)
                .Include(t => t.PatientRecord)
                .Where(t => t.workerID == userId); // Filter by user ID

            //return View(treatmentRecord.Select(t => t.patientID).Distinct().ToList()); // Return only unique PatientIDs
            return View(treatmentRecord.ToList());
        }

        public ActionResult FilteredPatientIndex(string id)
        {
            // save the currently viewed patient ID
            Session["PatientID"] = id;

            return RedirectToAction("FilteredPatientIndex", "PatientRecords");
        }

        public ActionResult FilteredTreatmentsIndex(string id)
        {
            // save the currently viewed patient ID
            Session["PatientID"] = id;

            // Fetch all the treatment records that correspond to the patient ID
            var treatmentRecord = db.TreatmentRecord
                .Include(t => t.iCAREWorker)
                .Include(t => t.PatientRecord)
                .Where(t => t.patientID == id); // Filter by user ID

            return View(treatmentRecord.ToList());
        }

        // GET: TreatmentRecords/Details/5
        /*public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TreatmentRecord treatmentRecord = db.TreatmentRecord.Find(id);
            if (treatmentRecord == null)
            {
                return HttpNotFound();
            }
            return View(treatmentRecord);
        }*/
        public ActionResult Details(string workerID, string patientID)
        {
            if (workerID == null || patientID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TreatmentRecord treatmentRecord = db.TreatmentRecord.Find(workerID, patientID);
            if (treatmentRecord == null)
            {
                return HttpNotFound();
            }
            return View(treatmentRecord);
        }

        // GET: TreatmentRecords/Create
        public ActionResult Create()
        {
            ViewBag.workerID = new SelectList(db.iCAREWorker, "ID", "ID");
            ViewBag.patientID = new SelectList(db.PatientRecord, "ID", "ID");
            return View();
        }

        // POST: TreatmentRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,workerID,patientID,description,treatmentDate")] TreatmentRecord treatmentRecord)
        {
            if (ModelState.IsValid)
            {
                db.TreatmentRecord.Add(treatmentRecord);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.workerID = new SelectList(db.iCAREWorker, "ID", "ID", treatmentRecord.workerID);
            ViewBag.patientID = new SelectList(db.PatientRecord, "ID", "ID", treatmentRecord.patientID);
            return View(treatmentRecord);
        }

        // GET: TreatmentRecords/Edit/5
        /*public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TreatmentRecord treatmentRecord = db.TreatmentRecord.Find(id);
            if (treatmentRecord == null)
            {
                return HttpNotFound();
            }
            ViewBag.workerID = new SelectList(db.iCAREWorker, "ID", "profession", treatmentRecord.workerID);
            ViewBag.patientID = new SelectList(db.PatientRecord, "ID", "name", treatmentRecord.patientID);
            return View(treatmentRecord);
        }*/

        public ActionResult Edit(string workerID, string patientID)
        {
            if (workerID == null || patientID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TreatmentRecord treatmentRecord = db.TreatmentRecord.Find(workerID, patientID);
            if (treatmentRecord == null)
            {
                return HttpNotFound();
            }
            //ViewBag.workerID = new SelectList(db.iCAREWorker, "ID", "profession", treatmentRecord.workerID);
            //ViewBag.patientID = new SelectList(db.PatientRecord, "ID", "name", treatmentRecord.patientID);
            return View(treatmentRecord);
        }


        // POST: TreatmentRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,workerID,patientID,description,treatmentDate")] TreatmentRecord treatmentRecord)
        {
            if (ModelState.IsValid)
            {
                db.Entry(treatmentRecord).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.workerID = new SelectList(db.iCAREWorker, "ID", "profession", treatmentRecord.workerID);
            ViewBag.patientID = new SelectList(db.PatientRecord, "ID", "name", treatmentRecord.patientID);
            return View(treatmentRecord);
        }

        public ActionResult Release(string workerID, string patientID)
        {
            if (workerID == null || patientID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Retrieve the TreatmentRecord based on workerID and patientID
            TreatmentRecord treatmentRecord = db.TreatmentRecord.Find(workerID, patientID);

            if (treatmentRecord == null)
            {
                return HttpNotFound();
            }

            // Append the release date to the description
            treatmentRecord.description += " | Released: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            // Mark the entity as modified
            db.Entry(treatmentRecord).State = EntityState.Modified;

            // Save changes to the database
            db.SaveChanges();

            // Redirect to the FilteredTreatmentsIndex after updating the treatment record
            return RedirectToAction("FilteredTreatmentsIndex", new { id = patientID });
        }

        // GET: TreatmentRecords/Delete/5
        /*public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TreatmentRecord treatmentRecord = db.TreatmentRecord.Find(id);
            if (treatmentRecord == null)
            {
                return HttpNotFound();
            }
            return View(treatmentRecord);
        }*/
        public ActionResult Delete(string workerID, string patientID)
        {
            if (workerID == null || patientID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TreatmentRecord treatmentRecord = db.TreatmentRecord.Find(workerID, patientID);
            if (treatmentRecord == null)
            {
                return HttpNotFound();
            }
            return View(treatmentRecord);
        }

        // POST: TreatmentRecords/Delete/5
        /*[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            TreatmentRecord treatmentRecord = db.TreatmentRecord.Find(id);
            db.TreatmentRecord.Remove(treatmentRecord);
            db.SaveChanges();
            return RedirectToAction("Index");
        }*/
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string workerID, string patientID)
        {
            TreatmentRecord treatmentRecord = db.TreatmentRecord.Find(workerID, patientID);
            db.TreatmentRecord.Remove(treatmentRecord);
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

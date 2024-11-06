using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Group12_iCAREAPP.Models;
using Microsoft.Ajax.Utilities;

namespace Group12_iCAREAPP.Controllers
{
    public class PatientRecordsController : Controller
    {
        private Group12_iCAREDBEntities db = new Group12_iCAREDBEntities();

        // GET: PatientRecords
        /*public ActionResult Index()
        {
            var patientRecord = db.PatientRecord.Include(p => p.GeoCodes).Include(p => p.iCAREWorker);
            return View(patientRecord.ToList());
        }*/

        // GET: PatientRecords
        public ActionResult Index(string GeoUnitID)
        {
            ViewBag.geoUnitIDSorted = new SelectList(db.GeoCodes.OrderBy(b => b.description), "ID", "description");

            var patientRecord = db.PatientRecord.Include(p => p.GeoCodes).AsQueryable().Include(p => p.iCAREWorker);

            if (!GeoUnitID.IsNullOrWhiteSpace())
            {
                patientRecord = patientRecord.Where(p => p.geoUnitID == GeoUnitID);
            }

            return View(patientRecord.ToList());
        }

        // Returns a view for
        public ActionResult FilteredPatientIndex()
        {
            // Get the current user's ID from the session
            var patientId = Session["PatientID"]?.ToString();

            // Fetch the patient record that correspond to the patient ID
            var patientRecord = db.PatientRecord
                .Include(t => t.iCAREWorker)
                .Include(t => t.GeoCodes)
                .Where(t => t.ID == patientId); // Filter by current session patient ID*/

            return View(patientRecord);
        }

        // GET: PatientRecords/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientRecord patientRecord = db.PatientRecord.Find(id);
            if (patientRecord == null)
            {
                return HttpNotFound();
            }
            return View(patientRecord);
        }

        // GET: PatientRecords/Create
        public ActionResult Create()
        {
            ViewBag.geoUnitID = new SelectList(db.GeoCodes, "ID", "description");
            ViewBag.maintainWorkerID = new SelectList(db.iCAREWorker, "ID", "ID");
            return View();
        }

        // POST: PatientRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,name,address,dateOfBirth,height,weight,bloodGroup,bedID,treatmentArea,geoUnitID,maintainWorkerID")] PatientRecord patientRecord)
        {
            if (ModelState.IsValid)
            {
                db.PatientRecord.Add(patientRecord);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.geoUnitID = new SelectList(db.GeoCodes, "ID", "description", patientRecord.geoUnitID);
            ViewBag.maintainWorkerID = new SelectList(db.iCAREWorker, "ID", "ID", patientRecord.maintainWorkerID);
            return View(patientRecord);
        }

        // GET: PatientRecords/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientRecord patientRecord = db.PatientRecord.Find(id);
            if (patientRecord == null)
            {
                return HttpNotFound();
            }
            ViewBag.geoUnitID = new SelectList(db.GeoCodes, "ID", "description", patientRecord.geoUnitID);
            ViewBag.maintainWorkerID = new SelectList(db.iCAREWorker, "ID", "ID", patientRecord.maintainWorkerID);
            return View(patientRecord);
        }

        // POST: PatientRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,name,address,dateOfBirth,height,weight,bloodGroup,bedID,treatmentArea,geoUnitID,maintainWorkerID")] PatientRecord patientRecord)
        {
            if (ModelState.IsValid)
            {
                db.Entry(patientRecord).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.geoUnitID = new SelectList(db.GeoCodes, "ID", "description", patientRecord.geoUnitID);
            ViewBag.maintainWorkerID = new SelectList(db.iCAREWorker, "ID", "ID", patientRecord.maintainWorkerID);
            return View(patientRecord);
        }

        // GET: PatientRecords/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientRecord patientRecord = db.PatientRecord.Find(id);
            if (patientRecord == null)
            {
                return HttpNotFound();
            }
            return View(patientRecord);
        }

        // POST: PatientRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PatientRecord patientRecord = db.PatientRecord.Find(id);
            db.PatientRecord.Remove(patientRecord);
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

        /*public ActionResult AssignWorkerToPatient(string patientID)
        {
            // Get the current user's ID, assumed from authentication
            string workerID = Session["UserID"].ToString();

            // Retrieve the patient record from the database
            PatientRecord patient = db.PatientRecord.SingleOrDefault(p => p.ID == patientID);
            try
            {
                if (patient != null)
                {
                    //db.PatientRecord.Find(patientID).maintainWorkerID = workerID;
                    if (patient.maintainWorkerID == workerID)
                    {
                        return RedirectToAction("Index", "PatientRecords"); // return early
                    }

                    // Assign the worker's ID to the maintainWorkerID field
                    patient.maintainWorkerID = workerID;
                    db.Entry(patient).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.Message = "Worker ID added";
                }
                else
                {
                    ViewBag.Message = "Worker failed to be added";
                }
            }
            catch (DbEntityValidationException ex)
            {
                // Log each validation error for diagnosis
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("General error: " + ex.Message);
            }
            return RedirectToAction("Index", "PatientRecords");
        }*/

        /*public ActionResult AssignWorkerToPatient(string patientID)
        {
            // Get the current user's ID, assumed from authentication
            string workerID = Session["UserID"].ToString();
            string roleID = Session["UserRole"].ToString();

            bool canBeAssigned = false;


            //Check for an already existing assignments
            List<TreatmentRecord> treatments = db.TreatmentRecord
                                            .Where(p => p.patientID == patientID)
                                            .ToList();
            List<TreatmentRecord> assignedNurses = new List<TreatmentRecord>();
            List<TreatmentRecord> assignedDoctors = new List<TreatmentRecord>();

            foreach (TreatmentRecord item in treatments)
            {
                //Doctor list
                if (item.description.Equals("Assign") && item.iCAREWorker.roleID == "1")
                {
                    assignedDoctors.Add(item);
                }
                else if (item.description.Equals("Assign") && item.iCAREWorker.roleID == "2")
                {
                    assignedNurses.Add(item);
                }
            }

            if (roleID.Equals("1"))
            {
                //Check for assigned nurse
                if (assignedDoctors.Count == 0) { canBeAssigned = false; }
                else if (assignedNurses.Any()) { canBeAssigned = true; }

            }
            else
            {
                //Check for three assigned nurses
                if (assignedNurses.Count < 3) { canBeAssigned = true; }

            }

            // Retrieve the patient record from the database
            PatientRecord patient = db.PatientRecord.SingleOrDefault(p => p.ID == patientID);
            TreatmentRecord treatmentRecord = new TreatmentRecord();
            try
            {
                if (canBeAssigned)
                {
                    //Check all assign treatments for current worker id


                    // Assign the worker's ID to the maintainWorkerID field
                    patient.maintainWorkerID = workerID;
                    db.Entry(patient).State = EntityState.Modified;

                    treatmentRecord.patientID = patientID;
                    treatmentRecord.iCAREWorker = db.iCAREWorker.Find(workerID);
                    treatmentRecord.treatmentDate = DateTime.Now;
                    treatmentRecord.description = "Assign";
                    treatmentRecord.PatientRecord = patient;

                    db.TreatmentRecord.Add(treatmentRecord);

                    db.SaveChanges();
                    ViewBag.Message = "Worker ID added";
                }
                else
                {
                    ViewBag.Message = "Worker failed to be added";
                }
            }
            catch (DbEntityValidationException ex)
            {
                // Log each validation error for diagnosis
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("General error: " + ex.Message);
            }
            return RedirectToAction("Index", "PatientRecords");
        }*/

        public ActionResult AssignWorkerToPatient(string patientID)
        {
            // Get the current user's ID from the session
            string workerID = Session["UserID"]?.ToString();
            string roleID = Session["UserRole"]?.ToString();

            if (string.IsNullOrEmpty(workerID) || string.IsNullOrEmpty(roleID))
            {
                ViewBag.Message = "Invalid user session.";
                return RedirectToAction("Index", "PatientRecords");
            }

            bool canBeAssigned = false;

            // Retrieve existing treatment records for the patient
            List<TreatmentRecord> treatments = db.TreatmentRecord
                .Where(p => p.patientID == patientID)
                .ToList();

            //List<TreatmentRecord> assignedNurses = treatments
            //    .Where(t => t.description.Equals("Assign") && t.iCAREWorker.roleID == "2")
            //    .ToList();
            List<TreatmentRecord> assignedNurses = treatments
                .Where(t => t.iCAREWorker.roleID == "2")
                .ToList();

            //List<TreatmentRecord> assignedDoctors = treatments
            //    .Where(t => t.description.Equals("Assign") && t.iCAREWorker.roleID == "1")
            //    .ToList();
            List<TreatmentRecord> assignedDoctors = treatments
                .Where(t => t.iCAREWorker.roleID == "1")
                .ToList();

            // Determine if the worker can be assigned
            if (roleID.Equals("1")) // If the worker is a doctor
            {
                //canBeAssigned = assignedDoctors.Count == 0 && assignedNurses.Any();
                canBeAssigned = assignedDoctors.Count == 0 && assignedNurses.Any();
            }
            else // If the worker is a nurse
            {
                //canBeAssigned = assignedNurses.Count < 3;
                canBeAssigned = assignedNurses.Count < 3;
            }

            // Retrieve the patient record
            PatientRecord patient = db.PatientRecord.SingleOrDefault(p => p.ID == patientID);
            if (patient == null)
            {
                ViewBag.Message = "Patient not found.";
                return RedirectToAction("Index", "PatientRecords");
            }

            //try
            //{
                if (canBeAssigned)
                {
                // Update the maintainWorkerID for the patient
                //patient.maintainWorkerID = workerID;
                //db.Entry(patient).State = EntityState.Modified;

                // Create and add a new TreatmentRecord
                TreatmentRecord treatmentRecord = new TreatmentRecord
                {
                    ID = patientID + workerID,
                    patientID = patientID,
                    iCAREWorker = db.iCAREWorker.Find(workerID),
                    treatmentDate = DateTime.Now,
                    description = "Assign",
                    PatientRecord = patient
                };

                    db.TreatmentRecord.Add(treatmentRecord);

                    // Save all changes to the database
                    db.SaveChanges();
                    ViewBag.Message = "Worker successfully assigned to patient.";
                }
                else
                {
                    ViewBag.Message = "Worker cannot be assigned to this patient.";
                }
            //}
            //catch (DbEntityValidationException ex)
            //{
            //    // Log each validation error for diagnosis
            //    foreach (var validationErrors in ex.EntityValidationErrors)
            //    {
            //        foreach (var validationError in validationErrors.ValidationErrors)
            //        {
            //            System.Diagnostics.Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
            //        }
            //    }
            //    ViewBag.Message = "Validation error occurred while assigning worker.";
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine($"General error: {ex.Message}");
            //    ViewBag.Message = "An error occurred while assigning worker.";
            //}

            return RedirectToAction("Index", "PatientRecords");
        }

        //ListAssignedPatients
        /*public ActionResult ListAssignedPatients()
        {
            // Get the current user's ID
            string currentUserID = Session["UserID"].ToString();

            // Retrieve patients assigned to the current user
            var assignedPatients = db.PatientRecord
                                     .Where(p => p.maintainWorkerID == currentUserID)
                                     .ToList();

            return View(assignedPatients);
        }*/
    }
}

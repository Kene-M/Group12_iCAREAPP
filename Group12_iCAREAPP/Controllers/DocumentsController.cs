using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Group12_iCAREAPP.Models;
using Microsoft.Ajax.Utilities;

namespace Group12_iCAREAPP.Controllers
{
    public class DocumentsController : Controller
    {
        private Group12_iCAREDBEntities db = new Group12_iCAREDBEntities();

        // GET: Documents
        /*public ActionResult Index()
        {
            var document = db.Document.Include(d => d.DrugsDictionary).Include(d => d.PatientRecord).Include(d => d.iCAREWorker);
            return View(document.ToList());
        }*/

        public ActionResult Index(string WorkerID, string PatientID)
        {
            ViewBag.workerIDSorted = new SelectList(db.iCAREWorker.OrderBy(b => b.ID), "ID", "ID");
            ViewBag.patientIDSorted = new SelectList(db.PatientRecord.OrderBy(b => b.ID), "ID", "ID");

            var document = db.Document.Include(d => d.PatientRecord).AsQueryable().Include(d => d.iCAREWorker).AsQueryable().Include(d => d.DrugsDictionary);

            if (!WorkerID.IsNullOrWhiteSpace())
            {
                document = document.Where(d => d.workerID == WorkerID);
            }
            if (!PatientID.IsNullOrWhiteSpace())
            {
                document = document.Where(d => d.patientID == PatientID);
            }
            
            return View(document.ToList());
        }

        // GET: Documents/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Document.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // GET: Documents/Create
        public ActionResult Create()
        {
            ViewBag.drugUsedID = new SelectList(db.DrugsDictionary, "ID", "name");
            ViewBag.patientID = new SelectList(db.PatientRecord, "ID", "ID");
            ViewBag.workerID = new SelectList(db.iCAREWorker, "ID", "ID");

            // Pass dropdown options as a ViewBag item
            //ViewBag.DocTypeOptions = new SelectList(new List<string> { "document", "image" });
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,docName,docData,docType,dateOfCreation,drugUsedID,patientID,treatmentDescription,prescription,workerID")] Document document)
        {
            if (ModelState.IsValid)
            {
                db.Document.Add(document);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.drugUsedID = new SelectList(db.DrugsDictionary, "ID", "name", document.drugUsedID);
            ViewBag.patientID = new SelectList(db.PatientRecord, "ID", "name", document.patientID);
            ViewBag.workerID = new SelectList(db.iCAREWorker, "ID", "profession", document.workerID);
            return View(document);
        }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,docName,docType,dateOfCreation,drugUsedID,patientID,treatmentDescription,prescription,workerID")] Document document, HttpPostedFileBase docData)
        {
            if (ModelState.IsValid)
            {
                if (docData != null && docData.ContentLength > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        docData.InputStream.CopyTo(memoryStream);
                        document.docData = memoryStream.ToArray();  // Assign binary data
                        //document.docType = docData.ContentType;     // Assign file type
                    }
                }

                document.dateOfCreation = DateTime.Now; // Add creation date if needed
                db.Document.Add(document);              // Add document to DbSet
                db.SaveChanges();                       // Save changes to database
                return RedirectToAction("Index");
            }
            
            // Repopulate dropdowns if ModelState is invalid
            //ViewBag.DocTypeOptions = new SelectList(new List<string> { "document", "image" });
            ViewBag.drugUsedID = new SelectList(db.DrugsDictionary, "ID", "name", document.drugUsedID);
            ViewBag.patientID = new SelectList(db.PatientRecord, "ID", "ID", document.patientID);
            ViewBag.workerID = new SelectList(db.iCAREWorker, "ID", "ID", document.workerID);
            return View(document);
        }


        // For showing documents
        public ActionResult Display(string id)
        {
            var document = db.Document.Find(id);
            if (document == null || document.docData == null)
            {
                return HttpNotFound();
            }

            // Determine content type based on `docType` field
            string contentType;
            bool isInline = false;

            switch (document.docType)
            {
                case "image":
                    contentType = "image/png"; // Adjust this if images vary in type (jpg, png, etc.)
                    isInline = true;
                    break;
                case "document":
                    contentType = "application/pdf"; // Set to "application/pdf" for PDFs
                    isInline = true;
                    break;
                default:
                    contentType = "application/octet-stream";
                    break;
            }

            // Set inline or attachment based on type
            Response.Headers["Content-Disposition"] = isInline ? "inline" : "attachment";
            return File(document.docData, contentType);
        }

        /*public ActionResult Display(string id)
        {
            var document = db.Document.Find(id);
            if (document == null || document.docData == null)
            {
                return HttpNotFound();
            }

            string fileType = document.docType == "image" ? "image/png" : "application/pdf";
            string disposition = document.docType == "image" ? "inline" : "attachment";

            Response.Headers["Content-Disposition"] = $"{disposition}; filename={document.docName}";
            return File(document.docData, fileType);
        }*/

        // GET: Documents/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Document.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }


            // Assume db is your database context
            /*ViewBag.DrugOptions = db.DrugsDictionary
                .Select(d => new SelectListItem
                {
                    Value = d.ID.ToString(), // store drug ID as Value
                    Text = d.name             // display drug name as Text
                }).ToList();*/


            ViewBag.drugUsedID = new SelectList(db.DrugsDictionary, "ID", "name", document.drugUsedID);
            ViewBag.patientID = new SelectList(db.PatientRecord, "ID", "ID", document.patientID);
            ViewBag.workerID = new SelectList(db.iCAREWorker, "ID", "ID", document.workerID);
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,docName,docData,docType,dateOfCreation,drugUsedID,patientID,treatmentDescription,prescription,workerID")] Document document)
        {
            if (ModelState.IsValid)
            {
                db.Entry(document).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.drugUsedID = new SelectList(db.DrugsDictionary, "ID", "name", document.drugUsedID);
            ViewBag.patientID = new SelectList(db.PatientRecord, "ID", "ID", document.patientID);
            ViewBag.workerID = new SelectList(db.iCAREWorker, "ID", "ID", document.workerID);
            return View(document);
        }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, Document document, HttpPostedFileBase docFile)
        {
            // Ensure only drugs from the db have been entered
            var validDrugIDs = db.DrugsDictionary.Select(d => d.ID).ToList();

            if (!validDrugIDs.Contains(document.drugUsedID))
            {
                ModelState.AddModelError("drugUsedID", "Invalid drug selected.");
            }

            if (ModelState.IsValid)
            {
                var existingDocument = db.Document.Find(id);

                // Update fields
                existingDocument.docName = document.docName;
                existingDocument.docType = document.docType;
                existingDocument.dateOfCreation = document.dateOfCreation;
                existingDocument.drugUsedID = document.drugUsedID;
                existingDocument.patientID = document.patientID;
                existingDocument.treatmentDescription = document.treatmentDescription;
                existingDocument.prescription = document.prescription;
                existingDocument.workerID = document.workerID;

                // If a new file is uploaded, replace the existing document data
                if (docFile != null && docFile.ContentLength > 0)
                {
                    //using (var binaryReader = new BinaryReader(docFile.InputStream))
                    //{
                    //    existingDocument.docData = binaryReader.ReadBytes(docFile.ContentLength);
                    //}
                    using (var memoryStream = new MemoryStream())
                    {
                        docFile.InputStream.CopyTo(memoryStream);
                        existingDocument.docData = memoryStream.ToArray();  // Assign binary data
                        //document.docType = docData.ContentType;     // Assign file type
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }


            // Repopulate the options if the model is invalid
            /*ViewBag.DrugOptions = db.DrugsDictionary
                .Select(d => new SelectListItem
                {
                    Value = d.ID.ToString(),
                    Text = d.name
                }).ToList();*/


            ViewBag.drugUsedID = new SelectList(db.DrugsDictionary, "ID", "name", document.drugUsedID);
            ViewBag.patientID = new SelectList(db.PatientRecord, "ID", "ID", document.patientID);
            ViewBag.workerID = new SelectList(db.iCAREWorker, "ID", "ID", document.workerID);

            return View(document);
        }

        // GET: Documents/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Document.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Document document = db.Document.Find(id);
            db.Document.Remove(document);
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

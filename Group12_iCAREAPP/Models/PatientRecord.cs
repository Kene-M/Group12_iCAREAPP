//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Group12_iCAREAPP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class PatientRecord
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PatientRecord()
        {
            this.Document = new HashSet<Document>();
            this.TreatmentRecord = new HashSet<TreatmentRecord>();
        }
    
        public string ID { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public System.DateTime dateOfBirth { get; set; }
        public Nullable<decimal> height { get; set; }
        public Nullable<decimal> weight { get; set; }

        [StringLength(3)]
        public string bloodGroup { get; set; }
        public string bedID { get; set; }
        public string treatmentArea { get; set; }
        public string geoUnitID { get; set; }
        public string maintainWorkerID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Document> Document { get; set; }
        public virtual GeoCodes GeoCodes { get; set; }
        public virtual iCAREWorker iCAREWorker { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TreatmentRecord> TreatmentRecord { get; set; }
    }
}

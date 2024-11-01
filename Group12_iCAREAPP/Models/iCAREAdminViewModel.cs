using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Group12_iCAREAPP.Models
{
    public class iCAREAdminViewModel
    {
        public string ID { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public string passwordID { get; set; }
        public DateTime? dateHired { get; set; }
        public DateTime? dateFinished { get; set; }
    }

}
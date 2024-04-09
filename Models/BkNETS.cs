using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBCS_Web_Portal.Models
{
    public class BkNETSReportRequest
    {
        public int UserID { get; set; }
        public int CityID { get; set; }
        public int CycleNo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int InstID { get; set; }
        public string RepName { get; set; }
        public string BDelivered { get; set; }
    }
}
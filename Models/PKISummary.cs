using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBCS_Web_Portal.Models
{
    public class PKISummary
    {
    }
    public class DongleReport
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DeptID { get; set; }
        public int DesigID { get; set; }
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }
        public int IsCharged { get; set; }
        public int AuditID { get; set; }
        public int InstID { get; set; }
        public int BranchID { get; set; }
        public int IsPwdChanged { get; set; }
        public int IsBranchUser { get; set; }
        public int Locality { get; set; }
        public string Title { get; set; }
        public string CertificateStatus { get; set; }
        public string Country { get; set; }
        public string CreationDateTime { get; set; }
        public int IsAuth { get; set; }
        public string BranchName { get; set; }
        public string TelNo3 { get; set; }
    }

    public class PKISummaryRequest
    {
        public int BankId { get; set; }
    }
    public class PKISummaryResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<DongleReport> reports { get; set; }
    }
}
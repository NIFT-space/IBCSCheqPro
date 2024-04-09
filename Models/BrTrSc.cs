using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBCS_Web_Portal.Models
{
    public class Params_Rep
    {
        public string DDL_Bank { get; set; }
        public string DDL_City { get; set; }
        public string DDL_Cycle { get; set; }
    }
    public class Params_Rep2
    {
        public string DDL_Bank { get; set; }
        public string DDL_City { get; set; }
        public string DDL_Cycle { get; set; }
        public string DDL_BankType { get; set; }
    }
    public class Params_Rep3
    {
        public string DDL_Bank { get; set; }
        public string DDL_City { get; set; }
        public string DDL_Cycle { get; set; }
        public string DDL_Type { get; set; }
        public string DDL_BType { get; set; }
    }
    public class BrReportRequest
    {
        public int UserID { get; set; }
        public int CityID { get; set; }
        public int CycleNo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int InstID { get; set; }
        public string RepName { get; set; }
        public string URL { get; set; }
    }
    public class BrReport
    {
        public int BUpdated { get; set; }
        public int RepId { get; set; }
        public string RepName { get; set; }
        public string RepDesc { get; set; }
        public int BDownload { get; set; }
        public string RepSize { get; set; }
        public string RepType { get; set; }
        public string FileUpdateDate { get; set; }
        public string FileUpdateTime { get; set; }
        public string ReploadTime { get; set; }
        public string RepModified { get; set; }
        public int ToCityCode { get; set; }
        public string CityName { get; set; }
        public string RepCreationDate { get; set; }
        public string RepPath { get; set; }
        public string InstName { get; set; }
        public string BranchName { get; set; }
    }
    public class BrReportResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<BrReport> reports { get; set; }
    }
    public class IntercityBrReportRequest
    {
        public int UserID { get; set; }
        public int CityID { get; set; }
        public int CycleNo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int InstID { get; set; }
        public string RepName { get; set; }
        public int BDelivered { get; set; }
    }
}
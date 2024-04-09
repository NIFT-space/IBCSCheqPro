using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBCS_Web_Portal.Models
{
    public class InstRequest
    {
        public string UserId { get; set; }
        public string InstId { get; set; }

    }
    public class BkReport
    {
        public int BUpdated { get; set; }
        public int RepId { get; set; }
        public string RepName { get; set; }
        public string RepDesc { get; set; }
        public string BDownload { get; set; }
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
    }
    public class ReportCount
    {
        public int RepId { get; set; }
    }
    public class BkHVCountRequest
    {
        public int cycleNo { get; set; }
        public string dtFrom { get; set; }
        public string dtTo { get; set; }
        public string repName { get; set; }
    }

    public class BkHVCountResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<ReportCount> count { get; set; }
    }
    public class BkReportRequest
    {
        public int UserID { get; set; }
        public int CityID { get; set; }
        public int CycleNo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int InstID { get; set; }
        public string RepName { get; set; }
    }
    public class BkIntercityReportRequest
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
    public class BkReportResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<BkReport> reports { get; set; }
    }
    public class BkReportFile
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public byte[] file { get; set; }
    }
}
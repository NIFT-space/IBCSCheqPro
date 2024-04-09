using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBCS_Web_Portal.Models
{
    public class InstBank
    {
        public int InstId { get; set; }
        public string InstName { get; set; }
    }

    public class InstBankRequest
    {
        public int InstId { get; set; }
    }

    public class InstBankResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<InstBank> bkAdvice { get; set; }
    }


    public class Cycle
    {
        public int CycleNo { get; set; }
        public string CycleDesc { get; set; }
        public int Sorder { get; set; }
    }

    public class CycleResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<Cycle> cycle { get; set; }
    }



    public class ReportFile
    {
        public int BUpdated { get; set; }
        public int RepId { get; set; }
        public string RepName { get; set; }
        public string RepDesc { get; set; }
        public string RepSize { get; set; }
        public string RepType { get; set; }
        public string BDownload { get; set; }
        public string ReploadTime { get; set; }
        public string RepModified { get; set; }
        public string CycleDesc { get; set; }
        public int ToCityCode { get; set; }
        public string CityName { get; set; }
        public string RepCreationDate { get; set; }
        public string RepPath { get; set; }
        public string InstName { get; set; }
        public string FileUpdateDate { get; set; }
        public string FileUpdateTime { get; set; }
        public string FFormat { get; set; }
        public string BranchName { get; set; }
    }

    public class ReportFileRequest
    {
        public int User_InstID { get; set; }
        public int CityID { get; set; }
        public int CycleNo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int InstID { get; set; }
        public string RepName { get; set; }
    }

    public class ReportFileBranchRequest
    {
        public int UserID { get; set; }
        public int CityID { get; set; }
        public int CycleNo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int InstID { get; set; }
    }

    public class ReportFileResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<ReportFile> reportFile { get; set; }
    }


    public class ReportResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public string report { get; set; }
    }
    public class ReportRequest
    {
        public int iRepID { get; set; }
    }


    public class Hub
    {
        public int instid { get; set; }
        public string instname { get; set; }
        public string hubcode { get; set; }
        public string hubname { get; set; }
    }
    public class HubRequest
    {
        public int CityID { get; set; }
        public int InstID { get; set; }
        public string BankCode { get; set; }
    }
    public class HubResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<Hub> hub { get; set; }
    }

    public class HubFiles
    {
        public int HubImgId { get; set; }
        public string HubImgName { get; set; }
        public string ImgPath { get; set; }
        public string ImgSize { get; set; }
        public string ImgLoadTime { get; set; }
        public int InstId { get; set; }
        public int HubCode { get; set; }
        public string InstName { get; set; }
        public int BDownload { get; set; }
        public string HubName { get; set; }
        public string FSize { get; set; }
        public string InstId_s { get; set; }
        public string FileUpdateDate { get; set; }
        public string FileUpdateTime { get; set; }

    }
    public class HubFileRequest
    {
        public int InstID { get; set; }
        public int HubCode { get; set; }
        public int CycleNo { get; set; }
        public string DtFrom { get; set; }
        public string DtTo { get; set; }
    }
    public class HubFileResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<HubFiles> hubFiles { get; set; }
    }

    public class HubFileDownLogRequest
    {
        public string UserId { get; set; }
        public int UserLogID { get; set; }
        public int ImgID { get; set; }
    }
    public class HubFileDownLogResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
    }
    public class HubImgFileRequest
    {
        public int ImgID { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace IBCS_Core_Web_Portal.Models
{
	public interface ICL
	{
	}
    public class UploadICLLogResponse2
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<ICLLog2> logs { get; set; }
    }
    public class ICLLog2
    {
        public int hubimgid { get; set; }
        public string hubimgname { get; set; }
        public string imgpath { get; set; }
        public long imgsize { get; set; }
        public int instid { get; set; }
        public int hubcode { get; set; }
        public string instname { get; set; }
        public string hubname { get; set; }
        public string fsize { get; set; }
        public string Fileupdatedate { get; set; }
        public string Fileupdatetime { get; set; }
        public int Cycle_no { get; set; }
        public string cycle_desc { get; set; }
    }
    public class UploadICLLogRequest
    {
        public int InstID { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
    public class UploadICLLogResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<ICLLog> logs { get; set; }
    }
    public class ICLLog
    {
        public int LogId { get; set; }
        public int CycleNo { get; set; }
        public string FileName { get; set; }
        public int WebId { get; set; }
        public int BankId { get; set; }
        public int BranchId { get; set; }
        public string UserEmail { get; set; }
        public string FileUpdateDate { get; set; }
        public string FileUpdateTime { get; set; }
        public string CycleDesc { get; set; }
        public string CityName { get; set; }
        public string username { get; set; }
    }
    public class ICLDataFileRequest
    {
        public int fileId { get; set; }
    }
    public class ICLDataFileResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public ICLDataFile ICLDataFile { get; set; }
    }
    public class ICLDataFile
    {
        public int WebID { get; set; }
        public int bankID { get; set; }
        public int branchID { get; set; }
        public string userEmail { get; set; }
        public int fileID { get; set; }
        public string fileName { get; set; }
        public string contentType { get; set; }
        public byte[] fileData { get; set; }
    }
    public class UploadICLDataRequest
    {
        public int InstID { get; set; }
        public int BranchID { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
    public class UploadICLDataResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<ICLData> data { get; set; }
    }
    public class ICLData
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public int BankId { get; set; }
        public int BranchId { get; set; }
        public int CycleNo { get; set; }
        public string CycleDesc { get; set; }
        public int CityCode { get; set; }
        public string CityName { get; set; }
        public int WebId { get; set; }
        public string RecDateTime { get; set; }
        public string UserEmail { get; set; }
        public string FileUpdateDate { get; set; }
        public string FileUpdateTime { get; set; }
        public string InstName { get; set; }
        public string BranchName { get; set; }
    }
    public class ICLDataResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public int isUploaded { get; set; }
    }
    public class ICLLogRequest
    {
        public string fileName { get; set; }
        public string contentType { get; set; }
        public int bankId { get; set; }
        public int branchId { get; set; }
        public int cycleNo { get; set; }
        public int cityCode { get; set; }
        public int webId { get; set; }
        public string userEmail { get; set; }
        public string recDateTime { get; set; }
        public int validFlag { get; set; }
    }
    public class ICLDataRequest
    {
        public string fileName { get; set; }
        public string contentType { get; set; }
        public int bankId { get; set; }
        public int branchId { get; set; }
        public int cycleNo { get; set; }
        public int cityCode { get; set; }
        public int webId { get; set; }
        public string userEmail { get; set; }
        public int validFlag { get; set; }
        public byte[] fileData { get; set; }
    }
    public class CycleClose
    {
        public int CycleNo { get; set; }
        public string CycleDesc { get; set; }
        public string CycleStartTime { get; set; }
        public string CycleEndTime { get; set; }
    }

    public class IsCycleCloseRequest
    {
        public int CycleNo { get; set; }
        public int InstId { get; set; }
        public int CityId { get; set; }
    }
    public class IsCycleCloseResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<CycleClose> cycles { get; set; }
    }
    public class Response_OECHubFile_2
    {
        public string responseMessage { get; set; }
        public string responseCode { get; set; }
        public List<Response_OECHUBFile_List> lstOECHub { get; set; }
    }
    public class Response_OECHUBFile_List
    {
        public string hubimgid { get; set; }
        public string hubimgname { get; set; }
        public string imgpath { get; set; }
        public string imgsize { get; set; }
        public string ImgLoadTime { get; set; }
        public string instid { get; set; }
        public string hubcode { get; set; }
        public string instname { get; set; }
        public string InOutTag { get; set; }
        public string hubname { get; set; }
        public string fsize { get; set; }
        public string Fileupdatedate { get; set; }
        public string Fileupdatetime { get; set; }
        public int instid_int { get; set; }
        public string cityname { get; set; }
        public string cityid { get; set; }

    }
    public class NIBC_HUB_ICL_DownloadRequest
    {
        [Required]
        public string dtFrom { get; set; }
        [Required]
        public string dtTo { get; set; }
        [Required]
        public string rInstID { get; set; }
        //[Required]
        //public string rHubCode { get; set; }
        [Required]
        public string CycleNo { get; set; }
        [Required]
        public string PageName { get; set; }
        [Required]
        public string sUserId { get; set; }
        [Required]
        public string DDL_City { get; set; }
        [Required]
        public string DDL_BankType { get; set; }
    }
    public class NIBC_ICL_Download_ImgRequest
    {
        [Required]
        public string imgID { get; set; }
        [Required]
        public string sUserId { get; set; }
        [Required]
        public string userlogid { get; set; }
    }
    public class Response_OECFile_2
	{
		public string responseMessage { get; set; }
		public string responseCode { get; set; }
		public List<Response_OECFile_List> lstOEC { get; set; }
	}

	public class NIBC_ICL_DownloadRequest
	{
		[Required]
		public string dtFrom { get; set; }
		[Required]
		public string dtTo { get; set; }
		[Required]
		public string DDL_City { get; set; }
		[Required]
		public string DDL_Cycle { get; set; }
		[Required]
		public string DDL_Bank { get; set; }
		[Required]
		public string PageName { get; set; }
		[Required]
		public string sUserId { get; set; }
	}
	public class Response_OECFile_List
	{
		public string ImgID { get; set; }
		public string ImgName { get; set; }
		public string ImgPath { get; set; }
		public string ImgSize { get; set; }
		public string ImgLoadTime { get; set; }
		public string InstID { get; set; }
		public string BranchID { get; set; }
		public string InstName { get; set; }
		public string branch_name { get; set; }
		public string fsize { get; set; }
		public string RecBankBranch { get; set; }
		public string Fileupdatedate { get; set; }
		public string Fileupdatetime { get; set; }
		public string imgcreationdate { get; set; }
		public string TScroll { get; set; }
		public string FromCityCode { get; set; }
		public string cityname { get; set; }
		public string cityid { get; set; }

	}
}

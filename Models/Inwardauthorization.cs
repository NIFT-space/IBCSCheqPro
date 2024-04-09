using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBCS_Web_Portal.Models
{
    public class BindBanksDDResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<BindBanksReturnResponse> GetBanks { get; set; }

    }
    public class BindBanksDD
    {
        public int sInstId { get; set; }
        // public string instname { get; set; }
    }

    public class BindBanksReturnResponse
    {
        public int sInstId { get; set; }
        public string instname { get; set; }
    }


    public class BranchboardRequest
    {
        public string BankCode { get; set; }
        public int CityId { get; set; }
        public int BankId { get; set; }
        public int UserId { get; set; }

    }
    public class BranchDD
    {
        public int bankID { get; set; }

        public string bankName { get; set; }
        public int branchID { get; set; }

        public string branchName { get; set; }
    }

    public class BindCityDDResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<CityDDResponse> BindCity { get; set; }

    }
    public class CityDDRequest
    {
        public string sUserId { get; set; }

    }
    public class CityDDResponse
    {
        public int cityid { get; set; }
        public string cityname { get; set; }
    }



    public class BindCycleDDResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<CycleDDResponse> BindCycle { get; set; }

    }

    public class CycleDDResponse
    {
        public int cycle_no { get; set; }
        public string cycle_desc { get; set; }
        public int sorder { get; set; }

    }

    public class GetBranchDDResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<BranchDD> GetBranch { get; set; }

    }


    public class GetBranchRequest
    {
        public int brID { get; set; }
    }

    public class LoadDataFromDBResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<LoadDataFromDBReturn> LoadData { get; set; }

    }
    public class LoadDataAuthDBRequest
    {
        public string pdate { get; set; }

        public string bkcode { get; set; }
        public string citycode { get; set; }

        public string cycleno { get; set; }
    }
    public class LoadDataDBRequest
    {
        public string pdate { get; set; }

        public string bkcode { get; set; }
        public string citycode { get; set; }

        public string cycleno { get; set; }
        public string bkType { get; set; }
    }

    public class LoadDataFromDBReturn
    {
        public int bankID { get; set; }
        public string bankName { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public string CycleID { get; set; }
        public string Cycle_desc { get; set; }
        public int ClgItems { get; set; }
        public string ClgAmount { get; set; }
        public long Rtn_Items { get; set; }
        public int Rtn_Amount { get; set; }
        public int CityCode { get; set; }
        public string CityName { get; set; }
        public int Pay { get; set; }
        public string PayAmount { get; set; }
        public int NonPayCount { get; set; }
        public string NonPayAmount { get; set; }
        public int Reject { get; set; }
        public string RejectAmount { get; set; }
        public int NoAction { get; set; }
        public string NoActionAmount { get; set; }
        public int Authorize { get; set; }
        public string AuthorizeAmount { get; set; }
        public int NonAuth { get; set; }
        public string NonAuthAmount { get; set; }
        public int Defer { get; set; }
        public int DeferAmount { get; set; }

    }

    public class updateAuthorizeDB
    {
        public string sHostID { get; set; }
    }
    public class updateAuthorizeDBResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public bool AuthorizeDB { get; set; }

    }


    //public class updateAuthorizeDBRequest
    //{
    //    public string hostid { get; set; }
    //    public string AuthBy { get; set; }
    //    public string isAuth { get; set; }
    //    public string SetBy { get; set; }

    //}
    public class updateUnAuthorizeAllResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public updateUnAuthorize UnAuthorizeAll { get; set; }
    }


    //public class updateeUnAuthorizeDBRequest
    //{
    //    public string sUserId { get; set; }
    //    public string SetBy { get; set; }
    //    public string DDL_Bank { get; set; }
    //    public string ssbrid { get; set; }
    //    public string DDL_Cycle { get; set; }

    //}

    public class updateUnAuthorize
    {
        public int DDL_Cycle { get; set; }
    }



    public class updateRejectDBResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public bool UpdateReject { get; set; }
    }

    public class updateRejectDBRequest
    {
        public string sHostID { get; set; }
        public string bIsSet { get; set; }
        public string sUserId { get; set; }
    }

    public class UpdateRejectResponse
    {
        public int SPResponse { get; set; }
    }



    public class updateRejectAllResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public updateRejectAllReturn updateRejectAll { get; set; }
    }


    //public class updateRejectAllRequest
    //{
    //    public string sUserId { get; set; }
    //    public string ddlBankValue { get; set; }
    //    public string ssbrid { get; set; }
    //    public string ddlCycleValue { get; set; }

    //}

    public class updateRejectAllReturn
    {
        public int SpOutput { get; set; }

    }

    public class IsCycleCloResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<IsCycleCloseReturn> IsCycleClose { get; set; }
    }


    public class IsCycleCloRequest
    {
        public string cycno { get; set; }
        public string bkcode { get; set; }
        public string cityid { get; set; }

    }

    public class IsCycleCloseReturn
    {
        public string cycleNo { get; set; }
        public string cycle_desc { get; set; }
        public string cycleStartTime { get; set; }
        public string cycleEndTime { get; set; }


    }

    public class insertReturnLogResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public bool instRtnLog { get; set; }
    }
    public class CheckSendForReviewResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<CheckSendForReviewReturn> SendForReview { get; set; }
    }


    public class CheckSendForReviewRequest
    {
        public string hostid { get; set; }
        public string pdate { get; set; }
        public string bkcode { get; set; }
        public string brcode { get; set; }
        public string citycode { get; set; }
        public string cyclecode { get; set; }

    }

    public class CheckSendForReviewReturn
    {
        public int hosID { get; set; }
        public int ProID { get; set; }
        public DateTime capturedate { get; set; }
        public int runno { get; set; }
        public int batchno { get; set; }
        public int siteid { get; set; }
        public int sorterno { get; set; }
        public int dinno { get; set; }
        public int senderbank { get; set; }
        public int senderbranch { get; set; }
        public int receiverbank { get; set; }
        public int receiverbranch { get; set; }
        public int chqno { get; set; }
        public string accno { get; set; }
        public string amt { get; set; }
        public int trancode { get; set; }
        public string onus { get; set; }
        public int idxno { get; set; }
        public int cycleno { get; set; }
        public DateTime hosttime { get; set; }
        public int ffilepointer { get; set; }
        public int ffilesize { get; set; }
        public int bfilepointer { get; set; }
        public int bfilesize { get; set; }
        public int HostCoverID { get; set; }
        public int NonPrime { get; set; }
        public int UVfilepointer { get; set; }
        public int UVfilesize { get; set; }
        public int FakeTag { get; set; }
        public int Undersize_Image { get; set; }
        public int Folded_or_Torn_Document_Corners { get; set; }
        public int Folded_or_Torn_Document_Edges { get; set; }
        public int Framing_Error { get; set; }
        public int Document_Skew { get; set; }
        public int Oversize_image { get; set; }
        public int Piggy_Back { get; set; }
        public int Image_Too_Light { get; set; }
        public int Image_Too_Dark { get; set; }
        public int Horizontal_Streaks { get; set; }
        public int Below_Minimum_Compressed_Image_Size { get; set; }
        public int Above_Maximum_Compressed_Image_Size { get; set; }
        public int Spot_Noise { get; set; }
        public int Front_Rear_Dimension_Mismatch { get; set; }
        public int Carbon_Strip { get; set; }
        public int Out_of_Focus { get; set; }
        public int IQATag { get; set; }
        public int barcodeMatch { get; set; }
        public int UVStr { get; set; }
        public int Duplicate { get; set; }
        public int Average_Amount { get; set; }
        public int Clg_Chq_Count { get; set; }
        public int MICR_Present { get; set; }
        public int STD_Non_STD { get; set; }
        public int Water_Mark { get; set; }
        public int Status { get; set; }
        public int ReasonID { get; set; }
        public string Reason { get; set; }
        public DateTime TruncTime { get; set; }
        public int TruncBy { get; set; }
        public int Settled { get; set; }
        public DateTime SetTime { get; set; }
        public int SetBy { get; set; }
        public int AuthBy { get; set; }
        public DateTime AuthDateTime { get; set; }
        public int isAuth { get; set; }
        public int Rtn_Chq_Count { get; set; }
        public int ReasonID2 { get; set; }
        public int Reason2 { get; set; }

        public int ReasonID3 { get; set; }
        public int Reason3 { get; set; }
        public string Comment1 { get; set; }
        public string Comment2 { get; set; }
        public string Comment3 { get; set; }
        public string isDeffer { get; set; }
        public string Comment1By { get; set; }
        public string Comment2By { get; set; }
        public string Comment3By { get; set; }
        public DateTime Comment1Date { get; set; }
        public DateTime Comment2Date { get; set; }
        public DateTime Comment3Date { get; set; }

        public string bDownload { get; set; }
        public int isFraud { get; set; }
        public string TemplateID { get; set; }
        public string UVPercent { get; set; }

    }

    public class ReturnCheckIsAuthorizedResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<CheckSendForReviewReturn> CheckIsAuthorized { get; set; }
    }


    public class ReturnCheckIsAuthorizedRequest
    {
        public DateTime pdate { get; set; }
        public int bkcode { get; set; }
        public int brcode { get; set; }
        public int citycode { get; set; }

    }


    public class InwardAuthorizationResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<InwardAuthorizationReturn> InwAuthorization { get; set; }
    }



    public class InwardAuthorizationRequest
    {
        public string pdate { get; set; }
        public string bkcode { get; set; }
        public string brcode { get; set; }
        public string citycode { get; set; }
        public string cycleid { get; set; }
    }

    public class InwardAuthorizationReturn
    {
        public int hostID { get; set; }
        public int SBkId { get; set; }
        public string SBkNm { get; set; }
        public int SBrId { get; set; }
        public string SBrNm { get; set; }
        public int RBkId { get; set; }
        public string RBkNm { get; set; }
        public int RBrId { get; set; }
        public string RBrNm { get; set; }
        public int TrCode { get; set; }
        public int CycleId { get; set; }
        public string CycleDesc { get; set; }
        public int ChqNo { get; set; }
        public string AccNo { get; set; }
        public int TrId { get; set; }
        public int IWOWID { get; set; }
        public string ClgType { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int Amount { get; set; }
        public string IQA { get; set; }
        public string UV { get; set; }
        public string Bar { get; set; }
        public string MICR { get; set; }
        public string Std { get; set; }
        public string Dup { get; set; }
        public string WMark { get; set; }
        public int AvgChqSize { get; set; }
        public int TChq { get; set; }
        public int TCqR { get; set; }
        public int reasonid { get; set; }
        public string Reason { get; set; }
        public DateTime TruncTime { get; set; }
        public int TruncBy { get; set; }
        public int Settled { get; set; }
        public DateTime SetTime { get; set; }
        public int SetBy { get; set; }
        public int AuthBy { get; set; }
        public DateTime AuthDateTime { get; set; }
        public int IsAuth { get; set; }
        public string Undersize_Image { get; set; }
        public string Folded_or_Torn_Document_Corners { get; set; }
        public string Folded_or_Torn_Document_Edges { get; set; }
        public string Framing_Error { get; set; }
        public string Document_Skew { get; set; }
        public string Oversize_Image { get; set; }
        public string Piggy_Back { get; set; }
        public string Image_Too_Light { get; set; }
        public string Image_Too_Dark { get; set; }
        public string Horizontal_Streaks { get; set; }
        public string Below_Minimum_Compressed_Image_Size { get; set; }
        public string Above_Maximum_Compressed_Image_Size { get; set; }
        public string Spot_Noise { get; set; }
        public string Front_Rear_Dimension_Mismatch { get; set; }
        public string Carbon_Strip { get; set; }
        public string Out_of_Focus { get; set; }
        public int ReasonID2 { get; set; }
        public int Reason2 { get; set; }
        public int ReasonID3 { get; set; }
        public int Reason3 { get; set; }
        public string Comment1 { get; set; }
        public string Comment2 { get; set; }
        public string Comment3 { get; set; }
        public string idDeffer { get; set; }
        public string Comment1By { get; set; }
        public string Comment2By { get; set; }
        public string Comment3By { get; set; }
        public DateTime Comment1date { get; set; }

        public DateTime Comment2date { get; set; }
        public DateTime Comment3date { get; set; }


    }

    public class SingleInwardAuth
    {
        public int hostID { get; set; }
        public int SBkId { get; set; }
        public string SBkNm { get; set; }
        public int SBrId { get; set; }
        public string SBrNm { get; set; }
        public int RBkId { get; set; }
        public string RBkNm { get; set; }
        public int RBrId { get; set; }
        public string RBrNm { get; set; }
        public int TrCode { get; set; }
        public int CycleId { get; set; }
        public string CycleDesc { get; set; }
        public int ChqNo { get; set; }
        public string AccNo { get; set; }
        public int TrId { get; set; }
        public int IWOWID { get; set; }
        public string ClgType { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int Amount { get; set; }
        public string IQA { get; set; }
        public string UV { get; set; }
        public string Bar { get; set; }
        public string MICR { get; set; }
        public string Std { get; set; }
        public string Dup { get; set; }
        public string WMark { get; set; }
        public int AvgChqSize { get; set; }
        public int TChq { get; set; }
        public string TChqR { get; set; }
        public string reasonid { get; set; }
        public string Reason { get; set; }
        public DateTime TruncTime { get; set; }
        public int TruncBy { get; set; }
        public int Settled { get; set; }
        public DateTime SetTime { get; set; }
        public int SetBy { get; set; }
        public int AuthBy { get; set; }
        public DateTime AuthDateTime { get; set; }
        public int IsAuth { get; set; }
        public string Undersize_Image { get; set; }
        public string Folded_or_Torn_Document_Corners { get; set; }
        public string Folded_or_Torn_Document_Edges { get; set; }
        public string Framing_Error { get; set; }
        public string Document_Skew { get; set; }
        public string Oversize_Image { get; set; }
        public string Piggy_Back { get; set; }
        public string Image_Too_Light { get; set; }
        public string Image_Too_Dark { get; set; }
        public string Horizontal_Streaks { get; set; }
        public string Below_Minimum_Compressed_Image_Size { get; set; }
        public string Above_Maximum_Compressed_Image_Size { get; set; }
        public string Spot_Noise { get; set; }
        public string Front_Rear_Dimension_Mismatch { get; set; }
        public string Carbon_Strip { get; set; }
        public string Out_of_Focus { get; set; }
        public int ReasonID2 { get; set; }
        public int Reason2 { get; set; }
        public int ReasonID3 { get; set; }
        public int Reason3 { get; set; }
        public string Comment1 { get; set; }
        public string Comment2 { get; set; }
        public string Comment3 { get; set; }
        public string isDeffer { get; set; }
        public string Comment1By { get; set; }
        public string Comment2By { get; set; }
        public string Comment3By { get; set; }
        public DateTime Comment1date { get; set; }
        public DateTime Comment2date { get; set; }
        public DateTime Comment3date { get; set; }


    }

    public class updateAuthorizeDBRequest
    {
        public string hostid { get; set; }
        public string AuthBy { get; set; }
        public DateTime AuthDateTime { get; set; }
        public string isAuth { get; set; }
        public string SetBy { get; set; }
        public DateTime SetTime { get; set; }
        public int Settled { get; set; }

    }


    public class updateeUnAuthorizeDBRequest
    {
        public string sUserId { get; set; }
        public string SetBy { get; set; }
        public string DDL_Bank { get; set; }
        public string ssbrid { get; set; }
        public string DDL_Cycle { get; set; }

    }
    public class updateAuthorizeAllDBRequest
    {
        public string sUserId { get; set; }
        public string DDL_Bank { get; set; }
        public string ssbrid { get; set; }
        public string DDL_Cycle { get; set; }

    }


    public class updateRejectAllRequest
    {
        public string sUserId { get; set; }
        public string DDL_Bank { get; set; }
        public string ssbrid { get; set; }
        public string DDL_Cycle { get; set; }

    }

    public class CheckSendReturn
    {
        public int hostID { get; set; }
        public int ProID { get; set; }
        public DateTime capturedate { get; set; }
        public int runno { get; set; }
        public int batchno { get; set; }
        public int siteid { get; set; }
        public int sorterno { get; set; }
        public int dinno { get; set; }
        public int senderbank { get; set; }
        public int senderbranch { get; set; }
        public int receiverbank { get; set; }
        public int receiverbranch { get; set; }
        public int chqno { get; set; }
        public string accno { get; set; }
        public int amt { get; set; }
        public int trancode { get; set; }
        public string onus { get; set; }
        public int idxno { get; set; }
        public int cycleno { get; set; }
        public DateTime hosttime { get; set; }
        public int ffilepointer { get; set; }
        public int ffilesize { get; set; }
        public int bfilepointer { get; set; }
        public int bfilesize { get; set; }
        public int HostCoverID { get; set; }
        public int NonPrime { get; set; }
        public int UVfilepointer { get; set; }
        public int UVfilesize { get; set; }
        public int FakeTag { get; set; }
        public int Undersize_Image { get; set; }
        public int Folded_or_Torn_Document_Corners { get; set; }
        public int Folded_or_Torn_Document_Edges { get; set; }
        public int Framing_Error { get; set; }
        public int Document_Skew { get; set; }
        public int Oversize_image { get; set; }
        public int Piggy_Back { get; set; }
        public int Image_Too_Light { get; set; }
        public int Image_Too_Dark { get; set; }
        public int Horizontal_Streaks { get; set; }
        public int Below_Minimum_Compressed_Image_Size { get; set; }
        public int Above_Maximum_Compressed_Image_Size { get; set; }
        public int Spot_Noise { get; set; }
        public int Front_Rear_Dimension_Mismatch { get; set; }
        public int Carbon_Strip { get; set; }
        public int Out_of_Focus { get; set; }
        public int IQATag { get; set; }
        public int barcodeMatch { get; set; }
        public int UVStr { get; set; }
        public int Duplicate { get; set; }
        public int Average_Amount { get; set; }
        public int Clg_Chq_Count { get; set; }
        public int MICR_Present { get; set; }
        public int STD_Non_STD { get; set; }
        public int Water_Mark { get; set; }
        public int Status { get; set; }
        public int ReasonID { get; set; }
        public string Reason { get; set; }
        public DateTime TruncTime { get; set; }
        public int TruncBy { get; set; }
        public int Settled { get; set; }
        public DateTime SetTime { get; set; }
        public int SetBy { get; set; }
        public int AuthBy { get; set; }
        public DateTime AuthDateTime { get; set; }
        public int isAuth { get; set; }
        public int Rtn_Chq_Count { get; set; }
        public int ReasonID2 { get; set; }
        public int Reason2 { get; set; }

        public int ReasonID3 { get; set; }
        public int Reason3 { get; set; }
        public string Comment1 { get; set; }
        public string Comment2 { get; set; }
        public string Comment3 { get; set; }
        public string isDeffer { get; set; }
        public string Comment1By { get; set; }
        public string Comment2By { get; set; }
        public string Comment3By { get; set; }
        public DateTime Comment1Date { get; set; }
        public DateTime Comment2Date { get; set; }
        public DateTime Comment3Date { get; set; }

        public string bDownload { get; set; }
        public int isFraud { get; set; }
        public string TemplateID { get; set; }
        public string UVPercent { get; set; }

    }
}
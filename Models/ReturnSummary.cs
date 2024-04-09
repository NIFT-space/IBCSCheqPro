using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBCS_Web_Portal.Models
{
    public class SummeryResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<SummeryReturnResponse> SummeryInward { get; set; }
    }
    public class SummeryReturnRequest
    {
        public DateTime pdate { get; set; }
        public int bkcode { get; set; }
        public int citycode { get; set; }
        public int cycleno { get; set; }

    }
    public class SummeryReturnResponse
    {
        public int BankID { get; set; }
        public string BankName { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public int CycleID { get; set; }
        public string cycle_desc { get; set; }
        public int ClgItems { get; set; }
        public int ClgAmount { get; set; }
        public int Rtn_Amount { get; set; }
        public int CityCode { get; set; }
        public string CityName { get; set; }
        public int Pay { get; set; }
        public int PayAmount { get; set; }
        public int NonPayAmount { get; set; }
        public int Reject { get; set; }
        public int RejectAmount { get; set; }
        public int NoAction { get; set; }
        public string NoActionAmount { get; set; }
        public int Authorize { get; set; }
        public int AuthorizeAmount { get; set; }
        public int NonAuth { get; set; }
        public int NonAuthAmount { get; set; }
        public int Defer { get; set; }
        public int DeferAmount { get; set; }

    }
    public class Transaction_WI_isAuthSumm
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<TransactionisAuthSummRetrun> TransactionisAuthSumm { get; set; }
    }
    public class TransactionisAuthSumRequest
    {
        public string pdate { get; set; }
        public string bkcode { get; set; }
        public string brcode { get; set; }
        public string citycode { get; set; }
        public string cyclecode { get; set; }
        public string bkType { get; set; }

    }
    public class TransactionisAuthSummRetrun
    {
        public string hostID { get; set; }
        public string SBkId { get; set; }
        public string SBkNm { get; set; }
        public string SBrId { get; set; }
        public string SBrNm { get; set; }
        public string RBkId { get; set; }
        public string RBkNm { get; set; }
        public string RBrId { get; set; }
        public string RBrNm { get; set; }
        public string TrCode { get; set; }
        public string CycleId { get; set; }
        public string CycleDesc { get; set; }
        public string ChqNo { get; set; }
        public string AccNo { get; set; }
        public string TrId { get; set; }
        public string IWOWID { get; set; }
        public string ClgType { get; set; }
        public string CityId { get; set; }
        public string CityName { get; set; }
        public string Amount { get; set; }
        public string IQA { get; set; }
        public string UV { get; set; }
        public string Bar { get; set; }
        public string MICR { get; set; }
        public string Std { get; set; }
        public string Dup { get; set; }
        public string WMark { get; set; }
        public string AvgChqSize { get; set; }
        public string TChq { get; set; }
        public string TChqR { get; set; }
        public string TemplateID { get; set; }
        public string UVperc { get; set; }
        public string ReasonID { get; set; }
        public string Reason { get; set; }
        public string TruncTime { get; set; }
        public string TruncBy { get; set; }
        public int Settled { get; set; }
        public string SetTime { get; set; }
        public string SetBy { get; set; }
        public string AuthBy { get; set; }
        public string AuthDateTime { get; set; }
        public int isAuth { get; set; }
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
        public string ReasonID2 { get; set; }
        public string Reason2 { get; set; }
        public string ReasonID3 { get; set; }
        public string Reason3 { get; set; }
        public string Comment1 { get; set; }
        public string Comment2 { get; set; }
        public string Comment3 { get; set; }
        public string isDeffer { get; set; }
        public string Comment1By { get; set; }
        public string Comment2By { get; set; }
        public string Comment3By { get; set; }
        public string Comment1Date { get; set; }
        public string Comment2Date { get; set; }
        public string Comment3Date { get; set; }

    }

    public class RSLoadDataFromDBResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<RSLoadDFrDBResponse> RSLoadData { get; set; }
    }
    public class RSLoadDataFromDBRequest
    {
        public string sUserId { get; set; }
        public string sCycleCode { get; set; }
        public string bkCode { get; set; }
        public string CityCode { get; set; }
    }
    public class RSLoadDFrDBResponse
    {
        public string InstID { get; set; }
        public string InstName { get; set; }
        public string branchid { get; set; }
        public string branch_name { get; set; }

    }
    public class MainTale
    {
        public int pCnt { get; set; }
        public decimal pAmt { get; set; }
        public int nCnt { get; set; }
        public decimal nAmt { get; set; }
        public int aCnt { get; set; }
        public decimal aAmt { get; set; }
        public int rCnt { get; set; }
        public decimal rAmt { get; set; }
        public int dCnt { get; set; }
        public decimal dAmt { get; set; }
    }

    public class Mapped
    {
        //public int hostID { get; set; }
        //public int TrCode { get; set; }
        //public string SBkNm { get; set; }
        //public string SBrNm { get; set; }
        //public int AccNo { get; set; }
        //public int ChqNo { get; set; }
        //public string Std { get; set; }
        //public decimal Amount { get; set; }
        public string hostID { get; set; }
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
        public int AccNo { get; set; }
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
        public int TChqR { get; set; }
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
        public int reasonid { get; set; }
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
    }
}
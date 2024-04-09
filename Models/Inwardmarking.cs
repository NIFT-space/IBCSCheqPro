using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBCS_Web_Portal.Models
{

    public class BindReturnRequest
    {
        public string def_cycno { get; set; }
        public string def_bkcode { get; set; }
        public string def_cityid { get; set; }
        public string dt_ { get; set; }
    }

    public class BindReturn
    {
        public int ResonID { get; set; }
        public string ReasonDesc { get; set; }
        public int IsActive { get; set; }
    }



    public class BindReturnDDResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<BindReturnResponse> BindReturn { get; set; }
    }

    public class BindReturnResponse
    {
        public int ResonID { get; set; }
        public string ReasonDesc { get; set; }
        public int IsActive { get; set; }
    }
    //public class BindReturnResponse
    //{
    //    public string responseCode { get; set; }
    //    public string responseMessage { get; set; }
    //    public List<BindReturn> RtnBank { get; set; }
    //}

    public class TransactionwiseInwardResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<TransactionwiseInwardReturn> TransactionWInward { get; set; }
    }
    public class TransactionwiseInwardRequest
    {
        public string pdate { get; set; }
        public string bkcode { get; set; }
        public string brcode { get; set; }
        public string citycode { get; set; }
        public string cyclecode { get; set; }

    }
    public class TransactionwiseInwardReturn
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
        public int AvgChqSize { get; set; }
        public int TChq { get; set; }
        public int TChqR { get; set; }
        public string Tempid { get; set; }
        public string Uvperc { get; set; }
        public int ReasonID { get; set; }
        public string Reason { get; set; }
        public string TruncTime { get; set; }
        public string TruncBy { get; set; }
        public int Settled { get; set; }
        public string SetTime { get; set; }
        public int SetBy { get; set; }
        public int AuthBy { get; set; }
        public string AuthDateTime { get; set; }
        public int isAuth { get; set; }
        public string Undersize_Image { get; set; }
        public string Folded_or_Torn_Document_Corners { get; set; }
        public string Folded_or_Torn_Document_Edges { get; set; }
        public string Framing_Error { get; set; }
        public string Document_Skew { get; set; }
        public string Oversize_image { get; set; }
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
        public int reasonid_inward { get; set; }
        public int ReasonID3 { get; set; }
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
        public int bDownload { get; set; }

        public int aCount { get; set; }
        public decimal aAmount { get; set; }
        public int sCount { get; set; }
        public decimal sAmount { get; set; }
    }
    public class SingleReturn
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
        public int UVPercent { get; set; }

    }


    //public class BindReturnDDResponse
    //{
    //    public string responseCode { get; set; }
    //    public string responseMessage { get; set; }
    //    public List<BindReturnResponse> BindReturn { get; set; }
    //}

    //public class BindReturnDDRequest
    //{
    //    public string CycleValue { get; set; }
    //    public string CityValue { get; set; }
    //    public string def_cycno { get; set; }
    //    public string def_bkcode { get; set; }
    //    public string def_cityid { get; set; }
    //}
    //public class BindReturnResponse
    //{
    //    public int ResonID { get; set; }
    //    public int ReasonDesc { get; set; }
    //    public int IsActive { get; set; }
    //}
    public class updateReturnDBResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public bool updateReturn { get; set; }
    }

    //public class MarkReturn_ClickResponse
    //{
    //    public string responseCode { get; set; }
    //    public string responseMessage { get; set; }
    //    public List<BranchDD> updateRejectAll { get; set; }
    //}

    //public class BindBranchDDResponse
    //{
    //    public string responseCode { get; set; }
    //    public string responseMessage { get; set; }
    //    public List<BranchDD> GetBranches { get; set; }
    //}

    public class updateReturnDBRequest
    {
        public string sHostID { get; set; }
        public string sUserId { get; set; }
        public string sReasonid { get; set; }
        public string sReason { get; set; }
    }
	public class insertReturnLogRequest
	{
		public string sUserId { get; set; }
		public string sHostID { get; set; }
		public string sDetail { get; set; }
        public string reasonid {  get; set; }

	}
	public class dtsum_
    {
        public int aCount { get; set; }
        public decimal aAmount { get; set; }
    }
    public class dtsum2_
    {
        public int sCount { get; set; }
        public decimal sAmount { get; set; }
    }

    //public class IsDefferAllowedReturn
    //{
    //    public string cycleNo { get; set; }
    //    public string cycle_desc { get; set; }
    //    public DateTime cycleStartTime { get; set; }
    //    public DateTime cycleEndTime { get; set; }

    //}
    //public class TransactionwiseInwardResponse
    //{
    //    public string responseCode { get; set; }
    //    public string responseMessage { get; set; }
    //    public List<TransactionwiseInwardReturn> TransactionWInward { get; set; }
    //}
    //public class TransactionwiseInwardRequest
    //{
    //    public string pdate { get; set; }
    //    public string bkcode { get; set; }
    //    public string brcode { get; set; }
    //    public string citycode { get; set; }
    //    public string cyclecode { get; set; }

    //}
    //public class TransactionwiseInwardReturn
    //{
    //    public string hostID { get; set; }
    //    public string SBkId { get; set; }
    //    public string SBkNm { get; set; }
    //    public string SBrId { get; set; }
    //    public string SBrNm { get; set; }
    //    public string RBkId { get; set; }
    //    public string RBkNm { get; set; }
    //    public string RBrId { get; set; }
    //    public string RBrNm { get; set; }
    //    public string TrCode { get; set; }
    //    public string CycleId { get; set; }
    //    public string CycleDesc { get; set; }
    //    public string ChqNo { get; set; }
    //    public string AccNo { get; set; }
    //    public string TrId { get; set; }
    //    public string IWOWID { get; set; }
    //    public string ClgType { get; set; }
    //    public string CityId { get; set; }
    //    public string CityName { get; set; }
    //    public string Amount { get; set; }
    //    public string IQA { get; set; }
    //    public string UV { get; set; }
    //    public string Bar { get; set; }
    //    public string MICR { get; set; }
    //    public string Std { get; set; }
    //    public string Dup { get; set; }
    //    public string WMark { get; set; }
    //    public int AvgChqSize { get; set; }
    //    public int TChq { get; set; }
    //    public int TChqR { get; set; }
    //    public int ReasonID { get; set; }
    //    public string Reason { get; set; }
    //    public string TruncTime { get; set; }
    //    public string TruncBy { get; set; }
    //    public int Settled { get; set; }
    //    public string SetTime { get; set; }
    //    public int SetBy { get; set; }
    //    public int AuthBy { get; set; }
    //    public string AuthDateTime { get; set; }
    //    public int isAuth { get; set; }
    //    public string Undersize_Image { get; set; }
    //    public string Folded_or_Torn_Document_Corners { get; set; }
    //    public string Folded_or_Torn_Document_Edges { get; set; }
    //    public string Framing_Error { get; set; }
    //    public string Document_Skew { get; set; }
    //    public string Oversize_image { get; set; }
    //    public string Piggy_Back { get; set; }
    //    public string Image_Too_Light { get; set; }
    //    public string Image_Too_Dark { get; set; }
    //    public string Horizontal_Streaks { get; set; }
    //    public string Below_Minimum_Compressed_Image_Size { get; set; }
    //    public string Above_Maximum_Compressed_Image_Size { get; set; }
    //    public string Spot_Noise { get; set; }
    //    public string Front_Rear_Dimension_Mismatch { get; set; }
    //    public string Carbon_Strip { get; set; }
    //    public string Out_of_Focus { get; set; }
    //    public int ReasonID2 { get; set; }
    //    public int Reason2 { get; set; }
    //    public int reasonid_inward { get; set; }
    //    public int ReasonID3 { get; set; }
    //    public string Reason3 { get; set; }
    //    public string Comment1 { get; set; }
    //    public string Comment2 { get; set; }
    //    public string Comment3 { get; set; }
    //    public string isDeffer { get; set; }
    //    public string Comment1By { get; set; }
    //    public string Comment2By { get; set; }
    //    public string Comment3By { get; set; }
    //    public string Comment1Date { get; set; }
    //    public string Comment2Date { get; set; }
    //    public string Comment3Date { get; set; }
    //    public int bDownload { get; set; }


    //}
    //public class InsertReturnLogResponse
    //{
    //    public string responseCode { get; set; }
    //    public string responseMessage { get; set; }
    //    public List<BranchDD> InsertReturn { get; set; }
    //}

    //public class InsertReturnLogRequest
    //{
    //    public string sHostID { get; set; }
    //    public string sDetail { get; set; }
    //}
    //public class BindBranchResponse
    //{
    //    public string responseCode { get; set; }
    //    public string responseMessage { get; set; }
    //    public List<BranchDDResponse> BindBranch { get; set; }
    //}

    //public class BindBranchRequest
    //{
    //    public int sUserId { get; set; }
    //    public int sCycleCode { get; set; }
    //    public int bkCode { get; set; }
    //    public int CityCode { get; set; }
    //}
    //public class BindBranchRequest2
    //{
    //    public string UserId { get; set; }
    //    public string bkId { get; set; }
    //    public string CityId { get; set; }
    //}

    //public class BranchDDResponse
    //{
    //    public int InstID { get; set; }
    //    public string InstName { get; set; }
    //    public int branchid { get; set; }
    //    public string branch_name { get; set; }

    //}
    //public class View1_RowUpdatingResponse
    //{
    //    public string responseCode { get; set; }
    //    public string responseMessage { get; set; }
    //    public bool RowUpdating { get; set; }
    //}


    //public class View1_RowUpdatingRequest
    //{
    //    public string sHostID { get; set; }
    //    public string sComment1 { get; set; }
    //    public string sUserId { get; set; }
    //}


    //public class GridView1_RowCommandResponse
    //{
    //    public string responseCode { get; set; }
    //    public string responseMessage { get; set; }
    //    public bool RowCommand { get; set; }
    //}


    //public class GridView1_RowCommandRequest
    //{
    //    public int sUserId { get; set; }
    //    public string CommandName { get; set; }
    //    public string CommandArgument { get; set; }
    //    public string isAuth { get; set; }
    //    public int sHostId { get; set; }

    //}


    //public class MarkReturnReasonsResponse
    //{
    //    public string responseCode { get; set; }
    //    public string responseMessage { get; set; }
    //    public bool MarkReturn { get; set; }
    //}


    //public class MarkReturnReasonsRequest
    //{
    //    public int sUserId { get; set; }
    //    public int lstRTN { get; set; }
    //    public int sHostId { get; set; }

    //}



    //public class insertReturnLogResponse
    //{
    //    public string responseCode { get; set; }
    //    public string responseMessage { get; set; }
    //    public bool instRtnLog { get; set; }
    //}


    //public class insertReturnLogRequest
    //{
    //    public string sUserId { get; set; }
    //    public string sHostID { get; set; }
    //    public string sDetail { get; set; }

    //}
}
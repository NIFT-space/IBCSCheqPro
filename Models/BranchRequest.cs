using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBCS_Web_Portal.Models
{
    public class BranchboardBranchRequest
    {
        public string BankCode { get; set; }
        public int CityId { get; set; }
        public int BankId { get; set; }
        public int UserId { get; set; }

    }
    public class PanPakResponse
    {
        public bool PanPaks { get; set; }
    }
    public class vNetScorePakwiseRequest
    {
        public string pdate { get; set; }
        public string bkcode { get; set; }
    }
    public class vNetScorePakwise
    {
        public string BankID { get; set; }
        public string BankName { get; set; }
        public string CycleID { get; set; }
        public string cycle_desc { get; set; }
        public string IWOWID { get; set; }
        public string ClgType { get; set; }
        public string Items { get; set; }
        public string Amount { get; set; }
    }
    public class vNetScorePakwiseResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<vNetScorePakwise> vNetResult { get; set; }
    }
    public class vNetScoreCitywise
    {
        public string BankID { get; set; }
        public string BankName { get; set; }
        public string CycleID { get; set; }
        public string cycle_desc { get; set; }
        public string Items { get; set; }
        public string Amount { get; set; }
        public string IWOWID { get; set; }
        public string ClgType { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
    }
    public class vNetScoreCitywiseResponse
    {
        public List<vNetScoreCitywise> vNetResult { get; set; }
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
    }
    public class vNetScoreBranchwise
    {
        public string BankID { get; set; }
        public string BankName { get; set; }
        public string BranchID { get; set; }
        public string BranchName { get; set; }
        public string CycleID { get; set; }
        public string cycle_desc { get; set; }
        public string Items { get; set; }
        public string Amount { get; set; }
        public string IWOWID { get; set; }
        public string ClgType { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
    }
    public class vNetScoreBranchwiseResponse
    {
        public List<vNetScoreBranchwise> vNetResult { get; set; }
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
    }
    public class vTransactionwise
    {
        public int hostid { get; set; }
        public int SBkId { get; set; }
        public string SBkNm { get; set; }
        public int SBrId { get; set; }
        public string SBrNm { get; set; }
        public int RBkId { get; set; }
        public string RBkNm { get; set; }
        public int RBrId { get; set; }
        public string RBrNm { get; set; }
        public int TrCode { get; set; }
        public string CityId { get; set; }
        public string CityName { get; set; }
        public int CycleId { get; set; }
        public string CycleDesc { get; set; }
        public string ChqNo { get; set; }
        public string Accno { get; set; }
        public string TrId { get; set; }
        public string IWOWID { get; set; }
        public string ClgType { get; set; }
        public string Amount { get; set; }
        public string IQA { get; set; }
        public string UV { get; set; }
        public string QR { get; set; }
        public string Std { get; set; }
        public string MICR { get; set; }
        public string Dup { get; set; }
        public string isFraud { get; set; }
        public string isDeffer { get; set; }
        public string WMark { get; set; }
        public string TempID { get; set; }
        public string UVPerc { get; set; }
        public string AvgChqSize { get; set; }
        public string TChqR { get; set; }
        public string TChq { get; set; }
        public string underSizeImage { get; set; }
        public string folderOrTornDocumentCorners { get; set; }
        public string foldedOrTornDocumentEdges { get; set; }
        public string framingError { get; set; }
        public string documentSkew { get; set; }
        public string overSizeImage { get; set; }
        public string piggyBack { get; set; }
        public string imageTooLight { get; set; }
        public string imageTooDark { get; set; }
        public string horizontalStreaks { get; set; }
        public string belowMinimumCompressedImageSize { get; set; }
        public string aboveMaximumCompressedImageSize { get; set; }
        public string spotNoise { get; set; }
        public string frontRearDimensionMismatch { get; set; }
        public string carbonStrip { get; set; }
        public string outOfFocus { get; set; }
    }
    public class vTransactionwiseResponse
    {
        public List<vTransactionwise> vTran { get; set; }
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
    }

    public class vNetBoardResponse
    {
        public List<NetBoard> NetBoard { get; set; }
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
    }
    public class HostNets
    {
        public string bankid { get; set; }
        public string bankname { get; set; }
        public string cycleid { get; set; }
        public string cyclename { get; set; }
        public int Owcount { get; set; }
        public double Owamount { get; set; }
        public int Incount { get; set; }
        public double Inamount { get; set; }
        public int Netcount { get; set; }
        public double Netamount { get; set; }
        public string CurrencyType { get; set; }
        public int SortOrder { get; set; }
    }
	public class NetBoard
	{
		public string cyclename { get; set; }
		public int Owcount { get; set; }
		public double Owamount { get; set; }
		public int Incount { get; set; }
		public double Inamount { get; set; }
		public int Netcount { get; set; }
		public double Netamount { get; set; }
        public string CurrencyType { get; set; }
		///PK_
		public string PK_cyclename { get; set; }
		public int PK_Owcount { get; set; }
		public double PK_Owamount { get; set; }
		public int PK_Incount { get; set; }
		public double PK_Inamount { get; set; }
		public int PK_Netcount { get; set; }
		public double PK_Netamount { get; set; }
		public string PK_CurrencyType { get; set; }
		///PK_RTN
		public string rtn_cyclename { get; set; }
		public int rtn_Owcount { get; set; }
		public double rtn_Owamount { get; set; }
		public int rtn_Incount { get; set; }
		public double rtn_Inamount { get; set; }
		public int rtn_Netcount { get; set; }
		public double rtn_Netamount { get; set; }
		public string rtn_CurrencyType { get; set; }
		///FC_
		public string FC_cyclename { get; set; }
		public int FC_Owcount { get; set; }
		public double FC_Owamount { get; set; }
		public int FC_Incount { get; set; }
		public double FC_Inamount { get; set; }
		public int FC_Netcount { get; set; }
		public double FC_Netamount { get; set; }
		public string FC_CurrencyType { get; set; }
		///FC_RTN
		public string FC_RTNcyclename { get; set; }
		public int FC_RTNOwcount { get; set; }
		public double FC_RTNOwamount { get; set; }
		public int FC_RTNIncount { get; set; }
		public int FC_RTNNetcount { get; set; }
		public double FC_RTNInamount { get; set; }
		public double FC_RTNNetamount { get; set; }
		public string FC_RTNCurrencyType { get; set; }
	}
}
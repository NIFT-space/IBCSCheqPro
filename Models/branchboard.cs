using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBCS_Web_Portal.Models
{
    public class HostNIBC
    {
        public int hostId { get; set; }
        public int senderBankId { get; set; }
        public string senderBankName { get; set; }
        public int senderBranchId { get; set; }
        public string senderBranchName { get; set; }
        public int recieverBankId { get; set; }
        public string recieverBankName { get; set; }
        public int recieverBranchId { get; set; }
        public string recieverBranchName { get; set; }
        public string trCode { get; set; }
        public int cycleId { get; set; }
        public string cycleDescription { get; set; }
        public string chequeNo { get; set; }
        public string accountNo { get; set; }
        public int trId { get; set; }
        public string IWOWId { get; set; }
        public string clearingType { get; set; }
        public int cityId { get; set; }
        public string cityName { get; set; }
        public double amount { get; set; }
        public string IQA { get; set; }
        public string UV { get; set; }
        public string bar { get; set; }
        public string MICR { get; set; }
        public string std { get; set; }
        public string dup { get; set; }
        public string WMark { get; set; }
        public double avgChqSize { get; set; }
        public int tChq { get; set; }
        public int tChqR { get; set; }
        public string Templateid { get; set; }
        public string UVperc { get; set; }
        public string truncTime { get; set; }
        public int truncBy { get; set; }
        public int settled { get; set; }
        public string setTime { get; set; }
        public int setBy { get; set; }
        public int authBy { get; set; }
        public string authDateTime { get; set; }
        public int isAuth { get; set; }
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
        public int reasonID2 { get; set; }
        public int reason3 { get; set; }
        public string comment1 { get; set; }
        public int reasonID3 { get; set; }
        public int reason2 { get; set; }
        public string comment2 { get; set; }
        public string comment3 { get; set; }
        public string isDeffer { get; set; }
    }
    public class LogImageFileRequest
    {
        public int userId { get; set; }
        public int userLogId { get; set; }
        public int hostId { get; set; }
    }
    public class SingleRecord
    {
        public int hostId { get; set; }
        public int senderBankId { get; set; }
        public string senderBankName { get; set; }
        public int senderBranchId { get; set; }
        public string senderBranchName { get; set; }
        public int recieverBankId { get; set; }
        public string recieverBankName { get; set; }
        public int recieverBranchId { get; set; }
        public string recieverBranchName { get; set; }
        public string trCode { get; set; }
        public int cycleId { get; set; }
        public string cycleDescription { get; set; }
        public string chequeNo { get; set; }
        public string accountNo { get; set; }
        public int trId { get; set; }
        public string IWOWId { get; set; }
        public string clearingType { get; set; }
        public int cityId { get; set; }
        public string cityName { get; set; }
        public double amount { get; set; }
        public string IQA { get; set; }
        public string UV { get; set; }
        public string bar { get; set; }
        public string MICR { get; set; }
        public string std { get; set; }
        public string dup { get; set; }
        public string WMark { get; set; }
        public double avgChqSize { get; set; }
        public int tChq { get; set; }
        public int tChqR { get; set; }
        public string truncTime { get; set; }
        public int truncBy { get; set; }
        public int settled { get; set; }
        public string setTime { get; set; }
        public int setBy { get; set; }
        public int authBy { get; set; }
        public string authDateTime { get; set; }
        public int isAuth { get; set; }
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
        public int reasonID2 { get; set; }
        public int reason3 { get; set; }
        public string comment1 { get; set; }
        public int reasonID3 { get; set; }
        public int reason2 { get; set; }
        public string comment2 { get; set; }
        public string comment3 { get; set; }
        public string isDeffer { get; set; }
    }


public class Reason
{
    public string txtReason { get; set; }
    public int ddReason { get; set; }
}
}
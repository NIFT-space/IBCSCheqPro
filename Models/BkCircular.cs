using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBCS_Core_Web_Portal.Models
{
    public class Circular
    {
        public int CirId { get; set; }
        public string Repname { get; set; }
        public string RepDesc { get; set; }
        public string RepSize { get; set; }
        public string RepLoadTime { get; set; }
        public bool BSpecial { get; set; }
        public bool BDownload { get; set; }
        public string FileUpdateDate { get; set; }
        public string FileUpdateTime { get; set; }
    }

    public class CircularReport : Circular
    {
        public byte[] Report { get; set; }
        public string RepType { get; set; }
    }
    public class CircularRequest
    {
        public int CirId { get; set; }
    }

    public class CircularFileResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<Circular> circularFile { get; set; }
    }
    public class CircularFileCirIDResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public CircularReport circularFile { get; set; }
    }
    public class CircularDownLogRequest
    {
        public string userid { get; set; }
        public int userlogid { get; set; }
        public int circularid { get; set; }
    }

}
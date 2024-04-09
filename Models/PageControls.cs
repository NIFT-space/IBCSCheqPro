using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBCS_Web_Portal.Models
{
    public class ProcessDT
    {
        public string pdate { get; set; }
    }
    public class GetDateResponse
    {
        public string responseCode { get; set; }
        //public string responseMessage { get; set; }
        public List<string> pdate { get; set; }
    }

    public class Info
    {
        public string infotext { get; set; }
    }
    public class InfoResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<Info> infotext { get; set; }
    }
    public class PageAll_Request
    {
        public string userid { get; set; }
        public string pagename { get; set; }
    }
    public class PageAllowedResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public bool PageAllowed { get; set; }

        public class PageRequest
        {
            public string Userid { get; set; }
            public string PageName { get; set; }
        }
        public class Pageurl
        {
            public bool url { get; set; }
            public string responseCode { get; set; }
            public string responseMessage { get; set; }
        }
        public class RepRequest
        {
            public string pdate { get; set; }
            public string cycleno { get; set; }
            public string bankid { get; set; }
            public string branchid { get; set; }
            public string cycno2 { get; set; }
            public string Isbranchuser { get; set; }
        }
        public class RepCount
        {
            public string norm_count { get; set; }
            public string samed_count { get; set; }
            public string int_count { get; set; }
            public string dol_count { get; set; }
        }
        public class RepCities
        {
            public string cityid { get; set; }
        }
        public class RepResponse
        {
            public string responseCode { get; set; }
            public string responseMessage { get; set; }
            public List<RepCities> cities { get; set; }
        }
        public class ImgRequest
        {
            public string pdate { get; set; }
            public string cycleno { get; set; }
            public string bankid { get; set; }
            public string branchid { get; set; }
            public string cycno2 { get; set; }
            public string Isbranchuser {  get; set; }
        }
        public class ImgCount
        {
            public int count { get; set; }
        }
        public class ImgResponse
        {
            public string responseCode { get; set; }
            public string responseMessage { get; set; }
            public List<ImgCount> count { get; set; }
        }
    }
}
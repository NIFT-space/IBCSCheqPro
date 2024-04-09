using IBCS_Core_Web_Portal.Helper;
using IBCS_Core_Web_Portal.Models;
using IBCS_Core_Web_Portal.Pages.Reports;
using IBCS_Web_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using Nancy;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.IO.Pipes;
using System.Net.Mail;

namespace IBCS_Core_Web_Portal.Pages.Imagedata
{
    public class FileTrafficModel : PageModel
    {
        public const string SessionCount = "_count";
        public const string SessionFile = "_file";
        public static string apiURL_bktr = "";
        public static string apiURL = "";
        public static string userid = "";
        public static string userlogid = "";
        public static string bankcode = "";
        public static string emailaddress = "";
        public static string branchcode = "";
        public static string dtFrom_ = "";
        public static string dtTo_ = "";
        public static string Auth_ = "";
        byte[] bytes;

        [BindProperty]
        public IFormFile Upload { get; set; }

        public List<ICLLog> reports { get; set; }
        public List<ICLLog2> reports2 { get; set; }


        public static string sel_ddlbank, sel_ddlcity, sel_ddlcycle;
        public static string selected_city, selected_bank;
        public void OnGet()
        {
            try
            {
                userid = HttpContext.Session.GetString(loginModel.SessionKeyName1);
                userlogid = HttpContext.Session.GetString(loginModel.SessionKeyName9);
                bankcode = HttpContext.Session.GetString(loginModel.SessionKeyName10);
                branchcode = HttpContext.Session.GetString(loginModel.SessionKeyName11);
                emailaddress = HttpContext.Session.GetString(loginModel.SessionKeyName12);
                dtFrom_ = HttpContext.Session.GetString(GetProdtModel.dtFrom);
                dtTo_ = HttpContext.Session.GetString(GetProdtModel.dtTo);
                Auth_ = HttpContext.Session.GetString(loginModel.SessionKeyName16);

                if (Auth_ != null && Request.Cookies["IBCS.Auth.Token"] != null)
                {
                    if (!Auth_.Equals(Request.Cookies["IBCS.Auth.Token"].ToString()))
                    {
                        Response.Clear();
                        Response.Redirect("/Sessionexpire", true);
                        reports = new List<ICLLog>();
                        reports2 = new List<ICLLog2>();
                    }
                    else
                    {
                        ///GOOD TO GO///
                        GetFiles();
                        GetFiles2();
                    }
                }
                else
                {
                    Response.Clear();
                    Response.Redirect("/Sessionexpire", true);
                    reports = new List<ICLLog>();
                    reports2 = new List<ICLLog2>();
                }

                Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate"); // HTTP 1.1.
                Response.Headers.Append("Pragma", "no-cache"); // HTTP 1.0.
                Response.Headers.Append("Last-Modified", System.DateTime.Now.ToString());

                if (userid == null)
                {
                    Response.Redirect("/Sessionexpire", true);
                    reports = new List<ICLLog>();
                    reports2 = new List<ICLLog2>();
                }
                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                apiURL_bktr = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "BkTrSc/";
                apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "FileTraffic/";


                if (userid == null)
                {
                    return;
                }

            }
            catch (ThreadAbortException ex)
            {
                LogWriter.WriteToLog(ex);
                reports = new List<ICLLog>();
                reports2 = new List<ICLLog2>();
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                Response.Redirect("/NotAllowed", true);
                reports = new List<ICLLog>();
                reports2 = new List<ICLLog2>();
            }
        }
        public JsonResult GetFiles()
        {
            try
            {
                String dtFrom = "", dtTo = "";
                string sUserId = userid;
                dtFrom = dtFrom_;
                dtTo = dtTo_;

                DataTable dt = new DataTable();
                DataTable dtSort = new DataTable();

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);
                    UploadICLLogRequest req = new UploadICLLogRequest
                    {
                        FromDate = dtFrom,
                        ToDate = dtTo,
                        InstID = Convert.ToInt32(bankcode)
                        //BranchID = Convert.ToInt32(branchcode)
                    };

                    var postTask = client.PostAsJsonAsync<UploadICLLogRequest>("GetUploadICLLogs", req);
                    postTask.Wait();

                    var result = postTask.Result;           
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<UploadICLLogResponse>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                            reports = readTask.Result.logs;
                        else
                            reports = new List<ICLLog>();

                    }
                    else
                        reports = new List<ICLLog>();
                }
                return new JsonResult(reports);
            }
            catch (Exception ex)
            {
                reports = new List<ICLLog>();
                LogWriter.WriteToLog("Exception on reports - " + ex);
                return new JsonResult("Failed");
            }
        }

        public JsonResult GetFiles2()
        {
            try
            {
                String dtFrom = "", dtTo = "";
                string sUserId = userid;
                dtFrom = dtFrom_;
                dtTo = dtTo_;

                DataTable dt = new DataTable();
                DataTable dtSort = new DataTable();

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);
                    UploadICLLogRequest req = new UploadICLLogRequest
                    {
                        FromDate = dtFrom,
                        ToDate = dtTo,
                        InstID = Convert.ToInt32(bankcode)
                        //BranchID = Convert.ToInt32(branchcode)
                    };

                    var postTask = client.PostAsJsonAsync<UploadICLLogRequest>("GetUploadICL2Logs", req);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<UploadICLLogResponse2>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                            reports2 = readTask.Result.logs;
                        else
                            reports2 = new List<ICLLog2>();

                    }
                    else
                        reports2 = new List<ICLLog2>();
                }
                return new JsonResult(reports2);
            }
            catch (Exception ex)
            {
                reports2 = new List<ICLLog2>();
                LogWriter.WriteToLog("Exception on reports2 - " + ex);
                return new JsonResult("Failed");
            }
        }

    }
}

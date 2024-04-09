using IBCS_Core_Web_Portal.Helper;
using IBCS_Core_Web_Portal.Models;
using IBCS_Core_Web_Portal.Pages.Reports;
using IBCS_Web_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace IBCS_Core_Web_Portal.Pages.Imagedata
{
    public class Down_branchdfModel : PageModel
    {
        public const string SessionCount = "_count";
        public const string SessionFile = "_file";
        public static string comm_apiURL = "";
        public static string apiURL = "";
        public static string userid = "";
        public static string userlogid = "";
        public static string bankcode = "";
        public static string dtFrom_ = "";
        public static string dtTo_ = ""; public static string Auth_ = "";
        public List<InstBank> instBanks { get; set; }
        public List<City> cities { get; set; }
        public List<Cycle> cycle { get; set; }
        public List<ReportFile> reports { get; set; }
        public static List<ReportFile> repsave;
        public static string sel_ddlbank, sel_ddlcity, sel_ddlcycle;
        public static string selected_city, selected_bank;
        public void OnGet()
        {
            try
            {

                userid = HttpContext.Session.GetString(loginModel.SessionKeyName1);
                userlogid = HttpContext.Session.GetString(loginModel.SessionKeyName9);
                bankcode = HttpContext.Session.GetString(loginModel.SessionKeyName10);
                dtFrom_ = HttpContext.Session.GetString(GetProdtModel.dtFrom);
                dtTo_ = HttpContext.Session.GetString(GetProdtModel.dtTo);
                Auth_ = HttpContext.Session.GetString(loginModel.SessionKeyName16);

                if (Auth_ != null && Request.Cookies["IBCS.Auth.Token"] != null)
                {
                    if (!Auth_.Equals(Request.Cookies["IBCS.Auth.Token"].ToString()))
                    {
                        Response.Clear();
                        Response.Redirect("/Sessionexpire", true);
                    }
                    else
                    {
                        ///GOOD TO GO///
                    }
                }
                else
                {
                    Response.Clear();
                    Response.Redirect("/Sessionexpire", true);
                }

                Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate"); // HTTP 1.1.
                Response.Headers.Append("Pragma", "no-cache"); // HTTP 1.0.
                Response.Headers.Append("Last-Modified", System.DateTime.Now.ToString());
                if (userid == null)
                {
                    Response.Redirect("/Sessionexpire",true);
                }
                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "Down_branchdf/";
                comm_apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "BkTrSc/";
                if (userid == null)
                {
                    return;
                }

                BindBankDD();

                BindCityDD();

                BindCycleDD();

            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception on OnGet - " + ex);
                Response.Redirect("/NotAllowed",true);
            }
        }
        private void BindBankDD()
        {
            try
            {
                // Bank
                string sUserId = userid;// "26887";//Convert.ToString(Session["myid"]).Trim();
                String sInstId = bankcode;//"54";//Session["BankCode"].ToString();
                DataTable dtInst = new DataTable();
                InstRequest req = new InstRequest()
                {
                    UserId = sUserId,
                    InstId = sInstId
                };

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);

                    var postTask = client.PostAsJsonAsync<InstRequest>("GetInstBank", req);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<InstBankResponse>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                            instBanks = readTask.Result.bkAdvice;

                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception on BindBankDD - " + ex);
            }
        }
        private void BindCityDD()
        {
            try
            {
                String CityCount = "";
                string sUserId = userid;//"26887";//Convert.ToString(Session["myid"]).Trim();
                DataTable dtCity = new DataTable();

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);
                    CityRequest req = new CityRequest
                    {
                        userID = sUserId
                    };
                    var postTask = client.PostAsJsonAsync<CityRequest>("GetUserCities", req);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<CityResponse>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                            cities = readTask.Result.cities;
                        CityCount = cities.Count.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception on BindCityDD - " + ex);
            }
        }
        private void BindCycleDD()
        {
            try
            {
                //Cycle
                DataTable dtCycle = new DataTable();


                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(comm_apiURL);

                    var postTask = client.GetAsync("GetCycleListWithoutIC");
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<CycleResponse>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                            cycle = readTask.Result.cycle;
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception on BindCycleDD - " + ex);
            }
        }
        public JsonResult OnPostGetFiles([FromBody] Params_Rep PR)
        {
            try
            {
                String dtFrom = "", dtTo = "";
                string sUserId = userid;

                dtFrom = dtFrom_;//"2023-06-27 00:00:000";//Convert.ToString(Session["dtFrom"] + "");
                dtTo = dtTo_;// "2023-06-27 23:59:000";// Convert.ToString(Session["dtTo"] + "");
                //}

                DataTable dt = new DataTable();
                DataTable dtSort = new DataTable();

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);
                    ReportFileBranchRequest req = new ReportFileBranchRequest
                    {
                        CycleNo = Convert.ToInt32(PR.DDL_Cycle),
                        DateFrom = dtFrom,
                        DateTo = dtTo,
                        UserID = Convert.ToInt32(sUserId),
                        CityID = (PR.DDL_City != "All" ? Convert.ToInt32(PR.DDL_City) : 0),
                        InstID = Convert.ToInt32(PR.DDL_Bank),
                    };

                    var postTask = client.PostAsJsonAsync<ReportFileBranchRequest>("GetReportFiles", req);
                    postTask.Wait();
                   
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<ReportFileResponse>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                            reports = readTask.Result.reportFile;

                    }
                }
                return new JsonResult(reports);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception on reports - " + ex);
                return new JsonResult("Failed");
            }
        }
        public ActionResult OnPostDownloadFile(string? v301x913, string? c036x9210)
        {
            try
            {
                int chkr = 0;
                int t_ = 0;
                if (HttpContext.Session.GetString(SessionFile) != null)
                {
                    if (HttpContext.Session.GetString(SessionFile) == v301x913)
                    {
                        if (HttpContext.Session.GetString(Down_branchdfModel.SessionCount) != null)
                        {
                            t_ = Convert.ToInt32(HttpContext.Session.GetString(Down_branchdfModel.SessionCount));
                        }

                        if (t_ < 5)
                        {

                        }
                        else
                        {
                            LogWriter.WriteToLog("Download Limit Reached. Please try again later!");
                            Response.Redirect("/Imagedata/Down_branchdf?dolrchd=1");

                        }
                    }
                    else
                    {
                        HttpContext.Session.SetString(SessionFile, v301x913);
                        HttpContext.Session.SetString(Down_branchdfModel.SessionCount, "0");
                    }
                }
                else
                {
                    HttpContext.Session.SetString(SessionFile, v301x913);
                    HttpContext.Session.SetString(Down_branchdfModel.SessionCount, "0");
                }

                if (repsave != null)
                {
                    foreach (var report in repsave)
                    {
                        if (Convert.ToString(report.RepId) == v301x913)
                        {
                            chkr = 0;
                            break;
                        }
                        else
                        {
                            chkr = 1;
                        }
                    }
                    if (chkr == 1)
                    {
                        Response.Redirect("/Imagedata/Down_branchdf");
                    }
                }
                string RepId = v301x913;
                string RepName = c036x9210;
                String DtTm = "";
                DtTm = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
                DtTm = DtTm.Replace(" ", "").Replace("/", "").Replace(":", "");
                byte[] MyData = new byte[0];

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);

                    ReportRequest req = new ReportRequest
                    {
                        iRepID = Convert.ToInt32(v301x913)
                    };
                    BkReportFile circular = new BkReportFile();
                    var responseTask = client.PostAsJsonAsync<ReportRequest>("getBkReportFile", req);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {

                        var readTask = result.Content.ReadAsAsync<BkReportFile>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                            circular = readTask.Result;
                    }

                    MyData = circular.file; //(byte[]) myRow["Report"];
                    long fileSize = (long)MyData.Length;
                    int ArraySize = new int();
                    ArraySize = MyData.GetUpperBound(0);

                    //HTTP POST
                    CircularDownLogRequest req2 = new CircularDownLogRequest
                    {
                        userid = userid,//Session["myid"].ToString(),
                        userlogid = Convert.ToInt32(userlogid),//Convert.ToInt32(Session["userlogid"]),
                        circularid = Convert.ToInt32(v301x913)
                    };

                    var postTask = client.PutAsJsonAsync<CircularDownLogRequest>("setReportDownLog", req2);
                    postTask.Wait();

                    var result2 = postTask.Result;

                    //LogWriter.WriteToLog(sSQL);

                    //Send the File to Download.  
                    string ct = HttpContext.Session.GetString(Down_branchdfModel.SessionCount);
                    string ct_str = Convert.ToString(Convert.ToInt32(ct) + 1);
                    ct = ct_str;
                    HttpContext.Session.SetString(SessionCount, ct_str);
                    return File(MyData, "application/octet-stream", c036x9210);
                }
            }
            catch (Exception ex)
            {
                byte[] MyData = new byte[0];
                LogWriter.WriteToLog("Exception on BindCycleDD - " + ex);
                return File(MyData, "application/octet-stream", c036x9210);
            }
        }
    }
}

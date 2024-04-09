using IBCS_Core_Web_Portal.Helper;
using IBCS_Core_Web_Portal.Models;
using IBCS_Core_Web_Portal.Pages.Reports;
using IBCS_Web_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using static IBCS_Core_Web_Portal.Pages.Cycleinfo.BankEmailModel;

namespace IBCS_Core_Web_Portal.Pages.Imagedata
{
    public class HubICLFileModel : PageModel
    {
        public const string SessionCount = "_count";
        public const string SessionFile = "_file";
        public static string apiURL_bktr = "";
        public static string apiURL = "";
        public static string apiURL_Hub = "";
        public static string img_apiURL = "";
        public static string userid = "";
        public static string userlogid = "";
        public static string bankcode = "";
        public static string dtFrom_ = "";
        public static string dtTo_ = "";
        public static string Auth_ = "";
        public List<InstBank> instBanks { get; set; }
        public List<Hub> hubs { get; set; }
        public List<City> cities { get; set; }
        public List<Cycle> cycle { get; set; }
        public List<Response_OECHUBFile_List> reports { get; set; }
        public static List<BkReport> repsave;

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
                    Response.Redirect("/Sessionexpire", true);
                }
                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                apiURL_bktr = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "BkTrSc/";
                apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "ImageICL/";
                apiURL_Hub = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "ImageHubDownload/";
                img_apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.img") + "GetImageFromBin/";

                if (userid == null)
                {
                    return;
                }

                BindCityDD();

                BindBankDD();

                BindCycleDD();
                BindBankTypeDD();
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                Response.Redirect("/NotAllowed", true);
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
                    client.BaseAddress = new Uri(apiURL_bktr);
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
                LogWriter.WriteToLog(ex);
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
                    client.BaseAddress = new Uri(apiURL_bktr);

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
                LogWriter.WriteToLog(ex);
            }
        }
        //public JsonResult OnPostBindHubDD(int CityID, int InstID)
        //{
        //    try
        //    {
        //        String CityCount = "";
        //        string sUserId = userid;//"26887";//Convert.ToString(Session["myid"]).Trim();
        //        DataTable dtCity = new DataTable();

        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(apiURL_Hub);
        //            HubRequest req = new HubRequest
        //            {
        //                CityID = CityID,
        //                InstID = InstID
        //            };
        //            var postTask = client.PostAsJsonAsync<HubRequest>("GetHub", req);
        //            postTask.Wait();

        //            var result = postTask.Result;
        //            if (result.IsSuccessStatusCode)
        //            {
        //                var readTask = result.Content.ReadAsAsync<HubResponse>();
        //                readTask.Wait();

        //                if (readTask.Result.responseMessage.ToLower() == "success")
        //                    hubs = readTask.Result.hub;
        //                //CityCount = hubs.Count.ToString();
        //            }
        //        }
        //        return new JsonResult(hubs);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogWriter.WriteToLog("Exception on BindHubDD - " + ex);
        //        return new JsonResult("Failed");
        //    }
        //}
        private void BindCycleDD()
        {
            try
            {
                //Cycle
                DataTable dtCycle = new DataTable();


                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL_bktr);

                    var postTask = client.GetAsync("GetCycleListWithoutIC");
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<CycleResponse>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                        {
                            cycle = readTask.Result.cycle;
                            cycle.RemoveAll(c => c.CycleNo == 3);
                            cycle.RemoveAll(c => c.CycleNo == 4);
                            cycle.RemoveAll(c => c.CycleNo == 7);
                            cycle.RemoveAll(c => c.CycleNo == 22);
                            cycle.RemoveAll(c => c.CycleNo == 41);
                            cycle.RemoveAll(c => c.CycleNo == 42);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception on BindCycleDD - " + ex);
            }
        }
        private void BindBankTypeDD()
        {
            try
            {

                DataTable dtType = new DataTable();
                dtType.Columns.Add("ClgID");
                dtType.Columns.Add("ClgType");
                dtType.Rows.Add("1", "Inward");
                dtType.Rows.Add("2", "Outward");
                ViewData["DtType"] = dtType;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }

        }
        public JsonResult OnPostGetFiles([FromBody] Params_Rep2 PR)
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
                    client.BaseAddress = new Uri(img_apiURL);
                    NIBC_HUB_ICL_DownloadRequest req = new NIBC_HUB_ICL_DownloadRequest
                    {
                        dtFrom = dtFrom,
                        dtTo = dtTo,
                        rInstID = PR.DDL_Bank,
                        //DDL_City = PR.DDL_City,
                        CycleNo = PR.DDL_Cycle,
                        sUserId = sUserId,
                        DDL_City = (PR.DDL_City != "All" ? PR.DDL_City : ""),
                        PageName = "ImageInwardOECFile",
                        DDL_BankType = PR.DDL_BankType
                    };

                    var postTask = client.PostAsJsonAsync<NIBC_HUB_ICL_DownloadRequest>("HUB_ICL_File", req);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<Response_OECHubFile_2>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                            reports = readTask.Result.lstOECHub;

                    }
                }
                return new JsonResult(reports);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return new JsonResult("Failed");
            }
        }
        public ActionResult OnPostDownloadFile(string? v301x913, string? c036x9210)
        {

            try
            {
				//LogWriter.WriteToLog("started download file");
				int chkr = 0;
                int t_ = 0;
                if (HttpContext.Session.GetString(SessionFile) != null)
                {
                    if (HttpContext.Session.GetString(SessionFile) == v301x913)
                    {
                        if (HttpContext.Session.GetString(BkTrSc.SessionCount) != null)
                        {
                            t_ = Convert.ToInt32(HttpContext.Session.GetString(SessionCount));
                        }

                        if (t_ < 5)
                        {

                        }
                        else
                        {
                            LogWriter.WriteToLog("Download Limit Reached. Please try again later!");
                            return new JsonResult("RLimit");
                            //Response.Redirect("/Reports/BkTrSc?dolrchd=1");
                        }
                    }
                    else
                    {
                        HttpContext.Session.SetString(SessionFile, v301x913);
                        HttpContext.Session.SetString(BkTrSc.SessionCount, "0");
                    }
                }
                else
                {
                    HttpContext.Session.SetString(SessionFile, v301x913);
                    HttpContext.Session.SetString(BkTrSc.SessionCount, "0");
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
                        Response.Redirect("/Reports/BkTrSc");
                    }
                }
                string RepId = v301x913;
                string RepName = c036x9210;
				//LogWriter.WriteToLog("repid repname" + RepId + "-"+RepName);
				String DtTm = "";
                DtTm = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
                DtTm = DtTm.Replace(" ", "").Replace("/", "").Replace(":", "");
                byte[] MyData = new byte[0];

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(img_apiURL);

                    NIBC_ICL_Download_ImgRequest req = new NIBC_ICL_Download_ImgRequest
                    {
                        imgID = v301x913,
                        sUserId = userid,
                        userlogid = "0"
                    };
					//LogWriter.WriteToLog("req details:" + v301x913 + userid);
					BkReportFile circular = new BkReportFile();
                    var responseTask = client.PostAsJsonAsync<NIBC_ICL_Download_ImgRequest>("HUB_ICL_File_Download", req);
                    responseTask.Wait();
					//LogWriter.WriteToLog("api call HUB_ICL_File_Download");
					var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {

                        var readTask = result.Content.ReadAsStringAsync();
                        readTask.Wait();

                        if (readTask.IsCompletedSuccessfully)
                            MyData = Convert.FromBase64String(readTask.Result);
                    }
					//LogWriter.WriteToLog("circular file" + circular.file);
					//MyData = circular.file; //(byte[]) myRow["Report"];
					LogWriter.WriteToLog("Downloaded Hub File of Size: " +MyData.Length);
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
                    string ct = HttpContext.Session.GetString(BkTrSc.SessionCount);
                    string ct_str = Convert.ToString(Convert.ToInt32(ct) + 1);
                    ct = ct_str;
                    HttpContext.Session.SetString(SessionCount, ct_str);
                    return File(MyData, "application/octet-stream", c036x9210);
                }
            }
            catch (Exception ex)
            {
                byte[] MyData = new byte[0];
				LogWriter.WriteToLog(ex.Message);
				LogWriter.WriteToLog(ex.Source);
				LogWriter.WriteToLog(ex.StackTrace);
				LogWriter.WriteToLog(ex.InnerException);

				LogWriter.WriteToLog("string exc "+ex.Message);
				LogWriter.WriteToLog("string exc " + "string exc " + ex.Source);
				LogWriter.WriteToLog("string exc " + ex.StackTrace);
				LogWriter.WriteToLog("string exc " + ex.InnerException);
				return new JsonResult("Exception handled");
			}
        }
    }
}

using IBCS_Core_Web_Portal.Helper;
using IBCS_Web_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using NIBC.Models;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace IBCS_Core_Web_Portal.Pages.Marking
{
    public class InwardmarkingModel : PageModel
    {
        public const string SessionKeyName1 = "_hostID";
        public static string apiURL = "";
        public static string img_apiURL = "";
        public static string userid = "";
        public static string userlogid = "";
        public static string bankcode = "";
        public static string dtFrom_ = "";
        public static string dtTo_ = "";
        public List<Branch> Br_;
        public static string apiUrl_mark = "";
        public static string apiUrl_auth = "";
        public static string apiURL_Brbd = "";
        public static string Auth_ = "";
        public List<InstBank> instBanks { get; set; }
        public List<BindReturnResponse> ReturnBanks { get; set; }
        public List<City> cities { get; set; }
        public List<Cycle> cycle { get; set; }
        public BranchResponse branches { get; set; }
        public static string selected_city, selected_bank;
        public List<CheckSendForReviewReturn> obj { get; set; }
        DataTable _imbrTable = new DataTable();

        public static DataTable oDt;
        public Reason objReason { get; set; }
        public SingleReturn objSgl { get; set; }
        public List<TransactionwiseInwardReturn> objtwise { get; set; }
        public List<dtsum_> dts1 { get; set; }
        public List<dtsum2_> dts2 { get; set; }
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
				CheckPageAllowed cpa = new CheckPageAllowed();

				if (cpa.Validatepageallowed(userid, "Inwardmarking") == false)
                {
					Response.Redirect("/NotAllowed",true);
				}
                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                apiUrl_mark = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "Inwardmarking/";
                apiUrl_auth = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "Inwardauthorization/";
                apiURL_Brbd = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "Branchboard/";
                //img_apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.img") + "GetImageFromBin/";
                img_apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "GetImageFromBin/";
                BindBankDD();
                BindCityDD();
                BindCycleDD();
                //BindBranch();

                //GetFiles();
                //if (d1 != null || d2 != null || d3 != null || d4 != null)
                //{
                   // BindReturnDD(d1, d2, d3);
                    //GetFiles(d1, d2, d3, d4);
               // }
            }
            catch(Exception ex)
            {
                LogWriter.WriteToLog(ex);
				Response.Redirect("/NotAllowed",true);
			}
        }
        public void OnGetSelectedDt(string dt)
        {
            try
            {
                var dtcheck_1 = dtFrom_;
                var dtcheck_2 = dt;

                if(dtcheck_1 == dtcheck_2)
                {
                    ///OKAY////
                }
                else
                {
                    dtFrom_ = dtcheck_2;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
        public static String GetFromIntlDate(String DDMMYYYY)
        {
            try
            {
                if (isIntlDate(DDMMYYYY, "-") == false)
                {
                    char[] delimiterChars = { '/', '-' };
                    string DD = "", MM = "", YYYY = "";
                    string[] DateArray = DDMMYYYY.Split(delimiterChars);
                    DD = DateArray[0];
                    MM = DateArray[1];
                    YYYY = DateArray[2];
                    return (YYYY + "-" + MM + "-" + DD + " 00:00:00");
                }
                else
                {
                    return (DDMMYYYY + " 00:00:00");
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return "";
            }
        }
        public static Boolean isIntlDate(String DateString, String Delimiter)
        {
            try
            {
                Int32 DelimiterPlace = -1;
                DelimiterPlace = DateString.IndexOf(Delimiter);
                if (DelimiterPlace > 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return false;
            }
        }
        private void BindBankDD()
        {
            try
            {
                // Bank
                string sUserId = userid;//Convert.ToString(Session["myid"]).Trim();
                string sInstId = bankcode;//Session["BankCode"].ToString();
                DataTable dtInst = new DataTable();
                InstRequest req = new InstRequest()
                {
                    UserId = sUserId,
                    InstId = sInstId
                };

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL_Brbd);

                    var postTask = client.PostAsJsonAsync("GetInstBank", req);
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
                string CityCount = "";
                string sUserId = userid;//Convert.ToString(Session["myid"]).Trim();
                DataTable dtCity = new DataTable();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL_Brbd);
                    CityRequest req = new CityRequest
                    {
                        userID = sUserId
                    };
                    var postTask = client.PostAsJsonAsync("GetUserCities", req);
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
                    client.BaseAddress = new Uri(apiURL_Brbd);

                    var postTask = client.GetAsync("GetCycleList");
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
        private List<Branch> BindBranch(string bank_id, string city_id)
        {
            try
            {
                //Branch
                Br_ = new List<Branch>();
                DataTable dtBranch = new DataTable();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL_Brbd);
                    BranchboardBranchRequest req = new BranchboardBranchRequest
                    {
                        BankCode = bankcode,
                        BankId = Convert.ToInt32(bank_id),
                        CityId = Convert.ToInt32(city_id),
                        UserId = Convert.ToInt32(userid),
                    };

                    var postTask = client.PostAsJsonAsync("GetBranch", req);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<BranchResponse>();
                        readTask.Wait();

                        //if (readTask.Result.responseMessage.ToLower() == "success")
                        branches = readTask.Result;

                        foreach (var item in branches.branches)
                        {
                            Br_.Add(item);
                        }
                    }
                    return Br_;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception on BindBranch - " + ex);
                return Br_;
            }

        }
        private void BindReturnDD(string DDL_bank, string DDL_City, string DDL_cycle)
        {
            try
            {
                
                // Bank
                //    string CycleValue = "26887";//Convert.ToString(Session["myid"]).Trim();
                //   string CityValue = "54";//Session["BankCode"].ToString();
                string def_cycno = DDL_cycle;
                string def_bkcode = DDL_bank;
                string def_cityid = DDL_City;

                DataTable dtInst = new DataTable();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl_mark);
                    BindReturnRequest req = new BindReturnRequest()
                    {
                        def_cycno = def_cycno,
                        def_bkcode = def_bkcode,
                        def_cityid = def_cityid,
                        dt_ = dtFrom_
                    };

                    var postTask = client.PostAsJsonAsync("BindReturnDD", req);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<BindReturnDDResponse>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                            ReturnBanks = readTask.Result.BindReturn;
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
        private bool IsCycleClose(string Bank, string def_cycno, string def_cityid)
        {
            try
            {
                string sCyc = string.Empty;
                string sEnd = "00:00";
                string inString = System.DateTime.Now.ToString();
                string sBankCode = Bank;
                int iBankCode = 0;
                bool isParsable = Int32.TryParse(sBankCode, out iBankCode);
                DataTable dtCyc = new DataTable();
                List<IsCycleCloseReturn> isCycleCloResponses = new List<IsCycleCloseReturn>();
                IsCycleCloRequest req = new IsCycleCloRequest();
                req.cycno = def_cycno;
                req.bkcode = sBankCode;
                req.cityid = def_cityid;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl_auth);

                    var postTask = client.PostAsJsonAsync("IsCycleClose", req);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<IsCycleCloResponse>();
                        readTask.Wait();

                        isCycleCloResponses = readTask.Result.IsCycleClose;

                        if(isCycleCloResponses == null)
                        {
                            return true;
                        }

                        if(isCycleCloResponses.Count == 0)
                        {
                            return true;
                        }
                        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(IsCycleCloseReturn));

                        foreach (PropertyDescriptor p in props)
                            dtCyc.Columns.Add(p.Name, p.PropertyType);
                        foreach (var c in isCycleCloResponses)
                            dtCyc.Rows.Add(c.cycleNo, c.cycle_desc, c.cycleStartTime, c.cycleEndTime);
                    }
                }
                if (dtCyc == null)
                {
                    return true;
                }
                if (dtCyc.Rows.Count > 0)
                {
                    sEnd = dtCyc.Rows[0]["cycleEndTime"].ToString();
                    sCyc = dtCyc.Rows[0]["cycle_desc"].ToString();
                    if (sEnd == "00:00")
                        return true;
                }
                else
                {
                    return true;
                }
                DateTime rTime;
                bool isTime = DateTime.TryParse(sEnd, out rTime);
                if (!isTime)
                {
                    return true;
                }
                DateTime dt = DateTime.Now;
                if (dtFrom_ != null)
                {
                    string dt_ = dtFrom_;
                    string d1 = dtFrom_.Substring(0, 10);
                    string[] aStartDate = d1.ToString().Split('-');
                    string d2 = aStartDate[2] + "-" + aStartDate[1] + "-" + aStartDate[0];
                    inString = d2 + " " + sEnd;
                }
                else
                {
                    inString = dt.ToString("dd-MM-yyyy") + " " + sEnd;
                }
                //
                string sTime = DateTime.Now.ToString("dd-MM-yyyy") + " " + DateTime.Now.ToString("HH:mm");
                string eTime = inString;

                ///////// DOLLAR/INTERCITY //////////
                if (def_cycno == "20")
                {
                    sTime = inString.Substring(0, 10) + " " + DateTime.Now.ToString("HH:mm");
                }

                DateTime sdt = System.DateTime.Now;
                DateTime edt = System.DateTime.Now;
                if (DateTime.TryParseExact(sTime, "dd-MM-yyyy HH:mm", null, DateTimeStyles.None, out sdt))
                {

                }
                if (DateTime.TryParseExact(eTime, "dd-MM-yyyy HH:mm", null, DateTimeStyles.None, out edt))
                {

                }
                // For Test
                //edt = edt.AddDays(230);
                //

                TimeSpan span = edt.Subtract(sdt);
                if (span.Ticks < 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return true;
            }
        }
        private bool IsDefferAllowed(string bankcode_,string sCity, string sCycle)
        {
            try
            {
                string sCyc = string.Empty;
                string sEnd = "00:00";
                string inString = System.DateTime.Now.ToString();
                string sBankCode = bankcode_;
                int iBankCode = 0;
                bool isParsable = Int32.TryParse(sBankCode, out iBankCode);

                DataTable dtCyc = new DataTable();

                List<IsCycleCloseReturn> isCycleCloResponses = new List<IsCycleCloseReturn>();
                BindReturnRequest req = new BindReturnRequest();
                req.def_cycno = sCycle;
                req.def_bkcode = sBankCode;
                req.def_cityid = sCity;
                req.dt_ = dtFrom_;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl_auth);

                    var postTask = client.PostAsJsonAsync("IsDefferClose", req);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<IsCycleCloResponse>();
                        readTask.Wait();

                        isCycleCloResponses = readTask.Result.IsCycleClose;

                        if (isCycleCloResponses == null)
                        {
                            return true;
                        }

                        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(IsCycleCloseReturn));

                        foreach (PropertyDescriptor p in props)
                            dtCyc.Columns.Add(p.Name, p.PropertyType);
                        foreach (var c in isCycleCloResponses)
                            dtCyc.Rows.Add(c.cycleNo, c.cycle_desc, c.cycleStartTime, c.cycleEndTime);
                    }
                }

                if (dtCyc == null)
                    return true;

                if (dtCyc.Rows.Count > 0)
                {
                    sEnd = dtCyc.Rows[0]["cycleEndTime"].ToString();
                    sCyc = dtCyc.Rows[0]["cycle_desc"].ToString();
                    if (sEnd == "00:00")
                        return true;
                }
                else
                {
                    return true;
                }
                DateTime rTime;
                bool isTime = DateTime.TryParse(sEnd, out rTime);
                if (!isTime)
                {
                    return true;
                }
                DateTime dt = DateTime.Now;
                if (dtFrom_ != null)
                {
                    string d1 = dtFrom_.Substring(0, 10);
                    string[] aStartDate = d1.ToString().Split('-');
                    string d2 = aStartDate[2] + "-" + aStartDate[1] + "-" + aStartDate[0];
                    inString = d2 + " " + sEnd;
                }
                else
                {
                    inString = dt.ToString("dd-MM-yyyy") + " " + sEnd;
                }
                //
                string sTime = DateTime.Now.ToString("dd-MM-yyyy") + " " + DateTime.Now.ToString("HH:mm");
                string eTime = inString;

                ///////// DOLLAR/INTERCITY //////////
                if (sCycle == "20")
                {
                    sTime = inString.Substring(0, 10) + " " + DateTime.Now.ToString("HH:mm");
                }

                DateTime sdt = System.DateTime.Now;
                DateTime edt = System.DateTime.Now;
                if (DateTime.TryParseExact(sTime, "dd-MM-yyyy HH:mm", null, DateTimeStyles.None, out sdt))
                {

                }
                if (DateTime.TryParseExact(eTime, "dd-MM-yyyy HH:mm", null, DateTimeStyles.None, out edt))
                {

                }
                // For Test
                //edt = edt.AddDays(230);
                //

                TimeSpan span = edt.Subtract(sdt);
                if (span.Ticks < 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return false;
            }
        }
        public bool updateReturnDB(string sReasonid, string sReason)
        {
            try
            {
                bool updreturn_ = false;
				bool updreturnlog_ = false;
				updateReturnDBRequest req = new updateReturnDBRequest();
				insertReturnLogRequest req_ = new insertReturnLogRequest();
				DataTable dt = new DataTable();
                string sUserId = userid; //Convert.ToString(Session["myid"]).Trim();

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl_mark);

                    //for (int i = 0; i < dt.Rows.Count; i++)
                    //{
                        String sHostID = HttpContext.Session.GetString(SessionKeyName1);

                        if (sHostID != null)
                        {
                            req.sHostID = sHostID;
                            req.sUserId = sUserId;
                            req.sReasonid = sReasonid;
                            req.sReason = sReason;

                            var postTask = client.PostAsJsonAsync<updateReturnDBRequest>("updateReturnDB", req);
                            postTask.Wait();

                            var result = postTask.Result;
                            if (result.IsSuccessStatusCode)
                            {
                                var readTask = result.Content.ReadAsAsync<updateReturnDBResponse>();
                                readTask.Wait();

                                if (readTask.Result.responseMessage.ToLower() == "success")
                                    updreturn_ = readTask.Result.updateReturn;
                            }

                        ///INSERT RETURN LOG////
                        req_.sHostID = sHostID;
                        req_.sUserId = sUserId;
                        req_.sDetail = sReason;
                        req_.reasonid = sReasonid;

						postTask = client.PostAsJsonAsync<insertReturnLogRequest>("insertReturnLog", req_);
						postTask.Wait();
						result = postTask.Result;
						if (result.IsSuccessStatusCode)
						{
							var readTask_ = result.Content.ReadAsAsync<insertReturnLogResponse>();
							readTask_.Wait();

							if (readTask_.Result.responseMessage.ToLower() == "success")
                            {
                                //Success
                            }
						}
					}
                        else
                        {
                            return false;
                        }
                    //}
                }

                return true;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);

                return false;
            }
        }
        public class branchboarddetails
        {
            public string? bkcode { get; set; }
            public string? citycode { get; set; }
            public string? cycle { get; set; }
            public string? brcode { get; set; }
        }
        public JsonResult OnPostMarkingFiles([FromBody] branchboarddetails brd)
        {
            try
            {
                //string DDL_Bank = "", DDL_City = "", DDL_Cycle = "", DDL_Branch = "";
                string pdate = dtFrom_.Substring(0, 10);//"2023-08-02";
                string DDL_Bank = brd.bkcode;//"2";
                string DDL_Branch = brd.brcode;//"72";
                string DDL_Cycle = brd.cycle;//"54";
                string DDL_City = brd.citycode;///212";
                DataTable dtSort = new DataTable();
                // DataTable dt = new DataTable();
                DataTable _imbrTable = new DataTable();
                oDt = new DataTable();

                // Inward Return

                oDt = new DataTable();
                TransactionwiseInwardRequest reqtwise = new TransactionwiseInwardRequest();
                reqtwise.pdate = dtFrom_.Substring(0, 10);
                reqtwise.bkcode = DDL_Bank;
                reqtwise.brcode = DDL_Branch;
                reqtwise.cyclecode = DDL_Cycle;
                reqtwise.citycode = DDL_City;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl_mark);
                    var postTask = client.PostAsJsonAsync<TransactionwiseInwardRequest>("TransactionwiseInward", reqtwise);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<TransactionwiseInwardResponse>();
                        readTask.Wait();

                        //if (readTask.Result.responseMessage.ToLower() == "success")
                        objtwise = readTask.Result.TransactionWInward;

                        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(TransactionwiseInwardReturn));

                        foreach (PropertyDescriptor p in props)
                            oDt.Columns.Add(p.Name, p.PropertyType);
                        foreach (var c in objtwise)
                            oDt.Rows.Add(c.hostID, c.SBkId, c.SBkNm, c.SBrId, c.SBrNm, c.RBkId, c.RBkNm, c.RBrId, c.RBrNm, c.TrCode, c.CycleId, c.CycleDesc, c.ChqNo, c.AccNo,
                                c.TrId, c.IWOWID, c.ClgType, c.CityId, c.CityName, c.Amount, c.IQA, c.UV, c.Bar,
                                c.MICR, c.Std, c.Dup, c.WMark, c.AvgChqSize
                                , c.TChq, c.TChqR, c.Tempid, c.Uvperc, c.ReasonID, c.Reason, c.TruncTime
                                , c.TruncBy, c.Settled, c.SetTime, c.SetBy, c.AuthBy, c.AuthDateTime
                                , c.isAuth, c.Undersize_Image, c.Folded_or_Torn_Document_Corners, c.Folded_or_Torn_Document_Edges, c.Framing_Error, c.Document_Skew, c.Oversize_image, c.Piggy_Back, c.Image_Too_Light,
                                c.Image_Too_Dark, c.Horizontal_Streaks, c.Below_Minimum_Compressed_Image_Size, c.Above_Maximum_Compressed_Image_Size, c.Spot_Noise
                                , c.Front_Rear_Dimension_Mismatch, c.Carbon_Strip, c.Out_of_Focus,
                                c.ReasonID2, c.Reason2, c.reasonid_inward, c.ReasonID3, c.Reason3, c.Comment1, c.Comment2, c.Comment3, c.isDeffer, c.Comment1By, c.Comment2By,
                                c.Comment3By, c.Comment1Date, c.Comment2Date, c.Comment3Date, c.bDownload);
                    }
                }
                if (oDt == null)
                {
                    //return;
                }
                //
                if (oDt.Rows.Count > 0)
                {
                    string sTruncBy = oDt.Rows[0]["AuthBy"].ToString();
                    string sUserID = userid;
                    if (sTruncBy == sUserID)
                    {
                        string sMsg = "Maker & Checker can not be same Person";

                        //return;
                    }
                    try
                    {
                        _imbrTable = oDt;
                    }
                    catch
                    {
                        _imbrTable = null;
                    }

                    int aCount = 0;
                    decimal aAmt = 0;
                    decimal oAmt1 = 0;
                    int sCount = 0;
                    decimal sAmt = 0;
                    decimal oAmt2 = 0;

                    string strAmt = string.Empty;

                    DataTable dt = new DataTable();
                    dt = oDt;

                    if (dt.Rows.Count <= 0)
                    {
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strAmt = string.Empty;
                        oAmt1 = 0;
                        oAmt2 = 0;

                        string sPay = dt.Rows[i]["ReasonId"].ToString();

                        if (sPay == "0")
                        {
                            strAmt = dt.Rows[i]["Amount"].ToString();
                            bool isParsable = decimal.TryParse(strAmt, out oAmt1);
                            if (isParsable)
                            {
                                aAmt = aAmt + oAmt1;
                                aCount++;
                            }
                        }
                        else
                        {
                            strAmt = dt.Rows[i]["Amount"].ToString();
                            bool isParsable = decimal.TryParse(strAmt, out oAmt2);
                            if (isParsable)
                            {
                                sAmt = sAmt + oAmt2;
                                sCount++;
                            }
                        }
                    }
                    DataTable dtSum = new DataTable();
                    DataTable dtSum2 = new DataTable();
                    dtSum.Columns.Add("aCount", System.Type.GetType("System.Int32"));
                    dtSum.Columns.Add("aAmount", System.Type.GetType("System.Decimal"));
                    dtSum2.Columns.Add("sCount", System.Type.GetType("System.Int32"));
                    dtSum2.Columns.Add("sAmount", System.Type.GetType("System.Decimal"));
                    dtSum.Rows.Add(aCount, aAmt);
                    dtSum2.Rows.Add(sCount, sAmt);

                    dts1 = CommonMethod.ConvertToList<dtsum_>(dtSum);

                    dts2 = CommonMethod.ConvertToList<dtsum2_>(dtSum2);
                }
                BindReturnDD(brd.bkcode,brd.citycode,brd.cycle);
                 var value = new { objtwise, dts1, dts2, ReturnBanks };
                return new JsonResult(value);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return new JsonResult("Failed");
            }
        }
        public List<TransactionwiseInwardReturn> GetFiles(string DDL_Bank, string DDL_City, string DDL_Cycle, string DDL_Branch)
        {
            try
            {
                string pdate = dtFrom_.Substring(0,10);//"2023-08-02";
                string bkcode = DDL_Bank;//"2";
                string brcode = DDL_Branch;//"72";
                string cyclecode = DDL_Cycle;//"54";
                string citycode = DDL_City;///212";
                DataTable dtSort = new DataTable();
                // DataTable dt = new DataTable();
                DataTable _imbrTable = new DataTable();
                oDt = new DataTable();

                // Inward Return

                oDt = new DataTable();
                TransactionwiseInwardRequest reqtwise = new TransactionwiseInwardRequest();
                reqtwise.pdate = dtFrom_.Substring(0, 10);
                reqtwise.bkcode = DDL_Bank;
                reqtwise.brcode = DDL_Branch;
                reqtwise.cyclecode = DDL_Cycle;
                reqtwise.citycode = DDL_City;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl_mark);
                    var postTask = client.PostAsJsonAsync<TransactionwiseInwardRequest>("TransactionwiseInward", reqtwise);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<TransactionwiseInwardResponse>();
                        readTask.Wait();

                        //if (readTask.Result.responseMessage.ToLower() == "success")
                        objtwise = readTask.Result.TransactionWInward;

                        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(TransactionwiseInwardReturn));

                        foreach (PropertyDescriptor p in props)
                            oDt.Columns.Add(p.Name, p.PropertyType);
                        foreach (var c in objtwise)
                            oDt.Rows.Add(c.hostID, c.SBkId, c.SBkNm, c.SBrId, c.SBrNm, c.RBkId, c.RBkNm, c.RBrId, c.RBrNm, c.TrCode, c.CycleId, c.CycleDesc, c.ChqNo, c.AccNo,
                                c.TrId, c.IWOWID, c.ClgType, c.CityId, c.CityName, c.Amount , c.IQA, c.UV, c.Bar,
                                c.MICR, c.Std, c.Dup, c.WMark, c.AvgChqSize
                                , c.TChq, c.TChqR, c.Tempid, c.Uvperc, c.ReasonID, c.Reason, c.TruncTime
                                , c.TruncBy, c.Settled, c.SetTime, c.SetBy, c.AuthBy, c.AuthDateTime
                                , c.isAuth, c.Undersize_Image, c.Folded_or_Torn_Document_Corners, c.Folded_or_Torn_Document_Edges, c.Framing_Error, c.Document_Skew, c.Oversize_image, c.Piggy_Back, c.Image_Too_Light,
                                c.Image_Too_Dark, c.Horizontal_Streaks, c.Below_Minimum_Compressed_Image_Size, c.Above_Maximum_Compressed_Image_Size, c.Spot_Noise
                                , c.Front_Rear_Dimension_Mismatch, c.Carbon_Strip, c.Out_of_Focus,
                                c.ReasonID2, c.Reason2, c.reasonid_inward, c.ReasonID3, c.Reason3, c.Comment1, c.Comment2, c.Comment3, c.isDeffer, c.Comment1By, c.Comment2By,
                                c.Comment3By, c.Comment1Date, c.Comment2Date, c.Comment3Date, c.bDownload);
                    }
                }
                if (oDt == null)
                {
                    //return;
                }
                //
                if (oDt.Rows.Count > 0)
                {
                    string sTruncBy = oDt.Rows[0]["AuthBy"].ToString();
                    string sUserID = userid;
                    if (sTruncBy == sUserID)
                    {
                        string sMsg = "Maker & Checker can not be same Person";
                        
                        //return;
                    }
                    try
                    {
                        _imbrTable = oDt;
                    }
                    catch
                    {
                        _imbrTable = null;
                    }

                    int aCount = 0;
                    decimal aAmt = 0;
                    decimal oAmt1 = 0;
                    int sCount = 0;
                    decimal sAmt = 0;
                    decimal oAmt2 = 0;

                    string strAmt = string.Empty;

                    DataTable dt = new DataTable();
                    dt = oDt;

                    if (dt.Rows.Count <= 0)
                    {
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strAmt = string.Empty;
                        oAmt1 = 0;
                        oAmt2 = 0;

                        string sPay = dt.Rows[i]["ReasonId"].ToString();

                        if (sPay == "0")
                        {
                            strAmt = dt.Rows[i]["Amount"].ToString();
                            bool isParsable = decimal.TryParse(strAmt, out oAmt1);
                            if (isParsable)
                            {
                                aAmt = aAmt + oAmt1;
                                aCount++;
                            }
                        }
                        else
                        {
                            strAmt = dt.Rows[i]["Amount"].ToString();
                            bool isParsable = decimal.TryParse(strAmt, out oAmt2);
                            if (isParsable)
                            {
                                sAmt = sAmt + oAmt2;
                                sCount++;
                            }
                        }
                    }
                    DataTable dtSum = new DataTable();
                    DataTable dtSum2 = new DataTable();
                    dtSum.Columns.Add("aCount", System.Type.GetType("System.Int32"));
                    dtSum.Columns.Add("aAmount", System.Type.GetType("System.Decimal"));
                    dtSum2.Columns.Add("sCount", System.Type.GetType("System.Int32"));
                    dtSum2.Columns.Add("sAmount", System.Type.GetType("System.Decimal"));
                    dtSum.Rows.Add(aCount, aAmt);
                    dtSum2.Rows.Add(sCount, sAmt);

                    dts1 = CommonMethod.ConvertToList<dtsum_>(dtSum);

                    dts2 = CommonMethod.ConvertToList<dtsum2_>(dtSum2);
                }

                return objtwise;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return objtwise;
            }
        }
        public static class CommonMethod
        {
            public static List<T> ConvertToList<T>(DataTable dt)
            {
                var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
                var properties = typeof(T).GetProperties();
                return dt.AsEnumerable().Select(row => {
                    var objT = Activator.CreateInstance<T>();
                    foreach (var pro in properties)
                    {
                        if (columnNames.Contains(pro.Name.ToLower()))
                        {
                            try
                            {
                                pro.SetValue(objT, row[pro.Name]);
                            }
                            catch (Exception ex) { }
                        }
                    }
                    return objT;
                }).ToList();
            }
        }
        public JsonResult OnPostMarkReturnClick(string reasonid, string reasondesc,string otherreason, string hostid,string DDL_Bank, string DDL_Branch, string DDL_City, string DDL_cycle)
         {
            try
            {
                string json = string.Empty;
                string sReasonId = string.Empty;
                string sReason = string.Empty;
                bool df = false;
                string sMsg;
                string pdate = dtFrom_.Substring(0, 10);
                String sHostID = HttpContext.Session.GetString(SessionKeyName1);
                if (sHostID == null)
                {
                    LogWriter.WriteToLog("Failed to Mark Return By - " + userid + " - " + bankcode);
                    return new JsonResult("AuthFailed");
                }
                DataTable dtSort = new DataTable();
                DataTable _imbrTable = new DataTable();
                oDt = new DataTable();
                using (var client = new HttpClient())
                {
                    CheckSendForReviewRequest reqR = new CheckSendForReviewRequest
                    {
                        hostid = sHostID,
                        pdate = pdate,
                        bkcode = DDL_Bank,
                        brcode = DDL_Branch,
                        cyclecode = DDL_cycle,
                        citycode = DDL_City,
                    };
                    //HTTP GET
                    client.BaseAddress = new Uri(apiUrl_auth);
                    var responseTask = client.PostAsJsonAsync("ReturnCheckIsAuthorized", reqR);
                    responseTask.Wait();

                    var resultLoad = responseTask.Result;
                    if (resultLoad.IsSuccessStatusCode)
                    {

                        var postTask = resultLoad.Content.ReadAsAsync<ReturnCheckIsAuthorizedResponse>();
                        postTask.Wait();

                        // if (readTask.Result.responseMessage.ToLower() == "success")
                        obj = postTask.Result.CheckIsAuthorized;

                    }

                    if (obj.Count > 0)
                    {
                        LogWriter.WriteToLog("Failed to Mark Return Because Return is Already Authorized By - " + userid + " - " + bankcode);
                        sMsg = "Return is Already Authorize";
                        return new JsonResult("AuthFailed");
                        //lblMsg.Text = "Return is Already Authorize";
                        //Session["IMBrTable"] = null;
                        //Session["IMFlTable"] = null;
                        //BindMainGrid();
                        //return;


                        //string sTruncBy = oDt.Rows[0]["AuthBy"].ToString();
                        //string sUserID = userid;//"26887";// Session["myid"].ToString();
                        //if (sTruncBy == sUserID)
                        //{
                        //    sMsg = "Maker & Checker can not be same Person";
                        //    return new JsonResult(sMsg);
                        //}
                        //try
                        //{
                        //    _imbrTable = oDt;
                        //}
                        //catch
                        //{
                        //    _imbrTable = null;
                        //}
                    }
                }
                if (reasonid == "900")
                {
                    DataTable dtRtn = new DataTable();
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(apiUrl_mark);

                        HttpResponseMessage resp = client.GetAsync("DeferForPhysical").Result;

                        if (resp.IsSuccessStatusCode)
                        {
                            json = resp.Content.ReadAsStringAsync().Result;
                            df = bool.TryParse(json, out df);
                        }
                    }
                    if (df == false)
                    {
                        LogWriter.WriteToLog("Failed to Mark Return Because Defer is Closed - " + userid + " - " + bankcode);
                        string sMsg1 = "Defer is Closed, You can not Save Data";
                    }
                }

                if (IsCycleClose(DDL_Bank, DDL_cycle, DDL_City))
                {
                    LogWriter.WriteToLog("Failed to Mark Return Because Cycle is Closed - " + userid + " - " + bankcode);
                    string sMsg2 = "Cycle is Closed, You can not Save Data";
                    return new JsonResult("CFailed");
                }

                 if (reasonid == "900")
                {
                    if (IsDefferAllowed(DDL_Bank, DDL_City, DDL_cycle))
                    {
                        LogWriter.WriteToLog("Failed to Mark Return Because Defer is Closed - " + userid + " - " + bankcode);
                        string sMsg5 = "Defer is Closed, You can only View Data";
                        return new JsonResult("DFailed");
                    }
                }
                if (reasonid == "801")
                {
                    if (otherreason == string.Empty || otherreason == null)
                    {
                        LogWriter.WriteToLog("Failed to Mark Return Because Other reason is blank - " + userid + " - " + bankcode);
                        string sMsg3 = "Please Select Other Reason Code or Clear Other Reason Textbox";
                        string sMsg4 = "Other Reason Can not be Blank";
                        return new JsonResult("Rfailed");
                    }
                    sReasonId = reasonid;
                    sReason = otherreason;

                    bool ress_ = updateReturnDB(sReasonId, sReason);
                    if(ress_ == true)
                    {
                        LogWriter.WriteToLog("Successfully Marked Return By - " + userid + " - " + bankcode + "For Host ID : " + hostid);
                        return new JsonResult("Success");
                    }
                    else
                    {
                        return new JsonResult("Failed");
                    }
                }

                // update DateTable
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();

                //for (int i = 0; i < dt.Rows.Count; i++)
                //{
                //    if (dt.Rows[i][0].ToString() == dt1.Rows[0][0].ToString())
                //    {
                //        dt.Rows[i]["ReasonID"] = ReturnBanks;
                //        sReasonId = ReturnBanks;

                //        if (objReason.ddReason.ToString() == "801")
                //        {
                //            dt.Rows[i]["Reason"] = ReturnBanks;
                //            sReason = ReturnBanks;
                //        }
                //        else
                //        {
                //            dt.Rows[i]["Reason"] = ReturnBanks;
                //            sReason = ReturnBanks;
                //        }
                //        dt.AcceptChanges();
                //        break;
                //    }
                //}

                sReasonId = reasonid;
                sReason = reasondesc;
                //Update GridView
                //for (int i = 0; i < Rows.Count; i++)
                //{
                //    if (dt.Rows[i][0].ToString() == sHostId)
                //    {
                //        if (objReason.ddReason.ToString() == "801")
                //        {
                //            GridView1.Rows[i].Cells[1].Text = objReason.txtReason;
                //        }
                //        else
                //        {
                //            GridView1.Rows[i].Cells[1].Text = objReason.ddReason;
                //        }
                //        break;
                //    }
                //}
                //calcSum();
                // update DB
                bool result = updateReturnDB(sReasonId, sReason);
                //GetFiles();

                if (result == true)
                {
                    LogWriter.WriteToLog("Successfully Marked Return By - " + userid + " - " + bankcode + "For Host ID : " + hostid);
                    return new JsonResult("Success");
                }
                else
                {
                    return new JsonResult("Failed");
                }
            }
            catch(Exception ex) {
                LogWriter.WriteToLog(ex);
                return new JsonResult("CFailed");
            }

        }
        public JsonResult OnGetUpdateComment(string comments, string DDL_Bank, string DDL_City, string DDL_Cycle, string hostid)
        {
            try
            {
                bool res_ = false;
                if (comments != null && comments != "")
                {
                    if (comments.Length >= 30)
                    {
                        comments = comments.Substring(0, 29);
                    }
                    if (IsCycleClose(DDL_Bank, DDL_Cycle, DDL_City))
                    {
                        LogWriter.WriteToLog("Failed to update comments ! Cycle is Closed - " + userid + " - " + bankcode);
                        string sMsg2 = "Cycle is Closed, You can not Save Data";
                        return new JsonResult("CFailed");
                    }

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(apiUrl_mark);

                        var postTask = client.GetAsync($"GetUpdateComment?comment={comments}&userid={userid}&hostid={hostid}");
                        postTask.Wait();

                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<insertReturnLogResponse>();
                            readTask.Wait();

                            if (readTask.Result.responseMessage.ToLower() == "success")
                            {
                                res_ = readTask.Result.instRtnLog;
                            }
                        }
                        if (res_ == true)
                        {
                            ReturnLogDB(hostid, "Maker Comment=" + comments);
                            LogWriter.WriteToLog("Successfully Updated comment By Maker - " + userid + " - " + bankcode);
                            return new JsonResult("Success");
                        }
                        else
                        {
                            LogWriter.WriteToLog("Failed to Update comment By Maker - " + userid + " - " + bankcode);
                            return new JsonResult("Failed");
                        }
                    }
                }
                return new JsonResult("NFailed");
            }
            catch(Exception ex)
            {
                LogWriter.WriteToLog("Failed to Update comment By Maker - " + userid + " - " + bankcode);
                LogWriter.WriteToLog(ex);
                return new JsonResult("Failed");
            }
        }
        public bool ReturnLogDB(string hostID, string sReason)
        {
            try
            {
                insertReturnLogRequest req_ = new insertReturnLogRequest();
                DataTable dt = new DataTable();
                string sUserId = userid;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl_mark);

                    if (hostID != null)
                    {
                        ///INSERT RETURN LOG////
                        req_.sHostID = hostID;
                        req_.sUserId = sUserId;
                        req_.sDetail = sReason;

                        var postTask = client.PostAsJsonAsync<insertReturnLogRequest>("insertReturnLog_auth", req_);
                        postTask.Wait();
                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask_ = result.Content.ReadAsAsync<insertReturnLogResponse>();
                            readTask_.Wait();

                            if (readTask_.Result.responseMessage.ToLower() == "success")
                            {
                                //Success
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                    //}
                }

                return true;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);

                return false;
            }
        }
        public JsonResult OnGetSelectedcity(string bank, string city)
        {
            try
            {

                if (city != null && bank != null)
                {
                    selected_bank = bank;
                    selected_city = city;
                    BindBranch(selected_bank, selected_city);
                    return new JsonResult(Br_);
                }
                return new JsonResult("Failed");
            }
            catch
            {
                return new JsonResult("Failed");
            }
        }
        public JsonResult OnGetSelectedbank(string bank, string city)
        {
            try
            {
                if (city != null && bank != null)
                {
                    selected_bank = bank;
                    selected_city = city;
                    BindBranch(selected_bank, selected_city);
                    return new JsonResult(Br_);
                }
                return new JsonResult("Failed");
            }
            catch
            {
                return new JsonResult("Failed");
            }
        }
        public JsonResult OnGetSingleData(SingleReturn objSgl)
        {
            try
            {
                string sHostId = objSgl.hostID.ToString();
                DataTable dt2 = new DataTable();
                string condition = "hostID = " + "'" + sHostId + "'";
                oDt.DefaultView.RowFilter = condition;
                dt2 = oDt.DefaultView.ToTable();
                return new JsonResult("Success");
            }
            catch
            {
                return new JsonResult("Failed");
            }
        }
        //public void OnGetImage_img(string hostid)
        //{
        //    string sHostId = hostid;//"156937";
        //    int userLogId = Convert.ToInt32(userlogid);
        //    string sUserId = userid;//"26887";

        //    using (var client1 = new HttpClient())
        //    {

        //        var req = new LogImageFileRequest()
        //        {
        //            userId = Convert.ToInt32(sUserId),
        //            userLogId = userLogId,
        //            hostId = Convert.ToInt32(sHostId)
        //        };
        //        client1.BaseAddress = new Uri(apiURL_Brbd);


        //        var postTask = client1.PostAsJsonAsync<LogImageFileRequest>("LogImageFile", req);
        //        postTask.Wait();
        //        var result1 = postTask.Result;
        //    }
        //    Byte[] jpegBytes = null;
        //    using (var client1 = new HttpClient())
        //    {
        //        client1.BaseAddress = new Uri(apiURL_Brbd);

        //        var postTask = client1.GetAsync("GetImage?bFront=0&imageid=" + sHostId);
        //        postTask.Wait();
        //        var result1 = postTask.Result;
        //        if (result1.IsSuccessStatusCode)
        //        {
        //            var readTask1 = result1.Content.ReadAsAsync<Byte[]>();
        //            readTask1.Wait();
        //            jpegBytes = readTask1.Result;
        //            var base64Image = Convert.ToBase64String(jpegBytes);
        //            var imageDataUrl = $"data:image/jpeg;base64,{base64Image}";
        //            ViewData["ImageDataURL"] = imageDataUrl;
        //        }
        //    }

        //    using (var client2 = new HttpClient())
        //    {
        //        client2.BaseAddress = new Uri(apiURL_Brbd);

        //        var postTask = client2.GetAsync("GetImage?bFront=1&imageid=" + sHostId);
        //        postTask.Wait();

        //        var result2 = postTask.Result;
        //        if (result2.IsSuccessStatusCode)
        //        {
        //            var readTask2 = result2.Content.ReadAsAsync<Byte[]>();
        //            readTask2.Wait();

        //            //if (readTask.Result.responseMessage.ToLower() == "success")
        //            jpegBytes = readTask2.Result;
        //            var base64Image = Convert.ToBase64String(jpegBytes);
        //            var imageDataUrl = $"data:image/jpeg;base64,{base64Image}";
        //            ViewData["ImageDataURL2"] = imageDataUrl;
        //        }
        //    }

        //    using (var client3 = new HttpClient())
        //    {
        //        client3.BaseAddress = new Uri(apiURL_Brbd);

        //        var postTask = client3.GetAsync("GetImage?bFront=2&imageid=" + sHostId);
        //        postTask.Wait();

        //        var result3 = postTask.Result;
        //        if (result3.IsSuccessStatusCode)
        //        {
        //            var readTask3 = result3.Content.ReadAsAsync<Byte[]>();
        //            readTask3.Wait();
        //            jpegBytes = readTask3.Result;
        //            var base64Image = Convert.ToBase64String(jpegBytes);
        //            var imageDataUrl = $"data:image/jpeg;base64,{base64Image}";
        //            ViewData["ImageDataURL3"] = imageDataUrl;
        //        }
        //    }
        //}
        public JsonResult OnGetCheckImages(string hostId,string DDL_bank, string DDL_City, string DDL_Cycle)
        {
            try
            {
                string sHostId = hostId;
                HttpContext.Session.SetString(SessionKeyName1, sHostId);
                int userLogId = Convert.ToInt32(userlogid);
                string sUserId = userid;

                using (var client1 = new HttpClient())
                {
                    var req = new LogImageFileRequest()
                    {
                        userId = Convert.ToInt32(sUserId),
                        userLogId = userLogId,
                        hostId = Convert.ToInt32(sHostId)
                    };
                    client1.BaseAddress = new Uri(apiURL_Brbd);
                    var postTask = client1.PostAsJsonAsync<LogImageFileRequest>("LogImageFile", req);
                    postTask.Wait();
                    var result1 = postTask.Result;
                }
                Byte[] jpegBytes = null;
                var imageDataUrl1 = "";
                var imageDataUrl2 = "";
                var imageDataUrl3 = "";

                using (var client1 = new HttpClient())
                {
                    client1.BaseAddress = new Uri(img_apiURL);

                    var postTask = client1.GetAsync("GetImage?bFront=0&imageid=" + sHostId);
                    postTask.Wait();
                    var result1 = postTask.Result;
                    if (result1.IsSuccessStatusCode)
                    {
                        var readTask1 = result1.Content.ReadAsAsync<Byte[]>();
                        readTask1.Wait();
                        jpegBytes = readTask1.Result;
                        var base64Image = Convert.ToBase64String(jpegBytes);
                        imageDataUrl1 = $"data:image/jpeg;base64,{base64Image}";
                        ViewData["ImageDataURL"] = imageDataUrl1;
                    }
                }

                using (var client2 = new HttpClient())
                {
                    client2.BaseAddress = new Uri(img_apiURL);

                    var postTask = client2.GetAsync("GetImage?bFront=1&imageid=" + sHostId);
                    postTask.Wait();

                    var result2 = postTask.Result;
                    if (result2.IsSuccessStatusCode)
                    {
                        var readTask2 = result2.Content.ReadAsAsync<Byte[]>();
                        readTask2.Wait();

                        //if (readTask.Result.responseMessage.ToLower() == "success")
                        jpegBytes = readTask2.Result;
                        var base64Image = Convert.ToBase64String(jpegBytes);
                        imageDataUrl2 = $"data:image/jpeg;base64,{base64Image}";
                        ViewData["ImageDataURL2"] = imageDataUrl2;
                    }
                }

                using (var client3 = new HttpClient())
                {
                    client3.BaseAddress = new Uri(img_apiURL);

                    var postTask = client3.GetAsync("GetImage?bFront=2&imageid=" + sHostId);
                    postTask.Wait();

                    var result3 = postTask.Result;
                    if (result3.IsSuccessStatusCode)
                    {
                        var readTask3 = result3.Content.ReadAsAsync<Byte[]>();
                        readTask3.Wait();
                        jpegBytes = readTask3.Result;
                        var base64Image = Convert.ToBase64String(jpegBytes);
                        imageDataUrl3 = $"data:image/jpeg;base64,{base64Image}";
                        ViewData["ImageDataURL3"] = imageDataUrl3;
                    }
                }

                //DataTable dt2 = new DataTable();
                //string condition = "HostId = " + "'" + sHostId + "'";
                //oDt.DefaultView.RowFilter = condition;
                //dt2 = oDt.DefaultView.ToTable();
                return new JsonResult("Success|" + imageDataUrl1 + "|" + imageDataUrl2 + "|" + imageDataUrl3);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return new JsonResult("Failed");
            }
        }

        public JsonResult OnGetFilterOut(string IQA, string MICR, string STD, string Defer, string FRH, string QR, string UV, string Dup, string amt_min
                    , string amt_max, string chq, string acc, string DDL_Bank, string DDL_City, string DDL_Cycle, string DDL_Branch)
        {
            try
            {
                string cIQA = string.Empty;
                string cUV = string.Empty;
                string cBar = string.Empty;
                string cMICR = string.Empty;
                string cStd = string.Empty;
                string cDup = string.Empty;
                string cDeffer = string.Empty;
                string cMinamt = string.Empty;
                string cMaxamt = string.Empty;
                string cAmt = string.Empty;
                string cChqno = string.Empty;
                string cAccno = string.Empty;
                string cFraud = string.Empty;

                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();

                if (oDt != null)
                {
                    dt = oDt;

                    //IQA
                    if (IQA == "Pass")
                    {
                        cIQA = "IQA = 'Pass'";
                    }
                    else if (IQA == "Fail")
                    {
                        cIQA = "IQA = 'Fail'";
                    }
                    //UV
                    if (UV == "N/A")
                    {
                        cUV = "UV = 'N/A'";
                    }
                    else if (UV == "UVB.Avg")
                    {
                        cUV = "UV = 'B.Avg/Chk%'";
                    }
                    else if (UV == "UVA.Avg")
                    {
                        cUV = "UV = 'A.Avg/Chk%'";
                    }
                    // Bar
                    if (QR == "N/A")
                    {
                        cBar = "Bar = 'N/A'";
                    }
                    else if (QR == "Not Ok")
                    {
                        cBar = "Bar = 'Not Ok'";
                    }
                    else if (QR == "Ok")
                    {
                        cBar = "Bar = 'Ok'";
                    }
                    // MICR
                    if (MICR == "Pass")
                    {
                        cMICR = "MICR = 'Pass'";
                    }
                    else if (MICR == "Fail")
                    {
                        cMICR = "MICR = 'Fail'";
                    }
                    // Std
                    if (STD == "Yes")
                    {
                        cStd = "Std = 'Yes'";
                    }
                    else if (STD == "No")
                    {
                        cStd = "Std = 'No'";
                    }
                    // Dup
                    if (Dup == "Not Dup")
                    {
                        cDup = "Dup = 'Not Dup'";
                    }
                    else if (Dup == "Dup Cheque")
                    {
                        cDup = "Dup = 'Dup Cheque'";
                    }
                    else if (Dup == "Represent again")
                    {
                        cDup = "Dup = 'Represent again'";
                    }
                    //Defer
                    if (Defer == "Yes")
                    {
                        cDeffer = "IsDeffer = 'Yes'";
                    }
                    else if (Defer == "No")
                    {
                        cDeffer = "IsDeffer = 'No'";
                    }
                    //Fraud History
                    if (FRH == "Yes")
                    {
                        cFraud = " WMark = 'Yes'";
                    }
                    else if (FRH == "No")
                    {
                        cFraud = " WMark = 'No'";
                    }
                    // Amount          
                    try
                    {
                        if ((string.IsNullOrEmpty(amt_min)) && (!string.IsNullOrEmpty(amt_max)))
                        {
                            amt_min = amt_max;
                        }

                        if ((!string.IsNullOrEmpty(amt_min)) && (string.IsNullOrEmpty(amt_max)))
                        {
                            amt_max = amt_min;
                        }
                        if ((!string.IsNullOrEmpty(amt_min)) && (!string.IsNullOrEmpty(amt_max)))
                        {
                            Decimal minAmt = System.Convert.ToDecimal(amt_min);
                            Decimal maxAmt = System.Convert.ToDecimal(amt_max);
                            if (maxAmt >= minAmt)
                            {
                                cAmt = "Amount" + " >= " + minAmt + " and " + "Amount" + " <= " + maxAmt;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        LogWriter.WriteToLog(ex);
                    }
                    // Cheque No
                    if (!string.IsNullOrEmpty(chq))
                    {
                        String str = chq.TrimStart(new Char[] { '0' });
                        cChqno = "Chqno" + " = '" + str + "'";
                    }

                    // Account No
                    if (!string.IsNullOrEmpty(acc))
                    {
                        String str = acc.TrimStart(new Char[] { '0' });
                        cAccno = "Accno" + " = '" + str + "'";
                    }
                    if (cIQA != "")
                    {
                        dt.DefaultView.RowFilter = cIQA;
                        dt1 = dt.DefaultView.ToTable();
                        dt = dt1.DefaultView.ToTable();
                    }
                    if (cUV != "")
                    {
                        dt.DefaultView.RowFilter = cUV;
                        dt1 = dt.DefaultView.ToTable();
                        dt = dt1.DefaultView.ToTable();
                    }
                    if (cBar != "")
                    {
                        dt.DefaultView.RowFilter = cBar;
                        dt1 = dt.DefaultView.ToTable();
                        dt = dt1.DefaultView.ToTable();
                    }
                    if (cMICR != "")
                    {
                        dt.DefaultView.RowFilter = cMICR;
                        dt1 = dt.DefaultView.ToTable();
                        dt = dt1.DefaultView.ToTable();
                    }
                    if (cStd != "")
                    {
                        dt.DefaultView.RowFilter = cStd;
                        dt1 = dt.DefaultView.ToTable();
                        dt = dt1.DefaultView.ToTable();
                    }
                    if (cDup != "")
                    {
                        dt.DefaultView.RowFilter = cDup;
                        dt1 = dt.DefaultView.ToTable();
                        dt = dt1.DefaultView.ToTable();
                    }
                    if (cDeffer != "")
                    {
                        dt.DefaultView.RowFilter = cDeffer;
                        dt1 = dt.DefaultView.ToTable();
                        dt = dt1.DefaultView.ToTable();
                    }
                    if (cFraud != "")
                    {
                        dt.DefaultView.RowFilter = cFraud;
                        dt1 = dt.DefaultView.ToTable();
                        dt = dt1.DefaultView.ToTable();
                    }
                    if (cAmt != "")
                    {
                        dt.DefaultView.RowFilter = cAmt;
                        dt1 = dt.DefaultView.ToTable();
                        dt = dt1.DefaultView.ToTable();
                    }
                    if (cChqno != "")
                    {
                        dt.DefaultView.RowFilter = cChqno;
                        dt1 = dt.DefaultView.ToTable();
                        dt = dt1.DefaultView.ToTable();
                    }
                    if (cAccno != "")
                    {
                        dt.DefaultView.RowFilter = cAccno;
                        dt1 = dt.DefaultView.ToTable();
                        dt = dt1.DefaultView.ToTable();
                    }


                    //if ((!string.IsNullOrEmpty(amt_min)) && (!string.IsNullOrEmpty(amt_max)))
                    //{
                    //    DataTable dt2 = new DataTable();
                    //    dt2 = dt1.Clone();
                    //    dt2.Columns["Amount"].DataType = typeof(Int32);

                    //    foreach (DataRow dr in dt1.Rows)
                    //    {
                    //        dt2.ImportRow(dr);
                    //    }
                    //    dt2.AcceptChanges();


                    //    DataView dv = dt2.DefaultView;
                    //    dv.Sort = "Amount DESC";
                    //    dt = dv.ToTable();
                    //}

                    cIQA = string.Empty;
                    cUV = string.Empty;
                    cBar = string.Empty;
                    cMICR = string.Empty;
                    cStd = string.Empty;
                    cDup = string.Empty;
                    cDeffer = string.Empty;
                    cMinamt = string.Empty;
                    cMaxamt = string.Empty;
                    cAmt = string.Empty;
                    cChqno = string.Empty;
                    cAccno = string.Empty;
                    cFraud = string.Empty;

                    objtwise = CommonMethod.ConvertToList<TransactionwiseInwardReturn>(dt);

                    return new JsonResult(objtwise);
                }
                return new JsonResult("NullData");
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return new JsonResult("Failed");
            }
        }

        public JsonResult OnGetClearOut()
        {
            try
            {
                DataTable dt = new DataTable();

                if (oDt != null)
                {
                    dt = oDt;

                    objtwise = CommonMethod.ConvertToList<TransactionwiseInwardReturn>(dt);

                    return new JsonResult(objtwise);
                }
                return new JsonResult("NullData");
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return new JsonResult("Failed");
            }
        }
    }
}

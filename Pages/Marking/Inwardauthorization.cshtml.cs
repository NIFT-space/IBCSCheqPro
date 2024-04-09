using IBCS_Core_Web_Portal.Helper;
using IBCS_Web_Portal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Hosting;
using Nancy.Json;
using System.ComponentModel;
using System.Data;
using System.Globalization;

namespace IBCS_Core_Web_Portal.Pages.Marking
{
    public class InwardauthorizationModel : PageModel
    {
        public const string viewbranch_ = "_branch";
        public static string apiUrl_mark = "";
		public static string apiURL_Brbd = "";
		public static string apiUrl_auth = "";
        public static string apiUrl_summ = "";
        public static string img_apiURL = ""; 
        public static string userid = "";
        public static string userlogid = "";
        public static string bankcode = "";
        public static string branchcode = "";
        public static string dtFrom_ = "";
        public static string dtTo_ = "";
        public static string Auth_ = "";
        public List<LoadDataFromDBReturn> obj { get; set; }

        public List<LoadDataFromDBReturn> objAuth { get; set; }
        public List<RSLoadDFrDBResponse> obj1 { get; set; }
        public List<CheckSendForReviewReturn> obj4 { get; set; }
        public List<CheckSendForReviewReturn> obj5 { get; set; }
        public List<InwardAuthorizationReturn> objA { get; set; }
        public SingleInwardAuth SglAuth { get; set; }

        public static DataTable oDt;
        public static DataTable oDt1;
        public List<InstBank> instBanks { get; set; }
        public List<City> cities { get; set; }
        public List<Cycle> cycle { get; set; }

        public static string sel_ddlbank, sel_ddlcity, sel_ddlcycle;
        // string selectedTab;

        public void OnGet(string d1, string d2, string d3)
        {
            try
            {
                userid = HttpContext.Session.GetString(loginModel.SessionKeyName1);
                userlogid = HttpContext.Session.GetString(loginModel.SessionKeyName9);
                bankcode = HttpContext.Session.GetString(loginModel.SessionKeyName10);
                branchcode = HttpContext.Session.GetString(loginModel.SessionKeyName11);
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

                if (cpa.Validatepageallowed(userid, "Inwardauthorization") == false)
                {
                    Response.Redirect("/NotAllowed",true);
                }
                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
				apiUrl_mark = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "Inwardmarking/";
				apiURL_Brbd = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "Branchboard/";
                apiUrl_auth = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "Inwardauthorization/";
                apiUrl_summ = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "ReturnSummary/";
                //img_apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.img") + "GetImageFromBin/";
                img_apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "GetImageFromBin/";
                BindBankDD();
                BindCycleDD();
                BindCityDD();
                //InwardAuthDetails();
                if (d1 != null || d2 != null || d3 != null)
                {
                    //obj = GetFiles(d1, d2, d3);

                }
            }
            catch (Exception ex)
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

                if (dtcheck_1 == dtcheck_2)
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

        public JsonResult OnPostFetchFiles(string DDL_Bank, string DDL_City, string DDL_Cycle)
        {
            try
            {
                string pdate = dtFrom_;
                string bkcode = DDL_Bank;
                string cycleno = DDL_Cycle;
                string citycode = DDL_City;
                DataTable dtSort = new DataTable();
                DataTable dt = new DataTable();
                DataTable _imbrTable = new DataTable();
                DataTable _rsTable = new DataTable();
                oDt = new DataTable();
                DataTable dtBranch = new DataTable();
                using (var client = new HttpClient())
                {
                    LoadDataAuthDBRequest reqR = new LoadDataAuthDBRequest
                    {
                        pdate = pdate,
                        bkcode = bkcode,
                        cycleno = cycleno,
                        citycode = citycode,
                    };
                    //HTTP GET
                    client.BaseAddress = new Uri(apiUrl_auth);
                    var responseTask = client.PostAsJsonAsync("LoadDataFromDBAUTH", reqR);
                    responseTask.Wait();

                    var resultLoad = responseTask.Result;
                    if (resultLoad.IsSuccessStatusCode)
                    {
                        var postTask = resultLoad.Content.ReadAsAsync<LoadDataFromDBResponse>();
                        postTask.Wait();

                        //if (readTask.Result.responseMessage.ToLower() == "success")
                        obj = postTask.Result.LoadData;

                        if (obj.Count > 0)
                        {
                            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(LoadDataFromDBReturn));

                            foreach (PropertyDescriptor p in props)
                                oDt.Columns.Add(p.Name, p.PropertyType);
                            foreach (var c in obj)
                                oDt.Rows.Add(c.bankID, c.bankName, c.BranchID, c.BranchName, c.CycleID, c.Cycle_desc, c.ClgItems, c.ClgAmount, c.Rtn_Items, c.Rtn_Amount,
                                    c.CityCode, c.CityName, c.Pay, c.PayAmount, c.NonPayCount, c.NonPayAmount, c.Reject, c.RejectAmount, c.NoAction, c.NoActionAmount, c.Authorize,
                                    c.AuthorizeAmount, c.NonAuth, c.NonAuthAmount, c.Defer, c.DeferAmount);
                        }
                    }
                }
                if (oDt.Rows.Count > 0)
                {
                    string sBrid = "";
                    string sSQL = "";
                    using (var client = new HttpClient())
                    {

                        RSLoadDataFromDBRequest reqR = new RSLoadDataFromDBRequest
                        {
                            bkCode = bkcode,
                            sCycleCode = cycleno,
                            CityCode = citycode,
                            sUserId = userid
                        };
                        //  HTTP GET
                        client.BaseAddress = new Uri(apiUrl_summ);
                        var responseTask = client.PostAsJsonAsync("RSLoadDataFromDB", reqR);
                        responseTask.Wait();

                        var resultLoad = responseTask.Result;
                        if (resultLoad.IsSuccessStatusCode)
                        {

                            var postTask = resultLoad.Content.ReadAsAsync<RSLoadDataFromDBResponse>();
                            postTask.Wait();

                            //   if (readTask.Result.responseMessage.ToLower() == "success")
                            obj1 = postTask.Result.RSLoadData;

                            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(RSLoadDFrDBResponse));

                            foreach (PropertyDescriptor p in props)
                                dtBranch.Columns.Add(p.Name, p.PropertyType);
                            foreach (var c in obj1)
                                dtBranch.Rows.Add(c.InstID, c.InstName, c.branchid, c.branch_name);
                        }
                    }
                    
                    int icnt = dtBranch.Rows.Count;
                    foreach (DataRow rt in dtBranch.Rows)
                    {
                        if (icnt == 0)
                        {
                        }
                        else if (icnt == 1)
                        {
                            sBrid += rt["branchid"];
                        }
                        else
                        {
                            sBrid += rt["branchid"];
                            sBrid += ",";
                        }
                        icnt--;
                    }


                    //ViewState["RsReturnsDB"] = oDt;

                    _rsTable = oDt;

                    DataTable dt1 = new DataTable();
                    DataTable dt2 = new DataTable();
                    DataTable dt3 = new DataTable();

                    if (bankcode == "999")
                    {
                        _rsTable.DefaultView.RowFilter = String.Format("Authorize > 0 ");
                        dt1 = _rsTable.DefaultView.ToTable();

                        _rsTable.DefaultView.RowFilter = String.Format("NonAuth > 0");
                        dt2 = _rsTable.DefaultView.ToTable();

                        _rsTable.DefaultView.RowFilter = String.Format("Reject  > 0 ");
                        dt3 = _rsTable.DefaultView.ToTable();
                    }
                    else
                    {
                        if (cycleno.Trim() == "20")
                        {
                            _rsTable.DefaultView.RowFilter = String.Format("Authorize > 0 ");
                            dt1 = _rsTable.DefaultView.ToTable();

                            _rsTable.DefaultView.RowFilter = String.Format("NonAuth > 0");
                            dt2 = _rsTable.DefaultView.ToTable();

                            _rsTable.DefaultView.RowFilter = String.Format("Reject  > 0 ");
                            dt3 = _rsTable.DefaultView.ToTable();
                        }
                        else
                        {
                            if (sBrid == "")
                            {

                            }
                            else
                            {
                                _rsTable.DefaultView.RowFilter = String.Format("Authorize > 0 And BranchID in ({0})", sBrid);
                                dt1 = _rsTable.DefaultView.ToTable();

                                _rsTable.DefaultView.RowFilter = String.Format("NonAuth > 0 And BranchID in ({0})", sBrid);
                                dt2 = _rsTable.DefaultView.ToTable();

                                _rsTable.DefaultView.RowFilter = String.Format("Reject  > 0 And BranchID in ({0})", sBrid);
                                dt3 = _rsTable.DefaultView.ToTable();
                            }
                        }
                    }
                    
                    if (dt2.Rows.Count > 0)
                    {
                        obj = CommonMethod.ConvertToList<LoadDataFromDBReturn>(dt2);
                    }
                    else
                    {
                        obj = null;
                    }
                    objAuth = CommonMethod.ConvertToList<LoadDataFromDBReturn>(_rsTable);
                    var value = new { obj, objAuth };
                    return new JsonResult(value);
                }

                return new JsonResult("");
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return new JsonResult("Failed");
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
        public JsonResult OnPostDetailsClick(string DDL_Bank, string DDL_branch, string DDL_City, string DDL_Cycle,string st_)
        {
            try
            {
                if (DDL_branch != null)
                {
                    HttpContext.Session.SetString(InwardauthorizationModel.viewbranch_, DDL_branch);
                }
                string pdate = dtFrom_;
                oDt1 = new DataTable();
                DataTable fino = new DataTable();

                using (var client = new HttpClient())
                {
                    InwardAuthorizationRequest reqR = new InwardAuthorizationRequest
                    {
                        pdate = pdate,
                        bkcode = DDL_Bank,
                        brcode = DDL_branch,
                        cycleid = DDL_Cycle,
                        citycode = DDL_City,
                    };
                    //HTTP GET
                    client.BaseAddress = new Uri(apiUrl_auth);
                    var responseTask = client.PostAsJsonAsync("InwardAuthorization", reqR);
                    responseTask.Wait();

                    var resultLoad = responseTask.Result;
                    if (resultLoad.IsSuccessStatusCode)
                    {
                        var postTask = resultLoad.Content.ReadAsAsync<InwardAuthorizationResponse>();
                        postTask.Wait();

                        // if (readTask.Result.responseMessage.ToLower() == "success")
                        objA = postTask.Result.InwAuthorization;

                        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(InwardAuthorizationReturn));


                        foreach (PropertyDescriptor p in props)
                            oDt1.Columns.Add(p.Name, p.PropertyType);
                        foreach (var c in objA)
                        {
                            oDt1.Rows.Add(c.hostID, c.SBkId, c.SBkNm, c.SBrId, c.SBrNm, c.RBkId, c.RBkNm, c.RBrId, c.RBrNm, c.TrCode,
                             c.CycleId, c.CycleDesc, c.ChqNo, c.AccNo, c.TrId, c.IWOWID, c.ClgType, c.CityId, c.CityName, c.Amount, c.IQA,
                             c.UV, c.Bar, c.MICR, c.Std, c.Dup, c.WMark, c.AvgChqSize, c.TChq, c.TChq, c.reasonid, c.Reason, c.TruncTime, c.TruncBy,
                             c.Settled, c.SetTime, c.SetBy, c.AuthBy, c.AuthDateTime, c.IsAuth, c.Undersize_Image, c.Folded_or_Torn_Document_Corners,
                             c.Folded_or_Torn_Document_Edges, c.Framing_Error, c.Document_Skew, c.Oversize_Image, c.Piggy_Back, c.Image_Too_Light,
                             c.Image_Too_Dark, c.Horizontal_Streaks, c.Below_Minimum_Compressed_Image_Size, c.Above_Maximum_Compressed_Image_Size,
                             c.Spot_Noise, c.Front_Rear_Dimension_Mismatch, c.Carbon_Strip, c.Out_of_Focus, c.ReasonID2, c.Reason2, c.ReasonID3,
                             c.Reason3, c.Comment1, c.Comment2, c.Comment3, c.idDeffer, c.Comment1By, c.Comment2By, c.Comment3By, c.Comment1date, c.Comment2date, c.Comment3date);
                        }
                    }
                    if (oDt1.Rows.Count > 0)
                    {
                        string sTruncBy = oDt1.Rows[0]["TruncBy"].ToString();
                        string sUserID = userid;
                        if (sTruncBy == sUserID)
                        {
                            LogWriter.WriteToLog("Failed to authorize because maker and checker cannot be the same person - " + userid + " - " + bankcode);
                            string sMsg = "Maker & checker can not be same person";
							return new JsonResult("mFailed");
						}

                        if (st_ == "0")
                        {
                            oDt1.DefaultView.RowFilter = String.Format("IsAuth = 0 and Settled = 0");
                            fino = oDt1.DefaultView.ToTable();
                        }
                        else if (st_ == "1")
                        {
                            oDt1.DefaultView.RowFilter = String.Format("IsAuth = 1 and Settled = 0");
                            fino = oDt1.DefaultView.ToTable();
                        }
                        else if (st_ == "2")
                        {
                            oDt1.DefaultView.RowFilter = String.Format("IsAuth = 0 and Settled = 1");
                            fino = oDt1.DefaultView.ToTable();
                        }

                        if (oDt1.Rows.Count > 0)
                        {
                            objA = CommonMethod.ConvertToList<InwardAuthorizationReturn>(fino);
                        }
                    }
                    else
                    {
                        //Session["IABrTable"] = null;
                        //BindMainGrid();
                    }
                }
                return new JsonResult(objA);
            }
            catch(Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return new JsonResult("Failed");
            }
        }

        //public void OnGetAuthorizationCheck()
        //{

        //    string sDateTime = "2023-08-02";
        //    string sBankCode = "62";
        //    string sBranchCode = "585";
        //    string sCycleCode = "5";
        //    string sCityCode = "50";
        //    DataTable dtSort = new DataTable();
        //    oDt = new DataTable();
        //    using (var client = new HttpClient())
        //    {

        //        CheckSendForReviewRequest reqR = new CheckSendForReviewRequest
        //        {
        //            pdate = sDateTime,
        //            bkcode = sBankCode,
        //            brcode = sBranchCode,
        //            cyclecode = sCycleCode,
        //            citycode = sCityCode,
        //        };
        //        //HTTP GET
        //        client.BaseAddress = new Uri(apiUrl_auth);
        //        var responseTask = client.PostAsJsonAsync("CheckSendForReview", reqR);
        //        responseTask.Wait();

        //        var resultLoad = responseTask.Result;
        //        if (resultLoad.IsSuccessStatusCode)
        //        {

        //            var postTask = resultLoad.Content.ReadAsAsync<CheckSendForReviewResponse>();
        //            postTask.Wait();

        //            // if (readTask.Result.responseMessage.ToLower() == "success")
        //            obj4 = postTask.Result.SendForReview;

        //            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(CheckSendForReviewReturn));

        //            foreach (PropertyDescriptor p in props)
        //                oDt.Columns.Add(p.Name, p.PropertyType);
        //            foreach (var c in obj4)
        //                oDt.Rows.Add(c.hosID, c.ProID, c.capturedate, c.runno, c.batchno, c.siteid, c.sorterno, c.dinno, c.senderbank, c.senderbranch,
        //                    c.receiverbank, c.receiverbranch, c.chqno, c.accno, c.amt, c.trancode, c.onus, c.idxno, c.cycleno, c.hosttime, c.ffilepointer,
        //                    c.ffilesize, c.bfilepointer, c.bfilesize, c.HostCoverID, c.NonPrime, c.UVfilepointer, c.UVfilesize, c.FakeTag, c.Undersize_Image,
        //                    c.Folded_or_Torn_Document_Corners, c.Folded_or_Torn_Document_Edges, c.Framing_Error, c.Document_Skew, c.Oversize_image
        //                    , c.Piggy_Back, c.Image_Too_Light, c.Image_Too_Dark, c.Horizontal_Streaks, c.Below_Minimum_Compressed_Image_Size
        //                    , c.Above_Maximum_Compressed_Image_Size, c.Spot_Noise, c.Front_Rear_Dimension_Mismatch, c.Carbon_Strip, c.Out_of_Focus, c.IQATag
        //                    , c.barcodeMatch, c.UVStr, c.Duplicate, c.Average_Amount, c.Clg_Chq_Count, c.MICR_Present, c.STD_Non_STD, c.Water_Mark, c.Status,
        //                    c.ReasonID, c.Reason, c.TruncTime, c.TruncBy, c.Settled, c.SetTime, c.SetBy, c.AuthBy, c.AuthDateTime, c.isAuth, c.Rtn_Chq_Count
        //                    , c.ReasonID2, c.Reason2, c.ReasonID3, c.Reason3, c.Comment1, c.Comment2, c.Comment3, c.isDeffer, c.Comment1By, c.Comment2By,
        //                    c.Comment3By, c.Comment1Date, c.Comment2Date, c.Comment3Date, c.bDownload, c.isFraud, c.TemplateID, c.UVPercent);
        //        }
        //    }

        //    if (oDt.Rows.Count > 0)
        //    {
        //        string sMsg = "Return is already send for review";
        //        //lblMsg.Text = "Return is already send for review";
        //    }

        //    //Check  if Retrun Cycle is Authorized
        //    oDt = new DataTable();
        //    using (var client = new HttpClient())
        //    {

        //        CheckSendForReviewRequest reqR = new CheckSendForReviewRequest
        //        {
        //            pdate = sDateTime,
        //            bkcode = sBankCode,
        //            brcode = sBranchCode,
        //            cyclecode = sCycleCode,
        //            citycode = sCityCode,
        //        };
        //        //HTTP GET
        //        client.BaseAddress = new Uri(apiUrl_auth);
        //        var responseTask = client.PostAsJsonAsync("ReturnCheckIsAuthorized", reqR);
        //        responseTask.Wait();

        //        var resultLoad = responseTask.Result;
        //        if (resultLoad.IsSuccessStatusCode)
        //        {

        //            var postTask = resultLoad.Content.ReadAsAsync<ReturnCheckIsAuthorizedResponse>();
        //            postTask.Wait();

        //            // if (readTask.Result.responseMessage.ToLower() == "success")
        //            obj5 = postTask.Result.CheckIsAuthorized;

        //            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(CheckSendForReviewReturn));

        //            foreach (PropertyDescriptor p in props)
        //                oDt.Columns.Add(p.Name, p.PropertyType);
        //            foreach (var c in obj5)
        //                oDt.Rows.Add(c.hosID, c.ProID, c.capturedate, c.runno, c.batchno, c.siteid, c.sorterno, c.dinno, c.senderbank, c.senderbranch,
        //                    c.receiverbank, c.receiverbranch, c.chqno, c.accno, c.amt, c.trancode, c.onus, c.idxno, c.cycleno, c.hosttime, c.ffilepointer,
        //                    c.ffilesize, c.bfilepointer, c.bfilesize, c.HostCoverID, c.NonPrime, c.UVfilepointer, c.UVfilesize, c.FakeTag, c.Undersize_Image,
        //                    c.Folded_or_Torn_Document_Corners, c.Folded_or_Torn_Document_Edges, c.Framing_Error, c.Document_Skew, c.Oversize_image
        //                    , c.Piggy_Back, c.Image_Too_Light, c.Image_Too_Dark, c.Horizontal_Streaks, c.Below_Minimum_Compressed_Image_Size
        //                    , c.Above_Maximum_Compressed_Image_Size, c.Spot_Noise, c.Front_Rear_Dimension_Mismatch, c.Carbon_Strip, c.Out_of_Focus, c.IQATag
        //                    , c.barcodeMatch, c.UVStr, c.Duplicate, c.Average_Amount, c.Clg_Chq_Count, c.MICR_Present, c.STD_Non_STD, c.Water_Mark, c.Status,
        //                    c.ReasonID, c.Reason, c.TruncTime, c.TruncBy, c.Settled, c.SetTime, c.SetBy, c.AuthBy, c.AuthDateTime, c.isAuth, c.Rtn_Chq_Count
        //                    , c.ReasonID2, c.Reason2, c.ReasonID3, c.Reason3, c.Comment1, c.Comment2, c.Comment3, c.isDeffer, c.Comment1By, c.Comment2By,
        //                    c.Comment3By, c.Comment1Date, c.Comment2Date, c.Comment3Date, c.bDownload, c.isFraud, c.TemplateID, c.UVPercent);
        //        }
        //    }
        //}
        public JsonResult OnPostClickTab(string Tab, string DDL_Bank, string DDL_City, string DDL_Cycle)
        {
            try
            {
                string pdate = dtFrom_;
                string bkcode = DDL_Bank;
                string cycleno = DDL_Cycle;
                string citycode = DDL_City;
                DataTable dtSort = new DataTable();
                DataTable dt = new DataTable();
                DataTable _imbrTable = new DataTable();
                DataTable _rsTable = new DataTable();
                oDt = new DataTable();
                DataTable dtBranch = new DataTable();
                using (var client = new HttpClient())
                {

                    LoadDataAuthDBRequest reqR = new LoadDataAuthDBRequest
                    {
                        pdate = pdate,
                        bkcode = bkcode,
                        cycleno = cycleno,
                        citycode = citycode,
                    };
                    //HTTP GET
                    client.BaseAddress = new Uri(apiUrl_auth);
                    var responseTask = client.PostAsJsonAsync("LoadDataFromDBAUTH", reqR);
                    responseTask.Wait();

                    var resultLoad = responseTask.Result;
                    if (resultLoad.IsSuccessStatusCode)
                    {

                        var postTask = resultLoad.Content.ReadAsAsync<LoadDataFromDBResponse>();
                        postTask.Wait();

                        // if (readTask.Result.responseMessage.ToLower() == "success")
                        obj = postTask.Result.LoadData;

                        if (obj.Count >0)
                        {
                            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(LoadDataFromDBReturn));

                            foreach (PropertyDescriptor p in props)
                                oDt.Columns.Add(p.Name, p.PropertyType);
                            foreach (var c in obj)
                                oDt.Rows.Add(c.bankID, c.bankName, c.BranchID, c.BranchName, c.CycleID, c.Cycle_desc, c.ClgItems, c.ClgAmount, c.Rtn_Items, c.Rtn_Amount,
                                    c.CityCode, c.CityName, c.Pay, c.PayAmount, c.NonPayCount, c.NonPayAmount, c.Reject, c.RejectAmount, c.NoAction, c.NoActionAmount, c.Authorize,
                                    c.AuthorizeAmount, c.NonAuth, c.NonAuthAmount, c.Defer, c.DeferAmount);
                        }
                    }
                }
                if (oDt.Rows.Count > 0)
                {
                    string sBrid = "";
                    string sSQL = "";
                    using (var client = new HttpClient())
                    {

                        RSLoadDataFromDBRequest reqR = new RSLoadDataFromDBRequest
                        {
                            bkCode = bkcode,
                            sCycleCode = cycleno,
                            CityCode = citycode,
                            sUserId = userid
                        };
                        //  HTTP GET
                        client.BaseAddress = new Uri(apiUrl_summ);
                        var responseTask = client.PostAsJsonAsync("RSLoadDataFromDB", reqR);
                        responseTask.Wait();

                        var resultLoad = responseTask.Result;
                        if (resultLoad.IsSuccessStatusCode)
                        {
                            var postTask = resultLoad.Content.ReadAsAsync<RSLoadDataFromDBResponse>();
                            postTask.Wait();

                            //   if (readTask.Result.responseMessage.ToLower() == "success")
                            obj1 = postTask.Result.RSLoadData;

                            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(RSLoadDFrDBResponse));

                            foreach (PropertyDescriptor p in props)
                                dtBranch.Columns.Add(p.Name, p.PropertyType);
                            foreach (var c in obj1)
                                dtBranch.Rows.Add(c.InstID, c.InstName, c.branchid, c.branch_name);
                        }
                    }

                    int icnt = dtBranch.Rows.Count;
                    foreach (DataRow rt in dtBranch.Rows)
                    {
                        if (icnt == 0)
                        {
                        }
                        else if (icnt == 1)
                        {
                            sBrid += rt["branchid"];
                        }
                        else
                        {
                            sBrid += rt["branchid"];
                            sBrid += ",";
                        }
                        icnt--;
                    }

                    _rsTable = oDt;

                    DataTable dt1 = new DataTable();
                    DataTable dt2 = new DataTable();
                    DataTable dt3 = new DataTable();

                    if (bankcode == "999")
                    {
                        _rsTable.DefaultView.RowFilter = String.Format("Authorize > 0 ");
                        dt1 = _rsTable.DefaultView.ToTable();

                        _rsTable.DefaultView.RowFilter = String.Format("NonAuth > 0");
                        dt2 = _rsTable.DefaultView.ToTable();

                        _rsTable.DefaultView.RowFilter = String.Format("Reject  > 0 ");
                        dt3 = _rsTable.DefaultView.ToTable();
                    }
                    else
                    {
                        if (cycleno.Trim() == "20")
                        {
                            _rsTable.DefaultView.RowFilter = String.Format("Authorize > 0 ");
                            dt1 = _rsTable.DefaultView.ToTable();

                            _rsTable.DefaultView.RowFilter = String.Format("NonAuth > 0");
                            dt2 = _rsTable.DefaultView.ToTable();

                            _rsTable.DefaultView.RowFilter = String.Format("Reject  > 0 ");
                            dt3 = _rsTable.DefaultView.ToTable();
                        }
                        else
                        {
                            if (sBrid == "")
                            {

                            }
                            else
                            {
                                _rsTable.DefaultView.RowFilter = String.Format("Authorize > 0 And BranchID in ({0})", sBrid);
                                dt1 = _rsTable.DefaultView.ToTable();

                                _rsTable.DefaultView.RowFilter = String.Format("NonAuth > 0 And BranchID in ({0})", sBrid);
                                dt2 = _rsTable.DefaultView.ToTable();

                                _rsTable.DefaultView.RowFilter = String.Format("Reject  > 0 And BranchID in ({0})", sBrid);
                                dt3 = _rsTable.DefaultView.ToTable();
                            }
                        }
                    }
                    if (Tab == "1")
                    {
                        obj = CommonMethod.ConvertToList<LoadDataFromDBReturn>(dt2);
                    }
                    else if (Tab == "2")
                    {
                        obj = CommonMethod.ConvertToList<LoadDataFromDBReturn>(dt1);
                    }
                    else if (Tab == "3")
                    {
                        obj = CommonMethod.ConvertToList<LoadDataFromDBReturn>(dt3);
                    }

                }
                return new JsonResult(obj);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return new JsonResult("Failed");
            }
        }
        public JsonResult OnPostUpdateComment(string comments, string DDL_Bank, string DDL_City, string DDL_Cycle, string hostid)
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
                        client.BaseAddress = new Uri(apiUrl_auth);

                        var postTask = client.GetAsync($"UpdateAuthComment?comment={comments}&userid={userid}&hostid={hostid}");
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
                            ReturnLogDB(hostid, "Authorizer Comment=" + comments);
                            LogWriter.WriteToLog("Successfully Updated comment By Authorizer - " + userid + " - " + bankcode);
                            return new JsonResult("Success");
                        }
                        else
                        {
                            LogWriter.WriteToLog("Failed to Update comment By Authorizer - " + userid + " - " + bankcode);
                            return new JsonResult("Failed");
                        }
                    }
                }
                return new JsonResult("Failed");
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Failed to Update comment By Authorizer - " + userid + " - " + bankcode);
                LogWriter.WriteToLog(ex);
                return new JsonResult("Failed");
            }
        }
        public JsonResult OnPostCheckImages(string hostId)
        {
            try
            {
                string sHostId = hostId;
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
                return new JsonResult("Success|" + imageDataUrl1 + "|" + imageDataUrl2 + "|" + imageDataUrl3);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return new JsonResult("Failed");
            }
        }
        public JsonResult OnPostAuthorizeUpdate(string hostID, string IsAuth,string selectedbank, string selectedCycle, string selectedCity)
        {
            try
            {
                bool res = false;
                if (IsCycleClose(selectedbank, selectedCycle, selectedCity))
                {
                    LogWriter.WriteToLog("Unable to authorize because cycle is closed - " + userid + " - " + bankcode);
                    string sMsg = "Cycle is closed, you can not save data";
                    res = false;
                    return new JsonResult("CycleFailed");
                }
                else
                {
                    if (IsAuth == "1")
                    {
                        res = updateAuthorizeDB(hostID, "0");

                        if (res == true)
                        {
                            LogWriter.WriteToLog("Successfully authorized By - " + userid + " - " + bankcode + " - " + branchcode + " - HostID: " + hostID + "- 0");
                        }
                        else
                        {
                            LogWriter.WriteToLog("Unable to authorize for details - " + userid + " - " + bankcode + " - " + branchcode + " - HostID: " + hostID + "- 0");
                        }
                    }
                    else
                    {
                        res = updateAuthorizeDB(hostID, "1");
                        if (res == true)
                        {
                            LogWriter.WriteToLog("Successfully authorized By - " + userid + " - " + bankcode + " - " + branchcode + " - HostID: " + hostID + "- 1");
                        }
                        else
                        {
                            LogWriter.WriteToLog("Unable to authorize for details - " + userid + " - " + bankcode + " - " + branchcode + " - HostID: " + hostID + "- 1");
                        }
                    }
                }

                return new JsonResult(res);
            }
            catch(Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return new JsonResult("errFailed");
            }
        }
        private bool updateAuthorizeDB(string hostID, string IsAuth)
        {
            try
            {
                string sUserId = userid;
                using (var client = new HttpClient())
                {
                    updateAuthorizeDBRequest reqR = new updateAuthorizeDBRequest
                    {
                        hostid = hostID,
                        isAuth = IsAuth,
                        AuthBy = sUserId,
                        SetBy = sUserId
                    };
                    //HTTP GET
                    client.BaseAddress = new Uri(apiUrl_auth);
                    var responseTask = client.PostAsJsonAsync<updateAuthorizeDBRequest>("updateAuthorizeDB", reqR);
                    responseTask.Wait();
                    var readTask = responseTask.Result.Content.ReadAsAsync<updateAuthorizeDBResponse>();
                    readTask.Wait();
                    if (readTask.Result.responseMessage.ToLower() == "success")
                    {
                        ReturnLogDB(hostID, "Single Return Authorized by User ID: "+sUserId);

						return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return false;
            }
        }
        public JsonResult OnPostRejectedUpdate(string hostID, string Settled,string selectedbank, string selectedCycle, string selectedCity)
        {
            try
            {
                bool res = false;
                if (IsCycleClose(selectedbank, selectedCycle, selectedCity))
                {
                    string sMsg = "Cycle is closed, you can not save data";
                    res = false;
                    return new JsonResult("CycleFailed");
                }
                else
                {
                    if (Settled == "1")
                    {
                        res = updateRejectDB(hostID, "0");
                        if (res == true)
                        {
                            LogWriter.WriteToLog("Successfully Rejected By - " + userid + " - " + bankcode + " - " + branchcode + " - HostID: " + hostID + "- 0");
                        }
                        else
                        {
                            LogWriter.WriteToLog("Unable to Rejected for details - " + userid + " - " + bankcode + " - " + branchcode + " - HostID: " + hostID + "- 0");
                        }
                    }
                    else
                    {
                        res = updateRejectDB(hostID, "1");
                        if (res == true)
                        {
                            LogWriter.WriteToLog("Successfully Rejected By - " + userid + " - " + bankcode + " - "+branchcode+ "- HostID: " + hostID + "- 1");
                        }
                        else
                        {
                            LogWriter.WriteToLog("Unable to Rejected for details - " + userid + " - " + bankcode + " - " + branchcode + " - HostID: " + hostID + "- 1");
                        }
                    }
                }
                return new JsonResult(res);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return new JsonResult("errFailed");
            }
        }
        private bool updateRejectDB(string hostID, string Settled)
        {
            try
            {
                string sUserId = userid;
                using (var client = new HttpClient())
                {

                    updateRejectDBRequest reqR = new updateRejectDBRequest
                    {
                        sHostID = hostID,
                        bIsSet = Settled,
                        sUserId = sUserId
                    };
                    //HTTP GET
                    client.BaseAddress = new Uri(apiUrl_auth);
                    var responseTask = client.PostAsJsonAsync<updateRejectDBRequest>("updateRejectDB", reqR);
                    responseTask.Wait();
                    var readTask = responseTask.Result.Content.ReadAsAsync<updateRejectDBResponse>();
                    readTask.Wait();
                    if (readTask.Result.responseMessage.ToLower() == "success")
                    {
						ReturnLogDB(hostID, "Single Return Rejected by User ID: " + sUserId);
						return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return false;
            }
        }
        public JsonResult OnPostAuthorizeAll(string selectedBank,string selectedCycle, string selectedCity)
        {
            try
            {
                bool res_ = false;
                if (IsCycleClose(selectedBank, selectedCycle, selectedCity))
                {
                    string sMsg = "Unable to authorize because Cycle is closed - ";
                    LogWriter.WriteToLog(sMsg + userid + " - " + bankcode );
                    return new JsonResult("CycleFailed");
                }
                res_ = updateAuthorizeAll(selectedCycle, selectedCity, selectedBank);
                if (res_ == true)
                {
                    LogWriter.WriteToLog("Successfully Authorized-All By - " + userid + " - " + bankcode + " - Branch Code: " +  branchcode);
                }
                else
                {
                    LogWriter.WriteToLog("Unable to Authorize-All for details - " + userid + " - " + bankcode + " - Branch Code: " + branchcode);
                }
                return new JsonResult(res_);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return new JsonResult("errFailed");
            }
        }

        public bool updateAuthorizeAll(string cycle_,string city_, string bank_)
        {
            try
            {
                string sUserId = userid;
                string branch_ = HttpContext.Session.GetString(InwardauthorizationModel.viewbranch_);
                if(branch_ == null)
                {
                    branch_ = "0";
                }
                using (var client = new HttpClient())
                {

                    updateeUnAuthorizeDBRequest reqR = new updateeUnAuthorizeDBRequest
                    {
                        sUserId = sUserId,
                        SetBy = sUserId,
                        DDL_Bank = bank_,
                        ssbrid = branch_,
                        DDL_Cycle = cycle_
                    };

                    client.BaseAddress = new Uri(apiUrl_auth);
                    var responseTask = client.PostAsJsonAsync<updateeUnAuthorizeDBRequest>("updateAuthorizeAll", reqR);
                    responseTask.Wait();
                    var readTask = responseTask.Result.Content.ReadAsAsync<updateUnAuthorizeAllResponse>();
                    readTask.Wait();
                    if (readTask.Result.responseMessage.ToLower() == "success")
                    {
						ReturnLogDB("0", "All Return Authorized by User ID: " + sUserId + " For bank :"+ bank_ +" For branch: " + branch_);
						return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return false;
            }
        }


        public JsonResult OnPostUnAuthorizeAll(string selectedBank,string selectedCycle, string selectedCity)
        {
            try
            {
                bool res_= false;
                if (IsCycleClose(selectedBank, selectedCycle, selectedCity))
                {
                    string sMsg = "Cycle is closed, you can not save data";
                    return new JsonResult("CycleFailed");
                }
                res_ = updateUnAuthorizeAll(selectedCycle, selectedCity, selectedBank);
                if (res_ == true)
                {
                    LogWriter.WriteToLog("Successfully UnAuthorized-All By - " + userid + " - " + bankcode + " - Branch Code: " + branchcode);
                }
                else
                {
                    LogWriter.WriteToLog("Unable to UnAuthorized-All for details - " + userid + " - " + bankcode + " - Branch Code: " + branchcode);
                }
                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return new JsonResult("errFailed");
            }
        }
        public bool updateUnAuthorizeAll(string cycle_, string city_, string bank_)
        {
            try
            {
                string sUserId = userid;
                string branch_ = HttpContext.Session.GetString(InwardauthorizationModel.viewbranch_);
                if (branch_ == null)
                {
                    branch_ = "0";
                }
                using (var client = new HttpClient())
                {

                    updateeUnAuthorizeDBRequest reqR = new updateeUnAuthorizeDBRequest
                    {
                        sUserId = sUserId,
                        SetBy = sUserId,
                        DDL_Bank = bank_,
                        ssbrid = branch_,
                        DDL_Cycle = cycle_
                    };

                    client.BaseAddress = new Uri(apiUrl_auth);
                    var responseTask = client.PostAsJsonAsync<updateeUnAuthorizeDBRequest>("updateUnAuthorizeAll", reqR);
                    responseTask.Wait();
                    var readTask = responseTask.Result.Content.ReadAsAsync<updateUnAuthorizeAllResponse>();
                    readTask.Wait();
                    if (readTask.Result.responseMessage.ToLower() == "success")
                    {
						ReturnLogDB("0", "All Return UnAuthorized by User ID: " + sUserId + " For bank :" + bank_ + " For branch: " + branch_);
						return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return false;
            }
        }

        public JsonResult OnPostRejectedAll(string selectedBank,string selectedCycle, string selectedCity)
        {
            try
            {
                bool res_ = false;
                if (IsCycleClose(selectedBank, selectedCycle, selectedCity))
                {
                    string sMsg = "Cycle is closed, you can not save data";
                    return new JsonResult("CycleFailed");
                }
                res_ = UpdateRejectedAll(selectedBank, selectedCycle, selectedCity);
                if (res_ == true)
                {
                    LogWriter.WriteToLog("Successfully Rejected-All By - " + userid + " - " + bankcode + " - Branch Code: " + branchcode);
                }
                else
                {
                    LogWriter.WriteToLog("Unable to Reject-All for details - " + userid + " - " + bankcode + " - Branch Code: " + branchcode);
                }

                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return new JsonResult("errFailed");
            }
        }
        public bool UpdateRejectedAll(string selectedBank,string selectedCycle, string selectedCity)
        {
            try
            {
                string sUserId = userid;
                string branch_ = HttpContext.Session.GetString(InwardauthorizationModel.viewbranch_);
                if (branch_ == null)
                {
                    branch_ = "0";
                }
                using (var client = new HttpClient())
                {
                    updateRejectAllRequest reqR = new updateRejectAllRequest
                    {
                        sUserId = sUserId,
                        DDL_Bank = selectedBank,
                        ssbrid = branch_,
                        DDL_Cycle = selectedCycle
                    };
                    //HTTP GET
                    client.BaseAddress = new Uri(apiUrl_auth);
                    var responseTask = client.PostAsJsonAsync<updateRejectAllRequest>("updateRejectAll", reqR);
                    responseTask.Wait();
                    var readTask = responseTask.Result.Content.ReadAsAsync<updateRejectAllResponse>();
                    readTask.Wait();
                    if (readTask.Result.responseMessage.ToLower() == "success")
                    {
						ReturnLogDB("0", "All Return Rejected by User ID: " + sUserId + " For bank :" + selectedBank + " For Branch: " + branch_);
						return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return false;
            }
        }

        private bool IsCycleClose(string Bank, string sCycle, string sCity)
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
                req.cycno = sCycle;
                req.bkcode = sBankCode;
                req.cityid = sCity;
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
                        if (isCycleCloResponses == null)
                        {
                            return true;
                        }
                        isCycleCloResponses = readTask.Result.IsCycleClose;

                        if (isCycleCloResponses.Count == 0)
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
		//public JsonResult OnGetSingleAuth(string hostID)
		//{
		//    try
		//    {
		//        string sHostId = hostID.ToString();
		//        DataTable dt2 = new DataTable();
		//        string condition = "hostID = " + "'" + sHostId + "'";
		//        oDt1.DefaultView.RowFilter = condition;
		//        dt2 = oDt1.DefaultView.ToTable();
		//        return new JsonResult("Success");
		//    }
		//    catch(Exception ex) {
		//        LogWriter.WriteToLog(ex);
		//        return new JsonResult("Failed");
		//    }
		//}
		public bool ReturnLogDB(string hostID,string sReason)
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
	}
}

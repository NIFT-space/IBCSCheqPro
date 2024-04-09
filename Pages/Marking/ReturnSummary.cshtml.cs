using IBCS_Core_Web_Portal.Helper;
using IBCS_Web_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.Data;
using static IBCS_Core_Web_Portal.Pages.Cycleinfo.BankEmailModel;

namespace IBCS_Core_Web_Portal.Pages.Marking
{
    public class ReturnSummaryModel : PageModel
    {
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

				if (cpa.Validatepageallowed(userid, "ReturnSummary") == false)
				{
					Response.Redirect("/NotAllowed",true);
				}
				var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                apiUrl_Bktrsc = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "BkTrSc/";
                apiUrl_mark = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "Inwardmarking/";
                apiUrl_auth = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "Inwardauthorization/";
                apiUrl_summ = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "ReturnSummary/";
                apiUrl_brbd = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "Branchboard/";
                //img_apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.img") + "GetImageFromBin/";
                img_apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "GetImageFromBin/";
                apiUrl_return = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "CPUMemo/";
                BindBankTypeDD();
            }
            catch (Exception ex) {
				LogWriter.WriteToLog("Exception on Get - " + ex);
				Response.Redirect("/NotAllowed",true);
			}
        }
        #region variables
        public static string apiUrl_Bktrsc = "";
        public static string apiUrl_mark = "";
        public static string apiUrl_auth = "";
        public static string apiUrl_summ = "";
        public static string apiUrl_brbd = "";
        public static string img_apiURL = "";
        public static string apiUrl_return = "";
        public static string sBrid = "";
        public static string bkcode = "";
        public static string citycode = "";
        public static string cycleno = "";
        public static object data;
        public static string Auth_ = "";
        public static List<TransactionisAuthSummRetrun> obj = new List<TransactionisAuthSummRetrun>();
        string strAmt = "";
        string strCnt = "";
        int oCnt = 0;
        Decimal oAmt = 0;

        int pCnt = 0;
        Decimal pAmt = 0;
        int nCnt = 0;
        Decimal nAmt = 0;
        int aCnt = 0;
        Decimal aAmt = 0;
        int rCnt = 0;
        Decimal rAmt = 0;
        int dCnt = 0;
        Decimal dAmt = 0;
        //Specific User City
        #endregion

        public List<InstBank> BindBankDD()
        {
            try
            {
                // Bank
                List<InstBank> instBanks = new List<InstBank>();
                string sUserId = userid;
                //String sInstId = general.GetInstitute(sUserId);
                String sInstId = bankcode;
                DataTable dtInst = new DataTable();
                InstRequest req = new InstRequest()
                {
                    UserId = sUserId,
                    InstId = sInstId
                };

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl_Bktrsc);

                    var postTask = client.PostAsJsonAsync<InstRequest>("GetInstBank", req);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<InstBankResponse>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                        {
                            foreach (var item in readTask.Result.bkAdvice)
                            {
                                instBanks.Add(item);
                            }
                            return instBanks;
                        }
                        else
                        {
                            return instBanks;
                        }
                    }
                    else
                    {
                        return instBanks;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<City> BindCityDD()
        {
            try
            {
                List<City> city = new List<City>();
                string sUserId = userid;
                DataTable dtCity = new DataTable();

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl_Bktrsc);
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
                        {
                            foreach (var item in readTask.Result.cities)
                            {
                                city.Add(item);
                            }
                            return city;
                        }
                        else
                        {
                            return city;
                        }
                    }
                    else
                    {
                        return city;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<Cycle> BindCycleDD()
        {
            try
            {
                DataTable dtCycle = new DataTable();

                List<Cycle> cycle = new List<Cycle>();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl_brbd);

                    var postTask = client.GetAsync("GetCycleList");
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<CycleResponse>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                        {
                            foreach (var item in readTask.Result.cycle)
                            {
                                cycle.Add(item);
                            }
                            return cycle;
                        }
                        else
                        {
                            return cycle;
                        }
                    }
                    else
                    {
                        return cycle;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private void BindBankTypeDD()
        {
            try
            {

                DataTable dtType = new DataTable();
                dtType.Columns.Add("ClgID");
                dtType.Columns.Add("ClgType");
                dtType.Rows.Add("0", "Inward");
                dtType.Rows.Add("1", "Outward");
                ViewData["DtType"] = dtType;
            }
            catch (Exception ex)
            {
                // LogWriter.WriteToLog(ex);
            }
        }
        public JsonResult OnPostLoadDataFromDb([FromBody]LoadDataDBRequest req)
        {
            List<LoadDataFromDBReturn> PayTable = new List<LoadDataFromDBReturn>();
            List<LoadDataFromDBReturn> NoActionTable = new List<LoadDataFromDBReturn>();
            List<LoadDataFromDBReturn> AuthorizeTable = new List<LoadDataFromDBReturn>();
            List<LoadDataFromDBReturn> RejectTable = new List<LoadDataFromDBReturn>();
            List<LoadDataFromDBReturn> DeferTable = new List<LoadDataFromDBReturn>();
            List<LoadDataFromDBReturn> loadDataFromDBReturns = new List<LoadDataFromDBReturn>();
            try
            {
                req.pdate = dtFrom_;
                bkcode = req.bkcode;
                citycode = req.citycode;
                cycleno = req.cycleno;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl_auth);
                    var postTask = client.PostAsJsonAsync<LoadDataDBRequest>("LoadDataFromDB", req);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<LoadDataFromDBResponse>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                        {
                            foreach (var data in readTask.Result.LoadData)
                            {
                                loadDataFromDBReturns.Add(data);
                            }
                            RSLoadDataFromDBRequest rdfb = new RSLoadDataFromDBRequest();
                            rdfb.sUserId = userid;
                            rdfb.bkCode = req.bkcode;
                            rdfb.CityCode = req.citycode;
                            rdfb.sCycleCode = req.cycleno;
                            var CountBranched = CountBranches(rdfb);
                            if (loadDataFromDBReturns != null)
                            {
                                if (bankcode == "999")
                                {
                                    PayTable = loadDataFromDBReturns.Where(x => x.Pay >= 0).ToList();
                                    NoActionTable = loadDataFromDBReturns.Where(x => x.NonAuth > 0).ToList();
                                    AuthorizeTable = loadDataFromDBReturns.Where(x => x.Authorize > 0).ToList();
                                    RejectTable = loadDataFromDBReturns.Where(x => x.Reject > 0).ToList();
                                    DeferTable = loadDataFromDBReturns.Where(x => x.Defer > 0).ToList();
                                }
                                else
                                {
                                    if (req.cycleno == "20")
                                    {
                                        PayTable = loadDataFromDBReturns.Where(x => x.Pay >= 0).ToList();
                                        NoActionTable = loadDataFromDBReturns.Where(x => x.NonAuth > 0).ToList();
                                        AuthorizeTable = loadDataFromDBReturns.Where(x => x.Authorize > 0).ToList();
                                        RejectTable = loadDataFromDBReturns.Where(x => x.Reject > 0).ToList();
                                        DeferTable = loadDataFromDBReturns.Where(x => x.Defer > 0).ToList();
                                    }
                                    else
                                    {
                                        PayTable = loadDataFromDBReturns.Where(x => x.Pay >= 0 && sBrid.Contains(x.BranchID.ToString())).ToList();
                                        NoActionTable = loadDataFromDBReturns.Where(x => x.NonAuth > 0 && sBrid.Contains(x.BranchID.ToString())).ToList();
                                        AuthorizeTable = loadDataFromDBReturns.Where(x => x.Authorize > 0 && sBrid.Contains(x.BranchID.ToString())).ToList();
                                        RejectTable = loadDataFromDBReturns.Where(x => x.Reject > 0 && sBrid.Contains(x.BranchID.ToString())).ToList();
                                        DeferTable = loadDataFromDBReturns.Where(x => x.Defer > 0 && sBrid.Contains(x.BranchID.ToString())).ToList();
                                    }
                                }
                            }
                            var MainTable = Count(PayTable, NoActionTable, AuthorizeTable, RejectTable, DeferTable);
                            return new JsonResult(new
                            {
                                MainTableCount = MainTable,
                                PayTable = PayTable,
                                NoActionTable = NoActionTable,
                                AuthorizeTable = AuthorizeTable,
                                RejectTable = RejectTable,
                                DeferTable = DeferTable
                            });
                        }
                        else
                        {
                            return new JsonResult(new { tabledata = loadDataFromDBReturns });
                        }
                    }
                    else
                    {
                        return new JsonResult(new { tabledata = loadDataFromDBReturns });
                    }
                }

            }
            catch (Exception ex)
            {
				LogWriter.WriteToLog("Exception on Post - " + ex);
				return new JsonResult(new { tabledata = loadDataFromDBReturns });

            }
        }

        private string CountBranches([FromBody] RSLoadDataFromDBRequest req)
        {
            List<RSLoadDFrDBResponse> datum = new List<RSLoadDFrDBResponse>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl_summ);
                var postTask = client.PostAsJsonAsync<RSLoadDataFromDBRequest>("RSLoadDataFromDB", req);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<RSLoadDataFromDBResponse>();
                    readTask.Wait();

                    if (readTask.Result.responseMessage.ToLower() == "success")
                    {
                        foreach (var data in readTask.Result.RSLoadData)
                        {
                            datum.Add(data);
                        }

                        if (datum.Count > 0)
                        {
                            int icnt = datum.Count;
                            foreach (var rt in datum)
                            {
                                if (icnt == 0)
                                {
                                }
                                else if (icnt == 1)
                                {
                                    sBrid += rt.branchid;
                                }
                                else
                                {
                                    sBrid += rt.branchid;
                                    sBrid += ",";
                                }
                                icnt--;
                            }
                            return sBrid;
                        }
                        else
                        {
                            return sBrid;
                        }
                    }
                    else
                    {
                        return sBrid;
                    }
                }
                else
                {
                    return sBrid;
                }
            }
        }

        private MainTale Count(List<LoadDataFromDBReturn> PayTabel, List<LoadDataFromDBReturn> NoActionTable,
        List<LoadDataFromDBReturn> AuthorizeTable, List<LoadDataFromDBReturn> RejectTable, List<LoadDataFromDBReturn> DeferTable)
        {
            MainTale maintable = new MainTale();
            // Pay
            for (int i = 0; i < PayTabel.Count; i++)
            {
                strCnt = PayTabel[i].Pay.ToString();
                bool isParsable = Int32.TryParse(strCnt, out oCnt);
                if (isParsable)
                {
                    pCnt = pCnt + oCnt;
                    oCnt = 0;
                    isParsable = false;
                }
                strAmt = PayTabel[i].PayAmount.ToString();
                isParsable = Decimal.TryParse(strAmt, out oAmt);
                if (isParsable)
                {
                    pAmt = pAmt + oAmt;
                    oAmt = 0;
                    isParsable = false;

                }
            }
            maintable.pCnt = pCnt;
            maintable.pAmt = pAmt;
            //No-Action
            for (int i = 0; i < NoActionTable.Count; i++)
            {
                strCnt = NoActionTable[i].NonAuth.ToString();
                bool isParsable = Int32.TryParse(strCnt, out oCnt);
                if (isParsable)
                {
                    nCnt = nCnt + oCnt;
                    oCnt = 0;
                    isParsable = false;
                }
                strAmt = NoActionTable[i].NonAuthAmount.ToString();
                isParsable = Decimal.TryParse(strAmt, out oAmt);
                if (isParsable)
                {
                    nAmt = nAmt + oAmt;
                    oAmt = 0;
                    isParsable = false;

                }
            }
            maintable.nAmt = nAmt;
            maintable.nCnt = nCnt;
            //Autorize
            for (int i = 0; i < AuthorizeTable.Count; i++)
            {
                strCnt = AuthorizeTable[i].Authorize.ToString();
                bool isParsable = Int32.TryParse(strCnt, out oCnt);
                if (isParsable)
                {
                    aCnt = aCnt + oCnt;
                    oCnt = 0;
                    isParsable = false;
                }
                strAmt = AuthorizeTable[i].AuthorizeAmount.ToString();
                isParsable = Decimal.TryParse(strAmt, out oAmt);
                if (isParsable)
                {
                    aAmt = aAmt + oAmt;
                    oAmt = 0;
                    isParsable = false;

                }
            }
            maintable.aCnt = aCnt;
            maintable.aAmt = aAmt;
            // Rejact
            for (int i = 0; i < RejectTable.Count; i++)
            {
                strCnt = RejectTable[i].Reject.ToString();
                bool isParsable = Int32.TryParse(strCnt, out oCnt);
                if (isParsable)
                {
                    rCnt = rCnt + oCnt;
                    oCnt = 0;
                    isParsable = false;
                }
                strAmt = RejectTable[i].RejectAmount.ToString();
                isParsable = Decimal.TryParse(strAmt, out oAmt);
                if (isParsable)
                {
                    rAmt = rAmt + oAmt;
                    oAmt = 0;
                    isParsable = false;

                }
            }
            maintable.rCnt = rCnt;
            maintable.rAmt = rAmt;
            //Defer
            for (int i = 0; i < DeferTable.Count; i++)
            {
                strCnt = DeferTable[i].Defer.ToString();
                bool isParsable = Int32.TryParse(strCnt, out oCnt);
                if (isParsable)
                {
                    dCnt = dCnt + oCnt;
                    oCnt = 0;
                    isParsable = false;
                }
                strAmt = DeferTable[i].DeferAmount.ToString();
                isParsable = Decimal.TryParse(strAmt, out oAmt);
                if (isParsable)
                {
                    dAmt = dAmt + oAmt;
                    oAmt = 0;
                    isParsable = false;

                }
            }
            maintable.dAmt = dAmt;
            maintable.dCnt = dCnt;
            return maintable;

        }

        public JsonResult OnPostDetails(string brcode, string tab, string bktype)
        {
            try
            {
                obj.Clear();
                if (data != null)
                {
                    data = null;
                }
                Transaction_WI_isAuthSumm tt = new Transaction_WI_isAuthSumm();
                TransactionisAuthSummRetrun transactionisAuthSummRetrun = new TransactionisAuthSummRetrun();
                TransactionisAuthSumRequest req = new TransactionisAuthSumRequest();
                DataTable _rsTable = new DataTable();
                Mapped reqrs = new Mapped();
                req.pdate = dtFrom_;
                req.bkcode = bkcode;
                req.brcode = brcode;
                req.citycode = citycode;
                req.cyclecode = cycleno;
                req.bkType = bktype;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl_summ);

                    var postTask = client.PostAsJsonAsync<TransactionisAuthSumRequest>("TransactionwiseInward_isAuthSum", req);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<Transaction_WI_isAuthSumm>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage == "Success")
                        {
                            obj = readTask.Result.TransactionisAuthSumm;
                            //foreach (var item in readTask.Result.TransactionisAuthSumm)
                            //{
                            //    obj.Add(item);
                            //}

                            data = new
                            {
                                bankname = readTask.Result.TransactionisAuthSumm.FirstOrDefault().RBkNm,
                                branchname = readTask.Result.TransactionisAuthSumm.FirstOrDefault().RBrNm,
                                city = readTask.Result.TransactionisAuthSumm.FirstOrDefault().CityName,
                                clearingtype = readTask.Result.TransactionisAuthSumm.FirstOrDefault().ClgType,
                                cycle = readTask.Result.TransactionisAuthSumm.FirstOrDefault().CycleDesc
                            };


                            if (obj.Count > 0)
                            {
                                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(TransactionisAuthSummRetrun));

                                foreach (PropertyDescriptor p in props)
                                    _rsTable.Columns.Add(p.Name, p.PropertyType);
                                foreach (var c in obj)
                                    _rsTable.Rows.Add(c.hostID, c.SBkId, c.SBkNm, c.SBrId, c.SBrNm, c.RBkId, c.RBkNm, c.RBrId, c.RBrNm, c.TrCode, c.CycleId,
                                        c.CycleDesc, c.ChqNo, c.AccNo, c.TrId, c.IWOWID, c.ClgType, c.CityId, c.CityName
                                        , c.Amount, c.IQA, c.UV, c.Bar, c.MICR, c.Std, c.Dup, c.WMark, c.AvgChqSize, c.TChq, c.TChqR,c.TemplateID,c.UVperc, c.ReasonID,
                                        c.Reason, c.TruncTime, c.TruncBy, c.Settled, c.SetTime, c.SetBy, c.AuthBy, c.AuthDateTime
                                , c.isAuth, c.Undersize_Image, c.Folded_or_Torn_Document_Corners, c.Folded_or_Torn_Document_Edges, c.Framing_Error,
                                 c.Document_Skew, c.Oversize_Image, c.Piggy_Back, c.Image_Too_Light, c.Image_Too_Dark, c.Horizontal_Streaks,
                                 c.Below_Minimum_Compressed_Image_Size, c.Above_Maximum_Compressed_Image_Size, c.Spot_Noise
                                , c.Front_Rear_Dimension_Mismatch, c.Carbon_Strip, c.Out_of_Focus,
                                c.ReasonID2, c.Reason2, c.ReasonID3, c.Reason3, c.Comment1, c.Comment2, c.Comment3, c.isDeffer, c.Comment1By, c.Comment2By,
                                c.Comment3By, c.Comment1Date, c.Comment2Date, c.Comment3Date);
                            }

                            if (_rsTable.Rows.Count > 0)
                            {
                                DataTable dt1 = new DataTable();
                                DataTable dt2 = new DataTable();
                                DataTable dt3 = new DataTable();
                                DataTable dt4 = new DataTable();
                                DataTable dt5 = new DataTable();

                                    _rsTable.DefaultView.RowFilter = String.Format("ReasonID = 0");
                                    dt1 = _rsTable.DefaultView.ToTable();
                                
                                    _rsTable.DefaultView.RowFilter = String.Format("ReasonID > 0 And Settled = 0 And IsAuth = 0");
                                    dt2 = _rsTable.DefaultView.ToTable();
                                
                                    _rsTable.DefaultView.RowFilter = String.Format("IsAuth = 1");
                                    dt3 = _rsTable.DefaultView.ToTable();
                                
                                    _rsTable.DefaultView.RowFilter = String.Format("Settled = 1");
                                    dt4 = _rsTable.DefaultView.ToTable();
                                
                                    _rsTable.DefaultView.RowFilter = String.Format("ReasonID = 900");
                                    dt5 = _rsTable.DefaultView.ToTable();
                                

                                //if (bankcode == "999")
                                //{

                                //    _rsTable.DefaultView.RowFilter = String.Format("Pay >= 0 ");
                                //    dt1 = _rsTable.DefaultView.ToTable();

                                //    _rsTable.DefaultView.RowFilter = String.Format("NoAction > 0");
                                //    dt2 = _rsTable.DefaultView.ToTable();

                                //    _rsTable.DefaultView.RowFilter = String.Format("Authorize > 0 ");
                                //    dt3 = _rsTable.DefaultView.ToTable();

                                //    _rsTable.DefaultView.RowFilter = String.Format("Reject  > 0");
                                //    dt4 = _rsTable.DefaultView.ToTable();

                                //    _rsTable.DefaultView.RowFilter = String.Format("Defer > 0 ");
                                //    dt5 = _rsTable.DefaultView.ToTable();
                                //}
                                //else
                                //{
                                //    if (cycleno.Trim() == "20")
                                //    {
                                //        _rsTable.DefaultView.RowFilter = String.Format("Pay >= 0 ");
                                //        dt1 = _rsTable.DefaultView.ToTable();

                                //        _rsTable.DefaultView.RowFilter = String.Format("NoAction > 0 ");
                                //        dt2 = _rsTable.DefaultView.ToTable();

                                //        _rsTable.DefaultView.RowFilter = String.Format("Authorize > 0 ");
                                //        dt3 = _rsTable.DefaultView.ToTable();

                                //        _rsTable.DefaultView.RowFilter = String.Format("Reject  > 0 ");
                                //        dt4 = _rsTable.DefaultView.ToTable();

                                //        _rsTable.DefaultView.RowFilter = String.Format("Defer > 0 ");
                                //        dt5 = _rsTable.DefaultView.ToTable();
                                //    }
                                //    else
                                //    {
                                //        if (sBrid == "")
                                //        {

                                //        }
                                //        else
                                //        {
                                //            _rsTable.DefaultView.RowFilter = String.Format("Pay >= 0 And BranchID in ({0})", sBrid);
                                //            dt1 = _rsTable.DefaultView.ToTable();

                                //            _rsTable.DefaultView.RowFilter = String.Format("NoAction > 0 And BranchID in ({0})", sBrid);
                                //            dt2 = _rsTable.DefaultView.ToTable();

                                //            _rsTable.DefaultView.RowFilter = String.Format("Authorize > 0 And BranchID in ({0})", sBrid);
                                //            dt3 = _rsTable.DefaultView.ToTable();

                                //            _rsTable.DefaultView.RowFilter = String.Format("Reject  > 0 And BranchID in ({0})", sBrid);
                                //            dt4 = _rsTable.DefaultView.ToTable();

                                //            _rsTable.DefaultView.RowFilter = String.Format("Defer > 0 And BranchID in ({0})", sBrid);
                                //            dt5 = _rsTable.DefaultView.ToTable();
                                //        }
                                //    }
                                //}
                                if (tab == "1")
                                {
                                    obj = CommonMethod.ConvertToList<TransactionisAuthSummRetrun>(dt1);
                                }
                                else if (tab == "2")
                                {
                                    obj = CommonMethod.ConvertToList<TransactionisAuthSummRetrun>(dt2);
                                }
                                else if (tab == "3")
                                {
                                    obj = CommonMethod.ConvertToList<TransactionisAuthSummRetrun>(dt3);
                                }
                                else if (tab == "4")
                                {
                                    obj = CommonMethod.ConvertToList<TransactionisAuthSummRetrun>(dt4);
                                }
                                else if (tab == "5")
                                {
                                    obj = CommonMethod.ConvertToList<TransactionisAuthSummRetrun>(dt5);
                                }
                            }


                            return new JsonResult(new { detail = obj, header = data });
                        }
                        return new JsonResult(new { detail = obj, header = data });

                    }
                    else
                    {

                        return new JsonResult(new { detail = obj, header = data });

                    }
                }
                //reqrs.hostID = "123";
                //reqrs.TrCode = 12345678;
                //reqrs.SBkNm = "064-Habib Metro Bank testing";
                //reqrs.SBrNm = "testng";
                //reqrs.Amount = 112;
                //reqrs.AccNo = 0000077901250;
                //reqrs.ChqNo = 00000111;
                //reqrs.Std = "2023 Amount in words and figure differs";
                //reqrs.Undersize_Image = "asdas";
                //reqrs.Folded_or_Torn_Document_Corners = "asdasd";
                //reqrs.Folded_or_Torn_Document_Edges = "asdasd";
                //reqrs.Framing_Error = "asdasd";
                //reqrs.Document_Skew = "asdasd";
                //reqrs.Oversize_Image = "asd";
                //reqrs.Piggy_Back = "asda";
                //reqrs.Image_Too_Light = "asd";
                //reqrs.Image_Too_Dark = "asdas";
                //reqrs.Horizontal_Streaks = "ok";
                //reqrs.Below_Minimum_Compressed_Image_Size = "asdas";
                //reqrs.Above_Maximum_Compressed_Image_Size = "asdasd";
                //reqrs.Front_Rear_Dimension_Mismatch = "asd";
                //reqrs.Carbon_Strip = "asdasd";
                //reqrs.Out_of_Focus = "Asd";
                //data = new
                //{
                //    bankname = "064-Habib Metro Bank testing",
                //    branchname = "testng",
                //    city = "Karachi",
                //    clearingtype = "Normal",
                //    cycle = "2"
                //};
                //object data2 = new
                //{
                //    hostID = 123,
                //    TrCode = 12345678,
                //    SBkNm = "064-Habib Metro Bank testing",
                //    SBrNm = "testng",
                //    RBrNm = "etstingrbr",
                //    RBkNm = "testirblsd",
                //    Amount = 112,
                //    AccNo = 0000077901250,
                //    ChqNo = 00000111,
                //    Std = "2023 Amount in words and figure differs",
                //    Undersize_Image = "asdas",
                //    Folded_or_Torn_Document_Corners = "asdasd",
                //    Folded_or_Torn_Document_Edges = "asdasd",
                //    Framing_Error = "asdasd",
                //    Document_Skew = "asdasd",
                //    Oversize_Image = "asd",
                //    Piggy_Back = "asda",
                //    Image_Too_Light = "asd",
                //    Image_Too_Dark = "asdas",
                //    Horizontal_Streaks = "ok",
                //    Below_Minimum_Compressed_Image_Size = "asdas",
                //    Above_Maximum_Compressed_Image_Size = "asdasd",
                //    Front_Rear_Dimension_Mismatch = "asd",
                //    Carbon_Strip = "asdasd",
                //    Out_of_Focus = "Asd"
                //};
                //obj.Add(data2);
                //obj.Add(data2);
                //return new Microsoft.AspNetCore.Mvc.JsonResult(new { detail = obj, header = data });

            }
            catch (Exception ex)
            {
				LogWriter.WriteToLog("Exception on Post - " + ex);
				return new JsonResult(new { detail = "", header = "" });
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
        public JsonResult OnPostCheckImages(string hostID)
        {
            try
            {
                string sHostId = hostID.ToString();
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
                    client1.BaseAddress = new Uri(apiUrl_brbd);


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
                return new JsonResult(new { Image1 = imageDataUrl1, Image2 = imageDataUrl2, Image3 = imageDataUrl3, header = data, data = obj });
            }
            catch (Exception ex)
            {
				LogWriter.WriteToLog("Exception on Post - " + ex);
				return new JsonResult("Failed");
            }
        }
    }
}

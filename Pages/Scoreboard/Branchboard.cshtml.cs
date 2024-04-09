using IBCS_Core_Web_Portal.Helper;
using IBCS_Core_Web_Portal.Models;
using IBCS_Web_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Metrics;
using System.Runtime.Intrinsics.Arm;

namespace IBCS_Core_Web_Portal.Pages.Marking
{
	public class BranchboardModel : PageModel
	{
        public static string apiURL = "";
        public static string img_apiURL = "";
        public static string userid = "";
        public static string userlogid = "";
        public static string bankcode = "";
        public static string dtFrom_ = "";
        public static string dtTo_ = "";
        public static string Auth_ = "";
        public const string clgtype = "_clg";
        public List<InstBank> instBanks { get; set; }
        public List<City> cities { get; set; }
        public List<Cycle> cycle { get; set; }
        public BranchResponse branches { get; set; }
        public List<Branch> Br_;
        public List<HostNIBC> obj { get; set; }
        public static string selected_city, selected_bank;
        public static string sel_ddlbank, sel_ddlcity, sel_ddlcycle, sel_ddlbranch;
        public static DataTable oDt;
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

				if (cpa.Validatepageallowed(userid, "Branchboard") == false)
				{
					Response.Redirect("/NotAllowed",true);
				}
				var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "Branchboard/";
                //img_apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.img") + "GetImageFromBin/";
                img_apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "GetImageFromBin/";
                //OnGetCheckImages(obj2);
                BindBankDD();
                BindCityDD();
                BindCycleDD();
                BindBankTypeDD();
            }
			catch (ThreadAbortException)
			{

			}
			catch (Exception ex) {
                LogWriter.WriteToLog("Exception on GetFiles - " + ex);
				Response.Redirect("/NotAllowed",true);
			}
        }
        //private JsonResult OnGetRefresh_branches(string bkid)
        //{
        //    return new JsonResult("Success|" + );
        //}
        public JsonResult OnGetSelectedcity(string bank, string city)
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
        public JsonResult OnGetSelectedbank(string bank, string city)
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
                    client.BaseAddress = new Uri(apiURL);

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
                string sUserId = userid;
                DataTable dtCity = new DataTable();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);
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
                    client.BaseAddress = new Uri(apiURL);

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
        private List<Branch> BindBranch(string bank_id, string city_id)
        {
            try
            {
                //Branch
               Br_ = new List<Branch>();
                DataTable dtBranch = new DataTable();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);
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

                        foreach(var item in branches.branches)
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
      
        
        public JsonResult OnPostFetchFiles([FromBody]branchboarddetails brd)
        {
            try
            {
                //branchboarddetails brd = new branchboarddetails();
                string sQry = string.Empty;
                string sDateTime = dtFrom_;
                string sBankCode = brd.bkcode;
                string sBranchCode = brd.brcode;
                string sCycleCode = brd.cycle;
                string sCityCode = brd.citycode;
                ViewData["clg_type"] = brd.bkType;
                DataTable dtSort = new DataTable();
                DataTable dt = new DataTable();
                oDt = new DataTable();

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);
                    //HTTP GET
                    var responseTask = client.GetAsync($"GetReportListing?sDateTime={sDateTime}&sBankCode={sBankCode}&sBranchCode={sBranchCode}" +
                                $"&sCycleCode={sCycleCode}&sCityCode={sCityCode}&sBkType={brd.bkType}");
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {

                        var readTask = result.Content.ReadAsAsync<List<HostNIBC>>();
                        readTask.Wait();

                        // if (readTask.Result.responseMessage.ToLower() == "success")
                        obj = readTask.Result;
                        if (obj != null)
                        {
                            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(HostNIBC));

                            foreach (PropertyDescriptor p in props)
                                oDt.Columns.Add(p.Name, p.PropertyType);
                            foreach (var c in obj)
                                oDt.Rows.Add(c.hostId, c.senderBankId, c.senderBankName, c.senderBranchId, c.senderBranchName, c.recieverBankId,
                                    c.recieverBankName, c.recieverBranchId, c.recieverBranchName, c.trCode, c.cycleId, c.cycleDescription, c.chequeNo,
                                    c.accountNo, c.trId, c.IWOWId, c.clearingType, c.cityId, c.cityName, c.amount, c.IQA, c.UV, c.bar, c.MICR, c.std,
                                    c.dup, c.WMark, c.avgChqSize, c.tChq, c.tChqR, c.Templateid, c.UVperc, c.truncTime, c.truncBy, c.settled, c.setTime, c.setBy, c.authBy, c.authDateTime,
                                    c.isAuth, c.underSizeImage, c.folderOrTornDocumentCorners, c.foldedOrTornDocumentEdges, c.framingError, c.documentSkew, c.overSizeImage,
                                    c.piggyBack, c.imageTooLight, c.imageTooDark, c.horizontalStreaks, c.belowMinimumCompressedImageSize, c.aboveMaximumCompressedImageSize,
                                    c.spotNoise, c.frontRearDimensionMismatch, c.carbonStrip, c.outOfFocus, c.reasonID2, c.reason3, c.comment1, c.reasonID3, c.reason2, c.comment2,
                                    c.comment3, c.isDeffer);
                        }
                    }
                }

                return new JsonResult(obj);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception on GetFiles - " + ex);
                return new JsonResult(obj);
            }
        }
        //public FileResult OnGetDownloadFile(string CirId, string Fname)
        //{
        //    try
        //    {
        //        String DtTm = "";
        //        DtTm = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
        //        DtTm = DtTm.Replace(" ", "").Replace("/", "").Replace(":", "");
        //        byte[] MyData = new byte[0];

        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(apiURL);

        //            CircularRequest req = new CircularRequest
        //            {
        //                CirId = Convert.ToInt32(CirId)
        //            };
        //            CircularReport circular = new CircularReport();
        //            var responseTask = client.PostAsJsonAsync<CircularRequest>("GetBankHelpFileCirID", req);
        //            responseTask.Wait();

        //            var result = responseTask.Result;
        //            if (result.IsSuccessStatusCode)
        //            {

        //                var readTask = result.Content.ReadAsAsync<CircularFileCirIDResponse>();
        //                readTask.Wait();

        //                if (readTask.Result.responseMessage.ToLower() == "success")
        //                    circular = readTask.Result.circularFile;
        //            }

        //            MyData = circular.Report; //(byte[]) myRow["Report"];
        //            long fileSize = (long)MyData.Length;
        //            int ArraySize = new int();
        //            ArraySize = MyData.GetUpperBound(0);

        //            //string sUserId = Convert.ToString(Session["myid"]).Trim();

        //            //HTTP POST
        //            CircularDownLogRequest req2 = new CircularDownLogRequest
        //            {
        //                userid = userid,//Session["myid"].ToString(),
        //                userlogid = Convert.ToInt32(userlogid),//Convert.ToInt32(Session["userlogid"]),
        //                circularid = Convert.ToInt32(CirId)
        //            };

        //            var postTask = client.PutAsJsonAsync<CircularDownLogRequest>("setBankHelpDownLog", req2);
        //            postTask.Wait();

        //            var result2 = postTask.Result;

        //            //LogWriter.WriteToLog(sSQL);

        //            //Send the File to Download.  
        //            return File(MyData, "application/octet-stream", Fname);
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        byte[] MyData = new byte[0];
        //        LogWriter.WriteToLog("Exception on OnGetDownloadFile - " + ex);
        //        return File(MyData, "application/octet-stream", Fname);
        //    }
        //}


        public JsonResult OnGetCheckImages(string hostId)
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
					client1.BaseAddress = new Uri(apiURL);


					var postTask = client1.PostAsJsonAsync<LogImageFileRequest>("LogImageFile", req);
					postTask.Wait();
					var result1 = postTask.Result;
				}
				Byte[] jpegBytes = null;
                var imageDataUrl1= "";
                var imageDataUrl2= "";
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

                return new JsonResult("Success|" + imageDataUrl1+"|" + imageDataUrl2 + "|" + imageDataUrl3);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception on OnGetCheckImages - " + ex);
                return new JsonResult("Failed");
            }
        }

        /////FILTER OUT////
        ///
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

                    if ((!string.IsNullOrEmpty(amt_min)) && (!string.IsNullOrEmpty(amt_max)))
                    {
                        DataTable dt2 = new DataTable();
                        dt2 = dt1.Clone();
                        dt2.Columns["Amount"].DataType = Type.GetType("System.Decimal");

                        foreach (DataRow dr in dt1.Rows)
                        {
                            dt2.ImportRow(dr);
                        }
                        dt2.AcceptChanges();


                        DataView dv = dt2.DefaultView;
                        dv.Sort = "Amount DESC";
                        dt = dv.ToTable();
                    }

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

                    obj = CommonMethod.ConvertToList<HostNIBC>(dt);

                    return new JsonResult(obj);
                }
                return new JsonResult("NullData");
            }
            catch (Exception ex) {
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

                    obj = CommonMethod.ConvertToList<HostNIBC>(dt);

                    return new JsonResult(obj);
                }
                return new JsonResult("NullData");
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
		//#region Variables
		//public static string apiURL = "";
		//public List<InstBank> instBanks { get; set; }
		//public List<City> cities { get; set; }
		//public List<Cycle> cycle { get; set; }
		//public List<Branch> branches { get; set; }
		//public List<HostNIBC> obj { get; set; }


		//public static string sel_ddlbank, sel_ddlcity, sel_ddlcycle, sel_ddlbranch;
		//#endregion

		//public void OnGet(string d1, string d2, string d3, string d4)
		//{
		//	string userid = HttpContext.Session.GetString(loginModel.SessionKeyName1);

		//	if (userid == null)
		//	{

		//		Response.Redirect("/NotAllowed",true);
		//	}
		//	var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
		//	apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "Branchboard/";
		//	BindBankDD();
		//	BindCityDD();
		//	BindCycleDD();
		//	BindBranch();
		//	//BindData();
		//	if (d1 != null || d2 != null || d3 != null || d4 != null)
		//	{
		//		obj = GetFiles(d1, d2, d3, d4);
		//	}
		//	//GetFiles();
		//}
		//private void BindBankDD()
		//{
		//	try
		//	{
		//		// Bank
		//		string sUserId = "26887";//Convert.ToString(Session["myid"]).Trim();
		//		string sInstId = "54";//Session["BankCode"].ToString();
		//		DataTable dtInst = new DataTable();
		//		InstRequest req = new InstRequest()
		//		{
		//			UserId = sUserId,
		//			InstId = sInstId
		//		};

		//		using (var client = new HttpClient())
		//		{
		//			client.BaseAddress = new Uri(apiURL);

		//			var postTask = client.PostAsJsonAsync("GetInstBank", req);
		//			postTask.Wait();

		//			var result = postTask.Result;
		//			if (result.IsSuccessStatusCode)
		//			{
		//				var readTask = result.Content.ReadAsAsync<InstBankResponse>();
		//				readTask.Wait();

		//				if (readTask.Result.responseMessage.ToLower() == "success")
		//					instBanks = readTask.Result.bkAdvice;
		//			}
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		//LogWriter.WriteToLog(ex);
		//	}
		//}
		//private void BindCityDD()
		//{
		//	try
		//	{
		//		string CityCount = "";
		//		string sUserId = "26887";//Convert.ToString(Session["myid"]).Trim();
		//		DataTable dtCity = new DataTable();
		//		using (var client = new HttpClient())
		//		{
		//			client.BaseAddress = new Uri(apiURL);
		//			CityRequest req = new CityRequest
		//			{
		//				userID = sUserId
		//			};
		//			var postTask = client.PostAsJsonAsync("GetUserCities", req);
		//			postTask.Wait();

		//			var result = postTask.Result;
		//			if (result.IsSuccessStatusCode)
		//			{
		//				var readTask = result.Content.ReadAsAsync<CityResponse>();
		//				readTask.Wait();

		//				if (readTask.Result.responseMessage.ToLower() == "success")
		//					cities = readTask.Result.cities;
		//				CityCount = cities.Count.ToString();
		//			}
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		//LogWriter.WriteToLog(ex);
		//	}
		//}
		//private void BindCycleDD()
		//{
		//	try
		//	{
		//		//Cycle
		//		DataTable dtCycle = new DataTable();
		//		using (var client = new HttpClient())
		//		{
		//			client.BaseAddress = new Uri(apiURL);

		//			var postTask = client.GetAsync("GetCycleList");
		//			postTask.Wait();

		//			var result = postTask.Result;
		//			if (result.IsSuccessStatusCode)
		//			{
		//				var readTask = result.Content.ReadAsAsync<CycleResponse>();
		//				readTask.Wait();

		//				if (readTask.Result.responseMessage.ToLower() == "success")
		//					cycle = readTask.Result.cycle;
		//			}
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		//LogWriter.WriteToLog(ex);
		//	}
		//}
		//private void BindBranch()
		//{
		//	try
		//	{
		//		//Branch
		//		int cityID = 10;
		//		int UserId = 26887;
		//		string BKCode = "999";
		//		int BankId = 1;

		//		DataTable dtBranch = new DataTable();
		//		using (var client = new HttpClient())
		//		{
		//			client.BaseAddress = new Uri(apiURL);
		//			BranchboardBranchRequest req = new BranchboardBranchRequest
		//			{
		//				BankCode = BKCode,
		//				BankId = BankId,
		//				CityId = cityID,
		//				UserId = BankId,
		//			};

		//			var postTask = client.PostAsJsonAsync("GetBranch", req);
		//			postTask.Wait();

		//			var result = postTask.Result;
		//			if (result.IsSuccessStatusCode)
		//			{
		//				var readTask = result.Content.ReadAsAsync<BranchResponse>();
		//				readTask.Wait();

		//				if (readTask.Result.responseMessage.ToLower() == "success")
		//					branches = readTask.Result.branches;
		//			}
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		//LogWriter.WriteToLog(ex);
		//	}

		//}
		//public List<HostNIBC> GetFiles(string DDL_Bank, string DDL_City, string DDL_Cycle, string DDL_Branch)
		//{
		//	try
		//	{
		//		string sQry = string.Empty;
		//		string sDateTime = "2023-08-02";
		//		string sBankCode = "54"; //DDL_Bank.SelectedValue.ToString();
		//		string sBranchCode = "212";
		//		string sCycleCode = "2";
		//		string sCityCode = "72";
		//		DataTable dtSort = new DataTable();
		//		DataTable dt = new DataTable();

		//		sQry = $"sp_vTransactionwiseInward '{sDateTime}','{sBankCode}','{sBranchCode}','{sCycleCode}','{sCityCode}' ";
		//		using (var client = new HttpClient())
		//		{
		//			client.BaseAddress = new Uri(apiURL);





		//			//HTTP GET
		//			var responseTask = client.GetAsync($"GetReportListing?sSQL={sQry}");
		//			responseTask.Wait();

		//			var result = responseTask.Result;
		//			if (result.IsSuccessStatusCode)
		//			{

		//				var readTask = result.Content.ReadAsAsync<List<HostNIBC>>();
		//				readTask.Wait();

		//				// if (readTask.Result.responseMessage.ToLower() == "success")
		//				obj = readTask.Result;
		//			}
		//		}
		//		//    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(HostNIBC));
		//		//    foreach (PropertyDescriptor p in props)
		//		//        dt.Columns.Add(p.Name, p.PropertyType);
		//		//    foreach (var c in obj)
		//		//         dt.Rows.Add(c.hostId, c.senderBankId, c.senderBankName, c.senderBranchId, c.senderBranchName, c.recieverBankId,
		//		//            c.recieverBankName, c.recieverBranchId, c.recieverBranchName, c.trCode, c.cycleId, c.cycleDescription, c.chequeNo,
		//		//            c.accountNo, c.trId, c.IWOWId, c.clearingType, c.cityId, c.cityName, c.amount, c.IQA, c.UV, c.bar, c.MICR, c.std,
		//		//            c.dup, c.WMark, c.avgChqSize, c.tChq, c.tChqR, c.truncTime, c.truncBy, c.settled, c.setTime, c.setBy, c.authBy, c.authDateTime,
		//		//            c.isAuth, c.u   nderSizeImage, c.folderOrTornDocumentCorners, c.foldedOrTornDocumentEdges, c.framingError, c.documentSkew, c.overSizeImage,
		//		//            c.piggyBack, c.imageTooLight, c.imageTooDark, c.horizontalStreaks, c.belowMinimumCompressedImageSize, c.aboveMaximumCompressedImageSize,
		//		//            c.spotNoise, c.frontRearDimensionMismatch, c.carbonStrip, c.outOfFocus, c.reasonID2, c.reason3, c.comment1, c.reasonID3, c.reason2, c.comment2,
		//		//            c.comment3, c.isDeffer);
		//		//    if (dt.Rows.Count > 0)
		//		//    {
		//		//        dtSort = dt.Clone();
		//		//        dtSort.Columns["repsize"].DataType = Type.GetType("System.Decimal");
		//		//        dtSort.Columns["reploadtime"].DataType = Type.GetType("System.DateTime");

		//		//        foreach (DataRow dr in dt.Rows)
		//		//        {
		//		//            dtSort.ImportRow(dr);
		//		//        }
		//		//        dtSort.AcceptChanges();
		//		//        dt = dtSort.Copy();
		//		//    }

		//		//    return dt;
		//		//}
		//		//catch (Exception ex)
		//		//{
		//		//    //LogWriter.WriteToLog(ex);
		//		//    return null;
		//		return obj;
		//	}
		//	catch (Exception ex)
		//	{
		//		//LogWriter.WriteToLog(ex);
		//		return obj;
		//	}
		//}
		//public FileResult OnGetDownloadFile(string CirId, string Fname)
		//{

		//	String DtTm = "";
		//	DtTm = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
		//	DtTm = DtTm.Replace(" ", "").Replace("/", "").Replace(":", "");
		//	byte[] MyData = new byte[0];

		//	using (var client = new HttpClient())
		//	{
		//		client.BaseAddress = new Uri(apiURL);

		//		CircularRequest req = new CircularRequest
		//		{
		//			CirId = Convert.ToInt32(CirId)
		//		};
		//		CircularReport circular = new CircularReport();
		//		var responseTask = client.PostAsJsonAsync<CircularRequest>("GetBankHelpFileCirID", req);
		//		responseTask.Wait();

		//		var result = responseTask.Result;
		//		if (result.IsSuccessStatusCode)
		//		{

		//			var readTask = result.Content.ReadAsAsync<CircularFileCirIDResponse>();
		//			readTask.Wait();

		//			if (readTask.Result.responseMessage.ToLower() == "success")
		//				circular = readTask.Result.circularFile;
		//		}

		//		MyData = circular.Report; //(byte[]) myRow["Report"];
		//		long fileSize = (long)MyData.Length;
		//		int ArraySize = new int();
		//		ArraySize = MyData.GetUpperBound(0);

		//		//string sUserId = Convert.ToString(Session["myid"]).Trim();

		//		//HTTP POST
		//		CircularDownLogRequest req2 = new CircularDownLogRequest
		//		{
		//			userid = "1",//Session["myid"].ToString(),
		//			userlogid = 2,//Convert.ToInt32(Session["userlogid"]),
		//			circularid = Convert.ToInt32(CirId)
		//		};

		//		var postTask = client.PutAsJsonAsync<CircularDownLogRequest>("setBankHelpDownLog", req2);
		//		postTask.Wait();

		//		var result2 = postTask.Result;

		//		//LogWriter.WriteToLog(sSQL);

		//		//Send the File to Download.  
		//		return File(MyData, "application/octet-stream", Fname);

		//	}

		//}

	}
    public class branchboarddetails
    {
        public string? bkcode { get; set; }
        public string? citycode { get; set; }
        public string? cycle { get; set; }
        public string? brcode { get; set; }
        public string? bkType { get; set; }
    }
}
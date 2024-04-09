using IBCS_Core_Web_Portal.Helper;
using IBCS_Web_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.Data;
using static IBCS_Core_Web_Portal.Pages.Marking.NetboardModel;

namespace IBCS_Core_Web_Portal.Pages.Marking
{
    public class CityboardModel : PageModel
    {
        public static string apiURL = "";
        public static string img_apiURL = "";
        public static string userid = "";
        public static string userlogid = "";
        public static string bankcode = "";
        public static string dtFrom_ = "";
        public static string dtTo_ = "";
        public static string CityapiURL = "";
        public static string BranchApiURL = "";
        public static string Auth_ = "";
        public DataTable _pakTable;
        public DataTable _cityTable;
        public DataTable _branchTable;
        public DataTable _tranTable;
        public List<HostNIBC> obj { get; set; }

        public static DataTable oDt;
        vNetScorePakwiseRequest req = new vNetScorePakwiseRequest();
        public List<vNetScorePakwise> obj1 { get; set; }
        public List<vNetScoreCitywise> obj2 { get; set; }
        public List<vNetScoreBranchwise> obj3 { get; set; }
        public List<vTransactionwise> obj4 { get; set; }
        public List<InstBank> instBanks { get; set; }
        public static string selected_city, selected_bank;
        public static string sel_ddlbank, sel_ddlcity, sel_ddlcycle, sel_ddlbranch;
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

                if (cpa.Validatepageallowed(userid, "Cityboard") == false)
                {
                    Response.Redirect("/NotAllowed",true);
                }
                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                CityapiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "Cityboard/";
                BranchApiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "Branchboard/";
                //img_apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.img") + "GetImageFromBin/";
                img_apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "GetImageFromBin/";
                BindBankDD();
                GetFiles();
            }
            catch (Exception ex)
            {
				LogWriter.WriteToLog("Exception on Get - " + ex);
				Response.Redirect("/NotAllowed",true);
			}
        }

        private void BindBankDD()
        {
            try
            {
                // Bank
                string sUserId = userid;//"26887";//Convert.ToString(Session["myid"]).Trim();
                string sInstId = bankcode;//"54";//Session["BankCode"].ToString();
                DataTable dtInst = new DataTable();
                InstRequest req = new InstRequest()
                {
                    UserId = sUserId,
                    InstId = sInstId
                };

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BranchApiURL);

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
                //LogWriter.WriteToLog(ex);
            }
        }
        public List<vNetScorePakwise> GetFiles()
        {
            try
            {
                string pdate = dtFrom_;
                string bkcode = bankcode;
                DataTable oDt = new DataTable();
                _pakTable = new DataTable();


                using (var client = new HttpClient())
                {

                    var req = new vNetScorePakwiseRequest()
                    {
                        pdate = pdate,
                        bkcode = bkcode
                    };

                    client.BaseAddress = new Uri(CityapiURL);
                    var postTask = client.PostAsJsonAsync<vNetScorePakwiseRequest>("GetvNetScorePakwise", req);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<vNetScorePakwiseResponse>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                            obj1 = readTask.Result.vNetResult;

                        if (obj1 != null)
                        {
                            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(vNetScorePakwise));

                            foreach (PropertyDescriptor p in props)
                                oDt.Columns.Add(p.Name, p.PropertyType);
                            foreach (var c in obj1)
                                oDt.Rows.Add(c.BankID, c.BankName, c.CycleID, c.cycle_desc, c.IWOWID, c.ClgType, c.Items, c.Amount);
                        }
                    }
                }
                if (oDt == null || oDt.Rows.Count == 0)
                {
                    DataTable dt = new DataTable();
                    //gridMain1.DataSource = dt;
                    //  gridMain1.DataBind();
                    // Session["CNPakTable"] = null;
                }
                else
                {
                    if (oDt.Rows.Count > 0)
                    {
                        _pakTable = oDt;
                        // Session["CNPakTable"] = _pakTable;
                    }
                    else
                    {
                        //Session["CNPakTable"] = null;
                    }
                }


                ////---------------------------------------------------------------------------------
                //// City
                ////


                //DataTable oDt2 = new DataTable();
                //_cityTable = new DataTable();

                //// string sDateTime2 = Session["dtFrom"].ToString();
                //// string sBankCode2 = Session["BankCode"].ToString();

                //req.pdate = dtFrom_;// "2023-06-27";
                //req.bkcode = bankcode;// "54";


                //using (var client = new HttpClient())
                //{
                //    client.BaseAddress = new Uri(CityapiURL);
                //    var postTask = client.PostAsJsonAsync<vNetScorePakwiseRequest>("GetvNetScoreCitywise", req);
                //    postTask.Wait();

                //    var result = postTask.Result;
                //    if (result.IsSuccessStatusCode)
                //    {
                //        var readTask = result.Content.ReadAsAsync<vNetScoreCitywiseResponse>();
                //        readTask.Wait();

                //        obj2 = readTask.Result.vNetResult;
                //        if (obj2 != null)
                //        {
                //            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(vNetScoreCitywise));

                //            foreach (PropertyDescriptor p in props)
                //                oDt2.Columns.Add(p.Name, p.PropertyType);
                //            foreach (var c in obj2)
                //                oDt2.Rows.Add(c.BankID, c.BankName, c.CycleID, c.cycle_desc, c.Items, c.Amount, c.IWOWID, c.ClgType, c.CityCode, c.CityName);
                //        }
                //    }
                //}
                //////----------------------------------------------------------------------------------
                ////// Branch
                //////



                //DataTable oDt3 = new DataTable();
                //_branchTable = new DataTable();

                //// string sDateTime3 = Session["dtFrom"].ToString();
                ////string sBankCode3 = Session["BankCode"].ToString();

                //req.pdate = dtFrom_;// "2023-06-27";
                //req.bkcode = bankcode;// "54";


                //using (var client = new HttpClient())
                //{
                //    client.BaseAddress = new Uri(CityapiURL);
                //    var postTask = client.PostAsJsonAsync<vNetScorePakwiseRequest>("GetvNetScoreBranchwise", req);
                //    postTask.Wait();

                //    var result = postTask.Result;
                //    if (result.IsSuccessStatusCode)
                //    {
                //        var readTask = result.Content.ReadAsAsync<vNetScoreBranchwiseResponse>();
                //        readTask.Wait();

                //        obj3 = readTask.Result.vNetResult;
                //        if (obj3 != null)
                //        {
                //            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(vNetScoreBranchwise));

                //            foreach (PropertyDescriptor p in props)
                //                oDt3.Columns.Add(p.Name, p.PropertyType);
                //            foreach (var c in obj3)
                //                oDt3.Rows.Add(c.BankID, c.BankName, c.BranchID, c.BranchName, c.CycleID, c.cycle_desc, c.Items, c.Amount, c.IWOWID, c.ClgType,
                //                    c.CityCode, c.CityName);
                //        }
                //    }
                //}
                //////----------------------------------------------------------------------------------
                ////// Transaction
                //////
                //List<vTransactionwise> obj4 = new List<vTransactionwise>();
                //DataTable oDt4 = new DataTable();
                //_tranTable = new DataTable();

                ////string sDateTime4 = Session["dtFrom"].ToString();
                ////string sBankCode4 = Session["BankCode"].ToString();


                //req.pdate = dtFrom_;// "2023-06-15";
                //req.bkcode = bankcode;//"54";

                //using (var client = new HttpClient())
                //{
                //    client.BaseAddress = new Uri(CityapiURL);
                //    var postTask = client.PostAsJsonAsync<vNetScorePakwiseRequest>("GetvTransactionwise", req);
                //    postTask.Wait();

                //    var result = postTask.Result;
                //    if (result.IsSuccessStatusCode)
                //    {
                //        var readTask = result.Content.ReadAsAsync<vTransactionwiseResponse>();
                //        readTask.Wait();

                //        obj4 = readTask.Result.vTran;

                //        if (obj4 != null)
                //        {
                //            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(vTransactionwise));

                //            foreach (PropertyDescriptor p in props)
                //                oDt4.Columns.Add(p.Name, p.PropertyType);
                //            foreach (var c in obj4)
                //                oDt4.Rows.Add(c.hostid, c.SBkId, c.SBkNm, c.SBrId, c.SBrNm, c.RBkId, c.RBkNm, c.RBrId, c.RBrNm, c.TrCode,
                //                    c.CycleId, c.CycleDesc, c.ChqNo, c.IQA, c.UV, c.MICR, c.Dup, c.WMark, c.AvgChqSize, c.TChqR);
                //        }
                //    }
                //}
                return obj1;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return obj1;
            }
        }

        public JsonResult OnGetLoadDataFromDB(string bank)
        {
            List<vNetScorePakwise> ht2 = new List<vNetScorePakwise>();
            try
            {
                string sIWOWID = "";
                string sCycleID = "";
                string pdate = dtFrom_;
                string bkcode = bank;
                DataTable oDt = new DataTable();
                _pakTable = new DataTable();
                List<vNetScorePakwise> ht1 = new List<vNetScorePakwise>();

                using (var client = new HttpClient())
                {
                    var req = new vNetScorePakwiseRequest()
                    {
                        pdate = pdate,
                        bkcode = bkcode
                    };

                    client.BaseAddress = new Uri(CityapiURL);
                    var postTask = client.PostAsJsonAsync<vNetScorePakwiseRequest>("GetvNetScorePakwise", req);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<vNetScorePakwiseResponse>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                            obj1 = readTask.Result.vNetResult;

                        if (obj1 != null)
                        {
                            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(vNetScorePakwise));

                            foreach (PropertyDescriptor p in props)
                                oDt.Columns.Add(p.Name, p.PropertyType);
                            foreach (var c in obj1)
                                oDt.Rows.Add(c.BankID, c.BankName, c.CycleID, c.cycle_desc, c.IWOWID, c.ClgType, c.Items, c.Amount);
                        }
                    }
                }
                return new JsonResult(obj1);

            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return new JsonResult("Failed");
            }
        }

        public JsonResult OnGetCityData(string ddlbank, string CycleID, string IWOWID)
        {
            try
            {
                //---------------------------------------------------------------------------------
                // City
                //


                DataTable oDt2 = new DataTable();
                _cityTable = new DataTable();
                List<vNetScoreCitywise> ht2 = new List<vNetScoreCitywise>(); 
                // string sDateTime2 = Session["dtFrom"].ToString();
                // string sBankCode2 = Session["BankCode"].ToString();

                req.pdate = dtFrom_;// "2023-06-27";
                req.bkcode = ddlbank;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(CityapiURL);
                    var postTask = client.PostAsJsonAsync<vNetScorePakwiseRequest>("GetvNetScoreCitywise", req);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<vNetScoreCitywiseResponse>();
                        readTask.Wait();

                        obj2 = readTask.Result.vNetResult;
                        if (obj2 != null)
                        {
                            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(vNetScoreCitywise));

                            foreach (PropertyDescriptor p in props)
                                oDt2.Columns.Add(p.Name, p.PropertyType);
                            foreach (var c in obj2)
                                oDt2.Rows.Add(c.BankID, c.BankName, c.CycleID, c.cycle_desc, c.Items, c.Amount, c.IWOWID, c.ClgType, c.CityCode, c.CityName);
                        }
                    }
                }
                if (oDt2.Rows.Count > 0)
                {
                    oDt2.DefaultView.RowFilter = String.Format("IWOWID = '{0}'", IWOWID) + " And " + String.Format("CycleID = '{0}'", CycleID);
                    DataTable dt_ = oDt2.DefaultView.ToTable();
                    ht2 = CommonMethod.ConvertToList<vNetScoreCitywise>(dt_);
                }
                return new JsonResult(ht2);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return new JsonResult("Failed");

            }
        }
        public JsonResult OnGetBranchData(string ddlbank,string CycleID, string IWOWID, string Cityid)
        {
            try
            {
                ////----------------------------------------------------------------------------------
                //// Branch
                ////

                DataTable oDt3 = new DataTable();
                _branchTable = new DataTable();
                List<vNetScoreBranchwise> ht3 = new List<vNetScoreBranchwise>();
                // string sDateTime3 = Session["dtFrom"].ToString();
                //string sBankCode3 = Session["BankCode"].ToString();

                req.pdate = dtFrom_;// "2023-06-27";
                req.bkcode = ddlbank;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(CityapiURL);
                    var postTask = client.PostAsJsonAsync<vNetScorePakwiseRequest>("GetvNetScoreBranchwise", req);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<vNetScoreBranchwiseResponse>();
                        readTask.Wait();

                        obj3 = readTask.Result.vNetResult;
                        if (obj3 != null)
                        {
                            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(vNetScoreBranchwise));

                            foreach (PropertyDescriptor p in props)
                                oDt3.Columns.Add(p.Name, p.PropertyType);
                            foreach (var c in obj3)
                                oDt3.Rows.Add(c.BankID, c.BankName, c.BranchID, c.BranchName, c.CycleID, c.cycle_desc, c.Items, c.Amount, c.IWOWID, c.ClgType,
                                    c.CityCode, c.CityName);
                        }
                    }
                    if (oDt3.Rows.Count > 0)
                    {
                        oDt3.DefaultView.RowFilter = String.Format("IWOWID = '{0}'", IWOWID) + " And " + String.Format("CycleID = '{0}'", CycleID) + " And " + String.Format("CityCode = '{0}'", Cityid);
                        DataTable dt_ = oDt3.DefaultView.ToTable();
                        ht3 = CommonMethod.ConvertToList<vNetScoreBranchwise>(dt_);
                    }
                }
                return new JsonResult(ht3);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return new JsonResult("Failed");

            }
        }

        public JsonResult OnGetTransactionData(string ddlbank, string CycleID, string IWOWID, string Cityid, string branchID)
        {
            try
            {
                ////----------------------------------------------------------------------------------
                //// Transaction
                ////
                List<vTransactionwise> ht4 = new List<vTransactionwise>();
                DataTable oDt4 = new DataTable();
                _tranTable = new DataTable();

                req.pdate = dtFrom_;// "2023-06-15";
                req.bkcode = ddlbank;//"54";

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(CityapiURL);
                    var postTask = client.PostAsJsonAsync<vNetScorePakwiseRequest>("GetvTransactionwise", req);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<vTransactionwiseResponse>();
                        readTask.Wait();

                        obj4 = readTask.Result.vTran;

                        if (obj4 != null)
                        {
                            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(vTransactionwise));

                            foreach (PropertyDescriptor p in props)
                                oDt4.Columns.Add(p.Name, p.PropertyType);
                            foreach (var c in obj4)
                                oDt4.Rows.Add(c.hostid, c.SBkId, c.SBkNm, c.SBrId, c.SBrNm, c.RBkId, c.RBkNm, c.RBrId, c.RBrNm, c.TrCode,c.CityId,c.CityName,
                                    c.CycleId, c.CycleDesc, c.ChqNo,c.Accno,c.TrId,c.IWOWID,c.ClgType, c.Amount, c.IQA, c.UV, c.QR,c.Std, c.MICR, c.Dup,c.isFraud,c.isDeffer, c.WMark,c.TempID,c.UVPerc, c.AvgChqSize, c.TChqR,c.TChq
                                    ,c.underSizeImage,c.folderOrTornDocumentCorners,c.foldedOrTornDocumentEdges,c.framingError,c.documentSkew,c.overSizeImage
                                    ,c.piggyBack,c.imageTooLight,c.imageTooDark,c.horizontalStreaks,c.belowMinimumCompressedImageSize,c.aboveMaximumCompressedImageSize,
                                    c.spotNoise,c.frontRearDimensionMismatch,c.carbonStrip,c.outOfFocus);

                            if (IWOWID == "0")
                            {
                                oDt4.DefaultView.RowFilter = String.Format("IWOWID = '{0}'", IWOWID) + " And " + String.Format("CycleID = '{0}'", CycleID) + " And " + String.Format("CityId = '{0}'", Cityid) + " And " + String.Format("RBrId = '{0}'", branchID);
                            }
                            else
                            {
                                oDt4.DefaultView.RowFilter = String.Format("IWOWID = '{0}'", IWOWID) + " And " + String.Format("CycleID = '{0}'", CycleID) + " And " + String.Format("CityId = '{0}'", Cityid) + " And " + String.Format("SBrId = '{0}'", branchID);
                            }
                            DataTable dt_ = oDt4.DefaultView.ToTable();
                            ht4 = CommonMethod.ConvertToList<vTransactionwise>(dt_);
                        } 
                    }
                }
                return new JsonResult(ht4);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return new JsonResult("Failed");
            }
        }
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
                    client1.BaseAddress = new Uri(BranchApiURL);


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
                LogWriter.WriteToLog("Exception on OnGetCheckImages - " + ex);
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
        //public List<HostNIBC> OnGetCheckImages(string hostId)
        //{
        //    try
        //    {
        //        string sQry = string.Empty;
        //        string sDateTime = dtFrom_;//"2023-08-02";
        //        string sBankCode = DDL_Bank;//"54"; //DDL_Bank.SelectedValue.ToString();
        //        string sBranchCode = DDL_Branch;// "212";
        //        string sCycleCode = DDL_Cycle;// "2";
        //        string sCityCode = DDL_City;// "72";
        //        DataTable dtSort = new DataTable();
        //        DataTable dt = new DataTable();
        //        oDt = new DataTable();
        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(BranchApiURL);
        //            //HTTP GET
        //            var responseTask = client.GetAsync($"GetReportListing?sDateTime={sDateTime}&sBankCode={sBankCode}&sBranchCode={sBranchCode}" +
        //                        $"&sCycleCode={sCycleCode}&sCityCode={sCityCode}");
        //            responseTask.Wait();

        //            var result = responseTask.Result;
        //            if (result.IsSuccessStatusCode)
        //            {

        //                var readTask = result.Content.ReadAsAsync<List<HostNIBC>>();
        //                readTask.Wait();

        //                // if (readTask.Result.responseMessage.ToLower() == "success")
        //                obj = readTask.Result;
        //                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(HostNIBC));

        //                foreach (PropertyDescriptor p in props)
        //                    oDt.Columns.Add(p.Name, p.PropertyType);
        //                foreach (var c in obj)
        //                    oDt.Rows.Add(c.hostId, c.senderBankId, c.senderBankName, c.senderBranchId, c.senderBranchName, c.recieverBankId,
        //                        c.recieverBankName, c.recieverBranchId, c.recieverBranchName, c.trCode, c.cycleId, c.cycleDescription, c.chequeNo,
        //                        c.accountNo, c.trId, c.IWOWId, c.clearingType, c.cityId, c.cityName, c.amount, c.IQA, c.UV, c.bar, c.MICR, c.std,
        //                        c.dup, c.WMark, c.avgChqSize, c.tChq, c.tChqR, c.truncTime, c.truncBy, c.settled, c.setTime, c.setBy, c.authBy, c.authDateTime,
        //                        c.isAuth, c.underSizeImage, c.folderOrTornDocumentCorners, c.foldedOrTornDocumentEdges, c.framingError, c.documentSkew, c.overSizeImage,
        //                        c.piggyBack, c.imageTooLight, c.imageTooDark, c.horizontalStreaks, c.belowMinimumCompressedImageSize, c.aboveMaximumCompressedImageSize,
        //                        c.spotNoise, c.frontRearDimensionMismatch, c.carbonStrip, c.outOfFocus, c.reasonID2, c.reason3, c.comment1, c.reasonID3, c.reason2, c.comment2,
        //                        c.comment3, c.isDeffer);
        //            }
        //            string sHostId = "156937";
        //            int userLogId = -1;
        //            string sUserId = "26887";

        //            using (var client1 = new HttpClient())
        //            {

        //                var req = new LogImageFileRequest()
        //                {
        //                    userId = Convert.ToInt32(sUserId),
        //                    userLogId = userLogId,
        //                    hostId = Convert.ToInt32(sHostId)
        //                };
        //                client1.BaseAddress = new Uri(BranchApiURL);


        //                var postTask = client1.PostAsJsonAsync<LogImageFileRequest>("LogImageFile", req);
        //                postTask.Wait();
        //                var result1 = postTask.Result;
        //            }
        //            Byte[] jpegBytes = null;
        //            using (var client1 = new HttpClient())
        //            {
        //                client1.BaseAddress = new Uri(BranchApiURL);

        //                var postTask = client1.GetAsync("GetImage?bFront=0&imageid=" + sHostId);
        //                postTask.Wait();
        //                var result1 = postTask.Result;
        //                if (result1.IsSuccessStatusCode)
        //                {
        //                    var readTask1 = result1.Content.ReadAsAsync<Byte[]>();
        //                    readTask1.Wait();
        //                    jpegBytes = readTask1.Result;
        //                    var base64Image = Convert.ToBase64String(jpegBytes);
        //                    var imageDataUrl = $"data:image/jpeg;base64,{base64Image}";
        //                    ViewData["ImageDataURL"] = imageDataUrl;
        //                }
        //            }

        //            using (var client2 = new HttpClient())
        //            {
        //                client2.BaseAddress = new Uri(BranchApiURL);

        //                var postTask = client2.GetAsync("GetImage?bFront=1&imageid=" + sHostId);
        //                postTask.Wait();

        //                var result2 = postTask.Result;
        //                if (result2.IsSuccessStatusCode)
        //                {
        //                    var readTask2 = result2.Content.ReadAsAsync<Byte[]>();
        //                    readTask2.Wait();

        //                    //if (readTask.Result.responseMessage.ToLower() == "success")
        //                    jpegBytes = readTask2.Result;
        //                    var base64Image = Convert.ToBase64String(jpegBytes);
        //                    var imageDataUrl = $"data:image/jpeg;base64,{base64Image}";
        //                    ViewData["ImageDataURL2"] = imageDataUrl;
        //                }
        //            }

        //            using (var client3 = new HttpClient())
        //            {
        //                client3.BaseAddress = new Uri(BranchApiURL);

        //                var postTask = client3.GetAsync("GetImage?bFront=2&imageid=" + sHostId);
        //                postTask.Wait();

        //                var result3 = postTask.Result;
        //                if (result3.IsSuccessStatusCode)
        //                {
        //                    var readTask3 = result3.Content.ReadAsAsync<Byte[]>();
        //                    readTask3.Wait();
        //                    jpegBytes = readTask3.Result;
        //                    var base64Image = Convert.ToBase64String(jpegBytes);
        //                    var imageDataUrl = $"data:image/jpeg;base64,{base64Image}";
        //                    ViewData["ImageDataURL3"] = imageDataUrl;
        //                }
        //            }

        //        }

        //        return obj;
        //    }
        //    catch (Exception ex)
        //    {
        //        return obj;
        //    }
        //}

        //public JsonResult OnGetSingleData(SingleRecord objSgl)
        //{
        //    string sHostId = objSgl.hostId.ToString();//"156937";
        //    DataTable dt2 = new DataTable();
        //    string condition = "HostId = " + "'" + sHostId + "'";
        //    oDt.DefaultView.RowFilter = condition;
        //    dt2 = oDt.DefaultView.ToTable();
        //    return new JsonResult("Success");
        //}
    }
}

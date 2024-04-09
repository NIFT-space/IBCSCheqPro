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
    public class UploadICLFileModel : PageModel
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

        public List<ICLData> reports { get; set; }


        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        public UploadICLFileModel(Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _environment = environment;
        }

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
                        reports = new List<ICLData>();
                    }
                    else
                    {
                        ///GOOD TO GO///
                        GetFiles();
                    }
                }
                else
                {
                    Response.Clear();
                    Response.Redirect("/Sessionexpire", true);
                    reports = new List<ICLData>();
                }

                Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate"); // HTTP 1.1.
                Response.Headers.Append("Pragma", "no-cache"); // HTTP 1.0.
                Response.Headers.Append("Last-Modified", System.DateTime.Now.ToString());

                if (userid == null)
                {
                    Response.Redirect("/Sessionexpire", true);
                    reports = new List<ICLData>();
                }
                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                apiURL_bktr = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "BkTrSc/";
                apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "FileUpload/";


                if (userid == null)
                {
                    return;
                }

            }
            catch (ThreadAbortException ex)
            {
                LogWriter.WriteToLog(ex);
                reports = new List<ICLData>();
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                Response.Redirect("/NotAllowed", true);
                reports = new List<ICLData>();
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
                    UploadICLDataRequest req = new UploadICLDataRequest
                    {
                        FromDate = dtFrom,
                        ToDate = dtTo,
                        InstID = Convert.ToInt32(bankcode),
                        BranchID = Convert.ToInt32(branchcode)
                    };

                    var postTask = client.PostAsJsonAsync<UploadICLDataRequest>("GetUploadICLData", req);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<UploadICLDataResponse>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                            reports = readTask.Result.data;
                        else
                            reports = new List<ICLData>();

                    }
                    else
                        reports = new List<ICLData>();
                }
                return new JsonResult(reports);
            }
            catch (Exception ex)
            {
                reports = new List<ICLData>();
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

                string RepId = v301x913;
                string RepName = c036x9210;
                String DtTm = "";
                DtTm = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
                DtTm = DtTm.Replace(" ", "").Replace("/", "").Replace(":", "");
                byte[] MyData = new byte[0];

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);

                    ICLDataFileRequest req = new ICLDataFileRequest
                    {
                        fileId = Convert.ToInt32(v301x913)
                    };
                    ICLDataFile circular = new ICLDataFile();
                    var responseTask = client.PostAsJsonAsync<ICLDataFileRequest>("getICLDataFile", req);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {

                        var readTask = result.Content.ReadAsAsync<ICLDataFileResponse>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                            circular = readTask.Result.ICLDataFile;
                    }

                    MyData = circular.fileData; //(byte[]) myRow["Report"];
                    long fileSize = (long)MyData.Length;
                    int ArraySize = new int();
                    ArraySize = MyData.GetUpperBound(0);

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
                LogWriter.WriteToLog("Exception on BindCycleDD - " + ex);
                return File(MyData, "application/octet-stream", c036x9210);
            }
        }




        public async Task OnPostAsync()
        {
            try
            {
                string filename = string.Empty;
                string fileContentType = string.Empty;
                string fileIWOW = string.Empty;
                string fileCycle = string.Empty;
                string fileCity = string.Empty;
                string fileBank = string.Empty;
                string fileBranch = string.Empty;
                string fileyyyy = string.Empty;
                string filemm = string.Empty;
                string filedd = string.Empty;
                string fileDate = string.Empty;
                string fileTime = string.Empty;
                string fileDot = string.Empty;
                string fileExt = string.Empty;

                bool isCycleClose = false;

                var file = Path.Combine(_environment.ContentRootPath, "Uploads", Upload.FileName);
                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    long iSize = Upload.Length;

                    if (iSize <= 102400000)
                    {
                        if (ValidateUploadFileHeader(fileStream))
                        {
                            if (ValidateUploadFile(file))
                            {
                                fileContentType = GetMimeTypeForFileExtension(file);
                                filename = Path.GetFileName(file);

                                if (filename.Length == 30)
                                {
                                    fileIWOW = filename.Substring(0, 1);
                                    fileCycle = filename.Substring(1, 2);
                                    fileCity = filename.Substring(3, 2);
                                    fileBank = filename.Substring(5, 3);
                                    fileBranch = filename.Substring(8, 4);
                                    fileyyyy = filename.Substring(12, 4);
                                    filemm = filename.Substring(16, 2);
                                    filedd = filename.Substring(18, 2);
                                    fileTime = filename.Substring(20, 6);
                                    fileDot = filename.Substring(26, 1);
                                    fileExt = filename.Substring(27, 3);

                                    string sCyc = GetCycleCpde(fileCycle);

                                    DateTime dt = DateTime.Now.Date;
                                    fileDate = fileyyyy + "-" + filemm + "-" + filedd;
                                    DateTime dt2 = DateTime.Parse(fileDate); //Convert.ToDateTime(fileDate);
                                    //if (dt.ToString("yyyy-mm-dd") != dt2.ToString("yyyy-mm-dd"))
                                    //{
                                    //    //lblMsg.Text = "";
                                    //    //string sMsg = "Date is Not Valid";
                                    //    //ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                                    //    //lblMsg.Text = sMsg;
                                    //    //return;
                                    //}
                                    //else 
                                    if (fileExt.ToUpper() != "ZIP")
                                    {
                                        //lblMsg.Text = "";
                                        //string sMsg = "Invalid File Suffix";
                                        //ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                                        //lblMsg.Text = sMsg;
                                        //return;
                                    }
                                    else if (fileContentType != "application/x-zip-compressed")
                                    {
                                        //lblMsg.Text = "";
                                        //string sMsg = "Invalid File Type";
                                        //ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                                        //lblMsg.Text = sMsg;
                                        //return;
                                    }
                                    else 
                                    {
                                        isCycleClose = IsCycleClose(fileCity, sCyc);
                                    }
                                    //await Upload.CopyToAsync(fileStream);
                                }
                            }
                        }
                    }
                }

                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    await Upload.CopyToAsync(fileStream);

                    if (isCycleClose)
                    {
                        string fileDateChk = string.Empty;
                        fileDate = fileyyyy + filemm + filedd;
                        fileDateChk = fileDate.Substring(0, 4);
                        fileDateChk += "-";
                        fileDateChk += fileDate.Substring(4, 2);
                        fileDateChk += "-";
                        fileDateChk += fileDate.Substring(6, 2);
                        IFormatProvider culture = new CultureInfo("en-US", true);
                        DateTime dt_ = DateTime.ParseExact(fileDateChk, "yyyy-MM-dd", culture);

                        using (BinaryReader br = new BinaryReader(fileStream))
                        {
                            fileStream.Position = 0;
                            bytes = br.ReadBytes((Int32)fileStream.Length);

                            //bytes = UseStreamDotReadMethod(fileStream);

                            var req = new ICLDataRequest
                            {
                                fileName = filename,
                                contentType = fileContentType,
                                bankId = Convert.ToInt32(bankcode),
                                branchId = Convert.ToInt32(branchcode),
                                cycleNo = Convert.ToInt32(fileCycle),
                                cityCode = Convert.ToInt32(fileCity),
                                webId = Convert.ToInt32(userid),
                                userEmail = emailaddress,
                                validFlag = 0,
                                fileData = bytes
                            };

                            using (var client = new HttpClient())
                            {
                                client.BaseAddress = new Uri(apiURL);
                                //HTTP GET
                                var responseTask = client.PostAsJsonAsync("insertICLData", req);
                                responseTask.Wait();

                                var result = responseTask.Result;
                                if (result.IsSuccessStatusCode)
                                {

                                    var readTask = result.Content.ReadAsAsync<ICLDataResponse>();
                                    readTask.Wait();

                                    if (readTask.Result.responseMessage.ToLower() == "success")
                                    {
                                        var req2 = new ICLLogRequest
                                        {
                                            fileName = filename,
                                            contentType = fileContentType,
                                            bankId = Convert.ToInt32(bankcode),
                                            branchId = Convert.ToInt32(branchcode),
                                            cycleNo = Convert.ToInt32(fileCycle),
                                            cityCode = Convert.ToInt32(fileCity),
                                            webId = Convert.ToInt32(userid),
                                            userEmail = emailaddress,
                                            recDateTime = DateTime.Now.ToShortDateString(),
                                            validFlag = 0
                                        };
                                        //HTTP GET
                                        var responseTask2 = client.PostAsJsonAsync("insertICLLog", req2);
                                        responseTask2.Wait();

                                        var result2 = responseTask2.Result;
                                        if (result2.IsSuccessStatusCode)
                                        {

                                            var readTask2 = result2.Content.ReadAsAsync<ICLDataResponse>();
                                            readTask2.Wait();

                                            if (readTask2.Result.responseMessage.ToLower() == "success")
                                            {
                                                //lblMsg.Text = "";
                                                //string sMsg = "File Uploaded Successfully";
                                                //ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                                                //lblMsg.Text = sMsg;
                                                //GridView1.PageIndex = 0;
                                                //this.LoadDataFromDB();
                                            }
                                        }
                                    }

                                }
                            }

                        }
                    }
                }

                GetFiles();
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception on OnPostAsync - " + ex);
            }
        }
        private bool IsCycleClose(string sCity, string sCycle)
        {
            try
            {
                string sCyc = string.Empty;
                string sEnd = "00:00";
                string inString = System.DateTime.Now.ToString();
                string sBankCode = bankcode;
                int iBankCode = 0;
                bool isParsable = Int32.TryParse(sBankCode, out iBankCode);

                //string sSQL = string.Empty;
                //sSQL = " Exec SP_CycleClose @cyclecode, @bkcode, @citycode";

                //cDataAccess odata = new cDataAccess();
                DataTable dtCyc = new DataTable();
                List<CycleClose> cycle = new List<CycleClose>();
                var req = new IsCycleCloseRequest
                {
                    CycleNo = Convert.ToInt32(sCycle),
                    CityId = Convert.ToInt32(sCity),
                    InstId = Convert.ToInt32(sBankCode)
                };

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);
                    //HTTP GET
                    var responseTask = client.PostAsJsonAsync("CheckCycleStatus", req);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {

                        var readTask = result.Content.ReadAsAsync<IsCycleCloseResponse>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                            cycle = readTask.Result.cycles;
                    }
                }
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(CycleClose));

                foreach (PropertyDescriptor p in props)
                    dtCyc.Columns.Add(p.Name, p.PropertyType);
                foreach (var c in cycle)
                    dtCyc.Rows.Add(c.CycleNo, c.CycleDesc, c.CycleStartTime, c.CycleEndTime);
                //dtCyc = odata.GetCycleTimeDataSet(sSQL, sCycle, sBankCode, sCity).Tables[0];

                if (dtCyc == null)
                    return true;

                if (dtCyc.Rows.Count > 0)
                {
                    sEnd = dtCyc.Rows[0]["cycleEndTime"].ToString();
                    sCyc = dtCyc.Rows[0]["cycledesc"].ToString();
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
                //edt = edt.AddDays(15);
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
        public byte[] UseStreamDotReadMethod(Stream stream)
        {
            byte[] bytes;
            List<byte> totalStream = new();
            byte[] buffer = new byte[32];
            int read;
            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                totalStream.AddRange(buffer.Take(read));
            }
            bytes = totalStream.ToArray();
            return bytes;
        }
        private string GetCycleCpde(string sCycle)
        {
            string result = "00";
            try
            {
                if (sCycle == "04")
                {
                    result = "02";
                }
                else if (sCycle == "07")
                {
                    result = "05";
                }
                else if (sCycle == "03")
                {
                    result = "01";
                }
                else if (sCycle == "22")
                {
                    result = "20";
                }
                /////SPECIAL CLEARING/////
                else if (sCycle == "41")
                {
                    result = "31";
                }
                else if (sCycle == "42")
                {
                    result = "32";
                }
                else if (sCycle == "43")
                {
                    result = "33";
                }
                else if (sCycle == "44")
                {
                    result = "34";
                }
                else if (sCycle == "45")
                {
                    result = "35";
                }
                return result;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return result;
            }
        }
        protected bool ValidateUploadFileHeader(FileStream  fs)
        {
            try
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    bytes = br.ReadBytes((Int32)fs.Length);
                }
                using (var inputStream = new MemoryStream(bytes))
                {
                    GZipStream decompStream = new GZipStream(inputStream, CompressionMode.Decompress);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return false;
            }
        }
        protected bool ValidateUploadFile(string filePath)
        {
            List<string> whiteListedMIMETypes = new List<string>();
            whiteListedMIMETypes.Add("application/zip");
            whiteListedMIMETypes.Add("application/x-zip-compressed");
            try
            {
                string ContentType = GetMimeTypeForFileExtension(filePath);

                // Check the list to see if the uploaded file is of an acceptable type
                if (whiteListedMIMETypes.Contains(ContentType.ToLowerInvariant()))
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
        public string GetMimeTypeForFileExtension(string filePath)
        {
            const string DefaultContentType = "application/octet-stream";

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(filePath, out string contentType))
            {
                contentType = DefaultContentType;
            }

            return contentType;
        }
    }
}

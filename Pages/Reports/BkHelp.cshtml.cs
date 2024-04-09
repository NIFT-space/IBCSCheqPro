using IBCS_Core_Web_Portal.Helper;
using IBCS_Core_Web_Portal.Models;
using IBCS_Web_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.Data;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace IBCS_Core_Web_Portal.Pages.Reports
{
    public class BkHelp : PageModel
    {
        public const string SessionCount = "_count";
        public const string SessionFile = "_file";
        public static string apiURL = "";
        public static string userid = "";
        public static string userlogid = "";
        public static string Auth_ = "";
        public List<Circular> circular { get; set;}
        public static List<Circular> repsave;
        public void OnGet()
        {
            try
            {
                userid = HttpContext.Session.GetString(loginModel.SessionKeyName1);
                userlogid = HttpContext.Session.GetString(loginModel.SessionKeyName9);
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
                apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "BkHelp/";

                GetFiles();
            }
            catch  (Exception ex) {
                LogWriter.WriteToLog("Exception on OnGet - " + ex);
                Response.Redirect("/NotAllowed",true);
            }
        }

        protected DataTable GetFiles()
        {
            try
            {
                
                DataTable dtSort = new DataTable();
                DataTable dt = new DataTable();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);
                    //HTTP GET
                    var responseTask = client.GetAsync("GetBankHelpFiles");
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {

                        var readTask = result.Content.ReadAsAsync<CircularFileResponse>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                            circular = readTask.Result.circularFile;

                        if (circular != null)
                        {
                            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(Circular));

                            foreach (PropertyDescriptor p in props)
                                dt.Columns.Add(p.Name, p.PropertyType);
                            foreach (var c in circular)
                                dt.Rows.Add(c.CirId, c.RepDesc, c.RepSize,c.Repname, c.RepLoadTime, c.BSpecial, c.BDownload, c.FileUpdateDate, c.FileUpdateTime);
                        }

                    }
                }
                
                if (dt.Rows.Count > 0)
                {
                    dtSort = dt.Clone();
                    dtSort.Columns["repsize"].DataType = Type.GetType("System.Decimal");
                    dtSort.Columns["reploadtime"].DataType = Type.GetType("System.DateTime");

                    foreach (DataRow dr in dt.Rows)
                    {
                        dtSort.ImportRow(dr);
                    }
                    dtSort.AcceptChanges();
                    dt = dtSort.Copy();
                }

                return dt;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception on GetFiles - " + ex);
                return null;
            }
        }

        public ActionResult OnPostDownloadFile(string? v301x913, string? c036x9210)
        {
            try {

                int chkr = 0;
                int t_ = 0;
                if (HttpContext.Session.GetString(SessionFile) != null)
                {
                    if (HttpContext.Session.GetString(SessionFile) == v301x913)
                    {
                        if (HttpContext.Session.GetString(BkHelp.SessionCount) != null)
                        {
                            t_ = Convert.ToInt32(HttpContext.Session.GetString(BkHelp.SessionCount));
                        }

                        if (t_ < 5)
                        {

                        }
                        else
                        {
                            LogWriter.WriteToLog("Download Limit Reached. Please try again later!");
                            Response.Redirect("/Reports/BkHelp?dolrchd=1");
                            
                        }
                    }
                    else
                    {
                        HttpContext.Session.SetString(SessionFile, v301x913);
                        HttpContext.Session.SetString(BkHelp.SessionCount, "0");
                    }
                }
                else
                {
                    HttpContext.Session.SetString(SessionFile, v301x913);
                    HttpContext.Session.SetString(BkHelp.SessionCount, "0");
                }

                if (repsave != null)
                {
                    foreach (var report in repsave)
                    {
                        if (Convert.ToString(report.CirId) == v301x913)
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
                        Response.Redirect("/Reports/BkHelp");
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

                    CircularRequest req = new CircularRequest
                    {
                        CirId = Convert.ToInt32(v301x913)
                    };
                    CircularReport circular = new CircularReport();
                    var responseTask = client.PostAsJsonAsync<CircularRequest>("GetBankHelpFileCirID", req);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {

                        var readTask = result.Content.ReadAsAsync<CircularFileCirIDResponse>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                            circular = readTask.Result.circularFile;
                    }

                    MyData = circular.Report; //(byte[]) myRow["Report"];
                    long fileSize = (long)MyData.Length;
                    int ArraySize = new int();
                    ArraySize = MyData.GetUpperBound(0);

                    //HTTP POST
                    CircularDownLogRequest req2 = new CircularDownLogRequest
                    {
                        userid = userid,
                        userlogid = Convert.ToInt32(userlogid),
                        circularid = Convert.ToInt32(v301x913)
                    };

                    var postTask = client.PutAsJsonAsync<CircularDownLogRequest>("setBankHelpDownLog", req2);
                    postTask.Wait();

                    var result2 = postTask.Result;

                    //LogWriter.WriteToLog(sSQL);

                    //Send the File to Download.  
                    string ct = HttpContext.Session.GetString(BkHelp.SessionCount);
                    string ct_str = Convert.ToString(Convert.ToInt32(ct) + 1);
                    ct = ct_str;
                    HttpContext.Session.SetString(SessionCount, ct_str);
                    return File(MyData, "application/octet-stream", c036x9210);
                }
            }
            catch(Exception ex)
            {
                byte[] MyData = new byte[0];
                LogWriter.WriteToLog("Exception on OnGet - " + ex);
                return File(MyData, "application/octet-stream", c036x9210);
            }
        }
    }
}

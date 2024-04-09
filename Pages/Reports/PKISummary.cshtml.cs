using IBCS_Core_Web_Portal.Helper;
using IBCS_Web_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NIBC.Models;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Xml.Linq;

namespace IBCS_Core_Web_Portal.Pages.Reports
{
    public class PKISummaryModel : PageModel
    {
        public static string apiURL = "", apiURL_br="";
        public static string userid = "";
        public static string userlogid = "";
        public static string bankcode = "";
        public static string dtFrom_ = "";
        public static string dtTo_ = "";
        public static string Auth_ = "";
        public List<InstBank> instBanks { get; set; }
        public List<City> cities { get; set; }
        public List<Cycle> cycle { get; set; }
        public List<DongleReport> reports { get; set; }
        public static DataTable pdt_;
        public class BrReturn
        {
            public string sel_ddlbank { get; set; }
        }

        string sel_bank = "";
        public void OnGet(string d1)
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
            apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "PKISummary/";
            apiURL_br = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "BkTrSc/";
            BindBankDD();
            if (d1 != null)
            {
                reports = GetFiles(d1);
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
                    client.BaseAddress = new Uri(apiURL_br);

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
        public void OnGetGetDdlbank(BrReturn obj1)
        {

            if (obj1 != null)
            {
                sel_bank = obj1.sel_ddlbank;
            }
        }
        public List<DongleReport> GetFiles(string DDL_Bank)
        {
            try
            {
                DataTable _dgTable = new DataTable();
                DataTable oDt = new DataTable();

                if (bankcode == DDL_Bank)
                {
                    pdt_ = new DataTable();
                    PKISummaryRequest req = new PKISummaryRequest()
                    {
                        BankId = Convert.ToInt32(DDL_Bank)
                    };

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(apiURL);

                        var postTask = client.PostAsJsonAsync<PKISummaryRequest>("getPKISummary", req);
                        postTask.Wait();

                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<PKISummaryResponse>();
                            readTask.Wait();

                            if (readTask.Result.responseMessage.ToLower() == "success")
                                reports = readTask.Result.reports;

                            if (reports != null)
                            {
                                if (reports.Count > 0)
                                {
                                    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(DongleReport));

                                    foreach (PropertyDescriptor p in props)
                                        oDt.Columns.Add(p.Name, p.PropertyType);
                                    foreach (var c in reports)
                                        oDt.Rows.Add(c.UserID, c.UserName, c.FirstName, c.LastName, c.DeptID, c.DesigID, c.EmailAddress, c.IsActive, c.IsCharged,
                                            c.AuditID, c.InstID, c.BranchID, c.IsPwdChanged, c.IsBranchUser, c.Locality, c.Title, c.CertificateStatus, c.Country
                                            , c.CreationDateTime, c.IsAuth, c.BranchName, c.TelNo3);
                                }
                            }
                        }
                    }
                    if (oDt.Rows.Count > 0)
                    {
                        pdt_ = oDt;
                        //TempData["DongleRpt"] = reports;
                        //TempData.Keep();
                    }
                    if (oDt.Rows.Count > 0)
                    {
                        _dgTable = oDt;

                        DataTable dt1 = new DataTable();
                        DataTable dt2 = new DataTable();
                        DataTable dt3 = new DataTable();

                        dt1 = oDt.Copy();

                        _dgTable.DefaultView.RowFilter = String.Format("IsActive" + " = 1");
                        dt2 = _dgTable.DefaultView.ToTable();

                        _dgTable.DefaultView.RowFilter = String.Format("IsActive" + " = 0");
                        dt3 = _dgTable.DefaultView.ToTable();

                        //ViewState["DongleRpt"] = dt1;
                        //ViewState["DongleActive"] = dt2;
                        //ViewState["DongleInActive"] = dt3;
                    }
                    else
                    {
                        //ViewState["DongleRpt"] = null;
                        //ViewState["DongleActive"] = null;
                        //ViewState["DongleInActive"] = null;
                    }
                }
                return reports;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception on reports - " + ex);
                return reports;
            }
        }
        //public FileResult OnGetDownloadData()
        //{
        //    string filename = "PKR-Clearing.xlsx";
        //    try
        //    {
        //        StringBuilder sb = null;
        //        DataTable dt = new DataTable();
        //        GVtoCSV odt = new GVtoCSV();


        //        if (pdt_.Rows.Count > 0)
        //        {
        //            dt = pdt_;
        //            if (dt.Rows.Count > 0)
        //            {
        //                DataTable dt1 = new DataTable();
        //                dt1 = dt.DefaultView.ToTable(false, new string[] { "FirstName", "LastName", "Title", "branchname", "IsActive", "TelNo3" });
        //                dt1.Columns["FirstName"].ColumnName = "First_Name";
        //                dt1.Columns["LastName"].ColumnName = "Last_Name";
        //                dt1.Columns["branchname"].ColumnName = "Branch_name";
        //                dt1.Columns["IsActive"].ColumnName = "Is_Active";
        //                dt1.Columns["TelNo3"].ColumnName = "Expiry_Date";
        //                dt1.AcceptChanges();
        //                sb = odt.ExportGridToCSV(dt1);
        //            }
        //        }
        //        return File(sb.ToString(), "application/octet-stream", filename);


        //    }
        //    catch (Exception ex)
        //    {
        //        byte[] MyData = new byte[0];
        //        LogWriter.WriteToLog("Exception on Downloadfile - " + ex);
        //        return File(MyData, "application/octet-stream", filename);
        //    }
        //}
    }
}

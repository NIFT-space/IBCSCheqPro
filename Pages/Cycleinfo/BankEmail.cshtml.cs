using IBCS_Core_Web_Portal.Helper;
using IBCS_Web_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Xml;

namespace IBCS_Core_Web_Portal.Pages.Cycleinfo
{
    public class BankEmailModel : PageModel
    {
        #region User
        public class User
        {
            public string userID { get; set; }
        }
        public class Person
        {
            public string Name { get; set; }
        }
        #endregion
        public List<City> city { get; set; }
        public List<Branch> Branch { get; set; }
        public List<Domain> Domain { get; set; }
        public List<BankUserEmail> BankUserDetails { get; set; }
        public static string apiURL = "";
        public static string userid = "";
        public static string userlogid = "";
        public static string bankcode = "";
        public static string branchcode = "";
        public static string dtFrom_ = "";
        public static string dtTo_ = "";
        public static DataTable pdt_;
        public static string Auth_ = "";
        public string city_select = "";
        //Specific User City

        public void OnGet()
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
                    Response.Redirect("/Sessionexpire", true);
                }

                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "BankEmail/";
                LoadCitySpecificUser();
                //LoadBranchSpecificUser();
                LoadBankDomains();
                LoadReports();
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                Response.Redirect("/NotAllowed", true);
            }
        }

        public List<City> LoadCitySpecificUser()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);
                    User req = new User()
                    {
                        userID = userid
                    };
                    //HTTP post
                    var responseTask = client.PostAsJsonAsync<User>("GetUserCities", req);
                    if (responseTask.Result.IsSuccessStatusCode)
                    {

                        var readTask = responseTask.Result.Content.ReadAsAsync<CityResponse>();
                        if (readTask.Result.responseCode == "00" && readTask.Result.responseMessage == "Success")
                        {
                            city = readTask.Result.cities;
                        }
                        return city;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch(Exception ex) {
                LogWriter.WriteToLog(ex);
                return city;
            }
        }
        public List<Branch> LoadBranchSpecificUser()
        {
            
            string city_ = "0";

            if(city_select != null && city_select != "")
            {
                city_ = city_select;
            }
            else
            {
                city_ = "0";
            }

            try
            {
                
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);
                    BranchRequest req = new BranchRequest()
                    {
                        instID = bankcode,
                        cityID = city_
                    };
                    //HTTP Post
                    var responseTask = client.PostAsJsonAsync<BranchRequest>("GetCityBankBranch", req);
                    if (responseTask.Result.IsSuccessStatusCode)
                    {

                        var readTask = responseTask.Result.Content.ReadAsAsync<BranchResponse>();
                        if (readTask.Result.responseCode == "00" && readTask.Result.responseMessage == "Success")
                        {
                            Branch = readTask.Result.branches;
                        }
                        return Branch;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return Branch;
            }
        }

        public List<Domain> LoadBankDomains()
        {
            
            try
            {
                
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);
                    DomainRequest req = new DomainRequest()
                    {
                        bankCode = bankcode
                    };
                    //HTTP Post
                    var responseTask = client.PostAsJsonAsync<DomainRequest>("GetBankDomain", req);
                    if (responseTask.Result.IsSuccessStatusCode)
                    {

                        var readTask = responseTask.Result.Content.ReadAsAsync<DomainResponse>();
                        if (readTask.Result.responseCode == "00" && readTask.Result.responseMessage == "Success")
                        {
                            Domain = readTask.Result.domains;
                        }
                        return Domain;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return Domain;
            }
        }
        public JsonResult OnPostSetBankBranchUserEmail([FromBody] RegBankUserEmailReqest registration)
        {
            try
            {
                var dt = DateTime.Now.ToString();
                registration.Updatedate = dt;
                registration.IsUpdate = "1";
                registration.BankCode = HttpContext.Session.GetString(loginModel.SessionKeyName10);
                 registration.UserID = HttpContext.Session.GetString(loginModel.SessionKeyName1);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);
                    //HTTP Post
                    var responseTask = client.PutAsJsonAsync<RegBankUserEmailReqest>("SetBankBranchUserEmail", registration);
                    responseTask.Wait();
                    if (responseTask.Result.IsSuccessStatusCode)
                    {

                        var readTask = responseTask.Result.Content.ReadAsAsync<RegBankUserEmailResponse>();
                        if (readTask.Result.responseCode == "00" && readTask.Result.responseCode == "Success")
                        {
                            return new JsonResult("Record inserted successfully");
                        }
                        //return readTask.Result;
                        return new JsonResult("Record not inserted");

                    }
                    else
                    {
                        return new JsonResult("Record not inserted");
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return new JsonResult("Record not inserted");
            }
        }
        public List<BankUserEmail> LoadReports()
        {
            
            DataTable oDt = new DataTable();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiURL);
                BankUserEmailRequest req = new BankUserEmailRequest()
                {
                    bankCode = bankcode,
                    branchCode = branchcode
                };
                //HTTP Post
                var responseTask = client.PostAsJsonAsync<BankUserEmailRequest>("GetBankUserEmailReport", req);
                responseTask.Wait();
                if (responseTask.Result.IsSuccessStatusCode)
                {

                    var readTask = responseTask.Result.Content.ReadAsAsync<BankUserEmailResponse>();
                    if (readTask.Result.responseCode == "00" && readTask.Result.responseMessage == "Success")
                    {
                        BankUserDetails = readTask.Result.bankUserDetail;
                    }
                    if (BankUserDetails != null)
                    {
                        if (BankUserDetails.Count > 0)
                        {
                            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(BankUserEmail));

                            foreach (PropertyDescriptor p in props)
                                oDt.Columns.Add(p.Name, p.PropertyType);
                            foreach (var c in BankUserDetails)
                                oDt.Rows.Add(c.regID, c.userID, c.userPWD, c.userName1, c.email1, c.domain1, c.mobile1, c.phone1, c.userName2,
                                    c.email2, c.domain2, c.mobile2, c.phone2, c.bankCode, c.branchCode, c.cityID, c.updatedate, c.isUpdate
                                    , c.cityName);

                            pdt_ = oDt;
                        }
                    }
                    return BankUserDetails;
                }
                else
                {
                    return null;
                }
            }
        }

        public JsonResult OnPostUpdBankBranchUserEmail([FromBody] UpdBankUserEmailReqest request)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiURL);
                //HTTP Post
                var responseTask = client.PutAsJsonAsync<UpdBankUserEmailReqest>("UpdBankBranchUserEmail", request);
                responseTask.Wait();
                if (responseTask.Result.IsSuccessStatusCode)
                {
                    var readTask = responseTask.Result.Content.ReadAsAsync<RegBankUserEmailResponse>();
                    if (readTask.Result.responseCode == "00" && readTask.Result.responseMessage == "Success")
                    {
                        return new JsonResult("Record edited successfully");
                    }
                    //return readTask.Result;
                    return new JsonResult("Record not edited");

                }
                else
                {
                    return new JsonResult("Record not edited");
                }
            }
        }
        public FileResult OnGetDownloadData()
        {
            string filename = "Bankemail.xlsx";
            try
            {
                StringBuilder sb = null;
                DataTable dt = new DataTable();
                GVtoCSV odt = new GVtoCSV();


                if (pdt_.Rows.Count > 0)
                {
                    dt = pdt_;
                    if (dt.Rows.Count > 0)
                    {
                        DataTable dt1 = new DataTable();
                        dt1 = dt.DefaultView.ToTable(false, new string[] { "BankCode", "BranchCode", "CityID", "CityName",
                    "UserName1", "Email1", "Domain1", "Mobile1","UserName2", "Email2", "Domain2", "Mobile2"  });
                        dt1.Columns["BankCode"].ColumnName = "Bank_Code";
                        dt1.Columns["BranchCode"].ColumnName = "Branch_Code";
                        dt1.Columns["CityID"].ColumnName = "City_Code";
                        dt1.Columns["CityName"].ColumnName = "City_Name";
                        dt1.Columns["UserName1"].ColumnName = "User_Name1";
                        dt1.Columns["Email1"].ColumnName = "Email1";
                        dt1.Columns["Domain1"].ColumnName = "Domain1";
                        dt1.Columns["Mobile1"].ColumnName = "Mobile1";
                        dt1.Columns["UserName2"].ColumnName = "User_Name2";
                        dt1.Columns["Email2"].ColumnName = "Email2";
                        dt1.Columns["Domain2"].ColumnName = "Domain2";
                        dt1.Columns["Mobile2"].ColumnName = "Mobile2";
                        dt1.AcceptChanges();
                        sb = odt.ExportGridToCSV(dt1);
                    }
                }
                return File(sb.ToString(), "application/octet-stream", filename);


            }
            catch (Exception ex)
            {
                byte[] MyData = new byte[0];
                LogWriter.WriteToLog("Exception on Downloadfile - " + ex);
                return File(MyData, "application/octet-stream", filename);
            }
        }
        public JsonResult OnGetCity_ID(string city)
        {
            try
            {
                if (city != null && city != "")
                {
                    city_select = city;
                    return new JsonResult(LoadBranchSpecificUser());
                    
                }
                else
                {
                    city_select = "0";
                }
            }
            catch(Exception ex)
            {
                LogWriter.WriteToLog (ex);
            }
            return new JsonResult("Failed");
        }
        public JsonResult OnGetBr_ID(string br)
        {
            try
            {
                if (br != null && br != "")
                {
                    branchcode = br;
                    return new JsonResult(LoadReports());
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
            return new JsonResult("Failed");
        }
    }
}

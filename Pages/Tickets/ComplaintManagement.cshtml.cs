using IBCS_Core_Web_Portal.Helper;
using IBCS_Web_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using NIBC.Models;
using System.Data;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static IBCS_Core_Web_Portal.Pages.Cycleinfo.BankEmailModel;

namespace IBCS_Core_Web_Portal.Pages.Tickets
{
    public class ComplaintManagementModel : PageModel
    {
        public static string userid = "";
        public static string username_ = "";
        public static string fullname_ = "";
        public static string EmailAddress = "";
        public static string userlogid = "";
        public static string bankcode = "";
        public static string branchcode = "";
        public static string dtFrom_ = "";
        public static string dtTo_ = "";
        public static string Auth_ = "";
        public List<TicketData> Alltickets { get; set; }
        public List<CPUTicketData> cPUTicketData { get; set; }

        public SearchTicket Status { get; set; }
        string cityofinci = "";

        public static string DateFrom, DateTo, status;
        public List<City2> city { get; set; }
        public List<BankCode> bankCode { get; set; }
        public Branches branches { get; set; }
        public List<ClearingDate> clearingDates { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Complainee_Bankcode { get; set; }
        public string Complainee_Branchcode { get; set; }
        public string dtto_ { get; set; }
        public string dtfrom_ { get; set; }
        public string tick_status { get; set; }
        public UserBranch userBranch { get; set; }
        public UserBank userBank { get; set; }
        public static string DDL_City = "Please Select", DDL_Complaint = "Please Select", DDL_Date = "Please Select", DDL_Bankcode = "Please Select", DDL_Branch = "Please Select", DDL_Cycle = "Please Select", MobNumber_, chqnumber, amount, compdetails;
        public static string TckNo;
        public static string Ticketstatus;
        public static string comment;
        public ViewTicketResponse viewTicket { get; set; }
        public List<ViewTicket> ticketList { get; set; }
        public List<ViewComment>? comments { get; set; }
        public const string SessionKeyNamedt1 = "_dtTo";
        public const string SessionKeyNamedt2 = "_dtFrom";
        public const string SessionKeyNamest = "_status";
        
        public void OnGet()
        {
            try
            {
                userid = HttpContext.Session.GetString(loginModel.SessionKeyName1);
                username_ = HttpContext.Session.GetString(loginModel.SessionKeyName4);
                fullname_ = HttpContext.Session.GetString(loginModel.SessionKeyName5);
                userlogid = HttpContext.Session.GetString(loginModel.SessionKeyName9);
                bankcode = HttpContext.Session.GetString(loginModel.SessionKeyName10);
                branchcode = HttpContext.Session.GetString(loginModel.SessionKeyName11);
                EmailAddress = HttpContext.Session.GetString(loginModel.SessionKeyName12);
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

                UserName = username_;
                Email = EmailAddress;
                Complainee_Bankcode = getBank();
                Complainee_Branchcode = getBranch();
                OnGetData();
                DDL_city();
                editbank();
                
                if (TckNo != null)
                {
                    Viewticket();

                }
            }
            catch(Exception ex)
            {
                LogWriter.WriteToLog(ex);
                Response.Redirect("/NotAllowed",true);
            }

        }
        public JsonResult OnGetNewTicket()
        {
            string res, msg;
            try
            {

                if (MobNumber_ != "" && MobNumber_ != null)
                {
                    Regex mob = new Regex("^[0-9]*$");
                    bool chk1 = mob.IsMatch(MobNumber_);
                    if (chk1 == false)
                    {
                        msg = "Mobile number is not valid";
                        return new JsonResult("MFailed");
                    }
                }
                Regex regdetail = new Regex("^[ A-Za-z0-9'`()]*$");
                Regex num = new Regex("^[0-9]*$");

                ///////cheqno
                if (chqnumber != "" && chqnumber != null)
                {
                    bool chk1 = num.IsMatch(chqnumber);
                    if (chk1 == false)
                    {
                        msg = "Cheque number is not valid";
                        return new JsonResult("CFailed");
                    }
                }

                ///////Amount
                if (amount != "" && amount != null)
                {
                    bool chk1 = num.IsMatch(amount);
                    if (chk1 == false)
                    {
                        msg = "Amount is not valid";
                        return new JsonResult("AFailed");
                    }
                }

                ///////details
                if (compdetails != "" && compdetails != null)
                {
                    bool chk1 = regdetail.IsMatch(compdetails);
                    if (chk1 == false)
                    {
                        msg = "Detail is not valid";
                        return new JsonResult("DFailed");
                    }
                }


                using (HttpClient client = new HttpClient())
                {
                    string URL = getLocalHost() + "Complaintform/PostNewTicket";
                    var Data = new NewTicket()
                    {
                        Name = fullname_,
                        Mobnumber = MobNumber_,
                        Email = EmailAddress,
                        Complainee_Bankcode = bankcode,//getBank().Substring(0, 3),
                        Complainee_Branchcode = branchcode,//getBranch().Substring(0, 4),
                        Complaint = DDL_Complaint,
                        DDL_Cycle = DDL_Cycle,
                        City = DDL_City,
                        CompDetails = compdetails,
                        date1 = DateTime.Now,
                        Date = DDL_Date,
                        BankCode = DDL_Bankcode.Substring(0, 3),
                        BranchCode = DDL_Branch.Substring(0, 4),
                        Chequenumber = chqnumber,
                        Amount = amount,
                        date2 = DateTime.Now
                    };
                    var json = JsonConvert.SerializeObject(Data);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = client.PostAsync(URL, content);
                    response.Wait();
                    var result = response.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        //res = "New Ticket Added";
                        return new JsonResult("Success");
                    }
                    else
                    {
                        //res = "Unable to add ticket! Please try again";
                        return new JsonResult("exfailed");
                    }

                }

            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in NewTicket: " + ex);
                //res = "Error Adding New Ticket";
                return new JsonResult("exfailed");
            }
        }

        public string getBank()
        {
            try
            {
                string Bk = bankcode;
                using (HttpClient client = new HttpClient())
                {
                    string Url = $"{getLocalHost()}Complaintform/GetBankName?Bk={Bk}";
                    HttpResponseMessage response = client.GetAsync(Url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string content = response.Content.ReadAsStringAsync().Result;
                        userBank = JsonConvert.DeserializeObject<UserBank>(content);
                    }
                }
                return userBank.instname;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in getBank: " + ex);
                return "No Bank Found";
            }
        }

        public string getBranch()
        {
            try
            {
                string Bk = bankcode;
                string Br = branchcode;
                using (HttpClient client = new HttpClient())
                {
                    string Url = $"{getLocalHost()}Complaintform/GetBranchName?Bk={Bk}&Br={Br}";
                    HttpResponseMessage response = client.GetAsync(Url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string content = response.Content.ReadAsStringAsync().Result;
                        userBranch = JsonConvert.DeserializeObject<UserBranch>(content);
                    }
                }
                return userBranch.Branchname;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in getBranch: " + ex);
                return "No Branch Found";
            }
        }

        public void DDL_city()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string Url = getLocalHost() + "Ticketform/Get_City";
                    HttpResponseMessage response = client.GetAsync(Url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string content = response.Content.ReadAsStringAsync().Result;
                        city = JsonConvert.DeserializeObject<List<City2>>(content);
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in DDL_city: " + ex);
            }
        }

        public void editbank()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string Url = getLocalHost() + "TicketForm/GetBankCode";
                    HttpResponseMessage response = client.GetAsync(Url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string content = response.Content.ReadAsStringAsync().Result;
                        bankCode = JsonConvert.DeserializeObject<List<BankCode>>(content);
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in editbank: " + ex);
            }
        }
        public class ClearingDate
        {
            public string date { get; set; }
        }

        public string getLocalHost()
        {
            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var localhost = MyConfig.GetValue<string>("AppSettings:ibcs.api");
            return localhost;
        }
        public void OnGetone()
        {
            OnGetData();

        }
        //show all
        public void OnPostButton5_Click()
        {
            try
            {
                HttpContext.Session.Remove(ComplaintManagementModel.SessionKeyNamedt1);
                HttpContext.Session.Remove(ComplaintManagementModel.SessionKeyNamedt2);
                HttpContext.Session.Remove(ComplaintManagementModel.SessionKeyNamest);
                string session = bankcode;

                if (session == "999")
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string Url = getLocalHost() + "TicketForm/GetTicketData";
                        HttpResponseMessage response = client.GetAsync(Url).Result;
                        //var result = response.Result;
                        if (response.IsSuccessStatusCode)
                        {
                            string content = response.Content.ReadAsStringAsync().Result;
                            Alltickets = JsonConvert.DeserializeObject<List<TicketData>>(content);
                        }
                    }
                }
                else
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string url = $"{getLocalHost()}TicketForm/GetCPUTicketData?myid={userid}";
                        HttpResponseMessage response = client.GetAsync(url).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            string content = response.Content.ReadAsStringAsync().Result;
                            cPUTicketData = JsonConvert.DeserializeObject<List<CPUTicketData>>(content);
                            DataTable dtCity = new DataTable();
                            dtCity = ConvertListToDataTable(cPUTicketData);
                            if (dtCity.Rows.Count != 0)
                            {

                                int cou = 0;
                                string[] cityofinc = new string[ConvertListToDataSet(cPUTicketData).Tables[0].Rows.Count];
                                DataSet sDS = ConvertListToDataSet(cPUTicketData);
                                foreach (DataRow DR in sDS.Tables[0].Rows)
                                {
                                    cityofinc[cou++] = DR["cityid"].ToString();
                                }
                                if (dtCity.Rows.Count == 1)
                                {
                                    cityofinci = " (" + cityofinc[0] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 2)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 3)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 4)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 5)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 6)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 7)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 8)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 9)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 10)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 11)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 12)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 13)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 14)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 15)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 16)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 17)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 18)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 19)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 20)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 21)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ", " + cityofinc[20] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 22)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ", " + cityofinc[20] + ", " + cityofinc[21] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 23)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ", " + cityofinc[20] + ", " + cityofinc[21] + ", " + cityofinc[22] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 24)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ", " + cityofinc[20] + ", " + cityofinc[21] + ", " + cityofinc[22] + ", " + cityofinc[23] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 25)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ", " + cityofinc[20] + ", " + cityofinc[21] + ", " + cityofinc[22] + ", " + cityofinc[23] + ", " + cityofinc[24] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 26)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ", " + cityofinc[20] + ", " + cityofinc[21] + ", " + cityofinc[22] + ", " + cityofinc[23] + ", " + cityofinc[24] + ", " + cityofinc[25] + ")";
                                    GetQuery(cityofinci);
                                }
                                else
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ", " + cityofinc[20] + ", " + cityofinc[21] + ", " + cityofinc[22] + ", " + cityofinc[23] + ", " + cityofinc[24] + ", " + cityofinc[25] + ", " + cityofinc[26] + ")";
                                    GetQuery(cityofinci);
                                    //using (HttpClient cl = new HttpClient())
                                    //{
                                    //    string Url = $"{getLocalHost()}TicketForm/GetQuery_moreRows?bankcode={26887/*Session["BankCode"].ToString()*/}";
                                    //    HttpResponseMessage res = cl.GetAsync(Url).Result;

                                    //    if (res.IsSuccessStatusCode)
                                    //    {

                                    //        string cont = res.Content.ReadAsStringAsync().Result;
                                    //        Alltickets = JsonConvert.DeserializeObject<List<TicketData>>(cont);

                                    //    }

                                    //}
                                }
                            }
                            else
                            {
                                using (HttpClient cl = new HttpClient())
                                {
                                    string Url = $"{getLocalHost()}TicketForm/GetQuery_zeroRows?bankcode={bankcode}&branchcode={branchcode}";
                                    HttpResponseMessage res = cl.GetAsync(Url).Result;

                                    if (res.IsSuccessStatusCode)
                                    {

                                        string cont = res.Content.ReadAsStringAsync().Result;
                                        Alltickets = JsonConvert.DeserializeObject<List<TicketData>>(cont);

                                    }

                                }
                            }
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in OnPostButton5_Click: " + ex);
            }
            //return Page();
        }


        public void OnGetData()
        {
            String session = bankcode;
            try
            {
                if (session == "999")
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string Url = getLocalHost() + "TicketForm/GetTicketData";
                        HttpResponseMessage response = client.GetAsync(Url).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            string content = response.Content.ReadAsStringAsync().Result;
                            Alltickets = JsonConvert.DeserializeObject<List<TicketData>>(content);
                        }
                    }
                }
                ///////FOR CPU USERS/////////
                else
                {
                    ////////////////////////////////

                    string apiUrl = $"{getLocalHost()}TicketForm/GetCPUTicketData?myid={userid}";


                    using (HttpClient client = new HttpClient())
                    {

                        HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            string content = response.Content.ReadAsStringAsync().Result;
                            cPUTicketData = JsonConvert.DeserializeObject<List<CPUTicketData>>(content);
                            DataTable dtCity = new DataTable();
                            dtCity = ConvertListToDataTable(cPUTicketData);


                            //string[] cityofinc = new string[dtCity.Rows.Count];
                            if (dtCity.Rows.Count != 0)
                            {

                                int cou = 0;
                                string[] cityofinc = new string[dtCity.Rows.Count];
                                DataTable sDT = dtCity;
                                foreach (DataRow DR in sDT.Rows)
                                {
                                    cityofinc[cou++] = DR["cityid"].ToString();
                                }

                                if (dtCity.Rows.Count == 1)
                                {
                                    //cityofinc = new string[dtCity.Rows.Count];
                                    cityofinci = " (" + cityofinc[0] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 2)
                                {

                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 3)
                                {

                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 4)
                                {

                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 5)
                                {

                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 6)
                                {

                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 7)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 8)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 9)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 10)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 11)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 12)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 13)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 14)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 15)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 16)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 17)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 18)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 19)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 20)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 21)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ", " + cityofinc[20] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 22)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ", " + cityofinc[20] + ", " + cityofinc[21] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 23)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ", " + cityofinc[20] + ", " + cityofinc[21] + ", " + cityofinc[22] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 24)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ", " + cityofinc[20] + ", " + cityofinc[21] + ", " + cityofinc[22] + ", " + cityofinc[23] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 25)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ", " + cityofinc[20] + ", " + cityofinc[21] + ", " + cityofinc[22] + ", " + cityofinc[23] + ", " + cityofinc[24] + ")";
                                    GetQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 26)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ", " + cityofinc[20] + ", " + cityofinc[21] + ", " + cityofinc[22] + ", " + cityofinc[23] + ", " + cityofinc[24] + ", " + cityofinc[25] + ")";
                                    GetQuery(cityofinci);
                                }
                                else
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ", " + cityofinc[20] + ", " + cityofinc[21] + ", " + cityofinc[22] + ", " + cityofinc[23] + ", " + cityofinc[24] + ", " + cityofinc[25] + ", " + cityofinc[26] + ")";
                                    GetQuery(cityofinci);
                                    //using (HttpClient cl = new HttpClient())
                                    //{
                                    //    string apiUri = $"{getLocalHost()}TicketForm/GetTicketDataAllRows?bankcode={bankcode}";
                                    //    HttpResponseMessage res = client.GetAsync(apiUrl).Result;
                                    //    if (res.IsSuccessStatusCode)
                                    //    {
                                    //        string result = res.Content.ReadAsStringAsync().Result;
                                    //        Alltickets = JsonConvert.DeserializeObject<List<TicketData>>(result);
                                    //    }
                                    //}
                                }
                            }

                        }
                        ///////FOR ONE BANK///////
                        else
                        {
                            using (HttpClient cl = new HttpClient())
                            {

                                string apiUri = $"{getLocalHost()}TicketForm/GetTicketDataOneBank?bankcode={bankcode}&branchcode={branchcode}";
                                HttpResponseMessage res = client.GetAsync(apiUrl).Result;
                                if (res.IsSuccessStatusCode)
                                {
                                    string result = res.Content.ReadAsStringAsync().Result;
                                    Alltickets = JsonConvert.DeserializeObject<List<TicketData>>(result);

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in OnGetData: " + ex);
            }
        }

        ////NEW TICKET BUTTON WORKING
        public void OnGetButton3_Click()
        {
            try
            {
                string prevPage = Request.Headers["Referrer"].ToString();
                Response.Redirect("NewComplain?prev=" + prevPage);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in OnGetButton3_Click: " + ex);
            }
        }

        public static DataTable ConvertListToDataTable(List<CPUTicketData> list)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("cityid", typeof(int));
            dt.Columns.Add("cityname", typeof(string));
            foreach (var item in list)
            {
                dt.Rows.Add(item.CityId, item.Cityname);
            }
            return dt;
        }

        public static DataSet ConvertListToDataSet(List<CPUTicketData> list)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("cityid", typeof(int));
            dt.Columns.Add("cityname", typeof(string));
            foreach (var item in list)
            {
                dt.Rows.Add(item.CityId, item.Cityname);
            }
            ds.Tables.Add(dt);
            return ds;
        }

        public void GetQuery(string cityofinc)
        {

            try
            {
                string bankcode_ = bankcode;

                using (HttpClient client = new HttpClient())
                {
                    string Url = $"{getLocalHost()}TicketForm/GetQuery?bankcode={bankcode_}&cityofinc={cityofinc}";
                    HttpResponseMessage response = client.GetAsync(Url).Result;

                    if (response.IsSuccessStatusCode)
                    {

                        string content = response.Content.ReadAsStringAsync().Result;
                        Alltickets = JsonConvert.DeserializeObject<List<TicketData>>(content);
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in GetQuery: " + ex);
            }
        }

        //Search Ticket
        public void OnPostSearch_Ticket()
        {
            try
            {
                String session = bankcode;
                if (session == "999")
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string Url = $"{getLocalHost()}TicketForm/GetTicketData_Nift?DtFrom={DateFrom}&DtTo={DateTo}&status={status}";
                        HttpResponseMessage response = client.GetAsync(Url).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            string content = response.Content.ReadAsStringAsync().Result;
                            Alltickets = JsonConvert.DeserializeObject<List<TicketData>>(content);
                        }
                    }
                }
                else
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string url = $"{getLocalHost()}TicketForm/GetCPUTicketData?myid={userid}";
                        HttpResponseMessage response = client.GetAsync(url).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            string content = response.Content.ReadAsStringAsync().Result;
                            cPUTicketData = JsonConvert.DeserializeObject<List<CPUTicketData>>(content);
                            DataTable dtCity = new DataTable();
                            dtCity = ConvertListToDataTable(cPUTicketData);
                            if (dtCity.Rows.Count != 0)
                            {

                                int cou = 0;
                                string[] cityofinc = new string[ConvertListToDataSet(cPUTicketData).Tables[0].Rows.Count];
                                DataSet sDS = ConvertListToDataSet(cPUTicketData);
                                foreach (DataRow DR in sDS.Tables[0].Rows)
                                {
                                    cityofinc[cou++] = DR["cityid"].ToString();
                                }


                                if (dtCity.Rows.Count == 1)
                                {
                                    cityofinci = " (" + cityofinc[0] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 2)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 3)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 4)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 5)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 6)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 7)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 8)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 9)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 10)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 11)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 12)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 13)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 14)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 15)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 16)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 17)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 18)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 19)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 20)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 21)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ", " + cityofinc[20] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 22)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ", " + cityofinc[20] + ", " + cityofinc[21] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 23)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ", " + cityofinc[20] + ", " + cityofinc[21] + ", " + cityofinc[22] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 24)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ", " + cityofinc[20] + ", " + cityofinc[21] + ", " + cityofinc[22] + ", " + cityofinc[23] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 25)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ", " + cityofinc[20] + ", " + cityofinc[21] + ", " + cityofinc[22] + ", " + cityofinc[23] + ", " + cityofinc[24] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 26)
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ", " + cityofinc[20] + ", " + cityofinc[21] + ", " + cityofinc[22] + ", " + cityofinc[23] + ", " + cityofinc[24] + ", " + cityofinc[25] + ")";
                                    SearchQuery(cityofinci);
                                }
                                else
                                {
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ", " + cityofinc[20] + ", " + cityofinc[21] + ", " + cityofinc[22] + ", " + cityofinc[23] + ", " + cityofinc[24] + ", " + cityofinc[25] + ", " + cityofinc[26] + ")";
                                    SearchQuery(cityofinci);
                                    //using (HttpClient cl = new HttpClient())
                                    //{
                                    //    string Url = $"{getLocalHost()}TicketForm/GetTicketData_moreRows?DtFrom={DateFrom}&DtTo={DateTo}&status={status}&bankcode={bankcode}";
                                    //    HttpResponseMessage rps = cl.GetAsync(Url).Result;

                                    //    if (rps.IsSuccessStatusCode)
                                    //    {
                                    //        string cnt = rps.Content.ReadAsStringAsync().Result;
                                    //        Alltickets = JsonConvert.DeserializeObject<List<TicketData>>(cnt);
                                    //    }
                                    //}
                                }
                            }
                            else
                            {
                                using (HttpClient cl = new HttpClient())
                                {
                                    string Url = $"{getLocalHost()}TicketForm/GetTicketData_NoRows?DtFrom={DateFrom}&DtTo={DateTo}&status={status}&bankcode={bankcode}";
                                    HttpResponseMessage rps = cl.GetAsync(Url).Result;

                                    if (rps.IsSuccessStatusCode)
                                    {

                                        string cnt = rps.Content.ReadAsStringAsync().Result;
                                        Alltickets = JsonConvert.DeserializeObject<List<TicketData>>(cnt);
                                    }
                                }
                            }
                        }
                    }
                }
                dtto_ = HttpContext.Session.GetString(ComplaintManagementModel.SessionKeyNamedt1);
                dtfrom_ = HttpContext.Session.GetString(ComplaintManagementModel.SessionKeyNamedt2);
                tick_status = HttpContext.Session.GetString(ComplaintManagementModel.SessionKeyNamest);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in OnPostSearch_Ticket: " + ex);
            }
        }

        public void SearchQuery(string cityofinc)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string Url = $"{getLocalHost()}TicketForm/GetSearchQuery?DtFrom={DateFrom}&DtTo={DateTo}&status={status}&cityofinci={cityofinc}&bankcode={bankcode}";
                    HttpResponseMessage response = client.GetAsync(Url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string content = response.Content.ReadAsStringAsync().Result;
                        Alltickets = JsonConvert.DeserializeObject<List<TicketData>>(content);
                    }
                }
            }
            catch (Exception ex)
            { 
                LogWriter.WriteToLog("Exception in SearchQuery: " + ex);
            }
        }

        public void OnGetSelectValues(SearchTicket obj1)
        {

            if (obj1 != null)
            {
                if (obj1.DtFrom == null)
                {
                    DateFrom = "";
                }
                if (obj1.DtTo == null)
                {
                    DateTo = "";
                }
                else
                {
                    
                    DateFrom = obj1.DtFrom;
                    DateTo = obj1.DtTo;
                    if (DateTo != null)
                    {
                        HttpContext.Session.SetString(ComplaintManagementModel.SessionKeyNamedt1, DateTo);
                    }
                    if (DateFrom != null)
                    {
                        HttpContext.Session.SetString(ComplaintManagementModel.SessionKeyNamedt2, DateFrom);
                    }
                }

                status = obj1.Status;
                HttpContext.Session.SetString(ComplaintManagementModel.SessionKeyNamest, status);
            }

        }

        public JsonResult OnGetSelectCity(User_tick obj1)
        {
            try
            {
                if (obj1 != null)
                {
                    DDL_City = obj1.City;
                }
                DDL_city();
                return new JsonResult(OnGetDDL_Branch());
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in OnGetSelectCity: " + ex);
                return new JsonResult("Failed");
            }
        }

        public void OnGetSelectBranch(User_tick obj1)
        {
            try
            {
                if (obj1 != null)
                {
                    DDL_Branch = obj1.BranchCode;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in OnGetSelectBranch: " + ex);
            }
        }

        public void OnGetSelectDate(User_tick obj1)
        {
            try
            {
                if (obj1 != null)
                {
                    DDL_Date = obj1.Date;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in OnGetSelectDate: " + ex);
            }
        }

        public void OnGetSelectCycle(User_tick obj1)
        {
            try
            {
                if (obj1 != null)
                {
                    DDL_Cycle = obj1.DDL_Cycle;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in OnGetSelectCycle: " + ex);
            }
        }

        public void OnGetSelectNumber(User_tick obj1)
        {
            try
            {
                if (obj1 != null)
                {
                    MobNumber_ = obj1.Mobnumber;
                }
            }
            catch
            {

            }
        }

        public void OnGetSelectChq(User_tick obj1)
        {
            try
            {
                if (obj1 != null)
                {
                    chqnumber = obj1.Chequenumber;
                }
            }
            catch
            {

            }
        }

        public void OnGetSelectAmount(User_tick obj1)
        {
            try
            {
                if (obj1 != null)
                {
                    amount = obj1.Amount;
                }
            }
            catch
            {

            }
        }

        public void OnGetSelectComplaint(User_tick obj1)
        {
            try
            {
                if (obj1 != null)
                {
                    DDL_Complaint = obj1.Complaint;
                }
            }
            catch { }

        }

        public void OnGetSelectCompDetails(User_tick obj1)
        {
            try
            {
                if (obj1 != null)
                {
                    compdetails = obj1.CompDetails;
                }
            }
            catch
            {

            }
        }

        public JsonResult OnGetSelectBankcode(User_tick obj1)
        {
            try
            {
                if (obj1 != null)
                {
                    DDL_Bankcode = obj1.BankCode;
                }
                editbank();
                return new JsonResult(OnGetDDL_Branch());
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in OnGetSelectCycle: " + ex);
                return new JsonResult("Failed");
            }
        }
        public JsonResult OnGetDDL_Branch()
        {
            int bank, city;
            List<Brname> brname_ = new List<Brname>();
            try
            {
                if (DDL_Bankcode == "Please Select" && DDL_City == "Please Select")
                {
                    if (branches != null)
                    {
                        branches.Branchname.Clear();
                    }
                }
                else if (DDL_Bankcode == "Please Select" && DDL_City != "Please Select")
                {
                    city = int.Parse(DDL_City.Substring(0, 3));
                    using (HttpClient client = new HttpClient())
                    {
                        string Url = $"{getLocalHost()}Complaintform/GetDDLBranchNames_Fromcity?city={city}&bankcode={bankcode}&branchcode={branchcode}";
                        HttpResponseMessage response = client.GetAsync(Url).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            string content = response.Content.ReadAsStringAsync().Result;
                            branches = JsonConvert.DeserializeObject<Branches>(content);
                            foreach (var item in branches.Branchname)
                            {
                                brname_.Add(item);
                            }
                        }
                    }
                }
                else if (DDL_Bankcode != "Please Select" && DDL_City == "Please Select")
                {
                    bank = int.Parse(DDL_Bankcode.Substring(0, 3));
                    using (HttpClient client = new HttpClient())
                    {
                        string Url = $"{getLocalHost()}Complaintform/GetDDLBranchNames_Frombank?bank={bank}&bankcode={bankcode}&branchcode={branchcode}";
                        HttpResponseMessage response = client.GetAsync(Url).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            string content = response.Content.ReadAsStringAsync().Result;
                            branches = JsonConvert.DeserializeObject<Branches>(content);
                            foreach (var item in branches.Branchname)
                            {
                                brname_.Add(item);
                            }
                        }
                    }
                }
                else
                {
                    city = int.Parse(DDL_City.Substring(0, 3));
                    bank = int.Parse(DDL_Bankcode.Substring(0, 3));
                    using (HttpClient client = new HttpClient())
                    {
                        string Url = $"{getLocalHost()}Complaintform/GetDDLBranchNames?city={city}&bank={bank}&bankcode={bankcode}&branchcode={branchcode}";
                        HttpResponseMessage response = client.GetAsync(Url).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            string content = response.Content.ReadAsStringAsync().Result;
                            branches = JsonConvert.DeserializeObject<Branches>(content);

                            foreach (var item in branches.Branchname)
                            {
                                brname_.Add(item);
                            }

                        }
                    }
                }
                return new JsonResult(brname_);

            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in OnGetDDL_Branch: " + ex);
                return new JsonResult(branches);
            }
        }
        //GetTicketNo 
        public JsonResult OnGetTicketNo(Ticket obj1)
        {
            try
            {
                if (obj1 != null)
                {
                    TckNo = obj1.TicketNo;
                }

                return new JsonResult(Viewticket());
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in OnGetTicketNo: " + ex);
                return new JsonResult("Failed");
            }
        }

        public JsonResult OnGetDisplayComments()
        {
            try
            {
                return new JsonResult(ShowComments());
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in OnGetTicketNo: " + ex);
                return new JsonResult("Failed");
            }
        }

        public JsonResult Viewticket()
        {
            try
            {

                int tck = int.Parse(TckNo);
                using (HttpClient client = new HttpClient())
                {
                    string Url = $"{getLocalHost()}ViewTicket/GetTicket?tck={tck}";
                    HttpResponseMessage response = client.GetAsync(Url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string content = response.Content.ReadAsStringAsync().Result;
                        viewTicket = JsonConvert.DeserializeObject<ViewTicketResponse>(content);
                        if (viewTicket.ResponseCode == "00")
                        {
                            ticketList = viewTicket.Ticket;
                            GetStatus(ticketList);
                        }
                        else
                        {
                            ticketList = null;
                        }
                    }
                }
                return new JsonResult(ticketList);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in Viewticket: " + ex);
                return new JsonResult(ticketList);
                //LogWriter.WriteToLog(ex);
                //ticketList.Clear();
            }
        }

        public void GetStatus(List<ViewTicket> tk)
        {
            foreach (ViewTicket ticket in tk)
            {
                Ticketstatus = ticket.status;
            }
        }


        //PostComment
        public JsonResult OnGetPostComment()
        {
            string commentStatus = "";
            try
            {
                if (Ticketstatus != "CLOSED")
                {
                    using (HttpClient client = new HttpClient())
                    {

                        string URL = getLocalHost() + "ViewTicket/PostComment";
                        var comt = new Comment()
                        {
                            Name = fullname_,
                            CommentDetails = comment,
                            Bankname = bankcode,
                            Ticket = TckNo,
                            Dated = DateTime.Now


                        };
                        var json = JsonConvert.SerializeObject(comt);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = client.PostAsync(URL, content);
                        response.Wait();
                        var result = response.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            commentStatus = "Comment Successfully Post";

                        }
                        else
                        {
                            commentStatus = "Error Posting Comment";

                        }
                    }

                    /////Changing status of Message/ALERT
                    using (HttpClient client = new HttpClient())
                    {

                        string URL = getLocalHost() + "ViewTicket/Updatestatus";
                        var comt = new StatusComment()
                        {
                            Ticket = TckNo,
                            Bankcode = bankcode,
                            Dated = DateTime.Now
                        };
                        var json = JsonConvert.SerializeObject(comt);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = client.PutAsync(URL, content);
                        response.Wait();
                        var result = response.Result;
                        if (result.IsSuccessStatusCode)
                        {


                        }
                        else
                        {


                        }
                    }

                }
                return new JsonResult(commentStatus);
            }

            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in OnGetPostComment: " + ex);
                commentStatus = "Error Posting Comment";
                return new JsonResult(commentStatus);
            }
        }

        public JsonResult ShowComments()
        {
            try
            {

                using (HttpClient client = new HttpClient())
                {
                    string Url = $"{getLocalHost()}ViewTicket/GetComments?tk={TckNo}";
                    HttpResponseMessage response = client.GetAsync(Url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string content = response.Content.ReadAsStringAsync().Result;
                        comments = JsonConvert.DeserializeObject<List<ViewComment>>(content);
                    }
                }
                return new JsonResult(comments);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in ShowComments: " + ex);
                return new JsonResult(comments);
                //LogWriter.WriteToLog(ex);
                //ticketList.Clear();
            }
        }

        //GetComment
        public JsonResult OnGetComment(Ticket obj1)
        {
            try
            {
                if (obj1 != null)
                {
                    comment = obj1.comment;


                }

                Viewticket();
                return new JsonResult(Ticketstatus);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in ShowComments: " + ex);
                return new JsonResult("Failed");
            }
        }




        //Close Ticket
        public JsonResult OnGetCloseTicket()
        {
            string TckStatus = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string Url = $"{getLocalHost()}ViewTicket/CloseTicket?tk={TckNo}";
                    HttpResponseMessage response = client.GetAsync(Url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        TckStatus = "Ticket Closed Successfully";


                    }
                    else
                    {
                        TckStatus = "Error Ticket Not Closed";
                    }
                }
                return new JsonResult(TckStatus);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in OnGetCloseTicket: " + ex);
                TckStatus = "Error Ticket Not Closed";
                return new JsonResult(TckStatus);
            }
        }

    }
}

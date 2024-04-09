using IBCS_Web_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using NIBC.Models;
using System.Data;
using System.Text.RegularExpressions;
using System.Text;
using IBCS_Core_Web_Portal.Models;
using System.Xml.Linq;
using IBCS_Core_Web_Portal.Helper;

namespace IBCS_Core_Web_Portal.Pages.Tickets
{
    public class OtherRequestModel : PageModel
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
        public string dtto_ { get; set; }
        public string dtfrom_ { get; set; }
        public string tick_status { get; set; }
        public List<RequestData> AllRequests { get; set; }
        public List<CPUTicketData> cPUTicketData { get; set; }


        string cityofinci = "";

        public static string DateFrom, DateTo, status;
        public static List<City2> city { get; set; }
        public List<BankCode> bankCode { get; set; }

        public static string UserName { get; set; }
        public string Email { get; set; }
        public static string Complainee_Bankcode { get; set; }
        public static string Complainee_Branchcode { get; set; }
        public UserBranch userBranch { get; set; }
        public UserBank userBank { get; set; }
        public static string DDL_City = "Please Select", DDL_Complaint = "Please Select", compdetails;
        public static string DDL_RequestType = "Please Select";
        public static int RequestNo = 0;
        public static string Ticketstatus;
        public const string SessionKeyNamedt1 = "_dtTo";
        public const string SessionKeyNamedt2 = "_dtFrom";
        public const string SessionKeyNamest = "_status";
        public ViewRequestResponse viewRequest { get; set; }

        public List<ViewRequest> requestList { get; set; }


        public void OnGet()
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

            UserName = username_;
            Email = EmailAddress;
            Complainee_Bankcode = getBank();
            Complainee_Branchcode = getBranch();
            OnGetData();
            DDL_city();
            editbank();
            if (RequestNo != 0)
            {
                Viewrequest();
            }
        }
        public JsonResult OnGetNewRequest()
        {
            string res, msg;
            try
            {

                Regex regdetail = new Regex("^[ A-Za-z0-9'`()]*$");
                Regex num = new Regex("^[0-9]*$");

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
                    string URL = getLocalHost() + "OtherRequest/PostNewRequest";
                    var Data = new Request()
                    {
                        dateopened = DateTime.Now,
                        city = DDL_City,
                        reqtype = DDL_RequestType,
                        reqbkcode = getBank().Substring(0, 3),
                        reqbrcode = getBranch().Substring(0, 4),
                        name = fullname_,
                        details = compdetails,
                        email = EmailAddress
                    };
                    var json = JsonConvert.SerializeObject(Data);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = client.PostAsync(URL, content);
                    response.Wait();
                    var result = response.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return new JsonResult("Success");
                    }
                    else
                    {
                        return new JsonResult("exfailed");
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in NewRequest: " + ex);
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
                HttpContext.Session.Remove(OtherRequestModel.SessionKeyNamedt1);
                HttpContext.Session.Remove(OtherRequestModel.SessionKeyNamedt2);
                HttpContext.Session.Remove(OtherRequestModel.SessionKeyNamest);
                string session = bankcode;

                if (session == "999")
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string Url = getLocalHost() + "OtherRequest/GetRequestData";
                        HttpResponseMessage response = client.GetAsync(Url).Result;
                        //var result = response.Result;
                        if (response.IsSuccessStatusCode)
                        {
                            string content = response.Content.ReadAsStringAsync().Result;
                            AllRequests = JsonConvert.DeserializeObject<List<RequestData>>(content);
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
                                    //    string Url = $"{getLocalHost()}OtherRequest/GetQuery_moreRows?bankcode={bankcode/*Session["BankCode"].ToString()*/}";
                                    //    HttpResponseMessage res = cl.GetAsync(Url).Result;

                                    //    if (res.IsSuccessStatusCode)
                                    //    {

                                    //        string cont = res.Content.ReadAsStringAsync().Result;
                                    //        AllRequests = JsonConvert.DeserializeObject<List<RequestData>>(cont);

                                    //    }
                                    //}
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
                        string Url = getLocalHost() + "OtherRequest/GetRequestData";
                        HttpResponseMessage response = client.GetAsync(Url).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            string content = response.Content.ReadAsStringAsync().Result;
                            AllRequests = JsonConvert.DeserializeObject<List<RequestData>>(content);
                        }
                    }
                }
                ///////FOR CPU USERS/////////
                else
                {
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
                                    cityofinci = " (" + cityofinc[0] + ", " + cityofinc[1] + ", " + cityofinc[2] + ", " + cityofinc[3] + ", " + cityofinc[4] + ", " + cityofinc[5] + ", " + cityofinc[6] + ", " + cityofinc[7] + ", " + cityofinc[8] + ", " + cityofinc[9] + ", " + cityofinc[10] + ", " + cityofinc[11] + ", " + cityofinc[12] + ", " + cityofinc[13] + ", " + cityofinc[14] + ", " + cityofinc[15] + ", " + cityofinc[16] + ", " + cityofinc[17] + ", " + cityofinc[18] + ", " + cityofinc[19] + ", " + cityofinc[20] + ", " + cityofinc[21] + ", " + cityofinc[22] + ", " + cityofinc[23] + ", " + cityofinc[24] + ", " + cityofinc[25] + ", " + cityofinc[26] +")";
                                    GetQuery(cityofinci);
                                    //using (HttpClient cl = new HttpClient())
                                    //{
                                    //    string apiUri = $"{getLocalHost()}OtherRequest/GetRequestDataAllRows?bankcode={bankcode}";
                                    //    HttpResponseMessage res = client.GetAsync(apiUrl).Result;
                                    //    if (res.IsSuccessStatusCode)
                                    //    {
                                    //        string result = res.Content.ReadAsStringAsync().Result;
                                    //        AllRequests = JsonConvert.DeserializeObject<List<RequestData>>(result);
                                    //    }
                                    //}
                                }
                            }
                        }
                    }
                }
            }
            // }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in OnGetData: " + ex);
            }
            //return Page();
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
                    string Url = $"{getLocalHost()}OtherRequest/GetQuery?bankcode={bankcode_}&cityofinc={cityofinc}";
                    HttpResponseMessage response = client.GetAsync(Url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string content = response.Content.ReadAsStringAsync().Result;
                        AllRequests = JsonConvert.DeserializeObject<List<RequestData>>(content);
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in GetQuery: " + ex);
            }
        }

        //Search Request
        public void OnPostSearch_Ticket()
        {
            try
            {
                String session = bankcode;
                if (session == "999")
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string Url = $"{getLocalHost()}OtherRequest/GetRequestData_Nift?DtFrom={DateFrom}&DtTo={DateTo}&status={status}";
                        HttpResponseMessage response = client.GetAsync(Url).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            string content = response.Content.ReadAsStringAsync().Result;
                            AllRequests = JsonConvert.DeserializeObject<List<RequestData>>(content);

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
                                    cityofinci = " ('" + cityofinc[0] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 2)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 3)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 4)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 5)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "', '" + cityofinc[4] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 6)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "', '" + cityofinc[4] + "', '" + cityofinc[5] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 7)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "', '" + cityofinc[4] + "', '" + cityofinc[5] + "', '" + cityofinc[6] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 8)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "', '" + cityofinc[4] + "', '" + cityofinc[5] + "', '" + cityofinc[6] + "', '" + cityofinc[7] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 9)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "', '" + cityofinc[4] + "', '" + cityofinc[5] + "', '" + cityofinc[6] + "', '" + cityofinc[7] + "', '" + cityofinc[8] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 10)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "', '" + cityofinc[4] + "', '" + cityofinc[5] + "', '" + cityofinc[6] + "', '" + cityofinc[7] + "', '" + cityofinc[8] + "', '" + cityofinc[9] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 11)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "', '" + cityofinc[4] + "', '" + cityofinc[5] + "', '" + cityofinc[6] + "', '" + cityofinc[7] + "', '" + cityofinc[8] + "', '" + cityofinc[9] + "', '" + cityofinc[10] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 12)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "', '" + cityofinc[4] + "', '" + cityofinc[5] + "', '" + cityofinc[6] + "', '" + cityofinc[7] + "', '" + cityofinc[8] + "', '" + cityofinc[9] + "', '" + cityofinc[10] + "', '" + cityofinc[11] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 13)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "', '" + cityofinc[4] + "', '" + cityofinc[5] + "', '" + cityofinc[6] + "', '" + cityofinc[7] + "', '" + cityofinc[8] + "', '" + cityofinc[9] + "', '" + cityofinc[10] + "', '" + cityofinc[11] + "', '" + cityofinc[12] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 14)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "', '" + cityofinc[4] + "', '" + cityofinc[5] + "', '" + cityofinc[6] + "', '" + cityofinc[7] + "', '" + cityofinc[8] + "', '" + cityofinc[9] + "', '" + cityofinc[10] + "', '" + cityofinc[11] + "', '" + cityofinc[12] + "', '" + cityofinc[13] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 15)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "', '" + cityofinc[4] + "', '" + cityofinc[5] + "', '" + cityofinc[6] + "', '" + cityofinc[7] + "', '" + cityofinc[8] + "', '" + cityofinc[9] + "', '" + cityofinc[10] + "', '" + cityofinc[11] + "', '" + cityofinc[12] + "', '" + cityofinc[13] + "', '" + cityofinc[14] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 16)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "', '" + cityofinc[4] + "', '" + cityofinc[5] + "', '" + cityofinc[6] + "', '" + cityofinc[7] + "', '" + cityofinc[8] + "', '" + cityofinc[9] + "', '" + cityofinc[10] + "', '" + cityofinc[11] + "', '" + cityofinc[12] + "', '" + cityofinc[13] + "', '" + cityofinc[14] + "', '" + cityofinc[15] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 17)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "', '" + cityofinc[4] + "', '" + cityofinc[5] + "', '" + cityofinc[6] + "', '" + cityofinc[7] + "', '" + cityofinc[8] + "', '" + cityofinc[9] + "', '" + cityofinc[10] + "', '" + cityofinc[11] + "', '" + cityofinc[12] + "', '" + cityofinc[13] + "', '" + cityofinc[14] + "', '" + cityofinc[15] + "', '" + cityofinc[16] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 18)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "', '" + cityofinc[4] + "', '" + cityofinc[5] + "', '" + cityofinc[6] + "', '" + cityofinc[7] + "', '" + cityofinc[8] + "', '" + cityofinc[9] + "', '" + cityofinc[10] + "', '" + cityofinc[11] + "', '" + cityofinc[12] + "', '" + cityofinc[13] + "', '" + cityofinc[14] + "', '" + cityofinc[15] + "', '" + cityofinc[16] + "', '" + cityofinc[17] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 19)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "', '" + cityofinc[4] + "', '" + cityofinc[5] + "', '" + cityofinc[6] + "', '" + cityofinc[7] + "', '" + cityofinc[8] + "', '" + cityofinc[9] + "', '" + cityofinc[10] + "', '" + cityofinc[11] + "', '" + cityofinc[12] + "', '" + cityofinc[13] + "', '" + cityofinc[14] + "', '" + cityofinc[15] + "', '" + cityofinc[16] + "', '" + cityofinc[17] + "', '" + cityofinc[18] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 20)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "', '" + cityofinc[4] + "', '" + cityofinc[5] + "', '" + cityofinc[6] + "', '" + cityofinc[7] + "', '" + cityofinc[8] + "', '" + cityofinc[9] + "', '" + cityofinc[10] + "', '" + cityofinc[11] + "', '" + cityofinc[12] + "', '" + cityofinc[13] + "', '" + cityofinc[14] + "', '" + cityofinc[15] + "', '" + cityofinc[16] + "', '" + cityofinc[17] + "', '" + cityofinc[18] + "', '" + cityofinc[19] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 21)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "', '" + cityofinc[4] + "', '" + cityofinc[5] + "', '" + cityofinc[6] + "', '" + cityofinc[7] + "', '" + cityofinc[8] + "', '" + cityofinc[9] + "', '" + cityofinc[10] + "', '" + cityofinc[11] + "', '" + cityofinc[12] + "', '" + cityofinc[13] + "', '" + cityofinc[14] + "', '" + cityofinc[15] + "', '" + cityofinc[16] + "', '" + cityofinc[17] + "', '" + cityofinc[18] + "', '" + cityofinc[19] + "', '" + cityofinc[20] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 22)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "', '" + cityofinc[4] + "', '" + cityofinc[5] + "', '" + cityofinc[6] + "', '" + cityofinc[7] + "', '" + cityofinc[8] + "', '" + cityofinc[9] + "', '" + cityofinc[10] + "', '" + cityofinc[11] + "', '" + cityofinc[12] + "', '" + cityofinc[13] + "', '" + cityofinc[14] + "', '" + cityofinc[15] + "', '" + cityofinc[16] + "', '" + cityofinc[17] + "', '" + cityofinc[18] + "', '" + cityofinc[19] + "', '" + cityofinc[20] + "', '" + cityofinc[21] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 23)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "', '" + cityofinc[4] + "', '" + cityofinc[5] + "', '" + cityofinc[6] + "', '" + cityofinc[7] + "', '" + cityofinc[8] + "', '" + cityofinc[9] + "', '" + cityofinc[10] + "', '" + cityofinc[11] + "', '" + cityofinc[12] + "', '" + cityofinc[13] + "', '" + cityofinc[14] + "', '" + cityofinc[15] + "', '" + cityofinc[16] + "', '" + cityofinc[17] + "', '" + cityofinc[18] + "', '" + cityofinc[19] + "', '" + cityofinc[20] + "', '" + cityofinc[21] + "', '" + cityofinc[22] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 24)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "', '" + cityofinc[4] + "', '" + cityofinc[5] + "', '" + cityofinc[6] + "', '" + cityofinc[7] + "', '" + cityofinc[8] + "', '" + cityofinc[9] + "', '" + cityofinc[10] + "', '" + cityofinc[11] + "', '" + cityofinc[12] + "', '" + cityofinc[13] + "', '" + cityofinc[14] + "', '" + cityofinc[15] + "', '" + cityofinc[16] + "', '" + cityofinc[17] + "', '" + cityofinc[18] + "', '" + cityofinc[19] + "', '" + cityofinc[20] + "', '" + cityofinc[21] + "', '" + cityofinc[22] + "', '" + cityofinc[23] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 25)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "', '" + cityofinc[4] + "', '" + cityofinc[5] + "', '" + cityofinc[6] + "', '" + cityofinc[7] + "', '" + cityofinc[8] + "', '" + cityofinc[9] + "', '" + cityofinc[10] + "', '" + cityofinc[11] + "', '" + cityofinc[12] + "', '" + cityofinc[13] + "', '" + cityofinc[14] + "', '" + cityofinc[15] + "', '" + cityofinc[16] + "', '" + cityofinc[17] + "', '" + cityofinc[18] + "', '" + cityofinc[19] + "', '" + cityofinc[20] + "', '" + cityofinc[21] + "', '" + cityofinc[22] + "', '" + cityofinc[23] + "', '" + cityofinc[24] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else if (dtCity.Rows.Count == 26)
                                {
                                    cityofinci = " ('" + cityofinc[0] + "', '" + cityofinc[1] + "', '" + cityofinc[2] + "', '" + cityofinc[3] + "', '" + cityofinc[4] + "', '" + cityofinc[5] + "', '" + cityofinc[6] + "', '" + cityofinc[7] + "', '" + cityofinc[8] + "', '" + cityofinc[9] + "', '" + cityofinc[10] + "', '" + cityofinc[11] + "', '" + cityofinc[12] + "', '" + cityofinc[13] + "', '" + cityofinc[14] + "', '" + cityofinc[15] + "', '" + cityofinc[16] + "', '" + cityofinc[17] + "', '" + cityofinc[18] + "', '" + cityofinc[19] + "', '" + cityofinc[20] + "', '" + cityofinc[21] + "', '" + cityofinc[22] + "', '" + cityofinc[23] + "', '" + cityofinc[24] + "', '" + cityofinc[25] + "')";
                                    SearchQuery(cityofinci);
                                }
                                else
                                {
                                    using (HttpClient cl = new HttpClient())
                                    {
                                        string Url = $"{getLocalHost()}OtherRequest/GetRequestData_moreRows?DtFrom={DateFrom}&DtTo={DateTo}&status={status}&bankcode={bankcode}";
                                        HttpResponseMessage rps = cl.GetAsync(Url).Result;

                                        if (rps.IsSuccessStatusCode)
                                        {
                                            string cnt = rps.Content.ReadAsStringAsync().Result;
                                            AllRequests = JsonConvert.DeserializeObject<List<RequestData>>(cnt);

                                        }
                                    }
                                }
                            }
                            else
                            {
                                using (HttpClient cl = new HttpClient())
                                {
                                    string Url = $"{getLocalHost()}OtherRequest/GetSearchQuery_brbk?DtFrom={DateFrom}&DtTo={DateTo}&status={status}&branchcode={branchcode}&bankcode={bankcode}";
                                    HttpResponseMessage rps = cl.GetAsync(Url).Result;

                                    if (rps.IsSuccessStatusCode)
                                    {
                                        string cnt = rps.Content.ReadAsStringAsync().Result;
                                        AllRequests = JsonConvert.DeserializeObject<List<RequestData>>(cnt);

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
                    string Url = $"{getLocalHost()}OtherRequest/GetSearchQuery?DtFrom={DateFrom}&DtTo={DateTo}&status={status}&cityofinci={cityofinc}&bankcode={bankcode}";
                    HttpResponseMessage response = client.GetAsync(Url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string content = response.Content.ReadAsStringAsync().Result;
                        AllRequests = JsonConvert.DeserializeObject<List<RequestData>>(content);
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
                    DateFrom = null;
                }
                if (obj1.DtTo == null)
                {
                    DateTo = null;
                }
                else
                {
                    DateFrom = obj1.DtFrom;
                    DateTo = obj1.DtTo;
                    if (DateTo != null)
                    {
                        HttpContext.Session.SetString(OtherRequestModel.SessionKeyNamedt1, DateTo);
                    }
                    if (DateFrom != null)
                    {
                        HttpContext.Session.SetString(OtherRequestModel.SessionKeyNamedt2, DateFrom);
                    }
                }
                status = obj1.Status;
                HttpContext.Session.SetString(OtherRequestModel.SessionKeyNamest, status);
            }
        }

        public void OnGetSelectCity(User_tick obj1)
        {
            if (obj1 != null)
            {
                DDL_City = obj1.City;
            }
            DDL_city();
        }

        public void OnGetSelectType(Request obj1)
        {
            if (obj1 != null)
            {
                DDL_RequestType = obj1.reqtype;
            }
        }

        //public void OnGetSelectBranch(User_tick obj1)
        //{
        //    if (obj1 != null)
        //    {
        //        DDL_Branch = obj1.BranchCode;
        //    }
        //}

        //public void OnGetSelectDate(User_tick obj1)
        //{
        //    if (obj1 != null)
        //    {
        //        DDL_Date = obj1.Date;

        //    }
        //}

        //public void OnGetSelectCycle(User_tick obj1)
        //{
        //    if (obj1 != null)
        //    {
        //        DDL_Cycle = obj1.DDL_Cycle;
        //    }
        //}

        public void OnGetSelectComplaint(User_tick obj1)
        {
            if (obj1 != null)
            {
                DDL_Complaint = obj1.Complaint;
            }
        }

        public void OnGetSelectCompDetails(User_tick obj1)
        {
            if (obj1 != null)
            {
                compdetails = obj1.CompDetails;
            }
        }

        //GetTicketNo 
        public JsonResult OnGetRequestNo(Requestinfo obj1)
        {
            if (obj1 != null)
            {
                RequestNo = obj1.RequestNo;
            }
            return new JsonResult(Viewrequest());
        }

        public JsonResult Viewrequest()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string Url = $"{getLocalHost()}OtherRequest/GetRequest?req={RequestNo}";
                    HttpResponseMessage response = client.GetAsync(Url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string content = response.Content.ReadAsStringAsync().Result;
                        viewRequest = JsonConvert.DeserializeObject<ViewRequestResponse>(content);
                        if (viewRequest.ResponseCode == "00")
                        {
                            requestList = viewRequest.ViewRequests;
                            GetStatus(requestList);
                        }
                        else
                        {
                            requestList = null;
                        }
                    }
                }
                return new JsonResult(requestList);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in Viewrequest: " + ex);
                return new JsonResult(requestList);
                //LogWriter.WriteToLog(ex);
                //ticketList.Clear();
            }
        }

        public void GetStatus(List<ViewRequest> rq)
        {
            foreach (ViewRequest request in rq)
            {
                Ticketstatus = request.status;
            }
        }

        //Close Request
        public JsonResult OnGetCloseRequest()
        {
            string ReqStatus = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string Url = $"{getLocalHost()}OtherRequest/CloseRequest?req={RequestNo}";
                    HttpResponseMessage response = client.GetAsync(Url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ReqStatus = "Request Closed Successfully";
                    }
                    else
                    {
                        ReqStatus = "Error Request Not Closed";
                    }
                }
                return new JsonResult(ReqStatus);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception in OnGetCloseRequest: " + ex);
                ReqStatus = "Error Request Not Closed";
                return new JsonResult(ReqStatus);
            }
        }

    }
}

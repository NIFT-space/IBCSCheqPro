using IBCS_Core_Web_Portal.Helper;
using IBCS_Web_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace IBCS_Core_Web_Portal.Pages
{
    public class ForgetPasswordModel : PageModel
    {
        public static string apiURL = "";
        public const string SessionKeyName_uid = "us_id";
        public const string SessionKeyName_cnic = "cnic_";
        public const string SessionKeyName_dob = "dob_";

        public const string SessionKeyName_ques = "ques_";
        public const string SessionKeyNameans = "ans_";
        public void OnGet()
        {
            HttpContext.Session.SetString(SessionKeyName_uid, "");
            HttpContext.Session.SetString(SessionKeyName_cnic, "");
            HttpContext.Session.SetString(SessionKeyName_dob, "");
            HttpContext.Session.SetString(SessionKeyName_ques, "");
            HttpContext.Session.SetString(SessionKeyNameans, "");
            try
            {
                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "GetUserDetails/";
            }
            catch
            {

            }
        }
        public JsonResult OnGetSenddetails(string userId, string cnic_no, string dob, string question, string answer)
        {
            try
            {
                Regex regdetail = new Regex("^[A-Za-z0-9.@]*$");
                bool chk1 = regdetail.IsMatch(userId);
                if (chk1 == false)
                {
                    LogWriter.WriteToLog("Invalid userId :  - " + userId + "-");
                    return new JsonResult("Failed");
                }
                Regex regques = new Regex("^[A-Za-z0-9]*$");
                bool chk2 = regques.IsMatch(question);
                if (chk2 == false)
                {
                    LogWriter.WriteToLog("Invalid question :  - " + question + "-");
                    return new JsonResult("Failed");
                }
                bool chk5 = regques.IsMatch(answer);
                if (chk5 == false)
                {
                    LogWriter.WriteToLog("Invalid answer :  - " + answer + "-");
                    return new JsonResult("Failed");
                }
                Regex regcnic = new Regex("^[0-9]{0,15}$");
                bool chk3 = regcnic.IsMatch(cnic_no);
                if (chk3 == false)
                {
                    LogWriter.WriteToLog("Invalid cnic_no :  - " + cnic_no + "-");
                    return new JsonResult("Failed");
                }
                Regex regdob = new Regex("^\\d{1,2}/\\d{1,2}/\\d{4}$");
                bool chk4 = regdob.IsMatch(dob);
                if (chk4 == false)
                {
                    LogWriter.WriteToLog("Invalid dob :  - " + dob + "-");
                    return new JsonResult("Failed");
                }

                FirstTime_UserDetail usrdetail = new FirstTime_UserDetail()
                {
                    userId = userId,
                    cnic = cnic_no,
                    dob = dob,
                    ques = question,
                    ans = answer,
                    pass1 = "",
                    pass2 = ""
                };

                bool getchek = Send_UserDetails(usrdetail);
                if (getchek == true)
                {
                    HttpContext.Session.SetString(SessionKeyName_uid, userId);
                    HttpContext.Session.SetString(SessionKeyName_cnic, cnic_no);
                    HttpContext.Session.SetString(SessionKeyName_dob, dob);
                    HttpContext.Session.SetString(SessionKeyName_ques, question);
                    HttpContext.Session.SetString(SessionKeyNameans, answer);
                    return new JsonResult("Success");
                }
                else
                {
                    return new JsonResult("Failed");
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception on OnPostSenddetails - " + ex);
                return new JsonResult("Failed");
            }
        }
        private bool Send_UserDetails(FirstTime_UserDetail frtus)
        {
            try
            {
                bool resp = false;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);

                    var postTask = client.PostAsJsonAsync<FirstTime_UserDetail>("PassCheck_User", frtus);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<FirstTime_UserResponse>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                            resp = readTask.Result.FT_Login;
                    }
                }
                return resp;
            }
            catch (Exception ex)
            {
                
                LogWriter.WriteToLog("Exception on Send_UserDetails - " + ex);
                return false;
            }
        }
    }
}

using IBCS_Core_Web_Portal.Helper;
using IBCS_Web_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace IBCS_Core_Web_Portal.Pages
{
    public class NewPasswordModel : PageModel
    {
        public static string apiURL = "";
        public static string userid_;
        public static string cnic_;
        public static string dob_;
        public static string ques_;
        public static string ans_;
        public void OnGet()
        {
            try
            {
                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "GetUserDetails/";
            }
            catch (Exception ex)
            {

            }
        }
        public JsonResult OnGetSenddetails(string password, string password2)
        {
            try
            {
                userid_ = HttpContext.Session.GetString(FirstTimeLoginModel.SessionKeyName_uid);
                cnic_ = HttpContext.Session.GetString(FirstTimeLoginModel.SessionKeyName_cnic);
                dob_ = HttpContext.Session.GetString(FirstTimeLoginModel.SessionKeyName_dob);
                ques_ = HttpContext.Session.GetString(FirstTimeLoginModel.SessionKeyName_ques);
                ans_ = HttpContext.Session.GetString(FirstTimeLoginModel.SessionKeyNameans);

                HashIt ht = new HashIt();

                string HASH_pwd = ht.GetHash(password);

                if ((HASH_pwd != null && HASH_pwd != "") && (password2 != null && password2 != "") && (userid_ != null && userid_ != "") && (cnic_ != null && cnic_ != "")
                    && (dob_ != null && dob_ != "") && (ques_ != null && ques_ != "") && (ans_ != null && ans_ != ""))
                {
                    FirstTime_UserDetail frtus_ = new FirstTime_UserDetail
                    {
                        userId = userid_,
                        cnic = cnic_,
                        dob = dob_,
                        ques = ques_,
                        ans = ans_,
                        pass1 = HASH_pwd,
                        pass2 = HASH_pwd
                    };
                    bool res = Send_UserDetails(frtus_);
                    if (res)
                    {
                        return new JsonResult("Success");
                    }
                    else
                    {
                        return new JsonResult("Failed");
                    }
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

                    var postTask = client.PostAsJsonAsync<FirstTime_UserDetail>("Passupd_User", frtus);
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
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}

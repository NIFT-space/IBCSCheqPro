using IBCS_Core_Web_Portal.Helper;
using IBCS_Web_Portal.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace IBCS_Core_Web_Portal.Pages
{
    public class LogOffModel : PageModel
    {
        public static string apiURL = "";
        public void OnGet()
        {
            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "GetUserDetails/";
            UserLog();

            HttpContext.Session.Clear();
            HttpContext.Session.Remove(loginModel.SessionKeyName1);
            HttpContext.Session.Remove(loginModel.SessionKeyName2);
            HttpContext.Session.Remove(loginModel.SessionKeyName3);
            HttpContext.Session.Remove(loginModel.SessionKeyName4);
            HttpContext.Session.Remove(loginModel.SessionKeyName5);
            HttpContext.Session.Remove(loginModel.SessionKeyName6);
            HttpContext.Session.Remove(loginModel.SessionKeyName7);
            HttpContext.Session.Remove(loginModel.SessionKeyName8);
            HttpContext.Session.Remove(loginModel.SessionKeyName9);
            HttpContext.Session.Remove(loginModel.SessionKeyName10);
            HttpContext.Session.Remove(loginModel.SessionKeyName11);
            HttpContext.Session.Remove(loginModel.SessionKeyName12);
            HttpContext.Session.Remove(loginModel.SessionKeyName13);
            HttpContext.Session.Remove(loginModel.SessionKeyName14);
            HttpContext.Session.Remove(loginModel.SessionKeyName15);
            HttpContext.Session.Remove(loginModel.SessionKeyName16);

            HttpContext.Request.Headers.Remove("Ser_CER");
			HttpContext.Request.Headers.Remove("Subj_DN");

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddMonths(-20),
            };
            Response.Cookies.Append(string.Empty, string.Empty, cookieOptions);

            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
        }
        public void UserLog()
        {
            try
            {
                string userlogid = HttpContext.Session.GetString(loginModel.SessionKeyName17);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);
                    StringContent req = new StringContent(userlogid, Encoding.UTF8, "application/json");

                    var postTask = client.PostAsync("TimeoutUserLog", req);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        ///OK
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
    }
}

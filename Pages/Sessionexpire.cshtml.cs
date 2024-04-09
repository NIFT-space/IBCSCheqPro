using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace IBCS_Core_Web_Portal.Pages
{
    public class SessionexpireModel : PageModel
    {
        public void OnGet()
        {
            LogOffModel lm = new LogOffModel();
            lm.UserLog();
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
    }
}

using IBCS_Web_Portal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Text.Json;
using Nancy.Json;
using System.Linq.Expressions;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel;
using System.Data;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using IBCS_Core_Web_Portal.Helper;
using Nancy.Extensions;
using System.Runtime.ConstrainedExecution;
using Microsoft.Extensions.Primitives;
using System.Collections;
using Nancy.Session;
using System.Net.Mail;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using IBCS_Core_Web_Portal.Pages.Reports;
using Nancy;
using System.Net.Http;
using System.Text;
using System.Net.Http.Json;

namespace IBCS_Core_Web_Portal.Pages
{
    public class loginModel : PageModel
    {
        #region Variables
        public static string _createdcaptcha, imageSrc;
        public static string apiURL = "" , IsLocal = "";
        Int64 UserLogID = -1;
        static string resp_UserName = "";
        static string resp_BankName = "";
        static String dong_BankCode = "", dong_BranchCode = "", dong_EmailAddress = "", dong_SerialNumber = "";
        bool LoggedIn = false;
        static int t_ = 0;

        List<UserDetailviaLogin> udv_ = new List<UserDetailviaLogin>();
        UserDetailviaLogin udv = new UserDetailviaLogin();

        public class Cred
        {
            public string userid { get; set; }
            public string password { get; set; }
        }
        ///////Sessions////////
        public const string SessionKeyName1 = "_myID";
        public const string SessionKeyName2 = "CertExpire";
        public const string SessionKeyName3 = "CaptchaCode";

        public const string SessionKeyName4 = "username";
        public const string SessionKeyName5 = "fullname";
        public const string SessionKeyName6 = "BankName";
        public const string SessionKeyName7 = "UserRoleDesc";
        public const string SessionKeyName8 = "lastin";
        public const string SessionKeyName9 = "UserLogID";
        public const string SessionKeyName10 = "BankCode";
        public const string SessionKeyName11 = "BranchCode";
        public const string SessionKeyName12 = "EmailAddress";
        public const string SessionKeyName13 = "SerialNo";
        public const string SessionKeyName14 = "ClientInfo";
        public const string SessionKeyName15 = "CertExpire";
        public const string SessionKeyName16 = "cacheKey";
        public const string SessionKeyName17 = "userloggid";
        public const string Isbranchuser = "_isbruser";
        public const string SessionCount = "_count";
        public const string SessionFile = "_file";

        private StringValues Serial_value;
        private StringValues Subject_value;
		private StringValues Cert_expiry;
		/////////////////////

		#endregion

		private readonly ILogger<loginModel> _logger;
        public loginModel(ILogger<loginModel> logger)
        {
            _logger = logger;
        }
        public void OnGet()
        {
            try
            {
                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "GetUserDetails/";
                IsLocal = MyConfig.GetValue<string>("AppSettings:environment");
				string userid = HttpContext.Session.GetString(loginModel.SessionKeyName1);

                if (userid != null)
                {
                    Response.Redirect("/IBCSHome");
                }

                /////////////////////////////////////////////////
                /////////////COMMENT THIS AREA///////////////////
                ///////FOR TESTING WIHTOUT CERTIFICATE///////////
                /////////////////////////////////////////////////
                //bool T_ = GetCertificate();
                //if (T_ == false)
                //{
                //    LogWriter.WriteToLog("Failed on GetCertificate");
                //    Response.Redirect("/NotAllowed", true);
                //}


                //if (dong_SerialNumber != null && dong_SerialNumber != "")
                //{
                //    bool x_logged = GetFirstTimeLogin(dong_SerialNumber.Trim());
                //    if (x_logged)
                //    {
                //        Response.Redirect("/FirstTimeLogin");
                //    }
                //}
                /////////////////////////////////////////////////
                /////////////////////////////////////////////////
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception on OnGet - " + ex);
                Response.Redirect("/NotAllowed",true);
            }
        }
        public void OnPost()
        {
        }
        /////////////Validate Captcha on backend///////////////////
        public JsonResult OnPostPassCaptcha(string data1)
        {
            if(data1 != null)
            {
                _createdcaptcha = data1;
                return new JsonResult("Success");
            }
            return new JsonResult("Failed");
        }
        /////////////////////////////////////////////////////////
        public IActionResult OnPostPassCred(Cred cred_, string userid, string password, string capt)
        {
            
            try
            {
                string uid_ = cred_.userid;
                string pd_ = cred_.password;

                ///Validate Captcha From Server Side
                if (_createdcaptcha != capt)
                {
                    return new JsonResult("CaptF");
                }
                //////////                
                
                ///Wrong Login Attempt Validation///
                ///////
                if (HttpContext.Session.GetString(SessionFile) != null)
                {
                    if (HttpContext.Session.GetString(SessionFile) == cred_.userid)
                    {
                        if (HttpContext.Session.GetString(loginModel.SessionCount) != null)
                        {
                            t_ = Convert.ToInt32(HttpContext.Session.GetString(loginModel.SessionCount));
                        }

                        if (t_ < 5)
                        {

                        }
                        else
                        {
                            LogWriter.WriteToLog("Login Limit Reached. Account is Locked for user - " + cred_.userid);
                            bool chk_ = LockUserAccount(uid_);
                            return new JsonResult("LoginLimit");
                        }
                    }
                    else
                    {
                        HttpContext.Session.SetString(SessionFile, cred_.userid);
                        HttpContext.Session.SetString(loginModel.SessionCount, "0");
                    }
                }
                else
                {
                    HttpContext.Session.SetString(SessionFile, cred_.userid);
                    HttpContext.Session.SetString(loginModel.SessionCount, "0");
                }

                string ct = HttpContext.Session.GetString(loginModel.SessionCount);
                string ct_str = "";
                /////////////////////
                

                HashIt ht = new HashIt();
                string HASH = ht.GetHash(pd_);

                if (uid_ != "" && pd_ != "")
                {
                    Regex regdetail = new Regex("^[A-Za-z0-9.@]*$");
                    bool chk1 = regdetail.IsMatch(uid_);
                    if (chk1 == false)
                    {
                        LogWriter.WriteToLog("Invalid userid and password is null :  - " + uid_ + "-"+ pd_);
                        ct_str = Convert.ToString(Convert.ToInt32(ct) + 1);
                        ct = ct_str;
                        HttpContext.Session.SetString(SessionCount, ct_str);
                        return new JsonResult("Failed");
                    }
                }
                else
                {
                    return new JsonResult("Failed");
                }

                UserDetailPostviaLogin post_ = new UserDetailPostviaLogin();
                post_.user = uid_;
                post_.pass = HASH;

                GetUserDetails_viaDongle(post_);

                ///Account Lock Validation///
                /////////
                if (udv.IsLock == true)
                {
                    string sEnd = udv.LockTime.ToString("HH:mm");

                    string sTime = DateTime.Now.ToString("dd-MM-yyyy") + " " + DateTime.Now.ToString("HH:mm");
                    //

                    string dt_ = Convert.ToString(udv.LockTime);
                    string d1 = dt_.Substring(0, 10);
                    string[] aStartDate = d1.ToString().Split('/');
                    string d2 = aStartDate[1] + "-" + aStartDate[0] + "-" + aStartDate[2];
                    string eTime = d2 + " " + sEnd;
                    //
                    DateTime sdt = System.DateTime.Now;
                    DateTime edt = System.DateTime.Now;
                    if (DateTime.TryParseExact(sTime, "dd-MM-yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out sdt))
                    {

                    }
                    if (DateTime.TryParseExact(eTime, "dd-MM-yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out edt))
                    {

                    }
                    TimeSpan span = sdt.Subtract(edt);
                    if (span.Ticks > 3000000000)
                    {
                        ///Unlock Account Automatically
                        bool chk = UnlockUserAccount(uid_);
                    }
                    else
                    {
                        LogWriter.WriteToLog("Account is Locked for user - " + cred_.userid);

                        return new JsonResult("LoginLimit");
                    }
                }
                ///////

                if (udv.instName == "" || udv.instName == null)
                {
                    LogWriter.WriteToLog("Failed on instName :  - udv instName: " + udv.instName);
                    ct_str = Convert.ToString(Convert.ToInt32(ct) + 1);
                    ct = ct_str;
                    HttpContext.Session.SetString(SessionCount, ct_str);
                    return new JsonResult("Failed");
                }

                if (udv.userId <= 0 || udv.userId == null)
                {
                    LogWriter.WriteToLog("Failed on userid :  - udv userid: " + udv.userId);
                    ct_str = Convert.ToString(Convert.ToInt32(ct) + 1);
                    ct = ct_str;
                    HttpContext.Session.SetString(SessionCount, ct_str);
                    return new JsonResult("Failed");
                }

                if (uid_ != udv.userName)
                {
                    LogWriter.WriteToLog("Failed on userName : " + uid_ + " - udv userName: " + udv.userName);
                    ct_str = Convert.ToString(Convert.ToInt32(ct) + 1);
                    ct = ct_str;
                    HttpContext.Session.SetString(SessionCount, ct_str);
                    return new JsonResult("Failed");
                }
                if (udv.Password != HASH)
                {
                    LogWriter.WriteToLog("Failed on Hash : " + HASH + " - udv Password: " + udv.Password);
                    ct_str = Convert.ToString(Convert.ToInt32(ct) + 1);
                    ct = ct_str;
                    HttpContext.Session.SetString(SessionCount, ct_str);
                    return new JsonResult("Failed");
                }

                /////////////////////////////////////////////////
                /////////////COMMENT THIS AREA///////////////////
                ///////FOR TESTING WIHTOUT CERTIFICATE///////////
                /////////////////////////////////////////////////
                //if (dong_EmailAddress != udv.emailAddress)
                //{
                //    LogWriter.WriteToLog("Failed on Dongle Emailaddress- dong_EmailAddress" + dong_EmailAddress + " - udv emailAddress: " + udv.emailAddress);
                //    ct_str = Convert.ToString(Convert.ToInt32(ct) + 1);
                //    return new JsonResult("Failed");
                //}
                //if (dong_BankCode.Trim() != string.Format("{0:000}", Convert.ToInt32(udv.instID)))
                //{
                //    LogWriter.WriteToLog("Failed on Dongle bank Code- dong_BankCode" + dong_BankCode + " - udv instID: " + udv.instID);
                //    ct_str = Convert.ToString(Convert.ToInt32(ct) + 1);
                //    return new JsonResult("Failed");
                //}
                //if (dong_BranchCode.Trim() != string.Format("{0:0000}", Convert.ToInt32(udv.branchID)))
                //{
                //    LogWriter.WriteToLog("Failed on Dongle branch Code- dong_BranchCode" + dong_BranchCode + " - udv branchID: " + udv.branchID);
                //    ct_str = Convert.ToString(Convert.ToInt32(ct) + 1);
                //    return new JsonResult("Failed");
                //}
                //if (dong_SerialNumber.Trim() != udv.SerialNo.ToUpperInvariant())
                //{
                //    LogWriter.WriteToLog("Failed on Dongle Serial Number - dong_serialnum" + dong_SerialNumber + " - udv serial num: " + udv.SerialNo);
                //    ct_str = Convert.ToString(Convert.ToInt32(ct) + 1);
                //    return new JsonResult("Failed");
                //}
                ////////////////////////////////////////////////
                ////////////////////////////////////////////////
                //////
                ////// Prevent Simultaneous Logins by a Single User ID 
                //////
                string key = dong_EmailAddress.Trim();

                string userRole = udv.UserRole;
                if (UserLogID > 0)
                {
                    HttpContext.Session.SetString(SessionKeyName9, Convert.ToString(UserLogID));
                }
                else
                {
                    HttpContext.Session.SetString(SessionKeyName9, Convert.ToString(0));
                }

                LoggedIn = true;

                // New Code Prevent Simultaneous Logins by a Single User ID
                //if (userList.Contains(EmailAddress.Trim()))
                //{
                //    Response.Redirect("notallowed", true);
                //}
                //else
                //{
                //    userList.Add(EmailAddress.Trim());
                //    Application["Users"] = userList;
                //}

                // Session Creation For Other Pages
                ////////////////////////
                HttpContext.Session.SetString(SessionKeyName1, Convert.ToString(udv.userId));
                HttpContext.Session.SetString(SessionKeyName4, udv.userName);

                HttpContext.Session.SetString(SessionKeyName5, udv.fullName);
                HttpContext.Session.SetString(SessionKeyName6, udv.instName);
                HttpContext.Session.SetString(SessionKeyName7, userRole);
                HttpContext.Session.SetString(SessionKeyName8, udv.timeIn);

               HttpContext.Session.SetString(SessionKeyName10, udv.instID);// dong_BankCode.Trim());
               HttpContext.Session.SetString(SessionKeyName11, udv.branchID); //dong_BranchCode.Trim());
               HttpContext.Session.SetString(SessionKeyName12, udv.emailAddress);//dong_EmailAddress.Trim());
               HttpContext.Session.SetString(SessionKeyName13, udv.SerialNo);//dong_SerialNumber);
                if(udv.Isbranchuser == false)
                {
                    HttpContext.Session.SetString(Isbranchuser, "Yes");
                }
                else
                {
                    HttpContext.Session.SetString(Isbranchuser, "No");
                }


                string guid = Guid.NewGuid().ToString();
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1),
                    // MaxAge = TimeSpan.FromSeconds(600),
                    Secure = true,
                    HttpOnly = true,
                    IsEssential = true
                };
                Response.Cookies.Append("IBCS.Auth.Token", guid, cookieOptions);
                string ExpireDate = DateTime.UtcNow.AddMinutes(60).ToString("ddd, dd MMM yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                Response.Headers.Add("Expires", ExpireDate + " GMT");
                HttpContext.Session.SetString(SessionKeyName16, guid);

                LogWriter.WriteToLog("Success on PassCred Code - UserID is : "+ udv.userId );
                return new JsonResult("Success");
            }
            catch(Exception ex){
                LogWriter.WriteToLog("Exception on PassCred Code: "+ ex);
                //CreateCaptchaImage();
                return new JsonResult("Failed");
            }
        }
        public JsonResult OnGetRef_Captcha()
        {
            try
            {
                LogWriter.WriteToLog("Success on Captcha Code");
                // CreateCaptchaImage();
                return new JsonResult("Success" + "|"+ imageSrc);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Failed on Captcha" + ex);
                //CreateCaptchaImage();
                LogWriter.WriteToLog("Exception Captcha : " + ex);
                return new JsonResult("Failed");
            }
        }
        protected bool GetCertificate()
        {
            try
            {
				string Ser_CER=""; string Subj_DN=""; string cert_exp="";

				///////FOR LINUX LIVE DEPLOYMENT
                if(IsLocal == "PROD")
                {
					LogWriter.WriteToLog(" _ _ _ - HTTP HEADERS _ _ _ _  ");
					var headers = HttpContext.Request.Headers;
					var body = HttpContext.Request.Body;
					foreach (var key in headers)
                    {

                        //LogWriter.WriteToLog("Keyname : " + key.Key + " | Key Value : " + key.Value);
                        if (key.Key == "SSL_CLIENT_S_DN")
                        {
                            Subj_DN = key.Value;
                        }

                        if (key.Key == "SSL_CLIENT_CERT")
                        {
                            byte[] client_cert = Convert.FromBase64String(key.Value);
                            X509Certificate2 objcert = new X509Certificate2(client_cert);
                            Ser_CER = objcert.SerialNumber;

							cert_exp = objcert.GetExpirationDateString();

						}
                    }
                }
				///////FOR LINUX UAT DEPLOYMENT
				else
				{
					LogWriter.WriteToLog(" _ _ _ - HTTP HEADERS _ _ _ _  ");
					//HttpContext.Request.Headers.TryGetValue("Ser_CER", out var Serial_value);//Request.Headers["Ser_CER"];
					//HttpContext.Request.Headers.TryGetValue("Subj_DN", out var Subject_value);// Request.Headers["Subj_DN"];
					HttpContext.Request.Headers.TryGetValue("Cert_valid", out var Cert_expiry);
					HttpContext.Request.Headers.TryGetValue("Certif", out var Serial_value);
					HttpContext.Request.Headers.TryGetValue("Subj_DN", out var Subject_value);
					Subj_DN = Subject_value.ToString();
					Ser_CER = Serial_value.ToString();
					cert_exp = Cert_expiry.ToString();
					
				}
				
				if (cert_exp != null)
                {
					LogWriter.WriteToLog("CERT expiry - " + cert_exp);
					HttpContext.Session.SetString(SessionKeyName2, cert_exp);
                }
				
				if (Ser_CER != null)
				{
					LogWriter.WriteToLog("CERT KEY - " + Ser_CER);
					dong_SerialNumber = Ser_CER;
				}

                //Subj_DN = "C=PK,L=10,O=National Institutional Facilitation Technologies Pvt Ltd, OU=Operations, OU=Terms of use at www.niftetrust.com/rpa (c)14,OU=Branch ID - 0001,OU=Bank ID - 062,CN=Yawer Aijaz,emailAddress=yawer.aijaz@mcb.com.pk";

                if (Subj_DN != null)
				{
					LogWriter.WriteToLog("Certt Subj - " + Subj_DN);

					int strindex = 0;
					char[] delimiterChars = { '/', ',' };

					//string[] words = Subj_DN.Split(delimiterChars);
					string[] words = Subj_DN.Split(delimiterChars);
					// 0  C=PK
					// 1  L=10
					// 2  O=National Institutional Facilitation Technologies Pvt Ltd
					// 3  OU=Terms of use at www.niftetrust.com
					// 4  rpa (c)14
					// 5  OU=Department
					// 6  CN=Name
					foreach (string s in words)
					{
						strindex = s.IndexOf("Bank ID");
						if (strindex >= 0)
						{
							dong_BankCode = s.Replace("OU=Bank ID - ", "");
							strindex = 0;
						}
						strindex = s.IndexOf("Branch ID");
						if (strindex >= 0)
						{
							dong_BranchCode = s.Replace("OU=Branch ID - ", "");
							strindex = 0;
						}
						strindex = s.IndexOf("@");
						if (strindex >= 0)
						{
							dong_EmailAddress = s.Replace("emailAddress=", "");
							strindex = 0;
						}
					}
				}
				else
				{
					return false;
				}
                ////////////////////////////////////////////////////////////////////////////

				///////FOR Windows DPELOYMENT
				//X509Certificate certt = HttpContext.Connection.ClientCertificate as X509Certificate;
    //            LogWriter.WriteToLog("Certt - " + certt);
                
    //            if(certt.Subject != null)
    //            {
				//	LogWriter.WriteToLog("Certt Subj - " + certt.Subject);
					


				//	int strindex = 0;
    //                char[] delimiterChars = { '/', ',' };

				//	//string[] words = Subj_DN.Split(delimiterChars);
				//	string[] words = certt.Subject.Split(delimiterChars);
				//	// 0  C=PK
				//	// 1  L=10
				//	// 2  O=National Institutional Facilitation Technologies Pvt Ltd
				//	// 3  OU=Terms of use at www.niftetrust.com
				//	// 4  rpa (c)14
				//	// 5  OU=Department
				//	// 6  CN=Name
				//	foreach (string s in words)
    //                {
    //                    strindex = s.IndexOf("Bank ID");
    //                    if (strindex >= 0)
    //                    {
    //                        dong_BankCode = s.Replace("OU=Bank ID - ", "");
    //                        strindex = 0;
    //                    }
    //                    strindex = s.IndexOf("Branch ID");
    //                    if (strindex >= 0)
    //                    {
    //                        dong_BranchCode = s.Replace("OU=Branch ID - ", "");
    //                        strindex = 0;
    //                    }
    //                    strindex = s.IndexOf("@");
    //                    if (strindex >= 0)
    //                    {
    //                        dong_EmailAddress = s.Replace("E=", "");
    //                        strindex = 0;
    //                    }
    //                }
    //                try
    //                {
				//		dong_SerialNumber = Convert.ToString(certt.GetSerialNumberString());// certt.SerialNumber.Replace("-", "");
    //                    LogWriter.WriteToLog("serial no - " + certt.GetSerialNumberString());
    //                }
    //                catch
    //                {
				//		LogWriter.WriteToLog("serial no - " + certt.GetSerialNumberString());
				//	}
    //            }
    //            else
    //            {
    //                return false;
    //            }
                ///////////////////////////////////////////////////////////////////////////////////////


                if (dong_BankCode == "")
                {
                    LogWriter.WriteToLog("bankcode null");
                    return false;
                }
                if (dong_BranchCode == "")
                {
					LogWriter.WriteToLog("branchcode null");
					return false;
                }
                if (dong_EmailAddress == "")
                {
					LogWriter.WriteToLog("Email null");

					return false;
                }
                if (dong_SerialNumber == "")
                {
                    LogWriter.WriteToLog("Seerial null");
                    return false;
                }
                LogWriter.WriteToLog("Success on Dongle Code");
                return true;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return  false;
            }
        }
		//handler.ClientCertificates.Add(objcrt);
		//X509Certificate2 cert2 = new X509Certificate2();

		//LogWriter.WriteToLog("X-client cert: - " + client);
		//if (client != null)
		//{
		//    byte[] certdata = Convert.FromBase64String(client);

		//    cert2 = new X509Certificate2(certdata);
		//    LogWriter.WriteToLog("Success on cert get");
		//}
		//else
		//{
		//    LogWriter.WriteToLog("client cert is null");

		//}


		//var certificates = userStore.Certificates;

		//Console.WriteLine("Available Certificates:");
		//for (int i = 0; i < certificates.Count; i++)
		//{
		//    Console.WriteLine($"{i + 1}. {certificates[i].Subject}");
		//}



		//Console.Write("Enter the number of the certificate to select: ");
		////if (int.TryParse(Console.ReadLine(), out int selectedIndex) && selectedIndex > 0 && selectedIndex <= certificates.Count)
		////{
		//int.TryParse(Console.ReadLine(), out int selectedIndex);
		//    X509Certificate2 selectedCertificate2 = certificates[selectedIndex - 1];

		//    if (!selectedCertificate2.HasPrivateKey)
		//    {
		//        Console.WriteLine("Selected certificate does not have a private key.");
		//    }
		//    else
		//    {
		//        Console.WriteLine("Selected certificate has a private key.");
		//    }
		////}
		////else
		////{
		//    //Console.WriteLine("Invalid selection.");
		////}

		//userStore.Close();


		//var Userstore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
		//LogWriter.WriteToLog("UserStore : " + Userstore);
		//Userstore.Open(OpenFlags.ReadOnly);
		//LogWriter.WriteToLog("UserStore open " );
		//var selectedCertificate = X509Certificate2UI.SelectFromCollection(
		//   Userstore.Certificates,
		//   "IBCS",
		//   "Certificate-Store",
		//   X509SelectionFlag.SingleSelection);
		//LogWriter.WriteToLog("Selected Certificate : " + selectedCertificate);
		//X509Certificate2Collection userCertificate = (X509Certificate2Collection)selectedCertificate;
		//X509Certificate2 objcrt = userCertificate[0];
		////Userstore.Close();
		//if (!objcrt.HasPrivateKey)
		//{
		//    return false;
		//    //Response.Redirect("NotAllowed");
		//}

		//HttpContext.Session.SetString(SessionKeyName2, objcrt.NotAfter.ToString("dd-MM-yyyy"));
		//if (objcrt.NotAfter < DateTime.Now)
		//{
		//    //Response.Redirect("NotAllowed");
		//}
		/////////////////////////////////////////////////////////////////////////////////
		// For Production
		//LogWriter.WriteToLog("Started");



		//HttpClientCertificate objcrt = Request.ClientCertificate;
		//if (!objcrt.IsPresent)
		//{
		//}
		//if (!objcrt.IsValid)
		//{
		//}
		//if (objcrt.Issuer.IndexOf("NIFTECH CA") <= 0)
		//{
		//}
		//if ((objcrt.ServerIssuer.IndexOf("VeriSign") >= 0) || (objcrt.ServerIssuer.IndexOf("Symantec") >= 0) || (objcrt.ServerIssuer.IndexOf("DigiCert") >= 0))
		//{
		//}
		//else
		//{
		//}
		//if (objcrt.ValidUntil < DateTime.Now)
		//{
		//}
		//Session["CertExpire"] = objcrt.ValidUntil.ToString("dd-MM-yyyy");
		private List<string> GetClientInfo()
        {
            List<string> sb = new List<string>();

            string ipaddress;
            try
            {
                //ipaddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                //if (ipaddress == "" || ipaddress == null)
                //{
                //    ipaddress = Request.ServerVariables["REMOTE_ADDR"];
                //}
                //else
                //{
                //    ipaddress = Request.UserHostAddress;
                //    sb.Add(ipaddress);
                //}
                //sb.Add(ipaddress);
                return sb;
            }
            catch (Exception ex)
            {
                //LogWriter.WriteToLog(ex);
                return sb;
            }
        }
        private bool UnlockUserAccount(string uid)
        {
            try
            {
                bool res = false;
                chklockuser chklockuser = new chklockuser();
                chklockuser.res_ = uid;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);

                    var postTask = client.PostAsJsonAsync<chklockuser>("Account_UnLock", chklockuser);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<bool>();
                        readTask.Wait();
                        
                        res = readTask.Result;

                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return false;
            }
        }
        public class chklockuser
        {
            public string res_ { get; set; }
        }
        private bool LockUserAccount(string uid)
        {
            try
            {
                bool res = false;
                chklockuser chklockuser = new chklockuser();
                chklockuser.res_ = uid;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);
                    var postTask = client.PostAsJsonAsync<chklockuser>("AccountLock_Check", chklockuser);
                    //var postTask = client.PostAsync("AccountLock_Check", new StringContent(uid, Encoding.UTF8, "application/json"));
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<bool>();
                        readTask.Wait();

                        res = readTask.Result;
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return false;
            }
        }
        private bool GetFirstTimeLogin(string req)
        {
            try
            {
                bool res = false;
                int islogged = 0;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);

                    var postTask = client.GetAsync("GetIsLoggedResult?req=" + req);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<IsLoggedResult>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                            islogged = readTask.Result.isLogged;
                        if (islogged == 1)
                        {
                            res = false;
                        }
                        else
                        {
                            res = true;
                        }
                    }
                }
                return res;
            }
            catch (Exception ex)
			{
				LogWriter.WriteToLog(ex);
				return false;
            }
        }

        protected void GetUserDetails_viaDongle(UserDetailPostviaLogin req_)
        {
           string URL = "";
            try
            {
                string json = string.Empty;
                //URL = apiURL + "GetUserByDetail";

                ////ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ////ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                //var request = (HttpWebRequest)WebRequest.Create(URL);
                //request.Timeout = 10000000;
                //request.Method = "POST";
                //request.ContentType = "application/json";

                //using (var streamwriter = new StreamWriter(request.GetRequestStream()))
                //{
                //    json = new JavaScriptSerializer().Serialize(new
                //    {
                //        user = req_.user,
                //        pass = req_.pass
                //        //BankCode = req_.BankCode,
                //        //BranchCode = req_.BranchCode,
                //        //EmailAddress = req_.EmailAddress,
                //        //SerialNo = req_.SerialNo
                //    });
                //    streamwriter.Write(json);
                //}

                //var response = request.GetResponse();

                //string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                //UserDetailResponse objres_bin = JsonConvert.DeserializeObject<UserDetailResponse>(responseString);

                //foreach (var ulist in objres_bin.UdList)
                //{
                //    udv.userId = ulist.userId;
                //    udv.userName = ulist.userName;
                //    udv.Password = ulist.Password;
                //    udv.instID = ulist.instID;
                //    udv.branchID = ulist.branchID;
                //    udv.emailAddress = ulist.emailAddress;
                //    udv.SerialNo = ulist.SerialNo;
                //    udv.instName = ulist.instName;
                //    udv.UserRole = ulist.UserRole;
                //    udv.RoleName = ulist.RoleName;
                //    udv.fullName = ulist.fullName;
                //    udv.timeIn = ulist.timeIn;
                //    udv.IspwdChanged = ulist.IspwdChanged;

                //    LogWriter.WriteToLog(ulist.userId + ulist.userName + ulist.instID);
                //}


                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);
                    LogWriter.WriteToLog("apiURL - " + apiURL + "req -" + req_.user + "-" +req_.pass);

                    var postTask = client.PostAsJsonAsync<UserDetailPostviaLogin>("GetUserByDetail", req_);
                    postTask.Wait();

                    var result = postTask.Result;
                    LogWriter.WriteToLog("reason" + result.ReasonPhrase);
                    LogWriter.WriteToLog("headers" + result.TrailingHeaders);
                    LogWriter.WriteToLog("content" + result.Content);
                    LogWriter.WriteToLog("req message" + result.RequestMessage);
                    LogWriter.WriteToLog("status code" + result.IsSuccessStatusCode);
                    LogWriter.WriteToLog("header2" + result.Headers);
                    LogWriter.WriteToLog("stat code" + result.StatusCode);
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<UserDetailResponse>();
                        readTask.Wait();

                        udv_ = readTask.Result.UdList;

                        foreach (var ulist in udv_)
                        {
                            udv.userId = ulist.userId;
                            udv.userName = ulist.userName;
                            udv.Password = ulist.Password;
                            udv.instID = ulist.instID;
                            udv.branchID = ulist.branchID;
                            udv.emailAddress = ulist.emailAddress;
                            udv.SerialNo = ulist.SerialNo;
                            udv.instName = ulist.instName;
                            udv.UserRole = ulist.UserRole;
                            udv.RoleName = ulist.RoleName;
                            udv.fullName = ulist.fullName;
                            udv.timeIn = ulist.timeIn;
                            udv.IspwdChanged = ulist.IspwdChanged;
                            udv.IsLock = ulist.IsLock;
                            udv.LockTime = ulist.LockTime;
                            udv.Isbranchuser = ulist.Isbranchuser;
                            HttpContext.Session.SetString(SessionKeyName17, ulist.userlogid);

                            LogWriter.WriteToLog(ulist.userId + ulist.userName + ulist.instID);
                        }
                    }
                }

                LogWriter.WriteToLog("Success on Get User Details API");
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
	}
}

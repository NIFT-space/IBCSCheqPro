using IBCS_Core_Web_Portal.Helper;
using IBCS_Web_Portal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using static IBCS_Web_Portal.Models.PageAllowedResponse;

namespace IBCS_Core_Web_Portal.Pages
{
    public class Cycle_desc_Status
    {
        public int Normal_Clg { get; set; }
        public int Normal_Rtn { get; set; }
        public int Sameday_Clg { get; set; }
        public int Sameday_Rtn { get; set; }
        public int Intercity_Clg { get; set; }
        public int Intercity_Rtn { get; set; }
        public int Dollar_Clg { get; set; }
        public int Dollar_Rtn { get; set; }

		//////Report
		public int Normal_rep { get; set; }
		public int Normal_RtnRep { get; set; }
		public int Sameday_Rep { get; set; }
		public int Sameday_RtnRep { get; set; }
		public int Intercity_Rep { get; set; }
		public int Intercity_RtnRep { get; set; }
		public int Dollar_Rep { get; set; }
		public int Dollar_RtnRep { get; set; }

	}
    public class IBCSHomeModel : PageModel
    {
		public Cycle_desc_Status cycle_Desc_Statuses { get; set; }
        public RepCount img_count_ { get; set; }
        public RepCount rep_count_ { get; set; }
        public static string apiURL = "",apiURL_bk = "";
		public static string userid = "";
		public static string userlogid = "";
		public static string bankcode = "";
		public static string branchcode = "";
		public static string dtFrom_ = "";
		public static string dtTo_ = "";
        public static string Auth_ = "";
        DataTable dt;
        public DataTable SessionKeyName_cycshow;
		public static string apiURL_getcontrol = "";

		public static string dat1 = "", dat2 = "", dat3 = "", dat4 = "", dat5 = "";
		public void OnGet()
        {
            try
            {
                userid = HttpContext.Session.GetString(loginModel.SessionKeyName1);
                userlogid = HttpContext.Session.GetString(loginModel.SessionKeyName9);
                bankcode = HttpContext.Session.GetString(loginModel.SessionKeyName10);
                dtFrom_ = HttpContext.Session.GetString(GetProdtModel.dtFrom);
                dtTo_ = HttpContext.Session.GetString(GetProdtModel.dtTo);
                branchcode = HttpContext.Session.GetString(loginModel.SessionKeyName11);
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
                //Response.Headers.Append("Expires", System.DateTime.Now.AddDays(-1).ToString());
                Response.Headers.Append("Last-Modified", System.DateTime.Now.ToString());

                if (userid == null)
                {
                    Response.Redirect("/Sessionexpire",true);
                }
                if(userid == "")
                {
                    Response.Redirect("/Sessionexpire",true);
                }

                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "GetControlsData/";
                apiURL_bk = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "BankEmail/";
				apiURL_getcontrol = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "GetControlsData/";
			}
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception on OnGet - " + ex);
                Response.Redirect("/Sessionexpire",true);
            }
        }
        public JsonResult OnGetProvideDT(string dt_)
        {
            try
            {
				LogWriter.WriteToLog("process date DT: " + dt_);
				//dtFrom_ = HttpContext.Session.GetString(GetProdtModel.dtFrom);

                if (string.IsNullOrEmpty(dtFrom_))
                {
                    dtFrom_ = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
					LogWriter.WriteToLog("DT1: " + HttpContext.Session.GetString(GetProdtModel.DT1));
					//dtFrom_ = HttpContext.Session.GetString(GetProdtModel.DT1);
                }

                LogWriter.WriteToLog("process date: " + dtFrom_);

				Providedt();
                return new JsonResult(img_count_.norm_count + "|" + img_count_.samed_count + "|" + img_count_.int_count + "|" +img_count_.dol_count + 
                    "&" + rep_count_.norm_count + "|" + rep_count_.samed_count + "|" +rep_count_.int_count + "|" +rep_count_.dol_count);
            }
            catch(Exception ex)
            {
                return new JsonResult("failed");
            }
		}
        public RepCount Providedt()
        {
            RepCount rep1 = new RepCount();
			RepCount rep2 = new RepCount();
			try
            {
                dt = new DataTable();
				
                /////////////////////IMAGE///////////////////////////////////
				if (bankcode != null)
                {
                    if (bankcode == "999")
                    {
						rep1.norm_count = "0";
						rep1.samed_count = "0";
						rep1.int_count = "0";
						rep1.dol_count = "0";
					}
                    else
                    {
                        try
                        {
							rep1.norm_count = GetCount_Img("2","4").norm_count;
							rep1.samed_count = GetCount_Img("5","7").samed_count;
							rep1.int_count = GetCount_Img("1","3").int_count;
							rep1.dol_count = GetCount_Img("20","22").dol_count;
						}
                        catch
                        {
							rep1.norm_count = "0";
							rep1.samed_count = "0";
							rep1.int_count = "0";
							rep1.dol_count = "0";
						}
					}
					img_count_ = rep1;
				}
                ///////////////////////REPORT/////////////////////////////////
                ///
                if (bankcode != null)
                {
                    if (bankcode == "999")
                    {
						rep2.norm_count = "0";
                        rep2.samed_count = "0";
						rep2.int_count = "0";
						rep2.dol_count = "0";
					}
                    else
                    {
                        try
                        {
							rep2.norm_count = GetCount_Rep("2", "4").norm_count;
							rep2.samed_count = GetCount_Rep("5", "7").samed_count;
							rep2.int_count = GetCount_Rep("1", "3").int_count;
							rep2.dol_count = GetCount_Rep("20", "22").dol_count;
						}
                        catch
                        {
							rep2.norm_count = "0";
							rep2.samed_count = "0";
							rep2.int_count = "0";
							rep2.dol_count = "0";
						}
                        
					}
					rep_count_ = rep2;
				}
                
                return rep_count_;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
				rep2.norm_count = "0";
				rep2.samed_count = "0";
				rep2.int_count = "0";
				rep2.dol_count = "0";
				rep_count_ = rep2;
				return rep_count_;
            }
        }
        public RepCount GetCount_Img(string cycno, string cycno2)
        {
			RepCount rc2 = new RepCount();
			List<RepCities> rc1 = new List<RepCities>();
            string bruser = HttpContext.Session.GetString(loginModel.Isbranchuser);
            try
            {
                int chk = 0;
				DataTable oDt = new DataTable();
				
				if (dtFrom_ == null)
                {
					dtFrom_ = "1990-11-11";
                }
                if (bankcode == null)
                {
					bankcode = "0";
                }
                if (branchcode == null)
                {
					branchcode = "0";
                }

                //foreach (DataRow city in GetCity().Rows)
                //{
                //List<string> sParam = new List<string>();
                //string citi = city["instid"].ToString();
                //sParam.Add("@pdate");
                //               sParam.Add(dtFrom_.Substring(0, 10));
                //               sParam.Add("@Cycno");
                //               sParam.Add(cycno);
                //   sParam.Add("@Cycno2");
                //   sParam.Add(cycno2);
                //               sParam.Add("@BkCode");
                //               sParam.Add(bankcode);
                //               sParam.Add("@BrCode");
                //               sParam.Add(branchcode);

                ImgRequest req = new ImgRequest()
                {
                    pdate = dtFrom_.Substring(0, 10),
                    cycleno = cycno,
                    bankid = bankcode,
                    branchid = branchcode,
                    cycno2 = cycno2,
                    Isbranchuser = bruser
                };

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);

                    var postTask = client.PostAsJsonAsync<ImgRequest>("GetCountImg", req);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<RepResponse>();
                        readTask.Wait();
                        if (readTask.Result.responseMessage.ToLower() == "success")
                            rc1 = readTask.Result.cities;
                    }
                }


                if (rc1 == null || !rc1.Any())
                {
                    rc2.norm_count = "0";
                    rc2.samed_count = "0";
                    rc2.int_count = "0";
                    rc2.dol_count = "0";
                }
                else
                {
                    foreach (DataRow city in GetCity().Rows)
                    {
                        string citi = city["instid"].ToString();
                        bool checker = rc1.Any(citi_ => citi_.cityid == citi);
                        if (checker == true)
                        {
                            if (cycno == "2")
                            {
                                rc2.norm_count = "1";
                            }
                            else if (cycno == "5")
                            {
                                rc2.samed_count = "1";
                            }
                            else if (cycno == "1")
                            {
                                rc2.int_count = "1";
                            }
                            else if (cycno == "20")
                            {
                                rc2.dol_count = "1";
                            }
                            break;
                        }
                        else
                        {
                            rc2.norm_count = "0";
                            rc2.samed_count = "0";
                            rc2.int_count = "0";
                            rc2.dol_count = "0";
                        }
                    }
                }
				
				return rc2;
            }
            catch (Exception ex)
            {
				LogWriter.WriteToLog(ex);
                rc2.norm_count = "0";
                            rc2.samed_count = "0";
                            rc2.int_count = "0";
                            rc2.dol_count = "0";
                return rc2;
            }
        }
        public RepCount GetCount_Rep(string cycno, string cycno2)
        {
			List<RepCities> rc2 = new List<RepCities>();
			RepCount rep_ = new RepCount();
            string bruser = HttpContext.Session.GetString(loginModel.Isbranchuser);
            try
            {
                int chk = 0;
                DataTable oDt = new DataTable();
                if (dtFrom_ == null)
                {
                    dtFrom_ = "1990-11-11";
                }
                if (bankcode == null)
                {
                    bankcode = "0";
                }
                if (branchcode == null)
                {
                    branchcode = "0";
                }
                //foreach (DataRow city in GetCity().Rows)
                //{
                    RepRequest req = new RepRequest()
                    {
                        pdate = dtFrom_.Substring(0, 10),
                        cycleno = cycno,
                        bankid = bankcode,
                        branchid = branchcode,
                        cycno2 = cycno2,
                        Isbranchuser = bruser
                    };

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(apiURL);

                        var postTask = client.PostAsJsonAsync<RepRequest>("GetCountReport", req);
                        postTask.Wait();

                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<RepResponse>();
                            readTask.Wait();

                            if (readTask.Result.responseMessage.ToLower() == "success")
                                rc2 = readTask.Result.cities;
                        }
                    }
                if (rc2 == null || !rc2.Any())
                {
                    rep_.norm_count = "0";
                    rep_.samed_count = "0";
                    rep_.int_count = "0";
                    rep_.dol_count = "0";
                }
                else
                {

                    foreach (DataRow city in GetCity().Rows)
                    {
                        string citi = city["instid"].ToString();
                        bool checker = rc2.Any(citi_ => citi_.cityid == citi);
                        if (checker == true)
                        {
                            if (cycno == "2")
                            {
                                rep_.norm_count = "1";
                            }
                            else if (cycno == "5")
                            {
                                rep_.samed_count = "1";
                            }
                            else if (cycno == "1")
                            {
                                rep_.int_count = "1";
                            }
                            else if (cycno == "20")
                            {
                                rep_.dol_count = "1";
                            }
                            break;
                        }
                        else
                        {
                            rep_.norm_count = "0";
                            rep_.samed_count = "0";
                            rep_.int_count = "0";
                            rep_.dol_count = "0";
                        }
                    }
                }
				return rep_;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                rep_.norm_count = "0";
                rep_.samed_count = "0";
                rep_.int_count = "0";
                rep_.dol_count = "0";
                return rep_;
            }
        }
        public DataTable GetCity()
        {
            try
            {
                List<City> cities = new List<City>();
				DataTable dtCity = new DataTable();
				using (var client = new HttpClient())
                {
					client.BaseAddress = new Uri(apiURL_bk);
					CityRequest req = new CityRequest()
					{
						userID = userid
					};
					//HTTP post
					var responseTask = client.PostAsJsonAsync<CityRequest>("GetUserCities", req);

                    var result = responseTask.Result;
                    if (responseTask.Result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<CityResponse>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                            cities = readTask.Result.cities;
                    }
               }
                if (cities != null)
                {
                    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(InstBank));
                    foreach (PropertyDescriptor p in props)
                        dtCity.Columns.Add(p.Name, p.PropertyType);
                    foreach (var c in cities)
                        dtCity.Rows.Add(c.cityid, c.cityname);
                }
                return dtCity;
            }
            catch (Exception ex)
            {
                DataTable dt = null;
                LogWriter.WriteToLog(ex);
                return dt;
            }
        }


		public JsonResult OnGetSelectedDt(string dt)
		{
			try
			{
				string startDate, endDate;
				string d1, d2;
				startDate = dt;
				endDate = dt;
				string[] aStartDate = startDate.Split('-');
				string[] aendDate = endDate.Split('-');
				d1 = aStartDate[2] + "-" + aStartDate[1] + "-" + aStartDate[0];
				d2 = aendDate[2] + "-" + aendDate[1] + "-" + aendDate[0];
				HttpContext.Session.SetString(GetProdtModel.dtFrom, GetFromIntlDate(d1));
				string ext;
				if (dat1 == dt)
				{

				}
				if (dat2 == dt)
				{
					ext = dat1;
					dat1 = dat2;
					dat2 = ext;
				}
				if (dat3 == dt)
				{
					ext = dat1;
					dat1 = dat3;
					dat3 = ext;
				}
				if (dat4 == dt)
				{
					ext = dat1;
					dat1 = dat4;
					dat4 = ext;
				}
				if (dat5 == dt)
				{
					ext = dat1;
					dat1 = dat5;
					dat5 = ext;
				}
				return new JsonResult("login");
			}
			catch (Exception ex)
			{
				LogWriter.WriteToLog(ex);
				return new JsonResult("Failed");
			}
		}
		public static String GetFromIntlDate(String DDMMYYYY)
		{
			try
			{
				if (isIntlDate(DDMMYYYY, "-") == false)
				{
					char[] delimiterChars = { '/', '-' };
					string DD = "", MM = "", YYYY = "";
					string[] DateArray = DDMMYYYY.Split(delimiterChars);
					DD = DateArray[0];
					MM = DateArray[1];
					YYYY = DateArray[2];
					return (YYYY + "-" + MM + "-" + DD + " 00:00:00");
				}
				else
				{
					return (DDMMYYYY + " 00:00:00");
				}
			}
			catch (Exception ex)
			{
				LogWriter.WriteToLog(ex);
				//LogWriter.WriteToLog(ex);
				return "";
			}
		}
		public static Boolean isIntlDate(String DateString, String Delimiter)
		{
			try
			{
				Int32 DelimiterPlace = -1;
				DelimiterPlace = DateString.IndexOf(Delimiter);
				if (DelimiterPlace > 2)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				LogWriter.WriteToLog(ex);
				//LogWriter.WriteToLog(ex);
				return false;
			}
		}
		public static String GetFromIntlDate()
		{
			try
			{
				string FromDate = "";

				FromDate = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 00:00:00";

				return FromDate;
			}
			catch (Exception ex)
			{
				LogWriter.WriteToLog(ex);
				//LogWriter.WriteToLog(ex);
				return "";
			}
		}
		public JsonResult OnGetLoadDt()
		{
			try
			{
				if (HttpContext.Session.GetString(GetProdtModel.DT1) == null)
				{
					var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
					apiURL_getcontrol = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "GetControlsData/";
					LoadData();
					BindData();
					SetDate();
				}
				return new JsonResult("Success" + "|" + dat1 + "|" + dat2 + "|" + dat3 + "|" + dat4 + "|" + dat5);
			}
			catch (Exception ex)
			{
				LogWriter.WriteToLog(ex);
				return new JsonResult("Failed");
			}
		}
		private void LoadData()
		{
			try
			{
				if (HttpContext.Session.GetString(GetProdtModel.DT1) == null)
				{
					DataTable dtDate = new DataTable();

					List<string> pdate_ = new List<string>();

					using (var client = new HttpClient())
					{
						client.BaseAddress = new Uri(apiURL_getcontrol);

						//HTTP GET

						var postTask = client.GetAsync("GetProcessDate");
						postTask.Wait();


						var result = postTask.Result;
						if (result.IsSuccessStatusCode)
						{
							var readTask = result.Content.ReadAsAsync<GetDateResponse>();
							readTask.Wait();

							//if (readTask.Result.responseMessage.ToLower() == "success")
							pdate_ = readTask.Result.pdate;
						}
					}


					PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(ProcessDT));
					foreach (PropertyDescriptor p in props)
						dtDate.Columns.Add(p.Name, p.PropertyType);
					foreach (var c in pdate_)
						dtDate.Rows.Add(c);

					for (int i = 0; i < dtDate.Rows.Count; i++)
					{
						DataRow dr = dtDate.Rows[i];
						if (i == 0)
						{
							dat1 = dr[0].ToString();
							HttpContext.Session.SetString(GetProdtModel.DT1, dat1);
						}
						if (i == 1)
						{
							dat2 = dr[0].ToString();
							HttpContext.Session.SetString(GetProdtModel.DT2, dat2);
						}
						if (i == 2)
						{
							dat3 = dr[0].ToString();
							HttpContext.Session.SetString(GetProdtModel.DT3, dat3);
						}
						if (i == 3)
						{
							dat4 = dr[0].ToString();
							HttpContext.Session.SetString(GetProdtModel.DT4, dat4);
						}
						if (i == 4)
						{
							dat5 = dr[0].ToString();
							HttpContext.Session.SetString(GetProdtModel.DT5, dat5);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogWriter.WriteToLog(ex);
			}

		}
		private void BindData()
		{
			try
			{
				DataTable dtDate = new DataTable();
				dtDate.Columns.Add("dt", typeof(String));
				if (HttpContext.Session.GetString(GetProdtModel.DT1) != null)
					dtDate.Rows.Add(HttpContext.Session.GetString(GetProdtModel.DT1));
				if (HttpContext.Session.GetString(GetProdtModel.DT2) != null)
					dtDate.Rows.Add(HttpContext.Session.GetString(GetProdtModel.DT2));
				if (HttpContext.Session.GetString(GetProdtModel.DT3) != null)
					dtDate.Rows.Add(HttpContext.Session.GetString(GetProdtModel.DT3));
				if (HttpContext.Session.GetString(GetProdtModel.DT4) != null)
					dtDate.Rows.Add(HttpContext.Session.GetString(GetProdtModel.DT4));
				if (HttpContext.Session.GetString(GetProdtModel.DT5) != null)
					dtDate.Rows.Add(HttpContext.Session.GetString(GetProdtModel.DT5));

			}
			catch (Exception ex)
			{
				LogWriter.WriteToLog(ex);
			}
		}
		private void SetDate()
		{
			try
			{
				if (HttpContext.Session.GetString(GetProdtModel.DDL_Date_State) != null)
				{
					return;
				}
				if (HttpContext.Session.GetString(GetProdtModel.DT1) == null)
				{
					HttpContext.Session.SetString(GetProdtModel.dtFrom, GetFromIntlDate());
				}
				else
				{
					string startDate, endDate;
					string d1, d2;
					startDate = HttpContext.Session.GetString(GetProdtModel.DT1);
					endDate = HttpContext.Session.GetString(GetProdtModel.DT1);
					string[] aStartDate = startDate.Split('-');
					string[] aendDate = endDate.Split('-');
					d1 = aStartDate[2] + "-" + aStartDate[1] + "-" + aStartDate[0];
					d2 = aendDate[2] + "-" + aendDate[1] + "-" + aendDate[0];

					HttpContext.Session.SetString(GetProdtModel.dtFrom, GetFromIntlDate(d1));
				}
			}
			catch (Exception ex)
			{
				LogWriter.WriteToLog(ex);
			}
		}
		
	}
}


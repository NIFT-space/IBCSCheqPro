using IBCS_Core_Web_Portal.Helper;
using IBCS_Web_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nancy.Routing;
using System.ComponentModel;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using static IBCS_Core_Web_Portal.Pages.Cycleinfo.BankEmailModel;

namespace IBCS_Core_Web_Portal.Pages
{
    public class GetProdtModel : PageModel
    {
        public const string DT1 = "dt1_";
        public const string DT2 = "dt2_";
        public const string DT3 = "dt3_";
        public const string DT4 = "dt4_";
        public const string DT5 = "dt5_";
        public const string DDL_Date_State = "DDL_Date_State";
        public const string dtFrom = "dtFrom";
        public const string dtTo = "dtTo";
        public static string apiURL_getcontrol = "";

        public static string dat1 = "", dat2 = "", dat3 = "", dat4 = "", dat5 = "";
		public void OnGet()
		{
			try
            {
				userid = HttpContext.Session.GetString(loginModel.SessionKeyName1);

				if (!string.IsNullOrEmpty(userid))
                {
					Response.Redirect("/Sessionexpire", true);
				}

				var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                apiURL_getcontrol = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "GetControlsData/";
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog("Exception on OnGet - " + ex);
				Response.Redirect("/Sessionexpire", true);
			}
        }

		public JsonResult OnGetLoadDt()
        {
            try
            {
                if (HttpContext.Session.GetString(DT1) == null)
                {
                    var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                    apiURL_getcontrol = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "GetControlsData/";
                    LoadData();
                    BindData();
                    SetDate(); 
                }
                    return new JsonResult("Success" + "|" + dat1 + "|" + dat2 + "|" + dat3 + "|" + dat4 + "|" + dat5);
            }
            catch(Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return new JsonResult("Failed");
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
                HttpContext.Session.SetString(dtFrom, GetFromIntlDate(d1));
                HttpContext.Session.SetString(dtTo, GetToIntlDate(d2));

                string ext;
                if(dat1 ==dt)
                {

                }
                if(dat2 == dt)
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
        private void LoadData()
        {
            try
            {
                if (HttpContext.Session.GetString(DT1) == null)
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
                            HttpContext.Session.SetString(DT1, dat1);
                        }
                        if (i == 1)
                        {
                            dat2 = dr[0].ToString();
							HttpContext.Session.SetString(DT2, dat2);
						}
                        if (i == 2)
                        {
                            dat3 = dr[0].ToString();
							HttpContext.Session.SetString(DT3, dat3);
						}
                        if (i == 3)
                        {
                            dat4 = dr[0].ToString();
							HttpContext.Session.SetString(DT4, dat4);
						}
                        if (i == 4)
                        {
                            dat5 = dr[0].ToString();
							HttpContext.Session.SetString(DT5, dat5);
						}
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                //LogWriter.WriteToLog(ex);
            }

        }
        private void BindData()
        {
            try
            {
                DataTable dtDate = new DataTable();
                dtDate.Columns.Add("dt", typeof(String));
                if (HttpContext.Session.GetString(DT1) != null)
                    dtDate.Rows.Add(HttpContext.Session.GetString(DT1));
                if (HttpContext.Session.GetString(DT2) != null)
                    dtDate.Rows.Add(HttpContext.Session.GetString(DT2));
                if (HttpContext.Session.GetString(DT3) != null)
                    dtDate.Rows.Add(HttpContext.Session.GetString(DT3));
                if (HttpContext.Session.GetString(DT4) != null)
                    dtDate.Rows.Add(HttpContext.Session.GetString(DT4));
                if (HttpContext.Session.GetString(DT5) != null)
                    dtDate.Rows.Add(HttpContext.Session.GetString(DT5));

                //if (HttpContext.Session.GetString(DDL_Date_State) != null)
                //{
                //	string iState = HttpContext.Session.GetString(DDL_Date_State);
                //	int oState;
                //	bool isParsable = Int32.TryParse(iState, out oState);
                //	if (isParsable)
                //	{
                //		DDL_Date.SelectedIndex = oState;
                //	}
                //	else
                //	{
                //		DDL_Date.SelectedIndex = 0;
                //	}
                //}
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                //LogWriter.WriteToLog(ex);
            }
        }
        //protected void DDL_Date_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //	try
        //	{
        //		string startDate, endDate;
        //		string d1, d2;
        //		startDate = DDL_Date.SelectedValue;
        //		endDate = DDL_Date.SelectedValue;
        //		string[] aStartDate = startDate.Split('-');
        //		string[] aendDate = endDate.Split('-');
        //		d1 = aStartDate[2] + "-" + aStartDate[1] + "-" + aStartDate[0];
        //		d2 = aendDate[2] + "-" + aendDate[1] + "-" + aendDate[0];
        //		Session["dtFrom"] = GetFromIntlDate(d1);
        //		Session["dtTo"] = GetToIntlDate(d2);
        //		// for date dropdown state
        //		Session["DDL_Date_State"] = DDL_Date.SelectedIndex;
        //		//
        //		Session["cyc_shown"] = null;
        //		Response.Redirect("DateInfoMessage", false);

        //	}
        //	catch (Exception ex)
        //	{
        //		//LogWriter.WriteToLog(ex);
        //	}
        //	finally
        //	{
        //		//GetData();
        //		//BindData();
        //	}
        //}
        private void SetDate()
        {
            try
            {
                if (HttpContext.Session.GetString(DDL_Date_State) != null)
                {
                    return;
                }
                if (HttpContext.Session.GetString(DT1) == null)
                {
                    HttpContext.Session.SetString(dtFrom, GetFromIntlDate());
                    HttpContext.Session.SetString(dtTo, GetToIntlDate());
                }
                else
                {
                    string startDate, endDate;
                    string d1, d2;
                    startDate = HttpContext.Session.GetString(DT1);
                    endDate = HttpContext.Session.GetString(DT1);
                    string[] aStartDate = startDate.Split('-');
                    string[] aendDate = endDate.Split('-');
                    d1 = aStartDate[2] + "-" + aStartDate[1] + "-" + aStartDate[0];
                    d2 = aendDate[2] + "-" + aendDate[1] + "-" + aendDate[0];

                    HttpContext.Session.SetString(dtFrom, GetFromIntlDate(d1));
                    HttpContext.Session.SetString(dtTo, GetToIntlDate(d2));
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                //LogWriter.WriteToLog(ex);
            }
        }
        //private string SetInfo()
        //{
        //	string sSQL = string.Empty;

        //	try
        //	{
        //		DataTable dtCyc = new DataTable();

        //		List<Info> infotext = new List<Info>();

        //		using (var client = new HttpClient())
        //		{
        //			client.BaseAddress = new Uri(apiURL_getcontrol);

        //			//HTTP GET

        //			var postTask = client.GetAsync("GetTickerInfo");
        //			postTask.Wait();

        //			var result = postTask.Result;
        //			if (result.IsSuccessStatusCode)
        //			{
        //				var readTask = result.Content.ReadAsAsync<InfoResponse>();
        //				readTask.Wait();

        //				if (readTask.Result.responseMessage.ToLower() == "success")
        //					infotext = readTask.Result.infotext;
        //			}
        //		}
        //		PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(ProcessDT));
        //		foreach (PropertyDescriptor p in props)
        //			dtCyc.Columns.Add(p.Name, p.PropertyType);
        //		foreach (var c in infotext)
        //			dtCyc.Rows.Add(c.infotext);

        //		if (dtCyc == null)
        //			return "";

        //		if (dtCyc.Rows.Count == 0)
        //			return "";

        //		if (dtCyc.Rows.Count > 0)
        //		{
        //			string sTick = "";
        //			for (int i = 0; i < dtCyc.Rows.Count; i++)
        //			{
        //				sTick += dtCyc.Rows[i]["InfoText"].ToString();
        //			}
        //			return sTick;
        //		}
        //		return "";
        //	}
        //	catch (Exception ex)
        //	{
        //		//LogWriter.WriteToLog(ex);
        //		return "";
        //	}
        //}
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
        public static String GetToIntlDate(String DDMMYYYY)
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
                    return (YYYY + "-" + MM + "-" + DD + " 23:59:00");
                }
                else
                {
                    return (DDMMYYYY + " 23:59:00");
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                //LogWriter.WriteToLog(ex);
                return "";
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

        public static String GetToIntlDate()
        {
            try
            {
                string ToDate = "";

                ToDate = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 23:59:00";

                return ToDate;
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
    }
}

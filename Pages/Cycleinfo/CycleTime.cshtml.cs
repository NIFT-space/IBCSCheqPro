using IBCS_Core_Web_Portal.Helper;
using IBCS_Web_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Reflection.Emit;

namespace IBCS_Core_Web_Portal.Pages.Cycleinfo
{
    public class CycleTimeModel : PageModel
    {
        public static string apiURL = "";
        public static string userid = "";
        public static string userlogid = "";
        public static string bankcode = "";
        public static string Auth_ = "";
        public static string dtFrom = "";
        public int chqer { get; set; }
        public List<ICCycleTiming> iCCycleTimings {get; set;}
        public List<ICWeeklyCycleTiming> iCCycleTimings2 { get; set; }
        public void OnGet()
        {
            try
            {
                userid = HttpContext.Session.GetString(loginModel.SessionKeyName1);
                userlogid = HttpContext.Session.GetString(loginModel.SessionKeyName9);
                bankcode = HttpContext.Session.GetString(loginModel.SessionKeyName10);
                Auth_ = HttpContext.Session.GetString(loginModel.SessionKeyName16);
                dtFrom = HttpContext.Session.GetString(GetProdtModel.dtFrom);
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
                apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "CycleTime/";
				Grid1LoadData();
				Grid2LoadData();

			}
            catch (ThreadAbortException)
            {

            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
  //      public void OnGetSelectedDt(string dt)
		//{
		//	userid = HttpContext.Session.GetString(loginModel.SessionKeyName1);
		//	userlogid = HttpContext.Session.GetString(loginModel.SessionKeyName9);
		//	bankcode = HttpContext.Session.GetString(loginModel.SessionKeyName10);
		//	Auth_ = HttpContext.Session.GetString(loginModel.SessionKeyName16);
		//	dtFrom = HttpContext.Session.GetString(GetProdtModel.dtFrom);
		//	if (Auth_ != null && Request.Cookies["IBCS.Auth.Token"] != null)
		//	{
		//		if (!Auth_.Equals(Request.Cookies["IBCS.Auth.Token"].ToString()))
		//		{
		//			Response.Clear();
		//			Response.Redirect("/Sessionexpire", true);
		//		}
		//		else
		//		{
		//			///GOOD TO GO///
		//		}
		//	}
		//	else
		//	{
		//		Response.Clear();
		//		Response.Redirect("/Sessionexpire", true);
		//	}

		//	Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate"); // HTTP 1.1.
		//	Response.Headers.Append("Pragma", "no-cache"); // HTTP 1.0.
		//	Response.Headers.Append("Last-Modified", System.DateTime.Now.ToString());

		//	if (userid == null)
		//	{
		//		Response.Redirect("/Sessionexpire", true);
		//	}

		//	var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
		//	apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "CycleTime/";
		//	chqer = 1;
		//	try
		//	{
		//		var dtcheck_1 = dtFrom;
		//		var dtcheck_2 = dt;

		//		if (dtcheck_1 == dtcheck_2)
		//		{
		//			///OKAY////
		//		}
		//		else
		//		{
		//			dtFrom = dtcheck_2;
		//		}
				
		//	}
		//	catch (Exception ex)
		//	{
		//		LogWriter.WriteToLog(ex);
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
				return false;
			}
		}
		private void Grid1LoadData()
        {
            try
            {
                DataTable dt = new DataTable();
                int iBankCode = 0;
                string sBankCode = bankcode;
                bool isParsable = Int32.TryParse(sBankCode, out iBankCode);
                

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);

                    //HTTP POST
                    ICCycleTimingRequest req = new ICCycleTimingRequest
                    {
                        SBankCode = sBankCode,
                        IBankCode = iBankCode,
                    };

                    var postTask = client.PostAsJsonAsync<ICCycleTimingRequest>("getICCycleTime", req);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<ICCycleTimingResponse>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                            iCCycleTimings = readTask.Result.iCCycleTiming;
                    }
                }
                //DataTable dtCyc = new DataTable();

                //PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(ICCycleTiming));
                //foreach (PropertyDescriptor p in props)
                //    dtCyc.Columns.Add(p.Name, p.PropertyType);
                //foreach (var c in iCCycleTimings)
                //    dtCyc.Rows.Add(c.CycleNo, c.Cycle_desc, c.Sorder, c.CityName, c.CycleStartTime, c.CycleEndTime);

                //if (dtCyc.Rows.Count > 0)
                //{
                //    dt = dtCyc.Clone();
                //    dt.Columns["cycleStartTime"].DataType = Type.GetType("System.DateTime");
                //    dt.Columns["cycleEndTime"].DataType = Type.GetType("System.DateTime");

                //    foreach (DataRow dr in dtCyc.Rows)
                //    {
                //        dt.ImportRow(dr);
                //    }
                //    dt.AcceptChanges();
                //    //
                //    //GridView1.DataSource = dt;
                //}
                //else
                //{
                //    //GridView1.DataSource = dt;
                //}
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
        private void Grid2LoadData()
        {
            try
            {
                string sSQL = string.Empty;
                DataTable dt = new DataTable();

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);

                    var postTask = client.GetAsync("getICWeeklyCycleTime");
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<ICWeeklyCycleTimingResponse>();
                        readTask.Wait();

                        if (readTask.Result.responseMessage.ToLower() == "success")
                            iCCycleTimings2 = readTask.Result.iCCycleTiming;
                    }
                }
                //DataTable dtCyc = new DataTable();

                //PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(ICWeeklyCycleTiming));
                //foreach (PropertyDescriptor p in props)
                //    dtCyc.Columns.Add(p.Name, p.PropertyType);
                //foreach (var c in iCCycleTimings2)
                //    dtCyc.Rows.Add(c.Cycle_no, c.Cycle_desc, c.Sorder, c.Monday, c.Tuesday, c.Wednesday, c.Thursday, c.Friday, c.Saturday, c.Sunday);

                //if (dtCyc.Rows.Count > 0)
                //{
                //    //GridView2.DataSource = dtCyc;
                //}
                //else
                //{
                //    //GridView2.DataSource = dt;
                //}
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
    }
}

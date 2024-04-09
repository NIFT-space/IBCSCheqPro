using IBCS_Web_Portal.Models;
using NIBC.Models;

namespace IBCS_Core_Web_Portal.Helper
{
	public class CheckPageAllowed
	{
		public static string apiURL = "";
		public static bool res_ = false;
		public bool Validatepageallowed(string userid_, string pagename_)
		{
			try
			{
				var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
				apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "GetControlsData/";
				using (var client = new HttpClient())
				{
					client.BaseAddress = new Uri(apiURL);
					PageAll_Request req = new PageAll_Request
					{
						userid = userid_,
						pagename = pagename_
					};

					var postTask = client.PostAsJsonAsync("GetIsPageAllowed", req);
					postTask.Wait();

					var result = postTask.Result;
					if (result.IsSuccessStatusCode)
					{
						var readTask = result.Content.ReadAsAsync<PageAllowedResponse>();
						readTask.Wait();

						//if (readTask.Result.responseMessage.ToLower() == "success")
						res_ = readTask.Result.PageAllowed;
					}
				}
				return res_;
			}
			catch (Exception ex)
			{
				LogWriter.WriteToLog("Exception - pageallowed - " + ex);
				return false;
			}
		}
	}
}
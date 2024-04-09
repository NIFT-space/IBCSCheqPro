using IBCS_Core_Web_Portal.Helper;
using IBCS_Web_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace IBCS_Core_Web_Portal.Pages.Marking
{
    public class NetboardModel : PageModel
    {
		public static string city_apiURL = "";
		public static string br_apiURL = "";
		public static string userid = "";
		public static string userlogid = "";
		public static string bankcode = "";
		public static string dtFrom_ = "";
		public static string dtTo_ = "";
        public static string Auth_ = "";
        public List<InstBank> instBanks { get; set; }
		public List<NetBoard> hostnet_ { get; set; }
		public List<NetBoard> obj { get; set; }

		public static string sel_ddlbank, sel_ddlcity, sel_ddlcycle, sel_ddlbranch;
		public static DataTable oDt;
		public void OnGet(string bank)
        {
			try
			{
				userid = HttpContext.Session.GetString(loginModel.SessionKeyName1);
				userlogid = HttpContext.Session.GetString(loginModel.SessionKeyName9);
				bankcode = HttpContext.Session.GetString(loginModel.SessionKeyName10);
				dtFrom_ = HttpContext.Session.GetString(GetProdtModel.dtFrom);
				dtTo_ = HttpContext.Session.GetString(GetProdtModel.dtTo);
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
                Response.Headers.Append("Last-Modified", System.DateTime.Now.ToString());

                if (userid == null)
				{
					Response.Redirect("/Sessionexpire",true);
				}
				CheckPageAllowed cpa = new CheckPageAllowed();

				if (cpa.Validatepageallowed(userid, "Netboard") == false)
				{
					Response.Redirect("/NotAllowed",true);
				}
				var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
				city_apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "Cityboard/";
				br_apiURL = MyConfig.GetValue<string>("AppSettings:ibcs.api") + "Branchboard/";

                BindBankDD();

                if (bank != null)
				{
					OnGetLoadDataFromDB(bank);
				}
					
				
			}
			catch (ThreadAbortException)
			{

			}
			catch (Exception ex)
			{
				LogWriter.WriteToLog("Exception on GetFiles - " + ex);
				Response.Redirect("/NotAllowed",true);
			}
		}
		private void BindBankDD()
		{
			try
			{
				// Bank
				string sUserId = userid;//Convert.ToString(Session["myid"]).Trim();
				string sInstId = bankcode;//Session["BankCode"].ToString();
				DataTable dtInst = new DataTable();
				InstRequest req = new InstRequest()
				{
					UserId = sUserId,
					InstId = sInstId
				};

				using (var client = new HttpClient())
				{
					client.BaseAddress = new Uri(br_apiURL);

					var postTask = client.PostAsJsonAsync("GetInstBank", req);
					postTask.Wait();

					var result = postTask.Result;
					if (result.IsSuccessStatusCode)
					{
						var readTask = result.Content.ReadAsAsync<InstBankResponse>();
						readTask.Wait();

						if (readTask.Result.responseMessage.ToLower() == "success")
							instBanks = readTask.Result.bkAdvice;
					}
				}
			}
			catch (Exception ex)
			{
				LogWriter.WriteToLog("Exception on BindBankDD - " + ex);
			}
		}
		public JsonResult OnGetLoadDataFromDB(string bank)
		{
			List<NetBoard> ht2 = new List<NetBoard>();
			DataTable _net;
			DataTable _pk;
			DataTable _pkRtn;
			DataTable _fc;
			DataTable _fcRtn;
			try
			{
				_net = new DataTable();
				_pk = new DataTable();
				_pkRtn = new DataTable();
				_fc = new DataTable();
				_fcRtn = new DataTable();

				DataTable oDt = new DataTable();
				string sDateTime = dtFrom_;//Session["dtFrom"].ToString();
				string sBankCode = bank;//Session["BankCode"].ToString();
				///if (DDL_Bank.SelectedIndex == -1)
				////{
					//sBankCode = Session["BankCode"].ToString();
				//}
				//else
				//{
				//	sBankCode = DDL_Bank.SelectedItem.Value;
				//}

				vNetScorePakwiseRequest req = new vNetScorePakwiseRequest();
				req.pdate = sDateTime;
				req.bkcode = bank;

				using (var client = new HttpClient())
				{
					client.BaseAddress = new Uri(city_apiURL);

					var postTask = client.PostAsJsonAsync<vNetScorePakwiseRequest>("GetvNetBoard", req);
					postTask.Wait();

					var result = postTask.Result;
					if (result.IsSuccessStatusCode)
					{
						var readTask = result.Content.ReadAsAsync<vNetBoardResponse>();
						readTask.Wait();

						if (readTask.Result.responseMessage.ToLower() == "success")
							hostnet_ = readTask.Result.NetBoard;

						if (hostnet_ != null)
						{
							PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(NetBoard));

							foreach (PropertyDescriptor p in props)
								oDt.Columns.Add(p.Name, p.PropertyType);
							foreach (var c in hostnet_)
								oDt.Rows.Add(c.cyclename, c.Owcount, c.Owamount, c.Incount, c.Inamount,c.Netcount, c.Netamount, c.CurrencyType);
						}
					}
				}

				if (oDt == null || oDt.Rows.Count == 0)
				{
					//DataTable dt = new DataTable();
					//gridMain1.DataSource = dt;
					//gridMain1.DataBind();
					//Session["PkTable"] = null;
					//Session["PkrTable"] = null;
					//Session["FcTable"] = null;
					//Session["FcrTable"] = null;
					//ltItemChart.Text = "";
					//ltlAmountChart.Text = "";
				}
				else
				{
					_net = oDt;

					for (int i = 0; i < 1; i++)
					{
						DataRow dr = _net.Rows[i];
						if (i == 0)
						{
							//lblBank.Text = dr[0] + " - " + dr[1].ToString();
						}
					}
					_net.DefaultView.RowFilter = String.Format("CurrencyType = '{0}'", "1");
					_pk = _net.DefaultView.ToTable();

					_net.DefaultView.RowFilter = String.Format("CurrencyType = '{0}'", "2");
					_pkRtn = _net.DefaultView.ToTable();

					_net.DefaultView.RowFilter = String.Format("CurrencyType = '{0}'", "3");
					_fc = _net.DefaultView.ToTable();

					_net.DefaultView.RowFilter = String.Format("CurrencyType = '{0}'", "4");
					_fcRtn = _net.DefaultView.ToTable();

					obj = CommonMethod.ConvertToList<NetBoard>(_pk);


					if (_pk.Rows.Count > 0)
					{
						//gridMain1.DataSource = _pk;
						//gridMain1.DataBind();
						
						
						for(int i =0; i<_pk.Rows.Count; i++)
						{
							NetBoard nb = new NetBoard();
							nb.PK_cyclename = Convert.ToString(_pk.Rows[i]["cyclename"]);
							nb.PK_Incount = Convert.ToInt32(_pk.Rows[i]["Incount"]);
							nb.PK_Inamount = Convert.ToDouble(_pk.Rows[i]["Inamount"]);
							nb.PK_Owcount = Convert.ToInt32(_pk.Rows[i]["Owcount"]);
							nb.PK_Owamount = Convert.ToDouble(_pk.Rows[i]["Owamount"]);
							nb.PK_Netcount = Convert.ToInt32(_pk.Rows[i]["Netcount"]);
							nb.PK_Netamount = Convert.ToDouble(_pk.Rows[i]["Netamount"]);
							obj.Add(nb);
						}

						
						int oCnt = Convert.ToInt32(_pk.Compute("SUM(OwCount)", string.Empty));
						Decimal oAmt = Convert.ToDecimal(_pk.Compute("SUM(OwAmount)", string.Empty));

						int iCnt = Convert.ToInt32(_pk.Compute("SUM(inCount)", string.Empty));
						Decimal iAmt = Convert.ToDecimal(_pk.Compute("SUM(inAmount)", string.Empty));

						//int nCnt = Convert.ToInt32(_pk.Compute("SUM(NetCount)", string.Empty));
						Decimal nAmt = Convert.ToDecimal(_pk.Compute("SUM(NetAmount)", string.Empty));
						//ShowData(_pk);
					}
					else
					{
						//ShowData(_pk);
						//Session["PkTable"] = null;
					}
					if (_pkRtn.Rows.Count > 0)
					{
                        for (int i = 0; i < _pkRtn.Rows.Count; i++)
						{
							NetBoard nb = new NetBoard();
							nb.rtn_cyclename = Convert.ToString(_pkRtn.Rows[i]["cyclename"]);
							nb.rtn_Incount = Convert.ToInt32(_pkRtn.Rows[i]["Incount"]);
							nb.rtn_Inamount = Convert.ToDouble(_pkRtn.Rows[i]["Inamount"]);
							nb.rtn_Owcount = Convert.ToInt32(_pkRtn.Rows[i]["Owcount"]);
							nb.rtn_Owamount = Convert.ToDouble(_pkRtn.Rows[i]["Owamount"]);
							nb.rtn_Netcount = Convert.ToInt32(_pkRtn.Rows[i]["Netcount"]);
							nb.rtn_Netamount = Convert.ToDouble(_pkRtn.Rows[i]["Netamount"]);
							obj.Add(nb);
						}
						//gridMain2.DataSource = _pkRtn;
						////gridMain2.DataBind();
						//Session["PkrTable"] = _pkRtn;
						//ShowData(_pkRtn);
					}
					else
					{
						//ShowData(_pkRtn);
						//Session["PkrTable"] = null;
					}
					if (_fc.Rows.Count > 0)
					{
                        for (int i = 0; i < _fc.Rows.Count; i++)
						{
							NetBoard nb = new NetBoard();
							nb.FC_cyclename = Convert.ToString(_fc.Rows[i]["cyclename"]);
							nb.FC_Incount = Convert.ToInt32(_fc.Rows[i]["Incount"]);
							nb.FC_Inamount = Convert.ToDouble(_fc.Rows[i]["Inamount"]);
							nb.FC_Owcount = Convert.ToInt32(_fc.Rows[i]["Owcount"]);
							nb.FC_Owamount = Convert.ToDouble(_fc.Rows[i]["Owamount"]);
							nb.FC_Netcount = Convert.ToInt32(_fc.Rows[i]["Netcount"]);
							nb.FC_Netamount = Convert.ToDouble(_fc.Rows[i]["Netamount"]);
							obj.Add(nb);
						}
						//ShowData(_fc);
						//gridMain3.DataSource = _fc;
						//gridMain3.DataBind();
						//Session["FcTable"] = _fc;
					}
					else
					{
						//ShowData(_fc);
						//Session["FcTable"] = null;
					}
					if (_fcRtn.Rows.Count > 0)
					{
                        for (int i = 0; i < _fcRtn.Rows.Count; i++)
						{
							NetBoard nb = new NetBoard();
							nb.FC_RTNcyclename = Convert.ToString(_fcRtn.Rows[i]["cyclename"]);
							nb.FC_RTNIncount = Convert.ToInt32(_fcRtn.Rows[i]["Incount"]);
							nb.FC_RTNInamount = Convert.ToDouble(_fcRtn.Rows[i]["Inamount"]);
							nb.FC_RTNOwcount = Convert.ToInt32(_fcRtn.Rows[i]["Owcount"]);
							nb.FC_RTNOwamount = Convert.ToDouble(_fcRtn.Rows[i]["Owamount"]);
							nb.FC_Netcount = Convert.ToInt32(_fcRtn.Rows[i]["Netcount"]);
							nb.FC_RTNNetamount = Convert.ToDouble(_fcRtn.Rows[i]["Netamount"]);
							obj.Add(nb);
						}
						//gridMain4.DataSource = _fcRtn;
						//gridMain4.DataBind();
						//ShowData(_fcRtn);
						//Session["FcrTable"] = _fc;
					}
					else
					{
						//ShowData(_fcRtn);
						//Session["FcrTable"] = null;
					}
				}
				return new JsonResult(obj);
				
			}
			catch (Exception ex)
			{
				LogWriter.WriteToLog(ex);
				return new JsonResult("Failed"); 
			}
		}
		public static class CommonMethod
		{
			public static List<T> ConvertToList<T>(DataTable dt)
			{
				var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
				var properties = typeof(T).GetProperties();
				return dt.AsEnumerable().Select(row => {
					var objT = Activator.CreateInstance<T>();
					foreach (var pro in properties)
					{
						if (columnNames.Contains(pro.Name.ToLower()))
						{
							try
							{
								pro.SetValue(objT, row[pro.Name]);
							}
							catch (Exception ex) { }
						}
					}
					return objT;
				}).ToList();
			}
		}
		//private void LoadDataFromCSV()
		//{
		//	DataTable _net;
		//	DataTable _pk;
		//	DataTable _pkRtn;
		//	DataTable _fc;
		//	DataTable _fcRtn;
		//	try
		//	{
		//		_net = new DataTable();
		//		_pk = new DataTable();
		//		_pkRtn = new DataTable();
		//		_fc = new DataTable();
		//		_fcRtn = new DataTable();

		//		//string FileName = Server.MapPath("../csv/HostNet_CW.csv");
		//		//FileStream fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
		//		//TextFileDataSet MyTextFileDataSet = new TextFileDataSet();
		//		//MyTextFileDataSet.SkipFirstRow = true;
		//		//RegexColumnBuilder builder = new RegexColumnBuilder();
		//		//builder.AddColumn("BankID", ',', RegexColumnType.STRING);
		//		//builder.AddColumn("BankName", ',', RegexColumnType.STRING);
		//		//builder.AddColumn("CycleID", ',', RegexColumnType.STRING);
		//		//builder.AddColumn("cycle_desc", ',', RegexColumnType.STRING);
		//		//builder.AddColumn("OwCount", ',', RegexColumnType.INTEGER);
		//		//builder.AddColumn("OwAmount", ',', RegexColumnType.DOUBLE);
		//		//builder.AddColumn("IwCount", ',', RegexColumnType.INTEGER);
		//		//builder.AddColumn("IwAmount", ',', RegexColumnType.DOUBLE);
		//		//builder.AddColumn("NetCount", ',', RegexColumnType.INTEGER);
		//		//builder.AddColumn("NetAmount", ',', RegexColumnType.DOUBLE);
		//		//builder.AddColumn("CurrencyType", ',', RegexColumnType.INTEGER);
		//		//MyTextFileDataSet.ColumnBuilder = builder;
		//		//MyTextFileDataSet.TableName = "Table1";
		//		//MyTextFileDataSet.Fill(fileStream);
		//		//fileStream.Close();
		//		//_net = MyTextFileDataSet.Tables[0];

		//		//for (int i = 0; i < 1; i++)
		//		//{
		//		//    DataRow dr = _net.Rows[i];
		//		//    if (i == 0)
		//		//    {
		//		//        //lblBank.Text = dr[0] + " " + dr[1].ToString();
		//		//    }
		//		//}

		//		//_net.DefaultView.RowFilter = String.Format("CurrencyType = '{0}'", "1");
		//		//_pk = _net.DefaultView.ToTable();

		//		//_net.DefaultView.RowFilter = String.Format("CurrencyType = '{0}'", "2");
		//		//_pkRtn = _net.DefaultView.ToTable();

		//		//_net.DefaultView.RowFilter = String.Format("CurrencyType = '{0}'", "3");
		//		//_fc = _net.DefaultView.ToTable();

		//		//_net.DefaultView.RowFilter = String.Format("CurrencyType = '{0}'", "4");
		//		//_fcRtn = _net.DefaultView.ToTable();



		//		//gridMain1.DataSource = _pk;
		//		//gridMain1.DataBind();

		//		//int oCnt = Convert.ToInt32(_pk.Compute("SUM(OwCount)", string.Empty));
		//		//Decimal oAmt = Convert.ToDecimal(_pk.Compute("SUM(OwAmount)", string.Empty));

		//		//int iCnt = Convert.ToInt32(_pk.Compute("SUM(iwCount)", string.Empty));
		//		//Decimal iAmt = Convert.ToDecimal(_pk.Compute("SUM(iwAmount)", string.Empty));

		//		////int nCnt = Convert.ToInt32(_pk.Compute("SUM(NetCount)", string.Empty));
		//		//Decimal nAmt = Convert.ToDecimal(_pk.Compute("SUM(NetAmount)", string.Empty));


		//		//gridMain1.FooterRow.Cells[3].Text = "Total";
		//		//gridMain1.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Left;

		//		//gridMain1.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
		//		//gridMain1.FooterRow.Cells[4].Text = String.Format("{0:N0}", oCnt);

		//		//gridMain1.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
		//		//gridMain1.FooterRow.Cells[5].Text = String.Format("{0:n}", oAmt);

		//		//gridMain1.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
		//		//gridMain1.FooterRow.Cells[6].Text = String.Format("{0:N0}", iCnt);

		//		//gridMain1.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Right;
		//		//gridMain1.FooterRow.Cells[7].Text = String.Format("{0:n}", iAmt);

		//		//gridMain1.FooterRow.Cells[9].HorizontalAlign = HorizontalAlign.Right;
		//		//gridMain1.FooterRow.Cells[9].Text = String.Format("{0:#,##0.00;(#,##0.00);0}", nAmt);

		//		//gridMain2.DataSource = _pkRtn;
		//		//gridMain2.DataBind();

		//		//gridMain3.DataSource = _fc;
		//		//gridMain3.DataBind();

		//		//gridMain4.DataSource = _fcRtn;
		//		//gridMain4.DataBind();

		//		//Session["PkTable"] = _pk;
		//		//Session["PkrTable"] = _pkRtn;
		//		//Session["FcTable"] = _fc;
		//		//Session["FcrTable"] = _fcRtn;
		//	}
		//	catch (Exception ex)
		//	{
		//		LogWriter.WriteToLog(ex);
		//	}
		//}
		//protected void btn_back_Click(object sender, EventArgs e)
		//{
		//	try
		//	{
		//		DataTable dt = new DataTable(); ;

		//		if (Session["PkTable"] == null)
		//		{
		//			MultiView1.ActiveViewIndex = 0;
		//			btn_back.BackColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");
		//			btn_back.ForeColor = System.Drawing.Color.White;

		//			btn_pkr_rtn.BackColor = System.Drawing.Color.White;
		//			btn_pkr_rtn.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");

		//			btn_fc_clg.BackColor = System.Drawing.Color.White;
		//			btn_fc_clg.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");

		//			btn_fc_rtn.BackColor = System.Drawing.Color.White;
		//			btn_fc_rtn.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");

		//			gridMain1.DataSource = dt;
		//			gridMain1.DataBind();
		//			ShowData(dt);

		//		}
		//		else
		//		{
		//			dt = (DataTable)Session["PkTable"];
		//			MultiView1.ActiveViewIndex = 0;

		//			btn_back.BackColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");
		//			btn_back.ForeColor = System.Drawing.Color.White;

		//			btn_pkr_rtn.BackColor = System.Drawing.Color.White;
		//			btn_pkr_rtn.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");

		//			btn_fc_clg.BackColor = System.Drawing.Color.White;
		//			btn_fc_clg.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");

		//			btn_fc_rtn.BackColor = System.Drawing.Color.White;
		//			btn_fc_rtn.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");


		//			if (dt.Rows.Count > 0)
		//			{
		//				int oCnt = Convert.ToInt32(dt.Compute("SUM(OwCount)", string.Empty));
		//				Decimal oAmt = Convert.ToDecimal(dt.Compute("SUM(OwAmount)", string.Empty));

		//				int iCnt = Convert.ToInt32(dt.Compute("SUM(iwCount)", string.Empty));
		//				Decimal iAmt = Convert.ToDecimal(dt.Compute("SUM(iwAmount)", string.Empty));

		//				//int nCnt = Convert.ToInt32(dt.Compute("SUM(NetCount)", string.Empty));
		//				Decimal nAmt = Convert.ToDecimal(dt.Compute("SUM(NetAmount)", string.Empty));


		//				gridMain1.FooterRow.Cells[3].Text = "Total";
		//				gridMain1.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Left;

		//				gridMain1.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
		//				gridMain1.FooterRow.Cells[4].Text = String.Format("{0:N0}", oCnt);

		//				gridMain1.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
		//				gridMain1.FooterRow.Cells[5].Text = String.Format("{0:n}", oAmt);

		//				gridMain1.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
		//				gridMain1.FooterRow.Cells[6].Text = String.Format("{0:N0}", iCnt);

		//				gridMain1.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Right;
		//				gridMain1.FooterRow.Cells[7].Text = String.Format("{0:n}", iAmt);

		//				gridMain1.FooterRow.Cells[9].HorizontalAlign = HorizontalAlign.Right;
		//				gridMain1.FooterRow.Cells[9].Text = String.Format("{0:#,##0.00;(#,##0.00);0}", nAmt);

		//				ShowData((DataTable)Session["PkTable"]);
		//			}
		//		}

		//	}
		//	catch (Exception ex)
		//	{
		//		LogWriter.WriteToLog(ex);
		//	}
		//}

		//protected void btn_pkr_rtn_Click(object sender, EventArgs e)
		//{

		//	try
		//	{
		//		DataTable dt = new DataTable();

		//		if (Session["PkrTable"] == null)
		//		{
		//			MultiView1.ActiveViewIndex = 1;
		//			btn_pkr_rtn.BackColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");
		//			btn_pkr_rtn.ForeColor = System.Drawing.Color.White;

		//			btn_back.BackColor = System.Drawing.Color.White;
		//			btn_back.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");

		//			btn_fc_clg.BackColor = System.Drawing.Color.White;
		//			btn_fc_clg.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");

		//			btn_fc_rtn.BackColor = System.Drawing.Color.White;
		//			btn_fc_rtn.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");

		//			gridMain2.DataSource = dt;
		//			gridMain2.DataBind();
		//			ShowData(dt);
		//		}
		//		else
		//		{
		//			dt = (DataTable)Session["PkrTable"];
		//			MultiView1.ActiveViewIndex = 1;

		//			btn_pkr_rtn.BackColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");
		//			btn_pkr_rtn.ForeColor = System.Drawing.Color.White;

		//			btn_back.BackColor = System.Drawing.Color.White;
		//			btn_back.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");

		//			btn_fc_clg.BackColor = System.Drawing.Color.White;
		//			btn_fc_clg.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");

		//			btn_fc_rtn.BackColor = System.Drawing.Color.White;
		//			btn_fc_rtn.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");



		//			if (dt.Rows.Count > 0)
		//			{
		//				int oCnt = Convert.ToInt32(dt.Compute("SUM(OwCount)", string.Empty));
		//				Decimal oAmt = Convert.ToDecimal(dt.Compute("SUM(OwAmount)", string.Empty));

		//				int iCnt = Convert.ToInt32(dt.Compute("SUM(iwCount)", string.Empty));
		//				Decimal iAmt = Convert.ToDecimal(dt.Compute("SUM(iwAmount)", string.Empty));

		//				//int nCnt = Convert.ToInt32(dt.Compute("SUM(NetCount)", string.Empty));
		//				Decimal nAmt = Convert.ToDecimal(dt.Compute("SUM(NetAmount)", string.Empty));


		//				gridMain2.FooterRow.Cells[3].Text = "Total";
		//				gridMain2.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Left;

		//				gridMain2.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
		//				gridMain2.FooterRow.Cells[4].Text = String.Format("{0:N0}", oCnt);

		//				gridMain2.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
		//				gridMain2.FooterRow.Cells[5].Text = String.Format("{0:n}", oAmt);

		//				gridMain2.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
		//				gridMain2.FooterRow.Cells[6].Text = String.Format("{0:N0}", iCnt);

		//				gridMain2.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Right;
		//				gridMain2.FooterRow.Cells[7].Text = String.Format("{0:n}", iAmt);

		//				gridMain2.FooterRow.Cells[9].HorizontalAlign = HorizontalAlign.Right;
		//				gridMain2.FooterRow.Cells[9].Text = String.Format("{0:#,##0.00;(#,##0.00);0}", nAmt);

		//				ShowData((DataTable)Session["PkrTable"]);
		//			}
		//		}

		//	}
		//	catch (Exception ex)
		//	{
		//		LogWriter.WriteToLog(ex);
		//	}
		//}

		//protected void btn_fc_clg_Click(object sender, EventArgs e)
		//{
		//	try
		//	{
		//		DataTable dt = new DataTable();

		//		if (Session["FcTable"] == null)
		//		{
		//			MultiView1.ActiveViewIndex = 2;
		//			btn_fc_clg.BackColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");
		//			btn_fc_clg.ForeColor = System.Drawing.Color.White;

		//			btn_pkr_rtn.BackColor = System.Drawing.Color.White;
		//			btn_pkr_rtn.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");

		//			btn_back.BackColor = System.Drawing.Color.White;
		//			btn_back.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");

		//			btn_fc_rtn.BackColor = System.Drawing.Color.White;
		//			btn_fc_rtn.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");

		//			gridMain3.DataSource = dt;
		//			gridMain3.DataBind();
		//			ShowData(dt);
		//		}
		//		else
		//		{
		//			MultiView1.ActiveViewIndex = 2;

		//			btn_fc_clg.BackColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");
		//			btn_fc_clg.ForeColor = System.Drawing.Color.White;

		//			btn_pkr_rtn.BackColor = System.Drawing.Color.White;
		//			btn_pkr_rtn.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");

		//			btn_back.BackColor = System.Drawing.Color.White;
		//			btn_back.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");

		//			btn_fc_rtn.BackColor = System.Drawing.Color.White;
		//			btn_fc_rtn.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");

		//			dt = (DataTable)Session["FcTable"];

		//			if (dt.Rows.Count > 0)
		//			{

		//				int oCnt = Convert.ToInt32(dt.Compute("SUM(OwCount)", string.Empty));
		//				Decimal oAmt = Convert.ToDecimal(dt.Compute("SUM(OwAmount)", string.Empty));

		//				int iCnt = Convert.ToInt32(dt.Compute("SUM(iwCount)", string.Empty));
		//				Decimal iAmt = Convert.ToDecimal(dt.Compute("SUM(iwAmount)", string.Empty));

		//				//int nCnt = Convert.ToInt32(dt.Compute("SUM(NetCount)", string.Empty));
		//				Decimal nAmt = Convert.ToDecimal(dt.Compute("SUM(NetAmount)", string.Empty));


		//				gridMain3.FooterRow.Cells[3].Text = "Total";
		//				gridMain3.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Left;

		//				gridMain3.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
		//				gridMain3.FooterRow.Cells[4].Text = String.Format("{0:N0}", oCnt);

		//				gridMain3.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
		//				gridMain3.FooterRow.Cells[5].Text = String.Format("{0:n}", oAmt);

		//				gridMain3.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
		//				gridMain3.FooterRow.Cells[6].Text = String.Format("{0:N0}", iCnt);

		//				gridMain3.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Right;
		//				gridMain3.FooterRow.Cells[7].Text = String.Format("{0:n}", iAmt);

		//				gridMain3.FooterRow.Cells[9].HorizontalAlign = HorizontalAlign.Right;
		//				gridMain3.FooterRow.Cells[9].Text = String.Format("{0:#,##0.00;(#,##0.00);0}", nAmt);
		//				ShowData((DataTable)Session["FcTable"]);
		//			}
		//		}

		//	}
		//	catch (Exception ex)
		//	{
		//		LogWriter.WriteToLog(ex);
		//	}

		//}
		//protected void btn_fc_rtn_Click(object sender, EventArgs e)
		//{
		//	try
		//	{
		//		DataTable dt = new DataTable();

		//		if (Session["FcTable"] == null)
		//		{
		//			MultiView1.ActiveViewIndex = 3;
		//			btn_fc_rtn.BackColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");
		//			btn_fc_rtn.ForeColor = System.Drawing.Color.White;

		//			btn_pkr_rtn.BackColor = System.Drawing.Color.White;
		//			btn_pkr_rtn.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");

		//			btn_fc_clg.BackColor = System.Drawing.Color.White;
		//			btn_fc_clg.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");

		//			btn_back.BackColor = System.Drawing.Color.White;
		//			btn_back.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");

		//			gridMain4.DataSource = dt;
		//			gridMain4.DataBind();
		//			ShowData(dt);
		//		}
		//		else
		//		{
		//			MultiView1.ActiveViewIndex = 3;

		//			btn_fc_rtn.BackColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");
		//			btn_fc_rtn.ForeColor = System.Drawing.Color.White;

		//			btn_pkr_rtn.BackColor = System.Drawing.Color.White;
		//			btn_pkr_rtn.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");

		//			btn_fc_clg.BackColor = System.Drawing.Color.White;
		//			btn_fc_clg.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");

		//			btn_back.BackColor = System.Drawing.Color.White;
		//			btn_back.ForeColor = System.Drawing.ColorTranslator.FromHtml("#d7182a");

		//			dt = (DataTable)Session["FcrTable"];

		//			if (dt.Rows.Count > 0)
		//			{

		//				int oCnt = Convert.ToInt32(dt.Compute("SUM(OwCount)", string.Empty));
		//				Decimal oAmt = Convert.ToDecimal(dt.Compute("SUM(OwAmount)", string.Empty));

		//				int iCnt = Convert.ToInt32(dt.Compute("SUM(iwCount)", string.Empty));
		//				Decimal iAmt = Convert.ToDecimal(dt.Compute("SUM(iwAmount)", string.Empty));

		//				//int nCnt = Convert.ToInt32(dt.Compute("SUM(NetCount)", string.Empty));
		//				Decimal nAmt = Convert.ToDecimal(dt.Compute("SUM(NetAmount)", string.Empty));


		//				gridMain4.FooterRow.Cells[3].Text = "Total";
		//				gridMain4.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Left;

		//				gridMain4.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
		//				gridMain4.FooterRow.Cells[4].Text = String.Format("{0:N0}", oCnt);

		//				gridMain4.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
		//				gridMain4.FooterRow.Cells[5].Text = String.Format("{0:n}", oAmt);

		//				gridMain4.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
		//				gridMain4.FooterRow.Cells[6].Text = String.Format("{0:N0}", iCnt);

		//				gridMain4.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Right;
		//				gridMain4.FooterRow.Cells[7].Text = String.Format("{0:n}", iAmt);

		//				gridMain4.FooterRow.Cells[9].HorizontalAlign = HorizontalAlign.Right;
		//				gridMain4.FooterRow.Cells[9].Text = String.Format("{0:#,##0.00;(#,##0.00);0}", nAmt);

		//				ShowData((DataTable)Session["FcrTable"]);
		//			}

		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		LogWriter.WriteToLog(ex);
		//	}

		//}
		private void ShowData(DataTable tb)
		{
			try
			{
				if (tb != null)
				{
					if (tb.Rows.Count > 0)
					{
						String chart = "";
						// put data from database to chart
						int owCount = 0;
						int iwCount = 0;
						int netCount = 0;
						decimal owAmount = 0;
						decimal iwAmount = 0;
						decimal netAmount = 0;
						for (int i = 0; i < tb.Rows.Count; i++)
						{
							owCount += Convert.ToInt32(tb.Rows[i]["OwCount"].ToString());
							iwCount += Convert.ToInt32(tb.Rows[i]["InCount"].ToString());
							netCount += Convert.ToInt32(tb.Rows[i]["NetCount"].ToString());
							owAmount += Convert.ToDecimal(tb.Rows[i]["OwAmount"].ToString());
							iwAmount += Convert.ToDecimal(tb.Rows[i]["InAmount"].ToString());
							netAmount += Convert.ToDecimal(tb.Rows[i]["NetAmount"].ToString());
						}
						// You can change your chart height by modify height value
						chart = "<canvas id=\"item-pie-chart\"width =\"100%\" height=\"70\"></canvas>";
						chart += "<script>";
						chart += "new Chart(document.getElementById(\"item-pie-chart\"),{type: 'pie', data:{";
						chart += "datasets: [{data:[";
						chart += owCount;
						chart += ",";
						chart += iwCount;
						chart += ",";
						chart += netCount;
						chart += ",],";
						chart += "backgroundColor: [window.chartColors.red,window.chartColors.green,window.chartColors.yellow,],";
						chart += "label: 'Pakistan Net Clearing'}],";
						chart += "labels: ['Outward','Inward','Net',]},";
						chart += "options: {legend: { labels: { fontSize:14,fontColor:'black',fontStyle:'bold' } }, title: {fontSize:16,fontColor:'black',fontStyle:'bold', display: true,text:'Count'},responsive: true, maintainAspectRatio: true,plugins: { labels:{ render: 'percentage',fontColor: ['black', 'black', 'black'],fontStyle:'bold',fontSize:16,precision: 2}},}"; // Chart title
						chart += "});";
						chart += "</script>";
						//ltItemChart.Text = chart;
						chart = "";
						chart = "<canvas id=\"amount-pie-chart\"width =\"100%\" height=\"70\"></canvas>";
						chart += "<script>";
						chart += "new Chart(document.getElementById(\"amount-pie-chart\"),{type: 'pie', data:{";
						chart += "datasets: [{data:[";
						chart += owAmount;
						chart += ",";
						chart += iwAmount;
						chart += ",";
						chart += netAmount;
						chart += ",],";
						chart += "backgroundColor: [window.chartColors.red,window.chartColors.green,window.chartColors.yellow,],";
						chart += "label: 'Pakistan Net Clearing'}],";
						chart += "labels: ['Outward','Inward','Net',]},";
						chart += "options: {legend: { labels: { fontSize:14,fontColor:'black',fontStyle:'bold' } }, title: {fontSize:16,fontColor:'black',fontStyle:'bold', display: true,text:'Amount'},responsive: true, maintainAspectRatio: true,plugins: { labels:{ render: 'percentage',fontColor: ['black', 'black', 'black'],fontStyle:'bold',fontSize:16,precision: 2}},}"; // Chart title
						chart += "});";
						chart += "</script>";
						//ltlAmountChart.Text = chart;
					}
					else
					{
						//ltItemChart.Text = "";
						//ltlAmountChart.Text = "";
					}
				}
				else
				{
					//ltItemChart.Text = "";
					//ltlAmountChart.Text = "";
				}
			}
			catch (Exception ex)
			{
				LogWriter.WriteToLog(ex);
			}
		}
		private bool isPanPakUser()
		{
			try
			{
				string sUserId = userid;
				string sSQL = string.Empty;
				PanPakResponse resp = new PanPakResponse();
				DataTable dtCity = new DataTable();
				using (var client = new HttpClient())
				{
					client.BaseAddress = new Uri(city_apiURL);
					var postTask = client.GetAsync($"IsPanPakUser?userid={sUserId}");
					postTask.Wait();

					var result = postTask.Result;
					if (result.IsSuccessStatusCode)
					{
						var readTask = result.Content.ReadAsAsync<PanPakResponse>();
						readTask.Wait();

						resp = readTask.Result;
						PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(PanPakResponse));

						foreach (PropertyDescriptor p in props)
							dtCity.Columns.Add(p.Name, p.PropertyType);
						dtCity.Rows.Add(resp.PanPaks);
					}
				}

				if (dtCity.Rows.Count >= 27)
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
		
	}
}

using System.Diagnostics;
using System.Reflection;

namespace IBCS_Core_Web_Portal.Helper
{
    public class LogWriter
    {
        public static string LogPath="";
        public LogWriter()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public static void WriteToLog(Exception ex)
        {
            try
            {
                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                LogPath = MyConfig.GetValue<string>("AppSettings:LogPath");

                string sYear = DateTime.Now.Year.ToString();
                string sMonth = DateTime.Now.Month.ToString();
                string sDay = DateTime.Now.Day.ToString();
                string sHrs = DateTime.Now.Hour.ToString();
                string sMin = DateTime.Now.Minute.ToString();
                string sFileName = "Err" + sYear + sMonth + sDay + sHrs + sMin + ".txt";
                string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                message += Environment.NewLine;
                message += string.Format("Message: {0}", ex.Message);
                message += Environment.NewLine;
                message += string.Format("Source: {0}", ex.Source);
                message += Environment.NewLine;
                message += string.Format("Stack: {0}", ex.StackTrace);
                message += Environment.NewLine;

                string msg = "", src = "", err = "";
                int issent = 0;
                int hr = 0;
                string cmd = string.Empty;


                hr = ex.HResult;
                msg = ex.Message;
                src = ex.Source;
                int len = ex.StackTrace.Length;
                int lenmsg = ex.Message.Length;
                try
                {
                    if (len >= 500)
                    {
                        //err = ex.StackTrace.PadRight(100);
                        err = ex.StackTrace.Substring(len - 500, 500);
                    }
                    else
                    {
                        err = ex.StackTrace;
                    }
                }
                catch
                {
                    err = ex.StackTrace.ToString();
                }

                if (msg.Contains("'"))
                {
                    msg = ex.Message.Replace("'", "");
                }
                try
                {
                    //if (msg.Length > 1000)
                    //{
                    //    msg = msg.PadRight(1000);
                    //}
                    //else
                    //{
                    //    msg = msg.PadRight(200);
                    //}
                    if (lenmsg >= 500)
                    {
                        msg = ex.Message.Substring(lenmsg - 500, 500);
                    }
                    else
                    {
                        msg = ex.Message;
                    }
                }
                catch
                {
                    msg = "Large Data in Message";
                }
                if (msg.Contains("'"))
                {
                    msg = msg.ToString().Replace("'", "");
                }
                if (src.Contains("'"))
                {
                    src = src.ToString().Replace("'", "");
                }
                //if (src.Length > 100)
                //{
                //    src = src.PadRight(100);
                //}
                if (err.Contains("'"))
                {
                    err = err.ToString().Replace("'", "");
                }
                cmd += " ('" + ex.HResult + "', '" + msg + "', '" + src + "' , '" + err + "' , '" + DateTime.Now + "' , " + issent + ")";

                try
                {
                    if (hr == -2146233040)
                    {

                    }
                    else if (hr == -2147467261)
                    {

                    }
                    else
                    {

                    }
                }
                catch (Exception)
                {

                }

                if (hr == -2146233040)
                {

                }
                else if (hr == -2147467261)
                {

                }
                else
                {
                    string filepath = LogPath;

                    if (!Directory.Exists(filepath))
                    {
                        Directory.CreateDirectory(filepath);
                    }
                    filepath = filepath + sFileName;
                    if (!File.Exists(filepath))
                    {
                        File.Create(filepath).Dispose();
                    }
                    using (StreamWriter sw = File.AppendText(filepath))
                    {
                        sw.WriteLine(message);
                        sw.Flush();
                        sw.Close();
                    }
                }
            }
            catch (Exception e)
            {
                e.ToString();

            }
        }

        public static void WriteToLog(string ex)
        {
            try
            {
                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                LogPath = MyConfig.GetValue<string>("AppSettings:LogPath");

                string sYear = DateTime.Now.Year.ToString();
                string sMonth = DateTime.Now.Month.ToString();
                string sDay = DateTime.Now.Day.ToString();
                string sHrs = DateTime.Now.Hour.ToString();
                string sMin = DateTime.Now.Minute.ToString();
                string sFileName = "Msg" + sYear + sMonth + sDay + sHrs + sMin + ".txt";
                string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                //message += ex.HResult + "    ";
                message += Environment.NewLine;
                message += string.Format("Message: {0}", ex);
                message += Environment.NewLine;

                string filepath = LogPath;

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }
                filepath = filepath + sFileName;
                if (!File.Exists(filepath))
                {
                    File.Create(filepath).Dispose();
                }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(message);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();

            }
        }
        public static void WriteToLog(Exception ex, string senderpagename)
        {
            try
            {
                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                LogPath = MyConfig.GetValue<string>("AppSettings:LogPath");
                string sYear = DateTime.Now.Year.ToString();
                string sMonth = DateTime.Now.Month.ToString();
                string sDay = DateTime.Now.Day.ToString();
                string sHrs = DateTime.Now.Hour.ToString();
                string sMin = DateTime.Now.Minute.ToString();
                string sFileName = "Dtl" + sYear + sMonth + sDay + sHrs + sMin + ".txt";
                string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                message += Environment.NewLine;
                if (ex.InnerException != null)
                {
                    message += "Inner Exception Type: ";
                    message += Environment.NewLine;
                    message += ex.InnerException.GetType().ToString();
                    message += Environment.NewLine;
                    message += "Inner Exception: ";
                    message += Environment.NewLine;
                    message += ex.InnerException.Message;
                    message += Environment.NewLine;
                    message += "Inner Source: ";
                    message += Environment.NewLine;
                    message += ex.InnerException.Source;
                    message += Environment.NewLine;
                    if (ex.InnerException.StackTrace != null)
                    {
                        message += "Inner Stack Trace: ";
                        message += Environment.NewLine;
                        message += ex.InnerException.StackTrace;
                        message += Environment.NewLine;
                    }
                }
                message += "Exception ID:";
                message += Environment.NewLine;
                message += "Exception Type: ";
                message += Environment.NewLine;
                message += ex.GetType().ToString();
                message += Environment.NewLine;
                message += "Exception: " + ex.Message;
                message += Environment.NewLine;
                message += "Source: " + senderpagename;
                message += Environment.NewLine;
                message += "Stack Trace: ";
                message += Environment.NewLine;
                if (ex.StackTrace != null)
                {
                    message += (ex.StackTrace);
                    message += Environment.NewLine;
                }
                StackTrace stackTrace = new StackTrace();
                StackFrame stackFrame1 = stackTrace.GetFrame(1);
                MethodBase methodBase1 = stackFrame1.GetMethod();
                string Parent_Method_name = methodBase1.Name;

                message += string.Format("Message: {0}", ex.Message);
                message += Environment.NewLine;
                message += string.Format("Source: {0}", ex.Source);
                message += Environment.NewLine;
                message += string.Format("Stack: {0}", ex.StackTrace);
                message += Environment.NewLine;

                string filepath = LogPath;

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }
                filepath = filepath + sFileName;
                if (!File.Exists(filepath))
                {
                    File.Create(filepath).Dispose();
                }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(message);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();

            }
        }
    }
}

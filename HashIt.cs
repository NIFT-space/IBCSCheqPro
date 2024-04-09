using System.Reflection.Metadata;
using System.Text;
using System.Security.Cryptography;
using IBCS_Core_Web_Portal.Helper;

namespace IBCS_Core_Web_Portal
{
    public class HashIt
    {
        public string GetHash(string Code)
        {
            try
            {
                string HASH;
                SHA256Managed shaHash = new SHA256Managed();

                Byte[] sText;
                StringBuilder objSB = new StringBuilder();

                objSB.Append(Code);

                sText = Encoding.UTF8.GetBytes(objSB.ToString());
                Byte[] sHash_utf8;
                sHash_utf8 = shaHash.ComputeHash(sText);
                HASH = Convert.ToBase64String(sHash_utf8);
                return HASH;
            }
            catch (Exception ex)
            { LogWriter.WriteToLog(ex);
              return ""; }
        }
        public static string DecryptString(string cipherText)
        {
            try
            {
                string tqef2 = "6c2a47cb557a43a690b479ac2382fa10";

                byte[] iv = new byte[16];
                byte[] buffer = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(tqef2);
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return "";
            }
        }
    }
}

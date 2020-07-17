using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Configuration;
using sms_submit;
using System.Text.RegularExpressions;
using System.Reflection;

namespace web.classes
{
    public static class Util
    {
        public static bool MatchTemplate(string username, string senderid, string content)
        {
            var uri = ConfigurationManager.AppSettings["TemplateUri"] + "username=" + username + "&senderid=" + senderid + "&template=" + HttpUtility.UrlEncode(content);
            var response = MakehttpGetCall(uri);
            if (response == "")
                return false;
            if (response.IndexOf("||") == -1)
                return false;
            string[] arr = response.Split(new string[] { "||" }, StringSplitOptions.None);
            if (arr[1].Contains("true") && arr[2].Contains("true"))
                return true;
            return false;
        }

        public static string MakehttpGetCall(string uri)
        {
            try
            {
                var responseFromServer = "";
                WebRequest webreq = WebRequest.Create(uri);
                WebResponse resp = webreq.GetResponse();
                using (Stream dataStream = resp.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(dataStream))
                    {
                        responseFromServer = reader.ReadToEnd();
                    }
                }
                resp.Close();
                return responseFromServer;
            }
            catch (Exception ex)
            {
                ____logconfig.Error_Write(____logconfig.LogLevel.EXC, 0, ex);
                return "";
            }

        }

        public static string ValidateMobileNumbers(string mobileNo)
        {
            string result = Regex.Replace(mobileNo, @"[^\d]", ""); //replace special character
            result = long.Parse(result).ToString();// remove leading 0's
            result = (result.StartsWith("91") && result.Length > 10) ? result.Substring(2, result.Length-2) : result;// This will have 10 digit mobile number
            result = (result.StartsWith("6") || result.StartsWith("7") || result.StartsWith("8") || result.StartsWith("9")) ? result : "";
            if (result == "" || result.Substring(1, result.Length-1).Distinct().Count() == 1) // check remaining character sare same
                return "";
            return result;
        }
    }
}
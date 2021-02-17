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
using System.Text;
using Web.classes;
using Newtonsoft.Json;

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

        public static bool isTemplateMatched(string SenderId,string Message) {
            List<string> getDLTresponse = new List<string>();
            getDLTresponse = DltResponse(new string[] { SenderId }, new string[] { Message });
            return getDLTresponse.Count > 0;
        }

        public static List<string> DltResponse(string[] senderIds, string[] messages)
        {
            StringBuilder sb_dlt = new StringBuilder();
            for (int i = 0; i < senderIds.Length; i++)
            {
                sb_dlt.Append("{\"message\":\"" + messages[i] + "\",\"senderId\":\"" + senderIds[i] + "\"},");
            }
            return GetDLTResponse(sb_dlt);
        }
        public static List<string> GetDLTResponse(StringBuilder message)
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["template_URL"].ToString());
                webRequest.ContentType = "application/json;charset=utf-8";
                webRequest.Headers.Add("Authorization", ConfigurationManager.AppSettings["dlt_auth_token"].ToString());
                webRequest.Method = "POST";
                webRequest.KeepAlive = true;
                webRequest.Timeout = 5000000;
                var xml = "{\"data\":[" + message.ToString().TrimEnd(',') + "]}";
                ____logconfig.Log_Write(____logconfig.LogLevel.INFO, 0, "Request =>" + xml, "DLT Template Match");
                StreamWriter stOut = new StreamWriter(webRequest.GetRequestStream(), System.Text.Encoding.UTF8);
                stOut.Write(xml);
                stOut.Close();
                StreamReader stIn = new StreamReader(webRequest.GetResponse().GetResponseStream());
                string strResponse = stIn.ReadToEnd();
                stIn.Close();
                ParseTemplateResponse(strResponse, out List<string> templateIds);
                return templateIds;
            }
            catch (Exception ex)
            {
                ____logconfig.Error_Write(____logconfig.LogLevel.EXC, 0, ex, "DLT Template Error");
                return new List<string>();
            }
        }
        public static void ParseTemplateResponse(string resp, out List<string> templateIds)
        {
            ____logconfig.Log_Write(____logconfig.LogLevel.INFO, 0, "Response =>" + resp, "DLT Template Match");
            templateIds = new List<string>();
            var parse = JsonConvert.DeserializeObject<DltTemplateResponse>(resp);
            if (parse.code == "2000")
            {
                var dataObject = JsonConvert.DeserializeObject<DltTemplateResponseResult>(JsonConvert.SerializeObject(parse.data));
                if (dataObject.message.Contains("complete"))
                {

                    foreach (var item in dataObject.result)
                    {
                        string[] data = item.ToString().Split(':');
                        var Id = data[1].Replace("}", "").Replace("\"", "").Trim();
                        templateIds.Add(Id);
                    }
                    ____logconfig.Log_Write(____logconfig.LogLevel.INFO, 0, "Response parse =>" + string.Join(",", templateIds), "DLT Template Match");
                }
                else
                {
                    ____logconfig.Log_Write(____logconfig.LogLevel.INFO, 0, "Response parse => Failed in not verified", "DLT Template Match");
                }
            }
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

 

        public static bool ContainsUnicodeCharacter(string input)
        {
            const int MaxAnsiCode = 255;

            return input.Any(c => c > MaxAnsiCode);
        }
    }
}
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
using System.Data;

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
        public static Tuple<bool, long, string> CheckSenderIdApproved(int checkSenderId, string username, string senderId, int getDefault)
        {
            if (checkSenderId == 0) return new Tuple<bool, long, string>(true, 0, senderId);
            var regexItem = new Regex("^[a-zA-Z0-9 ]*$");
            if (regexItem.IsMatch(senderId))
            {
                if (senderId.Length != 6) return new Tuple<bool, long, string>(false, 0, senderId);
            }
            else
            {
                return new Tuple<bool, long, string>(false, 0, senderId);
            }
            var sql = "";
            string[] arr_values;
            if (getDefault == 1)
            {
                sql = $@"SELECT d.* FROM displayname d JOIN user_master um 
                         ON d.username = um.username WHERE um.username = ?global.username 
                         AND Approved = 1 AND isDefault=1;";
                arr_values = new string[] { username };

            }
            else
            {
                sql = $@"SELECT d.* FROM displayname d JOIN user_master um 
                         ON d.username = um.username WHERE um.username = ?global.username 
                        AND DisplayName = ?dispnm.displayname  AND Approved = 1; ";
                arr_values = new string[] { username, senderId };
            }

            DataSet ds = DL.DL_ExecuteQuery(sql, arr_values);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                return new Tuple<bool, long, string>(true, long.Parse(ds.Tables[0].Rows[0]["PE_Id"].ToString()), ds.Tables[0].Rows[0]["DisplayName"].ToString());
            }
            return new Tuple<bool, long, string>(true, 0, senderId);
        }

        public static bool isTemplateMatched(string SenderId, string Message,string peId,string ref_key)
        {
            List<string> getDLTresponse = new List<string>();
            getDLTresponse = DltResponse(new string[] { SenderId }, new string[] { Message },new string[] { peId},new string[] { ref_key});
            return getDLTresponse.Count > 0;
        }

        public static bool checkPEId(int peIdSatus, string inputPEId, string approvedPEId, out long peId)
        {
            peId = 0;
            if (peIdSatus == 0) return true;

            if (peIdSatus == 1)
            {
                bool isParsed = long.TryParse(approvedPEId, out peId);
                if (isParsed && peId > 0) return true;
            }

            if (peIdSatus == 2)
            {
                bool isParsed = long.TryParse(inputPEId, out peId);
                if (isParsed && peId > 0) return true;
            }

            if (peIdSatus == 3)
            {
                bool isParsed = long.TryParse(inputPEId, out peId);
                if (isParsed && peId > 0) return true;
                bool isAppParsed = long.TryParse(approvedPEId, out peId);
                if (isAppParsed && peId > 0) return true;
            }

            return false;
        }
        public static bool checkTMId(int tmIdSatus, string inputTMId, out long tmId)
        {
            tmId = 0;
            if (tmIdSatus == 0 || tmIdSatus == 1) return true;
            if (tmIdSatus == 2)
            {
                bool isParsed = long.TryParse(inputTMId, out tmId);
                if (isParsed && tmId > 0) return true;
            }

            if (tmIdSatus == 3)
            {
                bool isParsed = long.TryParse(inputTMId, out tmId);
                if (isParsed && tmId > 0) return true;
                return true;
            }

            return false;
        }
        public static bool checkTemplateId(int tempIdSatus, string inputTempId, string approvedTempId, out long tempId)
        {

            tempId = 0;
            if (tempIdSatus == 0) return true;

            if (tempIdSatus == 1)
            {
                bool isParsed = long.TryParse(approvedTempId, out tempId);
                if (isParsed && tempId > 0) return true;
            }

            if (tempIdSatus == 2)
            {
                bool isParsed = long.TryParse(inputTempId, out tempId);
                if (isParsed && tempId > 0 && tempId.ToString().Length >= 10 && tempId.ToString().Length <= 20) return true;
            }

            if (tempIdSatus == 3)
            {
                bool isParsed = long.TryParse(inputTempId, out tempId);
                if (isParsed && tempId > 0 && tempId.ToString().Length >= 10 && tempId.ToString().Length <= 20) return true;
                bool isAppParsed = long.TryParse(approvedTempId, out tempId);
                if (isAppParsed && tempId > 0) return true;
            }

            return false;
        }
        public static List<string> DltResponse(string[] senderIds, string[] messages, string[] peId, string[] refid)
        {
            StringBuilder sb_dlt = new StringBuilder();
            for (int i = 0; i < senderIds.Length; i++)
            {
                sb_dlt.Append("{\"message\":\"" + HttpUtility.JavaScriptStringEncode(messages[i]) + "\",\"senderId\":\"" + senderIds[i] + "\",\"pe_id\":\"" + peId[i] + "\",\"ref_key\":\"" + refid[i] + "\"},");
            }
            return GetDLTResponse(sb_dlt);
        }


        public static List<string> GetDLTResponse(StringBuilder message)
        {
            var guid = Guid.NewGuid();
            var url = ConfigurationManager.AppSettings["templateURL"].ToString();
            var xml = "{\"data\":[" + message.ToString().TrimEnd(',') + "]}";
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["templateURL"].ToString());
                webRequest.ContentType = "application/json;charset=utf-8";
                webRequest.Headers.Add("Authorization", ConfigurationManager.AppSettings["dlt_auth_token"].ToString());
                webRequest.Method = "POST";
                webRequest.KeepAlive = true;
                webRequest.Timeout = 5000000;
                StreamWriter stOut = new StreamWriter(webRequest.GetRequestStream(), System.Text.Encoding.UTF8);
                stOut.Write(xml);
                stOut.Close();
                StreamReader stIn = new StreamReader(webRequest.GetResponse().GetResponseStream());
                string strResponse = stIn.ReadToEnd();
                stIn.Close();
                ____logconfig.Log_Write(____logconfig.LogLevel.INFO, 0, Environment.NewLine + guid.ToString() + "URL => " + url + Environment.NewLine + "Request =>" + xml + Environment.NewLine + " Response =>" + strResponse, "DLT Template Match Request Response");
                ParseTemplateResponse(strResponse, guid.ToString(), out List<string> templateIds);
                return templateIds;
            }
            catch (Exception ex)
            {
                ____logconfig.Log_Write(____logconfig.LogLevel.INFO, 0, Environment.NewLine + guid.ToString() + "URL => " + url + Environment.NewLine + "Request =>" + xml + Environment.NewLine + " Response =>" + ex.ToString(), "DLT Template Match Request Error");
                return new List<string>();
            }
        }
        public static void ParseTemplateResponse(string resp, string guid, out List<string> templateIds)
        {
            ____logconfig.Log_Write(____logconfig.LogLevel.INFO, 0, guid + " Response =>" + resp, "DLT Template Parse");
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
                    ____logconfig.Log_Write(____logconfig.LogLevel.INFO, 0, guid + " Response parse =>" + string.Join(",", templateIds), "DLT Template Parse");
                }
                else
                {
                    ____logconfig.Log_Write(____logconfig.LogLevel.INFO, 0, guid + " Response parse => Failed in not verified", "DLT Template Parse");
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
            result = (result.StartsWith("91") && result.Length > 10) ? result.Substring(2, result.Length - 2) : result;// This will have 10 digit mobile number
            result = (result.StartsWith("6") || result.StartsWith("7") || result.StartsWith("8") || result.StartsWith("9")) ? result : "";
            if (result == "" || result.Substring(1, result.Length - 1).Distinct().Count() == 1) // check remaining character sare same
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